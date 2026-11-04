// Isolated API fixtures: this script does not write to SQL Server.
import {pathToFileURL,fileURLToPath} from 'node:url';
import {writeFile} from 'node:fs/promises';
import assert from 'node:assert/strict';
const {chromium}=await import(pathToFileURL(process.env.HPUNI_PLAYWRIGHT_MODULE||'C:/PYTHON/playwright-local/node_modules/playwright/index.mjs'));
const browser=await chromium.launch({channel:'msedge',headless:true}),base='http://127.0.0.1:5080';
const evidence={at:new Date().toISOString(),mode:'isolated API fixtures; no SQL writes',checks:[],errors:[]};
const deps=[{id:'economics',name:'A Khoa Kinh tế',active:true},{id:'it',name:'Khoa CNTT',active:true}];
const user=permissions=>({id:'fixture-user',active:true,userName:'fixture.user',displayName:'Người kiểm thử',roleIds:['custom'],departmentId:'it',departmentIds:[],scope:'all',denied:[],grants:[{scope:'all',permissions}]});
const paged=(items,total=items.length,page=1)=>({items,total,page,pageSize:20});
async function surface(profile,custom){
 const context=await browser.newContext({viewport:{width:1440,height:1000}}),page=await context.newPage(),calls=[];
 page.on('pageerror',e=>evidence.errors.push(e.message));
 await page.route('**/api/v1/**',async route=>{
  const request=route.request(),url=new URL(request.url()),path=url.pathname.replace('/api/v1','');calls.push({path,method:request.method(),query:Object.fromEntries(url.searchParams),data:request.postData()});
  let body=await custom?.(path,url.searchParams,request);
  if(body===undefined){
   if(path==='/auth/me')body=profile;
   else if(path==='/auth/csrf')body={token:'fixture-csrf'};
   else if(path==='/departments')body=deps;
   else if(path==='/resource-workspace/summary')body={roomCount:0,assetCount:1,owned:1,pending:0,returns:0,openInventory:0,runningMaintenance:0,pendingLogs:0};
   else if(path==='/notifications')body={...paged([]),unread:0};
   else if(path.endsWith('/availability'))body={slots:[]};
   else body=paged([]);
  }
  if(body?.fixtureStatus)return route.fulfill({status:body.fixtureStatus,json:{title:'Không tìm thấy',code:'NOT_FOUND'}});
  await route.fulfill({status:200,json:body});
 });
 return{context,page,calls};
}
async function screenshot(page,name){await page.setViewportSize({width:390,height:844});await page.waitForFunction(()=>document.querySelector('.sidebar')?.getBoundingClientRect().right<=1||!document.querySelector('.sidebar'));assert.ok(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth+1));await page.screenshot({path:fileURLToPath(new URL('../docs/'+name,import.meta.url)),fullPage:true,animations:'disabled'});}
try{
 let saved=null;
 const asset={id:'asset-a',code:'A01',name:'Máy đã ngừng',category:'equipment',unit:'chiếc',departmentId:'it',total:1,damaged:0,lost:0,active:false,version:'AA=='};
 const manager=await surface(user(['resources.manage']),async(path,q,r)=>{
  if(path==='/equipment'||path==='/resource-workspace/assets')return paged([asset]);
  if(path==='/equipment/asset-a'&&r.method()==='PUT'){saved=r.postDataJSON();Object.assign(asset,saved);return asset;}
 });
 await manager.page.goto(base+'/resources.html#/assets');await manager.page.locator('[data-action=asset-view]').click();await manager.page.locator('[data-action=asset-edit]').click();
 assert.equal(await manager.page.locator('[name=active]').inputValue(),'false');await manager.page.locator('[name=name]').fill('Máy đổi tên');await manager.page.locator('[data-operation=server-asset] button[type=submit]').click();await manager.page.locator('#dialog').waitFor({state:'hidden'});assert.equal(saved.active,false);
 assert.ok(manager.calls.some(c=>c.path==='/resource-workspace/assets'&&c.query.pageSize==='20'));
 evidence.checks.push('Sửa nhóm ngừng hoạt động giữ active=false và đọc danh mục quản lý');
 await manager.page.goto(base+'/resources.html#/rooms');await manager.page.locator('[data-action=room-new]').click();await manager.page.locator('[name=departmentId]').selectOption('economics');await manager.page.locator('[name=id]').fill('P-NEW');await manager.page.locator('[name=name]').fill('Phòng thử');
 await manager.page.locator('[data-operation=room] button[type=submit]').click();await manager.page.locator('#dialog').waitFor({state:'hidden'});
 assert.equal(JSON.parse(manager.calls.find(c=>c.path==='/rooms'&&c.method==='POST').data).departmentId,'economics');
 evidence.checks.push('Phòng mới lưu đúng đơn vị người dùng chọn');await manager.context.close();

 const pages=await surface(user(['resources.manage']),async(path,q)=>{
  if(path==='/resource-workspace/assets'){const p=Number(q.get('page')||1);return paged(Array.from({length:p===1?20:1},(_,i)=>({...asset,id:'asset-'+((p-1)*20+i),name:'Tài sản trang '+p+' dòng '+i})),21,p);}
 });
 await pages.page.goto(base+'/resources.html#/assets');await pages.page.getByText('Tài sản trang 1 dòng 0',{exact:true}).waitFor();await pages.page.locator('[data-action=resource-page][data-id="2"]').click();await pages.page.getByText('Tài sản trang 2 dòng 0',{exact:true}).waitFor();
 assert.equal(await pages.page.locator('#results tbody tr').count(),1);assert.ok(pages.calls.some(c=>c.path==='/resource-workspace/assets'&&c.query.page==='2'));assert.equal(pages.calls.some(c=>['/equipment','/room-requests','/inventories'].includes(c.path)),false);
 evidence.checks.push('Tài sản chuyển trang 2 bằng SQL paging, không tải các lịch sử không liên quan');await pages.context.close();

 const cross=await surface({...user(['resources.manage']),scope:'unit',grants:[{scope:'unit',permissions:['resources.manage']}]},async path=>{
  if(path==='/resource-workspace/requests')return paged([{kind:'device',name:'Máy liên đơn vị',manageDepartmentId:'it',request:{id:'loan-cross',assetGroupId:'asset-a',assetName:'Máy liên đơn vị',departmentId:'economics',ownerId:'other',status:'approved',quantity:1,returned:0,returns:[],start:'2026-11-05T01:00:00Z',end:'2026-11-05T02:00:00Z',purpose:'Giảng dạy',version:'AA=='}}]);
 });
 await cross.page.goto(base+'/resources.html#/loans');await cross.page.locator('[data-action=request]').click();await cross.page.locator('[data-operation=handover]').waitFor();assert.equal(await cross.page.locator('[data-operation=review]').count(),0);evidence.checks.push('Quản lý tài sản có thể bàn giao cho người mượn khác đơn vị, không được tự xét duyệt');await cross.context.close();

 const users=await surface(user(['users.manage']),async(path,q)=>{
  if(path==='/users'){const p=Number(q.get('page')||1);return{...paged(Array.from({length:p===1?20:1},(_,i)=>({...user([]),id:'u-'+((p-1)*20+i),displayName:'Tài khoản trang '+p+' dòng '+i,active:true,userName:'u'+i,email:'fixture@example.invalid',version:'AA==',overrideRoleIds:[]})),21,p),summary:{total:21,active:21,roles:[],departments:[]}};}
 });
 await users.page.goto(base+'/admin.html#/users');await users.page.getByText('Tài khoản trang 1 dòng 0',{exact:true}).waitFor();await users.page.locator('[data-action=admin-page][data-page="2"]').click();await users.page.getByText('Tài khoản trang 2 dòng 0',{exact:true}).waitFor();assert.equal(await users.page.locator('#user-results tbody tr').count(),1);assert.ok(users.calls.some(c=>c.path==='/users'&&c.query.page==='2'&&c.query.pageSize==='20'));evidence.checks.push('Tài khoản quản trị tải trang 20 hồ sơ từ máy chủ, tổng số độc lập');await users.context.close();

 const inventory=await surface(user(['inventory.manage']));await inventory.page.goto(base+'/resources.html#/inventory');await inventory.page.getByRole('heading',{name:'Kiểm kê tài sản',exact:true}).waitFor();
 assert.equal(inventory.calls.some(c=>['/rooms','/equipment','/maintenance','/room-requests','/equipment-loans'].includes(c.path)),false);
 assert.equal(await inventory.page.locator('#nav a[href="#/approvals"]').count(),0);await inventory.page.locator('[data-action=inventory-new]').click();await inventory.page.locator('[name=unit]').selectOption('Khoa CNTT');await inventory.page.locator('#dialog').press('Escape');
 evidence.checks.push('Kiểm kê-only vào được workspace và không tải API ngoài quyền');await screenshot(inventory.page,'frontend-followup-inventory-mobile.png');await inventory.context.close();

 const logs=await surface(user(['logs.review']),async(path,q)=>{
  if(path==='/resource-workspace/logs')return paged([{kind:'room',name:'Phòng nhật ký',request:{id:'request-a',roomId:'r',ownerId:'other',departmentId:'it',status:'approved',start:'2026-11-03T01:00:00Z',end:'2026-11-03T02:00:00Z',attendees:1},log:{id:'log-a',requestId:'request-a',used:false,status:'submitted',condition:'Tốt',issue:'',version:'AA=='}}]);
  if(path==='/room-requests/request-a/log')return{id:'log-a',requestId:'request-a',used:false,status:'submitted',condition:'Tốt',issue:'',version:'AA=='};
 });
 await logs.page.goto(base+'/resources.html#/logs');await logs.page.locator('[data-action=log-open]').waitFor();assert.ok(logs.calls.some(c=>c.path==='/resource-workspace/logs'&&c.query.pageSize==='20'));assert.equal(logs.calls.some(c=>c.query.review==='true'&&['/room-requests','/equipment-loans'].includes(c.path)),false);
 evidence.checks.push('Logs-only vào được danh sách xác nhận, metadata dùng đúng quyền');await logs.context.close();

 for(const [permission,route] of [['roles.manage','roles'],['departments.manage','departments'],['permissions.manage','permissions'],['audit.read','history']]){
  const admin=await surface(user([permission]),async path=>path==='/roles'?[]:path==='/permissions'?['users.manage','reports.read']:undefined);
  await admin.page.goto(base+'/admin.html#/'+route);await admin.page.locator('#nav .active').waitFor();assert.equal(admin.calls.some(c=>c.path==='/users'),false);
  assert.equal(await admin.page.locator('#main button[data-action=role],#main button[data-action=department]').count(),0);
  assert.equal(await admin.page.locator('#nav .active').getAttribute('href'),'#/'+route);await admin.context.close();
 }
 evidence.checks.push('Bốn workspace quản trị chỉ đọc mở đúng màn, không gọi users và không hiện nút ghi');

 const scoped={...user(['budget.read','budget.write']),scope:'unit',grants:[{scope:'unit',permissions:['budget.read','budget.write']}]};
 const controls=await surface(scoped,async(path,q)=>{
  if(path==='/finance/summary')return{balances:[],received:0};
  if(path==='/finance/period')return{departmentId:q.get('departmentId'),closed:false,year:2026,reason:'',version:'AA=='};
  if(path==='/finance/adjustments')return paged([{id:'adjust-'+q.get('page'),departmentId:'it',year:2026,category:'equipment',amount:100,reason:'Trang '+q.get('page'),status:'pending',reviewNote:'',version:'AA=='}],21,Number(q.get('page')));
 });
 await controls.page.goto(base+'/accounting.html#/controls');await controls.page.locator('[data-period-filter]').waitFor();assert.equal(await controls.page.locator('[data-period-filter] select').inputValue(),'it');assert.equal(await controls.page.locator('[data-period-filter] option').count(),1);
 await controls.page.locator('[data-adjustment-page="2"]').click();await controls.page.getByRole('cell',{name:'Trang 2',exact:true}).waitFor();assert.ok(controls.calls.some(c=>c.path==='/finance/adjustments'&&c.query.page==='2'&&c.query.year==='2026'));
 assert.equal(controls.calls.some(c=>c.path==='/finance/period'&&c.query.departmentId==='economics'),false);
 evidence.checks.push('Điều chỉnh chọn đơn vị hợp lệ mặc định và tải trang 2 từ máy chủ');await screenshot(controls.page,'frontend-followup-controls-mobile.png');await controls.context.close();

 const voucher=await surface(user(['budget.read']),async path=>{
  if(path==='/finance/entries/e')return{id:'e',kind:'expense',voucher:'PC-01',date:'2026-11-04',departmentId:'it',year:2026,category:'maintenance',amount:100,content:'Sửa máy',paymentRequestId:'p'};
  if(path==='/finance/payments/p')return{id:'p',content:'Bảo trì máy',maintenanceId:'m'};
  if(path==='/finance/entries/e/attachments')return[];
 });
 await voucher.page.goto(base+'/payment-voucher.html?id=e');await voucher.page.locator('#print:not([disabled])').waitFor();await voucher.page.getByText('Khoản mục: Bảo trì · Năm 2026').waitFor();assert.equal(voucher.calls.some(c=>c.path==='/finance/payments'),false);assert.ok(voucher.calls.some(c=>c.path==='/finance/reversals'&&c.query.entryIds==='e'));
 evidence.checks.push('Chứng từ chỉ tải yêu cầu liên quan và đảo theo entryIds, nhãn tiếng Việt');await voucher.context.close();
 assert.deepEqual(evidence.errors,[]);evidence.status='passed';
}catch(e){evidence.status='failed';evidence.error=e.stack;throw e;}
finally{await writeFile(new URL('../docs/frontend-followup-ui.json',import.meta.url),JSON.stringify(evidence,null,2));await browser.close();}
