using Microsoft.EntityFrameworkCore;

namespace Hpuni.Infrastructure;

// Presentation/ownership groups only. Authorization continues to use permission grants.
public static class ActorTableSchemas
{
    public const string HistorySchema = "QuanTriVien";
    public static readonly IReadOnlyDictionary<string, string> Tables = new Dictionary<string, string>
    {
        ["NguoiDung"]="QuanTriVien", ["VaiTro"]="QuanTriVien", ["VaiTroNguoiDung"]="QuanTriVien",
        ["ThuocTinhNguoiDung"]="QuanTriVien", ["ThuocTinhVaiTro"]="QuanTriVien",
        ["DangNhapNgoai"]="QuanTriVien", ["MaXacThucNguoiDung"]="QuanTriVien",
        ["DonVi"]="QuanTriVien", ["QuyenVaiTro"]="QuanTriVien", ["TuyChinhVaiTroNguoiDung"]="QuanTriVien",
        ["PhamViDonViNguoiDung"]="QuanTriVien", ["QuyenBiTuChoiNguoiDung"]="QuanTriVien",
        ["PhienDangNhap"]="QuanTriVien", ["NhatKyHeThong"]="QuanTriVien",
        ["YeuCauChongLap"]="QuanTriVien", ["TrangThaiDocThongBao"]="QuanTriVien",
        ["LichSuCapNhatCauTruc"]="QuanTriVien",
        ["DeXuatNganSach"]="BanGiamHieu", ["PhanBoNganSach"]="BanGiamHieu", ["DieuChinhNganSach"]="BanGiamHieu",
        ["KyKeToan"]="KeToan", ["DeNghiThanhToan"]="KeToan", ["ChungTuTaiChinh"]="KeToan",
        ["ButToanDao"]="KeToan", ["TepDinhKemChungTu"]="KeToan", ["NoiDungTepChungTu"]="KeToan",
        ["Phong"]="QuanLyTaiSan", ["LichDatPhong"]="QuanLyTaiSan", ["NhomTaiSan"]="QuanLyTaiSan",
        ["BienDongThietBi"]="QuanLyTaiSan", ["PhieuBaoTri"]="QuanLyTaiSan", ["DotKiemKe"]="QuanLyTaiSan",
        ["ChiTietKiemKe"]="QuanLyTaiSan", ["HoSoTaiSanCoDinh"]="QuanLyTaiSan", ["PhieuThanhLyTaiSan"]="QuanLyTaiSan",
        ["PhieuDangKyPhong"]="GiangVien", ["NhatKySuDungPhong"]="GiangVien",
        ["PhieuMuonThietBi"]="GiangVien", ["PhieuTraThietBi"]="GiangVien", ["NhatKySuDungThietBi"]="GiangVien"
    };

    public static void Apply(ModelBuilder builder)
    {
        foreach (var entity in builder.Model.GetEntityTypes())
            if (entity.GetTableName() is { } name && Tables.TryGetValue(name, out var schema))
                entity.SetSchema(schema);
    }
}
