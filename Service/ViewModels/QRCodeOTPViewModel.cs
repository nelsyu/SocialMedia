using Data.Entities;

namespace Service.ViewModels
{
    public partial class QRCodeOTPViewModel
    {
        public int UserId { get; set; } = 0;
        public string QRCodeOTP { get; set; } = null!;
    }
}
