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

            var folders = GetFolders();
            var files = GetFiles();

            IndexVM model = new IndexVM();
            string myid = id.ToString();
            List<Folder> folist = db.Folders.ToList();
            List<Folder> foname = folist.Where(x => x.Parent == myid).ToList();
            model.Folders = foname;
            model.Files = files;

            return View(model);
        }

        private List<Folder> GetFolders()
        {
            List<Folder> folist = db.Folders.ToList();
            Folder _folderid = new Folder();
            string myid = _folderid.FolderID.ToString();
            List<Folder> foname = folist.Where(x => x.Parent == myid).ToList();
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
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
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
