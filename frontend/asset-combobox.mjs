// Keyboard/ARIA interaction follows the W3C APG editable combobox pattern:
// https://github.com/w3c/aria-practices/tree/main/content/patterns/combobox
export function mountAssetCombobox(select,{items,fetchPage,onChoose}){
 const field=select.closest('.field'),dialog=select.closest('dialog'),originalId=select.id;
 const selected=items.find(a=>a.id===select.value);
 select.hidden=true;select.id=originalId+'-value';select.required=false;
 const box=document.createElement('div');box.className='asset-combobox';
 box.innerHTML=`<div class="asset-combobox-control"><input id="${originalId}" role="combobox" aria-autocomplete="list" aria-expanded="false" aria-controls="asset-options" autocomplete="off" required placeholder="Gõ mã hoặc tên tài sản…"><button type="button" aria-label="Mở danh sách tài sản" aria-controls="asset-options" aria-expanded="false">⌄</button></div><div class="asset-combobox-popup" hidden><div id="asset-options" role="listbox" aria-label="Tài sản"></div><div class="asset-combobox-footer"><button type="button" data-prev>Trước</button><span role="status" aria-live="polite"></span><button type="button" data-next>Sau</button></div></div>`;
 field.append(box);
 const input=box.querySelector('input'),toggle=box.querySelector('.asset-combobox-control button'),popup=box.querySelector('.asset-combobox-popup'),list=box.querySelector('[role=listbox]'),status=box.querySelector('[role=status]'),prev=box.querySelector('[data-prev]'),next=box.querySelector('[data-next]');
 let rows=items,page=1,total=items.length,query='',revision=0,timer,active=-1;
 const label=a=>`${a.name}${a.api?.code||a.code?' · '+(a.api?.code||a.code):''}`;
 input.value=selected?label(selected):'';
 const alive=()=>box.isConnected&&dialog.open;
 function open(value){popup.hidden=!value;input.setAttribute('aria-expanded',String(value));toggle.setAttribute('aria-expanded',String(value));if(!value){active=-1;input.removeAttribute('aria-activedescendant');}}
 function highlight(index){active=index;[...list.children].forEach((e,i)=>e.setAttribute('aria-selected',String(i===active)));const option=list.children[active];if(option){input.setAttribute('aria-activedescendant',option.id);option.scrollIntoView({block:'nearest'});}}
 function choose(a){
  if(![...select.options].some(o=>o.value===a.id))select.add(new Option(a.name,a.id));
  select.value=a.id;input.value=label(a);input.setCustomValidity('');onChoose(a);select.dispatchEvent(new Event('change',{bubbles:true}));open(false);input.focus();
 }
 function render(){list.replaceChildren();active=-1;input.removeAttribute('aria-activedescendant');
  rows.forEach((a,i)=>{const option=document.createElement('div');option.id='asset-option-'+i;option.setAttribute('role','option');option.setAttribute('aria-selected','false');option.textContent=label(a);option.onpointerdown=e=>e.preventDefault();option.onclick=()=>choose(a);list.append(option);});
  status.textContent=total?`${total} tài sản · Trang ${page}/${Math.max(1,Math.ceil(total/20))}`:'Không tìm thấy tài sản';prev.disabled=page<=1;next.disabled=page*20>=total;
 }
 async function load(){const current=++revision;status.textContent='Đang tìm tài sản…';prev.disabled=next.disabled=true;
  try{const result=await fetchPage({page,search:query});if(current!==revision||!alive())return;rows=result.items;total=result.total;render();}
  catch(e){if(current===revision&&alive()){rows=[];list.replaceChildren();input.removeAttribute('aria-activedescendant');status.textContent=e.message||'Không tải được tài sản. Hãy thử lại.';}}
 }
 input.oninput=()=>{select.value='';input.setCustomValidity('Chọn một tài sản trong danh sách kết quả.');query=input.value.trim();page=1;revision++;rows=[];active=-1;input.removeAttribute('aria-activedescendant');list.replaceChildren();status.textContent='Đang tìm tài sản…';prev.disabled=next.disabled=true;open(true);clearTimeout(timer);timer=setTimeout(load,250);};
 toggle.onclick=()=>{input.focus();if(!popup.hidden){open(false);return;}clearTimeout(timer);query=select.value?'':input.value.trim();page=1;open(true);load();};
 input.onkeydown=e=>{
  if(e.key==='ArrowDown'||e.key==='ArrowUp'){e.preventDefault();if(popup.hidden){open(true);query=select.value?'':input.value.trim();page=1;load();}else if(rows.length)highlight(active<0?(e.key==='ArrowDown'?0:rows.length-1):(active+(e.key==='ArrowDown'?1:-1)+rows.length)%rows.length);}
  if(e.key==='Enter'&&!popup.hidden){e.preventDefault();e.stopPropagation();if(active>=0&&rows[active])choose(rows[active]);else if(rows.length===1)choose(rows[0]);}
  if(e.key==='Escape'&&!popup.hidden){e.preventDefault();e.stopPropagation();open(false);}
  if(e.key==='Tab')open(false);
 };
 box.onfocusout=()=>setTimeout(()=>{if(!box.contains(document.activeElement))open(false);},0);
 prev.onclick=()=>{if(page>1){page--;load();}};next.onclick=()=>{if(page*20<total){page++;load();}};
 render();
}
