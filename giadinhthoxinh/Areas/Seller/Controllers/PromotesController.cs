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
    public class PromotesController : Controller
    {
        private giadinhthoxinhEntities db = new giadinhthoxinhEntities();

        // GET: Admin/Promotes
        [SupplierFilter]
        public ActionResult Index(int? supplierID)
        {
            var userId = (int)Session["idUser"];
            var userSupplierIDs = db.tblSuppliers
                                    .Where(s => s.FK_iAccountID == userId && s.iState != 0)
                                    .Select(s => s.PK_iSupplierID)
                                    .ToList();
            var promotes = db.tblPromotes
                               .Where(c => userSupplierIDs.Contains(c.FK_iSupplierID));

            if (supplierID.HasValue)
            {
                promotes = promotes.Where(x => x.FK_iSupplierID == supplierID.Value);
            }

            var suppliers = db.tblSuppliers
                              .Where(s => s.FK_iAccountID == userId)
                              .Select(s => new SelectListItem
                              {
                                  Value = s.PK_iSupplierID.ToString(),
                                  Text = s.sSupplierName
                              }).ToList();

            ViewBag.Suppliers = suppliers;
            ViewBag.SelectedSupplierID = supplierID;

            return View(promotes.ToList());
        }

        // GET: Admin/Promotes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblPromote tblPromote = db.tblPromotes.Find(id);
            if (tblPromote == null)
            {
                return HttpNotFound();
            }
            return View(tblPromote);
        }

        // GET: Admin/Promotes/Create
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

            ViewBag.Suppliers = suppliers;
            return View();
        }

        // POST: Admin/Promotes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Admin/Promotes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PK_iPromoteID,FK_iSupplierID,sPromoteName,sPromoteRate,dtStartDay,dtEndDay")] tblPromote tblPromote)
        {
            if (ModelState.IsValid&&tblPromote.FK_iSupplierID.ToString()!=null&&tblPromote.sPromoteName!=null&&tblPromote.sPromoteRate!=null&&tblPromote.dtStartDay!=null&&tblPromote.dtEndDay!=null)
            {
                db.tblPromotes.Add(tblPromote);
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

            ViewBag.Suppliers = suppliers;
            ViewBag.Error = "Please fill out all required fields.";
            return View(tblPromote);
        }

        // GET: Admin/Promotes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblPromote tblPromote = db.tblPromotes.Find(id);
            if (tblPromote == null)
            {
                return HttpNotFound();
            }
            return View(tblPromote);
        }

        // POST: Admin/Promotes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PK_iPromoteID,,FK_iSupplierID,sPromoteName,sPromoteRate,dtStartDay,dtEndDay")] tblPromote tblPromote)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblPromote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblPromote);
        }

        // GET: Admin/Promotes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblPromote tblPromote = db.tblPromotes.Find(id);
            if (tblPromote == null)
            {
                return HttpNotFound();
            }
            return View(tblPromote);
        }

        // POST: Admin/Promotes/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var productsWithPromote = db.tblProducts.Where(x => x.FK_iPromoteID == id).ToList();
            if (productsWithPromote.Any())
            {
                return Json(new { success = false, message = "Vui lòng xóa các sản phẩm chứa khuyến mãi này" });
            }
            else
            {
                tblPromote tblPromote = db.tblPromotes.Find(id);
                db.tblPromotes.Remove(tblPromote);
                db.SaveChanges();
                return Json(new { success = true, message = "Xóa thành công" });
            }
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
