import {SESSION_KEY,changePassword} from './demo-auth.mjs';
import {ADMIN_KEY,adminSeed,normalizeAdmin,validAdmin} from './admin-data.mjs';
const profile=document.querySelector('.profile');
if(profile){
 function updateIdentity(){
  try{
   const session=JSON.parse(sessionStorage.getItem(SESSION_KEY)||'null'),raw=localStorage.getItem(ADMIN_KEY),db=raw?normalizeAdmin(JSON.parse(raw)):adminSeed();
   const user=validAdmin(db)&&db.users.find(u=>u.id===session?.userId);
   const name=profile.querySelector('strong'),subtitle=profile.querySelector('small'),avatar=profile.querySelector('.avatar');
   if(!user){if(name)name.textContent='Chưa đăng nhập';if(subtitle)subtitle.textContent='Vui lòng đăng nhập tài khoản';if(avatar)avatar.textContent='?';return;}
   if(name)name.textContent=user.name;
   const role=db.roles.find(r=>r.id===session.roleId&&user.roleIds.includes(r.id));
   if(subtitle)subtitle.textContent=[role?.name,user.unit].filter(Boolean).join(' · ');
   if(avatar)avatar.textContent=user.name.trim().split(/\s+/).slice(-2).map(s=>s[0]).join('').toLocaleUpperCase('vi');
  }catch{profile.querySelector('strong').textContent='Không đọc được tài khoản';}
 }
 updateIdentity();window.addEventListener('storage',e=>{if(e.key===ADMIN_KEY)updateIdentity();});window.addEventListener('focus',updateIdentity);window.addEventListener('pageshow',updateIdentity);window.addEventListener('hashchange',updateIdentity);
 const observer=new MutationObserver(updateIdentity);observer.observe(document.querySelector('#main'),{childList:true});
 profile.removeAttribute('data-action');profile.setAttribute('aria-expanded','false');profile.setAttribute('aria-controls','account-menu');
 const menu=document.createElement('div');menu.id='account-menu';menu.className='account-menu';menu.hidden=true;menu.innerHTML='<button type="button" id="change-password">Đổi mật khẩu</button><button type="button" class="logout" id="logout">Đăng xuất</button>';document.body.append(menu);
 const dialog=document.createElement('dialog');dialog.id='password-dialog';dialog.setAttribute('aria-labelledby','pw-title');dialog.innerHTML=`<h2 id="pw-title">Đổi mật khẩu</h2><p>Mật khẩu mới cần 8–128 ký tự, gồm chữ và số. Thay đổi áp dụng cho tài khoản đang đăng nhập trên trình duyệt này.</p><form id="password-form"><label for="pw-current">Mật khẩu hiện tại</label><input id="pw-current" type="password" autocomplete="current-password" required maxlength="128"><label for="pw-new">Mật khẩu mới</label><input id="pw-new" type="password" autocomplete="new-password" required minlength="8" maxlength="128"><label for="pw-confirm">Xác nhận mật khẩu mới</label><input id="pw-confirm" type="password" autocomplete="new-password" required minlength="8" maxlength="128"><label class="pw-show"><input id="pw-show" type="checkbox">Hiện mật khẩu</label><p id="pw-message" role="status" aria-live="polite"></p><div class="pw-actions"><button type="button" id="pw-close">Đóng</button><button type="submit">Lưu mật khẩu</button></div></form>`;document.body.append(dialog);
 const closeMenu=()=>{menu.hidden=true;profile.setAttribute('aria-expanded','false');};
 profile.addEventListener('click',()=>{menu.hidden=!menu.hidden;profile.setAttribute('aria-expanded',String(!menu.hidden));const r=profile.getBoundingClientRect();menu.style.top=`${r.bottom+8}px`;menu.style.right=`${Math.max(12,innerWidth-r.right)}px`;if(!menu.hidden)menu.querySelector('button').focus();});
 document.addEventListener('click',e=>{if(!menu.contains(e.target)&&!profile.contains(e.target))closeMenu();});
 document.addEventListener('keydown',e=>{if(e.key==='Escape'&&!menu.hidden){closeMenu();profile.focus();}});window.addEventListener('resize',closeMenu);
 menu.querySelector('#logout').addEventListener('click',()=>{sessionStorage.removeItem(SESSION_KEY);location.assign('login.html');});
 menu.querySelector('#change-password').addEventListener('click',()=>{closeMenu();dialog.querySelector('form').reset();dialog.querySelector('#pw-message').textContent='';for(const input of dialog.querySelectorAll('input:not([type=checkbox])'))input.type='password';dialog.querySelector('button[type=submit]').disabled=false;dialog.showModal();});
 dialog.querySelector('#pw-close').addEventListener('click',()=>dialog.close());dialog.addEventListener('close',()=>{dialog.querySelector('form').reset();profile.focus();});
 dialog.querySelector('#pw-show').addEventListener('change',e=>{for(const input of dialog.querySelectorAll('input:not([type=checkbox])'))input.type=e.target.checked?'text':'password';});
 dialog.querySelector('form').addEventListener('submit',async e=>{e.preventDefault();const message=dialog.querySelector('#pw-message'),submit=dialog.querySelector('button[type=submit]');message.className='';submit.disabled=true;try{
 const session=JSON.parse(sessionStorage.getItem(SESSION_KEY)||'null');if(!session?.userId)throw Error('Vui lòng đăng nhập trước khi đổi mật khẩu.');
 const raw=localStorage.getItem(ADMIN_KEY),db=raw?normalizeAdmin(JSON.parse(raw)):adminSeed();if(!validAdmin(db)||!db.users.some(u=>u.id===session.userId&&u.status==='active'))throw Error('Tài khoản không khả dụng. Vui lòng đăng nhập lại.');
 const next=dialog.querySelector('#pw-new').value;if(next!==dialog.querySelector('#pw-confirm').value)throw Error('Xác nhận mật khẩu mới chưa khớp.');
 await changePassword(session.userId,dialog.querySelector('#pw-current').value,next);e.target.reset();message.className='success';message.textContent='Đã đổi mật khẩu thành công. Hãy dùng mật khẩu mới ở lần đăng nhập tiếp theo.';
 }catch(err){message.textContent=err.message;}finally{submit.disabled=false;}});
}
