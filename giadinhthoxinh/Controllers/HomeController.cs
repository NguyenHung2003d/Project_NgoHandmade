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

        public ActionResult Index(int? categoryId, int? page)
        {
            int productInPage = 10;
            int pageNumber = (page ?? 1);
            IQueryable<tblProduct> products;

            if (categoryId.HasValue)
            {
                products = db.tblProducts.Where(x => x.FK_iCategoryID == categoryId.Value&&x.iState!=0).OrderByDescending(x => x.PK_iProductID);
                ViewBag.CategoryId = categoryId.Value;
            }
            else
            {
                products = db.tblProducts.Where(x=>x.iState!=0).OrderByDescending(x => x.PK_iProductID);
                ViewBag.CategoryId = null;
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

        public ActionResult ProductDetail(int? id) {
            if (id == null) {
                return RedirectToAction("Index"); // Hoặc trả về trang lỗi tùy chỉnh
            }
            var item = db.tblProducts.Find(id);
            return View(item);
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