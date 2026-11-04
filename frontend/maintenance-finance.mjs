import {normalizeAccounting} from './accounting-data.mjs';
export function syncMaintenanceCosts(source,resources){
 const db=normalizeAccounting(source);let changed=false;
 for(const m of resources.maintenance||[]){
  if(!(m.status==='completed'||m.costRecordedAt)||!Number.isSafeInteger(m.cost)||m.cost<=0)continue;
  if(db.paymentQueue.some(q=>q.sourceMaintenance===m.id))continue;
  const at=m.costRecordedAt||m.finishedAt||m.end,date=String(at).slice(0,10),asset=resources.assets.find(a=>a.id===m.asset);
  const paid=m.voucher&&db.expenses.find(e=>e.voucher===m.voucher&&e.value===m.cost);
  const row={id:'CP-'+m.id,sourceMaintenance:m.id,unit:asset?.unit||'Phòng Quản trị',year:Number(date.slice(0,4)),value:m.cost,voucher:'CP-'+m.id,date,content:'Chi phí bảo trì: '+m.name,at,status:paid?'paid':'pending',...(paid?{expenseId:paid.id,paidAt:paid.at}: {})};
  db.paymentQueue.push(row);changed=true;
 }
 if(changed){db.revision++;db.accountingAudit.push({action:'queue',source:'maintenance-sync',at:new Date().toISOString()});}
 return {db,changed};
}
