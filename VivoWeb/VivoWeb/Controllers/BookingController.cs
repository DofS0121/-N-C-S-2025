using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using VivoWeb.Models;

public class BookingController : Controller
{
    dbVivoWebDataContext db = new dbVivoWebDataContext();

    // GET: /Booking/Booking/1
    public ActionResult Booking(int tourId)
    {
        var tour = db.Tours.SingleOrDefault(t => t.TourId == tourId);
        if (tour == null) return HttpNotFound();

        var schedules = db.TourSchedules
            .Where(s => s.TourId == tourId && s.StartDate >= DateTime.Today && s.AvailableSeats > 0)
            .OrderBy(s => s.StartDate)
            .ToList();

        var viewModel = new BookingViewModel
        {
            TourId = tourId,
            Price = tour.Price,
            Schedules = schedules
        };

        return View(viewModel);
    }

    // POST: /Booking/Submit
    [HttpPost]
    public ActionResult Submit(Booking booking, List<PassengerInputModel> Passengers)
    {
        if (Session["UserId"] == null)
        {
            TempData["Error"] = "Bạn cần đăng nhập để đặt tour.";
            return RedirectToAction("Login", "Account");
        }

        int userId = (int)Session["UserId"];

        var schedule = db.TourSchedules.SingleOrDefault(s => s.ScheduleId == booking.ScheduleId);
        if (schedule == null) return HttpNotFound();

        var tour = db.Tours.SingleOrDefault(t => t.TourId == schedule.TourId);
        if (tour == null) return HttpNotFound();

        int totalPeople = Passengers?.Count ?? 0;
        if (totalPeople == 0)
        {
            ModelState.AddModelError("", "Vui lòng chọn ít nhất 1 hành khách.");
            return RedirectToAction("Booking", new { tourId = tour.TourId });
        }

        if (totalPeople > schedule.AvailableSeats)
        {
            ModelState.AddModelError("", "Không đủ chỗ trống cho lịch trình đã chọn.");
            return RedirectToAction("Booking", new { tourId = tour.TourId });
        }

        // Tính tổng tiền
        decimal total = 0;
        foreach (var p in Passengers)
        {
            if (p.PassengerType == "Người lớn") total += tour.Price;
            else if (p.PassengerType == "Trẻ em") total += tour.Price * 0.6m;
            else if (p.PassengerType == "Trẻ nhỏ") total += tour.Price * 0.1m;
        }

        // Tạo booking
        booking.UserId = userId;
        booking.BookingDate = DateTime.Now;
        booking.Status = "Chờ xác nhận";
        booking.TravelDate = schedule.StartDate;
        booking.NumberOfPeople = totalPeople;
        booking.Total = total;

        db.Bookings.InsertOnSubmit(booking);
        db.SubmitChanges(); // Lưu để có BookingId

        // Lưu hành khách
        foreach (var p in Passengers)
        {
            var passenger = new BookingPassenger
            {
                BookingId = booking.BookingId,
                FullName = p.FullName,
                Gender = p.Gender,
                DateOfBirth = p.DateOfBirth,
                PassengerType = p.PassengerType,
                Nationality = p.Nationality,
                Note = p.Note
            };
            db.BookingPassengers.InsertOnSubmit(passenger);
        }

        // Cập nhật số chỗ trống
        schedule.AvailableSeats -= totalPeople;

        db.SubmitChanges();

        return RedirectToAction("Success");
    }

    public ActionResult Success()
    {
        return View();
    }
}
