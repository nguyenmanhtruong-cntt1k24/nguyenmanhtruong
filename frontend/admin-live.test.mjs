import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import vm from 'node:vm';
import {PERMISSIONS,adminSeed,adminAction,accessCheck,adminUnits} from './admin-data.mjs';

const login=fs.readFileSync(new URL('./server-login.mjs',import.meta.url),'utf8');
const source=login.slice(login.indexOf('function availableWorkspaces'),login.indexOf("document.querySelector('.demo-accounts')"));
const workspaces=vm.runInNewContext('('+source.trim()+')');
const profile=permissions=>({active:true,scope:'all',roleIds:['custom-role'],denied:[],grants:[{scope:'all',permissions}]});
test('custom roles route by effective permissions, with denied and inactive protection',()=>{
 assert.deepEqual([...workspaces(profile(['budget.read']))],['accounting']);
 assert.deepEqual([...workspaces(profile(['budget.read','budget.approve']))],['executive']);
 assert.deepEqual([...workspaces(profile(['resources.manage']))],['resources']);
 for(const permission of ['requests.review','inventory.manage','logs.review'])assert.deepEqual([...workspaces(profile([permission]))],['resources']);
 for(const permission of ['roles.manage','permissions.manage','departments.manage','audit.read'])assert.deepEqual([...workspaces(profile([permission]))],['admin']);
 assert.deepEqual([...workspaces({...profile(['audit.read']),scope:'unit'})],[]);
 assert.deepEqual([...workspaces(profile(['reports.export']))],['reports']);
 assert.deepEqual([...workspaces(profile(['reports.read']))],['reports']);
 assert.deepEqual([...workspaces({...profile(['users.manage']),denied:['users.manage']})],[]);
 assert.deepEqual([...workspaces({...profile(['budget.read']),active:false})],[]);
});
const adminSource=fs.readFileSync(new URL('./server-admin.mjs',import.meta.url),'utf8').replace(/^import.*\r?\n/,'').replaceAll('export ','');
function bridgeFor(permissions=[]){
 const requests=[],identity={...profile(permissions),scope:'all'};
 const api={request:async(path,options)=>{requests.push({path,...options});return path.startsWith('/users?')?{items:[],total:45,summary:{total:123,active:100,roles:[],departments:[]}}:path.startsWith('/audit-events?')?{items:[],total:51}:[];},departments:async()=>[{id:'d',name:'Dept',active:true,version:'v'}]};
 const Admin=vm.runInNewContext(adminSource+';ServerAdmin',{api,URLSearchParams,identity,can:p=>permissions.includes(p)});
 const bridge=new Admin();bridge.rawUsers=[{id:'u',version:'v',denied:['reports.export']}];bridge.rawRoles=[{id:'r',version:'v',permissions:['reports.export']}];bridge.departments=[{id:'d',name:'Dept',active:true,version:'v'}];
 return {bridge,requests};
}
test('export permission can be removed, denied, or overridden independently',async()=>{
 const {bridge,requests}=bridgeFor(['users.manage','roles.manage','permissions.manage','audit.read']);
 await bridge.save('scope',{id:'u',scope:'all',units:[],denied:[]});assert.deepEqual([...requests.at(-1).data.denied],[]);
 await bridge.save('scope',{id:'u',scope:'all',units:[],denied:['reports.export']});assert.deepEqual([...requests.at(-1).data.denied],['reports.export']);
 await bridge.save('role',{id:'r',name:'Custom',scope:'all',permissions:['reports.read']});assert.deepEqual([...requests.at(-1).data.permissions],['reports.read']);
 await bridge.save('role-account',{id:'r',userId:'u',scope:'all',permissions:['reports.export']});assert.deepEqual([...requests.at(-1).data.permissions],['reports.export']);
 assert(PERMISSIONS.some(([id])=>id==='reports.export'));
});
test('export-only override grants export without report read and account denial wins',()=>{
 let state=adminSeed();
 state=adminAction(state,'role-account',{id:'lecturer',userId:'USR-002',scope:'self',permissions:['reports.export']});
 const query={userId:'USR-002',unit:'Khoa CNTT',ownerId:'USR-002'};
 assert.equal(accessCheck(state,{...query,permission:'reports.export'}).allowed,true);
 assert.equal(accessCheck(state,{...query,permission:'reports.read'}).allowed,false);
 state=adminAction(state,'scope',{id:'USR-002',scope:'self',units:[],denied:['reports.export']});
 assert.equal(accessCheck(state,{...query,permission:'reports.export'}).allowed,false);
 state=adminAction(state,'scope',{id:'USR-002',scope:'self',units:[],denied:[]});
 assert.equal(accessCheck(state,{...query,permission:'reports.export'}).allowed,true);
});
test('narrow user manager does not call protected roles or audit endpoints',async()=>{
 const {bridge,requests}=bridgeFor(['users.manage']);const result=await bridge.load();
 assert.equal(requests.some(r=>r.path==='/roles'||r.path==='/audit-events'),false);
 assert.equal(result.departments.length,1);
 assert.deepEqual([...adminUnits(result)],['Dept']);
});
test('department active state is persisted without deletion',async()=>{
 const {bridge,requests}=bridgeFor(['users.manage','roles.manage','permissions.manage','audit.read','departments.manage']);
 await bridge.save('department',{id:'d',code:'D',name:'Dept',notes:'',active:'false'});
 assert.equal(requests.at(-1).data.active,false);assert.equal(requests.at(-1).method,'PUT');
 await bridge.save('department',{id:'d',code:'D',name:'Dept',notes:'',active:'true'});
 assert.equal(requests.at(-1).data.active,true);
});
test('single admin readers only fetch their permitted directory without user access',async()=>{
 for(const [permission,path] of [['roles.manage','/roles'],['permissions.manage','/permissions'],['audit.read','/audit-events'],['departments.manage',null]]){
  const {bridge,requests}=bridgeFor([permission]);const result=await bridge.load();
  assert.equal(requests.some(r=>r.path.startsWith('/users?')),false);
  assert.deepEqual(requests.map(r=>r.path.split('?')[0]),path?[path]:[]);
  assert.equal(result.users.length,0);
 }
});
test('admin routes request only one server page with SQL filters and independent totals',async()=>{
 const {bridge,requests}=bridgeFor(['users.manage','audit.read']);
 let result=await bridge.load({route:'users',page:2,search:'Nguyen',departmentId:'d',status:'active',roleId:'r'});
 const users=new URL(requests.find(r=>r.path.startsWith('/users?')).path,'http://local');
 assert.equal(users.searchParams.get('page'),'2');assert.equal(users.searchParams.get('pageSize'),'20');
 for(const [key,value] of Object.entries({search:'Nguyen',departmentId:'d',status:'active',roleId:'r'}))assert.equal(users.searchParams.get(key),value);
 assert.equal(result.userTotal,45);assert.equal(result.summary.total,123);
 assert.equal(requests.some(r=>r.path.startsWith('/audit-events?')),false);
 requests.length=0;result=await bridge.load({route:'history',page:3});
 assert.equal(requests.some(r=>r.path.startsWith('/users?')),false);
 const audit=new URL(requests.find(r=>r.path.startsWith('/audit-events?')).path,'http://local');
 assert.equal(audit.searchParams.get('page'),'3');assert.equal(audit.searchParams.get('pageSize'),'20');assert.equal(result.auditTotal,51);
});
test('role override account selector loads a bounded page filtered by role',async()=>{
 const {bridge,requests}=bridgeFor(['users.manage']);const result=await bridge.roleAccounts('custom-role',3);
 const q=new URL(requests.at(-1).path,'http://local').searchParams;
 assert.equal(q.get('roleId'),'custom-role');assert.equal(q.get('page'),'3');assert.equal(q.get('pageSize'),'20');assert.equal(result.total,45);
});
test('read permissions do not enable administration writes',async()=>{
 const {bridge,requests}=bridgeFor(['departments.manage']);
 await assert.rejects(()=>bridge.save('department',{id:'d',active:'false'}));
 assert.equal(requests.length,0);
});
