using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VivoWeb.Models
{
	public class TourDetailViewModel
	{
        public int TourId { get; set; }
        public string TourName { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public decimal Price { get; set; }
        public int LocationId { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime ReturnDate { get; set; }
        public int AvailableSlots { get; set; }
        public int SoldSlots { get; set; }
        public string CategoryName { get; set; }
        public DateTime CreatedAt { get; set; }

        public double AverageRating { get; set; }
        public List<ReviewViewModel> Reviews { get; set; }
    }
}