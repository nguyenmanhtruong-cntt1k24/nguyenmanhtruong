# Kiểm tra frontend giảng viên — 03/11/2026

## Kết quả

- `node --check frontend/app.js`: đạt.
- `git diff --check -- .`: đạt; thư mục mới chưa được Git theo dõi.
- HTTP cục bộ phục vụ được HTML/CSS/JS; 6 route đã mở thành công bằng Playwright.
- 37/37 kiểm tra hành vi và responsive đạt trong một browser context riêng, không thay đổi dữ liệu mẫu của bản mở cho người dùng.

## Các kiểm tra hành vi

1. Phòng A.302 không nhận đăng ký giao với lịch đã duyệt.
2. Giờ kết thúc trước bắt đầu bị chặn.
3. Lưu nháp tạo phiếu ở tab Bản nháp.
4. Sửa và gửi nháp chuyển sang Chờ xét duyệt.
5. Tải lại trang vẫn giữ phiếu đã gửi.
6. Hủy phiếu giữ bản ghi với trạng thái Đã hủy.
7. Xác nhận nhận thiết bị chuyển Chờ nhận thiết bị sang Đang mượn.
8. Yêu cầu trả một phần chuyển Chờ tiếp nhận trả, chưa nhập kho.
9. Nhật ký chặn thời gian thực tế không hợp lệ.
10. Nhật ký gửi thành công xuất hiện trong danh sách chờ xác nhận.
11. Lọc phòng sức chứa tối thiểu 150 chỉ còn hội trường phù hợp.
12. Xuất CSV phát sinh tải xuống tên `phieu-cua-toi.csv`. Chưa kiểm tra nội dung CSV bằng ứng dụng bảng tính.
13. Không phát sinh pageerror JavaScript trong các bước kiểm tra.

## Responsive và hình ảnh

24 kiểm tra không tràn ngang toàn trang: 6 route tại mỗi kích thước 360, 768, 1024 và 1440 px. Bảng có vùng cuộn riêng.

Đã xem ảnh desktop 1440 px và mobile 390 px. Ảnh bàn giao: `preview-desktop-viewport.png`, `preview-mobile.png`. Ảnh full-page ban đầu `preview-desktop.png` có sai lệch vị trí thành phần fixed do quá trình chụp; dùng ảnh viewport để đánh giá sidebar.

Sau kiểm tra đã chỉnh nền nút ngày và canh đầu hai cột nội dung; đã chụp/xem lại ảnh viewport. Chưa kiểm tra độc lập trên Firefox/Safari, phần mềm đọc màn hình hoặc môi trường nhiều người dùng.

## Bàn giao

Máy chủ `http://127.0.0.1:5500` phục vụ thư mục `frontend`; trang mở mặc định `#/overview`. Lệnh mở trình duyệt mặc định đã hoàn thành thành công sau khi được cấp quyền môi trường. Đây là frontend dữ liệu mẫu, không có xác thực hoặc backend xét duyệt thực tế.

## Bổ sung trả thiết bị và giờ trống

- `node frontend/availability.test.mjs`: 14 assertion đạt; bao gồm hai ví dụ người dùng, ranh giới sát giờ, hợp nhất lịch trùng, lịch kín cả ngày và bỏ qua nháp/hủy/từ chối.
- 20 kiểm tra Playwright trong context riêng đạt: gợi ý điền sẵn, sửa giờ, đặt khoảng giữa hai lượt, pending giữ giờ ngay, giữ khoảng còn lại, đặt 07:00–11:30, bắt đầu tiếp từ 11:30, chặn sửa giờ đè lịch, hủy giải phóng, mục trả thiết bị, hỏng cần mô tả, hiển thị phân loại trả, chống gửi trả lặp, 6 kiểm tra responsive (360/768/1440 × 2 màn), không lỗi JavaScript.
- Kiểm tra bổ sung menu Trả thiết bị và breadcrumb đúng: 2/2 đạt.
- Trong quá trình kiểm tra đã phát hiện và sửa lỗi encoding nhãn, điều kiện giữ chỗ chờ duyệt còn dùng quy tắc cũ và menu Trả thiết bị chưa được chèn. Kết quả đạt là sau khi sửa các lỗi này.
- Ảnh kiểm tra: `preview-room-final.png`, `preview-returns-menu.png`. Hai ảnh tên `preview-room-availability*` là bản trung gian, không dùng để đánh giá bản cuối.
- Bản cuối chỉ đồng bộ lịch giữa các tab dùng chung localStorage; chưa kiểm tra hoặc triển khai dữ liệu nhiều người dùng qua máy chủ.
- Đã bổ sung giờ gợi ý cụ thể ngay trên thẻ (ví dụ 08:30–09:20 trong khoảng trống 08:30–09:30), kiểm tra nhãn bằng trình duyệt đạt; ảnh mới nhất `preview-room-final-v3.png` đã xem.

## Bỏ địa điểm mượn và đổi giao diện xanh–trắng

- Đọc trực tiếp Figma `6:103` bằng `get_design_context`, có ảnh tham chiếu. Áp dụng các token màu chính, sidebar, nền, chữ và viền vào CSS hiện có; không sửa file Figma.
- Bỏ trường địa điểm, kiểm tra bắt buộc và thuộc tính location khi tạo/sửa phiếu mượn; bỏ hiển thị địa điểm trên chi tiết và trang trả. Dữ liệu lịch sử trong localStorage không bị xóa.
- 12 kiểm tra Playwright đạt: không có trường địa điểm, gửi mượn không cần địa điểm, phiếu mới không lưu location, màu chính/nền/sidebar khớp token trên 7 route, mobile không tràn ngang và không pageerror.
- Kiểm tra thêm sidebar mobile ẩn đúng và mở menu tới Trả thiết bị thành công. Ảnh mobile dùng animations disabled để tránh chụp giữa hiệu ứng chuyển sidebar.
- `node --check`, 14 assertion thuật toán lịch và `git diff --check` đạt. Đã xem ảnh desktop và form mượn: `preview-blue-overview.png`, `preview-blue-borrow-form.png`; ảnh mobile cuối `preview-blue-mobile-final.png`.
# Bổ sung Ban Giám hiệu — 03/11/2026

- `node --check frontend/executive.js`, `node --check frontend/executive-data.mjs`: đạt.
- `node frontend/executive-data.test.mjs`: 16 kiểm tra đạt (tổng tiền, ngân sách sau duyệt, từ chối không tăng ngân sách, không biến đổi bản gốc, phiên bản, quyết định trùng, bộ lọc).
- `node frontend/availability.test.mjs`: 14 kiểm tra cũ vẫn đạt.
- `tools/qa_executive.js`: 36 kiểm tra Playwright đạt trong context tách biệt. Duyệt 400 triệu: 7,8 → 8,2 tỷ; tải lại còn dữ liệu; từ chối cần lý do và giữ nguyên tổng; lịch sử lưu ý kiến; hồ sơ tab khác đã duyệt bị chặn.
- CSV tải xuống có đúng đơn vị, tổng sau duyệt; báo cáo tài sản/phòng lọc đúng; không dữ liệu thì khóa xuất; chế độ in ẩn điều hướng và giữ báo cáo.
- 5 màn × 360/768/1440px không tràn trang. Đã sửa co giãn grid sau lần đầu phát hiện tràn mobile. Menu điện thoại chuyển trang được. Hộp hồ sơ 360px không tràn ngang; bảng khoản mục cuộn riêng.
- Không có `pageerror`. Giảng viên vẫn mở được. `git diff --check -- .` đạt (không bao phủ file untracked).
- Đã xem ảnh desktop tổng quan, báo cáo, hộp xét duyệt và mobile. Ảnh `preview-executive-mobile-draft.png` là bản trước sửa; các ảnh khác lưu trong `frontend/preview-executive-*.png`.
- Giới hạn: số liệu minh họa, lưu trình duyệt, chưa nối backend/kế toán; CSV và hộp thoại in PDF, chưa xuất Excel trực tiếp. Test không chỉnh dữ liệu trong context trình duyệt của người dùng.

# Quản lý phòng và tài sản — 03/11/2026

- `node --check` cho `app.js`, `resources.js`, `resource-store.mjs`, `resource-seed.mjs`: đạt. `git diff --check -- .` đạt (không bao phủ file untracked).
- `node frontend/resource-store.test.mjs`: 51 kiểm tra đạt. Bao gồm giữ dữ liệu cũ, chuyển trạng thái, từ chối cần lý do, kiểm tra revision, duyệt lịch, không bàn giao lặp, trả từng phần/hỏng/quá hạn, số lượng đồng thời lớn nhất, kiểm kê và bảo trì/chi phí. Phòng còn tài sản cố định hỏng/mất bị chặn mở phục vụ.
- `tools/qa_resources.js`: 27 kiểm tra Playwright đạt, context riêng, chuyển hai vai trò trên một tab. Luồng thêm phòng → giảng viên đăng ký → quản lý duyệt → giảng viên thấy kết quả; duyệt thiết bị → biên bản bàn giao → giảng viên xác nhận; trả hỏng từng phần → nhận phần còn lại → đóng phiếu; bổ sung nhật ký → gửi lại cùng mã → xác nhận; tạo tài sản, chốt kiểm kê có giải trình; sửa thiết bị hỏng phục hồi số tốt; bảo trì chặn giờ phòng; CSV và bố cục in. Không có pageerror.
- `tools/qa_resources_layout.js`: 34 kiểm tra đạt. 10 màn × 360/768/1440px không tràn trang; menu mobile chuyển trang, form tài sản 360px không tràn, Ban Giám hiệu vẫn mở được và không có pageerror.
- Kiểm tra hồi quy: 20 kiểm tra `tools/qa_lecturer_booking.js` đạt sau khi nối dữ liệu dùng chung; 14 assertions `availability.test.mjs` và 16 assertions `executive-data.test.mjs` đạt sau cùng.
- Các lần thử script gộp trước gặp lỗi chọn trùng tên nút trong và ngoài modal, một lần đọc tiêu đề trước khi hash route cập nhật; đã sửa phạm vi selector và chờ đúng trang. Các phiên dài nhiều tab bị đóng giữa chừng; không tính là lượt hoàn tất. Kết quả cuối lấy từ hai script tách ở trên, đều trả kết quả thành công.
- Đã xem ảnh `preview-resources-desktop.png`, `preview-resources-mobile.png`, `preview-resources-calendar.png`, `preview-resources-maintenance.png`, `preview-resources-asset-mobile.png`. Màu xanh–trắng dùng lại bộ CSS và palette Figma của bản trước. Không sửa canvas Figma.
- Toàn bộ QA chạy bằng context trình duyệt riêng; không ghi dữ liệu kiểm tra vào phiên người dùng. Dữ liệu người dùng cũ không bị reset. Chưa có backend/xác thực, giao dịch đồng thời nhiều người dùng hoặc xuất `.xlsx`; chứng từ bảo trì là dữ liệu mẫu, không phải liên kết kế toán thật.


## Kế toán 03/11/2026
17 kiểm tra accounting-data đạt; hồi quy executive 16/resource 51/availability 14 đạt. Playwright context riêng: tạo nháp/gửi BGH/lịch sử BGH, chặn chi vượt ngân sách, lưu chờ, ghi thu, menu mobile đạt; 21 kiểm tra bố cục (7 routes × 360/768/1440), không pageerror. Ảnh preview-accounting-desktop.png, preview-accounting-mobile.png. Chưa kiểm tra mọi bộ lọc hoặc download CSV/in qua UI.
## Quản trị viên — 03/11/2026

- `node frontend/admin-data.test.mjs`: 44 kiểm tra đạt. Định danh/email trùng, dữ liệu sai, bất biến nguồn, bản ghi trước/sau, tự khóa, thu hồi quyền quản trị, vai trò, giới hạn phạm vi, hạn chế riêng, trạng thái tài khoản và revision cũ.
- `tools/qa_admin.js`: 36 kiểm tra Playwright đạt trong context riêng; không chỉnh dữ liệu trình duyệt của người dùng. Tạo/sửa tài khoản, định danh trùng, khóa/mở khóa và kết quả mô phỏng, phạm vi tùy chọn, hạn chế riêng, tạo vai trò/ma trận, bảo vệ quyền quản trị, nhật ký trước/sau, tìm kiếm, biểu mẫu cũ. Sáu route × ba kích thước 360/768/1440 không tràn ngang, menu và form mobile đạt; không có pageerror.
- Đã xem ảnh tổng quan desktop/mobile và form mobile. Ma trận rộng cuộn trong bảng; form mobile cuộn trong dialog.
- Hồi quy pure tests: kế toán 17, BGH 16, tài sản 51, interval 14 đều đạt. `node --check` cho JS/module mới và `git diff --check -- .` đạt (diff chỉ kiểm tra file đã được Git theo dõi).
- Giới hạn xác minh: chưa có kiểm soát quyền phía máy chủ hoặc kết nối quyền vào các trang nghiệp vụ; chưa chạy lại toàn bộ UI các vai trò cũ. Bộ thử không thay thế kiểm tra backend sau này.

## Thêm/sửa/xóa khoa, mô tả và ghi chú tùy chọn — 03/11/2026

- 24 kiểm tra mới `departments.test.mjs`: migrate dữ liệu quản trị cũ không đổi nguồn, mã/tên trùng, thêm/sửa/xóa, tham chiếu đơn vị và phạm vi tài khoản, đơn vị thay thế, lịch sử, mô tả trống.
- `tools/qa_departments.js`: 13 kiểm tra Playwright đạt; thêm/sửa không ghi chú, danh mục đơn vị mới, đổi tên cập nhật tài khoản, xóa có chuyển đơn vị, khóa không lý do, vai trò không mô tả, ghi thu không nội dung, BGH duyệt không ý kiến và bố cục 360/768/1440. Context riêng, không reset dữ liệu của người dùng; không pageerror.
- Hồi quy pure: admin 44, resource 51, executive 16, accounting 17, interval 14 đạt. `tools/qa_resources.js` 27 kiểm tra đạt, đã thay kỳ vọng “bắt buộc giải trình” thành “cho phép ghi chú trống”. Kỳ vọng cũ trong `qa_executive.js` cũng được cập nhật; chưa chạy lại toàn bộ script BGH trong nhiệm vụ này.
- Cú pháp các JS/module thay đổi và `git diff --check -- .` đạt; diff không bao phủ file chưa được Git theo dõi. Đã xem ảnh danh mục khoa desktop/mobile; bảng mobile cuộn ngang bên trong.

## Chọn tài khoản trong cấu hình vai trò — 03/11/2026

- Pure: `role-account.test.mjs` 18 đạt (cấp/bỏ quyền riêng, không ảnh hưởng nguồn/vai trò/người khác, phạm vi, hạn chế, khôi phục kế thừa, đầu vào sai, stale revision, bảo vệ quản trị và gỡ vai trò).
- UI: `tools/qa_role_account.js` 9 đạt trong context riêng, 0 pageerror: chọn/lưu/nạp lại, mô phỏng cấp/bỏ quyền, khôi phục quyền chung, chuyển tài khoản và quay lại cấu hình chung, nhật ký, form mobile không tràn.
- Hồi quy `admin-data.test.mjs` 44, `departments.test.mjs` 24 đạt; cú pháp và `git diff --check -- .` đạt. Đã xem ảnh `preview-role-account-desktop.png`, `preview-role-account-mobile.png`.


## 03/11/2026 — Bỏ chọn chủ sở hữu
Màn Kiểm tra quyền đã bỏ trường Chủ sở hữu dữ liệu. Dữ liệu bản thân tự xét theo tài khoản cần kiểm tra. Kiểm tra UI: không còn trường owner, kiểm tra bản thân và tài khoản khóa đúng, không tràn 360/1440; 18 pure phân quyền đạt.
# Form đăng nhập — 03/11/2026

- `login.html`: giao diện xanh trắng, tên đăng nhập/email, hiện/ẩn mật khẩu, hỗ trợ quên mật khẩu, danh sách tài khoản demo.
- Playwright trong context riêng: 5 vai trò chuyển đúng trang; sai mật khẩu và tài khoản khóa bị từ chối; mobile 360px không tràn ngang. Đã xem ảnh desktop.
- `node --check frontend/login.js` đạt. Đây là đăng nhập mô phỏng, chưa có xác thực backend hoặc bảo vệ các URL workspace; nội dung workspace vẫn dùng danh tính demo có sẵn.
- Mật khẩu dùng thử chung `Demo@123`; không lưu mật khẩu nhập vào storage. Phiên mô phỏng chỉ lưu userId/roleId/thời gian trong sessionStorage.

# Menu tài khoản — Đổi mật khẩu và đăng xuất

- Menu tại tên tài khoản ở thanh trên cùng, dùng chung cho 5 vai trò. Có đóng khi bấm ngoài hoặc Escape; hộp đổi mật khẩu có nhãn và hiện/ẩn mật khẩu.
- Đổi mật khẩu yêu cầu đăng nhập, tài khoản hoạt động, mật khẩu hiện tại đúng, mật khẩu mới 8–128 ký tự gồm chữ và số, xác nhận khớp. Mật khẩu demo được lưu dưới dạng PBKDF2 có salt riêng trên trình duyệt; không lưu mật khẩu thô.
- Playwright context riêng: sai mật khẩu hiện tại, xác nhận không khớp, đổi thành công, đăng xuất xóa phiên, mật khẩu cũ bị từ chối, mật khẩu mới đăng nhập được. Menu hiện đủ 5 vai trò; mobile 360px không tràn ngang. Syntax JS đạt.
- Phạm vi vẫn là frontend demo: chưa có xác thực hoặc thu hồi phiên phía máy chủ, chưa chặn URL truy cập trực tiếp. Không thay đổi dữ liệu người dùng khi chạy QA.

# Cấp lại mật khẩu trong Quản lý tài khoản

- Nút Đặt lại mật khẩu trên từng dòng: mật khẩu mới, xác nhận, checkbox đồng ý; cần phiên quản trị có quyền quản lý tài khoản. Không tự mở khóa tài khoản.
- Lưu mật khẩu demo bằng PBKDF2 cùng cơ chế đổi mật khẩu; nhật ký chỉ lưu sự kiện, không lưu mật khẩu.
- Playwright context riêng: quản trị cấp lại cho USR-002, có audit password-reset, người dùng đăng nhập bằng mật khẩu mới thành công. Node syntax và bộ admin-data đạt.
- Đây là cơ chế demo trên trình duyệt, chưa có backend xác thực.

# In chứng từ chi — kế toán

- Sau ghi nhận khoản chi có nút In chứng từ chi; có thể in lại từ từng dòng chi trong Sổ thu – chi. Khoản thu, yêu cầu chưa chi và tổng hợp đầu kỳ không có nút in phiếu chi.
- Trang payment-voucher.html lấy khoản chi đã lưu theo id, hiển thị số/ngày chứng từ, đơn vị, nội dung, số tiền, mã giao dịch và bốn ô ký. Dữ liệu được gán bằng textContent, không nội suy HTML từ chứng từ.
- Playwright context riêng: tạo khoản chi 100.000 đồng, xem đúng nội dung và mã, kiểm tra gọi window.print bằng stub, chế độ print ẩn thanh công cụ, id không tồn tại vô hiệu hóa in. Đã xem ảnh chế độ in. Không gửi lệnh đến máy in thực tế.
- Kiểm tra cú pháp và 17 kiểm tra accounting-data đạt. Phiếu hiện là chứng từ minh họa của frontend.

# Đồng bộ chi phí bảo trì sang kế toán

- Bỏ chọn chứng từ mẫu khi hoàn tất bảo trì. Đang bảo trì có Lưu & in chứng từ chi phí phát sinh; sau lưu có thể in lại, hoàn tất có nút in tương tự. Số tiền gửi kế toán được giữ nguyên.
- Kế toán đọc các chi phí đã lưu (kể cả kế hoạch hoàn tất từ trước), tạo một yêu cầu theo sourceMaintenance, không tạo trùng khi tải lại hoặc sau khi chi. Chưa chi không trừ ngân sách; ghi nhận chi vẫn kiểm tra ngân sách và khớp số tiền/đơn vị/năm.
- Bản in phát sinh ghi rõ Chưa chi hoặc Đã chi. Không có chi phí thì không tạo yêu cầu chi.
- Playwright context riêng: lưu/in đang bảo trì 3.500.000 đồng, hàng chờ có một dòng sau reload, xử lý chi, bản in đổi Đã chi, hoàn tất bảo trì có nút in. Bộ 51 resource-store, 17 accounting-data và kiểm tra liên kết maintenance-finance đạt; syntax/diff check đạt.

