using System;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using giadinhthoxinh.Models;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace giadinhthoxinh.Controllers
{
    public class UserController : Controller
    {
        giadinhthoxinhEntities db = new giadinhthoxinhEntities();
        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        //Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string sEmail, string sPass)
        {
            var f_password = GetMD5(sPass);
            var user = db.tblUsers.FirstOrDefault(s => s.sEmail.Equals(sEmail) && s.sPass.Equals(f_password));

            if (user != null && user.iState!=0)
            {
                            var supplier = db.tblSuppliers.FirstOrDefault(s=>s.FK_iAccountID==user.PK_iAccountID);
            var permission = db.tblPermissions.FirstOrDefault(s => s.PK_iPermissionID == user.FK_iPermissionID);
                Session["User"] = user;
                Session["idUser"] = user.PK_iAccountID;
                Session["userName"] = user.sUserName;
                Session["Email"] = user.sEmail;
                Session["UserRole"]  = permission.sPermissionName;

                if (user.FK_iPermissionID == 2)
                {
                    Session["Nhanvien"] = user;
                    if (supplier == null)
                    {
                        return RedirectToAction("Create", "Suppliers", new { area = "Seller" });
                    }
                    return RedirectToAction("Index", "Home", new { area = "Seller" });
                }
                else if (user.FK_iPermissionID == 3)
                {
                    Session["Nhanvien"] = user;
                    Session["QuanLy"] = user;
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ViewBag.error = "Login failed";
                return View("Login");
            }
        }

        [HttpPost]
        public ActionResult ExternalLogin(string provider)
        {
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "User"));
        }

        public ActionResult ExternalLoginCallback()
        {
            var loginInfo = HttpContext.GetOwinContext().Authentication.GetExternalLoginInfo();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            var user = db.tblUsers.FirstOrDefault(u => u.sEmail == loginInfo.Email);
            if (user == null)
            {
                string password = "123";
                user = new tblUser
                {
                    sEmail = loginInfo.Email,
                    sUserName = loginInfo.DefaultUserName,
                    sPass = GetMD5(password),
                    FK_iPermissionID = 1
                };
                db.tblUsers.Add(user);
                db.SaveChanges();
            }

            Session["User"] = user;
            Session["idUser"] = user.PK_iAccountID;
            Session["userName"] = user.sUserName;
            Session["Email"] = user.sEmail;
            Session["UserRole"] = "User";

            return RedirectToAction("Index", "Home");
        }

        private class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }

        [HttpPost]
        public ActionResult SendVerificationCode(string email)
        {
            var user = db.tblUsers.FirstOrDefault(u => u.sEmail == email);
            if (user != null)
            {
                // Generate a verification code
                var verificationCode = new Random().Next(100000, 999999).ToString();
                Session["VerificationCode"] = verificationCode;
                Session["ResetEmail"] = email;

                // Send the verification code to the user's email
                SendEmail(email, "Password Reset Verification Code", $"Your verification code is: {verificationCode}");

                ViewBag.Message = "Verification code sent to your email.";
                ViewBag.ShowVerificationCodeSection = true;
                return View("Login");
            }
            else
            {
                ViewBag.Error = "Email not found.";
                ViewBag.ShowForgotPasswordSection = true;
                return View("Login");
            }
        }

        [HttpPost]
        public ActionResult VerifyCode(string verificationCode)
        {
            if (Session["VerificationCode"] != null && Session["VerificationCode"].ToString() == verificationCode)
            {
                ViewBag.Message = "Verification code is correct.";
                ViewBag.ShowNewPasswordSection = true;
                return View("Login");
            }
            else
            {
                ViewBag.Error = "Invalid verification code.";
                ViewBag.ShowVerificationCodeSection = true;
                return View("Login");
            }
        }

        [HttpPost]
        public ActionResult ResetPassword(string newPassword)
        {
            if (Session["ResetEmail"] != null)
            {
                var email = Session["ResetEmail"].ToString();
                var user = db.tblUsers.FirstOrDefault(u => u.sEmail == email);
                if (user != null)
                {
                    user.sPass = GetMD5(newPassword);
                    db.SaveChanges();

                    ViewBag.Message = "Password reset successfully.";
                    return View("Login");
                }
                else
                {
                    ViewBag.Error = "User not found.";
                    ViewBag.ShowNewPasswordSection = true;
                    return View("Login");
                }
            }
            else
            {
                ViewBag.Error = "Session expired. Please try again.";
                return View("Login");
            }
        }

        private void SendEmail(string toEmail, string subject, string body)
        {
            var fromEmail = "ducpd82@gmail.com";
            var fromPassword = "gsba ghgr ajkr owwg";

            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);

            smtpClient.Send(mailMessage);
        }

        public ActionResult AccountPartial()
        {
            
            if (Session["User"]!=null)
            {

                var kh = (tblUser)Session["User"];
                var thongtinkhachhang = db.tblUsers.Find(kh.PK_iAccountID);
                ViewBag.TenKH = thongtinkhachhang.sUserName.ToString();
            }


            return PartialView();
        }
        //Register    
        public ActionResult Register()
        {
            ViewBag.FK_iPermissionID = new SelectList(db.tblPermissions, "PK_iPermissionID", "sPermissionName");
            return View();
        }

        //public ActionResult Register1()
        //{
        //    return View();
        //}

        //POST: Register
        [HttpPost]
        public ActionResult Register(tblUser _user)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(_user.sUserName) || string.IsNullOrEmpty(_user.sEmail) || string.IsNullOrEmpty(_user.sPass))
                {
                    ViewBag.error = "All fields are required.";
                    ViewBag.FK_iPermissionID = new SelectList(db.tblPermissions, "PK_iPermissionID", "sPermissionName");
                    return View();
                }

                var check = db.tblUsers.FirstOrDefault(s => s.sEmail == _user.sEmail);
                if (check == null)
                {
                    _user.sPass = GetMD5(_user.sPass);
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.tblUsers.Add(_user);
                    db.SaveChanges();
                    return RedirectToAction("index", "home");
                }
                else
                {
                    ViewBag.error = "Email này đã tồn tại";
                    ViewBag.FK_iPermissionID = new SelectList(db.tblPermissions, "PK_iPermissionID", "sPermissionName");
                    return View();
                }
            }
            ViewBag.FK_iPermissionID = new SelectList(db.tblPermissions, "PK_iPermissionID", "sPermissionName");
            return View();
        }


        //create a string MD5
        public static string GetMD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x2");

            }
            return byte2String;
        }

        public ActionResult Permission()
        {
            return View();
        }
        public ActionResult Edit()
        {
           
            if (Session["User"] != null)
            {
                var nguoidung = (tblUser)Session["User"];
                var nguoidung_sua = db.tblUsers.Find(nguoidung.PK_iAccountID);
                return View(nguoidung_sua);
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
                
           
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(tblUser _user)
        {
            if (ModelState.IsValid)
            {
                var olddata = db.tblUsers.FirstOrDefault(s => s.PK_iAccountID == _user.PK_iAccountID);
                if (olddata != null)
                {
                    olddata.sUserName = _user.sUserName;
                    olddata.sEmail = _user.sEmail;
                    olddata.sPhone = _user.sPhone;
                    olddata.sAddress = _user.sAddress;
                    db.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.error = "Không tồn tại tài khoản";
                    return RedirectToAction("Login", "User"); ;
                }


            }
            //ViewBag.FK_iPermissionID = new SelectList(db.tblPermissions, "PK_iPermissionID", "sPermissionName", tblUser.FK_iPermissionID);
            return View();
        }
        public ActionResult EditPassword()
        {

            if (Session["User"] != null)
            {
               
                return View();
            }
            else
            {
                return RedirectToAction("Login", "User");
            }


        }
        [HttpPost]
        public ActionResult EditPassword(tblUser _user)
        {
            if (ModelState.IsValid)
            {
                var nguoidung = (tblUser)Session["User"];
                var nguoidung_sua = db.tblUsers.Find(nguoidung.PK_iAccountID);

                if (nguoidung_sua != null)
                {
                    var input = nguoidung_sua.sPass;
                    var oldPasswordInput = Request.Form["oldpass"];
                    var newPasswordInput = Request.Form["newpass"];
                    var confirmPasswordInput = Request.Form["confirmpass"];

                    if (string.IsNullOrEmpty(oldPasswordInput) || string.IsNullOrEmpty(newPasswordInput) || string.IsNullOrEmpty(confirmPasswordInput))
                    {
                        ViewBag.error = "All fields are required.";
                        return View();
                    }

                    if (newPasswordInput != confirmPasswordInput)
                    {
                        ViewBag.error = "New password and confirmation password do not match.";
                        return View();
                    }

                    var oldPasswordHash = GetMD5(oldPasswordInput);

                    if (input == oldPasswordHash)
                    {
                        nguoidung_sua.sPass = GetMD5(newPasswordInput);
                        db.SaveChanges();
                        ViewBag.error = "Password changed successfully!";
                    }
                    else
                    {
                        ViewBag.error = "Incorrect old password!";
                    }
                }
                else
                {
                    ViewBag.error = "User not found!";
                }
            }
            else
            {
                ViewBag.error = "Invalid model state!";
            }

            return View();
        }
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("index", "home");

        }

    }
}