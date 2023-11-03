using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.ViewModels
{
    public class OTPViewModel
    {
        public byte[] OTPQRCode { get; set; } = null!;
        public string? OTPQRCodeSK { get; set; }
    }
}
