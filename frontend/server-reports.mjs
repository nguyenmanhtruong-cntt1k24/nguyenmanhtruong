import {api,can} from './server-session.mjs';
import {mountReportDetails} from './report-details.mjs?v=frontend-complete-20261104.3';
const esc=v=>String(v??'').replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
const money=v=>Number(v).toLocaleString('vi-VN')+' ₫';
const cats={general:'Ngân sách chung',maintenance:'Bảo trì',equipment:'Thiết bị',teaching:'Giảng dạy',operations:'Vận hành'};
const table=(heads,rows)=>rows.length?`<div class="table-wrap"><table><thead><tr>${heads.map(h=>`<th>${h}</th>`).join('')}</tr></thead><tbody>${rows.map(r=>`<tr>${r.map(v=>`<td>${v}</td>`).join('')}</tr>`).join('')}</tbody></table></div>`:'<p class="empty">Chưa có dữ liệu trong bộ lọc và phạm vi tài khoản.</p>';
const query=o=>new URLSearchParams(Object.entries(o).filter(([,v])=>v!==''&&v!=null));
const revisions=new WeakMap();
export async function mountReports(container,{year=new Date().getFullYear(),departmentId='',category='',finance=true,filters=true}={}){
 if(!container)return;
 const revision=(revisions.get(container)||0)+1;revisions.set(container,revision);
 const current=()=>container.isConnected&&revisions.get(container)===revision;
 container.style.padding='20px';
 container.innerHTML='<p>Đang tải báo cáo từ máy chủ…</p>';
 try{
  const deps=filters?await api.departments():[];
  const q=query({year,departmentId,category:finance?category:null}),rq=query({year,departmentId});
  const canRead=can('reports.read'),canExport=can('reports.export'),hasFinance=finance&&can('budget.read');
  const [f,r]=canRead?await Promise.all([hasFinance?api.request('/reports/finance?'+q):null,api.request('/reports/resources?'+rq)]):[null,null];
  if(!current())return;
  const tools=`<div style="display:flex;gap:12px;flex-wrap:wrap;margin:16px 0">${canExport&&hasFinance?`<a class="btn secondary" href="/api/v1/reports/finance.csv?${esc(q)}" download>Xuất CSV tài chính</a>`:''}${canExport?`<a class="btn secondary" href="/api/v1/reports/resources.csv?${esc(rq)}" download>Xuất CSV tài sản/phòng</a>`:''}</div>`;
  const filter=filters?`<form data-report-filter style="display:flex;align-items:end;gap:12px;flex-wrap:wrap;margin:16px 0"><label>Năm lượt sử dụng<input name="year" type="number" min="2000" max="2100" required value="${year}"></label><label>Đơn vị<select name="departmentId"><option value="">Tất cả đơn vị có quyền</option>${deps.map(d=>`<option value="${esc(d.id)}" ${departmentId===d.id?'selected':''}>${esc(d.name)}</option>`).join('')}</select></label><button class="btn primary">Xem báo cáo</button></form>`:'';
  let body='';
  if(f){body+=`<h2>Tài chính năm ${year}</h2><p>Ngân sách và sổ thu/chi${category?' · '+esc(cats[category]):''}. Chi bảo trì đã ghi là một phần của tổng chi, không cộng thêm lần nữa.</p>${table(['ĐƠN VỊ / KHOẢN MỤC','ĐƯỢC CẤP','THU','ĐÃ CHI','KHẢ DỤNG','TRONG ĐÓ: BẢO TRÌ'],f.rows.map(x=>[`${esc(x.departmentName)}<small style="display:block">${esc(cats[x.category])}</small>`,money(x.allocated),money(x.received),money(x.spent),money(x.available),money(x.maintenanceSpent)]))}<h3>Yêu cầu chưa gán khoản mục</h3><p>Hàng chờ theo đơn vị/năm, chưa trừ ngân sách và không áp dụng bộ lọc khoản mục. Chi phí cũ chưa chuyển hàng chờ được thống kê mọi năm vì chưa có năm ghi nhận.</p>${table(['ĐƠN VỊ','TỔNG CHỜ CHI','TRONG ĐÓ: BẢO TRÌ','BẢO TRÌ CHƯA CHUYỂN (MỌI NĂM)'],f.pendingCosts.map(x=>[esc(x.departmentName),money(x.pending),money(x.maintenancePending),money(x.unqueuedMaintenanceAllTime)]))}`;}
  if(r)body+=`<h2>Tài sản và phòng</h2><p>Số lượng là hiện tại, không phải tồn tại cuối năm ${year}. Lượt mượn/đặt phòng tính theo thời điểm bắt đầu trong năm (giờ Việt Nam), gồm mọi trạng thái, thống kê theo đơn vị sở hữu nguồn lực. Số tốt bao gồm thiết bị đang mượn và bảo trì; không phải số có thể đặt ngay. Nguyên giá lịch sử chỉ cộng khi mọi nhóm trong đơn vị đã kê khai; không tự giảm sau thanh lý và không phải giá trị còn lại.</p>${table(['ĐƠN VỊ','NHÓM','SỞ HỮU','TỐT','HỎNG','MẤT','ĐANG BÀN GIAO/MƯỢN','PHIẾU QUÁ HẠN','LƯỢT MƯỢN TRONG NĂM','BẢO TRÌ LÊN LỊCH / ĐANG CHẠY','PHÒNG HOẠT ĐỘNG','LƯỢT PHÒNG / ĐÃ DUYỆT','NGUYÊN GIÁ LỊCH SỬ / KÊ KHAI','GIÁ TRỊ CÒN LẠI'],r.rows.map(x=>[esc(x.departmentName),x.assetGroups,x.owned,x.good,x.damaged,x.lost,x.outstanding,x.overdueLoans,x.loansStartingInYear,`${x.maintenancePlanned} / ${x.maintenanceRunning}`,x.activeRooms,`${x.roomRequestsStartingInYear} / ${x.approvedRoomRequestsStartingInYear}`,`${x.historicalDeclaredValue==null?'Chưa đủ kê khai':money(x.historicalDeclaredValue)}<small style="display:block">${x.acquisitionProfiles} / ${x.assetGroups} nhóm đã kê khai</small>`,x.purchaseValue==null?'Chưa xác định':money(x.purchaseValue)]))}<p style="font-size:12px;color:#64748b">Lấy dữ liệu lúc ${esc(new Date(r.generatedAt).toLocaleString('vi-VN'))} · SQL Server</p>`;
  container.innerHTML=filter+tools+(canRead?body:canExport?'<p>Bạn có quyền xuất CSV; quyền xem báo cáo trên màn hình chưa được cấp.</p>':'<p>Tài khoản chưa có quyền xem hoặc xuất báo cáo.</p>');
  const details=document.createElement('section');container.append(details);
  await mountReportDetails(details,{year,departmentId,category,finance});
  if(!current())return;
  container.querySelector('[data-report-filter]')?.addEventListener('submit',event=>{event.preventDefault();event.stopPropagation();const f=new FormData(event.target);mountReports(container,{year:Number(f.get('year')),departmentId:f.get('departmentId'),category,finance,filters});});
 }catch(e){if(current()){container.replaceChildren();const p=document.createElement('p');p.className='error-banner';p.setAttribute('role','alert');p.textContent='Không tải được báo cáo: '+e.message;container.append(p);}}
}
