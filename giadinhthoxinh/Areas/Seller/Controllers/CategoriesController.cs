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
    public class CategoriesController : Controller
    {
        private giadinhthoxinhEntities db = new giadinhthoxinhEntities();

        // GET: Seller/Categories
        [SupplierFilter]
        public ActionResult Index(int? supplierID)
        {
            var userId = (int)Session["idUser"];
            var userSupplierIDs = db.tblSuppliers
                                    .Where(s => s.FK_iAccountID == userId &&s.iState!=0)
                                    .Select(s => s.PK_iSupplierID)
                                    .ToList();
            var categories = db.tblCategories
                               .Where(c => userSupplierIDs.Contains(c.FK_iSupplierID));

            if (supplierID.HasValue)
            {
                categories = categories.Where(x => x.FK_iSupplierID == supplierID.Value);
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

            return View(categories.ToList());
        }


        // GET: Seller/Categories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCategory tblCategory = db.tblCategories.Find(id);
            if (tblCategory == null)
            {
                return HttpNotFound();
            }
            return View(tblCategory);
        }

        // POST: Seller/Categories/Create
        public ActionResult Create()
        {
            var userId = (int)Session["idUser"];
            var suppliers = db.tblSuppliers
                              .Where(s => s.FK_iAccountID == userId&&s.iState!=0)
                              .Select(s => new SelectListItem
                              {
                                  Value = s.PK_iSupplierID.ToString(),
                                  Text = s.sSupplierName
                              }).ToList();

            ViewBag.Suppliers = suppliers;
            return View();
        }

        // POST: Seller/Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PK_iCategoryID,FK_iSupplierID,sCategoryName")] tblCategory tblCategory)
        {
            if (ModelState.IsValid && tblCategory.FK_iSupplierID.ToString() != null && tblCategory.sCategoryName != null)
            {
                db.tblCategories.Add(tblCategory);
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
            return View(tblCategory);
        }

        // GET: Seller/Categories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCategory tblCategory = db.tblCategories.Find(id);
            if (tblCategory == null)
            {
                return HttpNotFound();
            }
            return View(tblCategory);
        }

        // POST: Seller/Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PK_iCategoryID,FK_iSupplierID,sCategoryName")] tblCategory tblCategory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tblCategory);
        }

        // GET: Seller/Categories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCategory tblCategory = db.tblCategories.Find(id);
            if (tblCategory == null)
            {
                return HttpNotFound();
            }
            return View(tblCategory);
        }

        // POST: Seller/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            var productsWithCategory = db.tblProducts.Where(x => x.FK_iCategoryID == id).ToList();
            if (productsWithCategory.Any())
            {
                return Json(new { success = false, message = "Vui lòng xóa các sản phẩm chứa danh mục này" });
            }
            else
            {
                tblCategory tblCategory = db.tblCategories.Find(id);
                db.tblCategories.Remove(tblCategory);
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