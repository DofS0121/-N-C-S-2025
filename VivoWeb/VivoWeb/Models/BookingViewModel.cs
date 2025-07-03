using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VivoWeb.Models
{
    public class BookingViewModel
    {
        public int TourId { get; set; }
        public decimal Price { get; set; }
        public List<TourSchedule> Schedules { get; set; }

        // Thông tin khách hàng
        public string CustomerFullName { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerEmail { get; set; }
        public string CustomerAddress { get; set; }

        // Danh sách hành khách (dùng để binding từ form nếu muốn)
        public List<PassengerInputModel> Passengers { get; set; }
    }
}