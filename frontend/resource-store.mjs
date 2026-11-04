import {DEFAULT_ROOMS,DEFAULT_DEVICES,createLecturerSeed,TODAY} from './resource-seed.mjs';
import {holdsRoom,overlaps,minutes} from './availability.mjs';
export const RESOURCE_KEY='hpuni-lecturer-v1';
export const VOUCHERS=[{id:'CT-2026-015',amount:24000000,label:'Bảo trì thiết bị · 24.000.000 đ'},{id:'CT-2026-016',amount:3000000,label:'Sửa chữa thiết bị · 3.000.000 đ'}];
const clone=v=>structuredClone(v);
const assert=(ok,message)=>{if(!ok)throw Error(message);};
const integer=n=>Number.isSafeInteger(n)&&n>=0;
export function normalizeResources(input=createLecturerSeed()){
  assert(input?.version===1&&Array.isArray(input.requests)&&Array.isArray(input.logs),'Dữ liệu tài nguyên không hợp lệ; không ghi đè dữ liệu cũ.');
  const db=clone(input);
  db.catalog??={rooms:clone(DEFAULT_ROOMS),devices:clone(DEFAULT_DEVICES)};
  assert(Array.isArray(db.catalog.rooms)&&Array.isArray(db.catalog.devices),'Danh mục không hợp lệ.');
  db.resourceRevision??=0;
  db.roomReservations??=[{id:'SHARED-A201-1',kind:'room',resource:'A201',date:TODAY,start:'07:00',end:'08:30',status:'approved'},{id:'SHARED-A201-2',kind:'room',resource:'A201',date:TODAY,start:'09:30',end:'11:30',status:'approved'}];
  db.assets??=[...db.catalog.devices.map((d,i)=>({id:'TS-'+d.id,name:d.name,device:d.id,room:'',unit:'Phòng Quản trị',location:'Kho thiết bị',quantity:d.available,unusable:0,lost:0,price:[15000000,18000000,5000000,22000000,12000000,600000][i]||0,purchased:'2025-09-01',version:1})),{id:'TS-AC401',name:'Điều hòa phòng A.401',device:'',room:'A401',unit:'Phòng Quản trị',location:'Nhà A · A.401',quantity:1,unusable:1,lost:0,price:18000000,purchased:'2024-06-01',version:1}];
  db.inventories??=[];db.maintenance??=[];db.resourceAudit??=[];
  return db;
}
export function readResources(storage=localStorage){const raw=storage.getItem(RESOURCE_KEY);return normalizeResources(raw?JSON.parse(raw):undefined);}
const isoDate=d=>`${d.getFullYear()}-${String(d.getMonth()+1).padStart(2,'0')}-${String(d.getDate()).padStart(2,'0')}`;
export function overdue(r,now=new Date()){return r.kind==='device'&&['borrowed','return_pending'].includes(r.status)&&r.quantity-(r.returned||0)>0&&r.date&&r.end&&new Date(`${r.date}T${r.end}`)<now;}
function span(start,end){const a=new Date(start).getTime(),b=new Date(end).getTime();assert(Number.isFinite(a)&&Number.isFinite(b)&&a<b,'Khoảng thời gian không hợp lệ.');return [a,b];}
function loanSpan(r){const a=new Date(`${r.date}T${r.start}`).getTime();return [a,['borrowed','return_pending'].includes(r.status)?Infinity:new Date(`${r.date}T${r.end}`).getTime()];}
function maintSpan(m){return [new Date(m.start).getTime(),m.status==='running'?Infinity:new Date(m.end).getTime()];}
function peak(events,lo,hi){let n=0,max=0;const points=[];for(const [a,b,q]of events){if(a<hi&&b>lo){points.push([Math.max(a,lo),q],[Math.min(b,hi),-q]);}}points.sort((a,b)=>a[0]-b[0]||a[1]-b[1]);for(const[,q]of points){n+=q;max=Math.max(max,n);}return max;}
export function deviceFree(db,id,date,start,end,exclude='',includePending=true){
  const device=db.catalog.devices.find(d=>d.id===id);if(!device)return 0;
  const [lo,hi]=span(`${date}T${start}`,`${date}T${end}`);
  const events=db.requests.filter(r=>r.kind==='device'&&r.resource===id&&r.id!==exclude&&(['approved','ready','borrowed','return_pending'].includes(r.status)||(includePending&&r.status==='pending'))).map(r=>[...loanSpan(r),Math.max(0,r.quantity-(r.returned||0))]);
  for(const m of db.maintenance.filter(m=>m.device===id&&['planned','running'].includes(m.status)&&m.mode!=='repair'))events.push([...maintSpan(m),m.quantity]);
  return Math.max(0,device.available-peak(events,lo,hi));
}
export function roomReservations(db,id,date,exclude=''){
  const result=[...db.requests,...db.roomReservations].filter(r=>r.kind==='room'&&r.resource===id&&r.date===date&&r.id!==exclude&&holdsRoom(r.status));
  const lo=new Date(date+'T00:00').getTime(),hi=new Date(date+'T23:59').getTime();
  for(const m of db.maintenance.filter(m=>m.room===id&&['planned','running'].includes(m.status))){const[a,b]=maintSpan(m);if(a<hi&&b>lo){const fmt=t=>new Date(t).toTimeString().slice(0,5);result.push({id:m.id,start:a<=lo?'00:00':fmt(a),end:b>=hi?'23:59':fmt(b),status:'approved',maintenance:true});}}
  return result;
}
function syncDevice(db,a){const d=db.catalog.devices.find(d=>d.id===a.device);if(d){d.name=a.name;d.available=Math.max(0,a.quantity-a.unusable-a.lost);}}
function busyAsset(db,a){
  if(!a.device)return 0;
  const events=db.requests.filter(r=>r.resource===a.device&&['approved','ready','borrowed','return_pending','pending'].includes(r.status)).map(r=>[...loanSpan(r),r.quantity-(r.returned||0)]);
  for(const m of db.maintenance.filter(m=>m.device===a.device&&m.mode==='service'&&['planned','running'].includes(m.status)))events.push([...maintSpan(m),m.quantity]);
  return peak(events,-Infinity,Infinity);
}
export function manageResources(source,action,p={},expectedRevision=source.resourceRevision,now=new Date()){
  const db=normalizeResources(source),time=now.toISOString();
  assert(expectedRevision===db.resourceRevision,'Dữ liệu đã thay đổi. Đóng và mở lại biểu mẫu để kiểm tra.');
  const request=db.requests.find(r=>r.id===p.id),asset=db.assets.find(a=>a.id===p.id),job=db.maintenance.find(m=>m.id===p.id);
  const text=(v,max=1000)=>{const s=String(v||'').trim();assert(s.length>0&&s.length<=max,`Vui lòng nhập nội dung từ 1 đến ${max} ký tự.`);return s;};
  const optional=(v,max=1000)=>{const s=String(v||'').trim();assert(s.length<=max,`Nội dung tối đa ${max} ký tự.`);return s;};
  if(action==='review'){
    assert(request?.status==='pending','Phiếu không còn chờ duyệt.');
    assert(['approved','rejected'].includes(p.status),'Quyết định không hợp lệ.');
    const reason=optional(p.notes);
    if(p.status==='approved'){
      assert(request.date>=isoDate(now),'Không duyệt phiếu có ngày sử dụng đã qua.');
      assert(minutes(request.start)<minutes(request.end),'Giờ sử dụng không hợp lệ.');
      if(request.kind==='room'){
        const room=db.catalog.rooms.find(r=>r.id===request.resource);
        assert(room?.available&&integer(request.students)&&request.students>0&&request.students<=room.seats,'Phòng không khả dụng hoặc không đủ sức chứa.');
        assert(minutes(request.start)>=420&&minutes(request.end)<=1080,'Giờ phục vụ phòng từ 07:00 đến 18:00.');
        assert(!roomReservations(db,request.resource,request.date,request.id).some(r=>overlaps(r.start,r.end,request.start,request.end)),'Lịch phòng bị trùng.');
      }else assert(integer(request.quantity)&&request.quantity>0&&deviceFree(db,request.resource,request.date,request.start,request.end,request.id,false)>=request.quantity,'Không đủ thiết bị trong khoảng đăng ký.');
    }
    Object.assign(request,{status:p.status,reason,reviewedAt:time,reviewer:'Quản lý phòng và tài sản'});
  }else if(action==='handover'){
    assert(request?.kind==='device'&&request.status==='approved','Chỉ bàn giao phiếu thiết bị đã duyệt.');
    assert(deviceFree(db,request.resource,request.date,request.start,request.end,request.id,false)>=request.quantity,'Không đủ thiết bị để bàn giao.');
    assert(Number(p.quantity)===request.quantity,'Số lượng bàn giao phải khớp số lượng được duyệt.');
    Object.assign(request,{status:'ready',handover:{quantity:request.quantity,condition:optional(p.notes),at:time,by:'Quản lý phòng và tài sản'}});
  }else if(action==='return'){
    assert(request?.kind==='device'&&request.status==='return_pending'&&request.returnRequest,'Không có yêu cầu trả đang chờ tiếp nhận.');
    const good=Number(p.good),damaged=Number(p.damaged),lost=Number(p.lost),q=good+damaged+lost;
    assert([good,damaged,lost].every(integer)&&q>0&&q<=request.quantity-(request.returned||0),'Số lượng tiếp nhận vượt phần còn mượn hoặc không hợp lệ.');
    assert(q<=request.returnRequest.quantity,'Không tiếp nhận vượt yêu cầu trả.');
    const notes=optional(p.notes);
    const a=db.assets.find(a=>a.device===request.resource);assert(a,'Chưa có hồ sơ tài sản liên kết.');
    a.unusable+=damaged;a.lost+=lost;syncDevice(db,a);
    request.returnHistory??=[];
    request.returnHistory.push({id:'TRA-'+Date.now(),good,damaged,lost,quantity:q,notes,at:time,overdue:overdue(request,now),declared:clone(request.returnRequest)});
    request.returned=(request.returned||0)+q;request.returnRequest=null;
    request.status=request.returned===request.quantity?'completed':'borrowed';
  }else if(action==='log'){
    const log=db.logs.find(l=>l.id===p.id);assert(log?.status==='pending','Nhật ký không còn chờ xác nhận.');
    assert(['confirmed','needs_update'].includes(p.status),'Kết quả xác nhận không hợp lệ.');
    Object.assign(log,{status:p.status,reviewNotes:optional(p.notes),reviewedAt:time});
  }else if(action==='room'){
    const id=text(p.id,30).toUpperCase();assert(/^[A-Z0-9-]+$/.test(id),'Mã phòng chỉ gồm chữ không dấu, số và dấu gạch ngang.');
    const old=db.catalog.rooms.find(r=>r.id===id),seats=Number(p.seats);assert(integer(seats)&&seats>=1&&seats<=1000,'Sức chứa từ 1 đến 1.000.');
    assert(['Phòng học','Phòng máy','Hội trường','Phòng thí nghiệm'].includes(p.type),'Loại phòng không hợp lệ.');
    assert(!p.available||!db.assets.some(a=>a.room===id&&(a.unusable>0||a.lost>0)),'Phòng còn tài sản cố định hỏng hoặc mất. Cập nhật kết quả xử lý trước khi mở phục vụ.');
    const active=db.requests.filter(r=>r.kind==='room'&&r.resource===id&&r.date>=isoDate(now)&&['pending','approved'].includes(r.status));
    assert(!active.some(r=>r.students>seats),'Sức chứa nhỏ hơn số người của phiếu đang hiệu lực.');
    assert(p.available||!active.length&&!db.roomReservations.some(r=>r.resource===id&&r.date>=isoDate(now)&&holdsRoom(r.status)),'Phòng còn lịch hiệu lực. Xử lý lịch trước khi ngừng phục vụ.');
    const values={id,name:text(p.name,150),building:text(p.building,60),seats,type:p.type,available:!!p.available,tags:String(p.tags||'').split(',').map(t=>t.trim()).filter(Boolean).slice(0,10),visual:p.type==='Hội trường'?'hall':p.type==='Phòng máy'?'lab':''};
    if(old){assert(p.edit,'Mã phòng đã tồn tại.');Object.assign(old,values);}else{assert(!p.edit,'Không tìm thấy phòng.');db.catalog.rooms.push(values);}
  }else if(action==='asset'){
    const id=text(p.id,30).toUpperCase();assert(/^[A-Z0-9-]+$/.test(id),'Mã tài sản chỉ gồm chữ không dấu, số và dấu gạch ngang.');
    const old=db.assets.find(a=>a.id===id),quantity=Number(p.quantity),price=Number(p.price);
    assert(integer(quantity)&&quantity>0&&quantity<=100000&&Number.isFinite(price)&&price>=0&&price<=1e12,'Số lượng hoặc nguyên giá không hợp lệ.');
    if(old){assert(p.edit,'Mã tài sản đã tồn tại.');assert(quantity>=old.unusable+old.lost+busyAsset(db,old),'Số lượng không đủ phần hỏng, mất và đang được giữ/mượn.');}
    else assert(!p.edit,'Không tìm thấy tài sản.');
    const room=p.room??old?.room??'';
    assert(!room||db.catalog.rooms.some(r=>r.id===room),'Phòng liên kết không tồn tại.');
    assert(!room||!(old?.device||p.loanable),'Thiết bị cho mượn không đồng thời khóa cố định một phòng.');
    const values={id,name:text(p.name,150),unit:text(p.unit,100),location:text(p.location,150),quantity,price,purchased:text(p.purchased,10),room};
    assert(/^\d{4}-\d{2}-\d{2}$/.test(values.purchased),'Ngày ghi tăng không hợp lệ.');
    if(old){Object.assign(old,values,{version:(old.version||1)+1});syncDevice(db,old);}
    else{const a={...values,device:p.loanable?id:'',unusable:0,lost:0,version:1};db.assets.push(a);if(p.loanable)db.catalog.devices.push({id,name:a.name,category:'Thiết bị khác',available:quantity,unit:'chiếc',tags:['Thiết bị được quản lý'],shape:'equipment'});}
  }else if(action==='inventory-create'){
    const rows=db.assets.filter(a=>!p.unit||a.unit===p.unit).map(a=>({id:a.id,name:a.name,expected:a.quantity,actual:null,condition:'',notes:''}));assert(rows.length,'Phạm vi không có tài sản.');
    db.inventories.push({id:'KK-'+Date.now(),name:text(p.name,150),unit:p.unit||'Toàn bộ đơn vị',date:text(p.date,10),status:'open',rows,createdAt:time});
  }else if(action==='inventory-save'){
    const inv=db.inventories.find(i=>i.id===p.id);assert(inv?.status==='open','Đợt kiểm kê đã hoàn tất hoặc không tồn tại.');
    for(const row of inv.rows){const val=p.rows.find(r=>r.id===row.id);assert(val,'Thiếu tài sản kiểm kê.');const actual=val.actual===''?null:Number(val.actual);assert(actual===null||integer(actual),'Số thực tế phải là số nguyên không âm.');assert(['','Tốt','Hỏng / cần xử lý','Mất / thiếu'].includes(val.condition),'Tình trạng kiểm kê không hợp lệ.');Object.assign(row,{actual,condition:val.condition,notes:String(val.notes||'').trim()});}
    if(p.finish){assert(inv.rows.every(r=>r.actual!==null&&r.condition),'Còn tài sản chưa ghi nhận đầy đủ.');inv.status='completed';inv.completedAt=time;}
  }else if(action==='maintenance-plan'){
    const a=db.assets.find(a=>a.id===p.asset);assert(a,'Không tìm thấy tài sản.');const[lo,hi]=span(p.start,p.end),q=Number(p.quantity);assert(integer(q)&&q>0,'Số lượng bảo trì phải lớn hơn 0.');
    assert(['service','repair'].includes(p.mode),'Loại bảo trì không hợp lệ.');
    assert(q<= (p.mode==='repair'?a.unusable:a.quantity-a.unusable-a.lost),'Số lượng bảo trì vượt phần tài sản tương ứng.');
    if(a.room)for(const r of [...db.requests,...db.roomReservations].filter(r=>r.resource===a.room&&holdsRoom(r.status))){const[x,y]=loanSpan(r);assert(!(x<hi&&y>lo),'Bảo trì trùng lịch phòng.');}
    if(a.device&&p.mode==='service'){
      const events=db.requests.filter(r=>r.resource===a.device&&['pending','approved','ready','borrowed','return_pending'].includes(r.status)).map(r=>[...loanSpan(r),r.quantity-(r.returned||0)]);
      for(const m of db.maintenance.filter(m=>m.device===a.device&&m.mode==='service'&&['planned','running'].includes(m.status)))events.push([...maintSpan(m),m.quantity]);
      assert(peak(events,lo,hi)+q<=a.quantity-a.unusable-a.lost,'Bảo trì trùng lịch mượn hoặc vượt số thiết bị tốt.');
    }
    assert(!db.maintenance.some(m=>m.asset===a.id&&['planned','running'].includes(m.status)&&maintSpan(m)[0]<hi&&maintSpan(m)[1]>lo),'Tài sản đã có kế hoạch trong khoảng này.');
    db.maintenance.push({id:'BT-'+Date.now(),asset:a.id,device:a.device,room:a.room,name:a.name,quantity:q,mode:p.mode,start:p.start,end:p.end,content:optional(p.content),status:'planned',createdAt:time});
  }else if(action==='maintenance-start'){
    assert(job?.status==='planned','Kế hoạch không ở trạng thái chờ thực hiện.');assert(new Date(job.start)<=now,'Chưa đến thời gian bắt đầu bảo trì.');job.status='running';job.startedAt=time;
  }else if(action==='maintenance-cost'){
    assert(job?.status==='running','Chỉ ghi chi phí khi đang bảo trì.');
    const cost=Number(p.cost);assert(integer(cost)&&cost>0,'Chi phí phải là số nguyên dương hợp lệ.');
    assert(!job.costRecordedAt||job.cost===cost,'Chi phí đã gửi kế toán, không thể thay đổi.');
    Object.assign(job,{cost,costRecordedAt:job.costRecordedAt||time});
  }else if(action==='maintenance-finish'){
    assert(job?.status==='running','Chỉ hoàn tất kế hoạch đang thực hiện.');
    const cost=Number(p.cost);assert(integer(cost),'Chi phí phải là số nguyên không âm hợp lệ.');
    assert(!job.costRecordedAt||job.cost===cost,'Chi phí đã gửi kế toán, không thể thay đổi.');
    assert(['good','unavailable'].includes(p.result),'Kết quả bảo trì không hợp lệ.');
    Object.assign(job,{status:'completed',result:p.result,resultNotes:optional(p.notes),cost,voucher:'',finishedAt:time,...(cost?{costRecordedAt:job.costRecordedAt||time}:{})});
    const a=db.assets.find(a=>a.id===job.asset);
    if(p.result==='good'&&job.mode==='repair')a.unusable=Math.max(0,a.unusable-job.quantity);
    if(p.result==='unavailable'&&job.mode==='service')a.unusable+=job.quantity;
    syncDevice(db,a);if(a.room){const room=db.catalog.rooms.find(r=>r.id===a.room);if(room)room.available=p.result==='good'&&db.assets.filter(x=>x.room===a.room).every(x=>x.unusable===0&&x.lost===0);}
  }else if(action==='reminder'){
    assert(request&&overdue(request,now),'Phiếu không còn quá hạn.');request.reminder={at:time,notes:optional(p.notes)};
  }else throw Error('Thao tác không được hỗ trợ.');
  db.resourceRevision++;
  db.resourceAudit.push({id:'HD-'+Date.now(),action,target:p.id||p.asset||'',at:time,by:'Quản lý phòng và tài sản'});
  return db;
}
