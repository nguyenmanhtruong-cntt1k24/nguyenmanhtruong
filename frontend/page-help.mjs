import {PAGE_GUIDES} from './page-help-data.mjs';

const workspace=location.pathname.split('/').pop()||'index.html';
const guides=PAGE_GUIDES[workspace];
const main=document.querySelector('#main');
if(guides&&main){
 const style=document.createElement('link');
 style.rel='stylesheet';style.href=new URL('./page-help.css',import.meta.url).href;document.head.append(style);
 const dialog=document.createElement('dialog');
 dialog.className='page-help-dialog';dialog.setAttribute('aria-labelledby','page-help-title');
 dialog.innerHTML='<div class="page-help-head"><h2 id="page-help-title"></h2><button type="button" class="page-help-close" aria-label="Đóng hướng dẫn">Đóng ✕</button></div><div class="page-help-content"></div>';
 document.body.append(dialog);
 let opener;
 const current=()=>guides[location.hash.replace(/^#\//,'')||'overview'];
 function addButton(){
  const heading=main.querySelector('.page-heading');
  if(!heading||heading.querySelector('[data-page-help]')||!current())return;
  const button=document.createElement('button');button.type='button';button.className='btn page-help-button';button.dataset.pageHelp='';button.textContent='Hướng dẫn';button.setAttribute('aria-haspopup','dialog');
  const actions=heading.querySelector('.heading-actions');
  (actions||heading).append(button);
 }
 function openGuide(button){
  const guide=current();if(!guide)return;
  opener=button;dialog.querySelector('h2').textContent='Hướng dẫn '+guide.title;
  const content=dialog.querySelector('.page-help-content');content.replaceChildren();
  for(const block of guide.blocks){
   const el=document.createElement(block.type);
   if(block.items){for(const text of block.items){const li=document.createElement('li');li.textContent=text;el.append(li);}}
   else el.textContent=block.text;
   content.append(el);
  }
  if(!dialog.open)dialog.showModal();dialog.scrollTop=0;
 }
 document.addEventListener('click',event=>{
  const target=event.target.closest('[data-page-help], [data-action="help"], [data-action="guide"]');
  if(!target||!current())return;
  event.preventDefault();event.stopImmediatePropagation();openGuide(target);
 },true);
 dialog.querySelector('.page-help-close').addEventListener('click',()=>dialog.close());
 dialog.addEventListener('click',event=>{if(event.target===dialog){const r=dialog.getBoundingClientRect();if(event.clientX<r.left||event.clientX>r.right||event.clientY<r.top||event.clientY>r.bottom)dialog.close();}});
 dialog.addEventListener('close',()=>{if(opener?.isConnected)opener.focus();});
 window.addEventListener('hashchange',()=>{if(dialog.open)dialog.close();addButton();});
 new MutationObserver(addButton).observe(main,{childList:true,subtree:true});
 addButton();
}
