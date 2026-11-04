import {pathToFileURL,fileURLToPath} from 'node:url';
import {writeFile} from 'node:fs/promises';
import assert from 'node:assert/strict';
const {chromium}=await import(pathToFileURL(process.env.HPUNI_PLAYWRIGHT_MODULE||'C:/PYTHON/playwright-local/node_modules/playwright/index.mjs'));
const base=process.env.HPUNI_BASE_URL||'http://127.0.0.1:5081';
const browser=await chromium.launch({channel:'msedge',headless:true});
const context=await browser.newContext({viewport:{width:1440,height:1000}}),page=await context.newPage();
const evidence={at:new Date().toISOString(),mode:'isolated API fixtures; no SQL writes',checks:[],errors:[]};
const calls=[];let stale=false,version='V1';
const day=new Date(Date.now()+3*86400000).toISOString().slice(0,10),past=new Date(Date.now()-2*86400000).toISOString().slice(0,10);
const identity={id:'owner',active:true,displayName:'Giảng viên kiểm thử',userName:'lecturer.fixture',email:'fixture@example.invalid',departmentId:'it',departmentIds:[],scope:'self',roleIds:['lecturer'],denied:[],grants:[{scope:'self',permissions:['rooms.book','devices.borrow','logs.write','reports.export']}]};
const asset={id:'asset-draft',name:'Máy chiếu thử nghiệm',code:'TB-QA',departmentId:'it',category:'Giảng dạy',unit:'chiếc',total:100,damaged:0,lost:0,active:true,version:'A1'};
const room=id=>({id,code:id,name:'Phòng '+id,departmentId:'it',type:'Phòng học',capacity:60,active:true,version:'R1'});
const req=(id,status='draft')=>({kind:'device',name:asset.name,logStatus:null,request:{id,assetGroupId:asset.id,assetName:asset.name,ownerId:'owner',departmentId:'it',status,start:day+'T01:00:00Z',end:day+'T03:00:00Z',quantity:2,returned:0,purpose:'Phục vụ giảng dạy',version,returns:[]}});
const paged=(items,total=items.length,p=1)=>({items,total,page:p,pageSize:20,pendingCount:45});
page.on('pageerror',e=>evidence.errors.push(e.message));
await page.route('**/api/v1/**',async route=>{
 const r=route.request(),u=new URL(r.url()),path=u.pathname.replace('/api/v1',''),p=Number(u.searchParams.get('page')||1);
 calls.push({path,query:Object.fromEntries(u.searchParams),method:r.method(),data:r.postData()});
 let body;
 if(path==='/auth/me')body=identity;
 else if(path==='/auth/csrf')body={token:'fixture-csrf'};
 else if(path==='/departments')body=[{id:'it',name:'Khoa CNTT',active:true}];
 else if(path==='/notifications')body={...paged([]),unread:0};
 else if(path==='/lecturer-workspace/summary')body={todayApproved:8,pendingCount:45,borrowedDevices:7,logsNeeded:3,agenda:[],recent:[req('draft-a')],weekStart:day};
 else if(path==='/lecturer-workspace/calendar')body={items:[],pendingCount:45,weekStart:day};
 else if(path==='/lecturer-workspace/requests')body={...paged([req(u.searchParams.get('id')|| (p===1?'request-page-1':'draft-a'))],21,p),counts:{all:21,room:0,device:21,pending:0,draft:21}};
 else if(path==='/lecturer-workspace/rooms')body={...paged([room(u.searchParams.get('roomId')||'P'+p)],21,p),types:['Phòng học','Phòng máy']};
 else if(path==='/lecturer-workspace/devices')body={...paged([{...asset,name:u.searchParams.get('assetGroupId')?asset.name:'<img src=x onerror=alert(1)>'}],21,p),categories:['Giảng dạy','Thực hành']};
 else if(path.endsWith('/availability'))body={slots:[{start:day+'T00:00:00Z',end:day+'T11:00:00Z'}]};
 else if(path==='/lecturer-workspace/returns'){
  const rp=Number(u.searchParams.get('readyPage')||1);
  body={...paged([req('borrowed-a','borrowed')],1,p),readyTotal:55,readyPage:rp,readyItems:Array.from({length:rp===3?15:20},(_,i)=>req('ready-'+((rp-1)*20+i),'ready'))};
 } else if(path==='/lecturer-workspace/logs'){
  const ep=Number(u.searchParams.get('eligiblePage')||1),item=req('log-'+ep,'borrowed');item.request.start=past+'T01:00:00Z';item.request.end=past+'T03:00:00Z';
  body={...paged([],0,p),eligibleItems:[item],eligibleTotal:21,eligiblePage:ep};
 } else if(path==='/equipment-loans/draft-a'&&r.method()==='PUT'){
  if(stale){version='V-OTHER';return route.fulfill({status:409,json:{title:'Dữ liệu đã thay đổi',code:'STALE_VERSION'}});}
  const input=r.postDataJSON();assert.equal(input.version,version);version='V-SAVED';body={...req('draft-a').request,...input,version};
 } else if(path.endsWith('/log'))return route.fulfill({status:404,json:{code:'NOT_FOUND'}});
 else throw Error('Unexpected fixture request '+r.method()+' '+path);
 await route.fulfill({status:200,json:body});
});
async function routeTo(hash,title){await page.goto(base+'/index.html#/'+hash);await page.getByRole('heading',{name:title,exact:true}).waitFor();}
async function mobile(name){await page.setViewportSize({width:390,height:844});await page.waitForTimeout(100);assert.ok(await page.evaluate(()=>document.documentElement.scrollWidth<=innerWidth+1),'mobile overflow: '+name);await page.screenshot({path:fileURLToPath(new URL('../docs/lecturer-complete-'+name+'.png',import.meta.url)),fullPage:true,animations:'disabled'});await page.setViewportSize({width:1440,height:1000});}
try{
 await routeTo('overview','Tổng quan');assert.ok(await page.locator('.stats').innerText().then(t=>t.includes('45')&&t.includes('07')));await mobile('overview');
 evidence.checks.push('Tổng quan hiển thị tổng SQL độc lập với các trang');
 await routeTo('requests','Phiếu của tôi');await page.locator('[data-page-target=requests][data-page="2"]').click();await page.locator('[data-detail="draft-a"]').waitFor();
 await page.locator('[data-detail="draft-a"]').click();assert.equal(await page.locator('[data-replace-draft]').count(),0);await page.locator('[data-edit="draft-a"]').click();
 await page.locator('#booking-form').waitFor();await page.locator('[name=count]').fill('3');await page.locator('button[value=draft]').click();await page.locator('#dialog').waitFor({state:'hidden'});
 assert.ok(calls.some(c=>c.path==='/equipment-loans/draft-a'&&c.method==='PUT'&&JSON.parse(c.data).quantity===3));
 evidence.checks.push('Chuyển trang máy chủ và sửa nháp thiết bị bằng PUT, giữ ID');
 await page.locator('[data-page-target=requests][data-page="2"]').click();await page.locator('[data-detail="draft-a"]').click();await page.locator('[data-edit="draft-a"]').click();
 await page.locator('#booking-form').waitFor();stale=true;await page.locator('[name=count]').fill('4');await page.locator('button[value=draft]').click();await page.locator('#form-error').getByText(/Phiếu đã thay đổi/).waitFor();
 assert.equal(await page.locator('[name=count]').inputValue(),'4');const before=calls.filter(c=>c.method==='PUT').length;await page.locator('button[value=draft]').click();await page.waitForTimeout(100);assert.equal(calls.filter(c=>c.method==='PUT').length,before);stale=false;
 evidence.checks.push('Nháp stale giữ nội dung nhập và chặn ghi đè cho đến khi mở lại');
 await routeTo('rooms','Đăng ký phòng');await page.locator('[data-page-target=rooms][data-page="2"]').click();await page.getByRole('heading',{name:'Phòng P2',exact:true}).waitFor();await mobile('rooms');
 assert.ok(calls.some(c=>c.path==='/lecturer-workspace/rooms'&&c.query.page==='2'));evidence.checks.push('Danh mục phòng phân trang, khả dụng chỉ tải cho trang hiện tại');
 await routeTo('devices','Đăng ký thiết bị');assert.equal(await page.locator('.resource-body img').count(),0);assert.ok((await page.locator('.resource-body h2').innerText()).includes('<img'));await mobile('devices');evidence.checks.push('Danh mục thiết bị và nội dung chứa HTML được escape');
 await routeTo('returns','Trả thiết bị');await page.locator('[data-page-target=returnsReady][data-page="2"]').click();await page.locator('[data-page-target=returnsReady][data-page="3"]').click();await page.getByText('ready-54',{exact:false}).waitFor();await mobile('returns');evidence.checks.push('Chờ nhận hơn 50 phiếu có phân trang riêng, không cắt lịch sử');
 await routeTo('logs','Nhật ký sử dụng');await page.locator('[data-page-target=logsEligible][data-page="2"]').click();await page.locator('[data-log="log-2"]').waitFor();await page.locator('[data-log="log-2"]').click();await page.locator('#log-form').waitFor();await mobile('log-form');evidence.checks.push('Nhật ký phân trang độc lập và tải chi tiết khi mở');
 assert.equal(calls.some(c=>['/equipment','/equipment-loans','/room-requests','/rooms'].includes(c.path)&&c.method==='GET'),false);
 assert.deepEqual(evidence.errors,[]);evidence.status='passed';
}catch(e){evidence.status='failed';evidence.error=e.stack;await page.screenshot({path:fileURLToPath(new URL('../docs/lecturer-completion-failure.png',import.meta.url)),fullPage:true});throw e;}
finally{await writeFile(new URL('../docs/lecturer-completion-ui.json',import.meta.url),JSON.stringify(evidence,null,2));await browser.close();}
