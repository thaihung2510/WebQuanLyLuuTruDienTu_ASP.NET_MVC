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

        QLLTDTEntities db = new QLLTDTEntities();

        public ActionResult Index()
        {
            List<ObjFile> ObjFiles = new List<ObjFile>();
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

            return View(ObjFiles);
        }
        public FileResult Download(string fileName) //downloading
        {
            string fullPath = Path.Combine(Server.MapPath("~/Content/Files"), fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(fullPath);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
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
        public ActionResult Index(ObjFile doc)
        {
            foreach (var file in doc.files)
            {
                if (file.ContentLength > 0)
                {

                    File _file = new models.File();
                    Account account = new Account();

                    _file.AccountID = (int)Session["UserID"];
                    _file.FileTypeID = 1;
                    _file.TagID = 2;
                    _file.FolderID = 1;
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
    } 
    
}
public class ObjFile //test without data
{
    public IEnumerable<HttpPostedFileBase> files { get; set; }
    public string File { get; set; }
    public long Size { get; set; }
    public string Type { get; set; }
}