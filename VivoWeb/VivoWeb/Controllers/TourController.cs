using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VivoWeb.Models; // Assuming VivoDataContext and Tour models are defined in this namespace

namespace VivoWeb.Controllers
{
    public class TourController : Controller
    {
        VivoTourWebDataContext db = new VivoTourWebDataContext();
        // GET: Tour
        public ActionResult Index()
        {
            var tours = db.Tours.ToList();
            return View(tours);
        }

        // Đăng ký
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(FormCollection collection, HttpPostedFileBase avatar)
        {
            try
            {
                var username = collection["username"];
                var password = collection["password"];
                var role = "Customer";  // Mặc định role là Customer
                var createdAt = DateTime.Now;

                // Kiểm tra mật khẩu có được nhập không
                if (string.IsNullOrEmpty(password))
                {
                    ViewBag.EnterPassword = "Vui lòng nhập mật khẩu!";
                    return View();
                }

                // Kiểm tra xác nhận mật khẩu có khớp không
                if (!password.Equals(password))
                {
                    ViewBag.SamePassword = "Mật khẩu và xác nhận mật khẩu phải giống nhau!";
                    return View();
                }

                // Kiểm tra xem email hoặc số điện thoại đã tồn tại chưa trong bảng Customers
                var existingCustomer = db.Customers.FirstOrDefault(p => p.email == collection["email"] || p.numberphone == collection["numberphone"]);
                if (existingCustomer != null)
                {
                    if (existingCustomer.email == collection["email"])
                    {
                        ViewBag.EmailExists = "Email này đã được sử dụng. Vui lòng nhập email khác.";
                    }    
                    if (existingCustomer.numberphone == collection["numberphone"])
                    {
                        ViewBag.PhoneExists = "Số điện thoại này đã được sử dụng. Vui lòng nhập số khác.";
                    }
                    return View();
                }

                // Xử lý upload avatar (giữ nguyên hàm ProcessUpload, trả về đường dẫn string)
                string avatarPath = ProcessUpload(avatar);

                // Tạo user mới
                User u = new User
                {
                    username = username,
                    password = password,
                    role = role,
                    createdAt = createdAt,
                    image = avatarPath
                };

                db.Users.InsertOnSubmit(u);
                db.SubmitChanges();

                // Tạo customer liên kết user mới
                Customer c = new Customer
                {
                    customer_name = collection["customer_name"],
                    email = collection["email"],
                    address = collection["address"],
                    numberphone = collection["numberphone"],
                    dob = DateTime.Parse(collection["dob"]),
                    user_id = u.user_id
                };

                db.Customers.InsertOnSubmit(c);
                db.SubmitChanges();

                // Sau khi đăng ký thành công, chuyển về trang đăng nhập hoặc trang chủ tùy bạn
                return RedirectToAction("Index", "Tour");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Lỗi khi đăng ký: " + ex.Message);
                return View();
            }
        }

        // Đăng nhập
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string numberphone, string password)
        {
            // Kiểm tra admin (theo tài khoản và mật khẩu cứng)
            if (numberphone == "admin" && password == "123")
            {
                Session["UserID"] = "admin"; // ID giả định cho admin
                Session["CustomerName"] = "Administrator";
                Session["UserAvatar"] = "/images/admin-avatar.png";
                Session["Role"] = "Admin";
                return RedirectToAction("Index", "Tour");
            }

            var user = db.Users
                .Join(db.Customers,
                u => u.user_id,
                c => c.user_id,
                (u, c) => new { User = u, Customer = c, Image = u.image }) // Thêm Image từ User
                .FirstOrDefault(x => x.Customer.numberphone == numberphone && x.User.password == password);

            if (user != null)
            {
                Session["UserID"] = user.User.user_id;
                Session["CustomerName"] = user.Customer.customer_name;
                Session["UserAvatar"] = string.IsNullOrEmpty(user.Image) ? "/images/default-avatar.png" : user.Image;
                Session["Role"] = "User";
                Session["user"] = user.Customer; // Gán toàn bộ thông tin customer vào session

                return RedirectToAction("Index", "Tour");
            }
            else
            {
                ViewBag.LoginError = "Username or Password is incorrect!";
                return View();
            }
        }

        public string ProcessUpload(HttpPostedFileBase file)
        {
            if (file == null || file.ContentLength == 0)
            {
                return "/images/default-avatar.png"; // Trả về ảnh mặc định nếu không có ảnh
            }

            string fileName = Path.GetFileName(file.FileName);
            string path = Path.Combine(Server.MapPath("~/Content/images/"), fileName);

            try
            {
                file.SaveAs(path);
                return "/Content/images/" + fileName;
            }
            catch (Exception)
            {
                return "/images/default-avatar.png"; // Nếu lỗi, trả về ảnh mặc định
            }
        }


    }
}