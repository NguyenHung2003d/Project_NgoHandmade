using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using giadinhthoxinh.Areas.Seller.Filter;
using giadinhthoxinh.Models;

namespace giadinhthoxinh.Areas.Seller.Controllers
{
    public class ImportOrdersController : Controller
    {
        private giadinhthoxinhEntities db = new giadinhthoxinhEntities();

        // GET: Admin/ImportOrders
        [SupplierFilter]
        public ActionResult Index()
        {
            var userId = (int)Session["idUser"];
            var userSupplierIDs = db.tblSuppliers
                                    .Where(s => s.FK_iAccountID == userId)
                                    .Select(s => s.PK_iSupplierID)
                                    .ToList();
            var tblImportOrders = db.tblImportOrders.Include(t => t.tblUser).Include(t => t.tblSupplier).Where(c => userSupplierIDs.Contains(c.FK_iSupplierID)&&c.iState!=0);
            return View(tblImportOrders.ToList());
        }

        // GET: Admin/ImportOrders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblImportOrder tblImportOrder = db.tblImportOrders.Find(id);
            if (tblImportOrder == null)
            {
                return HttpNotFound();
            }
            return View(tblImportOrder);
        }

        // GET: Admin/ImportOrders/Create
        public ActionResult Create()
        {
            var userId = (int)Session["idUser"];
            var suppliers = db.tblSuppliers
                              .Where(s => s.FK_iAccountID == userId && s.iState != 0)
                              .Select(s => new SelectListItem
                              {
                                  Value = s.PK_iSupplierID.ToString(),
                                  Text = s.sSupplierName
                              }).ToList();
            ViewBag.FK_iAccountID = new SelectList(db.tblUsers.Where(x => x.PK_iAccountID == userId), "PK_iAccountID", "sEmail");
            ViewBag.FK_iSupplierID = suppliers;
            return View();
        }

        // POST: Admin/ImportOrders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Admin/ImportOrders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PK_iImportOrderID,FK_iAccountID,FK_iSupplierID,dtDateAdded,sDeliver,iState")] tblImportOrder tblImportOrder)
        {
            if (ModelState.IsValid&&tblImportOrder.FK_iSupplierID.ToString()!=null&&tblImportOrder.dtDateAdded!=null&&tblImportOrder.sDeliver!=null)
            {
                db.tblImportOrders.Add(tblImportOrder);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var userId = (int)Session["idUser"];
            var suppliers = db.tblSuppliers
                              .Where(s => s.FK_iAccountID == userId && s.iState != 0)
                              .Select(s => new SelectListItem
                              {
                                  Value = s.PK_iSupplierID.ToString(),
                                  Text = s.sSupplierName
                              }).ToList();
            ViewBag.FK_iAccountID = new SelectList(db.tblUsers.Where(x => x.PK_iAccountID == userId), "PK_iAccountID", "sEmail");
            ViewBag.FK_iSupplierID = suppliers;

            ViewBag.Error = "Please fill out all required fields.";
            return View(tblImportOrder);
        }

        // GET: Admin/ImportOrders/Edit/5
        public ActionResult Edit(int? id)
        {
            var userID=(int)Session["idUser"];
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblImportOrder tblImportOrder = db.tblImportOrders.Find(id);
            if (tblImportOrder == null)
            {
                return HttpNotFound();
            }
            ViewBag.FK_iAccountID = new SelectList(db.tblUsers.Where(x=>x.PK_iAccountID== userID), "PK_iAccountID", "sEmail", tblImportOrder.FK_iAccountID);
            ViewBag.FK_iSupplierID = new SelectList(db.tblSuppliers.Where(x=>x.FK_iAccountID==userID&&x.iState!=0), "PK_iSupplierID", "sSupplierName", tblImportOrder.FK_iSupplierID);
            return View(tblImportOrder);
        }

        // POST: Admin/ImportOrders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PK_iImportOrderID,FK_iAccountID,FK_iSupplierID,dtDateAdded,sDeliver,iState")] tblImportOrder tblImportOrder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblImportOrder).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FK_iAccountID = new SelectList(db.tblUsers, "PK_iAccountID", "sEmail", tblImportOrder.FK_iAccountID);
            ViewBag.FK_iSupplierID = new SelectList(db.tblSuppliers, "PK_iSupplierID", "sSupplierName", tblImportOrder.FK_iSupplierID);
            return View(tblImportOrder);
        }

        // GET: Admin/ImportOrders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblImportOrder tblImportOrder = db.tblImportOrders.Find(id);
            if (tblImportOrder == null)
            {
                return HttpNotFound();
            }
            return View(tblImportOrder);
        }

        // POST: Admin/ImportOrders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblImportOrder tblImportOrder = db.tblImportOrders.Find(id);
            db.tblImportOrders.Remove(tblImportOrder);
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
