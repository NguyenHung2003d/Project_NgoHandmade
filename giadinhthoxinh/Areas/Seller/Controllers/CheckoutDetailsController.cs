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
    public class CheckoutDetailsController : Controller
    {
        private giadinhthoxinhEntities db = new giadinhthoxinhEntities();

        // GET: Admin/CheckoutDetails
        [SupplierFilter]
        public ActionResult Index()
        {
            var userId = (int)Session["idUser"];
            var suppliers = db.tblSuppliers.Where(x => x.FK_iAccountID == userId).Select(x=>x.PK_iSupplierID).ToList();
            var products = db.tblProducts.Where(x => suppliers.Contains(x.FK_iSupplierID)).Select(x=>x.PK_iProductID).ToList();
            var checkoutDetails = db.tblCheckoutDetails.Where(x => products.Contains((int)x.FK_iProductID)).ToList();
            return View(checkoutDetails);
        }

        // GET: Admin/CheckoutDetails/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCheckoutDetail tblCheckoutDetail = db.tblCheckoutDetails.Find(id);
            if (tblCheckoutDetail == null)
            {
                return HttpNotFound();
            }
            return View(tblCheckoutDetail);
        }

        // GET: Admin/CheckoutDetails/Create
        public ActionResult Create()
        {
            ViewBag.FK_iOrderID = new SelectList(db.tblOrders, "PK_iOrderID", "sCustomerName");
            ViewBag.FK_iProductID = new SelectList(db.tblProducts, "PK_iProductID", "sProductName");
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PK_iCheckoutDetailID,FK_iOrderID,FK_iProductID,iQuantity,fPrice")] tblCheckoutDetail tblCheckoutDetail)
        {
            if (ModelState.IsValid)
            {
                db.tblCheckoutDetails.Add(tblCheckoutDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FK_iOrderID = new SelectList(db.tblOrders, "PK_iOrderID", "sCustomerName", tblCheckoutDetail.FK_iOrderID);
            ViewBag.FK_iProductID = new SelectList(db.tblProducts, "PK_iProductID", "sProductName", tblCheckoutDetail.FK_iProductID);
            return View(tblCheckoutDetail);
        }

        // GET: Admin/CheckoutDetails/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCheckoutDetail tblCheckoutDetail = db.tblCheckoutDetails.Find(id);
            if (tblCheckoutDetail == null)
            {
                return HttpNotFound();
            }
            ViewBag.FK_iOrderID = new SelectList(db.tblOrders, "PK_iOrderID", "sCustomerName", tblCheckoutDetail.FK_iOrderID);
            ViewBag.FK_iProductID = new SelectList(db.tblProducts, "PK_iProductID", "sProductName", tblCheckoutDetail.FK_iProductID);
            return View(tblCheckoutDetail);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PK_iCheckoutDetailID,FK_iOrderID,FK_iProductID,iQuantity,fPrice")] tblCheckoutDetail tblCheckoutDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblCheckoutDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FK_iOrderID = new SelectList(db.tblOrders, "PK_iOrderID", "sCustomerName", tblCheckoutDetail.FK_iOrderID);
            ViewBag.FK_iProductID = new SelectList(db.tblProducts, "PK_iProductID", "sProductName", tblCheckoutDetail.FK_iProductID);
            return View(tblCheckoutDetail);
        }

        // GET: Admin/CheckoutDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCheckoutDetail tblCheckoutDetail = db.tblCheckoutDetails.Find(id);
            if (tblCheckoutDetail == null)
            {
                return HttpNotFound();
            }
            return View(tblCheckoutDetail);
        }

        // POST: Admin/CheckoutDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblCheckoutDetail tblCheckoutDetail = db.tblCheckoutDetails.Find(id);
            db.tblCheckoutDetails.Remove(tblCheckoutDetail);
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
