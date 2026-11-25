import {api,can} from './server-session.mjs';
const esc=v=>String(v??'').replace(/[&<>"']/g,c=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[c]));
export async function mountVoucherFiles(container,entryId){
 if(!container)return;
 container.style.cssText+=';margin-top:20px;padding-top:16px;border-top:1px solid #dbe4ef;overflow-wrap:anywhere';
 container.innerHTML='<p>Đang tải danh sách tệp…</p>';
 try{
  const files=await api.request(`/finance/entries/${encodeURIComponent(entryId)}/attachments`);if(!container.isConnected)return;
  container.innerHTML=`<h3>Tệp đính kèm chứng từ</h3><ul>${files.map(f=>`<li><a href="/api/v1/voucher-attachments/${encodeURIComponent(f.id)}" download>${esc(f.fileName)}</a> · ${Math.ceil(f.size/1024)} KiB</li>`).join('')}</ul>${files.length?'':'<p>Chưa có tệp đính kèm.</p>'}${can('transactions.write')?`<form data-attachment-form><label>Tải tệp PDF, PNG, JPEG (tối đa 2 MiB/tệp, 10 tệp/chứng từ)<input name="file" type="file" accept=".pdf,.png,.jpg,.jpeg" required></label><button class="btn primary" type="submit">Đính kèm tệp</button><p role="alert"></p></form>`:''}`;
  const form=container.querySelector('[data-attachment-form]');
  if(form){form.style.marginTop='16px';form.querySelector('input').style.cssText='display:block;margin:12px 0;width:100%;max-width:100%';}
  form?.addEventListener('submit',async event=>{
   event.preventDefault();event.stopPropagation();const button=form.querySelector('button'),error=form.querySelector('[role=alert]');button.disabled=true;error.textContent='';
   try{
    const file=form.elements.file.files[0];if(!file||file.size<1||file.size>2*1024*1024)throw Error('Tệp phải từ 1 byte đến 2 MiB.');
    const bytes=await file.arrayBuffer();const digest=await crypto.subtle.digest('SHA-256',bytes);const signature=file.name+':'+Array.from(new Uint8Array(digest),b=>b.toString(16).padStart(2,'0')).join('');
    if(form.uploadSignature!==signature){form.uploadSignature=signature;form.uploadKey=crypto.randomUUID();}
    const body=new FormData();body.append('file',file);
    const response=await fetch(`/api/v1/finance/entries/${encodeURIComponent(entryId)}/attachments`,{method:'POST',body,credentials:'same-origin',cache:'no-store',signal:AbortSignal.timeout(30000),headers:{'X-CSRF-TOKEN':api.csrf||await api.refreshCsrf(),'Idempotency-Key':form.uploadKey}});
    if(!response.ok){const problem=await api.problem(response);if(response.status===401||problem.code==='CSRF_INVALID')api.csrf=null;throw Error(problem.title||'Không tải được tệp.');}
    await response.json();await mountVoucherFiles(container,entryId);
   }catch(e){error.textContent=e.message;}finally{button.disabled=false;}
  });
 }catch(e){if(container.isConnected){container.textContent='Không tải được tệp: '+e.message;}}
}
