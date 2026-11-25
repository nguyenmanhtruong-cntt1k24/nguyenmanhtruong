import test from 'node:test';
import assert from 'node:assert/strict';
globalThis.location??={search:''};
const {formatDetailCell}=await import('./report-details.mjs');
test('Nguyên giá chưa kê khai khác số không và không biến thành NaN',()=>{
 const column={numeric:true,label:'Nguyên giá nhóm kê khai lịch sử (VND)'};
 assert.equal(formatDetailCell('',column),'Chưa kê khai');assert.equal(formatDetailCell(null,column),'Chưa kê khai');
 assert.equal(formatDetailCell('0',column),'0');assert.equal(formatDetailCell('1000000',column),'1.000.000');
});
test('Loại, khoản mục và trạng thái hiển thị tiếng Việt; văn bản khác được giữ',()=>{
 assert.equal(formatDetailCell('expense',{label:'Loại'}),'Chi');assert.equal(formatDetailCell('receipt',{label:'Loại'}),'Thu');
 assert.equal(formatDetailCell('maintenance',{label:'Khoản mục'}),'Bảo trì');assert.equal(formatDetailCell('borrowed',{label:'Trạng thái'}),'Đang mượn');
 assert.equal(formatDetailCell('PC-001',{label:'Số chứng từ'}),'PC-001');
});
