import {api,can,identity} from './server-session.mjs';
export const canGlobalAdmin=permission=>can(permission)&&identity.scope==='all'&&identity.grants.some(grant=>grant.scope==='all'&&grant.permissions.includes(permission));
export const canAdminWrite=()=>['users.manage','roles.manage','permissions.manage','audit.read'].every(canGlobalAdmin);
export class ServerAdmin {
 async load(options={}){
  const generation=(this.generation||0)+1;this.generation=generation;
  this.options={route:'overview',page:1,search:'',departmentId:'',status:'',roleId:'',...this.options,...options};
  const {route,page,search,departmentId,status,roleId}=this.options;
  const userRoute=['overview','users','permissions','simulator','departments'].includes(route);
  const q=new URLSearchParams({page:String(page),pageSize:['overview','departments'].includes(route)?'1':'20',includeSummary:'true',...(search?{search}:{}),...(departmentId?{departmentId}:{}),...(status?{status}:{}),...(roleId?{roleId}:{})});
  const [userPage,configuredRoles,departments,auditPage,permissions]=await Promise.all([can('users.manage')&&userRoute?api.request('/users?'+q):{items:[],total:0},canGlobalAdmin('roles.manage')?api.request('/roles'):[],api.departments(),canGlobalAdmin('audit.read')&&['overview','history'].includes(route)?api.request('/audit-events?'+new URLSearchParams({page:String(route==='overview'?1:page),pageSize:route==='overview'?'4':'20'})):{items:[],total:0},canGlobalAdmin('permissions.manage')?api.request('/permissions'):[]]);
  const users=userPage.items,audit=auditPage.items;
  if(generation!==this.generation)return null;
  if(userPage.summary)this.summary=userPage.summary;
  const roles=configuredRoles.length?configuredRoles:[...new Map(users.flatMap(user=>user.grants.map(grant=>[grant.roleId,{id:grant.roleId,name:grant.roleId,scope:grant.scope,permissions:grant.permissions}]))).values()];
  this.departments=departments;this.rawUsers=users;this.rawRoles=configuredRoles;
  const dep=id=>departments.find(d=>d.id===id)?.name||id;
  return {version:1,revision:0,server:true,departments,permissions,summary:this.summary,userTotal:userPage.total,auditTotal:auditPage.total,page,pageSize:20,roles:roles.map(r=>({...r,api:r,description:''})),users:users.map(u=>this.mapUser(u,roles)),audit:audit.reverse().map(a=>({...a,id:String(a.id),actor:a.actorId,target:a.targetId,summary:a.action,before:null,after:a.detailJson?JSON.parse(a.detailJson):null}))};
 }
 mapUser(u,roles=this.rawRoles){const dep=id=>this.departments.find(d=>d.id===id)?.name||id;return {...u,api:u,name:u.displayName,username:u.userName,unit:dep(u.departmentId),units:u.departmentIds.map(dep),status:u.active?'active':'locked',roleOverrides:Object.fromEntries(u.grants.filter(g=>{const role=roles.find(r=>r.id===g.roleId);if(u.overrideRoleIds)return u.overrideRoleIds.includes(g.roleId);return role&&(role.scope!==g.scope||JSON.stringify([...role.permissions].sort())!==JSON.stringify([...g.permissions].sort()));}).map(g=>[g.roleId,{scope:g.scope,permissions:g.permissions}]))};}
 async roleAccounts(roleId,page=1){const result=await api.request('/users?'+new URLSearchParams({roleId,page:String(page),pageSize:'20'}));this.rawUsers=result.items;return {...result,items:result.items.map(u=>this.mapUser(u))};}
 department(name){const d=this.departments.find(d=>d.name===name);if(!d)throw Error('Đơn vị không hợp lệ.');return d.id;}
 async save(action,p,password){
  if(!canAdminWrite()||(action==='department'&&!canGlobalAdmin('departments.manage')))throw Error('Thao tác quản trị cần đủ quyền cốt lõi toàn trường theo chính sách máy chủ. Bạn vẫn có thể xem các danh mục đã được cấp quyền.');
  const u=this.rawUsers.find(u=>u.id===p.id),r=this.rawRoles.find(r=>r.id===p.id),dep=this.departments.find(d=>d.id===p.id);
  if(action==='user')return api.request(u?`/users/${u.id}`:'/users',{method:u?'PUT':'POST',data:{userName:p.username,email:p.email,displayName:p.name,departmentId:this.department(p.unit),roleIds:p.roleIds,version:u?.version,...(!u?{password}:{})}});
  if(action==='status')return api.request(`/users/${u.id}/status`,{method:'POST',data:{active:p.status==='active',version:u.version,reason:p.reason}});
  if(action==='scope')return api.request(`/users/${u.id}/scope`,{method:'PUT',data:{scope:p.scope,departmentIds:p.units.map(n=>this.department(n)),denied:p.denied,version:u.version}});
  if(action==='role-account'){const target=this.rawUsers.find(u=>u.id===p.userId);return api.request(`/users/${target.id}/role-overrides`,{method:'PUT',data:{roleId:p.id,inherit:p.inherit==='on',scope:p.scope||'self',permissions:p.permissions,version:target.version}});}
  if(action==='role')return api.request(r?`/roles/${r.id}`:'/roles',{method:r?'PUT':'POST',data:{name:p.name,scope:p.scope,permissions:p.permissions,version:r?.version}});
  if(action==='department')return api.request(dep?`/departments/${dep.id}`:'/departments',{method:dep?'PUT':'POST',data:{code:p.code,name:p.name,notes:p.notes,active:p.active==='true',version:dep?.version}});
  throw Error('Chức năng này chưa có API. Hồ sơ không bị xóa.');
 }
}
