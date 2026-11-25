import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import vm from 'node:vm';

const controls=fs.readFileSync(new URL('./finance-controls.mjs',import.meta.url),'utf8').replace(/^import.*\r?\n/,'').replace('export async function','async function');
function harness(permissions=[]){
 const calls=[],departments=[{id:'outside',name:'Outside',active:true},{id:'inside',name:'Inside',active:true}];
 const elements=new Map();
 const container={isConnected:true,innerHTML:'',querySelector(selector){if(!elements.has(selector))elements.set(selector,{dataset:{},textContent:'',innerHTML:''});return elements.get(selector);}};
 const api={departments:async()=>departments,request:async path=>{calls.push(path);return path.startsWith('/finance/adjustments')?{items:[],total:21}:{departmentId:'inside',closed:false,year:2026,version:'v'};}};
 const run=vm.runInNewContext(controls+';mountFinanceControls',{api,identity:{id:'u'},keyed:async()=>{},URLSearchParams,FormData,departmentsFor:(permission,rows)=>permission==='budget.read'||permissions.includes(permission)?rows.filter(d=>d.id==='inside'):[]});
 return{run,calls,container,elements};
}

test('period actions belong to accounting permission, not budget approval',async()=>{
 const executive=harness(['budget.approve']);await executive.run(executive.container,{year:2026});
 assert.doesNotMatch(executive.elements.get('[data-period]').innerHTML,/data-period-change/);
 const accountant=harness(['transactions.write','budget.write']);await accountant.run(accountant.container,{year:2026});
 assert.match(accountant.elements.get('[data-period]').innerHTML,/data-period-change/);
 assert.match(accountant.container.innerHTML,/data-adjustment-create/);
 assert.doesNotMatch(accountant.container.innerHTML,/<form data-adjustment /);
});
test('finance controls page on server and select a readable default without borrowing write scope',async()=>{
 const {run,calls,container}=harness();
 await run(container,{year:2026,category:'equipment',page:2});
 const query=new URL('http://localhost'+calls[0]).searchParams;
 assert.equal(query.get('year'),'2026');assert.equal(query.get('category'),'equipment');assert.equal(query.get('page'),'2');assert.equal(query.get('pageSize'),'20');
 assert.match(calls[1],/departmentId=inside/);assert.doesNotMatch(calls[1],/outside/);
 assert.doesNotMatch(container.innerHTML,/Outside|data-adjustment class|data-period-change/);
 assert.match(container.innerHTML,/21 đề nghị/);
 assert.equal(typeof container.onsubmit,'function');assert.equal(typeof container.onclick,'function');
});
test('remount replaces finance handlers and stale controls never overwrite a new mount',async()=>{
 const {run,container}=harness();
 await run(container,{year:2026});const old=container.onsubmit;
 await run(container,{year:2027});assert.notEqual(old,container.onsubmit);assert.match(container.innerHTML,/2027/);
 let resolve;
 const pending=new Promise(r=>resolve=r),calls=[];
 const api={departments:async()=>[],request:async path=>{calls.push(path);if(path.includes('year=2028'))return pending;return{items:[],total:0};}};
 const mount=vm.runInNewContext(controls+';mountFinanceControls',{api,identity:{id:'u'},keyed:async()=>{},URLSearchParams,FormData,departmentsFor:()=>[]});
 const first=mount(container,{year:2028});await Promise.resolve();await mount(container,{year:2029});resolve({items:[],total:0});await first;
 assert.match(container.innerHTML,/2029/);assert.doesNotMatch(container.innerHTML,/2028/);
});
test('voucher reads only linked payment and entry reversal instead of complete histories',async()=>{
 const source=fs.readFileSync(new URL('./server-voucher.mjs',import.meta.url),'utf8').replace(/^import.*\r?\n/gm,'');
 const calls=[],article={prepend(){}},main={innerHTML:'',querySelector:()=>article,append(){}};
 const api={departments:async()=>[{id:'d',name:'Đơn vị'}],request:async path=>{calls.push(path);if(path.startsWith('/finance/entries/'))return{id:'entry',departmentId:'d',paymentRequestId:'payment',category:'maintenance',kind:'expense',amount:10,year:2026};if(path.startsWith('/finance/payments/'))return{id:'payment',content:'Sửa thiết bị'};return{items:[]};}};
 const context={api,requireSession:async()=>{},mountVoucherFiles:async()=>{},location:{search:'?id=entry'},URLSearchParams,window:{print(){}},document:{querySelector:selector=>selector==='#voucher'?main:{},createElement:()=>({style:{}}),head:{append(){}}}};
 await vm.runInNewContext('(async()=>{'+source+'})()',context);
 assert.deepEqual(calls,['/finance/entries/entry','/finance/payments/payment','/finance/reversals?entryIds=entry&page=1&pageSize=1']);
 assert.match(main.innerHTML,/Bảo trì/);assert.match(main.innerHTML,/Sửa thiết bị/);assert.doesNotMatch(main.innerHTML,/paymentRequestId=/);
});
