export function khtcHubHTML(requests, scope) {
 const pending=requests.filter(r=>scope(r)&&(r.status==='reviewing'||(r.status==='pending'&&!r.appraisal))).length;
 const cards=[
 ['estimates','Lập và tổng hợp dự toán','Kế toán lập, sửa hồ sơ nháp hoặc trả bổ sung và gửi KHTC thẩm định.'],
 ['appraisals','Thẩm định và trình duyệt',`Kiểm tra hồ sơ, căn cứ, định mức và nguồn kinh phí; ghi ý kiến hoặc trả bổ sung. Có ${pending} hồ sơ chờ trong phạm vi đang chọn.`],
 ['transactions','Quản lý thu – chi','Ghi nhận thu, chi và chứng từ; kiểm tra ngân sách trước khi thực hiện khoản chi.'],
 ['queue','Yêu cầu chi chờ xử lý','Theo dõi yêu cầu chưa đủ điều kiện thực hiện, kiểm tra trước khi ghi nhận chi.'],
 ['budgets','Theo dõi ngân sách','Theo dõi kinh phí được duyệt, đã chi và còn khả dụng theo đơn vị, năm tài chính.'],
 ['reports','Báo cáo tài chính','Tổng hợp ngân sách, xem sổ thu – chi, xuất CSV hoặc in báo cáo theo bộ lọc.']
 ];
 return `<section class="hero"><div class="hero-copy"><div class="hero-tag">PHÒNG KẾ HOẠCH – TÀI CHÍNH</div><h2>Điều phối kế hoạch, kiểm soát kinh phí.</h2><p>Lập dự toán, thẩm định và theo dõi thực hiện tài chính qua các không gian kế toán, thẩm định và phê duyệt riêng.</p><div class="hero-actions"><a class="btn primary" href="accounting.html?mode=demo#/estimates">Lập và theo dõi dự toán</a><a class="btn secondary" href="#/appraisals">Thẩm định · ${pending} hồ sơ chờ</a></div></div></section>
 <section class="khtc-grid" aria-label="Các nghiệp vụ liên quan Kế hoạch - Tài chính">${cards.map(([route,title,description])=>`<article class="panel khtc-card"><h2>${title}</h2><p>${description}</p><a class="btn secondary" href="${route==='appraisals'?'khtc':'accounting'}.html?mode=demo#/${route}">Mở ${title.toLowerCase()}</a></article>`).join('')}</section>
 <section class="panel khtc-roles"><h2>Phân công xử lý dự toán</h2><dl><dt>Cán bộ kế toán</dt><dd>Lập, sửa và gửi thẩm định; xử lý các hồ sơ trả bổ sung.</dd><dt>Cán bộ kế hoạch - tài chính</dt><dd>Kiểm tra, ghi ý kiến và trình duyệt hoặc trả bổ sung; không sửa số tiền trong bước thẩm định.</dd><dt>Hiệu trưởng/người được ủy quyền</dt><dd>Phê duyệt, không phê duyệt hoặc trả bổ sung kèm ý kiến. Khi sửa nội dung, người lập phải gửi thẩm định lại.</dd></dl><a class="btn secondary" href="executive.html?mode=demo#/approvals">Chuyển sang màn phê duyệt</a></section>`;
}
