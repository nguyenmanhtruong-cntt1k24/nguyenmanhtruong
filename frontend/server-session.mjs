import {HpuniApi} from './api-client.mjs';
export const demo = new URLSearchParams(location.search).get('mode') === 'demo';
export const api = new HpuniApi();
export let identity = null;
export const can = p => identity?.active && !identity.denied.includes(p) && identity.grants.some(g=>g.permissions.includes(p));
// Department-owned records have no personal owner: self does not imply unit.
export function allowsDepartment(profile,permission,departmentId){
  const match=scope=>scope==='all'||scope==='unit'&&departmentId===profile?.departmentId||scope==='units'&&profile?.departmentIds?.includes(departmentId);
  return !!(profile?.active&&!profile.denied?.includes(permission)&&match(profile.scope)&&profile.grants?.some(g=>g.permissions.includes(permission)&&match(g.scope)));
}
export const canForDepartment=(permission,departmentId,profile=identity)=>allowsDepartment(profile,permission,departmentId);
export const departmentsFor=(permission,departments)=>departments.filter(d=>canForDepartment(permission,d.id));
export async function requireSession(permission) {
  if(demo)return null;
  try {
    identity=await api.me();
    if(permission&&!(Array.isArray(permission)?permission.some(can):can(permission)))throw new Error('Tài khoản không có quyền vào không gian này.');
    return identity;
  } catch(e) {
    if(e.status===401)location.replace('login.html');
    else { const main=document.querySelector('#main')||document.body;main.replaceChildren();const p=document.createElement('p');p.className='error-banner';p.textContent=e.message+' Hãy tải lại trang hoặc đăng nhập bằng tài khoản phù hợp.';main.append(p);const a=document.createElement('a');a.href='login.html';a.textContent='Đăng nhập';main.append(a); }
    return await new Promise(()=>{});
  }
}
export function markConnected() {
  api.request('/data-status').then(status=>{
    if(!status.hasSampleData)return;
    document.querySelectorAll('.demo-label').forEach(e=>{e.textContent='SQL Server · Có dữ liệu [MẪU]';});
    if(!document.querySelector('#sample-data-notice')){
      const notice=document.createElement('p');notice.id='sample-data-notice';notice.setAttribute('role','note');
      notice.style.cssText='margin:16px 24px 0;padding:12px 16px;background:#fff6dc;border:1px solid #ead9a7;border-radius:8px;color:#725514;font-size:12px;line-height:1.6';
      notice.textContent='Bản ghi có nhãn [MẪU] dùng để minh họa. Các số liệu tổng hợp và báo cáo hiện bao gồm những bản ghi này.';
      document.querySelector('#main')?.before(notice);
    }
  }).catch(()=>{});
  import('./notifications.mjs?v=frontend-complete-20261104.3').then(m=>m.mountNotifications()).catch(()=>{});
  if(!document.querySelector('#server-mobile-header-style')){const style=document.createElement('style');style.id='server-mobile-header-style';style.textContent='@media(max-width:760px){#server-refresh{display:none}.topbar .breadcrumb{min-width:0;flex:1;overflow:hidden}.topbar .breadcrumb strong{overflow:hidden;text-overflow:ellipsis;white-space:nowrap}.topbar .top-actions{flex-shrink:0;gap:6px}.topbar .top-actions:has(#notification-open) [data-action="notifications"]{display:none}#notification-open{font-size:10px;padding:8px}.topbar .profile{gap:4px}}';document.head.append(style);}
  if(!document.querySelector('#server-extension-forms')){const style=document.createElement('style');style.id='server-extension-forms';style.textContent='.error-banner:empty{display:none}#finance-controls{padding:20px}#asset-lifecycle label,#finance-controls label,#server-reports label{display:inline-flex;flex-direction:column;gap:7px;max-width:100%;font-size:12px}#asset-lifecycle input,#asset-lifecycle select,#asset-lifecycle textarea,#finance-controls input,#finance-controls select,#server-reports select,#server-reports input{width:100%;max-width:100%;min-width:0;padding:10px;border:1px solid #ccd8e5;border-radius:7px;background:white;color:#172033;font:inherit}#finance-controls form{display:flex;flex-wrap:wrap;align-items:end;gap:12px;margin:16px 0}#finance-controls td form{min-width:180px}#asset-lifecycle h3,#finance-controls h3{margin:20px 0 12px}';document.head.append(style);}
  document.querySelectorAll('.demo-label').forEach(e=>e.textContent='Dữ liệu máy chủ · SQL Server');
  document.querySelectorAll('.semester').forEach(e=>e.textContent='Phiên đăng nhập được bảo vệ');
  if(!document.querySelector('#server-refresh')&&document.querySelector('.top-actions')){const b=document.createElement('button');b.id='server-refresh';b.className='btn';b.textContent='Tải lại dữ liệu';b.onclick=()=>location.reload();document.querySelector('.top-actions').prepend(b);}
}
export async function all(path) {
  const rows=[];
  for(let page=1;page<=100000;page++){
    const r=await api.request(`${path}${path.includes('?')?'&':'?'}page=${page}&pageSize=100`);
    if(!Array.isArray(r.items)||!Number.isInteger(r.total)||r.total<0)throw Error('Máy chủ trả về danh sách không hợp lệ.');
    rows.push(...r.items);if(rows.length>=r.total)return rows;
    if(!r.items.length)throw Error('Danh sách đã thay đổi trong khi tải. Vui lòng tải lại dữ liệu.');
  }
  throw Error('Danh sách vượt phạm vi phân trang của máy chủ. Hãy thu hẹp bộ lọc.');
}
const attempts=new Map();
export async function keyed(path,data) {
  const signature=JSON.stringify([path,data]);const key=attempts.get(signature)||crypto.randomUUID();attempts.set(signature,key);
  const result=await api.keyed(path,data,key);attempts.delete(signature);return result;
}
export const day=v=>new Date(new Date(v).getTime()+7*3600000).toISOString().slice(0,10);
export const time=v=>new Date(new Date(v).getTime()+7*3600000).toISOString().slice(11,16);
export const instant=(date,time)=>`${date}T${time.length===5?time+':00':time}+07:00`;
export const localInstant=v=>v?.length===16?v+':00+07:00':v;
