using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Net.Mail;
using Nhom19_QuanLyLuuTruDienTu.models;
using System.Net;

namespace Nhom19_QuanLyLuuTruDienTu.Controllers
{
    public class UserController : Controller
    {
        QLLTDTEntities db = new QLLTDTEntities();
        public ActionResult LoginUser()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginAcc(FormCollection frc)
        {
            var _username = frc["username"];
            var _pass = frc["pass"];
            var check = db.Accounts.Where(s => s.Username == _username && s.Password == _pass).FirstOrDefault();
            if (check == null)
            {
                ViewBag.ErrorInfo = "Sai thông tin";
                return View("Index");
            }
            else
            {

                db.Configuration.ValidateOnSaveEnabled = false;
                Session["Username"] = _username;
                Session["Password"] = _pass;
                //db.SaveChanges();
                ViewBag.Message = "Đăng nhập thành công";

                var folder = Path.Combine(Server.MapPath("~/Content/Files"), _username);
                if (!Directory.Exists(folder))
                {
                    TimeKeep _timeKeep = new TimeKeep();
                    DateTime _createdate = DateTime.Now;
                    _timeKeep.CreateDate = _createdate;
                    DateTime _modifydate = DateTime.Now;
                    _timeKeep.ModifiedDate = _modifydate;
                    DateTime _deletedate = DateTime.Now;
                    _timeKeep.DeletedDate = _deletedate;
                    db.TimeKeeps.Add(_timeKeep);

                    Folder _folder = new Folder();
                    _folder.FolderName = _username;
                    db.Folders.Add(_folder);
                    db.SaveChanges();
                    Directory.CreateDirectory(folder);
                }

                return RedirectToAction("Index", "Home");

            }
        }
        public ActionResult RegisterUser()
        {
            return View();
        }
        [HttpPost]
        public ActionResult RegisterUser(FormCollection frc)
        {
            var _name = frc["name"];
            var _address = frc["address"];
            var _phone = frc["phone"];
            var _email = frc["email"];
            var _username = frc["username"];
            var _pass = frc["pass"];
            var _repass = frc["repeat-pass"];
            if (_name == "" || _phone == "" || _email == "" || _pass == "" || _username == "" || _address == "")
            {
                ViewBag.ErrorRegister("Điền thiếu thông tin");
                return View();
            }

            if (_pass.Length < 8)
            {
                ViewBag.ErrorRegister("Mật khẩu không đủ độ bảo mật");
                return View();
            }
            if (_pass != _repass)
            {
                ViewBag.ErrorRegister("Sai mật khẩu");
                return View();
            }

            if (ModelState.IsValid)

            {
                var check_id = db.Accounts.Where(s => s.Username == _username).FirstOrDefault();

                if (check_id == null)//chưa có id
                {
                    AccountInfo _user = new AccountInfo();
                    _user.Name = _name;
                    _user.Phone = _phone;
                    _user.Email = _email;
                    _user.DateOfBirth = "2000-01-01".AsDateTime();
                    _user.Address = _address;
                    _user.Avatar = "";
                    db.AccountInfoes.Add(_user);


                    Account _usertk = new Account();
                    _usertk.Username = _username;
                    _usertk.Password = _pass;
                    _usertk.ResetPassWordCode = "";
                    _usertk.AccountTypeID = 1;

                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.Accounts.Add(_usertk);
                    //db.SaveChanges();

                    //Tao Folder
                    var folder = Path.Combine(Server.MapPath("~/Content/Files"), _username);
                    if (!Directory.Exists(folder))
                    {
                        TimeKeep _timeKeep = new TimeKeep();
                        DateTime _createdate = DateTime.Now;
                        _timeKeep.CreateDate = _createdate;
                        DateTime _modifydate = DateTime.Now;
                        _timeKeep.ModifiedDate = _modifydate;
                        DateTime _deletedate = DateTime.Now;
                        _timeKeep.DeletedDate = _deletedate;
                        db.TimeKeeps.Add(_timeKeep);

                        Folder _folder = new Folder();
                        _folder.FolderName = _username;
                        db.Folders.Add(_folder);
                        db.SaveChanges();
                        Directory.CreateDirectory(folder);
                    }
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorRegister();
                    return View();
                }
            }
            return View();
        }
        public ActionResult LogOutUser()
        {
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
        public ActionResult DetailUser(string username)
        {
            var user = db.Accounts.Where(s => s.Username == username).SingleOrDefault();
            var accountinfo = db.AccountInfoes.Where(s => s.AccountInfoID == user.AccountInfoID).SingleOrDefault();
            return View(accountinfo);
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }
        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/User/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("raovat.huflit@gmail.com", "File Fate Reset Password");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "raovat2889"; // Replace with actual password

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                body = "<br/><br/>We are excited to tell you that your Rao Vat account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            else if (emailFor == "ResetPassword")
            {
                body = "Chào bạn: " + emailID + "," +
                    "<br><br />Bạn vừa yêu cầu lấy lại mật khẩu tài khoản trên Website Filefate.somee.vn" +
                    "<br><br /><b>Chú ý:</b> Bạn có thể bỏ qua email này nếu <b>người yêu cầu lấy lại mật khẩu không phải là bạn</b>" +
                    "<br><br />Hãy click vào dòng dưới đây để lấy lại mật khẩu của mình" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>" +
                    "<br><br />Nếu bạn muốn liên hệ với chúng tôi thì bạn đừng Reply lại theo email này!" +
                    "<br><br />Để biết thêm thông tin, xin hãy liên hệ với chúng tôi theo thông tin dưới đây:" +
                    "<br><br />Địa chỉ: 363 Bình Trị Đông, Phường Bình Trị Đông A, Quận Bình Tân, TP Hồ Chí Minh" +
                    "<br><br />Website: https://Filefate.sommee.vn </center>";

            }


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        [HttpPost]
        public ActionResult ForgotPassWord(string EmailID)
        {
            string message = "";
            bool status = false;
            using (QLLTDTEntities db = new QLLTDTEntities())
            {
                var account = db.AccountInfoes.Where(a => a.Email == EmailID).FirstOrDefault();
                var accounttim = db.Accounts.Where(x => x.AccountInfoID == account.AccountInfoID).FirstOrDefault();
                if (account != null)
                {
                    //Gửi mail

                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.Email, resetCode, "ResetPassword");
                    db.Accounts.Where(x=>x.ResetPassWordCode == resetCode);
                    accounttim.ResetPassWordCode = resetCode;
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.SaveChanges();
                    message = "Đã gửi thư đến email " + EmailID + " vui lòng kiểm tra";
                }
                else
                {
                    message = "Không tìm thấy tài khoản";
                }
            }
            ViewBag.Message = message;
            return View();
        }


        public ActionResult ResetPassword(string id)
        {

            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            var user = db.Accounts.Where(x => x.ResetPassWordCode == id).FirstOrDefault();
            if (user != null)
            {
                ResetPasswordModel model = new ResetPasswordModel();
                model.ResetCode = id;
                return View(model);
            }
            else
            {
                return HttpNotFound();
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (QLLTDTEntities db = new QLLTDTEntities())
                {
                    var user = db.Accounts.Where(a => a.ResetPassWordCode == model.ResetCode).FirstOrDefault();

                    if (user != null)
                    {
                        user.Password = /*Crypto.Hash(model.NewPassword);*/ model.NewPassword;
                        user.ResetPassWordCode = "";
                        db.Configuration.ValidateOnSaveEnabled = false;
                        db.SaveChanges();
                        message = "Mật khẩu tạo mới thành công";
                    }
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }
    }
}