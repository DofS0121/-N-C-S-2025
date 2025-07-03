using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VivoWeb.Models
{
	public class ReviewViewModel
	{
        public string UserName { get; set; }
        public int Rating { get; set; }
        public string ReviewText { get; set; }
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}