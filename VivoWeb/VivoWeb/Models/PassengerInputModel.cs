using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VivoWeb.Models
{
    public class PassengerInputModel
    {
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Nationality { get; set; }
        public string Note { get; set; }

        public string PassengerType { get; set; } // "Người lớn", "Trẻ em", "Em bé"
    }
}