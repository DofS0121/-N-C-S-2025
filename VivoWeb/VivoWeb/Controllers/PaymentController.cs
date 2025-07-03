using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VivoWeb.Models;

namespace VivoWeb.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult Index()
        {
            return View();
        }

        dbVivoWebDataContext db = new dbVivoWebDataContext();

        [HttpGet]
        public ActionResult Confirm()
        {
            if (TempData["Booking"] == null || TempData["Passengers"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var booking = TempData["Booking"] as Booking;
            var passengers = TempData["Passengers"] as List<PassengerInputModel>;

            // Giá giả định theo loại hành khách
            var model = new PaymentViewModel
            {
                Booking = booking,
                Passengers = passengers,
                AdultPrice = booking.Total / booking.NumberOfPeople, // hoặc giá thực tế
                ChildPrice = (booking.Total / booking.NumberOfPeople) * 0.75m,
                InfantPrice = (booking.Total / booking.NumberOfPeople) * 0.5m
            };

            TempData.Keep(); // giữ lại sau redirect

            return View(model);
        }

        [HttpPost]
        public ActionResult Confirm(PaymentViewModel model)
        {
            // Lấy lại dữ liệu gốc từ TempData
            var booking = TempData["Booking"] as Booking;
            var passengers = TempData["Passengers"] as List<PassengerInputModel>;

            // Tính lại tiền theo tỉ lệ thanh toán
            var total = model.TotalAmount;
            if (model.PaymentRate == "50%")
            {
                total /= 2;
            }

            // Lưu dữ liệu vào DB
            booking.Total = total;
            db.Bookings.InsertOnSubmit(booking);
            db.SubmitChanges();

            // Có thể lưu Passengers nếu cần (nếu bạn có bảng BookingPassengers)

            ViewBag.Message = "Thanh toán thành công!";
            return RedirectToAction("Success");
        }
    }
}