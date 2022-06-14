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
        public ActionResult DownloadDetail(int id)
        {
            File file = db.Files.Where(s => s.FileID == id).FirstOrDefault();
            Session["FileDownload"] = file.FileName;
            Session["fiId"] = file.FileID;
            Session["finame"] = file.FileName;
            Session["fisize"] = file.Size;
            Session["fitype"] = file.FileType.TypeName;
            Session["fiowner"] = file.Account.Username;
            Session["ficreate"] = file.TimeKeep.CreateDate;
            Session["fiedit"] = file.TimeKeep.ModifiedDate;
            Session["fidesc"] = file.Description;
            Session["DownloadURL"] = "http://filefate.somee.com/Home/DownloadDetail/" + id;
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

        public FileResult Download(int id) //downloading
        {
            string fullPath = "";
            byte[] fileBytes;
            var fileDownload = db.Files.Find(id);
            if (Session["Username"] == null)
            {
                return null;
            }
            else
            {
                fullPath = fileDownload.Location;
                fileBytes = System.IO.File.ReadAllBytes(fullPath);
                return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileDownload.FileName);
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
            int folderid = (int)Session["FolderID"];
            double limit = (double)Session["limitSize"];
            double listSum = (double)files.Sum(item => item.ContentLength);
            double listSumInMB = Math.Round(listSum / 1048576, 2);
            if(listSumInMB>limit)
            {
                TempData["Message"] = "Your Storage not enough!!";

                return RedirectToAction("Details", "Folder", new { id = folderid });
            }
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
            return RedirectToAction("Details", "Folder", new { id = folderid });
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