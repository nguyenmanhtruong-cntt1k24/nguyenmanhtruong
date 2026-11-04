import {STORAGE_KEY} from './executive-data.mjs';
import {normalizeAccounting} from './accounting-data.mjs';
import {readResources} from './resource-store.mjs';
const root=document.querySelector('#voucher'),print=document.querySelector('#print');
try{
 const params=new URLSearchParams(location.search),maintenanceId=params.get('maintenance');
 const raw=localStorage.getItem(STORAGE_KEY);if(!raw&&!maintenanceId)throw Error('Chưa có dữ liệu chứng từ trên trình duyệt này.');
 const db=raw?normalizeAccounting(JSON.parse(raw)):null,id=params.get('id');let r=db?.expenses.find(r=>r.id===id);
 if(maintenanceId){const resources=readResources(),m=resources.maintenance.find(m=>m.id===maintenanceId);if(!m||!(m.cost>0)||!(m.status==='completed'||m.costRecordedAt))throw Error('Chưa có chi phí phát sinh đã lưu cho kế hoạch này.');const q=db?.paymentQueue.find(q=>q.sourceMaintenance===m.id),paid=q?.status==='paid';r={id:m.id,voucher:'CP-'+m.id,date:String(m.costRecordedAt||m.finishedAt).slice(0,10),unit:resources.assets.find(a=>a.id===m.asset)?.unit||'Phòng Quản trị',year:Number(String(m.costRecordedAt||m.finishedAt).slice(0,4)),content:'Chi phí bảo trì: '+m.name,value:m.cost};root.dataset.maintenance='true';root.dataset.payment=paid?'Đã chi':'Chưa chi — chờ kế toán xử lý';}
 if(!r?.voucher||!r.date)throw Error('Không tìm thấy khoản chi có chứng từ. Số liệu tổng hợp đầu kỳ không có phiếu chi riêng.');
 const text=(tag,value,cls)=>{const e=document.createElement(tag);e.textContent=value;if(cls)e.className=cls;return e;};
 const header=document.createElement('header');header.className='voucher-header';header.append(text('strong','ĐẠI HỌC HẢI PHÒNG'),text('p','Phòng Kế hoạch – Tài chính'));root.append(header,text('h1',maintenanceId?'CHỨNG TỪ CHI PHÍ PHÁT SINH':'PHIẾU CHI'),text('p','Ngày '+r.date.split('-').reverse().join('/'),'voucher-date'),text('p','Số chứng từ: '+r.voucher,'voucher-number'));if(maintenanceId)root.append(text('p','Trạng thái thanh toán: '+root.dataset.payment));
 const details=document.createElement('dl');for(const [label,value] of [['Đơn vị',r.unit],['Năm tài chính',r.year],['Nội dung chi',r.content||'—'],['Số tiền',Number(r.value).toLocaleString('vi-VN')+' đồng'],['Mã giao dịch',r.id]])details.append(text('dt',label),text('dd',value,label==='Số tiền'?'amount':''));root.append(details);
 const signatures=document.createElement('section');signatures.className='signatures';for(const label of ['Người lập phiếu','Kế toán trưởng','Người duyệt','Người nhận tiền']){const box=document.createElement('div');box.append(text('strong',label),text('small','(Ký, ghi rõ họ tên)'),text('div','','signature-space'));signatures.append(box);}root.append(signatures,text('p','Chứng từ minh họa được tạo từ khoản chi đã ghi nhận trên trình duyệt.','note'));
 document.title='Phiếu chi '+r.voucher+' · HPUNI';print.disabled=false;print.addEventListener('click',()=>window.print());
}catch(err){root.textContent=err.message;}
