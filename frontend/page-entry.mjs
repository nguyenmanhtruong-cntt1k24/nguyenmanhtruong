import {demo,requireSession,markConnected} from './server-session.mjs';
const page=location.pathname.split('/').pop()||'index.html';
// Hướng dẫn là tính năng bổ trợ; lỗi tải không được chặn đăng nhập hoặc nghiệp vụ.
if(['index.html','resources.html','admin.html','executive.html','accounting.html'].includes(page)){
 import('./page-help.mjs').catch(()=>console.warn('Chưa tải được hướng dẫn cho trang này.'));
}
const modules={'index.html':'app.js?v=backend-complete-20261104.4','resources.html':'resources.js?v=frontend-complete-20261104.3','admin.html':'admin.js','executive.html':'executive.js','accounting.html':'accounting.js?v=khtc-separate-20261125','payment-voucher.html':'payment-voucher.js','login.html':demo?'login.js':'server-login.mjs'};
if(demo)document.addEventListener('click',e=>{const a=e.target.closest('a[href]');if(!a)return;const url=new URL(a.href,location.href);if(url.origin===location.origin&&modules[url.pathname.split('/').pop()]){url.searchParams.set('mode','demo');a.href=url.href;}},true);
if(!demo&&['executive.html','accounting.html','payment-voucher.html'].includes(page)){
 try{await import(page==='payment-voucher.html'?'./server-voucher.mjs?v=frontend-complete-20261104.3':'./server-finance.mjs?v=khtc-live-20261125');}catch(e){const main=document.querySelector('#main')||document.body;const p=document.createElement('p');p.className='error-banner';p.textContent='Không tải được tài chính: '+e.message;main.replaceChildren(p);}
}else if(modules[page]){try{await import('./'+modules[page]);if(page==='login.html')document.querySelector('#login-form button[type=submit]').disabled=false;if(demo&&page==='admin.html')await import('./admin-password.js');}catch(e){const main=document.querySelector('#main')||document.body;const p=document.createElement('p');p.className='error-banner';p.setAttribute('role','alert');p.textContent='Không tải được dữ liệu: '+e.message+' Vui lòng tải lại trang.';main.replaceChildren(p);}}
