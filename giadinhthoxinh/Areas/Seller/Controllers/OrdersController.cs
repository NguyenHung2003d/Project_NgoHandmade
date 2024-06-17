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
    public class OrdersController : Controller
    {
        private giadinhthoxinhEntities db = new giadinhthoxinhEntities();

        // GET: Admin/Orders
        [SupplierFilter]
        public ActionResult Index()
        {
            
            return View();
        }
        /* public ActionResult Search()
         {
             IOrderedQueryable<tblOrder> model = (IOrderedQueryable<tblOrder>)db.tblOrders.OrderByDescending(x => x.dInvoidDate);
             return View(model);
         }*/
        [HttpGet]
        public ActionResult SearchDonHang(string searchString)
        {
           // string searchString = Request.Form["searchString"];
            Session["SearchDonHang"] = searchString;
            IOrderedQueryable<tblOrder> model = (IOrderedQueryable<tblOrder>)db.tblOrders.OrderByDescending(x => x.dInvoidDate);
            if (!String.IsNullOrEmpty(searchString))
            {
                model = (IOrderedQueryable<tblOrder>)model.Where(x => x.sCustomerPhone.Contains(searchString));             
            }
            return View(model);
        }
        public ActionResult DuyetDonHang(int id)// duyet don hang tao luon hoa don cho don hang do
        {//phai kiem tra xem co du so luong san pham ko

            var item = db.tblOrders.Find(id);
            if (Session["NhanVien"] != null)
            {
                tblUser nv = (tblUser)Session["NhanVien"];
                item.sBiller = nv.sUserName;
                item.sState = "Đang giao hàng";
                db.SaveChanges();
               
            }
            else
            {
                return RedirectToAction("KhongDuThamQuyen", "PhanQuyen");
            }
            return RedirectToAction("Index");

        }
        public ActionResult HoanThanhDonHang(int id)//hoan thanh don
        {
            var item = db.tblOrders.Find(id);
            if (Session["NhanVien"] != null)
            {
                tblUser nv = (tblUser)Session["NhanVien"];
                item.sBiller = nv.sUserName;
                item.sState = "Hoàn thành";
                db.SaveChanges();
               
            }
            else
            {
                return RedirectToAction("YeuCauDangNhap", "Login");
            }
            return RedirectToAction("Index");

        }
        public ActionResult HuyDonHang(int id)//huy don hang
        {
            var item = db.tblOrders.Find(id);
            if (Session["NhanVien"] != null)
            {
                tblUser nv = (tblUser)Session["NhanVien"];
                item.sBiller = nv.sUserName;
                item.sState = "Đã hủy";
                db.SaveChanges();
                //  TaoHoaDon(item.ma_DH);
            }
            else
            {
                return RedirectToAction("YeuCauDangNhap", "Login");
            }
            return RedirectToAction("Index");
        }
        public ActionResult DonHangChuaXuLy_Partial()
        {
            var userId = (int)Session["idUser"];
            var suppliers = db.tblSuppliers.Where(x => x.FK_iAccountID == userId).Select(x => x.PK_iSupplierID).ToList();
            var products = db.tblProducts.Where(x => suppliers.Contains(x.FK_iSupplierID)).Select(x => x.PK_iProductID).ToList();
            var checkoutDetails = db.tblCheckoutDetails.Where(x => products.Contains((int)x.FK_iProductID)).Select(x=>x.FK_iOrderID).ToList();
            var orders = db.tblOrders.Where(x => checkoutDetails.Contains(x.PK_iOrderID)&&x.sState=="Chờ xác nhận").OrderByDescending(x => x.dInvoidDate).ToList();
            return PartialView(orders);
        }

        public ActionResult DonHangDangGiao_Partial()
        {
            var userId = (int)Session["idUser"];
            var suppliers = db.tblSuppliers.Where(x => x.FK_iAccountID == userId).Select(x => x.PK_iSupplierID).ToList();
            var products = db.tblProducts.Where(x => suppliers.Contains(x.FK_iSupplierID)).Select(x => x.PK_iProductID).ToList();
            var checkoutDetails = db.tblCheckoutDetails.Where(x => products.Contains((int)x.FK_iProductID)).Select(x => x.FK_iOrderID).ToList();
            var orders = db.tblOrders.Where(x => checkoutDetails.Contains(x.PK_iOrderID) && x.sState == "Đang giao hàng").OrderByDescending(x => x.dInvoidDate).ToList();
            return PartialView(orders);
        }

        public ActionResult DonHangDaXuLy_Partial()
        {
            var userId = (int)Session["idUser"];
            var suppliers = db.tblSuppliers.Where(x => x.FK_iAccountID == userId).Select(x => x.PK_iSupplierID).ToList();
            var products = db.tblProducts.Where(x => suppliers.Contains(x.FK_iSupplierID)).Select(x => x.PK_iProductID).ToList();
            var checkoutDetails = db.tblCheckoutDetails.Where(x => products.Contains((int)x.FK_iProductID)).Select(x => x.FK_iOrderID).ToList();
            var orders = db.tblOrders.Where(x => checkoutDetails.Contains(x.PK_iOrderID) && (x.sState == "Hoàn thành" || x.sState == "Đã hủy")).OrderByDescending(x => x.dInvoidDate).ToList();
            return PartialView(orders);
        }
        // GET: Admin/Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblOrder tblOrder = db.tblOrders.Find(id);
            if (tblOrder == null)
            {
                return HttpNotFound();
            }
            return View(tblOrder);
        }

        // GET: Admin/Orders/Create
        public ActionResult Create()
        {
            ViewBag.FK_iAccountID = new SelectList(db.tblUsers, "PK_iAccountID", "sEmail");
            return View();
        }

        // POST: Admin/Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "PK_iOrderID,FK_iAccountID,sCustomerName,sCustomerPhone,sDeliveryAddress,dInvoidDate,sBiller,iDeliveryMethod,fSurcharge,iPaid,iState")] tblOrder tblOrder)
        {
            if (ModelState.IsValid)
            {
                db.tblOrders.Add(tblOrder);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.FK_iAccountID = new SelectList(db.tblUsers, "PK_iAccountID", "sEmail", tblOrder.FK_iAccountID);
            return View(tblOrder);
        }

        // GET: Admin/Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblOrder tblOrder = db.tblOrders.Find(id);
            if (tblOrder == null)
            {
                return HttpNotFound();
            }
            ViewBag.FK_iAccountID = new SelectList(db.tblUsers, "PK_iAccountID", "sEmail", tblOrder.FK_iAccountID);
            return View(tblOrder);
        }

        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PK_iOrderID,FK_iAccountID,sCustomerName,sCustomerPhone,sDeliveryAddress,dInvoidDate,sBiller,iDeliveryMethod,fSurcharge,iPaid,iState")] tblOrder tblOrder)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblOrder).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.FK_iAccountID = new SelectList(db.tblUsers, "PK_iAccountID", "sEmail", tblOrder.FK_iAccountID);
            return View(tblOrder);
        }

        // GET: Admin/Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblOrder tblOrder = db.tblOrders.Find(id);
            if (tblOrder == null)
            {
                return HttpNotFound();
            }
            return View(tblOrder);
        }

        // POST: Admin/Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblOrder tblOrder = db.tblOrders.Find(id);
            db.tblOrders.Remove(tblOrder);
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
