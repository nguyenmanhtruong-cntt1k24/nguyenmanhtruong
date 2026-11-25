import {api,can} from './server-session.mjs';

const esc=value=>String(value??'').replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
const query=values=>new URLSearchParams(Object.entries(values).filter(([,value])=>value!==''&&value!=null));
const labels={finance:'Sổ chứng từ',assets:'Danh mục tài sản hiện tại',loans:'Phiếu mượn theo năm',rooms:'Phiếu phòng theo năm'};
const versions=new WeakMap();
function renderCell(cell,column,row,kind){
 const value=esc(formatDetailCell(cell,column));
 if(kind==='finance'&&column.label==='Số chứng từ')return `<a href="payment-voucher.html?id=${encodeURIComponent(row.id)}" target="_blank" rel="noopener">${value}</a>`;
 if(kind==='assets'&&column.label==='Mã'&&(can('resources.manage')||can('budget.approve')))return `<a href="${can('resources.manage')?'resources':'executive'}.html?assetLifecycleId=${encodeURIComponent(row.id)}#/lifecycle">${value}</a>`;
 return value;
}
const categories={general:'Ngân sách chung',maintenance:'Bảo trì',equipment:'Thiết bị',teaching:'Giảng dạy',operations:'Vận hành'};
const kinds={receipt:'Thu',expense:'Chi'};
const statuses={draft:'Nháp',pending:'Chờ duyệt',approved:'Đã duyệt',rejected:'Từ chối',cancelled:'Đã hủy',ready:'Đã bàn giao',receipt_disputed:'Sai lệch nhận',borrowed:'Đang mượn',return_pending:'Chờ xác nhận trả',completed:'Hoàn tất'};
export function formatDetailCell(cell,column={}){
 if(cell==null||cell==='')return column.numeric?(column.label?.includes('Nguyên giá')?'Chưa kê khai':'Chưa có dữ liệu'):'';
 if(column.numeric){const value=Number(cell);return Number.isFinite(value)?value.toLocaleString('vi-VN'):String(cell);}
 if(column.label==='Khoản mục')return categories[cell]||String(cell);
 if(column.label==='Loại')return kinds[cell]||String(cell);
 if(column.label==='Trạng thái')return statuses[cell]||String(cell);
 return String(cell);
}

export async function mountReportDetails(container,{year=new Date().getFullYear(),departmentId='',category='',finance=true,kind=finance?'finance':'assets',page=1,pageSize=20}={}){
 if(!container)return;
 const revision=(versions.get(container)||0)+1;versions.set(container,revision);
 if(kind==='finance'&&(!finance||!can('budget.read')))kind='assets';
 const q=query({year,departmentId,category:kind==='finance'?category:null});
 const base='/api/v1/reports/details/'+kind;
 const options=Object.entries(labels).filter(([value])=>value!=='finance'||finance&&can('budget.read'));
 const selector=`<label>Chi tiết <select data-detail-kind>${options.map(([value,label])=>`<option value="${value}" ${kind===value?'selected':''}>${label}</option>`).join('')}</select></label>`;
 const tools=can('reports.export')?`<div style="display:flex;gap:12px;flex-wrap:wrap;margin:12px 0"><a class="btn secondary" download href="${base}/export/csv?${esc(q)}">CSV chi tiết</a><a class="btn secondary" download href="${base}/export/xlsx?${esc(q)}">Excel (.xlsx)</a><a class="btn secondary" target="_blank" rel="noopener" href="${base}/export/print?${esc(q)}">In / Lưu PDF</a></div>`:'';
 container.innerHTML=`<h2>Báo cáo chi tiết</h2>${selector}${tools}<div data-detail-body>Đang tải…</div>`;
 const remount=values=>mountReportDetails(container,{year,departmentId,category,finance,kind,page,pageSize,...values});
 container.querySelector('[data-detail-kind]')?.addEventListener('change',event=>remount({kind:event.target.value,page:1}));
 if(!can('reports.read')){container.querySelector('[data-detail-body]').textContent=can('reports.export')?'Chọn định dạng để xuất toàn bộ dữ liệu đúng phạm vi (tối đa 10.000 dòng).':'Tài khoản chưa có quyền xem hoặc xuất báo cáo.';return;}
 try{
  const report=await api.request('/reports/details/'+kind+'?'+query({...Object.fromEntries(q),page,pageSize}));
  if(!container.isConnected||versions.get(container)!==revision)return;
  const pages=Math.max(1,Math.ceil(report.total/pageSize));
  container.querySelector('[data-detail-body]').innerHTML=`<p>${esc(report.note)}</p><p>${esc(report.total)} hồ sơ · Trang ${page}/${pages} · ${esc(new Date(report.generatedAt).toLocaleString('vi-VN'))}</p>${report.items.length?`<div class="table-wrap"><table><thead><tr>${report.columns.map(column=>`<th>${esc(column.label)}</th>`).join('')}</tr></thead><tbody>${report.items.map(row=>`<tr>${row.cells.map((cell,i)=>`<td>${renderCell(cell,report.columns[i],row,kind)}</td>`).join('')}</tr>`).join('')}</tbody></table></div>`:'<p>Không có hồ sơ trong trang và bộ lọc này.</p>'}<div style="display:flex;align-items:center;gap:12px;flex-wrap:wrap;margin:12px 0"><button type="button" class="btn secondary" data-detail-prev ${page<=1?'disabled':''}>Trang trước</button><button type="button" class="btn secondary" data-detail-next ${page>=pages?'disabled':''}>Trang sau</button><label>Số dòng <select data-detail-size>${[20,50,100].map(size=>`<option value="${size}" ${size===pageSize?'selected':''}>${size}</option>`).join('')}</select></label></div><p style="font-size:12px;color:#64748b">Xuất lấy toàn bộ bộ lọc, không chỉ trang đang xem; tối đa 10.000 dòng. Trang in hỗ trợ lưu PDF bằng lệnh In của trình duyệt, chưa phải mẫu chứng từ có chữ ký.</p>`;
  container.querySelector('[data-detail-prev]')?.addEventListener('click',()=>remount({page:page-1}));
  container.querySelector('[data-detail-next]')?.addEventListener('click',()=>remount({page:page+1}));
  container.querySelector('[data-detail-size]')?.addEventListener('change',event=>remount({page:1,pageSize:Number(event.target.value)}));
 }catch(error){if(container.isConnected&&versions.get(container)===revision){const body=container.querySelector('[data-detail-body]');body.textContent='Không tải được chi tiết: '+error.message;body.setAttribute('role','alert');}}
}
