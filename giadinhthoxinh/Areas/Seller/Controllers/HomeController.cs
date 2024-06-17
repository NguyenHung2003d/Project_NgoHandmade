using giadinhthoxinh.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace giadinhthoxinh.Areas.Seller.Controllers
{
    public class HomeController : Controller
    {
        private giadinhthoxinhEntities db = new giadinhthoxinhEntities();

        public ActionResult Index(int? categoryId, int? page)
        {
            if (Session["NhanVien"] == null)
            {
                return RedirectToAction("KhongDuThamQuyen", "PhanQuyen");
            }
            else
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
        }
      
    }
}