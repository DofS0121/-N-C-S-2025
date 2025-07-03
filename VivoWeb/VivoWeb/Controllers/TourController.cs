using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VivoWeb.Models;
using Newtonsoft.Json;
namespace VivoWeb.Controllers
{
    public class TourController : Controller
    {
        dbVivoWebDataContext db = new dbVivoWebDataContext();

        // GET: Tour
        public ActionResult Index()
        {
            var tours = db.Tours.ToList();
            ViewBag.TourCategories = db.TourCategories.ToList();

            // Lấy danh sách location với các trường cần thiết
            var locations = db.Locations
                .Select(l => new
                {
                    l.LocationId,
                    l.Name,
                    l.ImageUrl,
                    l.CategoryId
                }).ToList();

            // Chuyển sang JSON để truyền xuống View
            ViewBag.LocationsJson = JsonConvert.SerializeObject(locations);

            return View(tours);
        }
        
    }
}