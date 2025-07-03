using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VivoWeb.Models;

namespace VivoWeb.Controllers
{
    public class UserController : Controller
    {

        dbVivoWebDataContext db = new dbVivoWebDataContext();
        // GET: User
        public ActionResult Index()
        {
            return View();
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
                var username = collection["Username"];
                var password = collection["PasswordHash"];
                var fullName = collection["FullName"];
                var email = collection["Email"];
                var phone = collection["Phone"];

                if (string.IsNullOrEmpty(password))
                {
                    ViewBag.PasswordError = "Vui lòng nhập mật khẩu!";
                    return View();
                }

                var existingUser = db.Users.FirstOrDefault(u =>
                    u.Username == username || u.Email == email || u.Phone == phone);

                if (existingUser != null)
                {
                    if (existingUser.Username == username)
                        ViewBag.UsernameExists = "Tên đăng nhập đã tồn tại.";
                    if (existingUser.Email == email)
                        ViewBag.EmailExists = "Email đã được sử dụng.";
                    if (existingUser.Phone == phone)
                        ViewBag.PhoneExists = "Số điện thoại đã được sử dụng.";
                    return View();
                }

                // Xử lý avatar
                string avatarPath = ProcessUpload(avatar);

                User newUser = new User
                {
                    Username = username,
                    PasswordHash = password,
                    FullName = fullName,
                    Email = email,
                    Phone = phone,
                    Role = "Customer",
                    Image = avatarPath
                };

                db.Users.InsertOnSubmit(newUser);
                db.SubmitChanges();

                return RedirectToAction("Login", "User");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Lỗi khi đăng ký: " + ex.Message;
                return View();
            }
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string phone, string password)
        {
            // Kiểm tra admin (tài khoản cứng)
            if (phone == "admin" && password == "123")
            {
                Session["UserID"] = 0; // ID giả định cho admin
                Session["UserName"] = "Administrator";
                Session["UserAvatar"] = "/images/admin-avatar.png";
                Session["Role"] = "Admin";
                return RedirectToAction("Index", "Tour");
            }

            // Tìm user trong database dựa trên Phone và PasswordHash
            var user = db.Users.FirstOrDefault(u => u.Phone == phone && u.PasswordHash == password);

            if (user != null)
            {
                Session["UserID"] = user.UserId;
                Session["UserName"] = user.FullName ?? user.Username;
                Session["UserAvatar"] = string.IsNullOrEmpty(user.Image) ? "/images/default-avatar.png" : user.Image;
                Session["Role"] = user.Role ?? "Customer";
                // Có thể lưu thêm user object nếu cần
                Session["User"] = user;

                return RedirectToAction("Index", "Tour");
            }
            else
            {
                ViewBag.LoginError = "Số điện thoại hoặc mật khẩu không đúng!";
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


        // Xem thông tin cá nhân //
        public ActionResult CustomerProfile()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userId = Convert.ToInt32(Session["UserID"]);

            // Lấy thông tin người dùng từ bảng Users
            var user = db.Users.FirstOrDefault(u => u.UserId == userId);
            if (user == null)
            {
                return HttpNotFound("Không tìm thấy thông tin người dùng.");
            }

            // Đặt ảnh mặc định nếu không có ảnh
            string avatarPath = string.IsNullOrEmpty(user.Image) ? "/images/default-avatar.png" : user.Image;
            Session["UserAvatar"] = avatarPath;

            var model = new ProfileViewModel
            {
                CustomerName = user.FullName,
                NumberPhone = user.Phone,
                Email = user.Email,
                Avatar = avatarPath
            };

            return View(model);
        }


        [HttpGet]
        public ActionResult UpdateProfile()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userId = Convert.ToInt32(Session["UserID"]);
            var user = db.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            var model = new ProfileViewModel
            {
                CustomerName = user.FullName,
                NumberPhone = user.Phone,
                Email = user.Email,
                //Address = "", // Nếu bạn không có cột Address trong bảng Users thì để trống hoặc bỏ
                Avatar = string.IsNullOrEmpty(user.Image) ? "/Content/images/default-avatar.png" : user.Image
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateProfile(ProfileViewModel model, HttpPostedFileBase avatar)
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "User");
            }

            int userId = Convert.ToInt32(Session["UserID"]);
            var user = db.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            if (avatar != null && avatar.ContentLength > 0)
            {
                string avatarPath = ProcessUpload(avatar); // Hàm xử lý lưu ảnh
                user.Image = avatarPath;
                Session["UserAvatar"] = avatarPath;
            }

            user.FullName = model.CustomerName;
            user.Phone = model.NumberPhone;
            user.Email = model.Email;

            db.SubmitChanges();

            Session["CustomerName"] = user.FullName;

            return RedirectToAction("CustomerProfile");
        }

        public ActionResult Logout()
        {
            Session.Clear();  // Xoá toàn bộ session
            return RedirectToAction("Login", "User");
        }


    }
}