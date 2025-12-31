
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Features.Member.DTOs
{
    public class UpdateProfileRequest
    {
        public string AdSoyad { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}
