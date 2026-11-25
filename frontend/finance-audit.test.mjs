import test from 'node:test';
import assert from 'node:assert/strict';
import fs from 'node:fs';
import vm from 'node:vm';

function harness(route='queue',canWrite=false) {
 const source=fs.readFileSync(new URL('./server-finance.mjs',import.meta.url),'utf8')
  .replace(/^import .*\r?\n/gm,'').replace("await requireSession('budget.read');",'')
  .split("document.addEventListener('click',async event=>")[0];
 const elements=new Map(),calls=[];
 const element=s=>{if(!elements.has(s))elements.set(s,{innerHTML:'',textContent:'',showModal(){}});return elements.get(s);};
 const api={departments:async()=>[],request:async path=>{calls.push(path);if(path.startsWith('/finance/payments/'))return{id:'payment',maintenanceId:'maintenance'};if(path.startsWith('/finance/summary'))return{balances:[],received:0};return{items:[],total:0};}};
 const context={api,URLSearchParams,Date,day:()=> '2026-11-10',location:{hash:'#/'+route},can:()=>false,canForDepartment:()=>canWrite,departmentsFor:()=>[],document:{querySelector:element}};
 const functions=vm.runInNewContext(source+`;({entryForm,fundingForm,voucher,render,load,setYear:v=>year=v,setCategory:v=>category=v,setProposals:v=>proposals=v})`,context);
 return{...functions,elements,calls};
}
test('payment form uses request year and maintenance budget even under another category filter',()=>{
 const h=harness();h.setCategory('equipment');
 h.entryForm('expense',{year:2025,remaining:100,departmentId:'it',content:'Bảo trì',maintenanceId:'m'});
 const html=h.elements.get('#dialog-body').innerHTML;
 assert.match(html,/name="date"[^>]*value="2025-01-01"[^>]*min="2025-01-01" max="2025-12-31"/);
 assert.match(html,/<option value="maintenance" selected>/);
});
test('manual entry date follows financial year while current year keeps today',()=>{
 const h=harness();h.setYear(2025);h.entryForm('receipt');
 assert.match(h.elements.get('#dialog-body').innerHTML,/name="date"[^>]*value="2025-01-01"/);
 h.setYear(2026);h.entryForm('receipt');
 assert.match(h.elements.get('#dialog-body').innerHTML,/name="date"[^>]*value="2026-11-10"/);
});
test('voucher opened from transactions fetches linked payment outside queue page',async()=>{
 const h=harness('transactions');
 await h.voucher({voucher:'PC1',kind:'expense',category:'maintenance',amount:100,paymentRequestId:'payment'});
 assert.deepEqual(h.calls,['/finance/payments/payment']);
 assert.match(h.elements.get('#dialog-body').innerHTML,/Hồ sơ bảo trì<\/dt><dd>maintenance/);
});
test('queue does not offer or send category filter for requests without categories',async()=>{
 const h=harness();h.setCategory('equipment');await h.load();h.render();
 assert.doesNotMatch(h.elements.get('#main').innerHTML,/name="category"/);
 assert.ok(h.calls.every(path=>!path.includes('category=')));
 const other=harness('estimates');other.render();assert.match(other.elements.get('#main').innerHTML,/name="category"/);
});
test('approved proposals offer receipt confirmation only until effective receipt exists',()=>{
 const h=harness('estimates',true);h.setYear(2026);
 const p={id:'p',year:2026,departmentId:'it',amount:100,status:'approved',title:'Kinh phí'};
 h.setProposals([p]);h.render();assert.match(h.elements.get('#main').innerHTML,/data-finance="receive-funding"/);
 h.setProposals([{...p,fundingReceiptId:'entry'}]);h.render();
 assert.doesNotMatch(h.elements.get('#main').innerHTML,/data-finance="receive-funding"/);
 assert.match(h.elements.get('#main').innerHTML,/data-finance="funding-voucher" data-id="entry"/);
 const reader=harness('estimates');reader.setProposals([p]);reader.render();
 assert.doesNotMatch(reader.elements.get('#main').innerHTML,/data-finance="receive-funding"/);
 h.fundingForm({...p,year:2025});const html=h.elements.get('#dialog-body').innerHTML;
 assert.match(html,/data-kind="funding"/);assert.match(html,/name="date"[^>]*value="2025-01-01"/);
 assert.doesNotMatch(html,/name="amount"/);
});
