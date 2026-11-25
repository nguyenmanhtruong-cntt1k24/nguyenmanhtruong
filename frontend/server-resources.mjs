import {api,all,keyed,day,time,instant,localInstant,can,identity} from './server-session.mjs';
export function resourceAllows(profile,permission,departmentId,ownerId=null){
  if(!profile?.active||profile.denied?.includes(permission))return false;
  const matches=scope=>scope==='all'||scope==='unit'&&departmentId===profile.departmentId||scope==='units'&&profile.departmentIds?.includes(departmentId)||scope==='self'&&ownerId!=null&&ownerId===profile.id;
  return matches(profile.scope)&&profile.grants.some(g=>g.permissions.includes(permission)&&matches(g.scope));
}
export function resourceDepartments(departments,profile,permission){return departments.filter(d=>d.active&&resourceAllows(profile,permission,d.id));}
export function assetPayload(fields,prior=null){return{code:fields.code,name:fields.name,category:fields.category,unit:fields.unit,departmentId:fields.departmentId,active:fields.active==null?(prior?.active??true):fields.active===true||fields.active==='true',version:prior?.version};}
export async function mapBounded(rows,work,limit=4){
  const result=new Array(rows.length);let cursor=0;
  await Promise.all(Array.from({length:Math.min(limit,rows.length)},async()=>{while(cursor<rows.length){const i=cursor++;result[i]=await work(rows[i],i);}}));
  return result;
}
export function logPayload(request,fields,version=null){
  const used=fields.used!=='false',room=request.kind==='room';
  return{used,actualStart:used?instant(request.date,fields.start):null,actualEnd:used?instant(request.date,fields.end):null,
    condition:fields.condition||'',issue:fields.notes||'',version,
    ...(room?{attendees:used?Number(fields.students):null}:{quantity:used?Number(fields.quantity||request.quantity):null})};
}
export function personalRequestsCsv(rows,profile,labels={},filter='all'){
  const permission='reports.export',grants=profile?.grants?.filter(g=>g.permissions.includes(permission))||[];
  if(!profile?.active||profile.denied?.includes(permission)||!grants.length)throw Error('Tài khoản chưa được cấp quyền xuất báo cáo.');
  const match=(scope,r)=>scope==='all'||scope==='unit'&&r.departmentId===profile.departmentId||scope==='units'&&profile.departmentIds?.includes(r.departmentId)||scope==='self'&&r.ownerId===profile.id;
  const selected=rows.filter(r=>r.ownerId===profile.id&&match(profile.scope,r)&&grants.some(g=>match(g.scope,r))&&
    (filter==='all'||(['room','device'].includes(filter)?r.kind===filter:r.status===filter)));
  const quote=value=>{let text=String(value??'');if(/^[\s\p{Cf}\p{Cc}]*[=+@-]/u.test(text))text="'"+text;return '"'+text.replaceAll('"','""')+'"';};
  return '\uFEFF'+[['Mã phiếu','Nguồn lực','Ngày','Bắt đầu','Kết thúc','Trạng thái'],...selected.map(r=>[r.id,r.name,r.date,r.start,r.end,labels[r.status]?.[0]||r.status])].map(row=>row.map(quote).join(',')).join('\r\n')+'\r\n';
}
export class ServerResources {
  constructor(review=false){this.review=review;this.snapshot=null;this.revision=0;}
  async load(){
    if(this.review)return this.loadPage(this.route||globalThis.location?.hash?.replace(/^#\//,'')||'overview',this.filters||{},this.page||1);
    const roomDirectory=['rooms.book','resources.manage','requests.review'].some(can),assetDirectory=['devices.borrow','resources.manage','requests.review'].some(can);
    const [departments,rooms,assets,regularRooms,regularLoans,logRooms,logLoans]=await Promise.all([api.departments(),roomDirectory?all('/rooms'+(this.review&&can('resources.manage')?'?manage=true':'')):[],assetDirectory?all('/equipment'+(this.review&&can('resources.manage')?'?manage=true':'')):[],can(this.review?'requests.review':'rooms.book')?all(`/room-requests?review=${this.review}`):[],can(this.review?'requests.review':'devices.borrow')||this.review&&can('resources.manage')?all(`/equipment-loans?review=${this.review}`):[],this.review&&can('logs.review')?all('/room-requests?logReview=true'):[],this.review&&can('logs.review')?all('/equipment-loans?logReview=true'):[]]);
    const roomRequests=[...new Map([...logRooms,...regularRooms].map(r=>[r.id,r])).values()],loans=[...new Map([...logLoans,...regularLoans].map(r=>[r.id,r])).values()];
    this.departments=departments;const department=id=>departments.find(d=>d.id===id)?.name||id;
    const roomRows=rooms.map(r=>({...r,api:r,building:department(r.departmentId),seats:r.capacity,available:r.active,tags:[],visual:r.type==='Phòng máy'?'lab':''}));
    const devices=assets.map(a=>({...a,api:a,category:a.category,available:a.total-a.damaged-a.lost,unit:a.unit,tags:[],shape:'equipment'}));
    const requests=[...roomRequests.map(r=>({...r,api:r,kind:'room',resource:r.roomId,name:rooms.find(x=>x.id===r.roomId)?.name||r.roomId,date:day(r.start),start:time(r.start),end:time(r.end),students:r.attendees,unit:department(r.departmentId),reason:r.reviewNote})),...loans.map(r=>({...r,api:r,kind:'device',resource:r.assetGroupId,name:r.assetName,date:day(r.start),start:time(r.start),end:time(r.end),unit:department(r.departmentId),reason:r.note,handover:r.status==='ready'?{quantity:r.quantity}:undefined,returnRequest:r.returns.find(t=>!t.acceptedAt)?this.returnRequest(r.returns.find(t=>!t.acceptedAt)):null,returnHistory:r.returns.filter(t=>t.acceptedAt).map(t=>({id:t.id,good:t.acceptedGood,damaged:t.acceptedDamaged,lost:t.acceptedLost,quantity:t.acceptedGood+t.acceptedDamaged+t.acceptedLost,notes:t.acceptanceNote,at:t.acceptedAt}))}))];
    const logs=[];
    for(const r of requests)r.ownerName=`${r.api.ownerId} · ${r.unit}`;
    if(can(this.review?'logs.review':'logs.write'))logs.push(...(await mapBounded(requests.filter(r=>(r.kind==='room'?r.status==='approved':['borrowed','return_pending','completed'].includes(r.status))&&resourceAllows(identity,this.review?'logs.review':'logs.write',r.api.departmentId,r.api.ownerId)),async r=>{
      try { const l=await api.request(`/${r.kind==='room'?'room-requests':'equipment-loans'}/${r.id}/log?review=${this.review}`);return {...l,api:l,request:r.id,date:l.actualStart?day(l.actualStart):r.date,start:l.actualStart?time(l.actualStart):'',end:l.actualEnd?time(l.actualEnd):'',students:l.attendees,notes:l.issue,reviewNotes:l.reviewNote,status:l.status==='submitted'?'pending':l.status==='needs_changes'?'needs_update':l.status}; }
      catch(e){if(e.status!==404)throw e;return null;}
    })).filter(Boolean));
    const inventories=[],maintenance=[];
    if(this.review&&can('inventory.manage'))inventories.push(...await mapBounded(await all('/inventories'),async s=>{const v=await api.request(`/inventories/${s.id}`);return {...v,api:v,unit:department(v.departmentId),date:day(v.at),rows:v.items.map(i=>({...i,id:i.assetGroupId,notes:i.note,condition:({'good':'Tốt','damaged':'Hỏng / cần xử lý','missing':'Mất / thiếu'})[i.condition]||''}))};}));
    if(this.review&&can('resources.manage'))for(const m of await all('/maintenance'))maintenance.push({...m,api:m,asset:m.assetGroupId,device:m.assetGroupId,room:'',name:assets.find(a=>a.id===m.assetGroupId)?.name||m.assetGroupId,content:m.content,resultNotes:m.note});
    this.snapshot={version:1,resourceRevision:++this.revision,catalog:{rooms:roomRows,devices},requests,logs,roomReservations:[],assets:assets.map(a=>({...a,api:a,device:a.id,room:'',unit:department(a.departmentId),location:department(a.departmentId),quantity:a.total,unusable:a.damaged,lost:a.lost,price:a.acquisitionValue??null,purchased:a.acquisitionDate||'',version:a.version})),inventories,maintenance,resourceAudit:[]};
    this.snapshot.resourceAudit=[...inventories.map(i=>({action:'inventory-create',target:i.name,at:i.at})),...requests.flatMap(r=>(r.api.returns||[]).filter(t=>t.acceptedAt).map(t=>({action:'return',target:r.name,at:t.acceptedAt})))].sort((a,b)=>new Date(a.at)-new Date(b.at));
    return this.snapshot;
  }
  roomRow(r){return{...r,api:r,building:this.departments.find(d=>d.id===r.departmentId)?.name||r.departmentId,seats:r.capacity,available:r.active,tags:[],visual:r.type==='Phòng máy'?'lab':''};}
  assetRow(a){const unit=this.departments.find(d=>d.id===a.departmentId)?.name||a.departmentId;return{...a,api:a,device:a.id,room:'',unit,location:unit,quantity:a.total,unusable:a.damaged,lost:a.lost,price:a.acquisitionValue??null,purchased:a.acquisitionDate||'',version:a.version};}
  deviceCatalogRow(a){return{...a,api:a,category:a.category,available:a.total-a.damaged-a.lost,unit:a.unit,tags:[],shape:'equipment'};}
  requestRow(r,kind,name,manageDepartmentId=null,logStatus=null,ownerName=null){const unit=this.departments.find(d=>d.id===r.departmentId)?.name||r.departmentId;return{...r,api:r,kind,manageDepartmentId,logStatus,resource:kind==='room'?r.roomId:r.assetGroupId,name:name||r.assetName||r.roomId,date:day(r.start),start:time(r.start),end:time(r.end),students:r.attendees,unit,ownerName:`${ownerName||r.ownerId} · ${unit}`,reason:kind==='room'?r.reviewNote:r.note,handover:r.status==='ready'?{quantity:r.quantity}:undefined,returnRequest:r.returns?.find(t=>!t.acceptedAt)?this.returnRequest(r.returns.find(t=>!t.acceptedAt)):null,returnHistory:(r.returns||[]).filter(t=>t.acceptedAt).map(t=>({id:t.id,good:t.acceptedGood,damaged:t.acceptedDamaged,lost:t.acceptedLost,quantity:t.acceptedGood+t.acceptedDamaged+t.acceptedLost,notes:t.acceptanceNote,at:t.acceptedAt}))};}
  logRow(l,r){return{...l,api:l,request:r.id,date:l.actualStart?day(l.actualStart):r.date,start:l.actualStart?time(l.actualStart):'',end:l.actualEnd?time(l.actualEnd):'',students:l.attendees,quantity:l.quantity??r.quantity,notes:l.issue,reviewNotes:l.reviewNote,status:l.status==='submitted'?'pending':l.status==='needs_changes'?'needs_update':l.status};}
  inventoryRow(v){return{...v,api:v,unit:this.departments.find(d=>d.id===v.departmentId)?.name||v.departmentId,date:day(v.at),rows:(v.items||[]).map(i=>({...i,id:i.assetGroupId,notes:i.note,condition:({'good':'Tốt','damaged':'Hỏng / cần xử lý','missing':'Mất / thiếu'})[i.condition]||''}))};}
  maintenanceRow(m,name){return{...m,api:m,asset:m.assetGroupId,device:m.assetGroupId,room:'',name:name||m.assetGroupId,content:m.content,resultNotes:m.note};}
  async loadPage(route,filters={},page=1){
    const revision=(this.pageRevision||0)+1;this.pageRevision=revision;
    const departments=this.departments||await api.departments();
    this.departments=departments;
    const empty={version:1,resourceRevision:++this.revision,catalog:{rooms:[],devices:[]},requests:[],logs:[],roomReservations:[],assets:[],inventories:[],maintenance:[],resourceAudit:[]};
    const key=({rooms:'rooms',calendar:'rooms',assets:'assets',approvals:'requests',loans:'requests',inventory:'inventories',maintenance:'maintenance',logs:'logs'})[route];
    const q=new URLSearchParams({page:String(page),pageSize:'20'});
    for(const k of ['search','status','from','to'])if(filters[k])q.set(k,filters[k]);
    if(route==='approvals'&&filters.kind)q.set('kind',filters.kind);if(route==='loans')q.set('kind','device');
    if(route==='rooms'&&filters.building){const dep=departments.find(d=>d.name===filters.building||d.id===filters.building);if(dep)q.set('departmentId',dep.id);}
    if(route==='assets'&&filters.unit){const dep=departments.find(d=>d.name===filters.unit||d.id===filters.unit);if(dep)q.set('departmentId',dep.id);}
    if(route==='calendar'&&filters.room)q.set('roomId',filters.room);
    const result=key?await api.request('/resource-workspace/'+key+'?'+q):route==='overview'?await api.request('/resource-workspace/summary'):null;
    if(revision!==this.pageRevision)return this.snapshot;
    this.route=route;this.filters={...filters};this.page=page;this.pageInfo=key?{page,total:result.total,pageSize:20}:null;
    if(key==='rooms')empty.catalog.rooms=result.items.map(r=>this.roomRow(r));
    if(key==='assets'){empty.assets=result.items.map(a=>this.assetRow(a));empty.catalog.devices=result.items.map(a=>({...a,api:a,available:a.total-a.damaged-a.lost,tags:[]}));}
    if(key==='requests')empty.requests=result.items.map(x=>this.requestRow(x.request,x.kind,x.name,x.manageDepartmentId,null,x.ownerName));
    if(key==='logs'){empty.requests=result.items.map(x=>this.requestRow(x.request,x.kind,x.name,x.manageDepartmentId,null,x.ownerName));empty.logs=result.items.map(x=>{const l=x.log,r=empty.requests.find(r=>r.id===x.request.id);return{...l,api:l,request:r.id,date:l.actualStart?day(l.actualStart):r.date,start:l.actualStart?time(l.actualStart):'',end:l.actualEnd?time(l.actualEnd):'',students:l.attendees,notes:l.issue,reviewNotes:l.reviewNote,status:l.status==='submitted'?'pending':l.status==='needs_changes'?'needs_update':l.status};});}
    if(key==='inventories')empty.inventories=result.items.map(v=>this.inventoryRow(v));
    if(key==='maintenance')empty.maintenance=result.items.map(x=>this.maintenanceRow(x.maintenance,x.name));
    if(route==='overview')empty.summary=result;
    this.snapshot=empty;return empty;
  }
  async inventoryDetail(id){const row=this.inventoryRow(await api.request('/inventories/'+encodeURIComponent(id)));const index=this.snapshot.inventories.findIndex(x=>x.id===id);if(index>=0)this.snapshot.inventories[index]=row;else this.snapshot.inventories.push(row);return row;}
  async pickAssets({page=1,search=''}={}){const result=await api.request('/resource-workspace/assets?'+new URLSearchParams({page,pageSize:20,search,status:'active'}));return{...result,items:result.items.map(a=>this.assetRow(a))};}
  returnRequest(t){return {...t,quantity:t.good+t.damaged+t.lost,notes:t.note,date:day(t.at)};}
  matchingDepartmentIds(search){
    const q=String(search||'').trim().toLocaleLowerCase('vi');if(!q||!this.departments?.length)return'';
    return this.departments.filter(d=>d.name?.toLocaleLowerCase('vi').includes(q)||d.id?.toLocaleLowerCase('vi').includes(q)).map(d=>d.id).join(',');
  }
  async loadLecturerPage(route,filters={},page=1){
    const revision=(this.pageRevision||0)+1;this.pageRevision=revision;
    this.departments=this.departments||await api.departments();
    const prevCatalog=this.snapshot?.catalog||{rooms:[],devices:[]};
    const empty={version:1,resourceRevision:++this.revision,catalog:{rooms:route==='rooms'?[]:prevCatalog.rooms,devices:route==='devices'?[]:prevCatalog.devices},requests:[],logs:[],roomReservations:this.snapshot?.roomReservations||[],assets:[],inventories:[],maintenance:[],resourceAudit:[],navPendingCount:this.snapshot?.navPendingCount??0};
    const pageSize=20;let info=null;
    if(route==='overview'){
      if(['rooms.book','devices.borrow','logs.write'].some(can)){
        const q=new URLSearchParams();if(filters.date)q.set('date',filters.date);
        const r=await api.request('/lecturer-workspace/summary'+(q.size?'?'+q:''));
        if(revision!==this.pageRevision)return this.snapshot;
        empty.lecturerSummary=r;empty.navPendingCount=r.pendingCount??0;empty.calendarWeekStart=r.weekStart;
        empty.agendaRequests=(r.agenda||[]).map(x=>this.requestRow(x.request,x.kind,x.name,x.manageDepartmentId,x.logStatus));
        empty.recentRequests=(r.recent||[]).map(x=>this.requestRow(x.request,x.kind,x.name,x.manageDepartmentId,x.logStatus));
        empty.requests=[...new Map([...empty.recentRequests,...empty.agendaRequests].map(x=>[x.id,x])).values()];
      }
    } else if(route==='calendar'){
      if(['rooms.book','devices.borrow'].some(can)){
        const q=new URLSearchParams();if(filters.date)q.set('date',filters.date);
        const r=await api.request('/lecturer-workspace/calendar'+(q.size?'?'+q:''));
        if(revision!==this.pageRevision)return this.snapshot;
        empty.calendarWeekStart=r.weekStart;empty.navPendingCount=r.pendingCount??0;
        empty.agendaRequests=(r.items||[]).map(x=>this.requestRow(x.request,x.kind,x.name,x.manageDepartmentId,x.logStatus));
        empty.requests=empty.agendaRequests;
      }
    } else if(route==='rooms'){
      if(['rooms.book','resources.manage','requests.review'].some(can)){
        const q=new URLSearchParams({page:String(page),pageSize:String(pageSize)});
        if(filters.search){q.set('search',filters.search);const deps=this.matchingDepartmentIds(filters.search);if(deps)q.set('departmentIds',deps);}
        if(filters.type)q.set('type',filters.type);if(filters.capacity>0)q.set('capacity',String(filters.capacity));
        if(filters.date)q.set('date',filters.date);if(filters.start&&filters.end){q.set('start',filters.start);q.set('end',filters.end);}
        const r=await api.request('/lecturer-workspace/rooms?'+q);
        if(revision!==this.pageRevision)return this.snapshot;
        empty.catalog.rooms=(r.items||[]).map(x=>this.roomRow(x));empty.roomTypes=r.types||[];empty.navPendingCount=r.pendingCount??0;
        info={page:r.page,total:r.total,pageSize:r.pageSize};
      }
    } else if(route==='devices'){
      if(['devices.borrow','resources.manage','requests.review'].some(can)){
        const q=new URLSearchParams({page:String(page),pageSize:String(pageSize)});
        if(filters.search){q.set('search',filters.search);const deps=this.matchingDepartmentIds(filters.search);if(deps)q.set('departmentIds',deps);}
        if(filters.type)q.set('category',filters.type);
        const r=await api.request('/lecturer-workspace/devices?'+q);
        if(revision!==this.pageRevision)return this.snapshot;
        empty.catalog.devices=(r.items||[]).map(a=>this.deviceCatalogRow(a));empty.assets=(r.items||[]).map(a=>this.assetRow(a));
        empty.deviceCategories=r.categories||[];empty.navPendingCount=r.pendingCount??0;
        info={page:r.page,total:r.total,pageSize:r.pageSize};
      }
    } else if(route==='requests'){
      if(['rooms.book','devices.borrow'].some(can)){
        const q=new URLSearchParams({page:String(page),pageSize:String(pageSize),tab:filters.tab||'all'});
        if(filters.search)q.set('search',filters.search);if(filters.status)q.set('status',filters.status);
        const r=await api.request('/lecturer-workspace/requests?'+q);
        if(revision!==this.pageRevision)return this.snapshot;
        empty.requests=(r.items||[]).map(x=>this.requestRow(x.request,x.kind,x.name,x.manageDepartmentId,x.logStatus));
        empty.requestCounts=r.counts||{all:r.total,room:0,device:0,pending:0,draft:0};empty.navPendingCount=r.pendingCount??0;
        info={page:r.page,total:r.total,pageSize:r.pageSize};
      }
    } else if(route==='returns'){
      if(can('devices.borrow')){
        const q=new URLSearchParams({page:String(page),readyPage:String(filters.readyPage||1),pageSize:String(pageSize)});if(filters.search)q.set('search',filters.search);
        const r=await api.request('/lecturer-workspace/returns?'+q);
        if(revision!==this.pageRevision)return this.snapshot;
        empty.activeReturnRequests=(r.items||[]).map(x=>this.requestRow(x.request,x.kind,x.name,x.manageDepartmentId,x.logStatus));
        empty.readyRequests=(r.readyItems||[]).map(x=>this.requestRow(x.request,x.kind,x.name,x.manageDepartmentId,x.logStatus));
        empty.requests=[...new Map([...empty.activeReturnRequests,...empty.readyRequests].map(x=>[x.id,x])).values()];
        empty.navPendingCount=r.pendingCount??0;info={page:r.page,total:r.total,pageSize:r.pageSize,readyTotal:r.readyTotal??empty.readyRequests.length,readyPage:r.readyPage||1};
      }
    } else if(route==='logs'){
      if(can('logs.write')){
        const q=new URLSearchParams({page:String(page),eligiblePage:String(filters.eligiblePage||1),pageSize:String(pageSize)});if(filters.search)q.set('search',filters.search);
        const r=await api.request('/lecturer-workspace/logs?'+q);
        if(revision!==this.pageRevision)return this.snapshot;
        empty.eligibleLogRequests=(r.eligibleItems||[]).map(x=>this.requestRow(x.request,x.kind,x.name,x.manageDepartmentId,'none'));
        const submittedReqs=(r.items||[]).map(x=>this.requestRow(x.request,x.kind,x.name,x.manageDepartmentId,x.log?.status));
        empty.requests=[...new Map([...empty.eligibleLogRequests,...submittedReqs].map(x=>[x.id,x])).values()];
        empty.logs=(r.items||[]).map(x=>{const req=empty.requests.find(y=>y.id===x.request.id);return this.logRow(x.log,req);});
        empty.navPendingCount=r.pendingCount??0;info={page:r.page,total:r.total,pageSize:r.pageSize,eligiblePage:r.eligiblePage||1,eligibleTotal:r.eligibleTotal||0};
      } else info={page:1,total:0,pageSize,eligiblePage:1,eligibleTotal:0};
    }
    if(revision!==this.pageRevision)return this.snapshot;
    this.route=route;this.filters={...filters};this.page=page;this.pageInfo=info;this.snapshot=empty;
    if(route==='rooms'&&empty.catalog.rooms.length)await this.availability(filters.date);
    return empty;
  }
  async loadRequestById(id){
    this.departments=this.departments||await api.departments();
    const r=await api.request('/lecturer-workspace/requests?'+new URLSearchParams({page:'1',pageSize:'1',id}));
    const item=r.items?.[0]?this.requestRow(r.items[0].request,r.items[0].kind,r.items[0].name,r.items[0].manageDepartmentId,r.items[0].logStatus):null;
    if(this.snapshot){const idx=this.snapshot.requests.findIndex(x=>x.id===id);if(item){if(idx>=0)this.snapshot.requests[idx]=item;else this.snapshot.requests.push(item);}else if(idx>=0)this.snapshot.requests.splice(idx,1);}
    return item;
  }
  async ensureResource(kind,id){
    this.departments=this.departments||await api.departments();
    const room=kind==='room';const list=room?this.snapshot.catalog.rooms:this.snapshot.catalog.devices;
    const r=await api.request(`/lecturer-workspace/${room?'rooms':'devices'}?`+new URLSearchParams({page:'1',pageSize:'1',[room?'roomId':'assetGroupId']:id}));
    const raw=r.items?.[0];if(!raw)throw Error('Nguồn lực đã ngừng hoạt động hoặc không còn tồn tại. Vui lòng tải lại.');
    const mapped=room?this.roomRow(raw):this.deviceCatalogRow(raw);
    const idx=list.findIndex(x=>x.id===id);if(idx>=0)list[idx]=mapped;else list.push(mapped);
    return mapped;
  }
  async ensureLog(request){
    if(!request||!can('logs.write')||!resourceAllows(identity,'logs.write',request.api?.departmentId??request.departmentId,request.api?.ownerId??request.ownerId))return null;
    try{
      const l=await api.request(`/${request.kind==='room'?'room-requests':'equipment-loans'}/${encodeURIComponent(request.id)}/log?review=false`);
      const mapped=this.logRow(l,request);const idx=this.snapshot.logs.findIndex(x=>x.request===request.id);
      if(idx>=0)this.snapshot.logs[idx]=mapped;else this.snapshot.logs.push(mapped);
      return mapped;
    }catch(e){if(e.status!==404)throw e;return null;}
  }
  async loadExportRequests(filters={}){
    this.departments=this.departments||await api.departments();const rows=[];
    for(let page=1;page<=1000;page++){
      const q=new URLSearchParams({page:String(page),pageSize:'100',tab:filters.tab||'all'});
      if(filters.search)q.set('search',filters.search);if(filters.status)q.set('status',filters.status);
      const r=await api.request('/lecturer-workspace/requests?'+q);
      if(!Array.isArray(r.items)||!Number.isInteger(r.total)||r.total<0)throw Error('Máy chủ trả về trang dữ liệu xuất không hợp lệ.');
      rows.push(...r.items.map(x=>this.requestRow(x.request,x.kind,x.name,x.manageDepartmentId,x.logStatus)));
      if(rows.length>=r.total)return rows;
      if(!r.items.length)throw Error('Danh sách đã thay đổi trong khi xuất. Vui lòng tải lại và thử lại.');
    }
    throw Error('Danh sách xuất vượt phạm vi phân trang. Hãy thu hẹp bộ lọc.');
  }
  async saveEquipmentDraft(id,data){return api.request(`/equipment-loans/${encodeURIComponent(id)}`,{method:'PUT',data});}
  async syncStaleDraft(id){const latest=await this.loadRequestById(id);return{status:latest?.status==='draft'?'draft':'changed',request:latest};}
  async availability(date,roomIds=null){
    const snapshot=this.snapshot;
    const targets=roomIds?roomIds.map(id=>({id})):snapshot.catalog.rooms;
    if(!targets?.length)return;
    const targetSet=roomIds?new Set(roomIds):null;
    const busy=(await mapBounded(targets,async room=>{
      const rows=[];
      const result=await api.roomAvailability(room.id,date);let cursor='07:00';
      for(const slot of result.slots){const a=time(slot.start),b=time(slot.end);if(cursor<a)rows.push({id:crypto.randomUUID(),kind:'room',resource:room.id,date,start:cursor,end:a,status:'approved'});cursor=b;}
      if(cursor<'18:00')rows.push({id:crypto.randomUUID(),kind:'room',resource:room.id,date,start:cursor,end:'18:00',status:'approved'});
      return rows;
    })).flat();
    if(this.snapshot!==snapshot)return;
    snapshot.roomReservations=snapshot.roomReservations.filter(x=>!(x.date===date&&(!targetSet||targetSet.has(x.resource)))).concat(busy);
  }
  async book(form,intent){
    const f=Object.fromEntries(new FormData(form));if(intent==='pending'&&!f.commitment)throw Error('Vui lòng xác nhận cam kết sử dụng.');
    if(!f.date||!f.start||!f.end||f.end<=f.start||!Number.isInteger(Number(f.count))||Number(f.count)<1)throw Error('Ngày, giờ hoặc số lượng không hợp lệ.');
    const room=form.dataset.kind==='room';const current=form.dataset.existing?this.snapshot?.requests?.find(r=>r.id===form.dataset.existing):null;
    const resourceId=form.dataset.resource||(room?current?.api?.roomId:current?.api?.assetGroupId)||current?.resource;
    if(!resourceId)throw Error('Không xác định được nguồn lực đăng ký.');
    const data={start:instant(f.date,f.start),end:instant(f.date,f.end),purpose:f.purpose??'',...(room?{roomId:resourceId,attendees:Number(f.count)}:{assetGroupId:resourceId,quantity:Number(f.count)})};
    const payload=JSON.stringify(data);
    let r=form._serverDraft;
    if(r&&form._serverPayload!==payload)r=room?await api.saveRoomDraft(r.id,{...data,version:r.version}):await this.saveEquipmentDraft(r.id,{...data,version:r.version});
    else if(!r&&form.dataset.existing){if(!current?.api?.version)throw Error('Không tìm thấy phiên bản bản nháp. Vui lòng tải lại phiếu.');r=room?await api.saveRoomDraft(current.id,{...data,version:current.api.version}):await this.saveEquipmentDraft(current.id,{...data,version:current.api.version});}
    else if(!r)r=await keyed(`/${room?'room-requests':'equipment-loans'}`,data);
    form._serverDraft=r;form._serverPayload=payload;
    form.dataset.existing=r.id;form.dataset.resource=resourceId;
    if(current)current.api={...current.api,...r,version:r.version};
    if(intent==='pending')await keyed(`/${room?'room-requests':'equipment-loans'}/${r.id}/submit`,{version:r.version,note:null});
    return r;
  }
  async action(id,operation,quantity=null){const r=this.snapshot.requests.find(x=>x.id===id);return keyed(`/${r.kind==='room'?'room-requests':'equipment-loans'}/${id}/${operation}`,{version:r.api.version,note:null,quantity});}
  async return(form){const r=this.snapshot.requests.find(x=>x.id===form.dataset.id),f=Object.fromEntries(new FormData(form));return keyed(`/equipment-loans/${r.id}/returns`,{version:r.api.version,good:Number(f.good),damaged:Number(f.damaged),lost:Number(f.lost),note:f.notes});}
  async log(form){const r=this.snapshot.requests.find(x=>x.id===form.dataset.id),prior=this.snapshot.logs.find(l=>l.request===r.id),f=Object.fromEntries(new FormData(form));const data=logPayload(r,f,prior?.api.version??null);return r.kind==='room'?api.saveRoomLog(r.id,data):keyed(`/equipment-loans/${r.id}/log`,data);}
  department(name){const d=this.departments.find(d=>d.name===name||d.id===name);if(!d)throw Error('Chọn một đơn vị cụ thể.');return d.id;}
  async manage(operation,p){
    const r=this.snapshot.requests.find(x=>x.id===p.id),a=this.snapshot.assets.find(x=>x.id===p.id),m=this.snapshot.maintenance.find(x=>x.id===p.id),inv=this.snapshot.inventories.find(x=>x.id===p.id);
    if(operation==='room-delete'){
      const row=this.snapshot.catalog.rooms.find(x=>x.id===p.id)?.api;
      if(!row)throw Error('Không tìm thấy phòng.');
      return api.request(`/rooms/${row.id}`,{method:'PUT',data:{code:row.code,name:row.name,type:row.type,departmentId:row.departmentId,capacity:row.capacity,active:false,version:row.version}});
    }
    if(operation==='asset-delete'){
      if(!a)throw Error('Không tìm thấy tài sản.');
      return api.request(`/equipment/${a.id}`,{method:'PUT',data:assetPayload({...a.api,active:false},a.api)});
    }
    if(operation==='review')return keyed(`/${r.kind==='room'?'room-requests':'equipment-loans'}/${r.id}/${p.status==='approved'?'approve':'reject'}`,{version:r.api.version,note:p.notes});
    if(operation==='handover')return keyed(`/equipment-loans/${r.id}/handover`,{version:r.api.version,note:p.notes,quantity:Number(p.quantity)});
    if(operation==='receipt-resolve')return keyed(`/equipment-loans/${r.id}/resolve-receipt`,{version:r.api.version,note:p.notes,quantity:null});
    if(operation==='return'){const t=r.api.returns.find(t=>!t.acceptedAt);return keyed(`/equipment-loans/${r.id}/accept-return`,{version:r.api.version,returnId:t.id,good:Number(p.good),damaged:Number(p.damaged),lost:Number(p.lost),note:p.notes});}
    if(operation==='log'){const l=this.snapshot.logs.find(l=>l.id===p.id),request=this.snapshot.requests.find(r=>r.id===l.request);const data={version:l.api.version,result:p.status==='needs_update'?'needs_changes':'confirmed',note:p.notes};return request.kind==='room'?api.reviewRoomLog(request.id,data):keyed(`/equipment-loans/${request.id}/log/review`,data);}
    if(operation==='inventory-create')return keyed('/inventories',{name:p.name,departmentId:this.department(p.unit)});
    if(operation==='inventory-save')return keyed(`/inventories/${inv.id}/count`,{version:inv.api.version,complete:p.finish,items:p.rows.map(r=>({assetGroupId:r.id,actual:r.actual===''?null:Number(r.actual),condition:({'Tốt':'good','Hỏng / cần xử lý':'damaged','Mất / thiếu':'missing'})[r.condition]||'',note:r.notes}))});
    if(operation==='maintenance-plan')return keyed('/maintenance',{assetGroupId:p.asset,mode:p.mode,quantity:Number(p.quantity),start:localInstant(p.start),end:localInstant(p.end),content:p.content});
    if(operation.startsWith('maintenance-')&&operation!=='maintenance-cost')return keyed(`/maintenance/${m.id}/${operation.slice(12)}`,{version:m.api.version,result:p.result||'',cost:Number(p.cost||0),note:p.notes});
    if(operation==='room'){const old=this.snapshot.catalog.rooms.find(r=>r.id===p.id);const data={code:old?.api.code||p.id,name:p.name,type:p.type,departmentId:p.departmentId?this.department(p.departmentId):old?.api.departmentId,capacity:Number(p.seats),active:p.available===true||p.available==='true',version:old?.api.version};return api.request(old?`/rooms/${old.id}`:'/rooms',{method:old?'PUT':'POST',data});}
    throw Error('Chức năng này chưa có API. Không ghi dữ liệu mô phỏng vào hệ thống thật.');
  }
}
