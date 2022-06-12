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
using DemoVNPay.Others;
using System.Configuration;

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
                return View("LoginUser");
            }
            else
            {
                db.Configuration.ValidateOnSaveEnabled = false;
                Session["Username"] = _username;
                Session["Password"] = _pass;
                Session["AccountType"] = check.AccountType.TypeName;
                Session["UserID"] = check.AccountID;
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
                var userfolderid = -1;
                userfolderid = db.Folders
                    .Where(m => m.FolderName == _username)
                    .Select(m => m.FolderID)
                    .FirstOrDefault();
                Session["UserFolderID"] = userfolderid;

                return RedirectToAction("Details", "Folder", new { id = userfolderid });

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
            return RedirectToAction("TrangChu", "Home");
        }
        public ActionResult DetailUser(string username)
        {
            var user = db.Accounts.Where(s => s.Username == username).SingleOrDefault();
            var accountinfo = db.AccountInfoes.Where(s => s.AccountInfoID == user.AccountInfoID).SingleOrDefault();
            return View(accountinfo);
        }
        public ActionResult EditUser(int id)
        {
            if (Session["user_email"] != null)
            {
                var a = Session["user_email"];
                return View(db.AccountInfoes.Where(s => s.Email == a.ToString()).FirstOrDefault());
            }
            return View(db.AccountInfoes.Where(s => s.AccountInfoID == id).FirstOrDefault());
        }
        [HttpPost]
        public ActionResult EditUser(int id, AccountInfo user)
        {
            db.AccountInfoes.Attach(user);
            //user.ErrorLogin = "NULL";
            if (user.Address == null)
            {
                user.Address = "NULL";
            }
            //if (user.ResetPasswordCode == null)
            //{
            //    user.ResetPasswordCode = Guid.NewGuid().ToString();
            //}

            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.Success = "Đổi thành công";
            return View();
        }
        public ActionResult UserPassEdit(string username)
        {
            if (Session["Username"] != null)
            {
                var a = Session["Username"];
                return View(db.Accounts.Where(s => s.Username == a.ToString()).FirstOrDefault());
            }
            return View(db.Accounts.Where(s => s.Username == username).FirstOrDefault());
        }

        [HttpPost]
        public ActionResult UserPassEdit(Account user)
        {
            db.Accounts.Attach(user);

            if (user.Password == null)
            {
                user.Password = "";
                return View();
            }
            if (user.Password != user.ConfirmPass)
            {
                ViewBag.ErrorRegister = "Sai Pass Confirm";
                return View();
            }

            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.ErrorRegister = "Đổi Pass thành công";
            return View();


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

        public ActionResult Payment()
        {
            string url = ConfigurationManager.AppSettings["Url"];
            string returnUrl = ConfigurationManager.AppSettings["ReturnUrl"];
            string tmnCode = ConfigurationManager.AppSettings["TmnCode"];
            string hashSecret = ConfigurationManager.AppSettings["HashSecret"];

            PayLib pay = new PayLib();

            pay.AddRequestData("vnp_Version", "2.0.0"); //Phiên bản api mà merchant kết nối. Phiên bản hiện tại là 2.0.0
            pay.AddRequestData("vnp_Command", "pay"); //Mã API sử dụng, mã cho giao dịch thanh toán là 'pay'
            pay.AddRequestData("vnp_TmnCode", tmnCode); //Mã website của merchant trên hệ thống của VNPAY (khi đăng ký tài khoản sẽ có trong mail VNPAY gửi về)
            pay.AddRequestData("vnp_Amount", "2500000"); //số tiền cần thanh toán, công thức: số tiền * 100 - ví dụ 10.000 (mười nghìn đồng) --> 1000000
            pay.AddRequestData("vnp_BankCode", ""); //Mã Ngân hàng thanh toán (tham khảo: https://sandbox.vnpayment.vn/apis/danh-sach-ngan-hang/), có thể để trống, người dùng có thể chọn trên cổng thanh toán VNPAY
            pay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss")); //ngày thanh toán theo định dạng yyyyMMddHHmmss
            pay.AddRequestData("vnp_CurrCode", "VND"); //Đơn vị tiền tệ sử dụng thanh toán. Hiện tại chỉ hỗ trợ VND
            pay.AddRequestData("vnp_IpAddr", Util.GetIpAddress()); //Địa chỉ IP của khách hàng thực hiện giao dịch
            pay.AddRequestData("vnp_Locale", "vn"); //Ngôn ngữ giao diện hiển thị - Tiếng Việt (vn), Tiếng Anh (en)
            pay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang"); //Thông tin mô tả nội dung thanh toán
            pay.AddRequestData("vnp_OrderType", "other"); //topup: Nạp tiền điện thoại - billpayment: Thanh toán hóa đơn - fashion: Thời trang - other: Thanh toán trực tuyến
            pay.AddRequestData("vnp_ReturnUrl", returnUrl); //URL thông báo kết quả giao dịch khi Khách hàng kết thúc thanh toán
            pay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString()); //mã hóa đơn

            string paymentUrl = pay.CreateRequestUrl(url, hashSecret);

            return Redirect(paymentUrl);
        }

        public ActionResult PaymentConfirm()
        {
            if (Request.QueryString.Count > 0)
            {
                string hashSecret = ConfigurationManager.AppSettings["HashSecret"]; //Chuỗi bí mật
                var vnpayData = Request.QueryString;
                PayLib pay = new PayLib();

                //lấy toàn bộ dữ liệu được trả về
                foreach (string s in vnpayData)
                {
                    if (!string.IsNullOrEmpty(s) && s.StartsWith("vnp_"))
                    {
                        pay.AddResponseData(s, vnpayData[s]);
                    }
                }

                long orderId = Convert.ToInt64(pay.GetResponseData("vnp_TxnRef")); //mã hóa đơn
                long vnpayTranId = Convert.ToInt64(pay.GetResponseData("vnp_TransactionNo")); //mã giao dịch tại hệ thống VNPAY
                string vnp_ResponseCode = pay.GetResponseData("vnp_ResponseCode"); //response code: 00 - thành công, khác 00 - xem thêm https://sandbox.vnpayment.vn/apis/docs/bang-ma-loi/
                string vnp_SecureHash = Request.QueryString["vnp_SecureHash"]; //hash của dữ liệu trả về

                bool checkSignature = pay.ValidateSignature(vnp_SecureHash, hashSecret); //check chữ ký đúng hay không?

                if (checkSignature)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        //Thanh toán thành công
                        var a = Session["Username"];
                        var usercurrent = db.Accounts.Where(x => x.Username == a.ToString()).FirstOrDefault();
                        usercurrent.AccountTypeID = 2;
                        db.SaveChanges();
                        var acctypecurrent = db.AccountTypes.Where(x => x.AccountTypeID == usercurrent.AccountTypeID).FirstOrDefault();
                        Session["Type"] = acctypecurrent.TypeName;
                        ViewBag.Message = "Thanh toán thành công hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId;
                    }
                    else
                    {
                        //Thanh toán không thành công. Mã lỗi: vnp_ResponseCode
                        ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý hóa đơn " + orderId + " | Mã giao dịch: " + vnpayTranId + " | Mã lỗi: " + vnp_ResponseCode;
                    }
                }
                else
                {
                    ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý";
                }
            }

            return View();
        }

    }
}