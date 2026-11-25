import {createSeed,validDatabase,UNITS,budgetRows} from './executive-data.mjs';
export function normalizeAccounting(db=createSeed()) { if(!validDatabase(db))throw Error('Dữ liệu tài chính không hợp lệ.');return {...structuredClone(db),receipts:db.receipts||[],paymentQueue:db.paymentQueue||[],accountingAudit:db.accountingAudit||[]}; }
export function accountingAction(source,action,p,revision=source.revision){
 if(source.revision!==revision)throw Error('Dữ liệu đã thay đổi. Đóng và mở lại biểu mẫu.');
 const d=normalizeAccounting(source), now=new Date().toISOString(), id=prefix=>`${prefix}-${Date.now()}-${Math.random().toString(36).slice(2,6)}`;
 const required=(v,label)=>{if(!String(v||'').trim())throw Error(`Vui lòng nhập ${label}.`);return String(v).trim();};
 const scope=()=>{if(!UNITS.includes(p.unit)||![2026,2027].includes(Number(p.year)))throw Error('Đơn vị hoặc năm không hợp lệ.');};
 if(action==='draft'){
  scope();const old=d.requests.find(r=>r.id===p.id);if(p.id&&(!old||!['draft','returned'].includes(old.status)))throw Error('Chỉ sửa dự toán nháp hoặc bị từ chối.');
  const items=p.items.map(i=>({name:required(i.name,'tên khoản mục'),quantity:Number(i.quantity),price:Number(i.price)}));
  if(!items.length||items.some(i=>!Number.isFinite(i.quantity)||i.quantity<=0||!Number.isFinite(i.price)||i.price<=0)||!Number.isSafeInteger(Math.round(items.reduce((s,i)=>s+i.quantity*i.price,0))))throw Error('Khoản mục và số tiền phải lớn hơn 0, trong giới hạn hợp lệ.');
  const value={id:old?.id||id('DT'),title:required(p.title,'tên dự toán'),unit:p.unit,year:Number(p.year),purpose:String(p.purpose||'').trim(),items,status:'draft',author:'Nguyễn Thu Hà · Phòng Kế hoạch – Tài chính',version:(old?.version||0)+1,submitted:'',updatedAt:now};
  if(old){d.requests[d.requests.indexOf(old)]={...value,workflow:old.workflow||[],reviews:[...(old.reviews||[]),...(old.opinion?[{opinion:old.opinion,status:old.status,at:old.decisionAt}]:[])]};}else d.requests.push(value);
 }else if(action==='submit'){
  const r=d.requests.find(r=>r.id===p.id);if(!r||r.status!=='draft')throw Error('Chỉ gửi dự toán nháp.');Object.assign(r,{status:'reviewing',submitted:now.slice(0,10),appraisal:null,workflow:[...(r.workflow||[]),{action:'Gửi KHTC thẩm định',at:now,actor:'Người lập'}],version:r.version+1});
 }else if(['appraise','return-review'].includes(action)){
  const r=d.requests.find(r=>r.id===p.id);
  if(!r||!(r.status==='reviewing'||(r.status==='pending'&&!r.appraisal)))throw Error('Hồ sơ không còn chờ thẩm định.');
  const opinion=required(p.opinion,'Ý kiến thẩm định');if(opinion.length>2000)throw Error('Ý kiến tối đa 2.000 ký tự.');
  const appraisal={opinion,at:now,actor:'Phòng Kế hoạch – Tài chính',amount:r.items.reduce((sum,i)=>sum+i.quantity*i.price,0)};
  Object.assign(r,{status:action==='appraise'?'pending':'returned',appraisal:action==='appraise'?appraisal:null,submitted:now.slice(0,10),version:r.version+1,workflow:[...(r.workflow||[]),{...appraisal,action:action==='appraise'?'Thẩm định đạt, trình BGH':'KHTC yêu cầu bổ sung'}]});
 }else if(['receipt','expense','queue'].includes(action)){
  scope();const value=Number(p.value);if(!Number.isSafeInteger(value)||value<=0)throw Error('Số tiền phải là số nguyên dương hợp lệ.');
  const voucher=required(p.voucher,'số chứng từ');if([...d.receipts,...d.expenses,...d.paymentQueue.filter(q=>q.status==='pending'&&q.id!==p.id)].some(t=>t.voucher?.toLowerCase()===voucher.toLowerCase()))throw Error('Số chứng từ đã tồn tại.');
  if(!/^\d{4}-\d{2}-\d{2}$/.test(p.date)||new Date(p.date+'T12:00:00').toISOString().slice(0,10)!==p.date||Number(p.date.slice(0,4))!==Number(p.year))throw Error('Ngày giao dịch phải hợp lệ và thuộc năm tài chính.');
  const row={id:id(action==='receipt'?'THU':'CHI'),unit:p.unit,year:Number(p.year),value,voucher,date:p.date,content:String(p.content||'').trim(),at:now};
  const available=budgetRows(d,p.year,p.unit)[0]?.available||0;
  const queued=p.id?d.paymentQueue.find(q=>q.id===p.id):null;if(p.id&&(!queued||queued.status!=='pending'))throw Error('Yêu cầu đã được xử lý.');
  if(queued)row.sourceRequest=queued.id;
  if(queued?.sourceMaintenance){if(action!=='expense'||value!==queued.value||p.unit!==queued.unit||Number(p.year)!==queued.year)throw Error('Khoản chi bảo trì phải khớp số tiền, đơn vị và năm của yêu cầu phát sinh.');row.sourceMaintenance=queued.sourceMaintenance;}
  if(action==='expense'&&value>available)throw Error('Vượt ngân sách khả dụng. Bạn có thể lưu yêu cầu chờ xử lý.');
  if(action==='receipt')d.receipts.push(row);else if(action==='queue'){if(queued)throw Error('Yêu cầu này đã nằm trong hàng chờ.');d.paymentQueue.push({...row,status:'pending'});}else{d.expenses.push(row);if(queued)Object.assign(queued,{status:'paid',paidAt:now,expenseId:row.id});}
 }else throw Error('Thao tác không hợp lệ.');
 d.revision++;d.accountingAudit.push({action,at:now,requestId:p.id||null});return d;
}
