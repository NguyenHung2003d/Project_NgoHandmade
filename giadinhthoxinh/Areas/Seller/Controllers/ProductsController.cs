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
    public class ProductsController : Controller
    {
        private giadinhthoxinhEntities db = new giadinhthoxinhEntities();

        // GET: Admin/Products
        [SupplierFilter]
        public ActionResult Index()
        {
            var userId = (int)Session["idUser"];
            var userSupplierIDs = db.tblSuppliers
                                    .Where(s => s.FK_iAccountID == userId && s.iState != 0)
                                    .Select(s => s.PK_iSupplierID)
                                    .ToList();
            var tblProducts = db.tblProducts.Include(t => t.tblCategory).Include(t => t.tblPromote).Where(c => userSupplierIDs.Contains(c.FK_iSupplierID)&&c.iState!=0);
            return View(tblProducts.ToList());
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblProduct tblProduct = db.tblProducts.Find(id);
            if (tblProduct == null)
            {
                return HttpNotFound();
            }
            return View(tblProduct);
        }

        // GET: Admin/Products/Create
        public ActionResult Create()
        {
            var userId = (int?)Session["idUser"];
            var suppliers = db.tblSuppliers
                              .Where(s => s.FK_iAccountID == userId && s.iState != 0)
                              .Select(s => new SelectListItem
                              {
                                  Value = s.PK_iSupplierID.ToString(),
                                  Text = s.sSupplierName
                              }).ToList();
            ViewBag.FK_iSupplierID = new SelectList(suppliers, "Value", "Text");

            // Initialize empty lists for categories and promotes, they will be populated via AJAX
            ViewBag.FK_iCategoryID = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.FK_iPromoteID = new SelectList(Enumerable.Empty<SelectListItem>());

            return View();
        }


        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Admin/Products/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "FK_iSupplierID,FK_iCategoryID,FK_iPromoteID,sProductName,sDescribe,fPrice,sColor,sSize,sImage,sUnit,iQuantity")] tblProduct tblProduct, HttpPostedFileBase fileAnh)
        {
            if (ModelState.IsValid && fileAnh != null && fileAnh.ContentLength > 0&& tblProduct.FK_iSupplierID.ToString()!=null&&tblProduct.FK_iCategoryID.ToString()!=null&&tblProduct.FK_iPromoteID.ToString()!=null)
            {
                // Save file
                string rootFolder = Server.MapPath("/Data/");
                string pathImage = rootFolder + fileAnh.FileName;
                fileAnh.SaveAs(pathImage);
                tblProduct.sImage = "/Data/" + fileAnh.FileName;

                db.tblProducts.Add(tblProduct);
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
            ViewBag.FK_iSupplierID = new SelectList(suppliers, "Value", "Text");

            // Initialize empty lists for categories and promotes, they will be populated via AJAX
            ViewBag.FK_iCategoryID = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewBag.FK_iPromoteID = new SelectList(Enumerable.Empty<SelectListItem>());

            ViewBag.Error = "Please fill out all required fields and upload an image.";
            return View(tblProduct);
        }

        public JsonResult GetCategoriesAndPromotions(int supplierId)
        {
            var categories = db.tblCategories
                .Where(c => c.FK_iSupplierID == supplierId)
                .Select(c => new
                {
                    Value = c.PK_iCategoryID,
                    Text = c.sCategoryName
                }).ToList();

            var promotes = db.tblPromotes
                .Where(p => p.FK_iSupplierID == supplierId)
                .Select(p => new
                {
                    Value = p.PK_iPromoteID,
                    Text = p.sPromoteName
                }).ToList();

            return Json(new { categories, promotes }, JsonRequestBehavior.AllowGet);
        }


        // GET: Admin/Products/Edit/5
        public ActionResult Edit(int? id)
        {
            var userID =(int)Session["idUser"];
            var suppliers = db.tblSuppliers.Where(x=>x.FK_iAccountID== userID&&x.iState!=0).Select(x=>x.PK_iSupplierID).ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblProduct tblProduct = db.tblProducts.Find(id);
            if (tblProduct == null)
            {
                return HttpNotFound();
            }
            ViewBag.FK_iCategoryID = new SelectList(db.tblCategories.Where(x=>suppliers.Contains(x.FK_iSupplierID)), "PK_iCategoryID", "sCategoryName", tblProduct.FK_iCategoryID);
            ViewBag.FK_iPromoteID = new SelectList(db.tblPromotes.Where(x => suppliers.Contains(x.FK_iSupplierID)), "PK_iPromoteID", "sPromoteName", tblProduct.FK_iPromoteID);
            return View(tblProduct);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PK_iProductID,FK_iSupplierID,FK_iCategoryID,FK_iPromoteID,sProductName,sDescribe,fPrice,sColor,sSize,sImage,sUnit")] tblProduct tblProduct, HttpPostedFileBase fileAnh)
        {
            if (ModelState.IsValid)
            {
                //Lưu file
                string rootFolder = Server.MapPath("/Data/");
                string pathImage = rootFolder + fileAnh.FileName;
                fileAnh.SaveAs(pathImage);
                //Lưu url hình ảnh
                tblProduct.sImage = "/Data/" + fileAnh.FileName;


                db.Entry(tblProduct).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FK_iCategoryID = new SelectList(db.tblCategories, "PK_iCategoryID", "sCategoryName", tblProduct.FK_iCategoryID);
            ViewBag.FK_iPromoteID = new SelectList(db.tblPromotes, "PK_iPromoteID", "sPromoteName", tblProduct.FK_iPromoteID);
            return View(tblProduct);
        }

        // GET: Admin/Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblProduct tblProduct = db.tblProducts.Find(id);
            if (tblProduct == null)
            {
                return HttpNotFound();
            }
            return View(tblProduct);
        }

        // POST: Admin/Products/Delete/5
        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblProduct tblProduct = db.tblProducts.Find(id);
            if (tblProduct != null)
            {
                tblProduct.iState = 0;
                db.Entry(tblProduct).State = EntityState.Modified;
                db.SaveChanges();
            }
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
