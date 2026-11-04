# Frontend giảng viên HPUNI

HTML, CSS và JavaScript thuần. Không cần cài thư viện hoặc build.

## Mở ứng dụng

Từ workspace chạy:

```powershell
python -m http.server 5500 --bind 127.0.0.1 --directory frontend
```

Mở `http://127.0.0.1:5500/#/overview`.

## Phạm vi hoạt động

- Tổng quan: số liệu từ dữ liệu mẫu, lịch tuần tương tác, phiếu gần đây và thông báo.
- Lịch của tôi: chọn ngày, xem lịch phòng đã duyệt.
- Đăng ký phòng: lọc tên, loại, sức chứa; kiểm tra thời gian, số sinh viên, bảo trì và lịch đã duyệt trùng.
- Đăng ký thiết bị: tìm/lọc, số lượng, thời gian và mục đích; kiểm tra số lượng các đăng ký giao nhau. Không yêu cầu nhập địa điểm sử dụng.
- Phiếu của tôi: lọc, xem chi tiết, lưu/sửa nháp, gửi đăng ký, hủy phiếu chờ duyệt/nháp, xác nhận nhận thiết bị, gửi yêu cầu trả từng phần và xuất CSV theo bộ lọc.
- Nhật ký: thời gian thực tế, số sinh viên, tình trạng, mô tả sự cố; gửi chờ quản lý xác nhận.
- Trả thiết bị: mục riêng trong menu, xem số đang giữ, trả từng phần và phân loại tốt/hỏng/mất; phiếu đã gửi trả hiển thị trạng thái chờ tiếp nhận.
- Dữ liệu lưu ở localStorage với khóa `hpuni-lecturer-v1`; các phiếu được hủy bằng trạng thái, không xóa lịch sử.

## Giới hạn demo

Mở trực tiếp không gian của tài khoản giảng viên giả lập Nguyễn Minh Anh. Không có xác thực thật, API hoặc màn quản lý xét duyệt. Phiếu mới không tự được duyệt; yêu cầu trả không tự nhập kho. Số lượng danh mục là dữ liệu mẫu; dữ liệu của các tài khoản khác và giao dịch nhiều người chưa có.

Ngày mẫu được tạo tương đối với ngày mở lần đầu. Ngày đã lưu được giữ lại khi tải trang. Font Be Vietnam Pro lấy qua Google Fonts; khi không có mạng dùng Arial. Không sử dụng tài khoản thật hoặc dữ liệu trong taikhoan.md.

Khôi phục dữ liệu mẫu khi cần bằng bảng điều khiển trình duyệt: `localStorage.removeItem('hpuni-lecturer-v1')`, sau đó tải lại. Chỉ thực hiện nếu muốn bỏ thay đổi demo của chính mình.

Thiết kế dựa trên đặc tả và Figma; chưa xác nhận trùng từng pixel. Đã đọc trực tiếp ngữ cảnh thiết kế và ảnh node `6:103` ngày 03/11/2026: xanh chính `#004080`, menu `#052c52`, nền `#f3f6fa`, chữ `#172b43`, chữ phụ `#63758a`, đường viền `#dee6ef`, thẻ trắng. Toàn bộ 7 màn dùng chung bảng màu xanh dương–trắng. Bố cục và các chức năng mở rộng hiện có được giữ. Responsive hỗ trợ mobile; dữ liệu giao diện không phải dữ liệu vận hành thật của Nhà trường.

## Đăng ký phòng theo giờ trống — cập nhật 03/11/2026

- Chọn ngày; danh sách mỗi phòng chỉ hiện khoảng giờ còn trống, trừ các lượt chờ duyệt và đã duyệt. Không hiển thị phòng đang bảo trì hoặc đã kín trong khoảng đang tìm.
- Nhấn khoảng trống để điền gợi ý 50 phút; thời gian có thể chỉnh tự do trong giờ phục vụ mẫu 07:00–18:00. Đây là cấu hình demo, chưa phải lịch phục vụ chính thức của trường.
- Nhập Từ giờ/Đến giờ để chỉ tìm các phòng trống trọn khoảng đó. Nút Xem cả ngày bỏ hai điều kiện giờ.
- Gửi đăng ký giữ toàn bộ khoảng đã chọn ngay; lưu nháp không giữ chỗ. Hủy giải phóng đúng khoảng đó. Hai lượt nối tiếp được dùng chung mốc kết thúc/bắt đầu.
- Ví dụ: 07:00–11:30 đã đặt thì không tìm được phòng cho 07:00–11:30; nhưng từ 11:30 vẫn đặt được. Đã đặt 07:00–08:30 và 09:30–11:30 thì khoảng 08:30–09:30 vẫn trống.
- Phòng A.201 có sẵn hai lượt mẫu của giảng viên khác vào ngày khởi tạo dữ liệu để minh họa khoảng trống 08:30–09:30. Các lượt này chỉ tham gia lịch chung, không xuất hiện trong Phiếu của tôi. Không thay thế hoặc xóa các phiếu cũ của người dùng.
- Các tab cùng trình duyệt cập nhật khi localStorage thay đổi; kiểm tra lại trước khi gửi. Nhiều người trên thiết bị khác nhau và giữ chỗ đồng thời nguyên tử cần backend, chưa nằm trong frontend demo.

Kiểm tra thuật toán: `node frontend/availability.test.mjs` từ workspace. Module `availability.mjs` dùng khoảng nửa mở, gộp các lịch bận rồi lấy phần trống.

Trước khi mở rộng đã đối chiếu ý tưởng kiểm tra giao nhau với [BookingManagement](https://github.com/Abedalqaders/BookingManagement), mục chống đặt trùng: hai khoảng giao nhau khi bắt đầu lượt này trước kết thúc lượt kia và kết thúc sau bắt đầu lượt kia. Chỉ tham khảo quy tắc, không sao chép mã nguồn hoặc bổ sung thư viện; chưa xác định giấy phép dự án từ trang đã đọc.
# Không gian Ban Giám hiệu

Mở `http://127.0.0.1:5500/executive.html#/overview`. Có nút chuyển không gian demo trong sidebar của cả Ban Giám hiệu và giảng viên.

- 5 màn: Tổng quan điều hành, Duyệt dự toán, Theo dõi ngân sách, Lịch sử xét duyệt, Báo cáo tổng hợp.
- Dự toán có khoản mục, số lượng, đơn giá, tổng tiền, đơn vị và kỳ. Phê duyệt/từ chối cần ý kiến và xác nhận; hồ sơ đã xử lý không duyệt lại. Kiểm tra lại trạng thái/phiên bản trước khi ghi.
- Ngân sách được tính từ dự toán đã duyệt; một lần lưu giữ cả quyết định và dữ liệu làm căn cứ ngân sách. Từ chối không tạo ngân sách. Chi thực hiện lấy từ dữ liệu mẫu, không tính hồ sơ chờ duyệt vào chi thực hiện.
- Bộ lọc năm 2026/2027, đơn vị, từ khóa và kết quả xét duyệt. Ngân sách và báo cáo phản ánh quyết định đã lưu.
- 3 báo cáo: ngân sách; tài sản & chi phí bảo trì; thống kê phòng. Xuất CSV UTF-8 đúng bộ lọc; In / Lưu PDF dùng hộp thoại in của trình duyệt. Chưa có xuất `.xlsx` trực tiếp.
- Tài sản/phòng là ảnh chụp dữ liệu mẫu 2026, không phải dữ liệu đồng bộ với không gian giảng viên. Năm không có dữ liệu hiển thị trạng thái rỗng.
- Dữ liệu riêng ở `hpuni-executive-v1`, giữ nguyên dữ liệu giảng viên. Không đọc được dữ liệu đã lưu thì tạm khóa xét duyệt, không ghi đè tự động.
- Đây là frontend demo, chưa có xác thực hoặc quyền máy chủ. Vai trò demo được cấp xem và xuất; cần kiểm tra độc lập các quyền khi nối backend. Chống xử lý hồ sơ cũ giữa các tab không thay thế giao dịch/khóa đồng thời của backend cho nhiều người dùng.
- Không xây lại các chức năng dự báo, phân tích hiệu quả hay đề xuất phân bổ đã bỏ khỏi phạm vi trong `plan.md`.

Kiểm tra dữ liệu: `node frontend/executive-data.test.mjs` (chạy tại thư mục workspace). QA trình duyệt dùng `tools/qa_executive.js` qua Playwright `browser_run_code_unsafe`; chạy trong browser context riêng, không thay đổi dữ liệu demo của người dùng.

# Không gian Quản lý phòng và tài sản

Mở `http://127.0.0.1:5500/resources.html#/overview`. Sidebar các vai trò có liên kết chuyển không gian demo.

- 10 màn: Tổng quan, Danh mục phòng, Lịch sử dụng phòng, Duyệt đăng ký, Mượn / trả thiết bị, Hồ sơ tài sản, Kiểm kê, Bảo trì, Xác nhận nhật ký, Báo cáo quản trị.
- Dùng chung khóa `hpuni-lecturer-v1` với giảng viên; bổ sung danh mục, hồ sơ tài sản, kiểm kê, bảo trì và nhật ký thao tác khi cần. Giữ nguyên phiếu, nhật ký và các trường dữ liệu cũ; không xóa file hoặc bản ghi. Bản Ban Giám hiệu vẫn dùng dữ liệu tài chính demo riêng.
- `resource-seed.mjs` chứa danh mục và seed giảng viên đã tách từ `app.js`. `resource-store.mjs` chuẩn hóa dữ liệu cũ và kiểm tra chuyển trạng thái, khoảng lịch, số lượng. Giảng viên đọc danh mục được quản lý cập nhật và các khoảng bảo trì.
- Duyệt phòng kiểm tra sức chứa, lịch giữ chỗ và bảo trì; duyệt mượn kiểm tra số thiết bị tốt khả dụng. Duyệt thiết bị → lập biên bản bàn giao → chờ giảng viên tự xác nhận nhận → đang mượn.
- Tiếp nhận trả từng phần: đối chiếu số thực tế tốt/hỏng/mất, lưu thời điểm và cờ quá hạn, giữ phần chưa xử lý. Hỏng/mất không cộng vào số tốt. Sửa chữa thành công mới phục hồi phần hỏng; báo mất đã được xác minh được ghi nhận riêng. Số `returned` là số đã xử lý trả gồm cả trường hợp mất đã xác minh, không đồng nghĩa toàn bộ là vật tư tốt thực nhận.
- Phiếu đã nhận chưa trả tiếp tục chiếm số lượng kể cả sau hạn đăng ký; hai khoảng chạm mốc không giao nhau. Khả dụng nhóm thiết bị dùng số lượng chiếm dụng đồng thời lớn nhất, tránh cộng dồn sai các lượt nối tiếp.
- Tài sản quản lý theo nhóm cùng loại, chưa theo serial từng chiếc. Có ghi tăng và sửa hồ sơ; loại cho mượn bổ sung danh mục giảng viên, loại cố định có thể liên kết phòng. Không giảm số lượng xuống dưới phần hỏng, mất và đang giữ/mượn. Không ngừng phục vụ phòng còn lịch hiệu lực.
- Kiểm kê lưu ảnh chụp tên và số lượng theo sổ lúc lập đợt. Có lưu tiến độ; chỉ hoàn tất khi các mục đủ kết quả, chênh lệch/bất thường có giải trình. Kết quả không tự sửa tồn kho.
- Bảo trì: kế hoạch → bắt đầu thực tế → hoàn tất. Kiểm tra xung đột trước khi lập lịch; phần đang tốt được giữ theo lịch, phần hỏng không trừ hai lần. Kế hoạch đang thực hiện tiếp tục chặn cho đến khi hoàn tất. Chi phí liên kết một trong hai chứng từ mẫu trong `VOUCHERS`; phải khớp số tiền và không gắn trùng một chứng từ. Không tạo khoản chi mới hoặc thay đổi tài chính Ban Giám hiệu.
- Nhật ký: xác nhận hoặc yêu cầu bổ sung kèm ý kiến. Giảng viên sửa và gửi lại cùng nhật ký, giữ lịch sử ý kiến, không tạo bản trùng.
- Báo cáo danh mục/tình trạng, mượn trả/quá hạn, chi phí bảo trì: bộ lọc đơn vị hoặc khoảng ngày phù hợp; CSV UTF-8 và in/lưu PDF của trình duyệt. Chưa có `.xlsx` trực tiếp. Nhắc việc quá hạn chỉ ghi nội bộ, không gửi email/tin nhắn.
- Chưa có backend, đăng nhập hay phân quyền máy chủ. Chuyển vai trò là thao tác demo. Các tab cập nhật qua sự kiện storage và kiểm tra revision trước lưu; không thay thế giao dịch/khóa nguyên tử của backend cho nhiều người dùng. Không dùng dữ liệu thật hoặc thông tin nhạy cảm.

Trước khi triển khai đã đối chiếu cách tách bàn giao, nhận trả, kiểm kê và tình trạng cho mượn với mã nguồn [Snipe-IT Asset model](https://github.com/grokability/snipe-it/blob/master/app/Models/Asset.php) và [cấu trúc giao diện tài sản](https://github.com/grokability/snipe-it/blob/master/resources/views/hardware/view.blade.php). Chỉ tham khảo ý tưởng, không sao chép mã hoặc cài thư viện của dự án.

Kiểm tra nghiệp vụ: `node frontend/resource-store.test.mjs` (51 kiểm tra). Kiểm tra luồng UI: `tools/qa_resources.js` (27 kiểm tra) và `tools/qa_resources_layout.js` (34 kiểm tra) qua Playwright `browser_run_code_unsafe`, dùng context riêng để không sửa dữ liệu người dùng.


## Kế toán — 03/11/2026
- Mở `accounting.html#/overview`: 7 màn tổng quan, dự toán, thu chi, chi chờ xử lý, ngân sách, lịch sử, báo cáo.
- Dùng chung `hpuni-executive-v1` với BGH; thêm receipts/paymentQueue/accountingAudit, không xóa dữ liệu. Dự toán nhiều dòng, nháp/sửa bị từ chối/gửi duyệt. Khoản chi kiểm tra ngân sách, chứng từ trùng, năm và ngày; thu không tự tăng ngân sách.
- CSV sổ thu chi và in báo cáo; lọc năm/đơn vị. Chưa backend, phân quyền thật, đính kèm chứng từ hoặc giao dịch đồng thời nguyên tử. Tài sản vẫn dùng chứng từ bảo trì demo riêng.
- Tham khảo ý tưởng kiểm soát ngân sách ERPNext: https://github.com/frappe/erpnext (không sao chép mã).
- Kiểm tra: accounting-data.test.mjs 17 đạt; executive 16, resource 51, availability 14 đạt; cú pháp và diff whitespace đạt. Playwright context riêng: tạo/gửi dự toán xuất hiện ở BGH, lịch sử BGH khi có nháp, chặn chi vượt ngân sách/lưu hàng chờ/ghi thu/menu mobile đạt; 7 routes × 3 widths không tràn, 0 pageerror trong lượt layout. Ảnh preview-accounting-desktop/mobile.png. Chưa kiểm tra mọi bộ lọc và thao tác CSV/in trên trình duyệt.
## Quản trị viên — 03/11/2026

- Mở `admin.html#/overview`. Sáu màn: tổng quan, tài khoản, vai trò và ma trận quyền, phạm vi dữ liệu, kiểm tra quyền, nhật ký quản trị.
- Tạo/sửa tài khoản, kiểm tra định danh/email trùng; gán nhiều vai trò. Khóa/mở khóa có lý do và xác nhận. Không xóa tài khoản hoặc vai trò.
- Năm vai trò gốc tương ứng năm không gian hiện có, cho phép tạo vai trò bổ sung. Cấu hình 16 quyền chức năng, phạm vi tối đa của vai trò và phạm vi riêng của tài khoản; tài khoản có thể bị hạn chế thêm từng chức năng.
- Mô phỏng chỉ cho phép khi tài khoản hoạt động, vai trò cấp chức năng, không bị hạn chế riêng và dữ liệu nằm trong cả hai phạm vi. Phạm vi gồm toàn trường, đơn vị công tác, đơn vị chọn (tài khoản) và bản thân. Khi có nhiều vai trò, chỉ cần một vai trò cấp chức năng và thỏa phạm vi.
- `hpuni-admin-v1` lưu cấu hình riêng trên trình duyệt; nhật ký giữ bản trước/sau. Biểu mẫu giữ revision và từ chối ghi nếu dữ liệu đã thay đổi; không phải giao dịch nhiều người dùng nguyên tử. Dữ liệu lỗi khóa ghi, không tự reset.
- Tài khoản đang thao tác cố định `USR-001`; chặn tự khóa và thu hồi quyền quản trị. Vai trò gốc quản trị phải giữ bốn quyền hệ thống cùng phạm vi toàn trường.
- Giới hạn: chưa đăng nhập/backend, không cấp/lưu mật khẩu hoặc gửi email. Cấu hình quyền áp dụng trong màn mô phỏng, **chưa kiểm soát truy cập các trang nghiệp vụ**. Không coi localStorage là cơ chế bảo mật thật. Các tài khoản seed chỉ là định danh demo.
- Tham khảo cách tách người dùng, vai trò, quyền và phạm vi của [Apache Casbin](https://github.com/apache/casbin), không sao chép mã hoặc thêm thư viện.
- Kiểm tra: `node frontend/admin-data.test.mjs` (44 kiểm tra), `tools/qa_admin.js` chạy bằng Playwright (36 kiểm tra). Ảnh `preview-admin-desktop.png`, `preview-admin-mobile.png`, `preview-admin-roles.png`, `preview-admin-form-mobile.png`.

## Cập nhật khoa và trường tùy chọn — 03/11/2026

- Quản trị viên có thêm `admin.html#/departments`: thêm, sửa, xóa khoa; mã/tên duy nhất; ghi chú tùy chọn. Danh mục khoa được dùng trong lựa chọn đơn vị tài khoản, bộ lọc, phạm vi và màn kiểm tra quyền của quản trị viên.
- Khi đổi tên, cập nhật đơn vị/phạm vi tài khoản liên quan. Khi xóa khoa đang có liên kết, người dùng chọn đơn vị thay thế và xác nhận xóa; không xóa tài khoản. Nhật ký giữ thông tin khoa trước/sau. Hồ sơ nghiệp vụ ở các không gian khác giữ nguyên tên đơn vị cũ; chưa đồng bộ danh mục sang bộ lọc kế toán/BGH.
- Dữ liệu quản trị cũ được bổ sung danh mục khoa mặc định khi thiếu trường `departments`; giữ nguyên tài khoản, vai trò, nhật ký. Tổng danh mục quyền hiện là 17, bổ sung `departments.manage` cho vai trò quản trị mặc định.
- Theo yêu cầu mới, nội dung, mục đích, mô tả, ghi chú, lý do và ý kiến trong các biểu mẫu đều cho phép để trống. Áp dụng từ HTML tới xử lý dữ liệu ở quản trị, kế toán, BGH, quản lý tài sản và giảng viên; kể cả ghi chú kiểm kê chênh lệch/hỏng/mất. Quy định bắt buộc các trường này trong ghi chú phiên bản cũ bên dưới đã được thay thế. Mã/tên định danh, số tiền/số lượng, thời gian và xác nhận thao tác vẫn được kiểm tra.
- Kiểm tra bổ sung: `departments.test.mjs` 24 đạt; `tools/qa_departments.js` 13 đạt; hồi quy quản trị 44, tài sản 51, BGH 16, kế toán 17, interval 14 đạt. UI quản lý tài sản 27 đạt sau khi cập nhật kỳ vọng ghi chú tùy chọn.

## Phân quyền từng tài khoản trong cấu hình vai trò — 03/11/2026

- Trong “Vai trò & quyền” → “Quyền & cấu hình”, ô “Chọn tài khoản phân quyền” cho chọn cấu hình chung hoặc một tài khoản đang thuộc vai trò đó. Chọn tài khoản sẽ nạp quyền/phạm vi riêng nếu đã có, hoặc quyền chung làm giá trị ban đầu.
- Lưu riêng qua `role-account` vào `user.roleOverrides[roleId] = {permissions, scope}`. Bộ quyền này thay thế phần quyền của vai trò đó đối với tài khoản đã chọn; các vai trò khác vẫn được kết hợp. Không đổi cấu hình chung, thông tin vai trò hay quyền người khác. Phạm vi tài khoản, hạn chế riêng và trạng thái khóa vẫn có hiệu lực.
- Chọn “Khôi phục quyền và phạm vi theo cấu hình chung của vai trò” rồi lưu để bỏ cấu hình riêng. Khi bỏ gán vai trò khỏi tài khoản, cấu hình riêng tương ứng được bỏ theo. Dữ liệu cũ không có override giữ nguyên hành vi.
- Nhật ký giữ trước/sau tài khoản; chặn thu hồi quyền quản trị của tài khoản đang thao tác và chặn biểu mẫu cũ như trước. Ma trận tại trang vai trò chỉ thể hiện quyền chung; cấu hình riêng được xem bằng cách chọn tài khoản trong dialog.
- 18 kiểm tra mới `role-account.test.mjs`, 9 kiểm tra Playwright `tools/qa_role_account.js` đạt, không pageerror. Đã xem ảnh dialog desktop/mobile. Vẫn là mô phỏng frontend; chưa thực thi quyền trên các không gian nghiệp vụ.

