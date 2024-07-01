using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using giadinhthoxinh.Models;
using PagedList;

namespace giadinhthoxinh.Controllers
{
    public class HomeController : Controller
    {
        private giadinhthoxinhEntities db = new giadinhthoxinhEntities(); // Use private field

        public ActionResult Index(int? page, int? categoryId, string sortOrder = "asc") {
            int productInPage = 10;
            int pageNumber = (page ?? 1);
            ViewBag.SortOrder = sortOrder;

            IQueryable<tblProduct> products = db.tblProducts.Where(x => x.iState != 0); // Start with products that are not deleted

            if (categoryId.HasValue) {
                products = products.Where(x => x.FK_iCategoryID == categoryId.Value);
                ViewBag.CategoryId = categoryId.Value;
            } else {
                ViewBag.CategoryId = null; // No category selected
            }

            // Apply sorting based on the selected order
            switch (sortOrder) {
                case "desc":
                    products = products.OrderByDescending(p => p.fPrice);
                    break;
                case "asc": // Explicitly handle ascending
                default:
                    products = products.OrderBy(p => p.fPrice);
                    break;
            }

            var categories = db.tblCategories.ToList();
            ViewBag.Categories = new SelectList(categories, "PK_iCategoryID", "sCategoryName");

            return View(products.ToPagedList(pageNumber, productInPage));
        }


        public ActionResult Search(string searchString, int? page) {
            Session["Search"] = searchString;
            int productInPage = 10;
            int pageNumber = (page ?? 1);

            //giadinhthoxinhEntities1 db = new giadinhthoxinhEntities1();
            var model = db.tblProducts.OrderByDescending(x => x.PK_iProductID); // Avoid unnecessary cast

            if (!String.IsNullOrEmpty(searchString)) {
                model = model.Where(x => x.sProductName.Contains(searchString)).OrderByDescending(x => x.PK_iProductID); // Preserve the original order
            }

            return View(model.ToPagedList(pageNumber, productInPage));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Ngo Handmade là nền tảng thương mại điện tử mới dành riêng cho các sản phẩm thủ công Việt Nam. Nó nhằm mục đích kết nối người mua và người bán theo cách thân thiện với người dùng, cung cấp các công cụ giúp các nghệ nhân quảng bá và bán các sản phẩm sáng tạo của họ";

            return View();
        }

        public ActionResult ProductDetail(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var product = db.tblProducts.Find(id);

            var userId = Session["idUser"];
            bool hasOrdered = false;

            if (userId != null)
            {
                hasOrdered = db.tblOrders.Any(o => o.FK_iAccountID == (int)userId && o.tblCheckoutDetails.Any(cd => cd.FK_iProductID == id));
            }

            ViewBag.HasOrdered = hasOrdered;
            ViewBag.Reviews = db.tblReviews.Where(r => r.FK_iProductID == id).ToList();

            return View(product);
        }

        public ActionResult Checkout()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        public ActionResult Promote()
        {
            var promotes = db.tblPromotes.ToList();

            return View(promotes);
        }
    }
}