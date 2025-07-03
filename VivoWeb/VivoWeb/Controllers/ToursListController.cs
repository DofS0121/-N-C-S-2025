using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VivoWeb.Models;
using PagedList;

namespace VivoWeb.Controllers
{
    public class ToursListController : Controller
    {
        dbVivoWebDataContext db = new dbVivoWebDataContext();

        // ToursListController.cs
        public ActionResult List(int? locationId, string searchTerm, int? page)
        {
            int pageSize = 10;
            int pageNumber = page ?? 1;

            var tours = db.Tours.AsQueryable();

            if (locationId.HasValue)
            {
                tours = tours.Where(t => t.LocationId == locationId);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                tours = tours.Where(t => t.Name.Contains(searchTerm));
            }

            // ✅ Lấy danh sách địa điểm và phân loại
            var foreignLocations = db.Locations
                .Where(l => l.CategoryId == 2)
                .OrderBy(l => l.Name)
                .ToList();

            var domesticLocations = db.Locations
                .Where(l => l.CategoryId == 1)
                .OrderBy(l => l.Name)
                .ToList();

            ViewBag.ForeignLocations = foreignLocations;
            ViewBag.DomesticLocations = domesticLocations;

            return View(tours.ToPagedList(pageNumber, pageSize));
        }
        public ActionResult Detail(int id)
        {
             var tour = db.Tours.FirstOrDefault(t => t.TourId == id);

            if (tour == null)
                return HttpNotFound();

            return View(tour);
        }


    }
}


