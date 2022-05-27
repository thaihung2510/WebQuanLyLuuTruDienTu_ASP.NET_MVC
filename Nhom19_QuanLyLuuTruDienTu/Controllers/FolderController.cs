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
    public class FolderController : Controller
    {
        QLLTDTEntities db = new QLLTDTEntities();
        // GET: Folder
        public ActionResult Index()
        {

            return View();
        }

        // GET: Folder/Details/5
        public ActionResult Details(int id)
        {
            var chitiet = db.Folders.Find(id);

            var folders = GetFolders(id);
            Session["FolderID"] = id;
            var files = GetFiles();

            IndexVM model = new IndexVM();
            model.Folders = folders;
            model.Files = files;

            return View(model);
        }

        private List<Folder> GetFolders(int id)
        {
            Folder _folderid = new Folder();
            List<Folder> foname = db.Folders.Where(x => x.Parent == id).ToList();
            return foname;
        }


        private List<File> GetFiles()
        {
            List<File> filist = db.Files.ToList();
            return filist;
        }

        // GET: Folder/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Folder/Create
        [HttpPost]
        public ActionResult Create(string foldername)
        {
            ViewBag.message = "Folder" + foldername.ToString() + "Tạo thành công";
            Folder _folder = new Folder();
            _folder.FolderName = foldername;
            var _folderid = (int)Session["FolderID"];
            var parent = db.Folders.Where(s => s.Parent == _folderid).FirstOrDefault();
            _folder.Parent = _folderid;
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

            return RedirectToAction("Index", "Home");
        }

        // GET: Folder/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Folder/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Folder/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Folder/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
