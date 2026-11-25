export function createEquipmentUi({api, can, run, panel, field, esc, date, instant, mutation, navigate, isActive}) {
  const root = document.querySelector('#content');
  let mode = 'equipment', page = 1, serial = 0;
  const labels = {draft:'Bản nháp',pending:'Chờ duyệt',approved:'Đã duyệt',rejected:'Từ chối',cancelled:'Đã hủy',ready:'Chờ xác nhận nhận',receipt_disputed:'Sai lệch khi nhận',borrowed:'Đang mượn',return_pending:'Chờ tiếp nhận trả',completed:'Hoàn tất'};
  Object.assign(labels,{planned:'Đã lên lịch',running:'Đang thực hiện',open:'Đang kiểm kê',submitted:'Chờ xác nhận',needs_changes:'Cần bổ sung',confirmed:'Đã xác nhận'});
  const movementNames = {'stock.received':'Nhập kho','handed.over':'Bàn giao','returned.good':'Nhận trả tốt','returned.damaged':'Xác nhận hỏng','returned.lost':'Xác nhận mất'};
  Object.assign(movementNames,{'repair.restored':'Sửa chữa phục hồi','service.damaged':'Bảo dưỡng cần xử lý hỏng'});
  const draw = (title, body) => panel(title, `<div data-equipment-ui>${body}</div>`);
  const button = (name, data) => `<button type="button" ${data}>${name}</button>`;
  function begin(title) { const token = ++serial; draw(title, '<p>Đang tải…</p>'); return () => token === serial && isActive(); }
  const pager = result => `<div class="actions">${page > 1 ? button('Trang trước',`data-eq-page="${page-1}"`) : ''}<span>Trang ${page} · ${result.total} kết quả</span>${page * result.pageSize < result.total ? button('Trang sau',`data-eq-page="${page+1}"`) : ''}</div>`;
  async function render(nextMode = mode, nextPage = 1) {
    mode = nextMode; page = nextPage;
    const current = begin('Thiết bị và tài sản');
    if(mode==='equipment-maintenance')return maintenanceList(current);
    if(mode==='equipment-inventories')return inventoryList(current);
    if (mode === 'equipment' || mode === 'equipment-assets') {
      const result = await api.request(`/equipment?${new URLSearchParams({page, pageSize:12})}`);
      const departments = mode === 'equipment-assets' ? await api.departments() : [];
      if (!current()) return;
      draw(mode === 'equipment-assets' ? 'Hồ sơ tài sản cho mượn' : 'Danh mục thiết bị', `
        <p>Số sẵn có hiện tại chưa trừ các phiếu đặt trước trong tương lai. Xem khả dụng theo lịch trước khi đăng ký.</p>
        <div class="grid">${result.items.map(a => `<article class="panel"><h2>${esc(a.code)} · ${esc(a.name)}</h2>
          <p>${esc(a.category)} · Đơn vị tính: ${esc(a.unit)}</p>
          <p>Tổng: ${a.total} · Hỏng: ${a.damaged} · Mất: ${a.lost}<br>Đã bàn giao chưa hoàn tất: ${a.outstanding}<br>Giữ bảo dưỡng hiện tại: ${a.maintenanceBlockedNow}<br><strong>Sẵn có hiện tại: ${a.availableNow}</strong></p>
          ${can('devices.borrow') ? button('Xem lịch và đăng ký',`data-eq-book="${esc(a.id)}"`) : ''}
          ${mode === 'equipment-assets' ? button('Nhập kho và xem biến động',`data-eq-asset="${esc(a.id)}"`) : ''}</article>`).join('') || '<p>Chưa có thiết bị.</p>'}</div>${pager(result)}
        ${mode === 'equipment-assets' ? `<h2>Thêm nhóm tài sản</h2><p>Hồ sơ và danh mục mượn dùng chung số lượng. Sau khi tạo, nhập kho để ghi nhận tăng số lượng.</p>
        <form id="eq-asset-form">${field('Mã tài sản','code','text','','required maxlength="30"')}${field('Tên tài sản','name','text','','required maxlength="150"')}
        ${field('Nhóm','category','text','','required maxlength="100"')}${field('Đơn vị tính','unit','text','chiếc','required maxlength="30"')}
        <label>Đơn vị quản lý<select name="departmentId">${departments.filter(d=>d.active).map(d=>`<option value="${esc(d.id)}">${esc(d.name)}</option>`).join('')}</select></label><button>Tạo nhóm tài sản</button></form>` : ''}`);
      root.querySelector('#eq-asset-form')?.addEventListener('submit', e => {
        e.preventDefault(); const data = {...Object.fromEntries(new FormData(e.target)), active:true};
        run(async()=>{const a=await mutation('equipment.create', key=>api.keyed('/equipment',data,key),data);await asset(a.id);}, e.submitter);
      });
    } else {
      const review = mode === 'equipment-review';
      const result = await api.request(`/equipment-loans?${new URLSearchParams({review,page,pageSize:15})}`);
      if (!current()) return;
      draw(review ? 'Quản lý mượn và trả' : 'Phiếu mượn của tôi', `<div class="scroll"><table><thead><tr><th>Thiết bị</th><th>Thời gian</th><th>Số lượng</th><th>Trạng thái</th><th></th></tr></thead><tbody>
        ${result.items.map(r=>`<tr><td>${esc(r.assetCode)} · ${esc(r.assetName)}</td><td>${esc(date(r.start))}<br>${esc(date(r.end))}</td><td>${r.quantity} · Đã xử lý trả ${r.returned}</td><td>${labels[r.status]}${r.overdue?' · Quá hạn':''}</td><td>${button('Chi tiết',`data-eq-loan="${esc(r.id)}"`)}</td></tr>`).join('')}
        </tbody></table></div>${result.total ? '' : '<p>Chưa có phiếu trong phạm vi được cấp.</p>'}${pager(result)}`);
    }
  }
  async function asset(id) {
    const current = begin('Nhập kho và biến động');
    // Look up the asset by its immutable id; the exact-id endpoint avoids catalogue page assumptions.
    const a = await api.request(`/equipment/${encodeURIComponent(id)}`);
    const movements = await api.request(`/equipment/${encodeURIComponent(id)}/movements?pageSize=20`);
    if (!current()) return;
    draw('Nhập kho và biến động', `<h2>${esc(a.code)} · ${esc(a.name)}</h2><p>Tổng ${a.total} · Sẵn có ${a.availableNow} · Hỏng ${a.damaged} · Mất ${a.lost}</p>
      <form id="eq-stock-form">${field('Số lượng nhập thêm','quantity','number','1','required min="1" max="1000000"')}<label>Ghi chú<textarea name="note" maxlength="2000"></textarea></label><button>Ghi nhận nhập kho</button></form>
      <h2>Biến động gần nhất</h2><ul>${movements.items.map(m=>`<li>${esc(date(m.at))} · ${esc(movementNames[m.kind]||m.kind)} · ${m.quantity} · ${esc(m.note)}</li>`).join('')||'<li>Chưa có biến động.</li>'}</ul>
      <p>Hiển thị ${movements.items.length}/${movements.total} biến động.</p>${button('Về danh sách','data-eq-back')}`);
    root.querySelector('#eq-stock-form').addEventListener('submit',e=>{
      e.preventDefault();const f=Object.fromEntries(new FormData(e.target));const data={quantity:Number(f.quantity),note:f.note,version:a.version};
      run(async()=>{await mutation(`equipment.stock:${id}`,key=>api.keyed(`/equipment/${id}/stock`,data,key),data);await asset(id);},e.submitter);
    });
  }
  async function booking(id, values = {}) {
    const current = begin('Đăng ký mượn thiết bị');
    const a = await api.request(`/equipment/${encodeURIComponent(id)}`);
    let available = null;
    if(values.start && values.end) available = await api.request(`/equipment/${id}/availability?${new URLSearchParams({start:instant(values.start),end:instant(values.end)})}`);
    if (!current()) return;
    draw('Đăng ký mượn thiết bị', `<h2>${esc(a.code)} · ${esc(a.name)}</h2><p>Giờ Việt Nam. Lịch mượn tối đa 30 ngày. Bản nháp chưa giữ số lượng; gửi thành công bắt đầu giữ số lượng.</p>
      <form id="eq-booking-form">${field('Bắt đầu','start','datetime-local',values.start||'','required')}${field('Kết thúc','end','datetime-local',values.end||'','required')}
      ${field('Số lượng','quantity','number',values.quantity||'1','required min="1" max="1000000"')}
      <label>Mục đích<textarea name="purpose" maxlength="2000">${esc(values.purpose)}</textarea></label>
      ${available ? `<p role="status">Còn khả dụng trong khoảng đã chọn: <strong>${available.available}</strong></p>` : ''}
      <div class="actions"><button name="intent" value="check">Kiểm tra khả dụng</button><button name="intent" value="create">Lưu phiếu mượn nháp</button></div></form>`);
    root.querySelector('#eq-booking-form').addEventListener('submit',e=>{
      e.preventDefault();const data=Object.fromEntries(new FormData(e.target));
      run(async()=>{
        if(e.submitter.value==='check') return booking(id,data);
        const payload={assetGroupId:id,start:instant(data.start),end:instant(data.end),quantity:Number(data.quantity),purpose:data.purpose};
        const r=await mutation('equipment-loan.create',key=>api.keyed('/equipment-loans',payload,key),payload);
        mode='equipment-loans';navigate(mode);await detail(r.id);
      },e.submitter);
    });
  }
  async function detail(id) {
    const review = mode === 'equipment-review'; const current = begin('Chi tiết phiếu mượn');
    const r = await api.request(`/equipment-loans/${encodeURIComponent(id)}?review=${review}`);
    if (!current()) return;
    const operations = review ? [...(r.status==='pending'&&can('requests.review')?['approve','reject']:[]),...(r.status==='approved'&&can('resources.manage')?['handover']:[]),...(r.status==='receipt_disputed'&&can('resources.manage')?['resolve-receipt']:[])]
      : [...(r.status==='draft'?['submit']:[]),...(['draft','pending'].includes(r.status)?['cancel']:[]),...(r.status==='ready'?['receipt']:[])];
    const names={submit:'Gửi xét duyệt',approve:'Duyệt mượn',reject:'Từ chối',cancel:'Hủy phiếu',handover:'Bàn giao',receipt:'Xác nhận nhận','resolve-receipt':'Đã xử lý sai lệch, mời xác nhận lại'};
    draw('Chi tiết phiếu mượn', `<p class="meta">${esc(r.id)}</p><h2>${esc(r.assetCode)} · ${esc(r.assetName)}</h2><p>${esc(date(r.start))} – ${esc(date(r.end))}</p>
      <p>Số đăng ký ${r.quantity}${['ready','receipt_disputed','borrowed','return_pending','completed'].includes(r.status)?` · Đã xử lý trả ${r.returned} · Chưa xử lý trả ${r.quantity-r.returned}`:''}</p><p class="status">${labels[r.status]}${r.overdue?' · Quá hạn':''}</p><p>${esc(r.purpose)}</p><p>${esc(r.note)}</p>
      ${r.reportedReceived!==null?`<p>Số thực nhận đã khai báo: ${r.reportedReceived}</p>`:''}
      ${operations.length?`<form id="eq-action-form">${operations.some(op=>['handover','receipt'].includes(op))?field('Số lượng bàn giao / thực nhận','quantity','number',r.quantity,'required min="0" max="1000000"'):''}<label>Ghi chú<textarea name="note" maxlength="2000"></textarea></label>
      <div class="actions">${operations.map(op=>`<button name="operation" value="${op}">${names[op]}</button>`).join('')}</div></form>`:''}
      <div class="actions">${button('Tải lại chi tiết',`data-eq-loan="${esc(r.id)}"`)}${button('Về danh sách','data-eq-back')}</div>
      <div id="eq-return"></div><h2>Lịch sử trả</h2><ul>${r.returns.map(t=>`<li>${esc(date(t.at))} · Khai báo: tốt ${t.good}, hỏng ${t.damaged}, mất ${t.lost}<br>${t.acceptedAt?`Đã tiếp nhận: tốt ${t.acceptedGood}, hỏng ${t.acceptedDamaged}, mất ${t.acceptedLost}`:'Chờ tiếp nhận'}<br>${esc(t.note)} ${esc(t.acceptanceNote)}</li>`).join('')||'<li>Chưa có yêu cầu trả.</li>'}</ul>`);
    root.querySelector('#eq-action-form')?.addEventListener('submit',e=>{
      e.preventDefault();const op=e.submitter.value;const values=Object.fromEntries(new FormData(e.target));
      const data={version:r.version,note:values.note,quantity:values.quantity===undefined?null:Number(values.quantity)};
      run(async()=>{await mutation(`equipment-loan.${op}:${id}`,key=>api.keyed(`/equipment-loans/${id}/${op}`,data,key),data);await detail(id);},e.submitter);
    });
    const pending = r.returns.find(x=>!x.acceptedAt);
    if(!review&&r.status==='borrowed'||review&&can('resources.manage')&&r.status==='return_pending'&&pending) {
      const accepting=review; const target=root.querySelector('#eq-return');
      target.innerHTML=`<h2>${accepting?'Tiếp nhận thực tế':'Yêu cầu trả thiết bị'}</h2><p>Chỉ phần tốt do quản lý thực nhận mới về kho cho mượn. Tổng tiếp nhận không vượt yêu cầu trả và phần còn giữ.</p>
      <form id="eq-return-form">${field('Tốt','good','number',pending?.good||'0','required min="0" max="1000000"')}${field('Hỏng','damaged','number',pending?.damaged||'0','required min="0" max="1000000"')}${field('Mất','lost','number',pending?.lost||'0','required min="0" max="1000000"')}<label>Ghi chú<textarea name="note" maxlength="2000"></textarea></label><button>${accepting?'Xác nhận tiếp nhận':'Gửi yêu cầu trả'}</button></form>`;
      target.querySelector('form').addEventListener('submit',e=>{
        e.preventDefault();const values=Object.fromEntries(new FormData(e.target));const data={version:r.version,good:Number(values.good),damaged:Number(values.damaged),lost:Number(values.lost),note:values.note,...(accepting?{returnId:pending.id}:{})};
        const path=`/equipment-loans/${id}/${accepting?'accept-return':'returns'}`;
        run(async()=>{await mutation(path,key=>api.keyed(path,data,key),data);await detail(id);},e.submitter);
      });
    }
    if(['borrowed','return_pending','completed'].includes(r.status)&&can(review?'logs.review':'logs.write'))await logSection(r,review,current);
  }
  async function maintenanceList(current) {
    const result=await api.request(`/maintenance?${new URLSearchParams({page,pageSize:15})}`);
    const assets=await api.request('/equipment?pageSize=100');if(!current())return;
    draw('Bảo trì thiết bị',`<p>Mỗi nhóm có một công việc chưa hoàn tất. Bảo dưỡng giữ phần tốt theo lịch; sửa chữa dùng phần hỏng đã ghi nhận.</p>
      <ul>${result.items.map(m=>`<li>${m.mode==='repair'?'Sửa chữa':'Bảo dưỡng'} · ${m.quantity} · ${esc(date(m.start))} · ${labels[m.status]} ${button('Chi tiết',`data-eq-maintenance="${esc(m.id)}"`)}</li>`).join('')||'<li>Chưa có công việc.</li>'}</ul>${pager(result)}
      <h2>Lập kế hoạch</h2><form id="eq-maintenance-plan"><label>Nhóm thiết bị<select name="assetGroupId" required>${assets.items.map(a=>`<option value="${esc(a.id)}">${esc(a.code)} · ${esc(a.name)} · Hỏng ${a.damaged}</option>`).join('')}</select></label>
      <label>Loại công việc<select name="mode"><option value="service">Bảo dưỡng phần tốt</option><option value="repair">Sửa chữa phần hỏng</option></select></label>${field('Số lượng','quantity','number','1','required min="1" max="1000000"')}${field('Bắt đầu','start','datetime-local','','required')}${field('Kết thúc dự kiến','end','datetime-local','','required')}<label>Nội dung<textarea name="content" maxlength="2000"></textarea></label><button>Lập kế hoạch bảo trì</button></form><p>Danh sách chọn hiển thị tối đa 100 nhóm đang hoạt động.</p>`);
    root.querySelector('form').addEventListener('submit',e=>{e.preventDefault();const d=Object.fromEntries(new FormData(e.target));const data={...d,quantity:Number(d.quantity),start:instant(d.start),end:instant(d.end)};
      run(async()=>{const m=await mutation('maintenance.plan',key=>api.keyed('/maintenance',data,key),data);await maintenanceDetail(m.id);},e.submitter);});
  }
  async function maintenanceDetail(id) {
    const current=begin('Công việc bảo trì');const m=await api.request(`/maintenance/${id}`);if(!current())return;
    draw('Công việc bảo trì',`<p class="meta">${esc(m.id)} · Nhóm ${esc(m.assetGroupId)}</p><p>${m.mode==='repair'?'Sửa chữa phần hỏng':'Bảo dưỡng phần tốt'} · ${m.quantity} thiết bị<br>${esc(date(m.start))} – ${esc(date(m.end))}</p><p class="status">${labels[m.status]}</p><p>${esc(m.content)} ${esc(m.note)}</p>
      ${m.status==='completed'?`<p>Kết quả: ${m.result==='good'?'Tốt':'Còn cần xử lý'} · Chi phí ghi nhận ${m.cost.toLocaleString('vi-VN')} đ (chưa xác nhận thanh toán)</p>`:''}
      ${['planned','running'].includes(m.status)?`<form id="eq-maintenance-action">${m.status==='running'?`<label>Kết quả<select name="result"><option value="good">Tốt</option><option value="unavailable">Còn cần xử lý</option></select></label>${field('Chi phí thực hiện (đồng)','cost','number','0','required min="0" max="1000000000000" step="1"')}`:''}<label>Ghi chú<textarea name="note" maxlength="2000"></textarea></label><div class="actions">${m.status==='planned'?'<button value="start">Bắt đầu bảo trì</button><button value="cancel">Hủy kế hoạch</button>':'<button value="finish">Hoàn tất bảo trì</button>'}</div></form>`:''}${button('Về danh sách','data-eq-back')}`);
    root.querySelector('form')?.addEventListener('submit',e=>{e.preventDefault();const op=e.submitter.value;const d=Object.fromEntries(new FormData(e.target));const data={version:m.version,result:d.result||'',cost:Number(d.cost||0),note:d.note};run(async()=>{await mutation(`maintenance.${op}:${id}`,key=>api.keyed(`/maintenance/${id}/${op}`,data,key),data);await maintenanceDetail(id);},e.submitter);});
  }
  async function inventoryList(current) {
    const result=await api.request(`/inventories?${new URLSearchParams({page,pageSize:15})}`);const departments=await api.departments();if(!current())return;
    draw('Kiểm kê tài sản',`<p>Ảnh chụp số theo sổ được giữ tại lúc lập đợt. Ghi nhận kiểm kê không tự thay đổi tồn hoặc tình trạng cho mượn.</p><ul>${result.items.map(s=>`<li>${esc(s.name)} · ${labels[s.status]} ${button('Kiểm đếm',`data-eq-inventory="${esc(s.id)}"`)}</li>`).join('')||'<li>Chưa có đợt.</li>'}</ul>${pager(result)}
      <h2>Lập đợt kiểm kê</h2><form id="eq-inventory-create">${field('Tên đợt','name','text','','required maxlength="150"')}<label>Đơn vị<select name="departmentId">${departments.filter(d=>d.active).map(d=>`<option value="${esc(d.id)}">${esc(d.name)}</option>`).join('')}</select></label><button>Lập đợt kiểm kê</button></form>`);
    root.querySelector('form').addEventListener('submit',e=>{e.preventDefault();const data=Object.fromEntries(new FormData(e.target));run(async()=>{const s=await mutation('inventory.create',key=>api.keyed('/inventories',data,key),data);await inventoryDetail(s.id);},e.submitter);});
  }
  async function inventoryDetail(id) {
    const current=begin('Ghi nhận kiểm kê');const s=await api.request(`/inventories/${id}`);if(!current())return;
    draw('Ghi nhận kiểm kê',`<h2>${esc(s.name)}</h2><p class="status">${labels[s.status]}</p><p>Kiểm đếm toàn bộ số sở hữu, gồm phần đang cho mượn; ghi rõ phạm vi thực tế trong ghi chú nếu có chênh lệch.</p><form id="eq-inventory-count">${s.items.map((r,i)=>`<fieldset data-count-row="${i}"><legend>${esc(r.code)} · ${esc(r.name)}</legend><p>Theo sổ ${r.expected} · Hỏng ${r.expectedDamaged} · Mất ${r.expectedLost}</p>${field('Thực tế','actual','number',r.actual??'','min="0" max="1000000"')}<label>Tình trạng<select name="condition">${[['','Chưa ghi nhận'],['good','Tốt'],['damaged','Hỏng/cần xử lý'],['missing','Mất/thiếu']].map(([v,n])=>`<option value="${v}" ${v===r.condition?'selected':''}>${n}</option>`).join('')}</select></label><label>Ghi chú<textarea name="note" maxlength="2000">${esc(r.note)}</textarea></label></fieldset>`).join('')}
      ${s.status==='open'?'<div class="actions"><button value="save">Lưu kiểm đếm</button><button value="complete">Hoàn tất kiểm kê</button></div>':''}</form>${button('Về danh sách','data-eq-back')}`);
    const form=root.querySelector('form');if(s.status!=='open'){form.querySelectorAll('input,select,textarea').forEach(e=>e.disabled=true);return;}
    form.addEventListener('submit',e=>{e.preventDefault();const items=[...form.querySelectorAll('[data-count-row]')].map((row,i)=>({assetGroupId:s.items[i].assetGroupId,actual:row.querySelector('[name=actual]').value===''?null:Number(row.querySelector('[name=actual]').value),condition:row.querySelector('[name=condition]').value,note:row.querySelector('[name=note]').value}));const data={version:s.version,items,complete:e.submitter.value==='complete'};
      run(async()=>{await mutation(`inventory.save:${id}`,key=>api.keyed(`/inventories/${id}/count`,data,key),data);await inventoryDetail(id);},e.submitter);});
  }
  async function logSection(r,review,current) {
    let log=null;try{log=await api.request(`/equipment-loans/${r.id}/log?review=${review}`);}catch(e){if(e.status!==404)throw e;}if(!current())return;
    root.querySelector('[data-equipment-ui]').insertAdjacentHTML('beforeend',`<section id="eq-use-log"><h2>Nhật ký sử dụng thiết bị</h2>${log?`<p class="status">${labels[log.status]}</p><p>${log.used?'Đã sử dụng':'Không sử dụng'} · ${esc(log.condition)} · ${esc(log.issue)}</p>${log.used?`<p>${esc(date(log.actualStart))} – ${esc(date(log.actualEnd))} · ${log.quantity} thiết bị</p>`:''}<p>${esc(log.reviewNote)}</p>`:'<p>Chưa có nhật ký. Gửi sau khi kết thúc lịch mượn.</p>'}</section>`);
    const section=root.querySelector('#eq-use-log');
    if(!review&&(!log||log.status==='needs_changes')) {
      section.insertAdjacentHTML('beforeend',`<form><label>Kết quả sử dụng<select name="used"><option value="true">Đã sử dụng</option><option value="false">Không sử dụng</option></select></label><div id="eq-log-actual">${field('Bắt đầu thực tế','actualStart','datetime-local','','')}${field('Kết thúc thực tế','actualEnd','datetime-local','','')}${field('Số lượng thực tế','quantity','number',log?.quantity||'1','min="1" max="1000000"')}</div><label>Tình trạng<textarea name="condition" maxlength="2000">${esc(log?.condition)}</textarea></label><label>Sự cố<textarea name="issue" maxlength="2000">${esc(log?.issue)}</textarea></label><button>Gửi nhật ký thiết bị</button></form>`);
      const form=section.querySelector('form');form.elements.used.value=log?.used===false?'false':'true';
      const toggle=()=>{const used=form.elements.used.value==='true';section.querySelector('#eq-log-actual').hidden=!used;section.querySelectorAll('#eq-log-actual input').forEach(i=>i.required=used);};form.elements.used.addEventListener('change',toggle);toggle();
      if(log?.actualStart){const local=v=>new Date(new Date(v).getTime()+7*3600000).toISOString().slice(0,16);form.elements.actualStart.value=local(log.actualStart);form.elements.actualEnd.value=local(log.actualEnd);}
      form.addEventListener('submit',e=>{e.preventDefault();const d=Object.fromEntries(new FormData(form));const used=d.used==='true';const data={used,actualStart:used?instant(d.actualStart):null,actualEnd:used?instant(d.actualEnd):null,quantity:used?Number(d.quantity):null,condition:d.condition,issue:d.issue,version:log?.version??null};run(async()=>{await mutation(`equipment-log.save:${r.id}`,key=>api.keyed(`/equipment-loans/${r.id}/log`,data,key),data);await detail(r.id);},e.submitter);});
    } else if(review&&log?.status==='submitted') {
      section.insertAdjacentHTML('beforeend','<form><label>Ý kiến<textarea name="note" maxlength="2000"></textarea></label><div class="actions"><button value="confirmed">Xác nhận nhật ký</button><button value="needs_changes">Yêu cầu bổ sung nhật ký</button></div></form>');
      section.querySelector('form').addEventListener('submit',e=>{e.preventDefault();const data={version:log.version,result:e.submitter.value,note:new FormData(e.target).get('note')};run(async()=>{await mutation(`equipment-log.review:${r.id}`,key=>api.keyed(`/equipment-loans/${r.id}/log/review`,data,key),data);await detail(r.id);},e.submitter);});
    }
  }
  root.addEventListener('click',e=>{
    const b=e.target.closest('[data-equipment-ui] button[type=button]');if(!b)return;
    run(async()=>{
      if(b.dataset.eqBook)await booking(b.dataset.eqBook);
      else if(b.dataset.eqAsset)await asset(b.dataset.eqAsset);
      else if(b.dataset.eqLoan)await detail(b.dataset.eqLoan);
      else if(b.dataset.eqMaintenance)await maintenanceDetail(b.dataset.eqMaintenance);
      else if(b.dataset.eqInventory)await inventoryDetail(b.dataset.eqInventory);
      else if(b.dataset.eqPage)await render(mode,Number(b.dataset.eqPage));
      else if(b.hasAttribute('data-eq-back'))await render(mode,page);
    },b);
  });
  return {render};
}
