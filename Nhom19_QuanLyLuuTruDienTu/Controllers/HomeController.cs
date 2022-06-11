using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Nhom19_QuanLyLuuTruDienTu.models;
using File = Nhom19_QuanLyLuuTruDienTu.models.File;

namespace Nhom19_QuanLyLuuTruDienTu.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult TrangChu()
        {
            return View();
        }

        QLLTDTEntities db = new QLLTDTEntities();

        public ActionResult Index()
        {
            List<ObjFile> ObjFiles = new List<ObjFile>();

            var folders = GetFolders();
            var files = GetFiles();

            IndexVM model = new IndexVM();
            model.Folders = folders;
            model.Files = files;

            if(Session["Username"] == null)
            {
                foreach (string strfile in Directory.GetFiles(Server.MapPath("~/Content/Files")))
                {
                    FileInfo fi = new FileInfo(strfile);
                    ObjFile obj = new ObjFile();
                    obj.File = fi.Name;
                    obj.Size = fi.Length;
                    obj.Type = GetFileType(fi.Extension);
                    ObjFiles.Add(obj);
                }
            }
            else
            {
                foreach (string strfile in Directory.GetFiles(Path.Combine(Server.MapPath("~/Content/Files"), (string)Session["Username"])))
                {
                    FileInfo fi = new FileInfo(strfile);
                    ObjFile obj = new ObjFile();
                    obj.File = fi.Name;
                    obj.Size = fi.Length;
                    obj.Type = GetFileType(fi.Extension);
                    ObjFiles.Add(obj);
                }
            }

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
                string str =(string)Session["Username"];
                List <Folder> folist = db.Folders.Where(x => x.Parent == str).ToList();
                return folist;
            }   
        }


        private List<File> GetFiles()
        {
            List<File> filist = db.Files.ToList();
            return filist;
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
        public ActionResult Delete(string fileName) //downloading
        {
            string fullPath = "";
            File fi;
            if (Session["Username"] == null)
            {
                fullPath = Path.Combine(Server.MapPath("~/Content/Files"), fileName);
                System.IO.File.Delete(fullPath);
                TempData["Message"] = "files deleted successfully";
            }
            else
            {
                fullPath = Path.Combine(Server.MapPath("~/Content/Files"), (string)Session["Username"], fileName);
                System.IO.File.Delete(fullPath);
                fi = db.Files.Where(f => f.FileName == fileName).FirstOrDefault();
                db.Files.Remove(fi);
                db.SaveChanges();
                TempData["Message"] = "files deleted successfully";

            }
            return RedirectToAction("Index");
        }
        private string GetFileType(string fileExtension) //file type
        {
            switch (fileExtension.ToLower())
            {
                case ".docx":
                case ".doc":
                    return "Microsoft Word Document";
                case ".xlsx":
                case ".xls":
                    return "Microsoft Excel Document";
                case ".txt":
                    return "Text Document";
                case ".jpg":
                case ".png":
                    return "Image";
                default:
                    return "Unknown";
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

            _folder.Parent = str;
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
        public ActionResult Index(ObjFile doc) //Upload
        {
            foreach (var file in doc.files)
            {
                if (file.ContentLength > 0)
                {

                    File _file = new models.File();
                    _file.AccountID = (int)Session["UserID"];
                    _file.FileTypeID = 1;
                    _file.Status = true;

                    var fileName = Path.GetFileName(file.FileName);
                    _file.FileName = fileName;
                    var fileUserPath = Path.Combine(Server.MapPath("~/Content/Files"), (string)Session["Username"]);
                    var filePath = Path.Combine(fileUserPath, fileName);
                    db.Files.Add(_file);

                    TimeKeep _timeKeep = new TimeKeep();
                    DateTime _createdate = DateTime.Now;
                    _timeKeep.CreateDate = _createdate;
                    DateTime _modifydate = DateTime.Now;
                    _timeKeep.ModifiedDate = _modifydate;
                    DateTime _deletedate = DateTime.Now;
                    _timeKeep.DeletedDate = _deletedate;
                    db.TimeKeeps.Add(_timeKeep);

                    db.SaveChanges();
                    file.SaveAs(filePath);
                }
            }
            TempData["Message"] = "files uploaded successfully";
            return RedirectToAction("Index");
        }

        public ActionResult Search(string searching)
        {
            return View(db.Files.Where(x => x.FileName.Contains(searching) || searching == null).ToList());
        }
    } 
    
}
public class ObjFile //test without data
{
    public IEnumerable<HttpPostedFileBase> files { get; set; }
    public string File { get; set; }
    public string location { get; set; }
    public string description { get; set; }
    public long Size { get; set; }
    public string Type { get; set; }
}