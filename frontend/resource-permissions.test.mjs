import {test} from 'node:test';
import assert from 'node:assert/strict';
globalThis.location={search:''};
let profile,requests=[];
const row={id:'room-request',roomId:'room',ownerId:'lecturer',departmentId:'it',start:'2026-11-01T01:00:00Z',end:'2026-11-01T03:00:00Z',attendees:20,status:'approved'};
globalThis.fetch=async(url,options={})=>{
 requests.push({url,options});const json=value=>new Response(JSON.stringify(value),{headers:{'Content-Type':'application/json'}});
 if(url.endsWith('/auth/me'))return json(profile);
 if(url.endsWith('/departments'))return json([{id:'it',name:'CNTT',active:true},{id:'law',name:'Luật',active:true}]);
 if(url.endsWith('/auth/csrf'))return json({token:'test'});
 if(url.includes('/resource-workspace/inventories?'))return json({items:[{id:'inventory',name:'Kiểm kê',departmentId:'it',at:'2026-11-04T01:00:00Z',itemCount:11,counted:4}],total:1});
 if(url.includes('/resource-workspace/logs?'))return json({items:[{kind:'room',name:'Phòng',request:row,log:{id:'log',requestId:row.id,status:'submitted',version:'AA=='}}],total:1});
 if(url.includes('/resource-workspace/assets?'))return json({items:[{id:'asset',name:'Tài sản',departmentId:'it',active:true,total:5,damaged:0,lost:0}],total:43});
 if(url.includes('/resource-workspace/requests?'))return json({items:[{kind:'device',name:'Máy chiếu',manageDepartmentId:'it',request:{id:'cross-unit-loan',assetGroupId:'asset',ownerId:'lecturer-law',departmentId:'law',status:'approved',start:row.start,end:row.end,quantity:1,returned:0,returns:[]}}],total:1});
 if(url.includes('/room-requests?logReview=true'))return json({items:[row],total:1});
 if(url.endsWith('/room-requests/room-request/log?review=true'))return json({id:'log',requestId:row.id,status:'submitted',version:'AA=='});
 if(url.includes('/inventories?'))return json({items:[{id:'inventory'}],total:1});
 if(url.endsWith('/inventories/inventory'))return json({id:'inventory',name:'Kiểm kê',departmentId:'it',items:[],at:'2026-11-04T01:00:00Z'});
 if(options.method==='POST'||options.method==='PUT')return json({});
 if(url.includes('?'))return json({items:[],total:0});
 throw Error('Unexpected '+url);
};
const {requireSession}=await import('./server-session.mjs');
const {ServerResources,resourceAllows,resourceDepartments,assetPayload}=await import('./server-resources.mjs');
const actor=permission=>({id:'me',active:true,departmentId:'it',departmentIds:['it','law'],scope:'units',denied:[],grants:[{scope:'unit',permissions:[permission]}]});

test('sửa tài sản đang ngừng hoạt động giữ false, đổi trạng thái cần lựa chọn rõ ràng',()=>{
 const prior={active:false,version:'AA=='};
 assert.equal(assetPayload({name:'Sửa tên'},prior).active,false);
 assert.equal(assetPayload({active:'false'},prior).active,false);
 assert.equal(assetPayload({active:'true'},prior).active,true);
 assert.equal(assetPayload({},null).active,true);
 assert.equal(assetPayload({},prior).version,'AA==');
});
test('đơn vị được chọn giao scope tài khoản và scope quyền, denied thắng và self không cấp quyền sở hữu',()=>{
 const p=actor('resources.manage'),deps=[{id:'it',active:true},{id:'law',active:true},{id:'inactive',active:false}];
 assert.deepEqual(resourceDepartments(deps,p,'resources.manage').map(d=>d.id),['it']);
 assert.equal(resourceAllows({...p,denied:['resources.manage']},'resources.manage','it'),false);
 assert.equal(resourceAllows({...p,scope:'self'},'resources.manage','it'),false);
 assert.equal(resourceAllows({...p,scope:'self'},'resources.manage','it','me'),true);
});
test('kiểm kê-only không tải danh mục, hồ sơ duyệt hoặc API bảo trì vượt quyền',async()=>{
 profile=actor('inventory.manage');await requireSession();requests=[];
 const snapshot=await new ServerResources(true).loadPage('inventory',{},1);
 assert.equal(snapshot.inventories.length,1);assert.equal(snapshot.catalog.rooms.length,0);
 assert.ok(requests.some(r=>r.url.includes('/resource-workspace/inventories?')));
 assert.ok(!requests.some(r=>/\/(rooms|equipment|room-requests|equipment-loans|maintenance)[?\/]/.test(r.url)));
});
test('logs-only tải metadata theo logs.review và nhật ký, không gọi API duyệt hoặc danh mục',async()=>{
 profile=actor('logs.review');await requireSession();requests=[];
 const snapshot=await new ServerResources(true).loadPage('logs',{},1);
 assert.equal(snapshot.logs.length,1);assert.equal(snapshot.requests[0].id,row.id);
 assert.ok(requests.some(r=>r.url.includes('/resource-workspace/logs?')));
 assert.ok(!requests.some(r=>r.url.includes('?review=true&')||r.url.includes('/rooms?')||r.url.includes('/equipment?')||r.url.includes('/inventories?')));
});
test('danh sách tài sản lấy một trang SQL, giữ bộ lọc và tổng; không tải toàn lịch sử nguồn lực',async()=>{
 profile=actor('resources.manage');await requireSession();requests=[];const bridge=new ServerResources(true);
 const snapshot=await bridge.loadPage('assets',{search:'máy chiếu',status:'inactive',unit:'CNTT'},2);
 const data=requests.filter(r=>!r.url.endsWith('/departments'));assert.equal(data.length,1);
 const q=new URL(data[0].url,'http://local').searchParams;
 assert.equal(q.get('page'),'2');assert.equal(q.get('pageSize'),'20');assert.equal(q.get('search'),'máy chiếu');assert.equal(q.get('departmentId'),'it');assert.equal(q.get('status'),'inactive');
 assert.equal(bridge.pageInfo.total,43);assert.equal(snapshot.assets.length,1);assert.equal(snapshot.requests.length,0);assert.equal(snapshot.inventories.length,0);
});
test('phòng được lưu với đơn vị đã chọn thay vì đơn vị cũ hoặc mặc định tài khoản',async()=>{
 profile=actor('resources.manage');await requireSession();requests=[];
 const bridge=new ServerResources(true);bridge.departments=[{id:'it',name:'CNTT'},{id:'law',name:'Luật'}];
 bridge.snapshot={requests:[],assets:[],maintenance:[],inventories:[],catalog:{rooms:[{id:'room',api:{code:'A101',departmentId:'it',version:'old'}}]}};
 await bridge.manage('room',{id:'room',name:'Đổi đơn vị',type:'Phòng học',departmentId:'law',seats:'30',available:'true'});
 const payload=JSON.parse(requests.find(r=>r.options.method==='PUT').options.body);
 assert.equal(payload.departmentId,'law');assert.equal(payload.version,'old');
});
test('bàn giao theo đơn vị sở hữu thiết bị, xét duyệt vẫn theo đơn vị người mượn',async()=>{
 profile=actor('resources.manage');profile.grants.push({scope:'unit',permissions:['requests.review']});await requireSession();
 const snapshot=await new ServerResources(true).loadPage('loans',{},1),loan=snapshot.requests[0];
 assert.equal(loan.api.departmentId,'law');assert.equal(loan.manageDepartmentId,'it');
 assert.equal(resourceAllows(profile,'resources.manage',loan.manageDepartmentId,loan.ownerId),true);
 assert.equal(resourceAllows(profile,'requests.review',loan.api.departmentId,loan.ownerId),false);
});

test('xóa phòng và tài sản ngừng hoạt động đúng phiên bản, giữ nguyên thông tin hồ sơ',async()=>{
 profile=actor('resources.manage');await requireSession();requests=[];
 const bridge=new ServerResources(true),room={id:'room',code:'A101',name:'Phòng A101',type:'Phòng học',departmentId:'it',capacity:30,active:true,version:'room-version'},asset={id:'asset',code:'TS01',name:'Máy chiếu',category:'Thiết bị',unit:'chiếc',departmentId:'it',active:true,version:'asset-version'};
 bridge.snapshot={requests:[],assets:[{id:asset.id,api:asset}],maintenance:[],inventories:[],catalog:{rooms:[{id:room.id,api:room}]}};
 await bridge.manage('room-delete',{id:room.id});await bridge.manage('asset-delete',{id:asset.id});
 const writes=requests.filter(r=>r.options.method==='PUT');assert.equal(writes.length,2);
 const roomData=JSON.parse(writes[0].options.body),assetData=JSON.parse(writes[1].options.body);
 assert.equal(roomData.active,false);assert.equal(roomData.version,room.version);assert.equal(roomData.capacity,30);
 assert.equal(assetData.active,false);assert.equal(assetData.version,asset.version);assert.equal(assetData.departmentId,'it');
});
