using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Nhom19_QuanLyLuuTruDienTu.models;

namespace Nhom19_QuanLyLuuTruDienTu.Controllers
{
    public class UserController : Controller
    {
        QLLTDTEntities db = new QLLTDTEntities();
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginAcc(FormCollection frc)
        {
            var _username = frc["username"];
            var _pass = frc["pass"];
            var _accid = 6; //test
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
                //test
                Session["UserID"] = _accid;
                Account account = new Account();
                //_accid = account.AccountID;
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
            if (ModelState.IsValid)
            {
                var check_id = db.Accounts.Where(s => s.Username == _username).FirstOrDefault();

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
    }
}