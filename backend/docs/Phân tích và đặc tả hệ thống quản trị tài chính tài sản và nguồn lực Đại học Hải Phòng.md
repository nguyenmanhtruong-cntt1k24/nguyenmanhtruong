**TRƯỜNG ĐẠI HỌC HẢI PHÒNG**

**KHOA CÔNG NGHỆ THÔNG TIN**

# PHÂN TÍCH VÀ ĐẶC TẢ USE CASE

**ĐỀ TÀI**

**NGHIÊN CỨU XÂY DỰNG HỆ THỐNG QUẢN TRỊ TÀI CHÍNH, TÀI SẢN VÀ KHAI THÁC HIỆU QUẢ NGUỒN LỰC ĐẠI HỌC DỰA TRÊN DỮ LIỆU TẠI TRƯỜNG ĐẠI HỌC HẢI PHÒNG**

GIẢNG VIÊN HƯỚNG DẪN: ........................................................

HỌ VÀ TÊN SINH VIÊN: ................................................................

MÃ SINH VIÊN: .............................................................................

LỚP: .............................................................................................

Hải Phòng, năm 2026

# Mục lục

1\. Giới thiệu

1.1 Bối cảnh và bài toán

1.2 Mục tiêu và giá trị

1.3 Phạm vi và nguyên tắc phân tích

2\. Đặc tả hệ thống

2.1 Tác nhân và trách nhiệm

2.2 Danh sách Use Case

2.3 Quy tắc nghiệp vụ và dữ liệu

2.4 Use Case tổng quát và phân rã

3\. Đặc tả chi tiết Use Case

3.1 Quản trị và truy cập hệ thống

3.2 Quản lý tài chính

3.3 Quản lý tài sản

3.4 Quản lý phòng học và phòng thí nghiệm

3.5 Mượn và sử dụng thiết bị

3.6 Báo cáo phân tích và hỗ trợ quyết định

4\. Biểu đồ hoạt động

5\. Biểu đồ tuần tự

6\. Kết luận và hướng phát triển

Danh sách chi tiết và mã Use Case nằm tại mục 2.2. Mỗi bảng đặc tả nêu mã biểu đồ hoạt động và tuần tự liên quan để thuận tiện đối chiếu.

# 1 Giới thiệu

## 1.1 Bối cảnh và bài toán

Đề tài hướng tới giải quyết khó khăn trong tổng hợp và đánh giá đồng thời hiệu quả sử dụng tài chính, cơ sở vật chất, trang thiết bị và các nguồn lực tại Trường Đại học Hải Phòng. Khi dữ liệu ngân sách, tài sản và sử dụng phòng, thiết bị được quản lý tách rời, việc đối chiếu chi phí với mức độ khai thác cần nhiều bước tổng hợp và khó hỗ trợ quyết định kịp thời.

Sản phẩm dự kiến là phần mềm quản trị và phân tích hiệu quả sử dụng nguồn lực đại học. Hệ thống kết nối dữ liệu nghiệp vụ từ lập dự toán, thu chi, hồ sơ tài sản, kiểm kê, bảo trì đến đăng ký và sử dụng thực tế phòng học, phòng thí nghiệm, thiết bị. Dữ liệu này tạo cơ sở cho báo cáo, phân tích, dự báo nhu cầu và đề xuất phân bổ.

## 1.2 Mục tiêu và giá trị

Quản lý tập trung các nghiệp vụ tài chính và nguồn lực trong cùng phạm vi phân quyền, bảo đảm khả năng truy vết từ số liệu tổng hợp đến dữ liệu phát sinh.

Ghi nhận việc sử dụng thực tế để đánh giá mức độ khai thác, thay vì chỉ căn cứ vào lịch đăng ký hoặc thời gian mượn.

Hỗ trợ người quản lý đối chiếu chi phí, thời gian khả dụng và mức sử dụng giữa các kỳ hoặc đơn vị.

Nghiên cứu mô hình dự báo nhu cầu và phương án phân bổ thỏa các ràng buộc về lịch, sức chứa, thiết bị và ngân sách.

## 1.3 Phạm vi và nguyên tắc phân tích

Phạm vi chức năng gồm sáu nhóm: quản trị hệ thống; tài chính; tài sản; phòng học và phòng thí nghiệm; mượn và sử dụng thiết bị; báo cáo, phân tích và hỗ trợ quyết định. Tài liệu đặc tả 48 Use Case ở cấp mục tiêu người dùng, kèm 1 biểu đồ tổng quát, 6 biểu đồ phân rã, 6 biểu đồ hoạt động và 12 biểu đồ tuần tự.

Các bước kiểm tra dùng chung và chức năng phụ trong quan hệ include được thể hiện trong luồng nghiệp vụ tương ứng. Quan hệ extend được mô tả bằng điều kiện phát sinh và nhánh xử lý. Đăng nhập và phân quyền là điều kiện chung của các nghiệp vụ được bảo vệ.

Nguồn lực trong phạm vi này tập trung vào tài chính, tài sản, phòng và thiết bị. Hệ thống chưa bao gồm nghiệp vụ nhân sự, tiền lương, quản lý đào tạo đầy đủ hoặc quy trình thanh lý tài sản. Các quy tắc cần khảo sát thêm được tách riêng tại mục 2.3; chưa coi là quy định đã ban hành của Nhà trường.

# 2 Đặc tả hệ thống

## 2.1 Tác nhân và trách nhiệm

| **Tác nhân** | **Trách nhiệm** |
| --- | --- |
| Ban Giám hiệu (Hiệu trưởng/người được ủy quyền đối với dự toán) | Quyết định phê duyệt, không phê duyệt hoặc trả lại dự toán kèm ý kiến; không trực tiếp sửa khoản mục, số tiền; xem báo cáo; thực hiện phân tích, dự báo và xem xét phương án phân bổ theo quyền. |
| Cán bộ kế hoạch - tài chính | Kiểm tra hồ sơ, căn cứ và nguồn kinh phí; ghi ý kiến thẩm định; trả bổ sung hoặc trình người có thẩm quyền phê duyệt. |
| Cán bộ kế toán | Lập, điều chỉnh, gửi dự toán thẩm định; ghi nhận thu chi; theo dõi ngân sách; khai thác báo cáo và chức năng phân tích theo quyền. |
| Cán bộ quản lý phòng và tài sản | Quản lý hồ sơ tài sản, kiểm kê, bảo trì, mượn trả và bàn giao thiết bị; cập nhật phòng, xét duyệt đăng ký, xác nhận nhật ký và phân tích mức khai thác phòng. |
| Cán bộ / Giảng viên | Tra cứu, đăng ký hoặc hủy theo điều kiện; sử dụng phòng và thiết bị; gửi nhật ký, xác nhận nhận và yêu cầu trả thiết bị. |
| Quản trị viên | Quản lý tài khoản, vai trò, quyền chức năng và phạm vi dữ liệu. |

“Người dùng hệ thống” là tên chung cho các tác nhân khi đăng nhập, đăng xuất và đổi mật khẩu. “Người dùng được phân quyền” trong nhóm báo cáo và hỗ trợ quyết định là vai trò khái quát, không phải một chức danh mới. Quyền xem, xuất, phân tích, dự báo và tạo đề xuất được cấp riêng.

Phòng Kế hoạch - Tài chính là đơn vị tổ chức; “Cán bộ kế hoạch - tài chính” là vai trò sử dụng hệ thống. Cán bộ kế toán có thể thuộc cùng phòng nhưng quyền lập/sửa và quyền thẩm định được phân biệt. Không mặc định mọi thành viên Ban Giám hiệu đều có quyền phê duyệt. Thẩm quyền cụ thể, việc kiêm nhiệm và phạm vi ủy quyền cần xác nhận theo quy định nội bộ của Trường.

### 2.1.1 Phòng Kế hoạch – Tài chính trong hệ thống

Trong phạm vi đề tài, Phòng Kế hoạch – Tài chính (KHTC) là đơn vị phụ trách nghiệp vụ kế hoạch tài chính, tổ chức lập và tổng hợp dự toán, thẩm định hồ sơ kinh phí, quản lý thu – chi và theo dõi thực hiện ngân sách. Phòng tham mưu, trình người có thẩm quyền quyết định đối với dự toán nội bộ. Nội dung dưới đây là phân công đề xuất cho hệ thống, cần xác nhận theo quy định nội bộ của Trường trước khi triển khai chính thức.

| **Nhóm chức năng** | **Nội dung trong phạm vi đề tài** | **Use Case liên quan** |
| --- | --- | --- |
| Lập và tổng hợp dự toán | Lập hồ sơ theo kỳ, đơn vị và khoản mục; tổng hợp nhu cầu kinh phí trong phạm vi dữ liệu được cấp. | UC-TC01; UC-PT01 |
| Điều chỉnh và gửi thẩm định | Sửa dự toán Nháp hoặc Trả bổ sung; gửi tới người có quyền thẩm định KHTC. | UC-TC02; UC-TC03 |
| Thẩm định và trình phê duyệt | Kiểm tra hồ sơ, căn cứ tính toán, định mức và nguồn kinh phí; ghi ý kiến, trình duyệt hoặc trả bổ sung. | UC-TC09 |
| Quản lý thu – chi | Ghi nhận khoản thu, khoản chi và chứng từ; lưu yêu cầu chi chờ xử lý khi chưa đủ điều kiện thực hiện. | UC-TC05; UC-TC06; UC-TC07 |
| Theo dõi ngân sách | Theo dõi kinh phí được duyệt, đã sử dụng và còn khả dụng; kiểm soát khoản chi vượt ngân sách. | UC-TC08; UC-TC06; UC-TC07 |
| Báo cáo tài chính | Tổng hợp số liệu, khai thác và xuất báo cáo theo quyền chức năng, phạm vi dữ liệu được cấp. | Nhóm UC-PT theo quyền |

### 2.1.2 Vai trò và phân quyền trong Phòng Kế hoạch - Tài chính

Phòng Kế hoạch - Tài chính được thể hiện bằng hai vai trò nghiệp vụ, không sử dụng một tài khoản chung cho cả phòng. Mỗi người sử dụng tài khoản cá nhân để lưu vết thao tác.

| **Vai trò** | **Quyền nghiệp vụ đề xuất** | **Giới hạn trách nhiệm** |
| --- | --- | --- |
| Cán bộ kế toán thuộc Phòng Kế hoạch - Tài chính | Lập, sửa và gửi dự toán; ghi nhận thu – chi; theo dõi ngân sách; xem hoặc xuất báo cáo khi được cấp quyền. | Chỉ sửa dự toán Nháp hoặc Trả bổ sung; việc sửa không tự làm tăng ngân sách được duyệt. |
| Cán bộ kế hoạch - tài chính | Xem hồ sơ được giao, ghi ý kiến thẩm định, trả bổ sung hoặc trình người có thẩm quyền. | Không tự sửa khoản mục, số tiền trong bước thẩm định; không tự cấp ngân sách. |
| Hiệu trưởng/người được ủy quyền thuộc vai trò lãnh đạo | Xem hồ sơ và kết quả thẩm định; phê duyệt, không phê duyệt hoặc trả bổ sung kèm ý kiến. | Quyết định trong phạm vi thẩm quyền; không trực tiếp sửa dự toán. |

Quản trị viên cấp quyền cho từng tài khoản theo vai trò và phạm vi đơn vị; quyền thẩm định cần được cấp rõ ràng, không tự phát sinh từ quyền lập dự toán. Việc một người được kiêm nhiệm lập và thẩm định, điều kiện tự thẩm định hồ sơ do mình lập và phạm vi ủy quyền phê duyệt là các quy tắc cần khảo sát, không mặc định được phép.

### 2.1.3 Luồng phối hợp giữa KHTC và người phê duyệt

1. Cán bộ kế toán lập, lưu Nháp và gửi dự toán sang Chờ thẩm định KHTC.
2. Cán bộ kế hoạch - tài chính kiểm tra hồ sơ. Nếu cần bổ sung, ghi lý do và trả người lập; nếu đạt, ghi ý kiến và chuyển sang Chờ phê duyệt.
3. Hiệu trưởng/người được ủy quyền xem hồ sơ cùng ý kiến KHTC và ra quyết định.
4. Khi nhận yêu cầu bổ sung, KHTC phối hợp người lập chỉnh sửa. Phiên bản mới phải được thẩm định lại trước khi trình duyệt; vẫn giữ lịch sử phiên bản và ý kiến trước đó.
5. Chỉ quyết định phê duyệt hợp lệ mới hình thành ngân sách tương ứng. Thẩm định đạt hoặc gửi trình chưa làm phát sinh ngân sách được duyệt.

Trên web demo, mục “Lập và theo dõi dự toán” phục vụ vai trò kế toán; mục “KHTC · Thẩm định” phục vụ vai trò thẩm định; màn lãnh đạo phục vụ quyết định phê duyệt. Đây là phân tách thao tác trong demo, chưa phải cơ chế xác thực và phân quyền KHTC tại API/SQL.

## 2.2 Danh sách Use Case

Mã UC gồm tiền tố nhóm và số thứ tự trong nhóm. Mỗi mã được sử dụng nhất quán trong danh sách, bảng đặc tả và các phần đối chiếu. Các mã này chuẩn hóa tài liệu, không thay đổi nội dung nghiệp vụ của sơ đồ.

## 2.2.1 Quản trị và truy cập hệ thống

| **Mã** | **Tên Use Case** | **Tác nhân** |
| --- | --- | --- |
| UC-HT01 | Đăng nhập | Người dùng hệ thống |
| UC-HT02 | Đăng xuất | Người dùng hệ thống |
| UC-HT03 | Đổi mật khẩu | Người dùng hệ thống |
| UC-HT04 | Tạo tài khoản | Quản trị viên |
| UC-HT05 | Cập nhật tài khoản | Quản trị viên |
| UC-HT06 | Khóa và mở khóa tài khoản | Quản trị viên |
| UC-HT07 | Quản lý vai trò | Quản trị viên |
| UC-HT08 | Phân quyền chức năng và phạm vi dữ liệu | Quản trị viên |

## 2.2.2 Quản lý tài chính

| **Mã** | **Tên Use Case** | **Tác nhân** |
| --- | --- | --- |
| UC-TC01 | Lập dự toán | Cán bộ kế toán |
| UC-TC02 | Điều chỉnh dự toán | Cán bộ kế toán |
| UC-TC03 | Gửi dự toán thẩm định | Cán bộ kế toán |
| UC-TC04 | Xét duyệt dự toán | Hiệu trưởng/người được ủy quyền |
| UC-TC09 | Thẩm định và trình duyệt dự toán | Cán bộ kế hoạch - tài chính |
| UC-TC05 | Ghi nhận khoản thu | Cán bộ kế toán |
| UC-TC06 | Ghi nhận khoản chi | Cán bộ kế toán |
| UC-TC07 | Lưu yêu cầu chi chờ xử lý | Cán bộ kế toán |
| UC-TC08 | Theo dõi ngân sách | Cán bộ kế toán; Ban Giám hiệu |

## 2.2.3 Quản lý tài sản

| **Mã** | **Tên Use Case** | **Tác nhân** |
| --- | --- | --- |
| UC-TS01 | Thêm tài sản | Cán bộ quản lý phòng và tài sản |
| UC-TS02 | Cập nhật hồ sơ tài sản | Cán bộ quản lý phòng và tài sản |
| UC-TS03 | Tra cứu tài sản | Cán bộ quản lý phòng và tài sản |
| UC-TS04 | Lập đợt kiểm kê | Cán bộ quản lý phòng và tài sản |
| UC-TS05 | Ghi nhận và hoàn tất kiểm kê | Cán bộ quản lý phòng và tài sản |
| UC-TS06 | Theo dõi tình trạng tài sản | Cán bộ quản lý phòng và tài sản |
| UC-TS07 | Lập kế hoạch bảo trì | Cán bộ quản lý phòng và tài sản |
| UC-TS08 | Ghi nhận kết quả và chi phí bảo trì | Cán bộ quản lý phòng và tài sản |

## 2.2.4 Quản lý phòng học và phòng thí nghiệm

| **Mã** | **Tên Use Case** | **Tác nhân** |
| --- | --- | --- |
| UC-PH01 | Tra cứu phòng và lịch sử dụng | Cán bộ / Giảng viên; Cán bộ quản lý phòng và tài sản |
| UC-PH02 | Đăng ký sử dụng phòng | Cán bộ / Giảng viên |
| UC-PH03 | Hủy đăng ký phòng | Cán bộ / Giảng viên |
| UC-PH04 | Cập nhật thông tin phòng | Cán bộ quản lý phòng và tài sản |
| UC-PH05 | Xét duyệt đăng ký phòng | Cán bộ quản lý phòng và tài sản |
| UC-PH06 | Gửi nhật ký sử dụng phòng thực tế | Cán bộ / Giảng viên |
| UC-PH07 | Kiểm tra và xác nhận nhật ký phòng | Cán bộ quản lý phòng và tài sản |

## 2.2.5 Mượn và sử dụng thiết bị

| **Mã** | **Tên Use Case** | **Tác nhân** |
| --- | --- | --- |
| UC-TB01 | Tra cứu thiết bị | Cán bộ / Giảng viên |
| UC-TB02 | Đăng ký mượn thiết bị | Cán bộ / Giảng viên |
| UC-TB03 | Hủy yêu cầu mượn | Cán bộ / Giảng viên |
| UC-TB04 | Xét duyệt yêu cầu mượn | Cán bộ quản lý phòng và tài sản |
| UC-TB05 | Ghi nhận bàn giao thiết bị | Cán bộ quản lý phòng và tài sản |
| UC-TB06 | Xác nhận nhận thiết bị | Cán bộ / Giảng viên |
| UC-TB07 | Ghi nhận sử dụng thiết bị thực tế | Cán bộ / Giảng viên |
| UC-TB08 | Gửi yêu cầu trả thiết bị | Cán bộ / Giảng viên |
| UC-TB09 | Tiếp nhận trả thiết bị | Cán bộ quản lý phòng và tài sản |
| UC-TB10 | Theo dõi sử dụng và quá hạn trả | Cán bộ quản lý phòng và tài sản |

## 2.2.6 Báo cáo phân tích và hỗ trợ quyết định

| **Mã** | **Tên Use Case** | **Tác nhân** |
| --- | --- | --- |
| UC-PT01 | Xem báo cáo tổng hợp | Ban Giám hiệu; Cán bộ kế toán; Cán bộ quản lý phòng và tài sản |
| UC-PT02 | Xuất báo cáo | Người dùng được cấp quyền xuất báo cáo |
| UC-PT03 | Phân tích hiệu quả sử dụng nguồn lực | Người dùng được cấp quyền phân tích |
| UC-PT04 | Dự báo nhu cầu nguồn lực | Người dùng được cấp quyền dự báo |
| UC-PT05 | Tạo phương án đề xuất phân bổ | Người dùng được cấp quyền đề xuất phân bổ |
| UC-PT06 | Xem và so sánh phương án phân bổ | Người dùng được cấp quyền đề xuất phân bổ |

## 2.3 Quy tắc nghiệp vụ và dữ liệu

### 2.3.1 Quy tắc thể hiện trong các biểu đồ

| **Mã** | **Quy tắc** |
| --- | --- |
| R01 | Kiểm tra cả quyền chức năng và phạm vi dữ liệu; không chỉ ẩn nút trên giao diện. |
| R02 | Phê duyệt và từ chối là hai kết quả thay thế của việc xét duyệt. Kiểm tra trạng thái hiện tại trước khi cập nhật. |
| R03 | Khoản chi phải kiểm tra ngân sách khả dụng trong cùng giao dịch với ghi nhận chi. Yêu cầu chờ xử lý chưa là khoản chi thực hiện. |
| R04 | Kiểm tra lại lịch phòng hoặc thiết bị khả dụng khi duyệt; lưu lịch hoặc giữ thiết bị đồng thời với kết quả duyệt. |
| R05 | Lịch đặt và thời gian mượn khác với thời gian sử dụng thực tế. Nhật ký phòng đã xác nhận là căn cứ phân tích sử dụng. |
| R06 | Gửi yêu cầu trả chưa làm thiết bị khả dụng. Chỉ cập nhật sau khi người quản lý xác nhận thực nhận; thiết bị hỏng chuyển Chờ xử lý. |
| R07 | Cho phép trả từng phần; chỉ hoàn tất phiếu khi đã xử lý đủ số lượng phải trả. |
| R08 | Chi phí bảo trì liên kết với chứng từ chi, tránh ghi nhận hai lần cùng một khoản chi. |
| R09 | Báo cáo, phân tích, dự báo và đề xuất phân bổ là những chức năng riêng. Xem báo cáo không bắt buộc chạy các chức năng còn lại. |
| R10 | Chỉ tiêu thiếu dữ liệu hoặc có mẫu số bằng 0 phải được thể hiện phù hợp; không thay kết quả không xác định bằng 0. |
| R11 | Dự báo phải kiểm tra dữ liệu lịch sử và lưu thông tin mô hình, tham số, thời điểm chạy cùng sai số đánh giá. |
| R12 | Lưu phương án Đề xuất không tự thay đổi ngân sách, lịch phòng hoặc cấp thiết bị. Không mặc định phương án tìm được là tối ưu toàn cục. |

### 2.3.2 Dữ liệu phục vụ phân tích và nghiên cứu

| **Nhóm dữ liệu** | **Nội dung cần có** |
| --- | --- |
| Tài chính | Kỳ, đơn vị, khoản mục, dự toán được duyệt, khoản thu chi thực hiện và chứng từ liên quan. |
| Tài sản | Mã, loại, đơn vị quản lý, vị trí, nguyên giá, tình trạng, kết quả kiểm kê và lịch sử bảo trì. |
| Phòng | Loại phòng, sức chứa, lịch khả dụng, lịch đã duyệt và nhật ký sử dụng thực tế đã xác nhận. |
| Thiết bị | Danh mục tài sản thiết bị, số lượng, lịch giữ, bàn giao, tiếp nhận trả, thời gian sử dụng thực tế và hư hỏng. |
| Nhu cầu và ràng buộc | Yêu cầu sử dụng theo kỳ, nhu cầu chưa đáp ứng, giới hạn ngân sách, sức chứa, thiết bị và lịch bảo trì. |

Thiết bị cần được liên kết với hồ sơ tài sản để tránh tính trùng số lượng, giá trị và chi phí. Dữ liệu lịch sử nhu cầu phải phân biệt nhu cầu đăng ký, nhu cầu được đáp ứng và mức sử dụng thực tế; số lượt sử dụng thấp có thể do thiếu nguồn lực, không nhất thiết là nhu cầu thấp.

### 2.3.3 Các chỉ tiêu phân tích

| **Chỉ tiêu** | **Cách xác định và điều kiện** |
| --- | --- |
| Tỷ lệ sử dụng thời gian | Thời gian sử dụng thực tế hợp lệ / thời gian khả dụng × 100%. Quy tắc loại trừ thời gian bảo trì và đóng cửa phải nhất quán. |
| Chi phí mỗi giờ sử dụng | Chi phí liên quan trong kỳ / tổng giờ sử dụng thực tế trong cùng kỳ. Chỉ tính khi thời gian sử dụng lớn hơn 0. |
| Mức sử dụng ngân sách | Đối chiếu khoản chi thực hiện với ngân sách được duyệt trong cùng kỳ và phạm vi; tách riêng yêu cầu chờ xử lý. |
| Mức đáp ứng nhu cầu | Đối chiếu nhu cầu được đáp ứng với nhu cầu hợp lệ theo cùng đơn vị đo. Chỉ tiêu cụ thể cần chốt khi khảo sát. |

Phân tích phải lưu kỳ, đơn vị, bộ lọc và quy tắc tính. Với dữ liệu chưa đầy đủ, hệ thống cần nêu giới hạn thay vì đưa ra nhận xét chắc chắn về hiệu quả của đơn vị hoặc nguồn lực.

### 2.3.4 Quy tắc đề xuất cần xác nhận khi khảo sát

Các nội dung dưới đây bổ sung mức chi tiết để đặc tả có thể triển khai. Đây là đề xuất phân tích cần thống nhất với đơn vị sử dụng, không phải mô tả quy định thực tế đã được xác minh.

| **Nội dung** | **Đề xuất và điểm cần chốt** |
| --- | --- |
| Tài khoản và mật khẩu | Tài khoản cấp nội bộ; tên đăng nhập duy nhất; thống nhất điều kiện mật khẩu, thời hạn phiên và cách xử lý phiên đang mở khi khóa tài khoản. |
| Điều chỉnh dự toán | Không sửa trực tiếp dự toán đã duyệt làm thay đổi ngân sách. Cần chốt quy trình điều chỉnh và quyền trình duyệt lại. |
| Hủy đăng ký | Xác định trạng thái, thời điểm được hủy và người có quyền hủy. Khi đã nhận thiết bị, chuyển sang quy trình trả thay vì hủy yêu cầu. |
| Hồ sơ và dữ liệu đầu vào | Chốt trường bắt buộc, loại tài sản, quy tắc mã duy nhất, đơn vị tiền tệ và giới hạn dữ liệu. Các trường trong đặc tả là bộ dữ liệu đề xuất tối thiểu. |
| Nhật ký thiết bị | Chốt cơ chế kiểm chứng hoặc xác nhận nhật ký thiết bị trước khi dùng để đánh giá hiệu suất; biểu đồ hiện mô tả kiểm tra và lưu nhật ký. |
| Phạm vi quản lý | Chốt quyền theo đơn vị, cách ủy quyền, người được xem báo cáo tổng hợp và người được tạo đề xuất phân bổ. |
| Thước đo và mô hình | Chốt định nghĩa thời gian khả dụng, cách phân bổ chi phí, độ dài dữ liệu tối thiểu, mô hình dự báo và mục tiêu tối ưu. |
| Xử lý đồng thời và lỗi | Kiểm tra trạng thái, phiên bản và tính toàn vẹn khi lưu. Chi tiết kỹ thuật, nhật ký thay đổi và cơ chế phục hồi cần được thiết kế tiếp. |

Quy trình dự toán bổ sung bước thẩm định KHTC trước khi trình người có thẩm quyền quyết định; AD01 và SD02 cần thể hiện bước này, cùng nhánh trả bổ sung. Hai cách thực hiện được thống nhất trong các Use Case lập và gửi dự toán. Phần điều kiện hủy và quản trị tài khoản được đặc tả từ sơ đồ phân rã vì chưa có biểu đồ hoạt động, tuần tự riêng.

## 2.4.1 Use Case tổng quát

Các tác nhân tương tác với các chức năng ở cấp tổng quát; bổ sung vai trò thẩm định KHTC theo mục 2.1. Tất cả tác nhân có quyền đăng nhập và đăng xuất; quyền nghiệp vụ và dữ liệu được giới hạn theo phân quyền.

Hình 2.1 Use Case tổng quát

## 2.4.2 Quản lý tài chính

Các chức năng chính và bước xử lý dùng chung được liên kết bằng include; các hành vi phát sinh có điều kiện được mô tả bằng extend. Danh sách đặc tả tương ứng nằm trong nhóm TC.

Hình 2.2 Use Case phân rã quản lý tài chính

## 2.4.3 Quản lý tài sản

Các chức năng chính và bước xử lý dùng chung được liên kết bằng include; các hành vi phát sinh có điều kiện được mô tả bằng extend. Danh sách đặc tả tương ứng nằm trong nhóm TS.

Hình 2.3 Use Case phân rã quản lý tài sản

## 2.4.4 Quản lý phòng học và phòng thí nghiệm

Các chức năng chính và bước xử lý dùng chung được liên kết bằng include; các hành vi phát sinh có điều kiện được mô tả bằng extend. Danh sách đặc tả tương ứng nằm trong nhóm PH.

Hình 2.4 Use Case phân rã quản lý phòng học và phòng thí nghiệm

## 2.4.5 Quản lý mượn và sử dụng thiết bị

Các chức năng chính và bước xử lý dùng chung được liên kết bằng include; các hành vi phát sinh có điều kiện được mô tả bằng extend. Danh sách đặc tả tương ứng nằm trong nhóm TB.

Hình 2.5 Use Case phân rã quản lý mượn và sử dụng thiết bị

## 2.4.6 Báo cáo phân tích và hỗ trợ quyết định

Các chức năng chính và bước xử lý dùng chung được liên kết bằng include; các hành vi phát sinh có điều kiện được mô tả bằng extend. Danh sách đặc tả tương ứng nằm trong nhóm PT.

Hình 2.6 Use Case phân rã báo cáo phân tích và hỗ trợ quyết định

## 2.4.7 Quản trị và truy cập hệ thống

Các chức năng chính và bước xử lý dùng chung được liên kết bằng include; các hành vi phát sinh có điều kiện được mô tả bằng extend. Danh sách đặc tả tương ứng nằm trong nhóm HT.

Hình 2.7 Use Case phân rã quản trị và truy cập hệ thống

# 3 Đặc tả chi tiết Use Case

## 3.1 Quản trị và truy cập hệ thống

### UC-HT01 Đăng nhập

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Người dùng hệ thống |
| Mục tiêu | Đăng nhập trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Tài khoản đã được cấp và còn hoạt động. |
| Luồng sự kiện chính | 1\. Người dùng mở giao diện đăng nhập và nhập tên đăng nhập, mật khẩu.  <br>2\. Hệ thống tìm tài khoản và kiểm tra trạng thái.  <br>3\. Hệ thống đối chiếu mật khẩu với mã băm đã lưu.  <br>4\. Hệ thống đọc vai trò, quyền chức năng và phạm vi dữ liệu.  <br>5\. Hệ thống tạo phiên và hiển thị trang chính theo quyền. |
| Luồng thay thế và ngoại lệ | E1. Tài khoản không tồn tại hoặc mật khẩu sai: báo thông tin đăng nhập không hợp lệ.  <br>E2. Tài khoản bị khóa: từ chối tạo phiên. |
| Điều kiện sau | Phiên đăng nhập hợp lệ được tạo; quyền truy cập được áp dụng. |
| Biểu đồ liên quan | Use Case phân rã nhóm HT. SD01. |

_Quy tắc triển khai chi tiết áp dụng theo mục 2.3.4 và cần được xác nhận trong khảo sát nghiệp vụ._

### UC-HT02 Đăng xuất

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Người dùng hệ thống |
| Mục tiêu | Đăng xuất trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Người dùng đang có phiên đăng nhập. |
| Luồng sự kiện chính | 1\. Người dùng chọn đăng xuất.  <br>2\. Hệ thống xác định phiên hiện tại.  <br>3\. Hệ thống kết thúc phiên đăng nhập.  <br>4\. Hệ thống chuyển về màn hình đăng nhập. |
| Luồng thay thế và ngoại lệ | E1. Phiên đã hết hạn: chuyển về màn hình đăng nhập.  <br>E2. Yêu cầu truy cập lại chức năng bảo vệ sau đăng xuất: yêu cầu xác thực lại. |
| Điều kiện sau | Phiên hiện tại không còn được dùng để truy cập hệ thống. |
| Biểu đồ liên quan | Use Case phân rã nhóm HT. Không có biểu đồ riêng. |

_Quy tắc triển khai chi tiết áp dụng theo mục 2.3.4 và cần được xác nhận trong khảo sát nghiệp vụ._

### UC-HT03 Đổi mật khẩu

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Người dùng hệ thống |
| Mục tiêu | Đổi mật khẩu trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Người dùng đã đăng nhập và biết mật khẩu hiện tại. |
| Luồng sự kiện chính | 1\. Người dùng nhập mật khẩu hiện tại và mật khẩu mới.  <br>2\. Hệ thống kiểm tra mật khẩu hiện tại.  <br>3\. Hệ thống kiểm tra điều kiện mật khẩu mới theo chính sách cấu hình.  <br>4\. Hệ thống lưu mã băm mật khẩu mới.  <br>5\. Hệ thống thông báo đổi mật khẩu thành công. |
| Luồng thay thế và ngoại lệ | E1. Mật khẩu hiện tại sai: không cập nhật.  <br>E2. Mật khẩu mới không đáp ứng chính sách: nêu điều kiện cần sửa. |
| Điều kiện sau | Mật khẩu được thay đổi; không lưu mật khẩu dưới dạng văn bản thuần. |
| Biểu đồ liên quan | Use Case phân rã nhóm HT. Không có biểu đồ riêng. |

_Quy tắc triển khai chi tiết áp dụng theo mục 2.3.4 và cần được xác nhận trong khảo sát nghiệp vụ._

### UC-HT04 Tạo tài khoản

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Quản trị viên |
| Mục tiêu | Tạo tài khoản trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Quản trị viên có quyền tạo tài khoản trong phạm vi được quản lý. |
| Luồng sự kiện chính | 1\. Quản trị viên chọn tạo tài khoản.  <br>2\. Nhập thông tin định danh và đơn vị của người dùng.  <br>3\. Hệ thống kiểm tra dữ liệu bắt buộc và tính duy nhất của tên đăng nhập.  <br>4\. Quản trị viên chọn vai trò được phép cấp.  <br>5\. Hệ thống lưu tài khoản và hiển thị kết quả. |
| Luồng thay thế và ngoại lệ | E1. Tên đăng nhập trùng hoặc dữ liệu thiếu: không lưu và yêu cầu sửa.  <br>E2. Vai trò vượt quyền được cấp: từ chối thao tác. |
| Điều kiện sau | Tài khoản mới được tạo với vai trò và phạm vi hợp lệ. |
| Biểu đồ liên quan | Use Case phân rã nhóm HT. Không có biểu đồ riêng. |

_Quy tắc triển khai chi tiết áp dụng theo mục 2.3.4 và cần được xác nhận trong khảo sát nghiệp vụ._

### UC-HT05 Cập nhật tài khoản

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Quản trị viên |
| Mục tiêu | Cập nhật tài khoản trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Tài khoản tồn tại và thuộc phạm vi quản lý. |
| Luồng sự kiện chính | 1\. Quản trị viên tìm tài khoản.  <br>2\. Hệ thống hiển thị thông tin được phép sửa.  <br>3\. Quản trị viên cập nhật thông tin người dùng.  <br>4\. Hệ thống kiểm tra tính hợp lệ và quyền cập nhật.  <br>5\. Hệ thống lưu và hiển thị thông tin mới. |
| Luồng thay thế và ngoại lệ | E1. Tài khoản không tồn tại: thông báo.  <br>E2. Thông tin không hợp lệ hoặc vượt phạm vi quản lý: không cập nhật. |
| Điều kiện sau | Thông tin tài khoản được cập nhật; quyền chỉ thay đổi qua nghiệp vụ phân quyền. |
| Biểu đồ liên quan | Use Case phân rã nhóm HT. Không có biểu đồ riêng. |

_Quy tắc triển khai chi tiết áp dụng theo mục 2.3.4 và cần được xác nhận trong khảo sát nghiệp vụ._

### UC-HT06 Khóa và mở khóa tài khoản

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Quản trị viên |
| Mục tiêu | Khóa và mở khóa tài khoản trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Tài khoản tồn tại; quản trị viên có quyền thay đổi trạng thái. |
| Luồng sự kiện chính | 1\. Quản trị viên chọn tài khoản.  <br>2\. Hệ thống hiển thị trạng thái hiện tại.  <br>3\. Quản trị viên chọn khóa hoặc mở khóa.  <br>4\. Hệ thống kiểm tra quyền và xác nhận thao tác.  <br>5\. Hệ thống lưu trạng thái mới. |
| Luồng thay thế và ngoại lệ | E1. Không đủ quyền: từ chối.  <br>E2. Tài khoản đã ở trạng thái yêu cầu: thông báo và không tạo thay đổi lặp. |
| Điều kiện sau | Trạng thái tài khoản được cập nhật; tài khoản bị khóa không được đăng nhập. |
| Biểu đồ liên quan | Use Case phân rã nhóm HT. Không có biểu đồ riêng. |

_Quy tắc triển khai chi tiết áp dụng theo mục 2.3.4 và cần được xác nhận trong khảo sát nghiệp vụ._

### UC-HT07 Quản lý vai trò

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Quản trị viên |
| Mục tiêu | Quản lý vai trò trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Quản trị viên được phép quản lý danh mục vai trò. |
| Luồng sự kiện chính | 1\. Quản trị viên mở danh sách vai trò.  <br>2\. Chọn tạo mới hoặc cập nhật vai trò.  <br>3\. Nhập tên, mô tả và phạm vi áp dụng.  <br>4\. Hệ thống kiểm tra thông tin và quyền quản lý.  <br>5\. Lưu danh mục vai trò. |
| Luồng thay thế và ngoại lệ | E1. Tên vai trò không hợp lệ hoặc trùng: yêu cầu sửa.  <br>E2. Thay đổi không được phép theo quyền quản trị: từ chối. |
| Điều kiện sau | Vai trò hợp lệ sẵn sàng được sử dụng khi phân quyền. |
| Biểu đồ liên quan | Use Case phân rã nhóm HT. Không có biểu đồ riêng. |

_Quy tắc triển khai chi tiết áp dụng theo mục 2.3.4 và cần được xác nhận trong khảo sát nghiệp vụ._

### UC-HT08 Phân quyền chức năng và phạm vi dữ liệu

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Quản trị viên |
| Mục tiêu | Phân quyền chức năng và phạm vi dữ liệu trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Đã có tài khoản, vai trò và danh mục quyền. |
| Luồng sự kiện chính | 1\. Quản trị viên chọn tài khoản hoặc vai trò.  <br>2\. Chọn các chức năng và đơn vị dữ liệu được phép truy cập.  <br>3\. Hệ thống kiểm tra quyền mà quản trị viên được phép cấp.  <br>4\. Quản trị viên xác nhận cấu hình.  <br>5\. Hệ thống lưu và áp dụng cấu hình quyền. |
| Luồng thay thế và ngoại lệ | E1. Cấp quyền vượt thẩm quyền: từ chối.  <br>E2. Phạm vi dữ liệu không hợp lệ: yêu cầu điều chỉnh. |
| Điều kiện sau | Quyền chức năng và phạm vi dữ liệu được lưu nhất quán. |
| Biểu đồ liên quan | Use Case phân rã nhóm HT. Không có biểu đồ riêng. |

_Quy tắc triển khai chi tiết áp dụng theo mục 2.3.4 và cần được xác nhận trong khảo sát nghiệp vụ._

## 3.2 Quản lý tài chính

### UC-TC01 Lập dự toán

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ kế toán |
| Mục tiêu | Lập dự toán trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Người dùng có quyền lập dự toán; kỳ và đơn vị lập dự toán được xác định. |
| Luồng sự kiện chính | 1\. Kế toán chọn lập dự toán.  <br>2\. Nhập kỳ dự toán, các khoản mục và số tiền.  <br>3\. Hệ thống kiểm tra thông tin bắt buộc và giá trị các khoản mục.  <br>4\. Kế toán sửa dữ liệu nếu cần và xác nhận lưu.  <br>5\. Hệ thống lưu dự toán ở trạng thái Nháp. |
| Luồng thay thế và ngoại lệ | E1. Dữ liệu thiếu hoặc số tiền không hợp lệ: yêu cầu sửa.  <br>E2. Không có quyền với đơn vị hoặc kỳ được chọn: từ chối. |
| Điều kiện sau | Dự toán Nháp được lưu, chưa làm phát sinh ngân sách được duyệt. |
| Biểu đồ liên quan | Use Case phân rã nhóm TC. AD01; SD02. |

### UC-TC02 Điều chỉnh dự toán

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ kế toán |
| Mục tiêu | Điều chỉnh dự toán trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có quyền sửa; dự toán Nháp hoặc Trả bổ sung. Không sửa trực tiếp khi đang thẩm định, chờ phê duyệt hoặc đã duyệt. |
| Luồng sự kiện chính | 1\. Kế toán mở dự toán.  <br>2\. Hệ thống kiểm tra quyền và trạng thái.  <br>3\. Kế toán sửa khoản mục hoặc số tiền.  <br>4\. Hệ thống kiểm tra tính hợp lệ và phiên bản hiện tại.  <br>5\. Hệ thống lưu nội dung điều chỉnh. |
| Luồng thay thế và ngoại lệ | E1. Dữ liệu không hợp lệ: không lưu.  <br>E2. Dự toán đã duyệt hoặc đã thay đổi: không sửa trực tiếp, yêu cầu xử lý theo quy tắc điều chỉnh hoặc tải lại. |
| Điều kiện sau | Lưu phiên bản mới ở trạng thái Nháp; giữ lịch sử nhưng kết quả thẩm định cũ không còn hiệu lực cho phiên bản mới. Phải gửi lại KHTC trước khi trình phê duyệt; không tự thay đổi ngân sách đã duyệt. |
| Biểu đồ liên quan | Use Case phân rã nhóm TC. AD01; SD02. |

_Quy tắc triển khai chi tiết áp dụng theo mục 2.3.4 và cần được xác nhận trong khảo sát nghiệp vụ._

### UC-TC03 Gửi dự toán thẩm định

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ kế toán |
| Mục tiêu | Chuyển dự toán hợp lệ tới Phòng Kế hoạch - Tài chính để thẩm định. |
| Điều kiện trước | Có quyền gửi; dự toán Nháp, đủ dữ liệu và thuộc phạm vi phụ trách. |
| Luồng sự kiện chính | 1. Kế toán chọn gửi thẩm định. <br>2. Hệ thống kiểm tra quyền, nội dung và phiên bản. <br>3. Chuyển sang Chờ thẩm định KHTC, ghi người gửi và thời điểm. <br>4. Hiển thị dự toán trong danh sách thẩm định KHTC. |
| Luồng thay thế và ngoại lệ | Thiếu dữ liệu, sai phiên bản hoặc đã gửi: không chuyển trạng thái, thông báo nguyên nhân. |
| Điều kiện sau | Chờ thẩm định KHTC; chưa hình thành ngân sách được duyệt. |
| Biểu đồ liên quan | Nhóm TC; AD01; SD02, bổ sung bước UC-TC09. |

### UC-TC09 Thẩm định và trình duyệt dự toán

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ kế hoạch - tài chính |
| Mục tiêu | Kiểm tra tính đầy đủ, căn cứ chi, định mức và khả năng cân đối nguồn trước khi trình phê duyệt. |
| Điều kiện trước | Dự toán Chờ thẩm định KHTC; người dùng có quyền thẩm định trong phạm vi dữ liệu. |
| Luồng sự kiện chính | 1. Mở dự toán và hồ sơ. <br>2. Kiểm tra nội dung, căn cứ, số tiền và nguồn kinh phí. <br>3. Ghi ý kiến thẩm định và chọn trình duyệt. <br>4. Hệ thống kiểm tra quyền, trạng thái và phiên bản; lưu ý kiến gắn với phiên bản. <br>5. Chuyển sang Chờ phê duyệt và chuyển hồ sơ tới người có thẩm quyền. |
| Luồng thay thế và ngoại lệ | E1. Cần bổ sung: bắt buộc ghi lý do, trả về Trả bổ sung cho người lập; không tự sửa số tiền. <br>E2. Thiếu ý kiến hoặc phiên bản thay đổi: không trình, yêu cầu kiểm tra lại. |
| Điều kiện sau | Chờ phê duyệt hoặc Trả bổ sung; lưu người thẩm định, thời điểm và ý kiến; chưa tạo ngân sách được duyệt. |
| Biểu đồ liên quan | Bổ sung actor KHTC và UC-TC09 vào nhóm TC; thêm bước thẩm định tại AD01, SD02. |

### UC-TC04 Xét duyệt dự toán

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Hiệu trưởng/người được ủy quyền |
| Mục tiêu | Ra quyết định đối với dự toán đã được KHTC thẩm định và trình. |
| Điều kiện trước | Có quyền phê duyệt; dự toán Chờ phê duyệt, có ý kiến thẩm định hợp lệ cho phiên bản hiện tại. |
| Luồng sự kiện chính | 1. Mở dự toán, hồ sơ và ý kiến KHTC. <br>2. Chọn phê duyệt. <br>3. Hệ thống kiểm tra thẩm quyền, trạng thái, phiên bản và kết quả thẩm định. <br>4. Lưu quyết định và ngân sách được duyệt trong cùng giao dịch; ngăn duyệt trùng. <br>5. Thông báo kết quả. |
| Luồng thay thế và ngoại lệ | E1. Không phê duyệt: bắt buộc ghi lý do, lưu Từ chối; không tạo ngân sách. <br>E2. Trả bổ sung: bắt buộc ghi ý kiến; chuyển Trả bổ sung cho KHTC phối hợp người lập xử lý. <br>E3. Chưa thẩm định hoặc sai phiên bản: không quyết định, yêu cầu kiểm tra lại. |
| Điều kiện sau | Lưu người quyết định, thời điểm, ý kiến và lịch sử; chỉ quyết định phê duyệt mới hình thành ngân sách. Người phê duyệt không sửa trực tiếp khoản mục hoặc số tiền. |
| Biểu đồ liên quan | Nhóm TC; AD01; SD02, có bước UC-TC09 trước quyết định. |

### UC-TC05 Ghi nhận khoản thu

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ kế toán |
| Mục tiêu | Ghi nhận khoản thu trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Người dùng có quyền ghi nhận thu trong phạm vi đơn vị. |
| Luồng sự kiện chính | 1\. Kế toán chọn khoản thu.  <br>2\. Nhập số tiền, ngày giao dịch, nội dung và chứng từ.  <br>3\. Hệ thống kiểm tra dữ liệu và chứng từ trùng.  <br>4\. Hệ thống lưu khoản thu và lịch sử thao tác.  <br>5\. Hệ thống hiển thị kết quả ghi nhận. |
| Luồng thay thế và ngoại lệ | E1. Dữ liệu không hợp lệ: yêu cầu sửa.  <br>E2. Chứng từ đã tồn tại: từ chối ghi nhận trùng. |
| Điều kiện sau | Khoản thu hợp lệ được lưu và đưa vào tổng hợp tài chính. |
| Biểu đồ liên quan | Use Case phân rã nhóm TC. AD02; SD03. |

### UC-TC06 Ghi nhận khoản chi

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ kế toán |
| Mục tiêu | Ghi nhận khoản chi trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có ngân sách liên quan; người dùng có quyền ghi nhận chi. |
| Luồng sự kiện chính | 1\. Kế toán nhập số tiền, ngày, nội dung và chứng từ chi.  <br>2\. Hệ thống kiểm tra quyền, dữ liệu và chứng từ trùng.  <br>3\. Hệ thống bắt đầu giao dịch, đọc và khóa ngân sách liên quan.  <br>4\. Nếu ngân sách khả dụng đủ, lưu khoản chi và cập nhật ngân sách đã sử dụng.  <br>5\. Hệ thống hoàn tất giao dịch và hiển thị kết quả. |
| Luồng thay thế và ngoại lệ | E1. Vượt ngân sách: không ghi nhận khoản chi; có thể chuyển sang lưu yêu cầu chờ xử lý.  <br>E2. Dữ liệu sai, chứng từ trùng hoặc cập nhật thất bại: không hoàn tất giao dịch. |
| Điều kiện sau | Khoản chi và ngân sách được cập nhật đồng thời, không chi vượt số khả dụng tại thời điểm xử lý. |
| Biểu đồ liên quan | Use Case phân rã nhóm TC. AD02; SD03. |

### UC-TC07 Lưu yêu cầu chi chờ xử lý

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ kế toán |
| Mục tiêu | Lưu yêu cầu chi chờ xử lý trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Khoản chi vượt ngân sách; người dùng chọn lưu chờ xử lý. |
| Luồng sự kiện chính | 1\. Hệ thống hiển thị cảnh báo vượt ngân sách.  <br>2\. Kế toán chọn lưu yêu cầu chờ xử lý.  <br>3\. Hệ thống kiểm tra lại dữ liệu của yêu cầu.  <br>4\. Hệ thống lưu yêu cầu với trạng thái Chờ xử lý.  <br>5\. Hệ thống thông báo đã lưu. |
| Luồng thay thế và ngoại lệ | E1. Người dùng không chọn lưu: hủy thao tác ghi nhận.  <br>E2. Dữ liệu yêu cầu không hợp lệ: yêu cầu bổ sung. |
| Điều kiện sau | Yêu cầu được lưu riêng; không được tính là khoản chi thực hiện hay ngân sách đã sử dụng. |
| Biểu đồ liên quan | Use Case phân rã nhóm TC. AD02; SD03. |

### UC-TC08 Theo dõi ngân sách

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ kế toán; Ban Giám hiệu |
| Mục tiêu | Theo dõi ngân sách trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Người dùng có quyền xem dữ liệu ngân sách theo đơn vị và kỳ. |
| Luồng sự kiện chính | 1\. Người dùng chọn kỳ và đơn vị.  <br>2\. Hệ thống xác định phạm vi dữ liệu được phép xem.  <br>3\. Hệ thống tổng hợp ngân sách được duyệt và các khoản chi thực hiện.  <br>4\. Hệ thống tính phần ngân sách còn khả dụng theo quy tắc áp dụng.  <br>5\. Hiển thị số liệu để theo dõi. |
| Luồng thay thế và ngoại lệ | E1. Không có dữ liệu: hiển thị thông báo phù hợp.  <br>E2. Chọn đơn vị ngoài phạm vi quyền: từ chối truy vấn. |
| Điều kiện sau | Người dùng xem được tình hình ngân sách; không tính yêu cầu chi chờ xử lý vào khoản đã chi. |
| Biểu đồ liên quan | Use Case phân rã nhóm TC. Liên quan AD02; SD03. |

## 3.3 Quản lý tài sản

### UC-TS01 Thêm tài sản

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Thêm tài sản trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Người dùng có quyền quản lý hồ sơ tài sản. |
| Luồng sự kiện chính | 1\. Người dùng chọn thêm tài sản.  <br>2\. Nhập mã, tên, loại, đơn vị quản lý, vị trí, nguyên giá và tình trạng.  <br>3\. Hệ thống kiểm tra hồ sơ và tính duy nhất của mã.  <br>4\. Người dùng xác nhận lưu.  <br>5\. Hệ thống tạo hồ sơ tài sản. |
| Luồng thay thế và ngoại lệ | E1. Mã trùng hoặc thông tin bắt buộc thiếu: yêu cầu sửa.  <br>E2. Giá trị không hợp lệ: không lưu. |
| Điều kiện sau | Hồ sơ tài sản được tạo để tra cứu, kiểm kê và theo dõi. |
| Biểu đồ liên quan | Use Case phân rã nhóm TS. Không có biểu đồ riêng. |

_Quy tắc triển khai chi tiết áp dụng theo mục 2.3.4 và cần được xác nhận trong khảo sát nghiệp vụ._

### UC-TS02 Cập nhật hồ sơ tài sản

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Cập nhật hồ sơ tài sản trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Tài sản tồn tại trong phạm vi quản lý. |
| Luồng sự kiện chính | 1\. Người dùng tra cứu và mở tài sản.  <br>2\. Hệ thống hiển thị hồ sơ hiện tại.  <br>3\. Người dùng sửa thông tin được phép cập nhật.  <br>4\. Hệ thống kiểm tra tính hợp lệ và quyền.  <br>5\. Hệ thống lưu hồ sơ mới. |
| Luồng thay thế và ngoại lệ | E1. Không tìm thấy tài sản: thông báo.  <br>E2. Thông tin cập nhật không hợp lệ: giữ dữ liệu cũ và yêu cầu sửa. |
| Điều kiện sau | Hồ sơ tài sản được cập nhật, không xóa lịch sử kiểm kê hoặc bảo trì. |
| Biểu đồ liên quan | Use Case phân rã nhóm TS. Không có biểu đồ riêng. |

_Quy tắc triển khai chi tiết áp dụng theo mục 2.3.4 và cần được xác nhận trong khảo sát nghiệp vụ._

### UC-TS03 Tra cứu tài sản

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Tra cứu tài sản trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Người dùng có quyền xem hồ sơ tài sản. |
| Luồng sự kiện chính | 1\. Người dùng nhập mã, tên hoặc chọn điều kiện lọc.  <br>2\. Hệ thống giới hạn dữ liệu theo quyền.  <br>3\. Hệ thống tìm các tài sản phù hợp.  <br>4\. Hiển thị danh sách và hồ sơ chi tiết khi người dùng chọn. |
| Luồng thay thế và ngoại lệ | E1. Không có kết quả: hiển thị danh sách rỗng.  <br>E2. Yêu cầu xem tài sản ngoài phạm vi: từ chối. |
| Điều kiện sau | Thông tin tài sản phù hợp được hiển thị; dữ liệu không thay đổi. |
| Biểu đồ liên quan | Use Case phân rã nhóm TS. Không có biểu đồ riêng. |

### UC-TS04 Lập đợt kiểm kê

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Lập đợt kiểm kê trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có tài sản trong phạm vi quản lý và quyền lập đợt kiểm kê. |
| Luồng sự kiện chính | 1\. Người dùng chọn đơn vị, phạm vi và thời gian.  <br>2\. Hệ thống kiểm tra quyền và dữ liệu.  <br>3\. Hệ thống lấy danh sách tài sản cần kiểm kê.  <br>4\. Hệ thống lưu đợt kiểm kê và dữ liệu hồ sơ tại thời điểm lập đợt.  <br>5\. Hiển thị danh sách làm căn cứ đối chiếu. |
| Luồng thay thế và ngoại lệ | E1. Phạm vi không hợp lệ hoặc không đủ quyền: từ chối.  <br>E2. Không có tài sản: thông báo và không tạo đợt rỗng. |
| Điều kiện sau | Đợt kiểm kê và dữ liệu đối chiếu được lưu. |
| Biểu đồ liên quan | Use Case phân rã nhóm TS. AD05; SD04. |

### UC-TS05 Ghi nhận và hoàn tất kiểm kê

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Ghi nhận và hoàn tất kiểm kê trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Đợt kiểm kê đã được lập và chưa hoàn tất. |
| Luồng sự kiện chính | 1\. Người dùng kiểm tra từng tài sản, nhập số lượng, tình trạng và ghi chú.  <br>2\. Hệ thống kiểm tra kết quả và đối chiếu với hồ sơ của đợt.  <br>3\. Nếu có chênh lệch, người dùng bổ sung giải trình và hệ thống lưu chênh lệch.  <br>4\. Người dùng chọn hoàn tất sau khi ghi nhận các tài sản.  <br>5\. Hệ thống kiểm tra mức độ đầy đủ, lưu trạng thái Hoàn tất và bảng tổng hợp. |
| Luồng thay thế và ngoại lệ | E1. Kết quả nhập sai: yêu cầu sửa.  <br>E2. Còn mục chưa xử lý: giữ trạng thái Đang kiểm kê và nêu nội dung cần bổ sung. |
| Điều kiện sau | Kết quả và chênh lệch được lưu; đợt chỉ hoàn tất khi đáp ứng điều kiện. |
| Biểu đồ liên quan | Use Case phân rã nhóm TS. AD05; SD04. |

### UC-TS06 Theo dõi tình trạng tài sản

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Theo dõi tình trạng tài sản trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Tài sản đã có hồ sơ và thông tin trạng thái. |
| Luồng sự kiện chính | 1\. Người dùng chọn phạm vi và điều kiện lọc.  <br>2\. Hệ thống lấy tình trạng tài sản và thông tin sử dụng, bảo trì liên quan.  <br>3\. Hiển thị tài sản sẵn sàng, đang sử dụng, bảo trì hoặc chưa khả dụng.  <br>4\. Người dùng xem chi tiết các trường hợp cần xử lý. |
| Luồng thay thế và ngoại lệ | E1. Chưa có dữ liệu trạng thái: thông báo cần cập nhật.  <br>E2. Không đủ quyền xem: từ chối. |
| Điều kiện sau | Người dùng xác định được tài sản khả dụng và tài sản cần xử lý. |
| Biểu đồ liên quan | Use Case phân rã nhóm TS. Liên quan SD05. |

### UC-TS07 Lập kế hoạch bảo trì

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Lập kế hoạch bảo trì trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Tài sản tồn tại; người dùng có quyền lập kế hoạch. |
| Luồng sự kiện chính | 1\. Người dùng chọn tài sản và nhập thời gian, nội dung bảo trì.  <br>2\. Hệ thống kiểm tra quyền và lịch sử dụng tài sản.  <br>3\. Nếu không xung đột, lưu kế hoạch và khoảng thời gian không khả dụng.  <br>4\. Đến khi thực hiện, người dùng xác nhận bắt đầu.  <br>5\. Hệ thống kiểm tra và chuyển kế hoạch sang Đang thực hiện, cập nhật trạng thái tài sản. |
| Luồng thay thế và ngoại lệ | E1. Xung đột lịch sử dụng: yêu cầu điều chỉnh.  <br>E2. Tài sản hoặc trạng thái kế hoạch không hợp lệ: không cập nhật. |
| Điều kiện sau | Kế hoạch được lưu; thời gian bảo trì được phản ánh vào khả dụng của tài sản. |
| Biểu đồ liên quan | Use Case phân rã nhóm TS. SD05. |

### UC-TS08 Ghi nhận kết quả và chi phí bảo trì

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Ghi nhận kết quả và chi phí bảo trì trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có kế hoạch bảo trì và kết quả thực hiện cần cập nhật. |
| Luồng sự kiện chính | 1\. Người dùng mở kế hoạch.  <br>2\. Nhập kết quả, thời điểm kết thúc và tình trạng sau bảo trì.  <br>3\. Nếu có chi phí phát sinh, nhập chi phí và liên kết chứng từ chi.  <br>4\. Hệ thống kiểm tra dữ liệu, quyền và trạng thái.  <br>5\. Lưu kết quả, thời gian ngừng hoạt động và cập nhật tài sản sẵn sàng hoặc chưa khả dụng. |
| Luồng thay thế và ngoại lệ | E1. Chi phí hoặc thời gian không hợp lệ: yêu cầu sửa.  <br>E2. Tài sản chưa hoạt động tốt: giữ trạng thái chưa khả dụng để tiếp tục xử lý. |
| Điều kiện sau | Kết quả bảo trì được lưu; chứng từ liên kết không tạo thêm khoản chi trùng. |
| Biểu đồ liên quan | Use Case phân rã nhóm TS. SD05. |

## 3.4 Quản lý phòng học và phòng thí nghiệm

### UC-PH01 Tra cứu phòng và lịch sử dụng

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ / Giảng viên; Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Tra cứu phòng và lịch sử dụng trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có dữ liệu phòng và người dùng được phép tra cứu. |
| Luồng sự kiện chính | 1\. Người dùng chọn ngày, thời gian, loại phòng và nhu cầu.  <br>2\. Hệ thống tìm phòng trong phạm vi được phép.  <br>3\. Hệ thống đối chiếu tình trạng phòng với lịch đã duyệt.  <br>4\. Hiển thị thông tin phòng và lịch tương ứng. |
| Luồng thay thế và ngoại lệ | E1. Khoảng thời gian không hợp lệ: yêu cầu sửa.  <br>E2. Không có phòng phù hợp: thông báo để đổi điều kiện tìm. |
| Điều kiện sau | Người dùng có thông tin để lựa chọn phòng; chưa giữ chỗ. |
| Biểu đồ liên quan | Use Case phân rã nhóm PH. AD03; SD06. |

### UC-PH02 Đăng ký sử dụng phòng

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ / Giảng viên |
| Mục tiêu | Đăng ký sử dụng phòng trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Người dùng có quyền đăng ký; phòng tồn tại. |
| Luồng sự kiện chính | 1\. Người dùng chọn phòng, thời gian, số người và mục đích.  <br>2\. Hệ thống kiểm tra dữ liệu, điều kiện phòng và lịch sử dụng.  <br>3\. Người dùng gửi đăng ký.  <br>4\. Hệ thống lưu yêu cầu Chờ duyệt.  <br>5\. Hiển thị mã yêu cầu và trạng thái. |
| Luồng thay thế và ngoại lệ | E1. Trùng lịch, phòng không khả dụng hoặc không đáp ứng nhu cầu: từ chối tạo đăng ký.  <br>E2. Giờ kết thúc không sau giờ bắt đầu: yêu cầu sửa. |
| Điều kiện sau | Yêu cầu Chờ duyệt được lưu; chưa trở thành lịch đã duyệt. |
| Biểu đồ liên quan | Use Case phân rã nhóm PH. AD03; SD06. |

### UC-PH03 Hủy đăng ký phòng

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ / Giảng viên |
| Mục tiêu | Hủy đăng ký phòng trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Đăng ký thuộc quyền của người dùng và đủ điều kiện hủy. |
| Luồng sự kiện chính | 1\. Người dùng mở đăng ký cần hủy.  <br>2\. Hệ thống kiểm tra quyền, trạng thái và điều kiện hủy.  <br>3\. Người dùng xác nhận hủy.  <br>4\. Hệ thống cập nhật trạng thái Đã hủy.  <br>5\. Nếu có lịch được giữ tương ứng, hệ thống cập nhật lịch khả dụng. |
| Luồng thay thế và ngoại lệ | E1. Không đủ quyền hoặc quá thời điểm cho phép: từ chối.  <br>E2. Đăng ký đã hủy: thông báo, không xử lý lặp. |
| Điều kiện sau | Đăng ký bị hủy được lưu vết; lịch liên quan được cập nhật phù hợp. |
| Biểu đồ liên quan | Use Case phân rã nhóm PH. Không có biểu đồ riêng. |

_Quy tắc triển khai chi tiết áp dụng theo mục 2.3.4 và cần được xác nhận trong khảo sát nghiệp vụ._

### UC-PH04 Cập nhật thông tin phòng

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Cập nhật thông tin phòng trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Phòng nằm trong phạm vi quản lý. |
| Luồng sự kiện chính | 1\. Người quản lý chọn phòng.  <br>2\. Nhập hoặc cập nhật mã, tên, loại phòng, sức chứa, vị trí và tình trạng.  <br>3\. Hệ thống kiểm tra tính hợp lệ và mã phòng.  <br>4\. Hệ thống kiểm tra ảnh hưởng đến khả năng sử dụng.  <br>5\. Lưu thông tin phòng. |
| Luồng thay thế và ngoại lệ | E1. Mã trùng hoặc sức chứa không hợp lệ: không lưu.  <br>E2. Thay đổi làm ảnh hưởng lịch đã duyệt: yêu cầu kiểm tra và xử lý lịch liên quan. |
| Điều kiện sau | Thông tin phòng được cập nhật để phục vụ đăng ký và phân tích. |
| Biểu đồ liên quan | Use Case phân rã nhóm PH. Không có biểu đồ riêng. |

_Quy tắc triển khai chi tiết áp dụng theo mục 2.3.4 và cần được xác nhận trong khảo sát nghiệp vụ._

### UC-PH05 Xét duyệt đăng ký phòng

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Xét duyệt đăng ký phòng trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có đăng ký Chờ duyệt thuộc phạm vi quản lý. |
| Luồng sự kiện chính | 1\. Người quản lý xem yêu cầu và chọn duyệt hoặc từ chối.  <br>2\. Hệ thống kiểm tra quyền và trạng thái.  <br>3\. Nếu duyệt, hệ thống bắt đầu giao dịch và kiểm tra lại lịch, tình trạng phòng.  <br>4\. Nếu còn đủ điều kiện, lưu lịch và trạng thái Đã duyệt trong cùng giao dịch.  <br>5\. Hiển thị kết quả cho người xử lý. |
| Luồng thay thế và ngoại lệ | E1. Từ chối: lưu lý do và trạng thái Từ chối.  <br>E2. Phát sinh xung đột lịch hoặc yêu cầu không còn hợp lệ: hủy giao dịch, không duyệt. |
| Điều kiện sau | Yêu cầu có kết quả xử lý; không tạo hai lịch đã duyệt xung đột. |
| Biểu đồ liên quan | Use Case phân rã nhóm PH. AD03; SD06. |

### UC-PH06 Gửi nhật ký sử dụng phòng thực tế

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ / Giảng viên |
| Mục tiêu | Gửi nhật ký sử dụng phòng thực tế trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có lịch được duyệt thuộc người dùng. |
| Luồng sự kiện chính | 1\. Người dùng chọn lịch đã duyệt.  <br>2\. Nếu có sử dụng, nhập giờ bắt đầu, kết thúc và số người thực tế.  <br>3\. Nếu không sử dụng, chọn Không sử dụng và nhập lý do.  <br>4\. Hệ thống kiểm tra quyền, trạng thái lịch và dữ liệu.  <br>5\. Lưu nhật ký Chờ xác nhận và hiển thị kết quả. |
| Luồng thay thế và ngoại lệ | E1. Dữ liệu thời gian hoặc số người không hợp lệ: yêu cầu sửa.  <br>E2. Không có quyền ghi nhật ký cho lịch đã chọn: từ chối. |
| Điều kiện sau | Nhật ký được lưu để người quản lý xác nhận; không đồng nhất thời gian đặt với thời gian thực tế. |
| Biểu đồ liên quan | Use Case phân rã nhóm PH. AD03; SD07. |

### UC-PH07 Kiểm tra và xác nhận nhật ký phòng

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Kiểm tra và xác nhận nhật ký phòng trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có nhật ký Chờ xác nhận trong phạm vi quản lý. |
| Luồng sự kiện chính | 1\. Người quản lý mở nhật ký và đối chiếu lịch sử dụng.  <br>2\. Chọn xác nhận hoặc yêu cầu bổ sung.  <br>3\. Hệ thống kiểm tra quyền và trạng thái nhật ký.  <br>4\. Nếu xác nhận, lưu trạng thái Đã xác nhận.  <br>5\. Hệ thống cung cấp dữ liệu đã xác nhận cho báo cáo và phân tích. |
| Luồng thay thế và ngoại lệ | E1. Nhật ký cần bổ sung: lưu lý do, chuyển Cần bổ sung để người dùng sửa và gửi lại.  <br>E2. Nhật ký đã thay đổi hoặc không đủ quyền: không cập nhật. |
| Điều kiện sau | Nhật ký được xác nhận hoặc trả lại để bổ sung, có trạng thái rõ ràng. |
| Biểu đồ liên quan | Use Case phân rã nhóm PH. AD03; SD07. |

## 3.5 Mượn và sử dụng thiết bị

### UC-TB01 Tra cứu thiết bị

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ / Giảng viên |
| Mục tiêu | Tra cứu thiết bị trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Người dùng có quyền tra cứu thiết bị. |
| Luồng sự kiện chính | 1\. Người dùng nhập tên, loại và thời gian cần sử dụng.  <br>2\. Hệ thống lấy danh mục và tình trạng thiết bị.  <br>3\. Đối chiếu lịch mượn, bảo trì và số lượng khả dụng.  <br>4\. Hiển thị thiết bị phù hợp để đăng ký. |
| Luồng thay thế và ngoại lệ | E1. Không có thiết bị phù hợp: thông báo.  <br>E2. Thời gian không hợp lệ: yêu cầu sửa. |
| Điều kiện sau | Danh sách thiết bị được hiển thị; tra cứu không giữ thiết bị. |
| Biểu đồ liên quan | Use Case phân rã nhóm TB. AD04; SD08. |

### UC-TB02 Đăng ký mượn thiết bị

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ / Giảng viên |
| Mục tiêu | Đăng ký mượn thiết bị trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Người dùng có quyền mượn; thiết bị đã có trong hệ thống. |
| Luồng sự kiện chính | 1\. Người dùng chọn thiết bị, số lượng, thời gian và mục đích.  <br>2\. Hệ thống kiểm tra dữ liệu và thiết bị khả dụng.  <br>3\. Người dùng gửi yêu cầu.  <br>4\. Hệ thống lưu phiếu Chờ duyệt.  <br>5\. Hiển thị trạng thái và kết quả. |
| Luồng thay thế và ngoại lệ | E1. Số lượng không hợp lệ hoặc vượt khả dụng: không tạo yêu cầu.  <br>E2. Thời gian mượn không hợp lệ: yêu cầu sửa. |
| Điều kiện sau | Yêu cầu Chờ duyệt được tạo; việc giữ thiết bị thực hiện khi duyệt. |
| Biểu đồ liên quan | Use Case phân rã nhóm TB. AD04; SD08. |

### UC-TB03 Hủy yêu cầu mượn

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ / Giảng viên |
| Mục tiêu | Hủy yêu cầu mượn trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Yêu cầu thuộc người dùng và đủ điều kiện hủy. |
| Luồng sự kiện chính | 1\. Người dùng chọn yêu cầu.  <br>2\. Hệ thống kiểm tra quyền và điều kiện hủy.  <br>3\. Người dùng xác nhận hủy.  <br>4\. Hệ thống lưu trạng thái Đã hủy và giải phóng phần thiết bị đã giữ nếu được phép.  <br>5\. Hiển thị kết quả. |
| Luồng thay thế và ngoại lệ | E1. Đã bàn giao hoặc không còn đủ điều kiện hủy: từ chối, hướng tới quy trình trả.  <br>E2. Yêu cầu đã được xử lý bởi thao tác khác: tải lại trạng thái. |
| Điều kiện sau | Yêu cầu bị hủy được lưu; không làm mất lịch sử mượn. |
| Biểu đồ liên quan | Use Case phân rã nhóm TB. Không có biểu đồ riêng. |

_Quy tắc triển khai chi tiết áp dụng theo mục 2.3.4 và cần được xác nhận trong khảo sát nghiệp vụ._

### UC-TB04 Xét duyệt yêu cầu mượn

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Xét duyệt yêu cầu mượn trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có phiếu Chờ duyệt trong phạm vi quản lý. |
| Luồng sự kiện chính | 1\. Người quản lý xem thiết bị, số lượng và thời gian.  <br>2\. Chọn duyệt hoặc từ chối.  <br>3\. Hệ thống kiểm tra quyền và trạng thái.  <br>4\. Nếu duyệt, khóa nguồn lực, kiểm tra lại khả dụng và giữ thiết bị theo lịch trong giao dịch.  <br>5\. Lưu kết quả và hiển thị trạng thái. |
| Luồng thay thế và ngoại lệ | E1. Không đủ thiết bị tại thời điểm duyệt: không xác nhận giữ thiết bị.  <br>E2. Từ chối: lưu trạng thái Từ chối và lý do. |
| Điều kiện sau | Phiếu có quyết định; phiếu được duyệt có phần thiết bị được giữ theo lịch. |
| Biểu đồ liên quan | Use Case phân rã nhóm TB. AD04; SD08. |

### UC-TB05 Ghi nhận bàn giao thiết bị

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Ghi nhận bàn giao thiết bị trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Phiếu mượn đã duyệt; thiết bị đủ điều kiện bàn giao. |
| Luồng sự kiện chính | 1\. Người quản lý mở phiếu.  <br>2\. Kiểm tra thiết bị thực giao, số lượng và tình trạng.  <br>3\. Lập biên bản bàn giao.  <br>4\. Hệ thống kiểm tra trạng thái phiếu và dữ liệu.  <br>5\. Lưu biên bản chờ người mượn xác nhận. |
| Luồng thay thế và ngoại lệ | E1. Thiết bị thực tế không khớp phiếu: điều chỉnh trước khi bàn giao.  <br>E2. Phiếu không còn ở trạng thái phù hợp: không lưu bàn giao. |
| Điều kiện sau | Biên bản được lưu; chưa coi là người mượn đã xác nhận nhận. |
| Biểu đồ liên quan | Use Case phân rã nhóm TB. AD04; SD08. |

### UC-TB06 Xác nhận nhận thiết bị

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ / Giảng viên |
| Mục tiêu | Xác nhận nhận thiết bị trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có biên bản bàn giao gắn với phiếu mượn của người dùng. |
| Luồng sự kiện chính | 1\. Người mượn kiểm tra số lượng và tình trạng thiết bị.  <br>2\. Đối chiếu với biên bản.  <br>3\. Nếu khớp, chọn xác nhận nhận.  <br>4\. Hệ thống kiểm tra quyền và biên bản.  <br>5\. Lưu thời điểm nhận và chuyển phiếu sang Đang mượn. |
| Luồng thay thế và ngoại lệ | E1. Thông tin không khớp: báo sai lệch để người quản lý điều chỉnh.  <br>E2. Biên bản không hợp lệ hoặc đã xử lý: không xác nhận lặp. |
| Điều kiện sau | Việc nhận thiết bị được ghi nhận; phiếu chuyển Đang mượn. |
| Biểu đồ liên quan | Use Case phân rã nhóm TB. AD04; SD08. |

### UC-TB07 Ghi nhận sử dụng thiết bị thực tế

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ / Giảng viên |
| Mục tiêu | Ghi nhận sử dụng thiết bị thực tế trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Thiết bị đã được bàn giao cho người dùng. |
| Luồng sự kiện chính | 1\. Người dùng mở phiếu Đang mượn.  <br>2\. Nhập các khoảng thời gian sử dụng và ghi chú.  <br>3\. Hệ thống kiểm tra quyền và dữ liệu nhật ký.  <br>4\. Lưu nhật ký gắn với phiếu và thiết bị.  <br>5\. Hiển thị kết quả. |
| Luồng thay thế và ngoại lệ | E1. Thời gian không hợp lệ hoặc ngoài phạm vi được phép: yêu cầu sửa.  <br>E2. Phiếu không thuộc người dùng: từ chối. |
| Điều kiện sau | Thời gian sử dụng thực tế được lưu riêng với thời gian giữ hoặc mượn thiết bị. |
| Biểu đồ liên quan | Use Case phân rã nhóm TB. AD04; SD08. |

_Quy tắc triển khai chi tiết áp dụng theo mục 2.3.4 và cần được xác nhận trong khảo sát nghiệp vụ._

### UC-TB08 Gửi yêu cầu trả thiết bị

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ / Giảng viên |
| Mục tiêu | Gửi yêu cầu trả thiết bị trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Phiếu còn thiết bị chưa được tiếp nhận trả. |
| Luồng sự kiện chính | 1\. Người mượn chọn phiếu và thiết bị cần trả.  <br>2\. Nhập số lượng và tình trạng tự khai.  <br>3\. Gửi yêu cầu trả.  <br>4\. Hệ thống kiểm tra và lưu yêu cầu.  <br>5\. Người mượn bàn giao thiết bị thực tế cho người quản lý. |
| Luồng thay thế và ngoại lệ | E1. Số lượng trả vượt phần còn mượn: yêu cầu sửa.  <br>E2. Phiếu đã hoàn tất: không tạo yêu cầu trả mới. |
| Điều kiện sau | Yêu cầu trả được lưu; thiết bị chưa tự chuyển thành khả dụng. |
| Biểu đồ liên quan | Use Case phân rã nhóm TB. AD04; SD08. |

### UC-TB09 Tiếp nhận trả thiết bị

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Tiếp nhận trả thiết bị trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có thiết bị được người mượn bàn giao trả và phiếu tương ứng. |
| Luồng sự kiện chính | 1\. Người quản lý kiểm tra thực tế số lượng, tình trạng.  <br>2\. Xác nhận số lượng thực nhận.  <br>3\. Hệ thống kiểm tra quyền, trạng thái và phần còn mượn.  <br>4\. Lưu thời điểm trả, đánh dấu quá hạn nếu có; thiết bị tốt trở lại khả dụng, thiết bị hỏng chuyển Chờ xử lý.  <br>5\. Đóng phiếu khi đã trả đủ, giữ phần chưa trả nếu còn thiếu. |
| Luồng thay thế và ngoại lệ | E1. Số lượng tiếp nhận không hợp lệ: không cập nhật.  <br>E2. Thiết bị hỏng: ghi nhận hư hỏng, không đưa ngay vào số khả dụng. |
| Điều kiện sau | Số lượng và trạng thái thiết bị phản ánh kết quả thực nhận; hỗ trợ trả từng phần. |
| Biểu đồ liên quan | Use Case phân rã nhóm TB. AD04; SD08. |

### UC-TB10 Theo dõi sử dụng và quá hạn trả

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Theo dõi sử dụng và quá hạn trả trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có dữ liệu phiếu mượn, bàn giao và tiếp nhận trả. |
| Luồng sự kiện chính | 1\. Người quản lý chọn khoảng thời gian, đơn vị hoặc thiết bị.  <br>2\. Hệ thống tổng hợp phần đang mượn, đã trả và còn phải trả.  <br>3\. Đối chiếu hạn trả với thời điểm trả hoặc thời điểm tra cứu.  <br>4\. Hiển thị các phiếu quá hạn và nhật ký sử dụng liên quan.  <br>5\. Người quản lý xem chi tiết để theo dõi. |
| Luồng thay thế và ngoại lệ | E1. Không có kết quả: hiển thị danh sách rỗng.  <br>E2. Dữ liệu thời hạn thiếu: nêu thông tin cần bổ sung, không kết luận sai về quá hạn. |
| Điều kiện sau | Tình hình sử dụng và quá hạn được hiển thị theo phạm vi quyền. |
| Biểu đồ liên quan | Use Case phân rã nhóm TB. AD04; SD08. |

## 3.6 Báo cáo phân tích và hỗ trợ quyết định

### UC-PT01 Xem báo cáo tổng hợp

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Ban Giám hiệu; Cán bộ kế toán; Cán bộ quản lý phòng và tài sản |
| Mục tiêu | Xem báo cáo tổng hợp trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Người dùng được cấp quyền xem báo cáo và phạm vi dữ liệu. |
| Luồng sự kiện chính | 1\. Người dùng chọn loại báo cáo, kỳ và đơn vị.  <br>2\. Hệ thống kiểm tra quyền.  <br>3\. Truy vấn dữ liệu theo bộ lọc được phép.  <br>4\. Tổng hợp số liệu tài chính, tài sản hoặc sử dụng nguồn lực.  <br>5\. Hiển thị bảng và biểu đồ báo cáo. |
| Luồng thay thế và ngoại lệ | E1. Không đủ quyền: từ chối.  <br>E2. Không có dữ liệu: thông báo, không tạo số liệu giả. |
| Điều kiện sau | Báo cáo được hiển thị; không tự kích hoạt phân tích, dự báo hay phân bổ. |
| Biểu đồ liên quan | Use Case phân rã nhóm PT. AD06; SD09. |

### UC-PT02 Xuất báo cáo

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Người dùng được cấp quyền xuất báo cáo |
| Mục tiêu | Xuất báo cáo trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Đã có báo cáo và người dùng có quyền xuất dữ liệu tương ứng. |
| Luồng sự kiện chính | 1\. Người dùng chọn xuất Excel hoặc PDF.  <br>2\. Hệ thống kiểm tra quyền xuất và phạm vi dữ liệu.  <br>3\. Tạo tệp từ dữ liệu báo cáo được phép.  <br>4\. Cung cấp tệp cho người dùng tải xuống. |
| Luồng thay thế và ngoại lệ | E1. Không có quyền xuất: từ chối dù người dùng có quyền xem.  <br>E2. Lỗi tạo tệp: thông báo và cho phép thử lại. |
| Điều kiện sau | Tệp báo cáo được tạo đúng bộ lọc và phạm vi quyền. |
| Biểu đồ liên quan | Use Case phân rã nhóm PT. AD06; SD09. |

### UC-PT03 Phân tích hiệu quả sử dụng nguồn lực

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Người dùng được cấp quyền phân tích |
| Mục tiêu | Phân tích hiệu quả sử dụng nguồn lực trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có dữ liệu sử dụng, chi phí và khả dụng phù hợp với chỉ tiêu. |
| Luồng sự kiện chính | 1\. Người dùng chọn nguồn lực, kỳ và chỉ tiêu.  <br>2\. Hệ thống kiểm tra quyền, lấy dữ liệu nguồn và kiểm tra chất lượng.  <br>3\. Tính thời gian khả dụng theo quy tắc đã chọn.  <br>4\. Tính chỉ số đủ điều kiện và so sánh theo kỳ, tiêu chí.  <br>5\. Lưu kết quả, bộ lọc, quy tắc tính và hiển thị nhận xét. |
| Luồng thay thế và ngoại lệ | E1. Thiếu dữ liệu: nêu chỉ tiêu chưa tính được.  <br>E2. Mẫu số bằng 0: ghi Không xác định cho chỉ số liên quan, không thay bằng 0. |
| Điều kiện sau | Kết quả phân tích có căn cứ dữ liệu và quy tắc tính để kiểm tra lại. |
| Biểu đồ liên quan | Use Case phân rã nhóm PT. AD06; SD10. |

### UC-PT04 Dự báo nhu cầu nguồn lực

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Người dùng được cấp quyền dự báo |
| Mục tiêu | Dự báo nhu cầu nguồn lực trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có dữ liệu nhu cầu lịch sử đáp ứng yêu cầu mô hình được chọn. |
| Luồng sự kiện chính | 1\. Người dùng chọn nguồn lực, đơn vị và kỳ dự báo.  <br>2\. Hệ thống kiểm tra quyền, tham số và dữ liệu lịch sử.  <br>3\. Chuẩn bị dữ liệu theo thứ tự thời gian.  <br>4\. Đánh giá mô hình trên giai đoạn lịch sử giữ lại và dự báo kỳ tiếp theo.  <br>5\. Lưu kết quả, sai số, phiên bản mô hình, tham số và thời điểm chạy. |
| Luồng thay thế và ngoại lệ | E1. Dữ liệu quá ngắn, thiếu hoặc không nhất quán: thông báo chưa đủ điều kiện.  <br>E2. Tham số không hợp lệ: yêu cầu điều chỉnh. |
| Điều kiện sau | Dự báo và sai số đánh giá được lưu; sai số lịch sử không phải bảo đảm kết quả tương lai. |
| Biểu đồ liên quan | Use Case phân rã nhóm PT. AD06; SD11. |

### UC-PT05 Tạo phương án đề xuất phân bổ

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Người dùng được cấp quyền đề xuất phân bổ |
| Mục tiêu | Tạo phương án đề xuất phân bổ trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có nguồn lực khả dụng, nhu cầu, mục tiêu và ràng buộc cần xem xét. |
| Luồng sự kiện chính | 1\. Người dùng chọn kỳ, nguồn lực, nhu cầu, mục tiêu và ràng buộc.  <br>2\. Hệ thống lấy lịch đã duyệt, chi phí và giới hạn ngân sách; đọc dự báo đã có nếu được chọn.  <br>3\. Kiểm tra dữ liệu và tính nhất quán của ràng buộc.  <br>4\. Bộ tính tìm phương án thỏa ràng buộc, đánh giá chi phí và mức đáp ứng.  <br>5\. Lưu phương án Đề xuất kèm đầu vào, tiêu chí và hiển thị kết quả. |
| Luồng thay thế và ngoại lệ | E1. Đầu vào thiếu hoặc ràng buộc mâu thuẫn: nêu nội dung cần sửa.  <br>E2. Không tìm được phương án: thông báo kết quả trong điều kiện tính toán, không tự nới ràng buộc. |
| Điều kiện sau | Phương án chỉ được lưu dưới dạng đề xuất; không tự thay đổi ngân sách, lịch hoặc cấp thiết bị. |
| Biểu đồ liên quan | Use Case phân rã nhóm PT. AD06; SD12. |

### UC-PT06 Xem và so sánh phương án phân bổ

| **Trường đặc tả** | **Nội dung** |
| --- | --- |
| Tác nhân | Người dùng được cấp quyền đề xuất phân bổ |
| Mục tiêu | Xem và so sánh phương án phân bổ trong phạm vi chức năng được phân quyền. |
| Điều kiện trước | Có các phương án đã lưu trong phạm vi được xem. |
| Luồng sự kiện chính | 1\. Người dùng mở danh sách phương án.  <br>2\. Chọn phương án và các chỉ số so sánh.  <br>3\. Hệ thống đọc đầu vào, ràng buộc và kết quả đã lưu.  <br>4\. Hiển thị chi phí, mức đáp ứng nhu cầu và điều kiện của từng phương án.  <br>5\. Người dùng xem xét phương án để hỗ trợ quyết định. |
| Luồng thay thế và ngoại lệ | E1. Phương án không tồn tại hoặc ngoài phạm vi quyền: từ chối.  <br>E2. Các phương án khác kỳ hoặc khác giả định: hiển thị khác biệt trước khi đối chiếu. |
| Điều kiện sau | Người dùng có cơ sở so sánh; xem phương án không làm thay đổi phân bổ thực tế. |
| Biểu đồ liên quan | Use Case phân rã nhóm PT. Liên quan AD06; SD12. |

# 4 Biểu đồ hoạt động

Biểu đồ hoạt động mô tả thứ tự công việc, điều kiện rẽ nhánh, vòng lặp và trách nhiệm của từng tác nhân. Sáu biểu đồ dưới đây tập trung vào các quy trình nghiệp vụ chính; chúng không thay thế danh sách 48 chức năng đã đặc tả.

| **Mã** | **Quy trình** | **Use Case liên quan** |
| --- | --- | --- |
| AD01 | Lập và xét duyệt dự toán | UC-TC01 đến UC-TC04 |
| AD02 | Ghi nhận thu chi | UC-TC05 đến UC-TC07 |
| AD03 | Đăng ký và sử dụng phòng | UC-PH01, UC-PH02, UC-PH05 đến UC-PH07 |
| AD04 | Mượn và trả thiết bị | UC-TB01, UC-TB02, UC-TB04 đến UC-TB10 |
| AD05 | Kiểm kê tài sản | UC-TS04, UC-TS05 |
| AD06 | Báo cáo và hỗ trợ quyết định | UC-PT01 đến UC-PT06 |

## 4.1 AD01 Lập và xét duyệt dự toán

Luồng: Nháp → Chờ thẩm định KHTC → Chờ phê duyệt → Đã duyệt/Từ chối. KHTC hoặc người phê duyệt có thể trả bổ sung kèm lý do. Người lập chỉnh sửa, gửi thẩm định lại; người phê duyệt không sửa nội dung. Chỉ Đã duyệt mới ghi nhận ngân sách. Đối chiếu: UC-TC01 đến UC-TC04 và UC-TC09.

Hình 4.1 Hoạt động lập và xét duyệt dự toán

## 4.2 AD02 Ghi nhận thu chi

Tách khoản thu với khoản chi. Khoản chi vượt ngân sách không được ghi nhận thực hiện; người dùng có thể lưu yêu cầu chờ xử lý. Đối chiếu: UC-TC05 đến UC-TC07.

Hình 4.2 Hoạt động ghi nhận thu chi

## 4.3 AD03 Đăng ký và sử dụng phòng

Sau khi duyệt lịch, người dùng ghi nhận có hoặc không sử dụng thực tế; người quản lý kiểm tra và xác nhận nhật ký. Đối chiếu: UC-PH01, UC-PH02, UC-PH05 đến UC-PH07.

Hình 4.3 Hoạt động đăng ký và sử dụng phòng

## 4.4 AD04 Mượn và trả thiết bị

Quy trình từ đăng ký đến duyệt, bàn giao, nhận, sử dụng và trả. Phiếu còn thiết bị chưa trả tiếp tục được theo dõi. Đối chiếu: UC-TB01, UC-TB02, UC-TB04 đến UC-TB10.

Hình 4.4 Hoạt động mượn và trả thiết bị

## 4.4 AD04 Mượn và trả thiết bị phần phóng to 1

Phần 1/2 của hình 4.4 hoạt động mượn và trả thiết bị. Đọc từ trên xuống dưới; vùng tiếp giáp được lặp để theo dõi luồng.

Hình 4.4 Hoạt động mượn và trả thiết bị — phần phóng to 1/2

## 4.4 AD04 Mượn và trả thiết bị phần phóng to 2

Phần 2/2 của hình 4.4 hoạt động mượn và trả thiết bị. Đọc từ trên xuống dưới; vùng tiếp giáp được lặp để theo dõi luồng.

Hình 4.4 Hoạt động mượn và trả thiết bị — phần phóng to 2/2

## 4.5 AD05 Kiểm kê tài sản

Lập đợt và lưu dữ liệu đối chiếu; ghi nhận từng tài sản, chênh lệch và kiểm tra đầy đủ trước khi hoàn tất. Đối chiếu: UC-TS04, UC-TS05.

Hình 4.5 Hoạt động kiểm kê tài sản

## 4.5 AD05 Kiểm kê tài sản phần phóng to 1

Phần 1/2 của hình 4.5 hoạt động kiểm kê tài sản. Đọc từ trên xuống dưới; vùng tiếp giáp được lặp để theo dõi luồng.

Hình 4.5 Hoạt động kiểm kê tài sản — phần phóng to 1/2

## 4.5 AD05 Kiểm kê tài sản phần phóng to 2

Phần 2/2 của hình 4.5 hoạt động kiểm kê tài sản. Đọc từ trên xuống dưới; vùng tiếp giáp được lặp để theo dõi luồng.

Hình 4.5 Hoạt động kiểm kê tài sản — phần phóng to 2/2

## 4.6 AD06 Báo cáo và hỗ trợ quyết định

Bốn nhánh chức năng độc lập gồm báo cáo, phân tích, dự báo và đề xuất phân bổ. Các nhánh đều kiểm tra quyền và điều kiện dữ liệu. Đối chiếu: UC-PT01 đến UC-PT06.

Hình 4.6 Hoạt động báo cáo và hỗ trợ quyết định

## 4.6 AD06 Báo cáo và hỗ trợ quyết định phần phóng to 1

Phần 1/3 của hình 4.6 hoạt động báo cáo và hỗ trợ quyết định. Đọc từ trái sang phải; xem hình tổng thể để theo dõi các nhánh nối qua vùng cắt.

Hình 4.6 Hoạt động báo cáo và hỗ trợ quyết định — phần phóng to 1/3

## 4.6 AD06 Báo cáo và hỗ trợ quyết định phần phóng to 2

Phần 2/3 của hình 4.6 hoạt động báo cáo và hỗ trợ quyết định. Đọc từ trái sang phải; xem hình tổng thể để theo dõi các nhánh nối qua vùng cắt.

Hình 4.6 Hoạt động báo cáo và hỗ trợ quyết định — phần phóng to 2/3

## 4.6 AD06 Báo cáo và hỗ trợ quyết định phần phóng to 3

Phần 3/3 của hình 4.6 hoạt động báo cáo và hỗ trợ quyết định. Đọc từ trái sang phải; xem hình tổng thể để theo dõi các nhánh nối qua vùng cắt.

Hình 4.6 Hoạt động báo cáo và hỗ trợ quyết định — phần phóng to 3/3

# 5 Biểu đồ tuần tự

Biểu đồ tuần tự mô tả thông điệp theo thời gian giữa tác nhân, giao diện, xử lý nghiệp vụ và cơ sở dữ liệu. Nhánh alt thể hiện các trường hợp thay thế; opt là xử lý tùy chọn; loop là xử lý lặp. Tên thành phần thể hiện trách nhiệm logic, chưa ràng buộc vào ngôn ngữ lập trình hoặc kiến trúc triển khai cụ thể.

| **Mã** | **Quy trình** | **Use Case liên quan** |
| --- | --- | --- |
| SD01 | Đăng nhập | UC-HT01 |
| SD02 | Lập và phê duyệt dự toán | UC-TC01 đến UC-TC04 |
| SD03 | Ghi nhận thu chi | UC-TC05 đến UC-TC07 |
| SD04 | Kiểm kê tài sản | UC-TS04, UC-TS05 |
| SD05 | Bảo trì tài sản | UC-TS07, UC-TS08 |
| SD06 | Đăng ký và phê duyệt sử dụng phòng | UC-PH01, UC-PH02, UC-PH05 |
| SD07 | Ghi nhận sử dụng phòng thực tế | UC-PH06, UC-PH07 |
| SD08 | Mượn và trả thiết bị | UC-TB01, UC-TB02, UC-TB04 đến UC-TB10 |
| SD09 | Xem và xuất báo cáo | UC-PT01, UC-PT02 |
| SD10 | Phân tích hiệu quả sử dụng nguồn lực | UC-PT03 |
| SD11 | Dự báo nhu cầu nguồn lực | UC-PT04 |
| SD12 | Đề xuất phân bổ nguồn lực | UC-PT05, UC-PT06 |

## 5.1 SD01 Đăng nhập

Kiểm tra tài khoản, trạng thái, mật khẩu và nạp quyền trước khi tạo phiên. Đối chiếu: UC-HT01.

Hình 5.1 Tuần tự đăng nhập

## 5.2 SD02 Lập và phê duyệt dự toán

Tách ba phiên: kế toán lập/gửi thẩm định; KHTC thẩm định/trình duyệt; Hiệu trưởng/người được ủy quyền quyết định. Mỗi chuyển trạng thái kiểm tra quyền và phiên bản. Trả bổ sung quay về người lập qua KHTC; sửa xong phải thẩm định lại. Đối chiếu: UC-TC01 đến UC-TC04 và UC-TC09.

Hình 5.2 Tuần tự lập và phê duyệt dự toán

## 5.3 SD03 Ghi nhận thu chi

Kiểm tra chứng từ trùng; ghi nhận chi cùng cập nhật ngân sách trong giao dịch; vượt ngân sách có thể lưu chờ. Đối chiếu: UC-TC05 đến UC-TC07.

Hình 5.3 Tuần tự ghi nhận thu chi

## 5.4 SD04 Kiểm kê tài sản

Lưu dữ liệu đối chiếu của đợt kiểm kê, xử lý từng tài sản và kiểm tra đầy đủ khi hoàn tất. Đối chiếu: UC-TS04, UC-TS05.

Hình 5.4 Tuần tự kiểm kê tài sản

## 5.5 SD05 Bảo trì tài sản

Lập kế hoạch, bắt đầu bảo trì, ghi nhận kết quả và chi phí liên kết chứng từ; không tạo khoản chi trùng. Đối chiếu: UC-TS07, UC-TS08.

Hình 5.5 Tuần tự bảo trì tài sản

## 5.6 SD06 Đăng ký và phê duyệt sử dụng phòng

Đăng ký và xét duyệt là hai phiên xử lý; khi duyệt phải kiểm tra lại lịch trong giao dịch. Đối chiếu: UC-PH01, UC-PH02, UC-PH05.

Hình 5.6 Tuần tự đăng ký và phê duyệt sử dụng phòng

## 5.7 SD07 Ghi nhận sử dụng phòng thực tế

Nhật ký được gửi Chờ xác nhận; người quản lý xác nhận hoặc yêu cầu sửa. Dữ liệu đã xác nhận phục vụ phân tích. Đối chiếu: UC-PH06, UC-PH07.

Hình 5.7 Tuần tự ghi nhận sử dụng phòng thực tế

## 5.8 SD08 Mượn và trả thiết bị

Tách rõ giữ thiết bị khi duyệt, bàn giao, xác nhận nhận, nhật ký và tiếp nhận trả; chỉ thiết bị tốt đã nhận lại mới khả dụng. Đối chiếu: UC-TB01, UC-TB02, UC-TB04 đến UC-TB10.

Hình 5.8 Tuần tự mượn và trả thiết bị

## 5.8 SD08 Mượn và trả thiết bị phần phóng to 1

Phần 1/2 của hình 5.8 tuần tự mượn và trả thiết bị. Đọc từ trên xuống dưới; vùng tiếp giáp được lặp để theo dõi luồng.

Hình 5.8 Tuần tự mượn và trả thiết bị — phần phóng to 1/2

## 5.8 SD08 Mượn và trả thiết bị phần phóng to 2

Phần 2/2 của hình 5.8 tuần tự mượn và trả thiết bị. Đọc từ trên xuống dưới; vùng tiếp giáp được lặp để theo dõi luồng.

Hình 5.8 Tuần tự mượn và trả thiết bị — phần phóng to 2/2

## 5.9 SD09 Xem và xuất báo cáo

Quyền xem và xuất được kiểm tra riêng; báo cáo không tự chạy phân tích hay dự báo. Đối chiếu: UC-PT01, UC-PT02.

Hình 5.9 Tuần tự xem và xuất báo cáo

## 5.10 SD10 Phân tích hiệu quả sử dụng nguồn lực

Kiểm tra chất lượng dữ liệu; xử lý mẫu số bằng 0; lưu kết quả cùng bộ lọc và quy tắc tính. Đối chiếu: UC-PT03.

Hình 5.10 Tuần tự phân tích hiệu quả sử dụng nguồn lực

## 5.11 SD11 Dự báo nhu cầu nguồn lực

Chuẩn bị chuỗi thời gian, đánh giá trên dữ liệu lịch sử giữ lại, dự báo và lưu thông tin mô hình. Đối chiếu: UC-PT04.

Hình 5.11 Tuần tự dự báo nhu cầu nguồn lực

## 5.12 SD12 Đề xuất phân bổ nguồn lực

Đầu vào có thể dùng nhu cầu nhập hoặc dự báo đã có; lưu phương án Đề xuất và các chỉ số, chưa thực hiện phân bổ. Đối chiếu: UC-PT05, UC-PT06.

Hình 5.12 Tuần tự đề xuất phân bổ nguồn lực

# 6 Kết luận và hướng phát triển

## 6.1 Kết quả phân tích

Tài liệu xác định sáu nhóm chức năng và 48 Use Case cho hệ thống quản trị tài chính, tài sản và khai thác hiệu quả nguồn lực đại học tại Trường Đại học Hải Phòng. Các đặc tả mô tả trách nhiệm tác nhân, điều kiện thực hiện, luồng chính, ngoại lệ và kết quả. Bộ 25 biểu đồ tạo cơ sở đối chiếu từ phạm vi chức năng đến quy trình và tương tác xử lý.

Điểm trọng tâm là liên kết quản trị nguồn lực với dữ liệu khai thác thực tế. Nhật ký sử dụng, tình trạng khả dụng, chi phí và dữ liệu nhu cầu giúp hệ thống vượt ra ngoài việc lưu danh mục, đồng thời cung cấp đầu vào có kiểm soát cho phân tích, dự báo và đề xuất phân bổ.

## 6.2 Hướng nghiên cứu và đánh giá

Bước tiếp theo là khảo sát đơn vị sử dụng để xác nhận quyền, quy trình, chỉ tiêu và dữ liệu có thể thu thập. Phần dự báo cần chọn mô hình phù hợp với quy mô và tính mùa vụ của dữ liệu, so sánh với phương pháp cơ sở và đánh giá theo thời gian. Chỉ công bố sai số sau khi thực nghiệm trên dữ liệu có nguồn gốc rõ ràng.

Phần phân bổ cần xác định mục tiêu và ràng buộc cụ thể, ví dụ đáp ứng nhu cầu sử dụng trong giới hạn phòng, thiết bị và ngân sách. Chất lượng phương án phải được đánh giá bằng mức đáp ứng, chi phí và vi phạm ràng buộc. Một phương án tìm được chưa đủ để khẳng định tối ưu toàn cục; người quản lý vẫn chịu trách nhiệm xem xét trước khi áp dụng.

## 6.3 Đầu ra cho thiết kế và kiểm thử

Danh sách Use Case là đầu vào cho kế hoạch phát triển và phân chia chức năng. Các luồng và ngoại lệ là căn cứ xây dựng test case, ưu tiên các tình huống vượt ngân sách, chứng từ trùng, duyệt đồng thời, trùng lịch, thiếu thiết bị, trả từng phần, dữ liệu phân tích thiếu và không tìm được phương án phân bổ.

Thiết kế cơ sở dữ liệu và biểu đồ lớp cần được xây dựng tiếp trên các thực thể tài khoản, vai trò, đơn vị, dự toán, giao dịch, chứng từ, tài sản, kiểm kê, bảo trì, phòng, lịch, nhật ký, phiếu mượn, dự báo và phương án. Việc triển khai nên hoàn thiện dữ liệu nền và nghiệp vụ cốt lõi trước, sau đó bổ sung báo cáo, phân tích và nghiên cứu mô hình khi dữ liệu đủ điều kiện.

## 6.4 Tài liệu đầu vào

Biểu đồ Use Case tổng quát 2.0 — phạm vi hệ thống và tác nhân.

Biểu đồ Use Case chi tiết phân rã — sáu nhóm chức năng.

Hoạt động 2.0 — sáu quy trình hoạt động.

Tuần tự 2.0 — mười hai quy trình tương tác.

04\. UseCase_v2(2).docx — mẫu bố cục và bảng đặc tả.

## Ghi chú đồng bộ sau bổ sung vai trò KHTC

Bản đặc tả đã bổ sung UC-TC09 và phân biệt người lập, người thẩm định, người phê duyệt. Các tệp Draw.io và hình minh họa cũ chưa được cập nhật trong lần sửa văn bản này; không dùng hình cũ để kết luận dự toán được gửi thẳng tới BGH. Khi cập nhật hình, giữ mã UC hiện có, thêm liên kết Cán bộ kế hoạch - tài chính → UC-TC09 và đặt bước thẩm định trước UC-TC04 trong AD01/SD02. Demo hiện có bước KHTC nhưng vẫn dùng chung khu vực kế toán; chưa phải phân quyền backend riêng.

Đây là mô hình nghiệp vụ đề xuất cho dự toán nội bộ, chưa thay thế quy định phân cấp/ủy quyền của Trường. Căn cứ tham khảo: [chức năng Phòng Kế hoạch - Tài chính Đại học Hải Phòng](https://next.dhhp.edu.vn/post/phong-ke-hoach-tai-chinh-38798.html) và [quy định thanh toán của Đại học Thủ đô Hà Nội, Điều 4 — chỉ dùng đối chiếu](https://kehoachtaichinh.hnmu.edu.vn/sites/default/files/2025-11/qd-24402025-quy-dinh-thanh-toan-dhtdhn_0.pdf).
