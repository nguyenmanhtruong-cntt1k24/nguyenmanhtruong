// Hướng dẫn theo từng trang chức năng HPUNI STII
export const PAGE_GUIDES = {
  "admin.html": {
    "users": {
      "title": "Quản lý tài khoản",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Tạo, cập nhật và quản lý trạng thái tài khoản người dùng."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở menu Quản trị viên → Quản lý tài khoản.",
            "Nhập họ tên, tên đăng nhập hoặc email vào ô Tìm tài khoản.",
            "Kết hợp bộ lọc Đơn vị, Trạng thái và Vai trò; xem số tài khoản phù hợp phía dưới.",
            "Đối chiếu cột Người dùng, Đơn vị / Vai trò và Trạng thái trước khi chọn thao tác."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Tạo tài khoản: nhấn + Tạo tài khoản, nhập họ tên, tên đăng nhập, email, đơn vị công tác, chọn vai trò và mật khẩu ban đầu; nhấn Tạo tài khoản.",
            "Chi tiết: mở thông tin tài khoản để đối chiếu định danh và cấu hình hiện có.",
            "Sửa: cập nhật họ tên, email, đơn vị và vai trò, sau đó nhấn Lưu tài khoản. Tên đăng nhập của tài khoản hiện có chỉ đọc.",
            "Đặt lại mật khẩu: nhập mật khẩu mới trong hộp Cấp lại mật khẩu và xác nhận. Sau khi thành công, các phiên cũ của tài khoản được thu hồi.",
            "Khóa hoặc Mở khóa: kiểm tra đúng người dùng, nhập lý do nếu cần, tích xác nhận và hoàn tất thay đổi trạng thái."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Tài khoản mới mặc định có phạm vi dữ liệu bản thân; chuyển sang Phạm vi dữ liệu để cấu hình tiếp. Mật khẩu ban đầu dài 12–128 ký tự, có chữ và số. Không có nút tự khóa tài khoản đang đăng nhập. Nhãn [MẪU] cho biết bản ghi minh họa và các số tổng hợp có thể bao gồm chúng."
        }
      ]
    },
    "overview": {
      "title": "Tổng quan hệ thống",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Theo dõi tình hình tài khoản và các thay đổi quản trị."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Tổng quan hệ thống.",
            "Đọc tổng tài khoản, tài khoản hoạt động và bị khóa trong phạm vi được cấp.",
            "Xem phân bố theo vai trò và các thao tác gần đây.",
            "Chuyển đến danh sách tài khoản hoặc nhật ký để xem chi tiết."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Tạo tài khoản: mở biểu mẫu cấp tài khoản.",
            "Danh sách tài khoản: tra cứu và cập nhật người dùng.",
            "Kiểm tra quyền: xem trước tình huống truy cập."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Một tài khoản có thể có nhiều vai trò; tổng theo vai trò có thể lớn hơn số tài khoản."
        }
      ]
    },
    "departments": {
      "title": "Quản lý đơn vị",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Quản lý danh mục khoa, phòng và ban."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Quản lý đơn vị.",
            "Tra cứu mã, tên và trạng thái đơn vị.",
            "Chọn nút thêm ở đầu trang để khai báo đơn vị theo biểu mẫu.",
            "Chọn Sửa để cập nhật thông tin hoặc trạng thái rồi lưu."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Sửa: thay đổi thông tin và trạng thái của đơn vị.",
            "Nút thêm đơn vị có thể mang nhãn + Thêm khoa trong giao diện hiện hành."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Đơn vị ngừng hoạt động vẫn được giữ để bảo toàn hồ sơ liên quan."
        }
      ]
    },
    "roles": {
      "title": "Vai trò và quyền",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Cấu hình quyền chức năng theo vai trò hoặc theo từng tài khoản."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Vai trò & quyền và xem ma trận chức năng.",
            "Chọn Quyền & cấu hình tại vai trò cần sửa.",
            "Chọn Cấu hình chung cho vai trò hoặc chọn một tài khoản cụ thể.",
            "Chọn quyền và phạm vi, tích xác nhận đã kiểm tra tác động rồi lưu."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Tạo vai trò: khai báo tên, mô tả, quyền và phạm vi.",
            "Khôi phục quyền và phạm vi theo cấu hình chung của vai trò: bỏ cấu hình riêng của tài khoản sau khi lưu."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Cấu hình riêng thay thế cấu hình của vai trò đang chọn đối với tài khoản đó; vai trò khác vẫn có thể cấp thêm quyền."
        }
      ]
    },
    "permissions": {
      "title": "Phạm vi dữ liệu",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Giới hạn dữ liệu được truy cập và hạn chế riêng của tài khoản."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Phạm vi dữ liệu.",
            "Tìm và lọc đúng tài khoản.",
            "Nhấn Cấu hình, chọn phạm vi dữ liệu; chọn danh sách đơn vị nếu dùng phạm vi các đơn vị được chọn.",
            "Đánh dấu quyền cần chặn thêm, tích xác nhận rồi lưu.",
            "Chuyển sang Kiểm tra quyền để đối chiếu kết quả."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Cấu hình: mở phạm vi và danh sách hạn chế riêng."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Bỏ chọn một hạn chế không cấp thêm quyền ngoài các vai trò đã gán."
        }
      ]
    },
    "simulator": {
      "title": "Kiểm tra quyền",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Xem trước kết quả truy cập theo tài khoản, chức năng và đơn vị dữ liệu."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Kiểm tra quyền.",
            "Chọn tài khoản cần kiểm tra; dùng bộ lọc hoặc chuyển trang để tìm nếu cần.",
            "Chọn chức năng và đơn vị của dữ liệu.",
            "Nhấn Kiểm tra quyền truy cập và đọc kết quả giải thích."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Kiểm tra quyền truy cập: đánh giá tình huống theo cấu hình hiện tại."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Phép thử không thay đổi hồ sơ nghiệp vụ. Máy chủ vẫn kiểm tra quyền khi thao tác thực tế."
        }
      ]
    },
    "history": {
      "title": "Nhật ký quản trị",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Đối chiếu người thao tác, thời điểm và nội dung thay đổi."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Nhật ký quản trị.",
            "Chọn các bộ lọc được cung cấp và tìm bản ghi cần tra cứu.",
            "Chuyển trang nếu có nhiều kết quả.",
            "Mở bản ghi để xem thông tin chi tiết trước và sau thay đổi."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Chi tiết bản ghi: xem nội dung thay đổi được lưu."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Khi hỗ trợ sự cố, ghi lại người dùng, thời điểm và mã hồ sơ để đối chiếu."
        }
      ]
    }
  },
  "index.html": {
    "overview": {
      "title": "Tổng quan",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Nắm lịch và tình hình đăng ký cá nhân."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Tổng quan.",
            "Đọc các chỉ tiêu và công việc cần xử lý.",
            "Mở phiếu hoặc liên kết lịch tương ứng để xem chi tiết."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Các liên kết công việc đưa đến trang nghiệp vụ tương ứng."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Số liệu phụ thuộc tài khoản và phạm vi hiện có."
        }
      ]
    },
    "calendar": {
      "title": "Lịch của tôi",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Tra cứu lịch sử dụng phòng đã được duyệt."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Lịch của tôi.",
            "Chọn ngày cần xem.",
            "Đối chiếu phòng, giờ bắt đầu, giờ kết thúc và số người.",
            "Mở lịch để xem phiếu liên quan."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Chọn ngày: đổi ngày xem lịch.",
            "Chi tiết: mở đăng ký tương ứng."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Phiếu nháp hoặc đang chờ duyệt chưa phải lịch sử dụng được chấp thuận."
        }
      ]
    },
    "rooms": {
      "title": "Đăng ký phòng",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Tìm phòng và gửi yêu cầu sử dụng."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Đăng ký phòng.",
            "Chọn ngày, thời gian và tiêu chí tìm phòng.",
            "Chọn phòng có sức chứa phù hợp, kiểm tra giờ trống.",
            "Nhập thông tin theo biểu mẫu rồi Lưu nháp hoặc gửi đăng ký.",
            "Theo dõi kết quả tại Phiếu của tôi."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Lưu nháp: lưu để bổ sung.",
            "Gửi đăng ký: chuyển yêu cầu đến người duyệt."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Nháp không giữ chỗ. Nếu báo trùng lịch, tải lại khả dụng và chọn thời gian hoặc phòng khác."
        }
      ]
    },
    "devices": {
      "title": "Đăng ký thiết bị",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Gửi yêu cầu mượn thiết bị theo thời gian và số lượng."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Đăng ký thiết bị.",
            "Tìm và chọn thiết bị.",
            "Nhập số lượng, thời gian và thông tin biểu mẫu.",
            "Kiểm tra khả dụng rồi lưu nháp hoặc gửi đăng ký.",
            "Theo dõi kết quả trong Phiếu của tôi."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Lưu nháp và gửi đăng ký có ý nghĩa tương tự trang đăng ký phòng."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Phiếu được duyệt chưa có nghĩa đã nhận thiết bị thực tế."
        }
      ]
    },
    "requests": {
      "title": "Phiếu của tôi",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Theo dõi đăng ký và tiếp tục xử lý phiếu cá nhân."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Phiếu của tôi.",
            "Lọc theo loại phiếu hoặc trạng thái.",
            "Mở phiếu để xem chi tiết và ý kiến xử lý.",
            "Sửa và gửi phiếu nháp; hủy khi nút được cho phép.",
            "Với bàn giao thiết bị, đối chiếu thực nhận trước khi xác nhận."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Chi tiết: xem nội dung và tiến trình.",
            "Sửa nháp: bổ sung trước khi gửi.",
            "Hủy: dừng phiếu khi trạng thái cho phép."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Kiểm tra trạng thái sau thao tác; không gửi lại liên tục khi chưa rõ kết quả."
        }
      ]
    },
    "returns": {
      "title": "Trả thiết bị",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Khai báo thiết bị trả và theo dõi việc tiếp nhận."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Trả thiết bị và chọn phiếu đang giữ thiết bị.",
            "Nhập số lượng trả lần này.",
            "Phân loại tốt, hỏng và mất đúng thực tế.",
            "Gửi yêu cầu trả và theo dõi đến khi quản lý tiếp nhận.",
            "Nếu trả một phần, tiếp tục theo dõi số lượng còn giữ."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Gửi yêu cầu trả: chuyển thông tin đến người quản lý."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Gửi yêu cầu chưa hoàn tất nhập kho; cần xác nhận tiếp nhận của quản lý."
        }
      ]
    }
  },
  "resources.html": {
    "overview": {
      "title": "Tổng quan",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Theo dõi tài nguyên và công việc vận hành."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Tổng quan trong không gian quản lý.",
            "Đọc thống kê trong phạm vi được cấp.",
            "Chọn công việc chờ xử lý để chuyển đến trang tương ứng."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Các liên kết tổng quan mở danh sách hoặc hồ sơ liên quan."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Kiểm tra bộ lọc và phạm vi trước khi so sánh số liệu."
        }
      ]
    },
    "rooms": {
      "title": "Danh mục phòng",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Khai báo và cập nhật thông tin phòng."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Danh mục phòng.",
            "Tìm phòng theo tiêu chí được cung cấp.",
            "Chọn Đăng ký phòng để khai báo một phòng mới.",
            "Điền mã, tên, đơn vị, sức chứa và thông tin biểu mẫu rồi lưu.",
            "Mở phòng và chọn Cập nhật khi cần sửa."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Đăng ký phòng: thêm hồ sơ danh mục.",
            "Cập nhật: sửa thông tin.",
            "Xóa: chuyển hồ sơ sang ngừng hoạt động theo cơ chế hiện hành."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Đây là khai báo tài nguyên. Hồ sơ còn nghiệp vụ chưa hoàn tất có thể bị chặn ngừng hoạt động; lọc hồ sơ ngừng hoạt động để phục hồi khi được phép."
        }
      ]
    },
    "calendar": {
      "title": "Lịch sử dụng phòng",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Tra cứu các khoảng sử dụng để tổ chức vận hành."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Lịch sử dụng phòng.",
            "Chọn ngày và bộ lọc phòng hoặc đơn vị được cung cấp.",
            "Đối chiếu thời gian, phòng và hồ sơ liên quan.",
            "Mở chi tiết trước khi xử lý yêu cầu."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Chọn ngày và bộ lọc: thay đổi phạm vi lịch hiển thị."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Đối chiếu trạng thái đăng ký; không coi yêu cầu chờ duyệt là lịch đã được duyệt."
        }
      ]
    },
    "approvals": {
      "title": "Duyệt đăng ký",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Xét yêu cầu sử dụng phòng và thiết bị."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Duyệt đăng ký và lọc hồ sơ chờ.",
            "Mở phiếu, kiểm tra người đăng ký, đơn vị, thời gian và số lượng.",
            "Đối chiếu khả dụng.",
            "Chọn duyệt hoặc từ chối, bổ sung ý kiến khi cần và xác nhận.",
            "Kiểm tra trạng thái kết quả."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Duyệt: chấp thuận yêu cầu.",
            "Từ chối: ghi nhận kết quả không chấp thuận."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Nếu hồ sơ đã thay đổi, tải lại và kiểm tra bản mới trước khi quyết định."
        }
      ]
    },
    "loans": {
      "title": "Mượn và trả thiết bị",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Bàn giao thiết bị và tiếp nhận yêu cầu trả."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Mượn / trả thiết bị.",
            "Chọn phiếu đã duyệt và đối chiếu số lượng thực giao.",
            "Ghi nhận bàn giao, theo dõi xác nhận của người nhận.",
            "Mở yêu cầu trả, kiểm tra số lượng tốt, hỏng và mất thực tế.",
            "Xác nhận tiếp nhận rồi kiểm tra tồn kho và phần còn giữ."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Bàn giao: ghi nhận giao thiết bị.",
            "Tiếp nhận trả: xác nhận thiết bị thực nhận về."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Không ghi thiết bị hỏng hoặc mất thành thiết bị tốt."
        }
      ]
    },
    "assets": {
      "title": "Hồ sơ tài sản",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Quản lý thông tin và tình trạng tài sản."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Hồ sơ tài sản.",
            "Tìm và mở tài sản cần xem.",
            "Chọn Đăng ký tài sản để khai báo tài sản mới.",
            "Nhập mã, tên, đơn vị, số lượng và thông tin biểu mẫu.",
            "Lưu và mở lại để kiểm tra; chọn Cập nhật khi cần sửa."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Đăng ký tài sản: thêm hồ sơ.",
            "Cập nhật: sửa thông tin.",
            "Xóa: ngừng hoạt động theo điều kiện hệ thống."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Kiểm tra đơn vị quản lý và tình trạng; không tạo hồ sơ trùng mã."
        }
      ]
    },
    "inventory": {
      "title": "Kiểm kê tài sản",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Đối chiếu tài sản theo hồ sơ với thực tế."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Kiểm kê tài sản.",
            "Chọn tài sản và đợt hoặc thông tin kiểm kê theo biểu mẫu.",
            "Đối chiếu số lượng hệ thống với số lượng thực tế.",
            "Ghi kết quả và sai lệch rồi lưu.",
            "Mở lại bản ghi để kiểm tra."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Thao tác ghi nhận kiểm kê lưu kết quả đối chiếu theo biểu mẫu."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Ghi đúng thực tế và giải thích sai lệch để người xử lý có căn cứ."
        }
      ]
    },
    "maintenance": {
      "title": "Bảo trì và sửa chữa",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Theo dõi tài sản đang bảo trì và kết quả hoàn thành."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Bảo trì & sửa chữa.",
            "Chọn tài sản, nhập số lượng và thông tin công việc.",
            "Lưu hồ sơ bảo trì.",
            "Khi hoàn thành, mở hồ sơ và cập nhật kết quả cùng số lượng phục hồi.",
            "Kiểm tra tình trạng và khả dụng sau xử lý."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Ghi nhận và cập nhật bảo trì theo các thao tác được cung cấp trên hồ sơ."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Chi phí bảo trì không tự đồng nghĩa đã tạo chứng từ thanh toán."
        }
      ]
    },
    "logs": {
      "title": "Xác nhận nhật ký",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Tra cứu và xử lý nhật ký sử dụng được gửi đến."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Xác nhận nhật ký nếu được cấp quyền.",
            "Chọn bản ghi cần xem.",
            "Đối chiếu người sử dụng, tài nguyên, thời gian và nội dung.",
            "Thực hiện thao tác xác nhận hoặc xử lý được hiển thị rồi kiểm tra trạng thái."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Mở bản ghi để xem nội dung và các thao tác được cấp quyền."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Chỉ xử lý nhật ký thực tế có trong danh sách; menu giảng viên hiện hành không có trang gửi nhật ký riêng."
        }
      ]
    },
    "lifecycle": {
      "title": "Nguyên giá và thanh lý",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Kê khai nguyên giá và theo dõi hồ sơ thanh lý."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Nguyên giá và thanh lý.",
            "Chọn tài sản cần kê khai hoặc lập hồ sơ thanh lý.",
            "Nhập nguyên giá hoặc số lượng và thông tin thanh lý theo biểu mẫu.",
            "Kiểm tra rồi lưu hoặc gửi duyệt theo bước được cung cấp.",
            "Theo dõi kết quả xử lý hồ sơ."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Các thao tác kê khai, gửi và duyệt chỉ xuất hiện theo quyền và trạng thái."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Nguyên giá chưa kê khai khác với bằng 0. Nguyên giá lịch sử không phải giá trị còn lại sau khấu hao."
        }
      ]
    },
    "reports": {
      "title": "Báo cáo quản trị",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Tra cứu số liệu phòng, tài sản và hoạt động nguồn lực."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Báo cáo quản trị.",
            "Chọn loại báo cáo, kỳ và đơn vị.",
            "Xem tổng hợp và chi tiết.",
            "Xuất định dạng được cung cấp nếu có quyền.",
            "Kiểm tra kỳ và phạm vi trong tệp tải xuống."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Xem báo cáo và xuất báo cáo phụ thuộc hai quyền riêng."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Không phải báo cáo nào cũng hỗ trợ tất cả định dạng."
        }
      ]
    }
  },
  "accounting.html": {
    "overview": {
      "title": "Tổng quan tài chính",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Theo dõi tình hình tài chính và hồ sơ cần xử lý."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Tổng quan tài chính.",
            "Kiểm tra năm và phạm vi số liệu.",
            "Đọc ngân sách, thu chi và công việc đang chờ.",
            "Chuyển đến hồ sơ hoặc sổ liên quan để kiểm tra."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Các liên kết tổng quan mở chức năng nghiệp vụ tương ứng."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Phân biệt ngân sách được duyệt, khoản thu và khoản chi thực hiện."
        }
      ]
    },
    "estimates": {
      "title": "Lập dự toán",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Lập hồ sơ đề nghị ngân sách và ghi nhận kinh phí được nhận."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Lập dự toán.",
            "Chọn năm tài chính, đơn vị và khoản mục.",
            "Nhập các dòng nội dung và số tiền, kiểm tra tổng.",
            "Lưu nháp hoặc gửi duyệt rồi theo dõi kết quả.",
            "Khi dự toán đã duyệt và thực nhận kinh phí, chọn Xác nhận nhận tiền; nhập số phiếu thu và ngày thực nhận."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Lưu nháp: lưu để sửa tiếp.",
            "Gửi duyệt: trình người có thẩm quyền.",
            "Xác nhận nhận tiền: tạo khoản thu bằng toàn bộ số tiền dự toán theo cơ chế hiện hành."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Ngày nhận thuộc năm tài chính dự toán. Ghi nhận nhận tiền không cộng ngân sách lần nữa; tránh lập thêm khoản thu trùng."
        }
      ]
    },
    "transactions": {
      "title": "Sổ thu chi",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Tra cứu giao dịch và chứng từ thu chi."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Sổ thu – chi.",
            "Chọn kỳ, đơn vị và các bộ lọc được cung cấp.",
            "Mở giao dịch để kiểm tra số chứng từ, ngày và số tiền.",
            "Khi có quyền ghi thu chi, nhập thông tin theo biểu mẫu và lưu.",
            "Mở chứng từ để quản lý tệp hoặc đảo chứng từ khi được phép."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Chứng từ: xem chi tiết giao dịch.",
            "Đảo chứng từ: sửa tác động ghi sổ bằng nghiệp vụ đảo, giữ bản gốc để truy vết."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Khoản thu không tự làm tăng ngân sách đã duyệt. Không tạo giao dịch khác chỉ để tránh thông báo trùng số chứng từ."
        }
      ]
    },
    "queue": {
      "title": "Yêu cầu chi chờ xử lý",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Theo dõi và thanh toán yêu cầu chi theo thẩm quyền."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Yêu cầu chi chờ xử lý.",
            "Chọn hồ sơ và kiểm tra năm, đơn vị, khoản mục, số tiền.",
            "Đối chiếu ngân sách và chứng từ.",
            "Nhập số tiền lần thanh toán cùng thông tin bắt buộc nếu được phép thanh toán.",
            "Lưu rồi kiểm tra phần đã thanh toán và phần còn lại."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Thao tác xử lý và thanh toán xuất hiện theo trạng thái và quyền."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Yêu cầu đang chờ chưa phải khoản đã chi. Khi thanh toán từng phần, nhập số tiền của lần hiện tại."
        }
      ]
    },
    "budgets": {
      "title": "Theo dõi ngân sách",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Theo dõi ngân sách, điều chỉnh và tình trạng sổ năm."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Theo dõi ngân sách.",
            "Chọn năm và đơn vị.",
            "Đối chiếu số được duyệt, đã chi và còn khả dụng.",
            "Trong khu vực Điều chỉnh và khóa sổ, lập đề nghị điều chỉnh khi cần.",
            "Chỉ khóa hoặc mở sổ đúng năm và theo quyền được giao."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Điều chỉnh và khóa sổ: mở nghiệp vụ điều chỉnh ngân sách và quản lý năm tài chính khi có quyền."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Điều chỉnh phải được duyệt mới tác động ngân sách; năm đã khóa hạn chế ghi giao dịch."
        }
      ]
    },
    "history": {
      "title": "Lịch sử thao tác",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Tra cứu hoạt động tài chính đã được ghi nhận."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Lịch sử thao tác.",
            "Lọc theo tiêu chí trang cung cấp.",
            "Chọn bản ghi để xem người thực hiện, thời điểm và nội dung.",
            "Đối chiếu chứng từ hoặc hồ sơ gốc khi cần."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Chi tiết: xem bản ghi hoặc hồ sơ liên quan khi có liên kết."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Lịch sử hỗ trợ tra cứu; sửa tác động tài chính phải qua nghiệp vụ tương ứng."
        }
      ]
    },
    "reports": {
      "title": "Báo cáo tài chính",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Xem và xuất số liệu tài chính theo kỳ."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Báo cáo tài chính.",
            "Chọn loại báo cáo, năm và đơn vị.",
            "Đối chiếu số tổng hợp với chi tiết.",
            "Chọn xuất hoặc in nếu được cấp quyền.",
            "Kiểm tra tệp hoặc bản in trước khi sử dụng."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "CSV, Excel hoặc In / Lưu PDF chỉ dùng khi được màn hình cung cấp."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Đọc số liệu cùng năm và phạm vi đang chọn."
        }
      ]
    }
  },
  "executive.html": {
    "overview": {
      "title": "Tổng quan điều hành",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Nắm tình hình ngân sách và hồ sơ cần quyết định."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Tổng quan điều hành.",
            "Đọc ngân sách được duyệt, đã chi và còn khả dụng.",
            "Xem hồ sơ đang chờ và tình hình theo đơn vị.",
            "Chọn Xét duyệt dự toán hoặc báo cáo để xem chi tiết."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Xét duyệt dự toán: mở danh sách hồ sơ.",
            "Xem báo cáo: mở số liệu tổng hợp."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Dự toán chờ duyệt và yêu cầu chi chờ xử lý chưa được tính là khoản chi thực hiện."
        }
      ]
    },
    "approvals": {
      "title": "Duyệt dự toán",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Xem hồ sơ và đưa ra quyết định xét duyệt."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Duyệt dự toán, chọn nhóm Chờ xét duyệt.",
            "Lọc theo năm và đơn vị.",
            "Mở hồ sơ, đọc các khoản mục và số tiền.",
            "Chọn phê duyệt hoặc từ chối, bổ sung ý kiến rồi xác nhận.",
            "Kiểm tra kết quả trong hồ sơ hoặc Lịch sử xét duyệt."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Chờ xét duyệt, Đã phê duyệt và Đã từ chối: đổi nhóm hồ sơ cần xem."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Nếu hệ thống yêu cầu người duyệt khác người lập, chuyển đến tài khoản có thẩm quyền phù hợp."
        }
      ]
    },
    "budgets": {
      "title": "Theo dõi ngân sách",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: So sánh tình hình ngân sách theo đơn vị và năm."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Theo dõi ngân sách.",
            "Chọn năm và đơn vị.",
            "Đọc số được duyệt, đã chi và khả dụng.",
            "Đối chiếu biểu đồ và số liệu chi tiết.",
            "Chuyển sang báo cáo để xem thêm khi cần."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Bộ lọc năm và đơn vị thay đổi phạm vi hiển thị."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Khoản thu chi được ghi nhận trong không gian kế toán; trang này phục vụ theo dõi."
        }
      ]
    },
    "history": {
      "title": "Lịch sử xét duyệt",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Tra cứu kết quả và ý kiến quyết định."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Lịch sử xét duyệt.",
            "Chọn tất cả kết quả, đã phê duyệt hoặc đã từ chối.",
            "Lọc năm và đơn vị.",
            "Mở hồ sơ để xem quyết định, người xử lý và thời điểm."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Các nhóm kết quả giúp thu hẹp danh sách cần tra cứu."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Đối chiếu đúng mã hồ sơ và lần quyết định khi xem lịch sử."
        }
      ]
    },
    "reports": {
      "title": "Báo cáo tổng hợp",
      "blocks": [
        {
          "type": "p",
          "text": "Mục đích: Xem số liệu điều hành trong kỳ và phạm vi lựa chọn."
        },
        {
          "type": "h3",
          "text": "Cách sử dụng"
        },
        {
          "type": "ol",
          "items": [
            "Mở Báo cáo tổng hợp.",
            "Chọn loại báo cáo, kỳ và đơn vị.",
            "Đọc bảng số liệu và tổng cộng.",
            "Chọn xuất CSV hoặc in khi màn hình cung cấp và tài khoản có quyền.",
            "Nếu dùng hộp thoại in, chọn máy in hoặc Lưu dưới dạng PDF."
          ]
        },
        {
          "type": "h3",
          "text": "Chức năng chính"
        },
        {
          "type": "ul",
          "items": [
            "Xuất CSV: tải bảng dữ liệu.",
            "In / Lưu PDF: mở chức năng in khi được cung cấp."
          ]
        },
        {
          "type": "p",
          "text": "Điểm cần kiểm tra: Kiểm tra kỳ và phạm vi trong kết quả; dữ liệu có nhãn mẫu cần được phân biệt khi sử dụng."
        }
      ]
    }
  }
};
