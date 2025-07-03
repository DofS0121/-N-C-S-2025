using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VivoWeb.Models
{
    public class PaymentViewModel
    {
        public Booking Booking { get; set; }
        public List<PassengerInputModel> Passengers { get; set; }

        public int Adults => Passengers.Count(p => p.PassengerType == "Người lớn");
        public int Children => Passengers.Count(p => p.PassengerType == "Trẻ em");
        public int Infants => Passengers.Count(p => p.PassengerType == "Em bé");

        public decimal AdultPrice { get; set; }
        public decimal ChildPrice { get; set; }
        public decimal InfantPrice { get; set; }

        public string PaymentMethod { get; set; } // Cash / BankTransfer
        public string PaymentRate { get; set; } // "100%" / "50%"

        public decimal TotalAmount =>
            Adults * AdultPrice +
            Children * ChildPrice +
            Infants * InfantPrice;

    }
}