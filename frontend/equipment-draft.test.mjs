import {test} from 'node:test';
import assert from 'node:assert/strict';
globalThis.location={search:''};
const calls=[];
let stale=false;
globalThis.fetch=async(url,options={})=>{
  calls.push({url,options});
  const result=(body,status=200)=>new Response(JSON.stringify(body),{status,headers:{'Content-Type':'application/json'}});
  if(url.endsWith('/auth/csrf'))return result({token:'fixture-csrf'});
  if(options.method==='PUT')return stale?result({title:'Dữ liệu đã thay đổi',code:'STALE_VERSION'},409):result({id:'draft-a',version:'version-after-save',status:'draft'});
  if(url.endsWith('/submit'))return result({id:'draft-a',status:'pending',version:'version-after-submit'});
  throw Error('Unexpected request '+url);
};
const {ServerResources}=await import('./server-resources.mjs');
const originalFormData=globalThis.FormData;
const form=()=>({dataset:{kind:'device',resource:'asset-a',existing:'draft-a'},fields:{date:'2026-12-10',start:'08:00',end:'10:00',count:'2',purpose:'Mượn phục vụ giảng dạy',commitment:'on'}});
const bridge=()=>{
  const instance=new ServerResources(false);
  instance.snapshot={requests:[{id:'draft-a',kind:'device',api:{version:'version-before-save'},resource:'asset-a'}],logs:[],catalog:{rooms:[],devices:[]}};
  return instance;
};

test('sửa nháp thiết bị giữ nguyên ID, gửi PUT/version và dùng version mới khi gửi duyệt',async()=>{
  globalThis.FormData=class extends Map{constructor(f){super(Object.entries(f.fields));}};
  try{
    calls.length=0;
    await bridge().book(form(),'pending');
    const writes=calls.filter(c=>c.options.method);
    assert.deepEqual(writes.map(c=>[c.url,c.options.method]),[
      ['/api/v1/equipment-loans/draft-a','PUT'],
      ['/api/v1/equipment-loans/draft-a/submit','POST']
    ]);
    const saved=JSON.parse(writes[0].options.body);
    assert.equal(saved.version,'version-before-save');assert.equal(saved.assetGroupId,'asset-a');assert.equal(saved.quantity,2);
    assert.equal(JSON.parse(writes[1].options.body).version,'version-after-save');
    assert.ok(writes.every(c=>c.options.headers['X-CSRF-TOKEN']==='fixture-csrf'));
  }finally{globalThis.FormData=originalFormData;}
});

test('nháp cũ trả 409 thì giữ nguyên biểu mẫu và không tự tạo/hủy/gửi lại phiếu',async()=>{
  globalThis.FormData=class extends Map{constructor(f){super(Object.entries(f.fields));}};
  try{
    calls.length=0;stale=true;
    const f=form();
    await assert.rejects(()=>bridge().book(f,'pending'),e=>e.status===409&&e.code==='STALE_VERSION');
    assert.equal(calls.filter(c=>c.options.method).length,1);
    assert.equal(f.dataset.existing,'draft-a');assert.equal(f.fields.count,'2');
  }finally{stale=false;globalThis.FormData=originalFormData;}
});
