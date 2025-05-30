namespace PhapClinicX.Models.ViewModels
{
    public class SanPhamDaBanViewModel
    {
        public DateTime NgayBan { get; set; }
        public string TenSanPham { get; set; } = string.Empty;
        public string TenPhongKham { get; set; } = string.Empty;
        public int SoLuong { get; set; } = 0;
        public decimal GiaGoc { get; set; } = 0;
        public decimal GiaBan { get; set; } = 0;
        public decimal TongTien { get; set; } = 0;
        public string? Image { get; set; }
    }


}
