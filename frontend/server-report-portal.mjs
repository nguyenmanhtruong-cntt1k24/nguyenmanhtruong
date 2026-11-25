import {requireSession,can,markConnected} from './server-session.mjs';
import {mountReports} from './server-reports.mjs?v=frontend-complete-20261104.3';
await requireSession();
if(!can('reports.read')&&!can('reports.export')){
 document.querySelector('#server-reports').textContent='Tài khoản chưa có quyền xem hoặc xuất báo cáo.';
}else{
 markConnected();await mountReports(document.querySelector('#server-reports'),{finance:can('budget.read')});
}
