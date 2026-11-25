// Khoảng nửa mở [bắt đầu, kết thúc): hai lượt chạm mốc giờ không trùng nhau.
export const ROOM_OPEN = '07:00';
export const ROOM_CLOSE = '18:00';
export const holdsRoom = status => ['pending', 'approved', 'completed'].includes(status);
export function minutes(time) {
  if (!/^\d{2}:\d{2}$/.test(time)) return NaN;
  const [h,m] = time.split(':').map(Number);
  return h < 24 && m < 60 ? h * 60 + m : NaN;
}
export function timeAt(value) {
  return `${String(Math.floor(value / 60)).padStart(2,'0')}:${String(value % 60).padStart(2,'0')}`;
}
export const overlaps = (start, end, otherStart, otherEnd) =>
  minutes(start) < minutes(otherEnd) && minutes(end) > minutes(otherStart);
export function freeIntervals(reservations, open = ROOM_OPEN, close = ROOM_CLOSE) {
  const lo=minutes(open), hi=minutes(close);
  if (!(lo < hi)) return [];
  const busy=reservations.filter(r=>holdsRoom(r.status))
    .map(r=>[Math.max(lo,minutes(r.start)),Math.min(hi,minutes(r.end))])
    .filter(([a,b])=>a<b).sort((a,b)=>a[0]-b[0]);
  const result=[];
  let cursor=lo;
  for(const [a,b] of busy){
    if(a>cursor) result.push({start:timeAt(cursor),end:timeAt(a)});
    cursor=Math.max(cursor,b);
  }
  if(cursor<hi) result.push({start:timeAt(cursor),end:timeAt(hi)});
  return result;
}
export const fitsInterval = (intervals,start,end) =>
  minutes(start)<minutes(end) && intervals.some(i=>minutes(start)>=minutes(i.start)&&minutes(end)<=minutes(i.end));

// Chỉ gợi ý giờ tương lai theo múi giờ Việt Nam; chừa ít nhất 5 phút nhập phiếu.
export function futureIntervals(intervals,date,now=new Date()) {
  const vn=new Date(now.getTime()+7*3600000),today=vn.toISOString().slice(0,10);
  if(date<today)return [];
  if(date>today)return intervals;
  const earliest=Math.ceil((vn.getUTCHours()*60+vn.getUTCMinutes()+5)/5)*5;
  return intervals.map(i=>({start:timeAt(Math.max(minutes(i.start),earliest)),end:i.end}))
    .filter(i=>minutes(i.start)<minutes(i.end));
}
