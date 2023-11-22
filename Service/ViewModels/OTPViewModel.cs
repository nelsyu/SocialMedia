namespace Service.ViewModels
{
    public class OTPViewModel
    {
        public byte[] OTPQRCode { get; set; } = null!;
        public string? OTPQRCodeSK { get; set; }
    }
}
