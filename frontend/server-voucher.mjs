import {api,requireSession} from './server-session.mjs';
import {mountVoucherFiles} from './voucher-files.mjs?v=frontend-complete-20261104.3';
await requireSession('budget.read');
const main=document.querySelector('#voucher'),esc=v=>String(v??'').replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
try{
 const id=new URLSearchParams(location.search).get('id');if(!id)throw Error('Thiếu mã chứng từ.');
 const [e,deps]=await Promise.all([api.request('/finance/entries/'+encodeURIComponent(id)),api.departments()]);
 const [p,reversals]=await Promise.all([e.paymentRequestId?api.request('/finance/payments/'+encodeURIComponent(e.paymentRequestId)):null,api.request('/finance/reversals?'+new URLSearchParams({entryIds:e.id,page:1,pageSize:1}))]);
 const reversal=reversals.items.find(r=>r.entryId===e.id);
 const category=({general:'Ngân sách chung',maintenance:'Bảo trì',equipment:'Thiết bị',teaching:'Giảng dạy',operations:'Vận hành'})[e.category]||e.category;
 main.innerHTML=`<article class="voucher"><h1>${e.kind==='receipt'?'PHIẾU THU':'PHIẾU CHI'} NỘI BỘ</h1><p>Số: <strong>${esc(e.voucher)}</strong> · Ngày ${esc(e.date)}</p><p>Đơn vị: ${esc(deps.find(d=>d.id===e.departmentId)?.name||e.departmentId)}</p><p>Nội dung: ${esc(e.content)}</p><h2>${Number(e.amount).toLocaleString('vi-VN')} đồng</h2><p>Khoản mục: ${esc(category)} · Năm ${e.year}</p>${e.fundingProposalId?`<p>Dự toán nhận kinh phí: ${esc(e.fundingProposalId)}</p>`:''}${p?`<p>Yêu cầu thanh toán: ${esc(p.content)} · <a href="accounting.html#/queue">${esc(p.id)}</a></p><p>Bảo trì: ${esc(p.maintenanceId||'Không có')}</p>`:''}<hr><p>Bản ghi từ hệ thống HPUNI. Tệp đính kèm nằm bên dưới; chưa áp dụng mẫu chứng từ kế toán chính thức.</p></article>`;
 if(reversal){const status=document.createElement('p');status.textContent='ĐÃ ĐẢO · '+reversal.reason;status.style.color='#b45309';main.querySelector('article').prepend(status);}
 const b=document.querySelector('#print');b.disabled=false;b.textContent='In bản ghi nội bộ';b.onclick=()=>window.print();
 const attachments=document.createElement('section');attachments.className='voucher-attachments';main.append(attachments);await mountVoucherFiles(attachments,e.id);
 const style=document.createElement('style');style.textContent='.voucher-attachments{max-width:800px;margin:24px auto;padding:24px;background:white}.voucher-attachments input{display:block;margin:12px 0;max-width:100%}@media print{.voucher-attachments{display:none}}';document.head.append(style);
}catch(e){main.textContent=e.message;}
