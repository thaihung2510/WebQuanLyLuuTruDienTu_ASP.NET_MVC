using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Nhom19_QuanLyLuuTruDienTu.models;
using File = Nhom19_QuanLyLuuTruDienTu.models.File;
using System.Net;
using System.Data.Entity;

namespace Nhom19_QuanLyLuuTruDienTu.Controllers
{
    public class HomeController : Controller
    {

        QLLTDTEntities db = new QLLTDTEntities();
        public ActionResult TrangChu()
        {
            return View();
        }
        public ActionResult DownloadDetail()
        {
            return View();
        }
        public ActionResult Index()
        {
            if (Session["Username"] == null)
            {
                return RedirectToAction("LoginUser", "User");
            }
            TempData["fullurl"] = HttpContext.Request.Url.AbsoluteUri;


            var folders = GetFolders();
            var files = GetFiles();

            IndexVM model = new IndexVM();
            model.Folders = folders;
            model.Files = files;

            
            return View(model);
        }

        //GetFolders GetFiles for each user
        private List<Folder> GetFolders()
        {
            if (Session["Username"] == null)
            {
                List<Folder> folist = db.Folders.ToList();

                return folist;
            }
            else
            {
                string str = (string)Session["Username"];
                var parent = db.Folders.Where(s => s.FolderName == str).FirstOrDefault();
                List<Folder> folist = db.Folders.Where(x => x.Parent == parent.FolderID).ToList();
                return folist;
            }
        }


        private List<File> GetFiles()
        {
            List<File> filist = db.Files.ToList();
            return filist;
        }

        public ActionResult Delete(string fileName) //downloading
        {
            string fullPath = "";
            byte[] fileBytes;
            fullPath = Path.Combine(Server.MapPath("~/Content/Files"), (string)Session["Username"], fileName);
            System.IO.File.Delete(fullPath);
            var fi = db.Files.Where(f => f.FileName == fileName).FirstOrDefault();
            db.Files.Remove(fi);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        public FileResult Download(string fileName) //downloading
        {
            string fullPath = "";
            byte[] fileBytes;
            if (Session["Username"] == null)
            {
                fullPath = Path.Combine(Server.MapPath("~/Content/Files"), fileName);
                fileBytes = System.IO.File.ReadAllBytes(fullPath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
            else
            {
                fullPath = Path.Combine(Server.MapPath("~/Content/Files"), (string)Session["Username"], fileName);
                fileBytes = System.IO.File.ReadAllBytes(fullPath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
            }
        }
        private int GetFileType(string fileExtension) //file type
        {
            switch (fileExtension.ToLower())
            {
                case ".docx":
                case ".doc":
                    return 1;
                case ".xlsx":
                case ".xls":
                    return 2;
                case ".txt":
                    return 3;
                case ".jpg":
                case ".png":
                case ".apng":
                case ".avif":
                case ".gif":
                case ".svg":
                case ".webp":
                    return 4;
                case ".rar":
                    return 5;
                case ".zip":
                    return 6;
                case ".pptx":
                case ".pptm":
                case ".ppt":
                    return 8;
                case ".pdf":
                    return 9;
                default:
                    return 7;
            }
        }

        [HttpPost]
        public ActionResult Folder(string foldername) //CreateFolder
        {
            //string folder = Server.MapPath(string.Format("~/Content/Files/{0}/{1}/", (string)Session["Username"], foldername));
            //if (!Directory.Exists(folder))
            //{
            //    Directory.CreateDirectory(folder);
            //    ViewBag.message = "Folder" + foldername.ToString() + "Tạo thành công";
            //    Folder _folder = new Folder();
            //    _folder.FolderName = foldername;
            //    string str = (string)Session["Username"];
            //    var parent = db.Folders.Where(s => s.FolderName == str).FirstOrDefault();
            //    _folder.Parent = parent.FolderID;
            //    db.Folders.Add(_folder);

            //    TimeKeep _timeKeep = new TimeKeep();
            //    DateTime _createdate = DateTime.Now;
            //    _timeKeep.CreateDate = _createdate;
            //    DateTime _modifydate = DateTime.Now;
            //    _timeKeep.ModifiedDate = _modifydate;
            //    DateTime _deletedate = DateTime.Now;
            //    _timeKeep.DeletedDate = _deletedate;
            //    db.TimeKeeps.Add(_timeKeep);

            //    db.SaveChanges();
            //}
            //else
            //{
            //    ViewBag.message = "Folder" + foldername.ToString() + "Đã tồn tại";
            //}
            ViewBag.message = "Folder" + foldername.ToString() + "Tạo thành công";
            Folder _folder = new Folder();
            _folder.FolderName = foldername;
            string str = (string)Session["Username"];
            var parent = db.Folders.Where(s => s.FolderName == str).FirstOrDefault();
            _folder.Parent = parent.FolderID;
            db.Folders.Add(_folder);

            TimeKeep _timeKeep = new TimeKeep();
            DateTime _createdate = DateTime.Now;
            _timeKeep.CreateDate = _createdate;
            DateTime _modifydate = DateTime.Now;
            _timeKeep.ModifiedDate = _modifydate;
            DateTime _deletedate = DateTime.Now;
            _timeKeep.DeletedDate = _deletedate;
            db.TimeKeeps.Add(_timeKeep);

            db.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public ActionResult Index(List<HttpPostedFileBase> files) //Upload
        {
            TempData["Message"] = "";
            foreach (var file in files)
            {
                if(file!=null)
                {
                    if (file.ContentLength > 0)
                    {

                        File _file = new models.File();
                        _file.AccountID = (int)Session["UserID"];
                        string extension = Path.GetExtension(file.FileName);
                        _file.FileTypeID = GetFileType(extension);
                        _file.Size = Math.Round((double)file.ContentLength / 1048576, 2);
                        _file.Description = "No Description";
                        _file.Status = true;
                        int _folderid = (int)Session["FolderID"];
                        _file.FolderID = _folderid;
                        var fileName = Path.GetFileName(file.FileName);
                        _file.FileName = fileName;
                        var fileUserPath = Path.Combine(Server.MapPath("~/Content/Files"), (string)Session["Username"]);
                        var filePath = Path.Combine(fileUserPath, fileName);
                        _file.Location = filePath;
                        db.Files.Add(_file);

                        var check = db.Accounts.Where(s => s.AccountID == _file.AccountID).FirstOrDefault();
                        double tempSum = (double)check.TotalSize + (double)_file.Size;
                        check.TotalSize = Math.Round(tempSum, 2);
                        db.Entry(check).State = EntityState.Modified;
                        Session["TotalSize"] = check.TotalSize;

                        TimeKeep _timeKeep = new TimeKeep();
                        DateTime _createdate = DateTime.Now;
                        _timeKeep.CreateDate = _createdate;
                        DateTime _modifydate = DateTime.Now;
                        _timeKeep.ModifiedDate = _modifydate;
                        _timeKeep.DeletedDate = null;
                        db.TimeKeeps.Add(_timeKeep);

                        db.SaveChanges();
                        file.SaveAs(filePath);
                        TempData["Message"] = "files uploaded successfully";
                    }
                }
                else
                {
                    TempData["Message"] = "Choose your upload file!";
                }
            }
            
            
            int foluserid = (int)Session["FolderID"];
            return RedirectToAction("Details", "Folder", new { id = foluserid });
        }

        public ActionResult DeleteFile(int id)
        {
            File file = db.Files.Find(id);
            file.Status = false;
            file.TimeKeep.DeletedDate = DateTime.Now;
            db.Entry(file).State = EntityState.Modified;
            db.SaveChanges();
            int foluserid = (int)Session["FolderID"];
            return RedirectToAction("Details", "Folder", new { id = foluserid });
        }

        public ActionResult Search(string searching)
        {
            return View(db.Files.Where(x => x.FileName.Contains(searching) || searching == null).ToList());
        }
    }

}