import {SESSION_KEY,preparePasswordReset} from './demo-auth.mjs';
import {ADMIN_KEY,normalizeAdmin,validAdmin,accessCheck} from './admin-data.mjs';
function authorized(){
 const db=normalizeAdmin(JSON.parse(localStorage.getItem(ADMIN_KEY)||'null')),session=JSON.parse(sessionStorage.getItem(SESSION_KEY)||'null');
 if(!validAdmin(db)||!session?.userId||session.roleId!=='admin'||!accessCheck(db,{userId:session.userId,permission:'users.manage',unit:'Phòng Quản trị',ownerId:'other'}).allowed)throw Error('Vui lòng đăng nhập bằng tài khoản quản trị có quyền quản lý tài khoản.');
 return {db,session};
}
const dialog=document.createElement('dialog');dialog.id='reset-password-dialog';dialog.setAttribute('aria-labelledby','reset-title');document.body.append(dialog);
let target='',revision=0;
document.addEventListener('click',e=>{
 const button=e.target.closest('[data-action="reset-password"]');if(!button)return;
 e.preventDefault();
 dialog.innerHTML='<h2 id="reset-title">Đặt lại mật khẩu</h2><p id="reset-user"></p><form id="reset-password-form"><p>Mật khẩu cũ sẽ được thay thế. Việc đặt lại không mở khóa tài khoản.</p><label for="reset-new">Mật khẩu mới</label><input id="reset-new" type="password" autocomplete="new-password" required minlength="8" maxlength="128"><label for="reset-confirm">Xác nhận mật khẩu mới</label><input id="reset-confirm" type="password" autocomplete="new-password" required minlength="8" maxlength="128"><p>Gồm 8–128 ký tự, có chữ và số.</p><label><input id="reset-show" type="checkbox"> Hiện mật khẩu</label><label><input name="confirm" type="checkbox" required> Tôi xác nhận cấp lại mật khẩu cho tài khoản này.</label><p id="reset-error" role="status" aria-live="polite"></p><div class="form-actions"><button type="button" class="btn" id="reset-close">Đóng</button><button type="submit" class="btn primary">Đặt lại mật khẩu</button></div></form>';
 dialog.querySelector('#reset-close').onclick=()=>dialog.close();dialog.querySelector('#reset-show').onchange=e=>dialog.querySelectorAll('input[autocomplete="new-password"]').forEach(i=>i.type=e.target.checked?'text':'password');
 try{const {db}=authorized(),user=db.users.find(u=>u.id===button.dataset.id);if(!user)throw Error('Không tìm thấy tài khoản.');target=user.id;revision=db.revision;dialog.querySelector('#reset-user').textContent=user.name+' · '+user.username;}catch(err){dialog.querySelector('#reset-error').textContent=err.message;dialog.querySelector('button[type=submit]').disabled=true;}
 dialog.showModal();
});
dialog.addEventListener('close',()=>{dialog.replaceChildren();target='';});
dialog.addEventListener('submit',async e=>{
 e.preventDefault();const submit=dialog.querySelector('button[type=submit]'),error=dialog.querySelector('#reset-error');submit.disabled=true;
 try{
 const password=dialog.querySelector('#reset-new').value;if(password!==dialog.querySelector('#reset-confirm').value)throw Error('Xác nhận mật khẩu mới chưa khớp.');
 const id=target,commit=await preparePasswordReset(id,password),{db,session}=authorized();
 if(db.revision!==revision||!db.users.some(u=>u.id===id))throw Error('Dữ liệu đã thay đổi. Đóng và mở lại biểu mẫu.');
 commit();
 db.audit.push({id:'PWD-'+crypto.randomUUID(),at:new Date().toISOString(),actor:session.userId,action:'password-reset',target:id,summary:'Đặt lại mật khẩu tài khoản',before:null,after:{id,status:'Đã đặt lại mật khẩu demo'}});db.revision++;
 try{localStorage.setItem(ADMIN_KEY,JSON.stringify(db));}catch{throw Error('Đã đổi mật khẩu nhưng chưa lưu được nhật ký quản trị.');}
 e.target.reset();error.textContent='Đã đặt lại mật khẩu thành công. Hãy cung cấp mật khẩu mới cho người dùng.';error.style.color='#176543';
 }catch(err){error.style.color='#a32e37';error.textContent=err.message;submit.disabled=false;}
});
