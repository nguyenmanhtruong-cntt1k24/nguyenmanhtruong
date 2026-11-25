import {api,identity,demo} from './server-session.mjs';

// SQL-backed task inbox. Text is assigned through DOM APIs, never interpreted as HTML.
export function mountNotifications(){
  if(demo||!identity?.active||document.querySelector('#notification-open'))return;
  const host=document.querySelector('.top-actions');if(!host)return;
  const button=document.createElement('button');button.id='notification-open';button.className='btn';button.type='button';button.textContent='Thông báo';
  button.setAttribute('aria-haspopup','dialog');host.prepend(button);
  const dialog=document.createElement('dialog');dialog.id='hpuni-notifications-dialog';
  dialog.style.cssText='width:650px;max-width:calc(100vw - 24px);max-height:85vh;border:0;border-radius:16px;padding:24px;overflow:auto;box-sizing:border-box';
  const heading=document.createElement('h2');heading.id='hpuni-notifications-heading';heading.textContent='Thông báo và nhắc việc';dialog.setAttribute('aria-labelledby',heading.id);
  const note=document.createElement('p');note.textContent='Công việc được cập nhật từ hồ sơ hiện tại theo quyền của bạn. Đã đọc không có nghĩa là đã xử lý.';note.style.cssText='font-size:13px;color:#64748b;margin:12px 0';
  const controls=document.createElement('div');controls.style.cssText='display:flex;gap:12px;align-items:center;flex-wrap:wrap;margin-bottom:12px';
  const label=document.createElement('label'),filter=document.createElement('input');filter.type='checkbox';label.append(filter,document.createTextNode(' Chỉ chưa đọc'));
  const refresh=document.createElement('button');refresh.className='btn';refresh.type='button';refresh.textContent='Làm mới';controls.append(label,refresh);
  const status=document.createElement('p');status.setAttribute('role','status');status.style.cssText='margin:8px 0;overflow-wrap:anywhere';
  const list=document.createElement('div'),pager=document.createElement('div');pager.style.cssText='display:flex;align-items:center;gap:12px;flex-wrap:wrap;margin:16px 0';
  const previous=document.createElement('button'),next=document.createElement('button'),position=document.createElement('span');
  previous.textContent='Trang trước';next.textContent='Trang sau';for(const b of[previous,next]){b.type='button';b.className='btn';}pager.append(previous,position,next);
  const close=document.createElement('button');close.className='btn primary';close.type='button';close.textContent='Đóng';close.onclick=()=>dialog.close();
  dialog.append(heading,note,controls,status,list,pager,close);document.body.append(dialog);
  let page=1,loading=false;
  async function badge(){
    if(!identity?.active||document.hidden)return;
    try{const r=await api.request('/notifications?pageSize=1&unreadOnly=true');button.textContent=`Thông báo (${r.unread>99?'99+':r.unread})`;}
    catch{button.textContent='Thông báo';}
  }
  async function load(){
    if(loading)return;loading=true;refresh.disabled=true;previous.disabled=true;next.disabled=true;status.textContent='Đang tải thông báo…';
    try{
      const r=await api.request('/notifications?'+new URLSearchParams({page:String(page),pageSize:'20',unreadOnly:String(filter.checked)}));
      if(page>1&&r.items.length===0){page=1;loading=false;return load();}
      button.textContent=`Thông báo (${r.unread>99?'99+':r.unread})`;list.replaceChildren();
      status.textContent=r.total?`${r.total} công việc · ${r.unread} chưa đọc`:'Không có công việc phù hợp.';
      for(const item of r.items){
        const row=document.createElement('article');row.style.cssText='padding:14px 0;border-bottom:1px solid #e2e8f0;overflow-wrap:anywhere';
        const title=document.createElement('h3');title.style.cssText='font-size:15px;margin:0 0 6px';title.textContent=(item.read?'':'● ')+item.title;if(item.severity==='warning')title.style.color='#b45309';
        const message=document.createElement('p');message.style.cssText='font-size:14px;margin:4px 0';message.textContent=item.message;
        const detail=document.createElement('p');detail.style.cssText='font-size:12px;color:#64748b;margin:4px 0 8px';detail.textContent=`${item.sourceId} · ${new Date(item.at).toLocaleString('vi-VN',{timeZone:'Asia/Ho_Chi_Minh'})}`;
        const actions=document.createElement('div');actions.style.cssText='display:flex;gap:12px;align-items:center;flex-wrap:wrap';
        const url=new URL(item.url,location.href);if(url.origin===location.origin){const link=document.createElement('a');link.href=url.href;link.textContent='Mở danh sách hồ sơ';link.onclick=()=>dialog.close();actions.append(link);}
        if(!item.read){const read=document.createElement('button');read.type='button';read.className='btn';read.textContent='Đánh dấu đã đọc';read.onclick=async()=>{
          read.disabled=true;
          try{await api.request(`/notifications/${encodeURIComponent(item.id)}/read`,{method:'POST',data:{}});await load();}
          catch(e){status.textContent=e.status===404?'Hồ sơ đã thay đổi hoặc bạn không còn quyền xem. Hãy làm mới.':e.message;read.disabled=false;}
        };actions.append(read);}
        row.append(title,message,detail,actions);list.append(row);
      }
      position.textContent=`Trang ${page} / ${Math.max(1,Math.ceil(r.total/20))}`;previous.disabled=page===1;next.disabled=page*20>=r.total;
    }catch(e){list.replaceChildren();status.textContent='Không tải được thông báo: '+e.message;}
    finally{loading=false;refresh.disabled=false;}
  }
  function open(){page=1;if(!dialog.open)dialog.showModal();load();}
  button.onclick=open;filter.onchange=()=>{page=1;load();};refresh.onclick=()=>load();previous.onclick=()=>{page--;load();};next.onclick=()=>{page++;load();};
  // Existing bell buttons across the original role layouts open the same live inbox.
  document.addEventListener('click',e=>{if(e.target.closest('[data-action="notifications"]')){e.preventDefault();e.stopImmediatePropagation();open();}},true);
  badge();document.addEventListener('visibilitychange',()=>{if(!document.hidden)badge();});
  setInterval(()=>{if(!document.hidden&&!dialog.open)badge();},60000);
}
