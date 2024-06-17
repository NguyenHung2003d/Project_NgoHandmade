using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using giadinhthoxinh.Models;

namespace giadinhthoxinh.Areas.Seller.Controllers
{
    public class SuppliersController : Controller
    {
        private giadinhthoxinhEntities db = new giadinhthoxinhEntities();

        // GET: Admin/Suppliers
        public ActionResult Index()
        {
            int userID = (int)Session["idUser"];
            return View(db.tblSuppliers.Where(x => x.FK_iAccountID == userID).ToList());
        }

        // GET: Admin/Suppliers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Suppliers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Admin/Suppliers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "sSupplierName,sPhone,sEmail,sAddress")] tblSupplier tblSupplier)
        {
            if (ModelState.IsValid&& tblSupplier.sSupplierName!=null&&tblSupplier.sPhone!=null&&tblSupplier.sEmail!=null&&tblSupplier.sAddress!=null)
            {
                tblSupplier.FK_iAccountID = (int)Session["idUser"];
                db.tblSuppliers.Add(tblSupplier);
                db.SaveChanges();
                return RedirectToAction("Index", "Home", new { area = "Seller" });
            }

            ViewBag.Error = "Please fill out all required fields.";
            return View(tblSupplier);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSupplier tblSupplier = db.tblSuppliers.Find(id);
            if (tblSupplier == null)
            {
                return HttpNotFound();
            }
            return View(tblSupplier);
        }

        // GET: Admin/Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSupplier tblSupplier = db.tblSuppliers.Find(id);
            if (tblSupplier == null)
            {
                return HttpNotFound();
            }
            return View(tblSupplier);
        }

        // POST: Admin/Suppliers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PK_iSupplierID,FK_iAccountID,sSupplierName,sPhone,sEmail,sAddress")] tblSupplier tblSupplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblSupplier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblSupplier);
        }

        // GET: Admin/Suppliers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblSupplier tblSupplier = db.tblSuppliers.Find(id);
            if (tblSupplier == null)
            {
                return HttpNotFound();
            }
            return View(tblSupplier);
        }

        // POST: Admin/Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            tblSupplier tblSupplier = db.tblSuppliers.Find(id);
            db.tblSuppliers.Remove(tblSupplier);
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
