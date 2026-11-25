import {HpuniApi} from './api-client.mjs';
import {createEquipmentUi} from './equipment-live.mjs';
const api = new HpuniApi();
const $ = s => document.querySelector(s);
const esc = v => String(v ?? '').replace(/[&<>"']/g, c => ({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
const labels = {draft:'Bản nháp',pending:'Chờ duyệt',approved:'Đã duyệt',rejected:'Từ chối',cancelled:'Đã hủy',submitted:'Chờ xác nhận',needs_changes:'Cần bổ sung',confirmed:'Đã xác nhận'};
const date = v => new Date(v).toLocaleString('vi-VN', {timeZone:'Asia/Ho_Chi_Minh'});
const local = v => v ? new Date(new Date(v).getTime()+7*3600000).toISOString().slice(0,16) : '';
const instant = v => v ? `${v}:00+07:00` : null;
let me, view='rooms', page=1, generation=0;
const can = p => me?.active && !me.denied.includes(p) && me.grants.some(g => g.permissions.includes(p));
function message(text='') { $('#message').textContent=text; $('#message').hidden=!text; }
function signedOut() { me=null; generation++; attempts.clear(); $('#login').hidden=false; $('#workspace').hidden=true; $('#logout').hidden=true; $('#identity').textContent=''; $('#content').replaceChildren(); }
function error(e) { if(e.status===401&&e.code!=='LOGIN_FAILED') signedOut(); message(e.status===401&&e.code!=='LOGIN_FAILED'?'Phiên đăng nhập đã kết thúc. Vui lòng đăng nhập lại.':`${e.message}${e.requestId?` (Mã: ${e.requestId})`:''}${e.status===409?' Tải lại danh sách trước khi thao tác tiếp.':''}`); }
async function run(task, target) { if(target?.disabled)return; if(target)target.disabled=true; message(); try { await task(); } catch(e) { error(e); } finally { if(target)target.disabled=false; } }
function panel(title, body) { $('#content').innerHTML=`<section class="panel"><h1>${esc(title)}</h1>${body}</section>`; }
function field(label,name,type='text',value='',extra='') { return `<label>${label}<input name="${name}" type="${type}" value="${esc(value)}" ${extra}></label>`; }
function activate(user) {
  me=user; $('#login').hidden=true; $('#workspace').hidden=false; $('#logout').hidden=false; $('#identity').textContent=user.displayName;
  const tabs=[['rooms','Danh mục phòng','rooms.book'],['mine','Phiếu của tôi','rooms.book'],['review','Duyệt phiếu','requests.review'],['manage','Thêm phòng','resources.manage'],['equipment','Thiết bị',can('resources.manage')?'resources.manage':'devices.borrow'],['equipment-loans','Phiếu mượn','devices.borrow'],['equipment-review','Quản lý mượn/trả',can('resources.manage')?'resources.manage':'requests.review'],['equipment-assets','Hồ sơ tài sản','resources.manage'],['equipment-maintenance','Bảo trì','resources.manage'],['equipment-inventories','Kiểm kê','inventory.manage'],['accounts','Tài khoản','users.manage'],['password','Đổi mật khẩu',null]].filter(t=>!t[2]||can(t[2]));
  $('#navigation').innerHTML=tabs.map(([id,name])=>`<button data-view="${id}" class="secondary">${name}</button>`).join('');
  view=tabs.some(t=>t[0]===view)?view:tabs[0][0];
}
async function render() {
  const current=++generation;
  $('#navigation').querySelectorAll('button').forEach(b=>b.setAttribute('aria-current',String(b.dataset.view===view)));
  panel('Đang tải…','');
  if(view.startsWith('equipment')) return equipment.render(view,page);
  if(view==='rooms') {
    const result=await api.rooms({page,pageSize:12}); if(current!==generation)return;
    panel('Danh mục phòng',`<p>Chọn phòng để xem giờ trống và lập phiếu.</p><div class="grid">${result.items.map(r=>`<article class="panel"><h2>${esc(r.code)} · ${esc(r.name)}</h2><p>${esc(r.type)} · ${r.capacity} chỗ</p><button data-room="${esc(r.id)}">Xem lịch và đặt phòng</button></article>`).join('')||'<p>Chưa có phòng. Quản trị viên cần thêm phòng vào hệ thống.</p>'}</div>${pager(result)}`);
  } else if(view==='mine'||view==='review') {
    const result=await api.roomRequests({review:view==='review',page,pageSize:15}); if(current!==generation)return;
    panel(view==='mine'?'Phiếu của tôi':'Duyệt phiếu',`<div class="scroll"><table><thead><tr><th>Mục đích</th><th>Thời gian</th><th>Trạng thái</th><th></th></tr></thead><tbody>${result.items.map(r=>`<tr><td>${esc(r.purpose)}</td><td>${esc(date(r.start))}<br>${esc(date(r.end))}</td><td>${labels[r.status]||esc(r.status)}</td><td><button data-request="${esc(r.id)}">Chi tiết</button></td></tr>`).join('')}</tbody></table></div>${result.total?'':'<p>Chưa có phiếu trong phạm vi được cấp.</p>'}${pager(result)}`);
  } else if(view==='manage') {
    const departments=await api.departments(); if(current!==generation)return;
    panel('Thêm phòng',`<form id="room-form">${field('Mã phòng','code','text','','required maxlength="40"')}${field('Tên phòng','name','text','','required maxlength="200"')}${field('Loại phòng','type','text','','required maxlength="100"')}<label>Đơn vị quản lý<select name="departmentId">${departments.filter(d=>d.active).map(d=>`<option value="${esc(d.id)}">${esc(d.name)}</option>`).join('')}</select></label>${field('Sức chứa','capacity','number','30','required min="1" max="10000"')}<button>Lưu phòng</button></form>`);
  } else if(view==='accounts') {
    const [users,roles,departments]=await Promise.all([api.request(`/users?page=${page}&pageSize=15`),api.request('/roles'),api.departments()]); if(current!==generation)return;
    panel('Tài khoản',`<div class="scroll"><table><thead><tr><th>Tài khoản</th><th>Họ tên</th><th>Trạng thái</th><th>Phạm vi</th></tr></thead><tbody>${users.items.map(u=>`<tr><td>${esc(u.userName)}</td><td>${esc(u.displayName)}</td><td>${u.active?'Hoạt động':'Đã khóa'}</td><td>${esc(u.scope)}</td></tr>`).join('')}</tbody></table></div>${pager(users)}<h2>Cấp tài khoản</h2><p>Tài khoản mới có phạm vi cá nhân. Quản trị viên có thể cấp phạm vi đơn vị hoặc toàn trường bên dưới.</p><form id="account-form">${field('Tên đăng nhập','userName','text','','required maxlength="100"')}${field('Họ tên','displayName','text','','required maxlength="200"')}${field('Email','email','email','','required maxlength="254"')}${field('Mật khẩu ban đầu','password','password','','required minlength="12" maxlength="128" autocomplete="new-password"')}<label>Đơn vị<select name="departmentId">${departments.filter(d=>d.active).map(d=>`<option value="${esc(d.id)}">${esc(d.name)}</option>`).join('')}</select></label><label>Vai trò<select name="roleId">${roles.map(r=>`<option value="${esc(r.id)}">${esc(r.name)}</option>`).join('')}</select></label><button>Cấp tài khoản</button></form><h2>Cấp phạm vi</h2><form id="scope-form"><label>Tài khoản<select name="id">${users.items.map(u=>`<option value="${esc(u.id)}" data-version="${esc(u.version)}" data-denied="${esc(JSON.stringify(u.denied))}">${esc(u.userName)}</option>`).join('')}</select></label><label>Phạm vi<select name="scope"><option value="self">Cá nhân</option><option value="unit">Đơn vị của tài khoản</option><option value="all">Toàn trường</option></select></label><p>Các quyền loại trừ hiện tại được giữ nguyên. Chỉ cấp phạm vi phù hợp trách nhiệm của tài khoản.</p><button>Cập nhật phạm vi</button></form>`);
  } else panel('Đổi mật khẩu',`<p>Sau khi đổi mật khẩu, tất cả phiên đăng nhập sẽ kết thúc.</p><form id="password-form">${field('Mật khẩu hiện tại','currentPassword','password','','required autocomplete="current-password"')}${field('Mật khẩu mới (12–128 ký tự, có chữ và số)','newPassword','password','','required minlength="12" maxlength="128" autocomplete="new-password"')}<button>Đổi mật khẩu</button></form>`);
}
function pager(result) { return `<div class="actions"><button data-page="${page-1}" ${page<=1?'disabled':''}>Trang trước</button><span>Trang ${page} · ${result.total} kết quả</span><button data-page="${page+1}" ${page*result.pageSize>=result.total?'disabled':''}>Trang sau</button></div>`; }
async function booking(id, day=new Date().toLocaleDateString('en-CA',{timeZone:'Asia/Ho_Chi_Minh'})) {
  const current=++generation; panel('Đang tải lịch phòng…','');
  const result=await api.roomAvailability(id,day); if(current!==generation)return;
  panel('Đăng ký phòng',`<form id="availability-form" data-id="${esc(id)}">${field('Ngày sử dụng','date','date',day,'required')}<button>Xem giờ trống</button></form><p>Giờ Việt Nam. Bản nháp chưa giữ chỗ; phiếu đã gửi chờ duyệt có giữ chỗ.</p><ul>${result.slots.map(s=>`<li>${esc(date(s.start))} – ${esc(date(s.end))}</li>`).join('')||'<li>Không còn giờ trống.</li>'}</ul><form id="booking-form" data-room="${esc(id)}">${field('Bắt đầu','start','datetime-local',local(result.slots[0]?.start),'required')}${field('Kết thúc','end','datetime-local','','required')}${field('Số người','attendees','number','1','required min="1"')}<label>Mục đích<textarea name="purpose" maxlength="2000"></textarea></label><button>Lưu bản nháp</button></form>`);
}
async function detail(id) {
  const current=++generation; panel('Đang tải phiếu…','');
  const review=view==='review'; const r=await api.roomRequest(id,review);
  let log=null; if(r.status==='approved'&&can(review?'logs.review':'logs.write')) {
    try { log=await api.request(`/room-requests/${encodeURIComponent(id)}/log?review=${review}`); }
    catch(e) { if(e.status!==404)throw e; }
  }
  if(current!==generation)return;
  const actions=review?(r.status==='pending'?['approve','reject']:[]):(['draft','pending'].includes(r.status)?[...(r.status==='draft'?['submit']:[]),'cancel']:[]);
  const names={submit:'Gửi xét duyệt',approve:'Phê duyệt',reject:'Từ chối',cancel:'Hủy phiếu'};
  panel('Chi tiết phiếu',`<p class="meta">${esc(r.id)} · Phòng ${esc(r.roomId)}</p><h2>${esc(r.purpose)}</h2><p>${esc(date(r.start))} – ${esc(date(r.end))}<br>${r.attendees} người</p><p class="status">${labels[r.status]}</p><p>${esc(r.reviewNote)}</p><form id="action-form"><label>Ghi chú<textarea name="note" maxlength="2000"></textarea></label><div class="actions">${actions.map(a=>`<button name="action" value="${a}">${names[a]}</button>`).join('')}<button type="button" data-reload>Về danh sách</button></div></form><div id="log"></div>`);
  $('#action-form').addEventListener('submit',e=>{e.preventDefault(); const op=e.submitter.value; run(async()=>{await mutation(`action:${id}:${op}:${r.version}`,key=>api.roomAction(id,op,{version:r.version,note:new FormData(e.target).get('note')},key),{version:r.version,note:new FormData(e.target).get('note')});await detail(id);},e.submitter);});
  if(r.status!=='approved'||!can(review?'logs.review':'logs.write'))return;
  $('#log').innerHTML=`<h2>Nhật ký sử dụng</h2>${log?`<p class="status">${labels[log.status]}</p><p>${esc(log.reviewNote)}</p><p>${log.used?'Đã sử dụng':'Không sử dụng'} · ${esc(log.condition)} · ${esc(log.issue)}</p>${log.actualStart?`<p>${esc(date(log.actualStart))} – ${esc(date(log.actualEnd))} · ${log.attendees} người</p>`:''}`:'<p>Chưa có nhật ký.</p>'}`;
  if(!review&&(!log||log.status==='needs_changes')) {
    $('#log').insertAdjacentHTML('beforeend',`<form id="log-form"><label>Kết quả<select name="used"><option value="true" ${log?.used===false?'':'selected'}>Đã sử dụng</option><option value="false" ${log?.used===false?'selected':''}>Không sử dụng</option></select></label><div id="actual-fields">${field('Bắt đầu thực tế','actualStart','datetime-local',local(log?.actualStart))}${field('Kết thúc thực tế','actualEnd','datetime-local',local(log?.actualEnd))}${field('Số người thực tế','attendees','number',log?.attendees||'1','min="1"')}</div><label>Tình trạng<textarea name="condition" maxlength="2000">${esc(log?.condition)}</textarea></label><label>Sự cố<textarea name="issue" maxlength="2000">${esc(log?.issue)}</textarea></label><button>Gửi nhật ký</button></form>`);
    const form=$('#log-form'); const toggle=()=>{const used=form.elements.used.value==='true';$('#actual-fields').hidden=!used;for(const input of $('#actual-fields').querySelectorAll('input'))input.required=used;};form.elements.used.addEventListener('change',toggle);toggle();
    form.addEventListener('submit',e=>{e.preventDefault();run(async()=>{const d=Object.fromEntries(new FormData(form)),used=d.used==='true';await api.saveRoomLog(id,{used,actualStart:used?instant(d.actualStart):null,actualEnd:used?instant(d.actualEnd):null,attendees:used?Number(d.attendees):null,condition:d.condition,issue:d.issue,version:log?.version});await detail(id);},e.submitter);});
  } else if(review&&log?.status==='submitted') {
    $('#log').insertAdjacentHTML('beforeend','<form id="log-review"><label>Ý kiến<textarea name="note" maxlength="2000"></textarea></label><div class="actions"><button value="confirmed">Xác nhận</button><button value="needs_changes">Yêu cầu bổ sung</button></div></form>');
    $('#log-review').addEventListener('submit',e=>{e.preventDefault();run(async()=>{await api.reviewRoomLog(id,{version:log.version,result:e.submitter.value,note:new FormData(e.target).get('note')});await detail(id);},e.submitter);});
  }
}
// Retain the exact key for a retry after an uncertain network result; never retry automatically.
const attempts=new Map();
const equipment=createEquipmentUi({api,can,run,panel,field,esc,date,instant,mutation,
  navigate(next){view=next;$('#navigation').querySelectorAll('button').forEach(b=>b.setAttribute('aria-current',String(b.dataset.view===view)));},
  isActive:()=>Boolean(me)&&view.startsWith('equipment')});
async function mutation(operation,send,payload) { const signature=JSON.stringify([operation,payload]);const key=attempts.get(signature)||crypto.randomUUID();attempts.set(signature,key);const result=await send(key);attempts.delete(signature);return result; }
$('#login-form').addEventListener('submit',e=>{e.preventDefault();run(async()=>{const f=e.target;await api.login(f.username.value.trim(),f.password.value);f.password.value='';activate(await api.me());await render();},e.submitter);});
$('#logout').addEventListener('click',e=>run(async()=>{await api.logout();signedOut();},e.currentTarget));
$('#navigation').addEventListener('click',e=>{const b=e.target.closest('[data-view]');if(b)run(async()=>{view=b.dataset.view;page=1;await render();},b);});
$('#content').addEventListener('click',e=>{const b=e.target.closest('button');if(!b||b.closest('[data-equipment-ui]')||b.type==='submit'&&b.closest('form'))return;run(async()=>{if(b.dataset.room)await booking(b.dataset.room);else if(b.dataset.request)await detail(b.dataset.request);else if(b.hasAttribute('data-page')){page=Number(b.dataset.page);await render();}else if(b.hasAttribute('data-reload'))await render();},b);});
$('#content').addEventListener('submit',e=>{const f=e.target;if(!['booking-form','availability-form','room-form','password-form','account-form','scope-form'].includes(f.getAttribute('id')))return;e.preventDefault();run(async()=>{const d=Object.fromEntries(new FormData(f));if(f.getAttribute('id')==='availability-form')await booking(f.dataset.id,d.date);else if(f.getAttribute('id')==='account-form'){await api.request('/users',{method:'POST',data:{...d,roleIds:[d.roleId]}});f.reset();await render();message('Đã cấp tài khoản với phạm vi cá nhân.');}else if(f.getAttribute('id')==='scope-form'){await api.request(`/users/${encodeURIComponent(d.id)}/scope`,{method:'PUT',data:{scope:d.scope,departmentIds:[],denied:JSON.parse(f.elements.id.selectedOptions[0].dataset.denied),version:f.elements.id.selectedOptions[0].dataset.version}});activate(await api.me());await render();message('Đã cập nhật phạm vi.');}else if(f.getAttribute('id')==='booking-form'){const payload={roomId:f.dataset.room,start:instant(d.start),end:instant(d.end),attendees:Number(d.attendees),purpose:d.purpose};const r=await mutation('create',key=>api.createRoomRequest(payload,key),payload);view='mine';$('#navigation').querySelectorAll('button').forEach(b=>b.setAttribute('aria-current',String(b.dataset.view===view)));await detail(r.id);}else if(f.getAttribute('id')==='room-form'){await api.request('/rooms',{method:'POST',data:{...d,capacity:Number(d.capacity),active:true}});f.reset();message('Đã thêm phòng.');}else{await api.changePassword(d.currentPassword,d.newPassword);f.reset();signedOut();message('Đã đổi mật khẩu. Vui lòng đăng nhập lại.');}},e.submitter);});
await run(async()=>{try{activate(await api.me());await render();}catch(e){signedOut();if(e.status!==401)throw e;}});
