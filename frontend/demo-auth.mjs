export const SESSION_KEY='hpuni-demo-session';
const PASSWORD_KEY='hpuni-demo-passwords-v1';
const records=()=>JSON.parse(localStorage.getItem(PASSWORD_KEY)||'{}');
async function digest(password,salt){
 const key=await crypto.subtle.importKey('raw',new TextEncoder().encode(password),'PBKDF2',false,['deriveBits']);
 const bytes=await crypto.subtle.deriveBits({name:'PBKDF2',salt:new TextEncoder().encode(salt),iterations:100000,hash:'SHA-256'},key,256);
 return Array.from(new Uint8Array(bytes),x=>x.toString(16).padStart(2,'0')).join('');
}
export async function verifyPassword(userId,password){const r=records()[userId];return r?await digest(password,r.salt)===r.hash:password==='Demo@123';}
export async function changePassword(userId,current,next){
 if(!await verifyPassword(userId,current))throw Error('Mật khẩu hiện tại không đúng.');
 if(next.length<8||next.length>128||!/[a-zA-Z]/.test(next)||!/[0-9]/.test(next))throw Error('Mật khẩu mới cần 8–128 ký tự, gồm chữ và số.');
 if(next===current)throw Error('Mật khẩu mới phải khác mật khẩu hiện tại.');
 const db=records(),salt=crypto.randomUUID();db[userId]={salt,hash:await digest(next,salt)};localStorage.setItem(PASSWORD_KEY,JSON.stringify(db));
}
export async function preparePasswordReset(userId,next){
 if(next.length<8||next.length>128||!/[a-zA-Z]/.test(next)||!/[0-9]/.test(next))throw Error('Mật khẩu mới cần 8–128 ký tự, gồm chữ và số.');
 const salt=crypto.randomUUID(),hash=await digest(next,salt);
 return ()=>{const db=records();db[userId]={salt,hash};localStorage.setItem(PASSWORD_KEY,JSON.stringify(db));};
}
