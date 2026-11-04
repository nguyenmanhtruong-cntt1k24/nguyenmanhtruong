export const ADMIN_KEY='hpuni-admin-v1', CURRENT_ADMIN='USR-001';
export const ADMIN_UNITS=['Ban Giám hiệu','Phòng Kế hoạch – Tài chính','Phòng Quản trị','Khoa CNTT','Khoa Kinh tế'];
const defaultDepartments=()=>[{id:'FAC-CNTT',code:'CNTT',name:'Khoa CNTT',notes:''},{id:'FAC-KT',code:'KT',name:'Khoa Kinh tế',notes:''}];
export const adminUnits=db=>[...ADMIN_UNITS.slice(0,3),...(db.departments||defaultDepartments()).map(k=>k.name)];
export function normalizeAdmin(source){const d=structuredClone(source);if(!Object.hasOwn(d,'departments')){d.departments=defaultDepartments();const r=d.roles?.find(r=>r.id==='admin');if(r&&!r.permissions.includes('departments.manage'))r.permissions.push('departments.manage');}return d;}
export const PERMISSIONS=[
 ['users.manage','Hệ thống','Quản lý tài khoản'],['roles.manage','Hệ thống','Quản lý vai trò'],['permissions.manage','Hệ thống','Phân quyền và phạm vi'],['audit.read','Hệ thống','Xem nhật ký quản trị'],['departments.manage','Hệ thống','Quản lý khoa'],
 ['budget.write','Tài chính','Lập và gửi dự toán'],['budget.approve','Tài chính','Phê duyệt dự toán'],['transactions.write','Tài chính','Ghi nhận thu – chi'],['budget.read','Tài chính','Xem ngân sách'],
 ['rooms.book','Phòng và tài sản','Đăng ký phòng'],['devices.borrow','Phòng và tài sản','Mượn và trả thiết bị'],['resources.manage','Phòng và tài sản','Quản lý phòng, tài sản'],['requests.review','Phòng và tài sản','Duyệt đăng ký, bàn giao'],['inventory.manage','Phòng và tài sản','Kiểm kê và bảo trì'],['logs.write','Phòng và tài sản','Ghi nhật ký sử dụng'],['logs.review','Phòng và tài sản','Xác nhận nhật ký'],['reports.read','Báo cáo','Xem và xuất báo cáo']
 ];
export const SCOPES={all:'Toàn trường',unit:'Đơn vị công tác',units:'Các đơn vị được chọn',self:'Dữ liệu của bản thân'};
const core=['users.manage','roles.manage','permissions.manage','audit.read'];
export function adminSeed(){
 const role=(id,name,scope,permissions,description)=>({id,name,scope,permissions,description,version:1});
 const user=(id,name,username,email,unit,roleId,scope,status='active')=>({id,name,username,email,unit,roleIds:[roleId],scope,units:[],denied:[],status,version:1,createdAt:'2026-11-03T08:00:00+07:00'});
 return {version:1,revision:0,departments:defaultDepartments(),roles:[
 role('admin','Quản trị viên','all',[...core,'departments.manage'],'Quản lý định danh và cấu hình phân quyền của hệ thống.'),
 role('executive','Ban Giám hiệu','all',['budget.approve','budget.read','reports.read'],'Xét duyệt dự toán và theo dõi tình hình toàn trường.'),
 role('accounting','Cán bộ kế toán','all',['budget.write','transactions.write','budget.read','reports.read'],'Lập dự toán, ghi nhận thu chi và báo cáo tài chính.'),
 role('resources','Quản lý phòng và tài sản','all',['resources.manage','requests.review','inventory.manage','logs.review','reports.read'],'Điều phối phòng, thiết bị, kiểm kê và bảo trì.'),
 role('lecturer','Cán bộ / Giảng viên','self',['rooms.book','devices.borrow','logs.write'],'Đăng ký và theo dõi hồ sơ của bản thân.')
 ],users:[
 user(CURRENT_ADMIN,'Trần Quốc Huy','admin.hpuni','admin@hpuni.demo','Phòng Quản trị','admin','all'),
 user('USR-002','Nguyễn Minh Anh','minhanh.gv','minhanh@hpuni.demo','Khoa CNTT','lecturer','self'),
 user('USR-003','Nguyễn Thu Hà','thuha.kt','thuha@hpuni.demo','Phòng Kế hoạch – Tài chính','accounting','all'),
 user('USR-004','Lê Hoàng Nam','hoangnam.ql','hoangnam@hpuni.demo','Phòng Quản trị','resources','all'),
 user('USR-005','Phạm Thanh Bình','thanhbinh.bgh','thanhbinh@hpuni.demo','Ban Giám hiệu','executive','all'),
 user('USR-006','Đỗ Thị Lan','thilan.gv','thilan@hpuni.demo','Khoa Kinh tế','lecturer','self','locked')
 ],audit:[]};
}
export function validAdmin(d){
 if(d?.users?.some(u=>u.roleOverrides!==undefined&&(!u.roleOverrides||Array.isArray(u.roleOverrides)||typeof u.roleOverrides!=='object'||Object.entries(u.roleOverrides).some(([id,v])=>!u.roleIds.includes(id)||!v||!['all','unit','self'].includes(v.scope)||!Array.isArray(v.permissions)||v.permissions.some(p=>!PERMISSIONS.some(x=>x[0]===p))))))return false;
 if(d?.departments&&(!Array.isArray(d.departments)||d.departments.some(k=>!k||typeof k.id!=='string'||typeof k.code!=='string'||typeof k.name!=='string'||!k.name.trim())||new Set(d.departments.map(k=>k.code)).size!==d.departments.length||new Set(adminUnits(d).map(u=>u.toLocaleLowerCase('vi'))).size!==adminUnits(d).length))return false;
 return d?.version===1&&Number.isInteger(d.revision)&&d.revision>=0&&Array.isArray(d.roles)&&Array.isArray(d.users)&&Array.isArray(d.audit)&&d.users.some(u=>u.id===CURRENT_ADMIN)&&d.roles.some(r=>r.id==='admin')&&new Set(d.users.map(u=>u.id)).size===d.users.length&&new Set(d.roles.map(r=>r.id)).size===d.roles.length&&d.roles.every(r=>typeof r.name==='string'&&['all','unit','self'].includes(r.scope)&&Array.isArray(r.permissions)&&r.permissions.every(p=>PERMISSIONS.some(x=>x[0]===p)))&&d.users.every(u=>typeof u.name==='string'&&typeof u.username==='string'&&typeof u.email==='string'&&adminUnits(d).includes(u.unit)&&['all','unit','units','self'].includes(u.scope)&&['active','locked'].includes(u.status)&&Array.isArray(u.units)&&u.units.every(x=>adminUnits(d).includes(x))&&Array.isArray(u.denied)&&u.denied.every(p=>PERMISSIONS.some(x=>x[0]===p))&&Array.isArray(u.roleIds)&&u.roleIds.every(id=>d.roles.some(r=>r.id===id)));
}
export function accessCheck(db,{userId,permission,unit,ownerId}){
 const u=db.users.find(u=>u.id===userId);if(!u)return {allowed:false,reason:'Không tìm thấy tài khoản.'};
 if(u.status!=='active')return {allowed:false,reason:'Tài khoản đang bị khóa.'};
 if(!PERMISSIONS.some(p=>p[0]===permission))return {allowed:false,reason:'Chức năng không hợp lệ.'};
 if(u.denied.includes(permission))return {allowed:false,reason:'Chức năng bị hạn chế riêng trên tài khoản.'};
 const roles=db.roles.filter(r=>u.roleIds.includes(r.id)).map(r=>({...r,...u.roleOverrides?.[r.id]})).filter(r=>r.permissions.includes(permission));
 if(!roles.length)return {allowed:false,reason:'Các vai trò được gán chưa có quyền chức năng này.'};
 const own=ownerId===u.id,match=s=>s==='all'||s==='unit'&&unit===u.unit||s==='self'&&own||s==='units'&&u.units.includes(unit);
 if(!match(u.scope))return {allowed:false,reason:'Dữ liệu nằm ngoài phạm vi được cấp cho tài khoản.'};
 const role=roles.find(r=>match(r.scope));if(!role)return {allowed:false,reason:'Dữ liệu nằm ngoài phạm vi áp dụng của vai trò.'};
 return {allowed:true,reason:`Được cấp qua vai trò ${role.name}; dữ liệu thỏa phạm vi tài khoản và vai trò.`};
}
export function adminAction(source,action,p,expectedRevision=source.revision){
 if(!validAdmin(source))throw Error('Dữ liệu quản trị không hợp lệ.');
 if(source.revision!==expectedRevision)throw Error('Dữ liệu đã thay đổi. Đóng và mở lại biểu mẫu.');
 const d=normalizeAdmin(source),now=new Date().toISOString(),id=prefix=>`${prefix}-${Date.now()}-${Math.random().toString(36).slice(2,7)}`;
 const required=(v,label)=>{const s=String(v||'').trim();if(!s||s.length>500)throw Error(`Vui lòng nhập ${label} hợp lệ (tối đa 500 ký tự).`);return s;};
 let target='',summary='',before=null,after=null;
 if(action==='user'){
  const old=d.users.find(u=>u.id===p.id);if(p.id&&!old)throw Error('Tài khoản không còn tồn tại.');before=old?structuredClone(old):null;
  const username=required(p.username,'tên đăng nhập').toLowerCase(),email=required(p.email,'email').toLowerCase();
  if(!/^[a-z0-9][a-z0-9._-]{2,39}$/.test(username))throw Error('Tên đăng nhập gồm 3–40 ký tự: chữ không dấu, số, dấu chấm, gạch dưới hoặc gạch ngang.');
  if(!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email))throw Error('Email không hợp lệ.');
  if(d.users.some(u=>u.id!==p.id&&(u.username.toLowerCase()===username||u.email.toLowerCase()===email)))throw Error('Tên đăng nhập hoặc email đã tồn tại.');
  if(!adminUnits(d).includes(p.unit))throw Error('Đơn vị không hợp lệ.');
  if(!Array.isArray(p.roleIds)||!p.roleIds.length||p.roleIds.some(id=>!d.roles.some(r=>r.id===id)))throw Error('Chọn ít nhất một vai trò hợp lệ.');
  const value={...(old||{}),id:old?.id||id('USR'),name:required(p.name,'họ tên'),username,email,unit:p.unit,roleIds:[...new Set(p.roleIds)],scope:old?.scope||'self',units:old?.units||[],denied:old?.denied||[],status:old?.status||'active',createdAt:old?.createdAt||now,updatedAt:now,version:(old?.version||0)+1};
  if(value.roleOverrides)value.roleOverrides=Object.fromEntries(Object.entries(value.roleOverrides).filter(([id])=>value.roleIds.includes(id)));
  if(old)d.users[d.users.indexOf(old)]=value;else d.users.push(value);target=value.id;after=value;summary=old?'Cập nhật tài khoản':'Tạo tài khoản demo';
 }else if(action==='status'){
  const u=d.users.find(u=>u.id===p.id);if(!u)throw Error('Không tìm thấy tài khoản.');
  if(p.id===CURRENT_ADMIN)throw Error('Không thể khóa tài khoản đang thao tác.');
  if(!['active','locked'].includes(p.status)||u.status===p.status)throw Error('Trạng thái tài khoản đã thay đổi.');
  before=structuredClone(u);u.status=p.status;u.statusReason=String(p.reason||'').trim();u.updatedAt=now;u.version++;target=u.id;after=u;summary=p.status==='locked'?'Khóa tài khoản':'Mở khóa tài khoản';
 }else if(action==='role'){
  const old=d.roles.find(r=>r.id===p.id);if(p.id&&!old)throw Error('Không tìm thấy vai trò.');before=old?structuredClone(old):null;
  const name=required(p.name,'tên vai trò');if(d.roles.some(r=>r.id!==p.id&&r.name.toLowerCase()===name.toLowerCase()))throw Error('Tên vai trò đã tồn tại.');
  if(!['all','unit','self'].includes(p.scope))throw Error('Phạm vi vai trò không hợp lệ.');
  if(!Array.isArray(p.permissions)||p.permissions.some(x=>!PERMISSIONS.some(v=>v[0]===x)))throw Error('Quyền chức năng không hợp lệ.');
  if(old?.id==='admin'&&(p.scope!=='all'||core.some(x=>!p.permissions.includes(x))))throw Error('Vai trò quản trị hệ thống phải giữ các quyền quản trị cốt lõi và phạm vi toàn trường.');
  const value={id:old?.id||id('ROLE'),name,description:String(p.description||'').trim(),scope:p.scope,permissions:[...new Set(p.permissions)],version:(old?.version||0)+1,updatedAt:now};
  if(old)d.roles[d.roles.indexOf(old)]=value;else d.roles.push(value);target=value.id;after=value;summary=old?'Cập nhật vai trò và quyền':'Tạo vai trò';
 }else if(action==='role-account'){
  const u=d.users.find(u=>u.id===p.userId),r=d.roles.find(r=>r.id===p.id);
  if(!u||!r||!u.roleIds.includes(r.id))throw Error('Tài khoản không còn thuộc vai trò này. Đóng và mở lại cấu hình.');
  before=structuredClone(u);u.roleOverrides??={};
  if(p.inherit)delete u.roleOverrides[r.id];
  else{
   if(!['all','unit','self'].includes(p.scope)||!Array.isArray(p.permissions)||p.permissions.some(x=>!PERMISSIONS.some(v=>v[0]===x)))throw Error('Quyền hoặc phạm vi tài khoản không hợp lệ.');
   u.roleOverrides[r.id]={scope:p.scope,permissions:[...new Set(p.permissions)]};
  }
  u.version++;u.updatedAt=now;target=u.id;after=u;summary=p.inherit?'Khôi phục quyền theo vai trò cho tài khoản':'Phân quyền riêng cho tài khoản';
 }else if(action==='scope'){
  const u=d.users.find(u=>u.id===p.id);if(!u)throw Error('Không tìm thấy tài khoản.');
  if(!Object.hasOwn(SCOPES,p.scope)||!Array.isArray(p.units)||p.units.some(x=>!adminUnits(d).includes(x))||p.scope==='units'&&!p.units.length)throw Error('Chọn phạm vi và ít nhất một đơn vị khi dùng phạm vi tùy chọn.');
  if(!Array.isArray(p.denied)||p.denied.some(x=>!PERMISSIONS.some(v=>v[0]===x)))throw Error('Quyền hạn chế không hợp lệ.');
  before=structuredClone(u);Object.assign(u,{scope:p.scope,units:p.scope==='units'?[...new Set(p.units)]:[],denied:[...new Set(p.denied)],updatedAt:now,version:u.version+1});target=u.id;after=u;summary='Cập nhật phạm vi và hạn chế tài khoản';
 }else if(action==='department'){
  const old=d.departments.find(k=>k.id===p.id);if(p.id&&!old)throw Error('Không tìm thấy khoa.');
  const code=required(p.code,'mã khoa').toUpperCase(),name=required(p.name,'tên khoa');
  if(!/^[A-Z0-9_-]{2,30}$/.test(code))throw Error('Mã khoa gồm 2–30 chữ không dấu, số, gạch dưới hoặc gạch ngang.');
  if(d.departments.some(k=>k.id!==p.id&&(k.code===code||k.name.toLocaleLowerCase('vi')===name.toLocaleLowerCase('vi')))||ADMIN_UNITS.slice(0,3).some(u=>u.toLocaleLowerCase('vi')===name.toLocaleLowerCase('vi')))throw Error('Mã hoặc tên khoa đã tồn tại.');
  before=old?structuredClone(old):null;
  const value={id:old?.id||id('FAC'),code,name,notes:String(p.notes||'').trim(),updatedAt:now};
  if(old){d.departments[d.departments.indexOf(old)]=value;for(const u of d.users){if(u.unit===old.name||u.units.includes(old.name)){if(u.unit===old.name)u.unit=name;u.units=u.units.map(v=>v===old.name?name:v);u.version++;u.updatedAt=now;}}}else d.departments.push(value);
  target=value.id;after=value;summary=old?'Cập nhật khoa':'Thêm khoa';
 }else if(action==='department-delete'){
  const old=d.departments.find(k=>k.id===p.id);if(!old)throw Error('Không tìm thấy khoa.');
  const linked=d.users.filter(u=>u.unit===old.name||u.units.includes(old.name));
  if(linked.length&&(!adminUnits(d).includes(p.replacement)||p.replacement===old.name))throw Error('Chọn đơn vị chuyển các tài khoản và phạm vi đang liên kết.');
  before=structuredClone(old);for(const u of linked){if(u.unit===old.name)u.unit=p.replacement;u.units=[...new Set(u.units.map(v=>v===old.name?p.replacement:v))];u.version++;u.updatedAt=now;}
  d.departments=d.departments.filter(k=>k.id!==old.id);target=old.id;after={deleted:true,replacement:p.replacement||'',affectedAccounts:linked.length};summary='Xóa khoa';
 }else throw Error('Thao tác không hợp lệ.');
 const current=d.users.find(u=>u.id===CURRENT_ADMIN);
 if(current.status!=='active'||current.scope!=='all'||core.some(permission=>!accessCheck(d,{userId:CURRENT_ADMIN,permission,unit:ADMIN_UNITS[0],ownerId:'other'}).allowed))throw Error('Không thể thu hồi quyền quản trị của tài khoản đang thao tác.');
 d.revision++;d.audit.push({id:id('AUD'),at:now,actor:CURRENT_ADMIN,action,target,summary,before,after:structuredClone(after)});return d;
}

