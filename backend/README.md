# Backend HPUNI-STII — ASP.NET Core và SQL Server

**Đợt hoàn thiện sau rà soát 04/11/2026:** đã có API `PUT /api/v1/equipment-loans/{id}` sửa nháp thiết bị, batch lịch sử trả theo trang, giữ header bảo vệ ở phản hồi lỗi và cấu hình proxy tin cậy opt-in. Hướng dẫn/cấu hình triển khai cùng origin tại [deployment-same-origin.md](docs/deployment-same-origin.md); chưa triển khai công khai. Nội dung số test và giới hạn ở các biên bản cũ bên dưới là lịch sử từng đợt.

**Bản mới đang chạy:** http://127.0.0.1:5081/login.html — có phân trang giảng viên, sửa nháp trực tiếp và125 kiểm tra C#/49 kiểm tra Node đạt. Xem [bàn giao mới nhất](docs/backend-frontend-completion-20261104.md). Cổng5080 là phiên bản cũ được giữ nguyên.

**Đợt hoàn thiện:** thông báo/nhắc việc, điều chỉnh ngân sách, khóa/mở sổ, thanh toán từng phần và đảo chứng từ; báo cáo chi tiết phân trang/CSV/XLSX/trang in PDF; hồ sơ nguyên giá và thanh lý có duyệt. Có adapter ClamAV và script sao lưu/khôi phục đã diễn tập. Xem [biên bản mới nhất](docs/completion-verification.md), [tài chính](docs/finance-controls-verification.md), [tài sản](docs/asset-lifecycle-verification.md) và [điều kiện production](docs/production-readiness.md). Các đoạn ghi số test bên dưới là lịch sử từng đợt. Chưa tính khấu hao, chưa triển khai công khai.

**Báo cáo và tệp chứng từ:** có báo cáo SQL thật, xuất CSV theo quyền và bộ lọc, upload/download PDF/PNG/JPEG gắn khoản thu/chi. Xem [kiểm chứng và giới hạn](docs/reports-files-verification.md). Đã đạt 62 kiểm thử C# và 8 JavaScript; dữ liệu/tệp chạy thử lưu trong SQL Server cục bộ.

**Tài chính đã nối API:** Kế toán/Ban Giám hiệu có dự toán, duyệt cấp ngân sách, thu/chi, hàng chờ và liên kết chi phí bảo trì với chứng từ. Xem [kiểm chứng và giới hạn](docs/finance-verification.md). Đã đạt 51 kiểm thử C# và 8 JavaScript; chỉ chạy cục bộ, chưa phát hành Vercel.

**Đã nối frontend gốc:** chạy backend rồi mở `http://127.0.0.1:5080/login.html` để dùng các màn giảng viên, quản lý và quản trị đã có trên GitHub. Xem [phạm vi và kiểm chứng](docs/original-frontend-integration.md). Bản GitHub/Vercel chưa được cập nhật, theo lựa chọn chạy thử cục bộ.

API dùng SQL Server cho xác thực/phân quyền, quản trị, phòng, thiết bị/tài sản, kiểm kê, bảo trì, nhật ký và tài chính. Các màn frontend gốc mặc định gọi máy chủ; demo cũ chỉ mở rõ ràng bằng `?mode=demo`. `live.html` vẫn là giao diện phụ cho các luồng trước.

## Giao diện kết nối SQL Server

Sau khi chạy backend, mở `http://127.0.0.1:5080/`. Máy chủ chuyển tới `login.html`, phục vụ giao diện và API cùng nguồn. Không mở bằng `file://` hoặc Live Server khác cổng.

- Đăng nhập `admin.hpuni` bằng mật khẩu đã nhập khi bootstrap. Trong **Tài khoản**, cấp tài khoản giảng viên và quản lý phòng/tài sản. Tài khoản mới có phạm vi cá nhân; chọn **Cấp phạm vi** phù hợp cho người quản lý, ví dụ đơn vị hoặc toàn trường. Phạm vi tài khoản và vai trò được xét đồng thời.
- Người quản lý dùng **Thêm phòng** để tạo danh mục. Giảng viên chọn **Danh mục phòng**, xem giờ trống, lưu nháp, rồi **Gửi xét duyệt**. Người quản lý xử lý tại **Duyệt phiếu**.
- Sau lịch đã duyệt, giảng viên ghi nhật ký trong chi tiết phiếu. Người quản lý yêu cầu bổ sung hoặc xác nhận. Nhật ký đã xác nhận không cho sửa.
- Dữ liệu, phiên bản và quyền được máy chủ kiểm tra. Mật khẩu, cookie và dữ liệu nghiệp vụ không được lưu vào localStorage. Khi báo dữ liệu cũ, về danh sách và mở lại phiếu.
- Giao diện được đóng gói trong thư mục `PublicPortal` khi `dotnet publish`. Chỉ bốn tệp công khai được phục vụ; không công khai thư mục workspace.

Đây là giao diện chức năng đợt đầu, chưa chuyển các màn thiết kế demo sang API. Chưa có giao diện chỉnh nháp, sửa phòng, lịch bảo trì, quản lý vai trò/override/khóa tài khoản đầy đủ; các API tương ứng đã có. Thiết bị, tài chính và báo cáo tiếp tục theo kế hoạch. Xem [kết quả kiểm tra giao diện](docs/portal-verification.md).

## Tài sản và mượn/trả thiết bị

Trong giao diện thật, cán bộ quản lý dùng **Hồ sơ tài sản** để tạo nhóm, rồi **Nhập kho và xem biến động** để ghi nhận số lượng. Giảng viên dùng **Thiết bị** để kiểm tra lịch và lập phiếu, **Phiếu mượn** để gửi/xác nhận nhận/yêu cầu trả. Cán bộ xử lý tại **Quản lý mượn/trả**. Phần hỏng/mất không tự trở lại tồn cho mượn.

Đã có API/giao diện mượn/trả; xem [phạm vi và kiểm tra](docs/equipment-verification.md). Trong **Kiểm kê**, lập đợt theo đơn vị, lưu kiểm đếm hoặc hoàn tất; kết quả không tự sửa tồn. Trong **Bảo trì**, lập lịch bảo dưỡng/sửa phần hỏng, bắt đầu và ghi kết quả. Nhật ký thiết bị nằm trong chi tiết phiếu mượn, gửi sau lịch và được quản lý xét duyệt. Xem [kiểm chứng kiểm kê/bảo trì/nhật ký](docs/asset-operations-verification.md). Chi phí chưa liên kết chứng từ/thanh toán. Cần migrate khi cập nhật; không bootstrap lại database đã có người dùng.

## Môi trường phát triển

- .NET SDK 10.0.401, net10.0; EF Core/Identity/OpenAPI/SQL Server provider 10.0.14.
- SQL Server instance `.\SQL2025` đang chạy, Windows Authentication. Database riêng `HpuniDevelopment`.
- Local config bật Encrypt=True và TrustServerCertificate=True chỉ cho phát triển trên máy; cấu hình chứng chỉ hợp lệ khi triển khai.
- Có thể thay connection string qua User Secrets hoặc biến môi trường `ConnectionStrings__Hpuni`, không commit thông tin đăng nhập.

## Khởi tạo và chạy

Tại thư mục `backend`, lần đầu:

```powershell
$env:ASPNETCORE_ENVIRONMENT='Development'
dotnet tool restore
dotnet restore Hpuni.sln --locked-mode
dotnet run --project src/Hpuni.Api -- --migrate
dotnet run --project src/Hpuni.Api -- --bootstrap
```

Lệnh bootstrap yêu cầu nhập mật khẩu ẩn tại terminal rồi tạo `admin.hpuni`. Mật khẩu cần 12–128 ký tự, có chữ và số; không có mật khẩu mặc định. Khi chạy tự động không có terminal tương tác, cung cấp Bootstrap:Password qua môi trường hoặc secret store. Bootstrap từ chối nếu database đã có tài khoản, không reset dữ liệu. Đã áp dụng migrations trong phiên triển khai, chưa tạo tài khoản quản trị phát triển tự động.

Các lần sau:

```powershell
$env:ASPNETCORE_ENVIRONMENT='Development'
dotnet run --project src/Hpuni.Api --urls http://127.0.0.1:5080
```

Địa chỉ:

- API: `http://127.0.0.1:5080/api/v1/`
- Sống: `http://127.0.0.1:5080/health/live`
- Sẵn sàng database: `http://127.0.0.1:5080/health/ready`
- OpenAPI JSON trong Development: `http://127.0.0.1:5080/openapi/v1.json`

Chưa có giao diện Swagger UI. OpenAPI và mã DTO trong `Hpuni.Application/Contracts.cs` mô tả hợp đồng. Có script PowerShell tiện dụng trong scripts/ nếu môi trường cho phép chạy script; máy hiện tại chặn .ps1, nên dùng các lệnh trực tiếp ở trên. Không thay ExecutionPolicy. Kiểm tra HTTP bằng `node scripts/smoke-http.mjs` khi server đang chạy.

## Cách gọi API

1. GET `/api/v1/auth/csrf`, giữ cookie và lấy `token`.
2. POST `/api/v1/auth/login` với `{userName,password}` và header `X-CSRF-TOKEN`.
3. Lấy token CSRF mới sau login, dùng cookie cho các API tiếp theo. Không lưu mật khẩu/cookie vào localStorage.
4. Dùng `version` Base64 từ phản hồi khi sửa hoặc chuyển trạng thái.
5. Tạo phiếu/submit/approve/reject/cancel và tạo/hủy block có header `Idempotency-Key` riêng. Khi retry cùng thao tác giữ nguyên key và payload; nếu nội dung đổi phải dùng key mới.
6. Logout/đổi mật khẩu xong lấy lại token CSRF trước lần đăng nhập tiếp theo.

Phạm vi mặc định tài khoản mới là self. Sau khi tạo quản lý/BGH/kế toán, quản trị viên cấp scope phù hợp qua `/users/{id}/scope`. Các lệnh ghi quản trị yêu cầu đủ bốn quyền quản trị cốt lõi toàn trường trong đợt đầu; đọc danh sách tài khoản có thể được phân quyền hẹp hơn.

Danh mục phòng dùng chung cho người có quyền tra cứu/đặt hoặc quản lý; phiếu và nhật ký được giới hạn theo người sở hữu và phạm vi. API không tin ownerId do trình duyệt gửi. Không cho sửa tùy ý trạng thái phiếu.

Frontend muốn gọi cùng origin cần được phục vụ qua reverse proxy chung hoặc cấu hình hosting tĩnh khi tích hợp. Backend hiện không mở CORS tùy ý và chưa tự thay đường dẫn/đăng nhập demo của frontend.

## Kiểm tra

```powershell
dotnet restore Hpuni.sln --locked-mode
dotnet build Hpuni.sln --no-restore
dotnet test Hpuni.sln --no-restore --logger "trx;LogFilePrefix=backend"
dotnet list Hpuni.sln package --vulnerable --include-transitive
```

Kiểm tra tích hợp cần quyền Windows tạo database SQL Server. Mỗi fixture tạo `HpuniTests_<thời điểm>_<id>` riêng, tự migrate và seed dữ liệu tổng hợp. Không dùng tài khoản trường, không xóa database sau test. Có thể đặt `HPUNI_TEST_SQLSERVER` để dùng instance kiểm thử khác; tên database vẫn được test tự sinh. Không chỉ định instance vận hành thật.

TRX trong `tests/*/TestResults/` bị gitignore; báo cáo tổng hợp tại `docs/verification.md`. Các test chạy cả SQL Server locking, rowversion, quyền/phạm vi và luồng API HTTP qua WebApplicationFactory.

## Migrations

```powershell
$env:ASPNETCORE_ENVIRONMENT='Development'
dotnet ef migrations add TenThayDoi --project src/Hpuni.Infrastructure --startup-project src/Hpuni.Api
dotnet ef migrations script --idempotent --project src/Hpuni.Infrastructure --startup-project src/Hpuni.Api --output docs/schema.sql
dotnet run --project src/Hpuni.Api -- --migrate
```

Normal startup không tự migrate/reset. `docs/schema.sql` là script SQL Server đầy đủ; `docs/foundation.sql` là mốc nền tảng đầu tiên, giữ lại để truy vết. Không rollback bằng cách xóa dữ liệu người dùng.

## Trước vận hành

Chốt chính sách nghiệp vụ, cập nhật runtime/SQL Server servicing, cấu hình HTTPS, AllowedHosts và connection string, đường dẫn `DataProtection:Path` bền vững, quyền truy cập tối thiểu và quy trình backup/restore. Khi chạy sau reverse proxy, cấu hình forwarded headers chỉ tin proxy xác định; không bật tin mọi proxy. Chưa có deployment production hoặc diễn tập restore trong đợt này.

Chi tiết: [quyết định](docs/decisions.md), [phạm vi UC/API](docs/acceptance-map.md), [kiểm chứng](docs/verification.md).
