const pad=n=>String(n).padStart(2,'0');
const iso=d=>`${d.getFullYear()}-${pad(d.getMonth()+1)}-${pad(d.getDate())}`;
const today=new Date();
export const TODAY=iso(today);
const offsetDate=n=>{const d=new Date(today);d.setDate(d.getDate()+n);return iso(d);};
export const DEFAULT_ROOMS = [
 {id:'A201',name:'Phòng học A.201',building:'Nhà A',seats:60,type:'Phòng học',tags:['Máy chiếu','Điều hòa','Bảng trắng'],visual:'',available:true},
 {id:'A302',name:'Phòng học A.302',building:'Nhà A',seats:60,type:'Phòng học',tags:['Máy chiếu','Điều hòa','Bảng trắng'],visual:'',available:true},
 {id:'B201',name:'Phòng máy B.201',building:'Nhà B',seats:40,type:'Phòng máy',tags:['40 máy tính','Máy chiếu','Điều hòa'],visual:'lab',available:true},
 {id:'A105',name:'Phòng học A.105',building:'Nhà A',seats:90,type:'Phòng học',tags:['Máy chiếu','Micro','Điều hòa'],visual:'',available:true},
 {id:'C101',name:'Hội trường C.101',building:'Nhà C',seats:200,type:'Hội trường',tags:['Âm thanh','Màn LED','Điều hòa'],visual:'hall',available:true},
 {id:'B305',name:'Phòng thí nghiệm B.305',building:'Nhà B',seats:30,type:'Phòng thí nghiệm',tags:['Thiết bị thực hành','Máy chiếu'],visual:'lab',available:true},
 {id:'A401',name:'Phòng học A.401',building:'Nhà A',seats:60,type:'Phòng học',tags:['Máy chiếu','Điều hòa'],visual:'hall',available:false}
];
export const DEFAULT_DEVICES = [
 {id:'MC01',name:'Máy chiếu Epson EB-X51',category:'Máy chiếu',available:5,unit:'bộ',tags:['3.800 lumens','HDMI','Kèm dây nguồn'],shape:'equipment'},
 {id:'LT01',name:'Laptop Dell Latitude 5420',category:'Máy tính',available:8,unit:'chiếc',tags:['Core i5','RAM 16 GB','Windows 11'],shape:'equipment'},
 {id:'MIC01',name:'Micro không dây Shure',category:'Âm thanh',available:4,unit:'bộ',tags:['2 micro','Kèm bộ thu','Pin sạc'],shape:'equipment'},
 {id:'CAM01',name:'Máy ảnh Canon EOS 80D',category:'Ghi hình',available:2,unit:'bộ',tags:['Ống kính 18–135','Thẻ nhớ','Túi bảo vệ'],shape:'equipment'},
 {id:'SP01',name:'Loa di động JBL EON',category:'Âm thanh',available:3,unit:'bộ',tags:['Bluetooth','Kèm chân loa','Cáp kết nối'],shape:'equipment'},
 {id:'PTR01',name:'Bút trình chiếu Logitech R400',category:'Phụ kiện',available:6,unit:'chiếc',tags:['USB','Laser đỏ','Phạm vi 15 m'],shape:'equipment'}
];
export const createLecturerSeed = () => ({version:1,requests:[
 {id:'PH-2026-001',kind:'room',resource:'B201',name:'Phòng máy B.201',date:TODAY,start:'07:00',end:'09:30',purpose:'Thực hành Lập trình Web',students:35,status:'approved',created:TODAY},
 {id:'PH-2026-002',kind:'room',resource:'A302',name:'Phòng học A.302',date:TODAY,start:'13:30',end:'15:30',purpose:'Cơ sở dữ liệu · CNTT K24',students:48,status:'approved',created:TODAY},
 {id:'TB-2026-003',kind:'device',resource:'MC01',name:'Máy chiếu Epson EB-X51',date:offsetDate(1),start:'08:00',end:'11:30',purpose:'Báo cáo chuyên đề khoa CNTT',quantity:1,returned:0,status:'pending',created:TODAY,location:'Nhà A · A.302'},
 {id:'TB-2026-004',kind:'device',resource:'MIC01',name:'Micro không dây Shure',date:offsetDate(-1),start:'08:00',end:'17:00',purpose:'Hội thảo khoa học sinh viên',quantity:2,returned:0,status:'borrowed',created:offsetDate(-2),location:'Hội trường C.101'},
 {id:'PH-2026-005',kind:'room',resource:'A105',name:'Phòng học A.105',date:offsetDate(-1),start:'09:00',end:'11:00',purpose:'Phân tích thiết kế hệ thống',students:52,status:'completed',created:offsetDate(-3)},
 {id:'TB-2026-006',kind:'device',resource:'LT01',name:'Laptop Dell Latitude 5420',date:offsetDate(2),start:'07:30',end:'11:00',purpose:'Giảng dạy môn Công nghệ phần mềm',quantity:1,returned:0,status:'ready',created:TODAY,location:'Phòng B.201'}
],logs:[{id:'NK-001',request:'PH-2026-005',date:offsetDate(-1),start:'09:00',end:'10:50',students:50,condition:'Tốt',notes:'Phòng và thiết bị hoạt động bình thường.',status:'pending'}]});
