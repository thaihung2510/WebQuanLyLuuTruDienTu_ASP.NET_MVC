using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Nhom19_QuanLyLuuTruDienTu.models;

namespace Nhom19_QuanLyLuuTruDienTu.Controllers
{
    public class FilesController : Controller
    {
        private QLLTDTEntities db = new QLLTDTEntities();

        // GET: Files
        public ActionResult Index()
        {
            var files = db.Files.Include(f => f.Account).Include(f => f.Account1).Include(f => f.FileType).Include(f => f.FileType1).Include(f => f.Folder).Include(f => f.Folder1).Include(f => f.TagName).Include(f => f.TimeKeep).Include(f => f.TimeKeep1);
            return View(files.ToList());
        }

        // GET: Files/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = db.Files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        // GET: Files/Create
        public ActionResult Create()
        {
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Username");
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Username");
            ViewBag.FileTypeID = new SelectList(db.FileTypes, "FileTypeID", "TypeName");
            ViewBag.FileTypeID = new SelectList(db.FileTypes, "FileTypeID", "TypeName");
            ViewBag.FolderID = new SelectList(db.Folders, "FolderID", "FolderName");
            ViewBag.FolderID = new SelectList(db.Folders, "FolderID", "FolderName");
            ViewBag.TagID = new SelectList(db.TagNames, "TagID", "NameTag");
            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID");
            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID");
            return View();
        }

        // POST: Files/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FileID,AccountID,TagID,FileTypeID,FolderID,TimeID,Location,FileName,Size,Description,Status")] File file)
        {
            if (ModelState.IsValid)
            {
                db.Files.Add(file);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Username", file.AccountID);
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Username", file.AccountID);
            ViewBag.FileTypeID = new SelectList(db.FileTypes, "FileTypeID", "TypeName", file.FileTypeID);
            ViewBag.FileTypeID = new SelectList(db.FileTypes, "FileTypeID", "TypeName", file.FileTypeID);
            ViewBag.FolderID = new SelectList(db.Folders, "FolderID", "FolderName", file.FolderID);
            ViewBag.FolderID = new SelectList(db.Folders, "FolderID", "FolderName", file.FolderID);
            ViewBag.TagID = new SelectList(db.TagNames, "TagID", "NameTag", file.TagID);
            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID", file.TimeID);
            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID", file.TimeID);
            return View(file);
        }

        // GET: Files/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("LoginUser", "User");
            }
            File file = db.Files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Username", file.AccountID);
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Username", file.AccountID);
            ViewBag.FileTypeID = new SelectList(db.FileTypes, "FileTypeID", "TypeName", file.FileTypeID);
            ViewBag.FileTypeID = new SelectList(db.FileTypes, "FileTypeID", "TypeName", file.FileTypeID);
            ViewBag.FolderID = new SelectList(db.Folders, "FolderID", "FolderName", file.FolderID);
            ViewBag.FolderID = new SelectList(db.Folders, "FolderID", "FolderName", file.FolderID);
            ViewBag.TagID = new SelectList(db.TagNames, "TagID", "NameTag", file.TagID);
            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID", file.TimeID);
            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID", file.TimeID);
            return View(file);
        }

        // POST: Files/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FileID,AccountID,TagID,FileTypeID,FolderID,TimeID,Location,FileName,Size,Description,Status")] File file)
        {
            if (ModelState.IsValid)
            {
                db.Entry(file).State = EntityState.Modified;
                db.SaveChanges();
                int foluserid = (int)Session["FolderID"];
                return RedirectToAction("Details", "Folder", new { id = foluserid });
            }
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Username", file.AccountID);
            ViewBag.AccountID = new SelectList(db.Accounts, "AccountID", "Username", file.AccountID);
            ViewBag.FileTypeID = new SelectList(db.FileTypes, "FileTypeID", "TypeName", file.FileTypeID);
            ViewBag.FileTypeID = new SelectList(db.FileTypes, "FileTypeID", "TypeName", file.FileTypeID);
            ViewBag.FolderID = new SelectList(db.Folders, "FolderID", "FolderName", file.FolderID);
            ViewBag.FolderID = new SelectList(db.Folders, "FolderID", "FolderName", file.FolderID);
            ViewBag.TagID = new SelectList(db.TagNames, "TagID", "NameTag", file.TagID);
            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID", file.TimeID);
            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID", file.TimeID);
            return View(file);
        }

        // GET: Files/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            File file = db.Files.Find(id);
            if (file == null)
            {
                return HttpNotFound();
            }
            return View(file);
        }

        // POST: Files/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            File file = db.Files.Find(id);
            db.Files.Remove(file);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
