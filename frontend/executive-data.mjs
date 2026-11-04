export const STORAGE_KEY = 'hpuni-executive-v1';
export const UNITS = ['Khoa CNTT','Khoa Kinh tế','Phòng Quản trị'];
export const amount = r => r.items.reduce((s,i)=>s+i.quantity*i.price,0);
export function createSeed(){
  const item=(name,quantity,price)=>({name,quantity,price});
  const request=(id,title,unit,year,status,submitted,items,extra={})=>({id,title,unit,year,status,submitted,items,version:1,author:'Nguyễn Thu Hà · Phòng Kế hoạch – Tài chính',purpose:'Đáp ứng kế hoạch giảng dạy, học tập và vận hành năm tài chính.',...extra});
  return {version:1,revision:0,requests:[
    request('DT-2026-014','Trang bị phòng thực hành máy tính','Khoa CNTT',2026,'pending','2026-11-01',[item('Máy tính phục vụ thực hành',20,18000000),item('Thiết bị mạng và lắp đặt',1,40000000)],{priority:true,purpose:'Bổ sung 20 máy tính và nâng cấp kết nối mạng cho phòng thực hành, phục vụ học kỳ II.'}),
    request('DT-2026-015','Bảo trì hệ thống điều hòa khu A','Phòng Quản trị',2026,'pending','2026-11-02',[item('Bảo dưỡng điều hòa',30,1200000),item('Vật tư thay thế',1,24000000)],{purpose:'Bảo dưỡng định kỳ thiết bị khu giảng đường A, thay thế vật tư đã xuống cấp.'}),
    request('DT-2026-016','Học liệu và thiết bị giảng dạy','Khoa Kinh tế',2026,'pending','2026-11-03',[item('Bộ học liệu chuyên ngành',10,4500000),item('Máy chiếu',2,22500000)]),
    request('DT-2027-001','Nâng cấp phòng học năm 2027','Phòng Quản trị',2027,'pending','2026-11-03',[item('Trang thiết bị phòng học',5,50000000)]),
    request('DT-2026-001','Ngân sách hoạt động Khoa CNTT','Khoa CNTT',2026,'approved','2026-01-05',[item('Hoạt động đào tạo và thiết bị',1,2400000000)],{decisionAt:'2026-01-10T09:00:00+07:00',opinion:'Thống nhất kế hoạch và các khoản mục dự toán.',reviewer:'Ban Giám hiệu'}),
    request('DT-2026-002','Ngân sách hoạt động Khoa Kinh tế','Khoa Kinh tế',2026,'approved','2026-01-05',[item('Hoạt động đào tạo và học liệu',1,1800000000)],{decisionAt:'2026-01-10T10:00:00+07:00',opinion:'Phê duyệt theo hồ sơ đã trình.',reviewer:'Ban Giám hiệu'}),
    request('DT-2026-003','Ngân sách vận hành và bảo trì','Phòng Quản trị',2026,'approved','2026-01-06',[item('Vận hành cơ sở vật chất',1,3600000000)],{decisionAt:'2026-01-11T09:00:00+07:00',opinion:'Thực hiện theo kế hoạch bảo trì năm.',reviewer:'Ban Giám hiệu'}),
    request('DT-2026-012','Mua bổ sung thiết bị trình chiếu','Khoa CNTT',2026,'rejected','2026-10-20',[item('Máy chiếu',5,24000000)],{decisionAt:'2026-10-23T14:30:00+07:00',opinion:'Cần bổ sung biên bản đánh giá hiện trạng và báo giá so sánh.',reviewer:'Ban Giám hiệu'})
  ],expenses:[
    {id:'CHI-001',unit:UNITS[0],year:2026,value:1560000000},
    {id:'CHI-002',unit:UNITS[1],year:2026,value:1080000000},
    {id:'CHI-003',unit:UNITS[2],year:2026,value:2520000000}
  ],resources:[
    {unit:UNITS[0],year:2026,rooms:8,assets:245,maintenance:120000000},
    {unit:UNITS[1],year:2026,rooms:12,assets:180,maintenance:65000000},
    {unit:UNITS[2],year:2026,rooms:34,assets:725,maintenance:380000000}
  ]};
}
export function validDatabase(d){return d?.version===1&&Number.isInteger(d.revision)&&Array.isArray(d.requests)&&Array.isArray(d.expenses)&&Array.isArray(d.resources)&&d.requests.every(r=>r&&typeof r.id==='string'&&Array.isArray(r.items)&&r.items.every(i=>typeof i.name==='string'&&Number.isFinite(i.price)&&i.price>=0&&Number.isFinite(i.quantity)&&i.quantity>0));}
export function budgetRows(db,year,unit=''){
  return UNITS.filter(u=>!unit||u===unit).map(u=>{
    const approved=db.requests.filter(r=>r.year===Number(year)&&r.unit===u&&r.status==='approved');
    const spent=db.expenses.filter(e=>e.year===Number(year)&&e.unit===u).reduce((s,e)=>s+e.value,0);
    const total=approved.reduce((s,r)=>s+amount(r),0);
    return {unit:u,total,spent,available:total-spent,count:approved.length};
  }).filter(r=>r.total||r.spent);
}
export function decide(db,{id,version,status,opinion='',at=new Date().toISOString()}){
  if(!['approved','rejected'].includes(status)) throw Error('Quyết định không hợp lệ.');
  const r=db.requests.find(r=>r.id===id);
  if(!r||r.status!=='pending'||r.version!==version) throw Error('Hồ sơ đã thay đổi hoặc đã được xử lý. Đóng và mở lại hồ sơ.');
  if(opinion.length>2000) throw Error('Ý kiến tối đa 2.000 ký tự.');
  const next=structuredClone(db), target=next.requests.find(r=>r.id===id);
  Object.assign(target,{status,opinion:opinion.trim(),decisionAt:at,reviewer:'Ban Giám hiệu · Phiên demo',version:r.version+1});
  next.revision++;
  return next;
}
