import {mountFinanceControls} from './finance-controls.mjs?v=frontend-complete-20261104.3';
import {mountAssetLifecycle} from './asset-lifecycle.mjs?v=frontend-complete-20261104.3';
import {api,keyed,can,requireSession,markConnected,day,departmentsFor,canForDepartment} from './server-session.mjs';
import {mountReports} from './server-reports.mjs?v=frontend-complete-20261104.3';
import {mountVoucherFiles} from './voucher-files.mjs?v=frontend-complete-20261104.3';

await requireSession('budget.read');
const $=s=>document.querySelector(s),esc=v=>String(v??'').replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
const money=v=>Number(v).toLocaleString('vi-VN')+' ₫';
const cats={general:'Ngân sách chung',maintenance:'Bảo trì',equipment:'Thiết bị',teaching:'Giảng dạy',operations:'Vận hành'};
const isKhtc=location.pathname.endsWith('khtc.html');
if(isKhtc&&!can('budget.appraise')){location.replace('login.html');throw Error('Cần quyền thẩm định KHTC.');}
const labels={reviewing:'Chờ KHTC thẩm định',returned:'Trả bổ sung',draft:'Nháp',pending:'Chờ duyệt',approved:'Đã duyệt',rejected:'Từ chối',part_paid:'Thanh toán một phần',paid:'Đã chi'};
const statusBadge=status=>`<span class="badge ${esc(status)}">${esc(labels[status]||status)}</span>`;
const allMenus=[['overview','Tổng quan tài chính'],['estimates','Dự toán'],['controls','Điều chỉnh và khóa sổ'],['budgets','Ngân sách'],['queue','Hàng chờ thanh toán'],['transactions','Sổ thu – chi'],['reports','Báo cáo và xuất tệp'],...(can('budget.approve')?[['lifecycle','Nguyên giá và thanh lý']]:[])];
const menus=isKhtc?[['overview','Phòng KHTC'],['estimates','Thẩm định dự toán'],['reports','Báo cáo tài chính']]:allMenus;
let proposals=[],payments=[],entries=[],reversals=[],departments=[],balances=[],received=0,page=1,total=0,year=new Date().getFullYear(),unit='',category='',editing=null,loadRevision=0;
const dep=id=>departments.find(d=>d.id===id)?.name||id;
const options=(rows,value)=>rows.map(([id,name])=>`<option value="${esc(id)}" ${id===String(value)?'selected':''}>${esc(name)}</option>`).join('');
const button=(label,action,id='',style='secondary')=>`<button type="button" class="btn ${style}" data-finance="${action}" data-id="${esc(id)}">${label}</button>`;
const table=(heads,rows)=>rows.length?`<div class="table-wrap"><table><thead><tr>${heads.map(h=>`<th>${h}</th>`).join('')}</tr></thead><tbody>${rows.map(r=>`<tr>${r.map(v=>`<td>${v}</td>`).join('')}</tr>`).join('')}</tbody></table></div>`:'<div class="empty"><h3>Chưa có dữ liệu phù hợp</h3><p>Chọn năm, đơn vị hoặc tạo hồ sơ mới.</p></div>';
let fieldSequence=0;
const field=(label,name,type='text',value='',extra='')=>{const id=`finance-${name}-${++fieldSequence}`;return `<div class="field"><label for="${id}">${label}</label><input id="${id}" name="${name}" type="${type}" value="${esc(value)}" required ${extra}></div>`;};
const select=(label,name,rows,value)=>`<div class="field"><label for="finance-${name}">${label}</label><select id="finance-${name}" name="${name}">${options(rows,value)}</select></div>`;
const unitField=(value,permission='transactions.write')=>select('Đơn vị','departmentId',departmentsFor(permission,departments).filter(d=>d.active).map(d=>[d.id,d.name]),value||unit);
const categoryField=value=>select('Khoản mục ngân sách','category',Object.entries(cats),value||category||'general');
const inScope=r=>r.year===Number(year)&&(!unit||r.departmentId===unit)&&(!category||!r.category||r.category===category);
const toast=message=>{$('#toast').textContent=message;$('#toast').classList.add('show');setTimeout(()=>$('#toast').classList.remove('show'),4500);};
async function load(){
 const revision=++loadRevision;
 const route=location.hash.slice(2)||'overview';
 const q=new URLSearchParams({year,page,pageSize:20,...(unit?{departmentId:unit}:{}),...(category&&route!=='queue'?{category}: {})});
 const path={estimates:'proposals',queue:'payments',transactions:'entries'}[route];
 const [deps,summary,list]=await Promise.all([api.departments(),api.request('/finance/summary?'+q),path?api.request('/finance/'+path+'?'+q):null]);
 const currentEntries=route==='transactions'?list.items:[];
 const currentReversals=currentEntries.length?(await api.request('/finance/reversals?'+new URLSearchParams({pageSize:100,entryIds:currentEntries.map(e=>e.id).join(',')}))).items:[];
 if(revision!==loadRevision)return false;
 departments=deps;balances=summary.balances;received=summary.received;total=list?.total||0;
 proposals=route==='estimates'?list.items:[];payments=route==='queue'?list.items:[];entries=currentEntries;reversals=currentReversals;
 return true;
}
async function paymentVouchers(id,voucherPage=1){
 const result=await api.request('/finance/entries?'+new URLSearchParams({paymentRequestId:id,page:voucherPage,pageSize:20}));
 modal('Các chứng từ thanh toán',table(['NGÀY','SỐ CHỨNG TỪ','SỐ TIỀN','CHI TIẾT'],result.items.map(e=>[esc(e.date),esc(e.voucher),money(e.amount),button('Mở chứng từ','voucher',e.id)]))+`<div class="actions"><button type="button" class="btn" data-voucher-page="${voucherPage-1}" ${voucherPage<=1?'disabled':''}>Trang trước</button><span>${result.total} chứng từ · Trang ${voucherPage}</span><button type="button" class="btn" data-voucher-page="${voucherPage+1}" ${voucherPage*20>=result.total?'disabled':''}>Trang sau</button></div>`);
 $('#dialog-body').querySelectorAll('[data-voucher-page]').forEach(b=>b.onclick=()=>paymentVouchers(id,Number(b.dataset.voucherPage)).catch(e=>toast(e.message)));
}
function render(){const route=location.hash.slice(2)||'overview',title=menus.find(m=>m[0]===route)?.[1]||'Tài chính';
 $('#nav').innerHTML=menus.map(([r,l])=>`<a class="nav-link ${route===r?'active':''}" href="#/${r}"><span>▦</span>${l}</a>`).join('');$('#breadcrumb').textContent=title;
 const actions=isKhtc?'':(departmentsFor('budget.write',departments).some(d=>d.active)?button('Lập dự toán','draft','','primary'):'')+(departmentsFor('transactions.write',departments).some(d=>d.active)?button('Ghi thu','receipt')+button('Ghi chi','expense')+button('Yêu cầu chi','payment')+button('Nhập chi phí bảo trì cũ','import'):'');
 const filters=`<form id="finance-filters" class="filters">${field('Năm tài chính','year','number',year,'min="2000" max="2100"')}${select('Đơn vị','unit',[['','Tất cả đơn vị có quyền'],...departmentsFor('budget.read',departments).map(d=>[d.id,d.name])],unit)}${route==='queue'?'':select('Khoản mục','category',[['','Tất cả khoản mục'],...Object.entries(cats)],category)}<button class="btn secondary">Áp dụng</button></form>`;
 const budgets=()=>table(['ĐƠN VỊ / KHOẢN MỤC','ĐƯỢC CẤP','ĐÃ CHI','KHẢ DỤNG'],balances.map(b=>[`${esc(dep(b.departmentId))}<small>${esc(cats[b.category])}</small>`,money(b.allocated),money(b.spent),`<strong>${money(b.available)}</strong>`]));
 let content='';
 if(route==='overview'){const sum=k=>balances.reduce((s,b)=>s+b[k],0);content=`<section class="hero"><div class="hero-copy"><div class="hero-tag">TÀI CHÍNH · DỮ LIỆU MÁY CHỦ</div><h2>${isKhtc?'Phòng Kế hoạch – Tài chính':'Ngân sách rõ ràng, chứng từ liên thông.'}</h2><p>${isKhtc?'Kiểm tra hồ sơ, căn cứ, định mức và khả năng cân đối nguồn; ghi ý kiến thẩm định, trả bổ sung hoặc trình phê duyệt.':'Dự toán được duyệt cấp ngân sách. Yêu cầu chờ chưa tính chi; khoản thu không tự tăng ngân sách.'}</p><div class="hero-actions">${actions}</div></div></section><section class="stats">${[['Được cấp',sum('allocated')],['Đã chi',sum('spent')],['Khả dụng',sum('available')],['Thu ghi nhận',received]].map(([l,v])=>`<article class="stat"><div class="stat-top">${l}</div><div class="stat-value">${money(v)}</div><div class="stat-foot">Năm ${year}</div></article>`).join('')}</section><section class="panel">${budgets()}</section>`;}
 else if(route==='estimates')content=table(['DỰ TOÁN','ĐƠN VỊ / KHOẢN MỤC','SỐ TIỀN','TRẠNG THÁI','THAO TÁC'],proposals.filter(inScope).map(p=>[`<strong>${esc(p.title)}</strong>`,`${esc(dep(p.departmentId))}<small>${esc(cats[p.category])}</small>`,money(p.amount),statusBadge(p.status),button('Xem','proposal',p.id)+(p.fundingReceiptId?button('Đã nhận tiền · Xem phiếu thu','funding-voucher',p.fundingReceiptId):p.status==='approved'&&canForDepartment('transactions.write',p.departmentId)?button('Xác nhận nhận tiền','receive-funding',p.id):'')+(!isKhtc&&canForDepartment('budget.write',p.departmentId)&&['draft','returned'].includes(p.status)?button('Sửa','edit',p.id):'')+(!isKhtc&&canForDepartment('budget.write',p.departmentId)&&p.status==='draft'?button('Gửi KHTC thẩm định','submit',p.id):'')+(isKhtc&&canForDepartment('budget.appraise',p.departmentId)&&['reviewing','pending'].includes(p.status)?button('Thẩm định / trả bổ sung','appraise',p.id):'')+(!isKhtc&&canForDepartment('budget.approve',p.departmentId)&&p.status==='pending'?button('Xét duyệt','review',p.id):'')]));
 else if(route==='controls')content='<div id="finance-controls"></div>';
 else if(route==='lifecycle'&&can('budget.approve'))content='<div id="asset-lifecycle"></div>';
 else if(route==='budgets')content=budgets();
 else if(route==='reports')content='<div id="server-reports"></div>';
 else if(route==='queue')content=`<p class="compact-note">Có thể thanh toán nhiều đợt, tối đa số còn lại; mỗi đợt có chứng từ riêng. Khoản đã đảo không tính vào số đã trả.</p>${table(['NỘI DUNG / NGUỒN','ĐƠN VỊ','SỐ TIỀN','TRẠNG THÁI','CHỨNG TỪ'],payments.filter(inScope).map(p=>[`${esc(p.content)}${p.maintenanceId?`<small>Bảo trì: ${esc(p.maintenanceId)}</small>`:''}`,esc(dep(p.departmentId)),`${money(p.amount)}<small>Đã trả: ${money(p.paid)} · Còn: ${money(p.remaining)}</small>`,statusBadge(p.status),button('Xem các chứng từ','payment-vouchers',p.id)+(canForDepartment('transactions.write',p.departmentId)&&p.remaining>0?button('Thanh toán','pay',p.id):'')]))}`;
 else content=table(['NGÀY / CHỨNG TỪ','LOẠI','ĐƠN VỊ / KHOẢN MỤC','NỘI DUNG','SỐ TIỀN','HỒ SƠ'],entries.filter(inScope).map(e=>[`${esc(e.date)}<small>${esc(e.voucher)}</small>`,e.kind==='receipt'?'Thu':'Chi',`${esc(dep(e.departmentId))}<small>${esc(cats[e.category])}</small>`,esc(e.content),money(e.amount),button('Xem chứng từ','voucher',e.id)+(reversals.some(r=>r.entryId===e.id)?'<small>Đã đảo</small>':canForDepartment('transactions.write',e.departmentId)?button('Đảo chứng từ','reverse',e.id):'')]));
 $('#main').innerHTML=`<div class="page-heading"><div><p class="eyebrow">HPUNI · ${year}</p><h1>${title}</h1></div><div class="heading-actions">${route==='overview'?'':actions}</div></div>${filters}<nav class="form-actions" aria-label="Các không gian tài chính"><a class="btn secondary" href="accounting.html#/estimates">Kế toán · Lập dự toán</a><a class="btn secondary" href="khtc.html#/estimates">KHTC · Thẩm định</a><a class="btn secondary" href="executive.html#/estimates">Người có thẩm quyền · Phê duyệt</a></nav><p class="compact-note">Kế toán lập và sửa → KHTC thẩm định, trình duyệt → Hiệu trưởng/người được ủy quyền quyết định. Người phê duyệt trả bổ sung kèm ý kiến, không sửa dự toán.</p><section class="panel">${content}</section>${['estimates','queue','transactions'].includes(route)?`<div class="actions"><button class="btn secondary" data-finance="previous" ${page<=1?'disabled':''}>Trang trước</button><span>${total} hồ sơ · Trang ${page}/${Math.max(1,Math.ceil(total/20))}</span><button class="btn secondary" data-finance="next" ${page*20>=total?'disabled':''}>Trang sau</button></div>`:''}`;
 if(route==='controls')mountFinanceControls($('#finance-controls'),{year,departmentId:unit,category,onChanged:async selectedDepartment=>{unit=selectedDepartment;if(await load())render();}}).catch(e=>toast(e.message));
 if(route==='lifecycle'&&can('budget.approve'))mountAssetLifecycle($('#asset-lifecycle')).catch(e=>toast(e.message));
 if(route==='reports')mountReports($('#server-reports'),{year,departmentId:unit,category,filters:false});
}
function modal(title,body){$('#dialog-title').textContent=title;$('#dialog-body').innerHTML=body;$('#dialog').showModal();}
function form(kind,body){return `<form id="finance-form" data-kind="${kind}"><div class="form-grid">${body}</div><p id="finance-error" class="error-banner" role="alert"></p><div class="form-actions">${button('Hủy','close')}<button class="btn primary" type="submit">Lưu</button></div></form>`;}
function proposalForm(p){editing=p||null;modal(p?'Sửa dự toán':'Lập dự toán',form('draft',unitField(p?.departmentId,'budget.write')+field('Năm','year','number',p?.year||year,'min="2000" max="2100"')+categoryField(p?.category)+field('Tên dự toán','title','text',p?.title||'','maxlength="150"')+field('Mục đích','purpose','text',p?.purpose||'','maxlength="2000"')+`<div class="field" style="grid-column:1/-1"><label>Khoản chi tiết</label><div id="finance-lines">${(p?.lines||[{name:'',quantity:1,price:1}]).map(lineForm).join('')}</div>${button('Thêm khoản chi tiết','add-line')}</div>`));}
function lineForm(l){return `<div class="finance-line">${field('Tên khoản','lineName','text',l.name,'maxlength="150"')}${field('Số lượng','quantity','number',l.quantity,'min="1" max="1000000"')}${field('Đơn giá (đồng)','price','number',l.price,'min="1" max="1000000000000" step="1"')}${button('Bỏ dòng','remove-line')}</div>`;}
function fundingForm(p){
 editing=p;const today=day(new Date()),date=today.startsWith(p.year+'-')?today:`${p.year}-01-01`;
 modal('Xác nhận nhận tiền dự toán',form('funding',`<p class="full"><strong>${esc(p.title)}</strong><br>${esc(dep(p.departmentId))} · Năm ${p.year}<br>Số tiền nhận: <strong>${money(p.amount)}</strong></p><p class="full">Chỉ xác nhận sau khi đã thực nhận đủ tiền. Hệ thống sẽ lập phiếu thu vào Sổ thu – chi, giữ nguyên ngân sách đã cấp khi duyệt.</p>`+field('Số phiếu thu','voucher','text','','maxlength="100"')+field('Ngày nhận tiền','date','date',date,`min="${p.year}-01-01" max="${p.year}-12-31"`)));
}
function entryForm(kind,p){
 editing=p||null;
 const targetYear=p?.year||Number(year),today=day(new Date()),defaultDate=today.startsWith(String(targetYear)+'-')?today:`${targetYear}-01-01`;
 modal(p?'Thanh toán yêu cầu':kind==='receipt'?'Ghi nhận khoản thu':'Ghi nhận khoản chi',form(kind,(p?`<p style="grid-column:1/-1">${esc(dep(p.departmentId))} · ${p.year} · Còn <strong>${money(p.remaining)}</strong><br>${esc(p.content)}</p>`+field('Số tiền đợt này (đồng)','amount','number',p.remaining,`min="1" max="${p.remaining}" step="1"`):unitField()+field('Năm','year','number',year,'min="2000" max="2100"')+field('Số tiền (đồng)','amount','number','','min="1" max="1000000000000" step="1"'))+categoryField(p?.maintenanceId?'maintenance':undefined)+field('Số chứng từ','voucher','text','','maxlength="100"')+field('Ngày chứng từ','date','date',defaultDate,p?`min="${targetYear}-01-01" max="${targetYear}-12-31"`:'')+field('Nội dung','content','text',p?.content||'','maxlength="2000"')));
}
async function voucher(e){const p=e.paymentRequestId?(payments.find(p=>p.id===e.paymentRequestId)||await api.request('/finance/payments/'+encodeURIComponent(e.paymentRequestId))):null;modal('Chứng từ '+e.voucher,`<dl class="finance-voucher"><dt>Loại</dt><dd>${e.kind==='receipt'?'Thu':'Chi'}</dd><dt>Ngày</dt><dd>${esc(e.date)}</dd><dt>Đơn vị</dt><dd>${esc(dep(e.departmentId))}</dd><dt>Khoản mục</dt><dd>${esc(cats[e.category])}</dd><dt>Số tiền</dt><dd><strong>${money(e.amount)}</strong></dd><dt>Nội dung</dt><dd>${esc(e.content)}</dd>${e.fundingProposalId?`<dt>Dự toán nhận kinh phí</dt><dd>${esc(e.fundingProposalId)}</dd>`:''}${p?`<dt>Yêu cầu thanh toán</dt><dd>${esc(p.id)}</dd><dt>Hồ sơ bảo trì</dt><dd>${esc(p.maintenanceId||'Không có')}</dd>`:''}</dl><p>Chứng từ nội bộ ghi nhận trong hệ thống. Tệp đính kèm nằm bên dưới; chưa áp dụng mẫu in kế toán chính thức.</p>${button('Đóng','close')}`);}
document.addEventListener('click',async event=>{const b=event.target.closest('[data-finance]');if(!b)return;const action=b.dataset.finance,id=b.dataset.id;try{
 if(action==='close')$('#dialog').close();
 else if(action==='draft')proposalForm();
 else if(action==='edit')proposalForm(proposals.find(p=>p.id===id));
 else if(action==='add-line')$('#finance-lines').insertAdjacentHTML('beforeend',lineForm({name:'',quantity:1,price:1}));
 else if(action==='remove-line')b.closest('.finance-line').remove();
 else if(action==='receipt'||action==='expense')entryForm(action);
 else if(action==='pay')entryForm('expense',payments.find(p=>p.id===id));
 else if(action==='receive-funding')fundingForm(proposals.find(p=>p.id===id));
 else if(action==='funding-voucher'){const e=await api.request('/finance/entries/'+encodeURIComponent(id));await voucher(e);const files=document.createElement('section');$('#dialog-body').insertBefore(files,$('#dialog-body').lastElementChild);mountVoucherFiles(files,e.id);}
 else if(action==='previous'||action==='next'){page+=action==='next'?1:-1;$('#main').inert=true;try{if(await load())render();}finally{$('#main').inert=false;}}
 else if(action==='payment-vouchers')await paymentVouchers(id);
 else if(action==='voucher'){const e=entries.find(e=>e.id===id)||await api.request('/finance/entries/'+encodeURIComponent(id));await voucher(e);const files=document.createElement('section');$('#dialog-body').insertBefore(files,$('#dialog-body').lastElementChild);mountVoucherFiles(files,e.id);}
 else if(action==='reverse'){editing=entries.find(e=>e.id===id);modal('Đảo chứng từ '+editing.voucher,form('reverse',`<p>Chứng từ gốc được giữ lại. Số tiền sẽ được loại khỏi tổng thu/chi và số đã thanh toán.</p>`+field('Lý do đảo chứng từ','reason','text','','maxlength="2000"')));}
 else if(action==='import'){editing=null;modal('Chuyển chi phí bảo trì cũ vào hàng chờ',form('import',field('Mã hồ sơ bảo trì đã hoàn tất','maintenanceId','text')+field('Năm tài chính ghi nhận chi phí','year','number',year,'min="2000" max="2100"')));}
 else if(action==='payment'){editing=null;modal('Yêu cầu chi chờ xử lý',form('payment',unitField()+field('Năm','year','number',year,'min="2000" max="2100"')+field('Số tiền (đồng)','amount','number','','min="1" max="1000000000000" step="1"')+field('Nội dung','content','text','','maxlength="2000"')));}
 else if(action==='proposal'||action==='review'||action==='appraise'){editing=proposals.find(p=>p.id===id);modal(action==='appraise'?'KHTC thẩm định dự toán':action==='review'?'Xét duyệt dự toán':'Chi tiết dự toán',`<h3>${esc(editing.title)}</h3><p>${esc(editing.purpose)}</p>${table(['KHOẢN','SỐ LƯỢNG','ĐƠN GIÁ'],editing.lines.map(l=>[esc(l.name),l.quantity,money(l.price)]))}<p>Tổng: <strong>${money(editing.amount)}</strong> · ${esc(labels[editing.status])}</p><p>${esc(editing.reviewNote)}</p>${editing.appraisalNote?`<p><strong>Ý kiến thẩm định KHTC:</strong> ${esc(editing.appraisalNote)}</p>`:''}${action==='appraise'?form('review',field('Ý kiến thẩm định / lý do trả bổ sung','note','text','','maxlength="2000"')+select('Kết quả','decision',[['appraise','Đủ điều kiện, trình phê duyệt'],['return-review','Trả người lập bổ sung']],'appraise')):action==='review'?form('review',field('Ý kiến xét duyệt','note','text','','maxlength="2000"')+select('Quyết định','decision',[['approve','Phê duyệt'],['reject','Không phê duyệt'],['return','Trả bổ sung']],'approve')):button('Đóng','close')}`);}
 else if(action==='submit'){b.disabled=true;const p=proposals.find(p=>p.id===id);await keyed(`/finance/proposals/${encodeURIComponent(id)}/submit`,{version:p.version});if(await load())render();toast('Đã gửi dự toán.');}
 }catch(e){toast(e.message);}finally{b.disabled=false;}});
document.addEventListener('submit',async event=>{const formEl=event.target;if(!['finance-form','finance-filters'].includes(formEl.id))return;event.preventDefault();const data=new FormData(formEl),v=Object.fromEntries(data);
 if(formEl.id==='finance-filters'){page=1;year=Number(v.year);unit=v.unit;category=v.category||'';$('#main').inert=true;$('#main').setAttribute('aria-busy','true');try{if(await load())render();}catch(e){toast(e.message);}finally{$('#main').inert=false;$('#main').setAttribute('aria-busy','false');}return;}
 const submit=formEl.querySelector('[type=submit]');submit.disabled=true;try{
  const kind=formEl.dataset.kind;
  if(kind==='funding'){await keyed('/finance/proposals/'+encodeURIComponent(editing.id)+'/receive-funding',{version:editing.version,voucher:v.voucher,date:v.date});$('#dialog').close();if(await load())render();toast('Đã ghi khoản thu nhận kinh phí vào Sổ thu – chi.');return;}
  if(kind==='draft'){const input={departmentId:v.departmentId,year:Number(v.year),category:v.category,title:v.title,purpose:v.purpose,lines:data.getAll('lineName').map((name,i)=>({name,quantity:Number(data.getAll('quantity')[i]),price:Number(data.getAll('price')[i])})),version:editing?.version||null};
   if(editing){formEl.retryKey??=crypto.randomUUID();await api.request('/finance/proposals/'+encodeURIComponent(editing.id),{method:'PUT',data:input,idempotencyKey:formEl.retryKey});}else await keyed('/finance/proposals',input);
  }else if(kind==='reverse')await keyed('/finance/entries/'+encodeURIComponent(editing.id)+'/reverse',{reason:v.reason});
  else if(kind==='import')await keyed('/finance/payments/from-maintenance/'+encodeURIComponent(v.maintenanceId),{year:Number(v.year)});
  else if(kind==='review')await keyed(`/finance/proposals/${encodeURIComponent(editing.id)}/${v.decision}`,{version:editing.version,note:v.note});
  else if(kind==='payment')await keyed('/finance/payments',{departmentId:v.departmentId,year:Number(v.year),amount:Number(v.amount),content:v.content});
  else await keyed('/finance/entries/'+kind,{departmentId:editing?.departmentId||v.departmentId,year:editing?.year||Number(v.year),amount:Number(v.amount),category:v.category,voucher:v.voucher,date:v.date,content:v.content,paymentRequestId:editing?.id||null,version:editing?.version||null});
  $('#dialog').close();if(await load())render();toast('Đã lưu vào SQL Server.');
 }catch(e){$('#finance-error').textContent=e.message;}finally{submit.disabled=false;}
});
document.addEventListener('click',e=>{const a=e.target.closest('[data-action]')?.dataset.action;if(a==='menu'){$('#sidebar').classList.toggle('open');$('#backdrop').classList.toggle('show');}if(a==='close')$('#dialog').close();if(a==='notifications'||a==='help')toast('Ngân sách được cấp sau phê duyệt; chọn Hàng chờ thanh toán để xử lý chi phí bảo trì.');});
$('#backdrop').onclick=()=>{$('#sidebar').classList.remove('open');$('#backdrop').classList.remove('show');};
window.addEventListener('hashchange',async()=>{page=1;$('#dialog').close();$('#sidebar').classList.remove('open');$('#backdrop').classList.remove('show');$('#main').inert=true;try{if(await load())render();}catch(e){toast(e.message);}finally{$('#main').inert=false;}});
document.querySelectorAll('[data-icon]').forEach(e=>e.textContent=({menu:'☰',close:'×',bell:'♧',chevron:'⌄',building:'▦',shield:'◇'})[e.dataset.icon]||'↗');
document.querySelector('.profile small').textContent='Phiên máy chủ · Quyền theo tài khoản';
const style=document.createElement('style');style.textContent='.finance-line{display:grid;grid-template-columns:2fr 1fr 1fr auto;gap:12px;align-items:end;margin-bottom:16px}.finance-voucher dd{margin:0 0 12px;overflow-wrap:anywhere}.finance-voucher dt{color:#64748b;font-size:12px}#finance-filters{display:flex;flex-wrap:wrap;align-items:end;gap:16px;margin-bottom:24px}#finance-filters .field{min-width:140px}#main .panel{overflow:hidden}#main td small{display:block;overflow-wrap:anywhere}#main .heading-actions{display:flex;flex-wrap:wrap;gap:8px}#finance-lines .field input{min-width:0;width:100%}@media(max-width:640px){.finance-line{grid-template-columns:1fr 1fr}.finance-line>.field:first-child{grid-column:1/-1}#finance-filters .field{flex:1;min-width:120px}}';document.head.append(style);
await load();markConnected();render();
if(location.pathname.endsWith('payment-voucher.html')){const id=new URLSearchParams(location.search).get('id');const e=entries.find(e=>e.id===id);if(e)await voucher(e);else toast('Không tìm thấy chứng từ trong phạm vi tài khoản.');}
