﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Nhom19_QuanLyLuuTruDienTu.models;
using File = Nhom19_QuanLyLuuTruDienTu.models.File;
using System.Web.Routing;
using System.Data.Entity;

namespace Nhom19_QuanLyLuuTruDienTu.Controllers
{
    public class FolderController : Controller
    {
        QLLTDTEntities db = new QLLTDTEntities();
        // GET: Folder
        public ActionResult Index()
        {
            var folders = GetFolders();
            var files = GetFiles();

            IndexVM model = new IndexVM();
            model.Folders = folders;
            model.Files = files;

            return View(model);
        }

        // GET: Folder/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.param = id;

            var chitiet = db.Folders.Find(id);

            var folders = GetFolders(id);
            Session["FolderID"] = id;
            var files = GetFiles(id);

            IndexVM model = new IndexVM();
            model.Folders = folders;
            model.Files = files;

            return View(model);
        }

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
            List<File> filist = db.Files.Where(x => x.Status == true).ToList();
            return filist;
        }

        private List<Folder> GetFolders(int id)
        {
            Folder _folderid = new Folder();
            List<Folder> foname = db.Folders.Where(x => x.Parent == id).ToList();
            return foname;
        }


        private List<File> GetFiles(int id)
        {
            File _filefollow = new File();
            List<File> filist = db.Files.Where(x => x.FolderID == id && x.Status == true).ToList();
            return filist;
        }

        public ActionResult OnRightClick()
        {
            return View();
        }

        // GET: Folder/Create
        public ActionResult Create()
        {
            return View();
        }

        public ActionResult FileDetails(int id)
        {
            File file = db.Files.Find(id);

            var _fileId = db.Files
                    .Where(m => m.FileID == id)
                    .Select(m => m.FileID)
                    .FirstOrDefault();
            Session["fiId"] = _fileId;
            var _fileName = db.Files
                    .Where(m => m.FileID == id)
                    .Select(m => m.FileName)
                    .FirstOrDefault();
            var _fileSize = db.Files
                    .Where(m => m.FileID == id)
                    .Select(m => m.Size)
                    .FirstOrDefault();
            var _fileTypeid = db.Files
                    .Where(m => m.FileID == id)
                    .Select(m => m.FileTypeID)
                    .FirstOrDefault();
            var _fileType = db.FileTypes
                    .Where(m => m.FileTypeID == _fileTypeid)
                    .Select(m => m.TypeName)
                    .FirstOrDefault();
            var _fileownerid = db.Files
                    .Where(m => m.FileID == id)
                    .Select(m => m.AccountID)
                    .FirstOrDefault();
            var _fileOwner = db.Accounts
                    .Where(m => m.AccountID == _fileownerid)
                    .Select(m => m.Username)
                    .FirstOrDefault();
            var _fileDescr = db.Files
                    .Where(m => m.FileID == id)
                    .Select(m => m.Description)
                    .FirstOrDefault();
            Session["finame"] = _fileName;
            Session["fisize"] = _fileSize;
            Session["fitype"] = _fileType;
            Session["fiowner"] = _fileOwner;
            Session["fidesc"] = _fileDescr;

            int _folderid = (int)Session["FolderID"];
            return RedirectToAction("Details", "Folder", new { id = _folderid });
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

            return RedirectToAction("Details", "Folder", new { id = _folderid });
        }

        public ActionResult Foldel(int id)
        {
            QLLTDTEntities db = new QLLTDTEntities();
            int fid = (int)Session["FolderID"];
            var fidel = db.Files.Where(x => x.FolderID == id).FirstOrDefault();
            var foldel = db.Folders.Where(x => x.FolderID == id).FirstOrDefault();
            var folpadel = db.Folders.Where(x => x.Parent == id).FirstOrDefault();
            //var folpadelcheck = db.Folders.Except(folpadel.Parent).FirstOrDefault();

            fidel.Status = false;
            
            if (fidel != null)
            {
                fidel.Status = false;
                db.Entry(fidel).State = EntityState.Modified;
            }
            if (folpadel != null)
            {
                db.Folders.Remove(folpadel);
                //db.Folders.Remove(folpadelcheck);
            }
            db.Entry(fidel).State = EntityState.Modified;
            db.Folders.Remove(foldel);
            db.SaveChanges();


            return RedirectToAction("Details", "Folder", new { id = fid });
        }

        // GET: Folder/Edit/5
        public ActionResult Edit(int id)
        {
            var _folder = db.Folders.Where(x => x.FolderID == id).SingleOrDefault();
            return PartialView("Edit", _folder);
        }

        // POST: Folder/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "FolderID,FolderName,TotalSize,Location,Parent")] Folder folder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(folder).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("Edit", folder);
            }
            return PartialView("Edit", folder);
        }


        // GET: Folder/Delete/5
        public ActionResult Delete(int id)
        {
            Folder folderid = db.Folders.Find(id);
            db.Folders.Remove(folderid);
            db.SaveChanges();
            return RedirectToAction("Details");
        }

        // POST: Folder/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Details");
            }
            catch
            {
                return View();
            }
        }
    }
}