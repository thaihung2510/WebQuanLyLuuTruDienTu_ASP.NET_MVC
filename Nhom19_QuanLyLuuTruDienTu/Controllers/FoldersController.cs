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
    public class FoldersController : Controller
    {
        private QLLTDTEntities db = new QLLTDTEntities();

        // GET: Folders
        public ActionResult Index()
        {
            var folders = db.Folders.Include(f => f.TimeKeep).Include(f => f.TimeKeep1);
            return View(folders.ToList());
        }

        // GET: Folders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Folder folder = db.Folders.Find(id);
            if (folder == null)
            {
                return HttpNotFound();
            }
            return View(folder);
        }

        // GET: Folders/Create
        public ActionResult Create()
        {
            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID");
            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID");
            return View();
        }

        // POST: Folders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FolderID,TimeID,FolderName,TotalSize,Location,Parent")] Folder folder)
        {
            if (ModelState.IsValid)
            {
                db.Folders.Add(folder);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID", folder.TimeID);
            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID", folder.TimeID);
            return View(folder);
        }

        // GET: Folders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Folder folder = db.Folders.Find(id);
            if (folder == null)
            {
                return HttpNotFound();
            }
            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID", folder.TimeID);
            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID", folder.TimeID);
            return View(folder);
        }

        // POST: Folders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FolderID,TimeID,FolderName,TotalSize,Location,Parent")] Folder folder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(folder).State = EntityState.Modified;
                db.SaveChanges();
                int folderid = (int)Session["FolderID"];
                return RedirectToAction("Details", "Folder", new { id = folderid });
            }
            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID", folder.TimeID);
            ViewBag.TimeID = new SelectList(db.TimeKeeps, "TimeID", "TimeID", folder.TimeID);
            int folderidd = (int)Session["FolderID"];
            return RedirectToAction("Details", "Folder", new { id = folderidd });
        }

        // GET: Folders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Folder folder = db.Folders.Find(id);
            if (folder == null)
            {
                return HttpNotFound();
            }
            return View(folder);
        }

        // POST: Folders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Folder folder = db.Folders.Find(id);
            db.Folders.Remove(folder);
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
