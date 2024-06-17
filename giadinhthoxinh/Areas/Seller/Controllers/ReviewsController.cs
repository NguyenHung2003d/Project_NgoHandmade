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
    public class ReviewsController : Controller
    {
        private giadinhthoxinhEntities db = new giadinhthoxinhEntities();

        // GET: Admin/Reviews
        [SupplierFilter]
        public ActionResult Index()
        {

            var userId = (int)Session["idUser"];
            var suppliers = db.tblSuppliers.Where(x => x.FK_iAccountID == userId).Select(x => x.PK_iSupplierID).ToList();
            var products = db.tblProducts.Where(x => suppliers.Contains(x.FK_iSupplierID)).Select(x => x.PK_iProductID).ToList();
            var reviews = db.tblReviews.Where(x => products.Contains(x.FK_iProductID)).ToList();

            return View(reviews);
        }

        // GET: Admin/Reviews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblReview tblReview = db.tblReviews.Find(id);
            if (tblReview == null)
            {
                return HttpNotFound();
            }
            return View(tblReview);
        }

        // GET: Admin/Reviews/Create
        public ActionResult Create()
        {
            ViewBag.FK_iProductID = new SelectList(db.tblProducts, "PK_iProductID", "sProductName");
            ViewBag.FK_iAccountID = new SelectList(db.tblUsers, "PK_iAccountID", "sEmail");
            return View();
        }

        // POST: Admin/Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PK_iReviewID,FK_iProductID,FK_iAccountID,iStarRating,dtReviewTime")] tblReview tblReview)
        {
            if (ModelState.IsValid)
            {
                db.tblReviews.Add(tblReview);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FK_iProductID = new SelectList(db.tblProducts, "PK_iProductID", "sProductName", tblReview.FK_iProductID);
            ViewBag.FK_iAccountID = new SelectList(db.tblUsers, "PK_iAccountID", "sEmail", tblReview.FK_iAccountID);
            return View(tblReview);
        }

        // GET: Admin/Reviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblReview tblReview = db.tblReviews.Find(id);
            if (tblReview == null)
            {
                return HttpNotFound();
            }
            ViewBag.FK_iProductID = new SelectList(db.tblProducts, "PK_iProductID", "sProductName", tblReview.FK_iProductID);
            ViewBag.FK_iAccountID = new SelectList(db.tblUsers, "PK_iAccountID", "sEmail", tblReview.FK_iAccountID);
            return View(tblReview);
        }

        // POST: Admin/Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PK_iReviewID,FK_iProductID,FK_iAccountID,iStarRating,dtReviewTime")] tblReview tblReview)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblReview).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FK_iProductID = new SelectList(db.tblProducts, "PK_iProductID", "sProductName", tblReview.FK_iProductID);
            ViewBag.FK_iAccountID = new SelectList(db.tblUsers, "PK_iAccountID", "sEmail", tblReview.FK_iAccountID);
            return View(tblReview);
        }

        // GET: Admin/Reviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblReview tblReview = db.tblReviews.Find(id);
            if (tblReview == null)
            {
                return HttpNotFound();
            }
            return View(tblReview);
        }

        // POST: Admin/Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            tblReview tblReview = db.tblReviews.Find(id);
            db.tblReviews.Remove(tblReview);
            db.SaveChanges();
            return Json(new { success = true, message = "Xóa thành công" });
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
