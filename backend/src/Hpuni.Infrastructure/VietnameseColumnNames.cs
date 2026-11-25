using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Hpuni.Infrastructure;

// Physical SQL names only: C# properties and API contracts remain stable.
public static class VietnameseColumnNames
{
    public static readonly IReadOnlyDictionary<string, string> Names = new Dictionary<string, string>
    {
        ["AcceptanceNote"] = "GhiChuNghiemThu",
        ["AcceptedAt"] = "NgayNghiemThu",
        ["AcceptedBy"] = "MaNguoiNghiemThu",
        ["AcceptedDamaged"] = "SoLuongHongNghiemThu",
        ["AcceptedGood"] = "SoLuongTotNghiemThu",
        ["AcceptedLost"] = "SoLuongMatNghiemThu",
        ["AccessFailedCount"] = "SoLanDangNhapSai",
        ["AcquisitionDate"] = "NgayMua",
        ["AcquisitionValue"] = "GiaTriMua",
        ["Action"] = "HanhDong",
        ["Active"] = "HoatDong",
        ["ActorId"] = "MaNguoiThucHien",
        ["Actual"] = "SoLuongThucTe",
        ["ActualEnd"] = "KetThucThucTe",
        ["ActualStart"] = "BatDauThucTe",
        ["Amount"] = "SoTien",
        ["AssetGroupId"] = "MaNhomTaiSan",
        ["AssetVersion"] = "PhienBanTaiSan",
        ["At"] = "ThoiDiem",
        ["AttachmentId"] = "MaTepDinhKem",
        ["Attendees"] = "SoNguoiThamDu",
        ["BuiltIn"] = "MacDinhHeThong",
        ["Capacity"] = "SucChua",
        ["Category"] = "DanhMuc",
        ["ChangedAt"] = "NgayThayDoi",
        ["ClaimType"] = "LoaiThuocTinh",
        ["ClaimValue"] = "GiaTriThuocTinh",
        ["Closed"] = "DaKhoa",
        ["Code"] = "MaSo",
        ["ConcurrencyStamp"] = "DauDongThoi",
        ["Condition"] = "TinhTrang",
        ["Content"] = "NoiDung",
        ["ContentType"] = "LoaiNoiDung",
        ["Cost"] = "ChiPhi",
        ["CreatedAt"] = "NgayTao",
        ["Damaged"] = "SoLuongHong",
        ["Date"] = "Ngay",
        ["DepartmentId"] = "MaDonVi",
        ["DetailJson"] = "ChiTietJson",
        ["DisplayName"] = "TenHienThi",
        ["Email"] = "ThuDienTu",
        ["EmailConfirmed"] = "DaXacNhanThuDienTu",
        ["End"] = "KetThuc",
        ["EntryId"] = "MaChungTu",
        ["Expected"] = "SoLuongDuKien",
        ["ExpectedDamaged"] = "SoLuongHongDuKien",
        ["ExpectedLost"] = "SoLuongMatDuKien",
        ["ExpiresAt"] = "NgayHetHan",
        ["FileName"] = "TenTep",
        ["Good"] = "SoLuongTot",
        ["HandedAt"] = "NgayBanGiao",
        ["Id"] = "Ma",
        ["Issue"] = "VanDe",
        ["Key"] = "Khoa",
        ["Kind"] = "PhanLoai",
        ["LinesJson"] = "DongChiTietJson",
        ["LoanId"] = "MaPhieuMuon",
        ["LockoutEnabled"] = "ChoPhepKhoaTaiKhoan",
        ["LockoutEnd"] = "KhoaDen",
        ["LoginProvider"] = "NhaCungCapDangNhap",
        ["Lost"] = "SoLuongMat",
        ["MaintenanceId"] = "MaPhieuBaoTri",
        ["MigrationId"] = "MaCapNhat",
        ["Mode"] = "CheDo",
        ["Name"] = "Ten",
        ["NormalizedEmail"] = "ThuDienTuChuanHoa",
        ["NormalizedName"] = "TenChuanHoa",
        ["NormalizedUserName"] = "TenDangNhapChuanHoa",
        ["NormalizedVoucher"] = "SoChungTuChuanHoa",
        ["Note"] = "GhiChu",
        ["Notes"] = "GhiChuChung",
        ["Operation"] = "ThaoTac",
        ["OwnerId"] = "MaNguoiSoHuu",
        ["PasswordHash"] = "MaBamMatKhau",
        ["PayloadHash"] = "MaBamNoiDung",
        ["PaymentRequestId"] = "MaDeNghiThanhToan",
        ["FundingProposalId"] = "MaDuToanNhanKinhPhi",
        ["Permission"] = "Quyen",
        ["PermissionsJson"] = "DanhSachQuyenJson",
        ["PhoneNumber"] = "SoDienThoai",
        ["PhoneNumberConfirmed"] = "DaXacNhanSoDienThoai",
        ["ProductVersion"] = "PhienBanSanPham",
        ["ProposalId"] = "MaDeXuat",
        ["ProviderDisplayName"] = "TenHienThiNhaCungCap",
        ["ProviderKey"] = "KhoaNhaCungCap",
        ["Purpose"] = "MucDich",
        ["Quantity"] = "SoLuong",
        ["ReadAt"] = "NgayDoc",
        ["Reason"] = "LyDo",
        ["Reference"] = "SoThamChieu",
        ["ReportedReceived"] = "SoLuongBaoDaNhan",
        ["RequestId"] = "MaPhieuDangKy",
        ["Result"] = "KetQua",
        ["ResultJson"] = "KetQuaJson",
        ["Returned"] = "SoLuongDaTra",
        ["ReviewNote"] = "GhiChuDuyet",
        ["ReviewedAt"] = "NgayDuyet",
        ["ReviewedBy"] = "MaNguoiXetDuyet",
        ["ReviewerId"] = "MaNguoiDuyet",
        ["RevokedAt"] = "NgayThuHoi",
        ["RoleId"] = "MaVaiTro",
        ["RoomId"] = "MaPhong",
        ["RowVersion"] = "PhienBanDong",
        ["Scope"] = "PhamVi",
        ["SecurityStamp"] = "DauBaoMat",
        ["SessionId"] = "MaDotKiemKe",
        ["Sha256"] = "MaBamSha256",
        ["Size"] = "KichThuoc",
        ["Source"] = "NguonGoc",
        ["SourceKey"] = "KhoaNguon",
        ["SourceVersion"] = "PhienBanNguon",
        ["Start"] = "BatDau",
        ["Status"] = "TrangThai",
        ["TargetId"] = "MaDoiTuong",
        ["Title"] = "TieuDe",
        ["Total"] = "TongSo",
        ["TwoFactorEnabled"] = "BatXacThucHaiYeuTo",
        ["Type"] = "Loai",
        ["Unit"] = "DonViTinh",
        ["UpdatedAt"] = "NgayCapNhat",
        ["Used"] = "DaSuDung",
        ["UserId"] = "MaNguoiDung",
        ["UserName"] = "TenDangNhap",
        ["Value"] = "GiaTri",
        ["Voucher"] = "SoChungTu",
        ["Year"] = "Nam",
    };

    public static string ColumnName(string table, string property)
    {
        if (property == "Id") return table switch
        {
            "NguoiDung" => "MaNguoiDung", "VaiTro" => "MaVaiTro",
            "ChungTuTaiChinh" => "MaChungTu",
            "PhieuDangKyPhong" => "MaPhieuDangKy", "PhieuMuonThietBi" => "MaPhieuMuon",
            "DeXuatNganSach" => "MaDeXuat", "TepDinhKemChungTu" => "MaTepDinhKem",
            _ => "Ma" + table
        };
        return (table, property) switch
        {
            ("NguoiDung", "DisplayName") => "TenNguoiDung",
            ("NguoiDung", "Email") => "DiaChiEmail",
            ("NguoiDung", "PasswordHash") => "MatKhau",
            ("NguoiDung", "DateOfBirth") => "NgayThangNamSinh",
            ("NguoiDung", "DepartmentName") => "DonVi",
            ("VaiTro", "Name") => "TenVaiTro",
            ("PhienDangNhap", "ExpiresAt") => "NgayHetPhien",
            ("NhatKyHeThong", "At") => "ThoiDien",
            ("NhatKyHeThong", "DetailJson") => "ChiTiet",
            (_, "Code") => "MaSo" + table,
            _ => Names.TryGetValue(property, out var name) ? name
                : throw new InvalidOperationException($"Missing Vietnamese SQL name: {table}.{property}")
        };
    }

    public static string TranslateSql(string sql) => Regex.Replace(sql, @"\[([A-Za-z0-9_]+)\]",
        match => Names.TryGetValue(match.Groups[1].Value, out var name) ? $"[{name}]" : match.Value);

    public static void Apply(ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
        {
            // Identity may have transient convention entities removed at model finalization.
            if (entity.GetTableName() is not { } table || !ActorTableSchemas.Tables.ContainsKey(table)) continue;
            var columns = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var property in entity.GetProperties())
            {
                var name = ColumnName(table, property.Name);
                if (!columns.Add(name)) throw new InvalidOperationException($"Duplicate Vietnamese SQL column: {entity.Name}.{name}");
                property.SetColumnName(name);
            }
            foreach (var index in entity.GetIndexes())
                if (index.GetFilter() is { } filter)
                    index.SetFilter(Regex.Replace(filter, @"\[([A-Za-z0-9_]+)\]", match =>
                    {
                        var prop = entity.GetProperties().FirstOrDefault(p => p.Name == match.Groups[1].Value ||
                            Names.TryGetValue(p.Name, out var original) && original == match.Groups[1].Value);
                        return prop == null ? match.Value : $"[{ColumnName(table, prop.Name)}]";
                    }));
        }
    }
}
