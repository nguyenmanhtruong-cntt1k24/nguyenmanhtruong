import {api} from './server-session.mjs';
const form=document.querySelector('#login-form'),password=form.elements.password,message=document.querySelector('#message');let user;
const destinations={admin:'admin.html',resources:'resources.html',lecturer:'index.html',accounting:'accounting.html',executive:'executive.html',reports:'reports.html',khtc:'khtc.html'};
let workspaces=[];
function availableWorkspaces(profile){
 const has=permission=>profile.active&&!profile.denied.includes(permission)&&profile.grants.some(grant=>grant.permissions.includes(permission));
 const global=permission=>has(permission)&&profile.scope==='all'&&profile.grants.some(grant=>grant.scope==='all'&&grant.permissions.includes(permission));
 const allowed={admin:has('users.manage')||['roles.manage','permissions.manage','departments.manage','audit.read'].some(global),resources:['resources.manage','requests.review','inventory.manage','logs.review'].some(has),khtc:has('budget.read')&&has('budget.appraise'),executive:has('budget.read')&&has('budget.approve'),accounting:has('budget.read')&&(has('budget.write')||has('transactions.write')),lecturer:['rooms.book','devices.borrow','logs.write'].some(has),reports:['reports.read','reports.export'].some(has)};
 return [...new Set([...(profile.roleIds||[]).filter(id=>allowed[id]),...Object.keys(allowed).filter(id=>allowed[id])])];
}
document.querySelector('.demo-accounts').hidden=true;document.querySelector('.demo-tag').textContent='Kết nối máy chủ';document.querySelector('.demo-note').textContent='Đăng nhập bằng tài khoản trên SQL Server. Dữ liệu demo không được nhập vào hệ thống.';
document.querySelector('label[for=username]').textContent='Tên đăng nhập';
form.elements.username.placeholder='Nhập tên đăng nhập';
function show(text){message.textContent=text;message.hidden=false;}
function enter(role){if(!workspaces.includes(role))return;sessionStorage.setItem('hpuni-workspace-role',role);location.assign(destinations[role]+'#/overview');}
document.querySelector('#toggle-password').addEventListener('click',e=>{password.type=password.type==='password'?'text':'password';e.currentTarget.textContent=password.type==='password'?'Hiện':'Ẩn';});
document.querySelector('#help').addEventListener('click',()=>show('Liên hệ quản trị viên để cấp lại mật khẩu. Tài khoản demo không dùng cho hệ thống thật.'));
form.addEventListener('submit',async e=>{e.preventDefault();const button=e.submitter;if(button.disabled)return;button.disabled=true;message.hidden=true;workspaces=[];try{user=await api.login(form.elements.username.value.trim(),password.value);password.value='';workspaces=availableWorkspaces(user);if(!workspaces.length)throw Error('Tài khoản chưa có quyền vào không gian làm việc được hỗ trợ. Liên hệ quản trị viên.');enter(workspaces[0]);}catch(e){show(e.message);}finally{button.disabled=false;}});
