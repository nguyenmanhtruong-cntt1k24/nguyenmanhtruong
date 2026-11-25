import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import vm from 'node:vm';
const source=fs.readFileSync(new URL('./server-session.mjs',import.meta.url),'utf8').replace(/^import.*\r?\n/,'').replaceAll('export ','');
function session(profile,request=async()=>({items:[],total:0})){
 const context=vm.createContext({HpuniApi:class{request=request;me=async()=>profile;},URLSearchParams,location:{search:''},crypto});
 vm.runInContext(source,context);vm.runInContext('identity='+JSON.stringify(profile),context);
 return vm.runInContext('({all,allowsDepartment,departmentsFor,requireSession})',context);
}
const user={id:'u',active:true,scope:'units',departmentId:'a',departmentIds:['a','b'],denied:[],grants:[{scope:'unit',permissions:['budget.read']},{scope:'all',permissions:['reports.read']}]};
test('unit choices intersect account scope with the same permission grant; self is not unit',()=>{
 const s=session(user);
 assert.equal(s.allowsDepartment(user,'budget.read','a'),true);
 assert.equal(s.allowsDepartment(user,'budget.read','b'),false);
 assert.equal(s.allowsDepartment(user,'reports.read','b'),true);
 assert.equal(s.allowsDepartment({...user,denied:['budget.read']},'budget.read','a'),false);
 assert.equal(s.allowsDepartment({...user,scope:'self'},'budget.read','a'),false);
 assert.deepEqual([...s.departmentsFor('budget.read',[{id:'a'},{id:'b'},{id:'c'}])].map(x=>x.id),['a']);
});
test('snapshot paging passes 5000 without truncation and detects an empty page before total',async()=>{
 let calls=0;const s=session(user,async()=>{const offset=calls++*100;return{items:Array.from({length:Math.min(100,5101-offset)},(_,i)=>({id:offset+i})),total:5101};});
 const rows=await s.all('/items?status=active');assert.equal(rows.length,5101);assert.equal(rows.at(-1).id,5100);assert.equal(calls,52);
 await assert.rejects(session(user,async()=>({items:[],total:1})).all('/items'),/Danh sách đã thay đổi/);
});
test('workspace guard accepts any explicitly allowed permission without granting others',async()=>{
 const profile={...user,grants:[{scope:'all',permissions:['inventory.manage']}]};
 assert.equal((await session(profile).requireSession(['resources.manage','inventory.manage'])).id,'u');
});
