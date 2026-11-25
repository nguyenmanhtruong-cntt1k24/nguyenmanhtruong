import {test} from 'node:test';
import assert from 'node:assert/strict';
globalThis.location={search:''};
const calls=[];let fail=false;
globalThis.fetch=async(url,options={})=>{
 calls.push({url,options});
 const json=(v,status=200)=>new Response(JSON.stringify(v),{status,headers:{'Content-Type':'application/json'}});
 if(url.endsWith('/auth/me'))return json({id:'user-1',active:true,departmentId:'dept',scope:'self',denied:[],grants:[{roleId:'lecturer',scope:'self',permissions:['rooms.book','devices.borrow','logs.write']}]});
 if(url.endsWith('/auth/csrf'))return json({token:'test-token'});
 if(url.endsWith('/departments'))return json([{id:'dept',name:'Khoa thử nghiệm',active:true}]);
 if(url.includes('/availability'))return json({slots:[{start:'2026-11-08T01:00:00Z',end:'2026-11-08T03:00:00Z'}]});
 if(url.includes('/submit')){if(fail){fail=false;throw Error('Network interrupted');}return json({status:'pending'});}
 if(url.includes('/room-requests?'))return json({items:[],total:0});
 if(url.includes('/rooms?'))return json({items:[{id:'room-guid',code:'A101',name:'Phòng thật',departmentId:'dept',capacity:30,active:true,type:'Phòng học',version:'AAAB'}],total:1});
 if(url.includes('/equipment-loans?'))return json({items:[{id:'loan-guid',assetGroupId:'asset-guid',assetName:'Thiết bị thật',ownerId:'user-1',departmentId:'dept',start:'2026-11-08T01:00:00Z',end:'2026-11-08T03:00:00Z',quantity:2,returned:0,status:'draft',version:'AAAC',returns:[]}],total:1});
 if(url.includes('/equipment?'))return json({items:[{id:'asset-guid',name:'Thiết bị thật',code:'TB1',departmentId:'dept',total:4,damaged:1,lost:0,active:true,version:'AAAD'}],total:1});
 if(url.endsWith('/log')&&['POST','PUT'].includes(options.method))return json({status:'submitted'});
 return json({},404);
};
const {requireSession,api}=await import('./server-session.mjs');
const {ServerResources,logPayload,personalRequestsCsv,mapBounded}=await import('./server-resources.mjs');
await requireSession();

test('ánh xạ giữ ID và phiên bản máy chủ, đổi ngày giờ Việt Nam, không dùng tên người mẫu',async()=>{
 const bridge=new ServerResources();const d=await bridge.load();
 assert.equal(d.requests[0].id,'loan-guid');assert.equal(d.requests[0].api.version,'AAAC');
 assert.equal(d.requests[0].start,'08:00');assert.equal(d.requests[0].date,'2026-11-08');
 assert.equal(d.requests[0].ownerName,'user-1 · Khoa thử nghiệm');
 assert.equal(d.catalog.devices[0].available,3);assert.equal(d.assets[0].quantity,4);
 assert.ok(calls.some(c=>c.url.includes('review=false')));
});
test('giờ bận được suy từ slot công khai, không giả danh phiếu của người khác',async()=>{
 const bridge=new ServerResources();await bridge.load();await bridge.availability('2026-11-08');
 assert.deepEqual(bridge.snapshot.roomReservations.map(r=>[r.start,r.end]),[['07:00','08:00'],['10:00','18:00']]);
 assert.ok(bridge.snapshot.roomReservations.every(r=>r.resource==='room-guid'));
});
test('thử lại sau mất mạng giữ khóa và version, không tự gửi lặp',async()=>{
 const bridge=new ServerResources();await bridge.load();fail=true;
 const before=calls.filter(c=>c.url.includes('/submit')).length;
 await assert.rejects(()=>bridge.action('loan-guid','submit'),/Network interrupted/);
 assert.equal(calls.filter(c=>c.url.includes('/submit')).length,before+1);
 await bridge.action('loan-guid','submit');
 const commands=calls.filter(c=>c.url.includes('/submit')).slice(-2);
 assert.equal(commands[0].options.headers['Idempotency-Key'],commands[1].options.headers['Idempotency-Key']);
 assert.equal(JSON.parse(commands[0].options.body).version,'AAAC');
});

test('nhật ký không sử dụng gửi null thực tế, không ép thành thời gian hoặc số lượng giả',()=>{
 for(const kind of ['room','device']){
  const data=logPayload({kind,date:'2026-11-01',quantity:3},{used:'false',start:'08:00',end:'10:00',students:'20',quantity:'2',notes:'Buổi học hủy'},'version');
  assert.equal(data.used,false);assert.equal(data.actualStart,null);assert.equal(data.actualEnd,null);
  assert.equal(kind==='room'?data.attendees:data.quantity,null);assert.equal(data.version,'version');assert.equal(data.issue,'Buổi học hủy');
 }
 const used=logPayload({kind:'device',date:'2026-11-01',quantity:3},{used:'true',start:'08:00',end:'10:00',quantity:'2'});
 assert.equal(used.used,true);assert.equal(used.quantity,2);assert.equal(used.actualStart,'2026-11-01T08:00:00+07:00');
});

test('bridge gửi biểu mẫu không sử dụng qua API thật với CSRF, version và null đúng loại nguồn',async()=>{
 const bridge=new ServerResources();await bridge.load();const OriginalFormData=globalThis.FormData;
 globalThis.FormData=class extends Map{constructor(form){super(Object.entries(form.fields));}};
 try{
  await bridge.log({dataset:{id:'loan-guid'},fields:{used:'false',notes:'Không sử dụng'}});
  let request=calls.at(-1);assert.equal(request.url,'/api/v1/equipment-loans/loan-guid/log');assert.equal(request.options.headers['X-CSRF-TOKEN'],'test-token');
  assert.deepEqual(JSON.parse(request.options.body),{used:false,actualStart:null,actualEnd:null,condition:'',issue:'Không sử dụng',version:null,quantity:null});
  bridge.snapshot.requests.push({id:'room-request',kind:'room',date:'2026-11-01'});bridge.snapshot.logs.push({request:'room-request',api:{version:'new-version'}});
  await bridge.log({dataset:{id:'room-request'},fields:{used:'false'}});request=calls.at(-1);assert.equal(request.options.method,'PUT');
  assert.deepEqual(JSON.parse(request.options.body),{used:false,actualStart:null,actualEnd:null,condition:'',issue:'',version:'new-version',attendees:null});
 }finally{globalThis.FormData=OriginalFormData;}
});

test('CSV cá nhân chống công thức, giao phạm vi và quyền từ chối, không xuất phiếu người khác',()=>{
 const profile={id:'me',active:true,departmentId:'it',scope:'all',departmentIds:[],denied:[],grants:[{scope:'unit',permissions:['reports.export']}]};
 const row={id:'mine',kind:'room',ownerId:'me',departmentId:'it',name:'\uFEFF\t=SUM(1,1)\n"ghi chú"',date:'2026-11-04',start:'08:00',end:'10:00',status:'pending'};
 const csv=personalRequestsCsv([row,{...row,id:'outside-unit',departmentId:'economics'},{...row,id:'not-mine',ownerId:'other'}],profile,{pending:['Chờ duyệt']});
 assert.ok(csv.startsWith('\uFEFF'));assert.ok(csv.includes('"\'\uFEFF\t=SUM(1,1)\n""ghi chú"""'));
 assert.ok(csv.includes('Chờ duyệt'));assert.ok(!csv.includes('outside-unit'));assert.ok(!csv.includes('not-mine'));
 assert.throws(()=>personalRequestsCsv([row],{...profile,denied:['reports.export']}),/quyền xuất/);
 assert.throws(()=>personalRequestsCsv([row],{...profile,grants:[]}),/quyền xuất/);
 assert.ok(!personalRequestsCsv([row],profile,{},'device').includes('mine'));
});

test('tải chi tiết song song giới hạn số request, giữ thứ tự và không nuốt lỗi',async()=>{
 let active=0,peak=0;const result=await mapBounded(Array.from({length:13},(_,i)=>i),async value=>{active++;peak=Math.max(peak,active);await new Promise(r=>setTimeout(r,2));active--;return value*2;});
 assert.equal(peak,4);assert.deepEqual(result,Array.from({length:13},(_,i)=>i*2));
 await assert.rejects(()=>mapBounded([1],async()=>{throw Error('Scoped API error');}),/Scoped API error/);
});

test('không gian giảng viên tải theo màn, nhận tổng SQL, lọc phòng server và chỉ kiểm tra khả dụng trang hiện tại',async()=>{
 const savedFetch=api.fetch;const seen=[];
 api.fetch=async(url,options={})=>{
  seen.push({url,options});
  const json=(v,status=200)=>new Response(JSON.stringify(v),{status,headers:{'Content-Type':'application/json'}});
  if(url.endsWith('/departments'))return json([{id:'dept',name:'Khoa CNTT',active:true}]);
  if(url.includes('/lecturer-workspace/summary'))return json({todayApproved:5,pendingCount:12,borrowedDevices:7,logsNeeded:3,weekStart:'2026-11-02',agenda:[],recent:[]});
  if(url.includes('/lecturer-workspace/requests?'))return json({items:[{kind:'device',name:'Máy chiếu',logStatus:null,request:{id:'loan-p2',assetGroupId:'asset-9',assetName:'Máy chiếu',ownerId:'user-1',departmentId:'dept',start:'2026-11-08T01:00:00Z',end:'2026-11-08T03:00:00Z',quantity:2,returned:0,status:'pending',version:'V1',returns:[]}}],total:45,page:2,pageSize:20,counts:{all:45,room:20,device:25,pending:12,draft:4},pendingCount:12});
  if(url.includes('/lecturer-workspace/rooms?'))return json({items:[{id:'room-p2',code:'B202',name:'Phòng B202',departmentId:'dept',capacity:60,active:true,type:'Hội trường',version:'R1'}],total:31,page:2,pageSize:20,types:['Phòng học','Phòng máy','Hội trường'],pendingCount:12});
  if(url.includes('/rooms/room-p2/availability'))return json({slots:[{start:'2026-11-08T00:00:00Z',end:'2026-11-08T11:00:00Z'}]});
  return savedFetch(url,options);
 };
 try{
  const bridge=new ServerResources(false);
  const ov=await bridge.loadLecturerPage('overview',{date:'2026-11-04'});
  assert.equal(ov.lecturerSummary.pendingCount,12);assert.equal(ov.lecturerSummary.borrowedDevices,7);
  assert.ok(!seen.some(c=>c.url.includes('/availability')||c.url.includes('/log')));
  const reqPage=await bridge.loadLecturerPage('requests',{tab:'all',search:'Máy chiếu'},2);
  assert.equal(bridge.pageInfo.total,45);assert.equal(bridge.pageInfo.page,2);assert.equal(reqPage.requestCounts.device,25);
  const roomPage=await bridge.loadLecturerPage('rooms',{date:'2026-11-08',search:'CNTT',type:'Hội trường',capacity:40},2);
  assert.ok(seen.some(c=>c.url.includes('departmentIds=dept')&&c.url.includes('type=H%E1%BB%99i+tr%C6%B0%E1%BB%9Dng')));
  assert.deepEqual(roomPage.roomTypes,['Phòng học','Phòng máy','Hội trường']);
  assert.equal(seen.filter(c=>c.url.includes('/availability')).length,1);
 }finally{api.fetch=savedFetch;}
});

test('sửa nháp thiết bị qua PUT giữ nguyên ID, assetGroupId, nối version mới khi gửi duyệt và đồng bộ khi stale 409',async()=>{
 const savedFetch=api.fetch,OriginalFormData=globalThis.FormData;const seen=[];let serverVersion='V-NEW-2';
 globalThis.FormData=class extends Map{constructor(form){super(Object.entries(form.fields));}};
 api.fetch=async(url,options={})=>{
  seen.push({url,options});
  const json=(v,status=200)=>new Response(JSON.stringify(v),{status,headers:{'Content-Type':'application/json'}});
  if(url.endsWith('/auth/csrf'))return json({token:'test-token'});
  if(url.endsWith('/departments'))return json([{id:'dept',name:'Khoa thử nghiệm',active:true}]);
  if(url.endsWith('/api/v1/equipment-loans/loan-guid')&&options.method==='PUT'){
   const body=JSON.parse(options.body);
   if(body.version==='STALE')return json({message:'Dữ liệu đã thay đổi.'},409);
   return json({id:'loan-guid',assetGroupId:body.assetGroupId,quantity:body.quantity,purpose:body.purpose,status:'draft',version:serverVersion,returns:[]});
  }
  if(url.endsWith('/api/v1/equipment-loans/loan-guid/submit'))return json({id:'loan-guid',status:'pending',version:'V-SUB'});
  if(url.includes('/lecturer-workspace/requests?')&&url.includes('id=loan-guid'))return json({items:[{kind:'device',name:'Thiết bị thật',logStatus:null,request:{id:'loan-guid',assetGroupId:'asset-guid',assetName:'Thiết bị thật',ownerId:'user-1',departmentId:'dept',start:'2026-11-08T01:00:00Z',end:'2026-11-08T03:00:00Z',quantity:3,returned:0,status:'draft',version:'V-SYNCED',returns:[]}}],total:1,page:1,pageSize:1,counts:{all:1},pendingCount:0});
  return savedFetch(url,options);
 };
 try{
  const bridge=new ServerResources(false);bridge.departments=[{id:'dept',name:'Khoa thử nghiệm',active:true}];
  bridge.snapshot={catalog:{rooms:[],devices:[]},requests:[{id:'loan-guid',kind:'device',resource:'asset-guid',api:{id:'loan-guid',assetGroupId:'asset-guid',version:'AAAC'}}],logs:[],roomReservations:[]};
  const form={dataset:{kind:'device',resource:'asset-guid',existing:'loan-guid'},fields:{date:'2026-11-08',start:'08:00',end:'10:00',count:'3',purpose:'Thực hành mạng',commitment:'on'}};
  await bridge.book(form,'pending');
  const putCall=seen.find(c=>c.options.method==='PUT');const subCall=seen.find(c=>c.url.endsWith('/submit'));
  assert.equal(putCall.url,'/api/v1/equipment-loans/loan-guid');
  assert.deepEqual(JSON.parse(putCall.options.body),{assetGroupId:'asset-guid',start:'2026-11-08T08:00:00+07:00',end:'2026-11-08T10:00:00+07:00',quantity:3,purpose:'Thực hành mạng',version:'AAAC'});
  assert.equal(JSON.parse(subCall.options.body).version,'V-NEW-2');
  bridge.snapshot.requests[0].api.version='STALE';
  const staleForm={dataset:{kind:'device',resource:'asset-guid',existing:'loan-guid'},fields:{date:'2026-11-08',start:'08:00',end:'10:00',count:'4',purpose:'Đổi số lượng'}};
  await assert.rejects(()=>bridge.book(staleForm,'draft'));
  const synced=await bridge.syncStaleDraft('loan-guid');
  assert.equal(synced.status,'draft');assert.equal(bridge.snapshot.requests[0].api.version,'V-SYNCED');
 }finally{api.fetch=savedFetch;globalThis.FormData=OriginalFormData;}
});

test('lazy log tôn trọng quyền logs.write và xuất CSV lấy đủ các trang theo bộ lọc',async()=>{
 const savedFetch=api.fetch;const seen=[];
 api.fetch=async(url,options={})=>{
  seen.push({url,options});const json=(v,status=200)=>new Response(JSON.stringify(v),{status,headers:{'Content-Type':'application/json'}});
  if(url.endsWith('/auth/me'))return json({id:'user-1',active:true,departmentId:'dept',scope:'self',denied:['logs.write'],grants:[{roleId:'lecturer',scope:'self',permissions:['rooms.book','devices.borrow','logs.write','reports.export']}]});
  if(url.includes('/lecturer-workspace/requests?'))return json({items:Array.from({length:url.includes('page=1')?100:1},(_,i)=>({kind:'room',name:'A101',logStatus:null,request:{id:'r-'+ (url.includes('page=1')?i:100),roomId:'rm1',ownerId:'user-1',departmentId:'dept',start:'2026-11-08T01:00:00Z',end:'2026-11-08T03:00:00Z',attendees:20,status:'pending',version:'V1'}})),total:101,page:url.includes('page=1')?1:2,pageSize:100,counts:{all:101},pendingCount:101});
  return savedFetch(url,options);
 };
 try{await requireSession();const bridge=new ServerResources(false);bridge.departments=[{id:'dept',name:'Khoa thử nghiệm',active:true}];await bridge.loadLecturerPage('logs');assert.ok(!seen.some(c=>c.url.includes('/lecturer-workspace/logs')));assert.equal(await bridge.ensureLog({id:'r-1',kind:'room',departmentId:'dept',ownerId:'user-1'}),null);const exported=await bridge.loadExportRequests({tab:'all'});assert.equal(exported.length,101);}finally{api.fetch=savedFetch;await requireSession();}
});
