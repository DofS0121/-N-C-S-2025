using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VivoWeb.Models
{
	public class ProfileViewModel
	{
        public string CustomerName { get; set; }
        public string NumberPhone { get; set; }
        public string Email { get; set; }
        //public string Address { get; set; } // Có thể để trống hoặc ẩn nếu chưa dùng
        public string Avatar { get; set; }
    }
}