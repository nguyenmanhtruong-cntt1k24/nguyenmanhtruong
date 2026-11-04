// Regression UI with controlled API fixtures; never writes to the user's SQL database.
import {pathToFileURL,fileURLToPath} from 'node:url';
import {writeFile} from 'node:fs/promises';
import assert from 'node:assert/strict';
const {chromium}=await import(pathToFileURL(process.env.HPUNI_PLAYWRIGHT_MODULE||'C:/PYTHON/playwright-local/node_modules/playwright/index.mjs'));
const base=process.env.HPUNI_UI_URL||'http://127.0.0.1:5080',browser=await chromium.launch({channel:'msedge',headless:true});
const evidence={at:new Date().toISOString(),mode:'API fixtures; no SQL writes',checks:[],errors:[]};
const output=new URL('../docs/frontend-fixes-ui.json',import.meta.url);
const user=permissions=>({id:'fixture-user',active:true,userName:'fixture.user',displayName:'Người kiểm thử',email:'fixture@example.invalid',roleIds:['custom'],departmentId:'it',departmentIds:[],scope:'all',denied:[],grants:[{roleId:'custom',scope:'all',permissions}]});
const assets=[{id:'asset-a',code:'A-01',name:'Máy A',departmentId:'it',total:3,damaged:0,lost:0,active:true},{id:'asset-b',code:'B-02',name:'Máy B',departmentId:'it',total:2,damaged:1,lost:0,active:true}];
const paged=(items,total=items.length,page=1,pageSize=20)=>({items,total,page,pageSize});
async function surface(profile,custom){
 const context=await browser.newContext({viewport:{width:1440,height:1000}}),page=await context.newPage(),calls=[];
 page.on('pageerror',e=>evidence.errors.push(e.message));
 await page.route('**/api/v1/**',async route=>{
  const request=route.request(),url=new URL(request.url()),path=url.pathname.replace('/api/v1','');calls.push({path,query:Object.fromEntries(url.searchParams),method:request.method()});
  const result=await custom?.(path,url.searchParams,request);
  let body=result;
  if(result===undefined){
   if(path==='/auth/me')body=profile;
   else if(path==='/auth/csrf')body={token:'fixture-csrf'};
   else if(path==='/departments')body=[{id:'it',name:'Khoa CNTT',code:'IT',active:true}];
   else if(path==='/notifications')body={...paged([]),unread:0};
   else if(path==='/finance/summary')body={balances:[{departmentId:'it',year:2026,category:'equipment',allocated:500,spent:0,available:500}],received:25};
   else body=paged([]);
  }
  if(body?.fixtureStatus)return route.fulfill({status:body.fixtureStatus,json:{message:'Không có nhật ký',code:'NOT_FOUND'}});
  await route.fulfill({status:200,json:body});
 });
 return {page,calls,context};
}
async function screenshot(page,name){await page.screenshot({path:fileURLToPath(new URL('../docs/'+name,import.meta.url)),fullPage:true,animations:'disabled'});}
async function mobile(page){await page.setViewportSize({width:390,height:844});await page.waitForFunction(()=>{const side=document.querySelector('.sidebar');return !side||side.getBoundingClientRect().right<=1;});assert.equal(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth+1),true);}
try{
 const finance=await surface(user(['budget.read']),async(path,q)=>{
  if(path==='/finance/payments'){const current=Number(q.get('page')||1);return paged(Array.from({length:current===1?20:1},(_,i)=>({id:'payment-'+((current-1)*20+i),departmentId:'it',year:2026,amount:100,paid:0,remaining:100,status:'pending',content:'Phiếu trang '+current+' dòng '+i,version:'AA=='})),21,current);}
 });
 await finance.page.goto(base+'/accounting.html#/overview');await finance.page.locator('.stat-value').filter({hasText:'500'}).first().waitFor();
 assert.equal(finance.calls.some(x=>['/finance/proposals','/finance/adjustments','/finance/entries'].includes(x.path)),false);
 evidence.checks.push('Tổng quan hiển thị số dư chỉ có điều chỉnh từ summary, không tải toàn bộ hồ sơ');
 await finance.page.locator('a[href="#/queue"]').click();await finance.page.getByText('Phiếu trang 1 dòng 0',{exact:true}).waitFor();
 assert.equal(await finance.page.locator('#main tbody tr').count(),20);await finance.page.locator('[data-finance=next]').click();await finance.page.getByText('Phiếu trang 2 dòng 0',{exact:true}).waitFor();
 assert.equal(await finance.page.locator('#main tbody tr').count(),1);assert.ok(finance.calls.some(x=>x.path==='/finance/payments'&&x.query.page==='2'&&x.query.year==='2026'));
 evidence.checks.push('Hàng chờ phân trang SQL page=2, giữ bộ lọc năm và không mất tổng số hồ sơ');await mobile(finance.page);await screenshot(finance.page,'frontend-fixes-finance-mobile.png');await finance.context.close();

 const exportProfile=user(['reports.export']);const exportOnly=await surface(exportProfile,async path=>path==='/auth/login'?exportProfile:undefined);
 await exportOnly.page.goto(base+'/login.html');await exportOnly.page.locator('#username').fill('custom.export');await exportOnly.page.locator('#password').fill('FixtureOnly123!');await exportOnly.page.locator('#login-form button[type=submit]').click();await exportOnly.page.waitForURL('**/reports.html#/overview');await exportOnly.page.getByRole('link',{name:'Excel (.xlsx)',exact:true}).waitFor();
 assert.equal(await exportOnly.page.locator('[data-detail-body] table').count(),0);assert.equal(exportOnly.calls.some(x=>x.path.startsWith('/finance')||x.path==='/reports/resources'),false);
 evidence.checks.push('Đăng nhập và vào workspace báo cáo bằng vai trò custom chỉ export, không ép quyền read/budget');await mobile(exportOnly.page);await screenshot(exportOnly.page,'frontend-fixes-reports-only-mobile.png');await exportOnly.context.close();

 const adminPermissions=['users.manage','roles.manage','permissions.manage','departments.manage','audit.read','reports.read','reports.export'];
 const adminProfile={...user(adminPermissions),roleIds:['admin'],grants:[{roleId:'admin',scope:'all',permissions:adminPermissions}]};
 let configuredRole={id:'admin',name:'Quản trị thử nghiệm',scope:'all',permissions:adminPermissions,version:'AA=='},savedRole=null;
 const admin=await surface(adminProfile,async(path,q,request)=>{
  if(path==='/users')return paged([{...adminProfile,version:'AA==',userName:adminProfile.userName,departmentIds:[],overrideRoleIds:[]}]);
  if(path==='/roles')return[configuredRole];
  if(path==='/roles/admin'&&request.method()==='PUT'){savedRole=request.postDataJSON();configuredRole={...configuredRole,...savedRole};return configuredRole;}
 });
 await admin.page.goto(base+'/admin.html#/roles');await admin.page.locator('[data-action=role][data-id=admin]').click();
 assert.equal(await admin.page.locator('[name=permissions][value="reports.read"]').isChecked(),true);assert.equal(await admin.page.locator('[name=permissions][value="reports.export"]').isChecked(),true);
 await admin.page.locator('[name=permissions][value="reports.export"]').uncheck();await admin.page.locator('#admin-form [name=confirm]').check();await admin.page.locator('#admin-form button[type=submit]').click();await admin.page.locator('#dialog').waitFor({state:'hidden'});
 assert.ok(savedRole.permissions.includes('reports.read'));assert.equal(savedRole.permissions.includes('reports.export'),false);evidence.checks.push('UI quản trị bỏ quyền export độc lập, payload giữ read và không âm thầm thêm lại export');await admin.context.close();

 const reports=await surface(user(['reports.read']),async path=>{
  if(path==='/reports/resources')return{year:2026,generatedAt:new Date().toISOString(),rows:[]};
  if(path==='/reports/details/assets')return{total:2,page:1,pageSize:20,generatedAt:new Date().toISOString(),note:'Nguyên giá lịch sử',columns:[{label:'Tên'},{label:'Nguyên giá',numeric:true}],items:[{cells:['Chưa khai','']},{cells:['Có giá trị 0','0']}]};
 });
 await reports.page.goto(base+'/reports.html');await reports.page.getByRole('cell',{name:'Chưa kê khai',exact:true}).waitFor();assert.equal(await reports.page.getByRole('cell',{name:'0',exact:true}).count(),1);
 assert.equal(await reports.page.getByRole('link',{name:'Excel (.xlsx)',exact:true}).count(),0);evidence.checks.push('Nguyên giá rỗng khác 0; read-only không có nút xuất');await reports.context.close();

 const lifecycle=await surface(user(['budget.read','budget.approve']),async(path,q)=>{
  if(path==='/asset-lifecycle/assets')return paged(assets);
  if(path.startsWith('/asset-lifecycle/profiles/'))return null;
  if(path==='/asset-lifecycle/disposals'){let items=assets.map((a,i)=>({id:'disposal-'+i,assetGroupId:a.id,assetCode:a.code,assetName:a.name,reference:'TL-'+i,good:1,damaged:0,reason:'Thử nghiệm',status:i?'approved':'pending',createdAt:new Date().toISOString(),reviewNote:'',version:'AA=='}));if(q.get('assetGroupId'))items=items.filter(x=>x.assetGroupId===q.get('assetGroupId'));if(q.get('status'))items=items.filter(x=>x.status===q.get('status'));return paged(items);}
 });
 await lifecycle.page.goto(base+'/executive.html#/lifecycle');await lifecycle.page.getByRole('cell',{name:'TL-1',exact:false}).waitFor();assert.equal(await lifecycle.page.locator('[data-lifecycle-asset]').inputValue(),'');
 await lifecycle.page.locator('[data-disposal-status]').selectOption('pending');await lifecycle.page.waitForFunction(()=>document.querySelectorAll('[data-open-lifecycle-asset]').length===1);
 await lifecycle.page.locator('[data-open-lifecycle-asset]').first().click();await lifecycle.page.getByRole('heading',{name:'Hồ sơ nguyên giá A-01'}).waitFor();evidence.checks.push('Hàng chờ tất cả tài sản, lọc trạng thái và mở đúng hồ sơ');await mobile(lifecycle.page);await screenshot(lifecycle.page,'frontend-fixes-lifecycle-mobile.png');await lifecycle.context.close();

 let savedLog=null;
 const lecturerRequest={id:'request-a',roomId:'room-a',ownerId:'fixture-user',departmentId:'it',start:'2026-11-03T01:00:00Z',end:'2026-11-03T03:00:00Z',status:'approved',attendees:20,purpose:'Kiểm tra nhật ký',version:'AA=='};
 const lecturer=await surface(user(['rooms.book','devices.borrow','logs.write']),async(path,q,request)=>{
  if(path==='/lecturer-workspace/logs')return{...paged(savedLog?[{kind:'room',name:'Phòng kiểm thử',request:lecturerRequest,log:savedLog}]:[]),eligibleItems:savedLog?[]:[{kind:'room',name:'Phòng kiểm thử',request:lecturerRequest}],eligibleTotal:savedLog?0:1,eligiblePage:1,pendingCount:0};
  if(path==='/rooms')return paged([{id:'room-a',code:'R01',name:'Phòng kiểm thử',departmentId:'it',capacity:30,active:true,type:'Phòng học'}]);
  if(path==='/equipment')return paged(assets);
  if(path==='/room-requests')return paged([{id:'request-a',roomId:'room-a',ownerId:'fixture-user',departmentId:'it',start:'2026-11-03T01:00:00Z',end:'2026-11-03T03:00:00Z',status:'approved',attendees:20,purpose:'Kiểm tra nhật ký',version:'AA=='}]);
  if(path==='/rooms/room-a/availability')return{slots:[{start:'2026-11-04T00:00:00Z',end:'2026-11-04T11:00:00Z'}]};
  if(path==='/room-requests/request-a/log'){if(request.method()==='GET')return savedLog||{fixtureStatus:404};savedLog={id:'log-a',...request.postDataJSON(),status:'submitted',version:'BB=='};return savedLog;}
 });
 await lecturer.page.goto(base+'/index.html#/logs');await lecturer.page.locator('[data-log="request-a"]').first().click();await lecturer.page.locator('#log-form [name=used]').selectOption('false');
 assert.equal(await lecturer.page.locator('#log-form [name=start]').isDisabled(),true);await lecturer.page.locator('#log-form button[type=submit]').click();await lecturer.page.locator('#dialog').waitFor({state:'hidden'});
 assert.equal(savedLog.used,false);assert.equal(savedLog.actualStart,null);assert.equal(savedLog.actualEnd,null);assert.equal(savedLog.attendees,null);evidence.checks.push('Biểu mẫu nhật ký Không sử dụng gửi null và giữ CSRF/version, đóng sau thành công');await lecturer.context.close();
 assert.deepEqual(evidence.errors,[]);evidence.status='passed';
}catch(error){evidence.status='failed';evidence.error=error.stack;throw error;}
finally{await writeFile(output,JSON.stringify(evidence,null,2));await browser.close();}
