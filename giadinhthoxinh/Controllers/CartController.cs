using giadinhthoxinh.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace giadinhthoxinh.Controllers {
    public class CartController : Controller {
        private static giadinhthoxinhEntities db = new giadinhthoxinhEntities();
        private List<tblProduct> listProduct = db.tblProducts.ToList();

        public List<ProductInCart> LayGioHang() {
            Cart lstGioHang = Session["GioHang"] as Cart;
            if (lstGioHang == null) {
                lstGioHang = new Cart();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang.lstproduct;
        }

        [HttpPost]
        public ActionResult ThemGioHang(int iMasp, string strURL) {
            int soluong = int.Parse(Request.Form["quantity_value"]);
            tblProduct sp = db.tblProducts.SingleOrDefault(n => n.PK_iProductID == iMasp);
            if (sp == null) {
                Response.StatusCode = 404;
                return null;
            }
            List<ProductInCart> giohang = LayGioHang();
            ProductInCart sanpham = giohang.Find(n => n.ProductID == iMasp);
            if (sanpham == null) {
                sanpham = new ProductInCart(iMasp, soluong);
                giohang.Add(sanpham);
                return Redirect(strURL);
            } else {
                sanpham.Quantity++;
                return Redirect(strURL);
            }
        }

        public ActionResult XoaGioHang(int iMaSP) {
            tblProduct sp = db.tblProducts.SingleOrDefault(n => n.PK_iProductID == iMaSP);
            if (sp == null) {
                Response.StatusCode = 404;
                return null;
            }
            List<ProductInCart> lstGioHang = LayGioHang();
            ProductInCart sanpham = lstGioHang.SingleOrDefault(n => n.ProductID == iMaSP);
            if (sanpham != null) {
                lstGioHang.RemoveAll(n => n.ProductID == iMaSP);
            }
            if (lstGioHang.Count == 0) {
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Cart");
        }

        private int TongSoLuong() {
            int iTongSoLuong = 0;
            List<ProductInCart> lstGioHang = LayGioHang();
            if (lstGioHang != null) {
                iTongSoLuong = lstGioHang.Sum(n => n.Quantity);
            }
            return iTongSoLuong;
        }

        public decimal TongTien() {
            decimal dTongTien = 0;
            List<ProductInCart> lstGioHang = LayGioHang();
            if (lstGioHang != null) {
                dTongTien = lstGioHang.Sum(n => n.TotalPrice);
            }
            return dTongTien;
        }

        public ActionResult Cart() {
            if (Session["GioHang"] == null) {
                return RedirectToAction("Index", "Home");
            }
            List<ProductInCart> lstGioHang = LayGioHang();
            ViewBag.soluongsp = TongSoLuong();
            ViewBag.tongtien = TongTien();
            return View(lstGioHang);
        }

        public ActionResult CartPartial() {
            if (TongSoLuong() == 0) {
                ViewBag.TongSoLuong = 0;
                return PartialView();
            }
            ViewBag.TongSoLuong = TongSoLuong();
            return PartialView();
        }

        public ActionResult Checkout(string shopAddress) {
            var productId = (int)Session["productId"];

            if (Session["User"] == null) {
                return RedirectToAction("Login", "User");
            } else {
                List<ProductInCart> lstGioHang = LayGioHang();
                ViewBag.TongTien = lstGioHang.Where(x => x.ProductID == productId).Sum(n => n.TotalPrice);
                ViewBag.SanPham = lstGioHang;
                ViewBag.NguoiNhan = (tblUser)Session["User"];
                ViewBag.ShopAddress = shopAddress;
            }

            return View();
        }

        [HttpPost]
        public ActionResult ShopAddress(int productId) {
            Session["productId"] = productId;
            var suppliers = db.tblProducts.Where(x => x.PK_iProductID == productId).Select(x => x.FK_iSupplierID).ToList();
            var shopAddress = db.tblSuppliers.Where(x => suppliers.Contains(x.PK_iSupplierID)).Select(x => x.sAddress).SingleOrDefault();
            return RedirectToAction("Checkout", "Cart", new { shopAddress = shopAddress });
        }

        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity, string[] size) {
            var product = db.tblProducts.Find(productId);
            if (product == null) {
                return HttpNotFound();
            }

            if (product.iQuantity < quantity) {
                TempData["Error"] = "Not enough stock available.";
                return RedirectToAction("ProductDetail", "Home", new { id = productId });
            }

            List<ProductInCart> giohang = LayGioHang();
            ProductInCart sanpham = giohang.Find(n => n.ProductID == productId);
            if (sanpham == null) {
                sanpham = new ProductInCart(productId, quantity);
                giohang.Add(sanpham);
            } else {
                sanpham.Quantity += quantity;
            }

            return RedirectToAction("Cart");
        }

        [HttpPost]
        public ActionResult Order(int totalAmount) {
            Session["sDeliveryAddress"] = Request.Form["sDeliveryAddress"];
            Session["sCustomerName"] = Request.Form["sCustomerName"];
            Session["sCustomerPhone"] = Request.Form["sCustomerPhone"];
            Session["TongTien"] = totalAmount.ToString();
            if (Session["User"] == null || Session["User"].ToString() == "") {
                return RedirectToAction("Login", "User");
            }
            if (Session["User"] == null) {
                RedirectToAction("Index", "Home");
            }

            if (int.Parse(Request.Form["iDeliveryMethod"]) == 1) {
                tblOrder ddh = new tblOrder();
                List<ProductInCart> gh = LayGioHang();

                tblUser kh = (tblUser)Session["User"];
                ddh.FK_iAccountID = kh.PK_iAccountID;
                ddh.sDeliveryAddress = Request.Form["sDeliveryAddress"];
                ddh.sCustomerName = Request.Form["sCustomerName"];
                ddh.sCustomerPhone = Request.Form["sCustomerPhone"];
                ddh.iDeliveryMethod = 1;
                ddh.iPaid = 0;
                ddh.dInvoidDate = DateTime.Now;
                ddh.fSurcharge = float.Parse(TongTien().ToString());
                ddh.sState = "Chờ xác nhận";
                ddh.iTotal = totalAmount;
                db.tblOrders.Add(ddh);
                db.SaveChanges();
                foreach (var item in gh) {
                    tblCheckoutDetail ctDH = new tblCheckoutDetail();
                    ctDH.FK_iOrderID = ddh.PK_iOrderID;
                    ctDH.FK_iProductID = item.ProductID;
                    ctDH.iQuantity = item.Quantity;
                    ctDH.fPrice = (double)item.Price;
                    db.tblCheckoutDetails.Add(ctDH);

                    var product = db.tblProducts.Find(item.ProductID);
                    if (product != null) {
                        product.iQuantity -= item.Quantity;
                    }
                }
                db.SaveChanges();
                Session["GioHang"] = null;
                return RedirectToAction("Index", "Home");
            }

            var vnpayUrl = CreateVNPayPaymentRequest(totalAmount);
            return Redirect(vnpayUrl);
        }

        private string CreateVNPayPaymentRequest(decimal totalAmount) {
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", "790M6AMP");
            vnpay.AddRequestData("vnp_Amount", ((int)(totalAmount * 100)).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", Request.UserHostAddress);
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang");
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", Url.Action("VNPayReturn", "Cart", null, Request.Url.Scheme));
            vnpay.AddRequestData("vnp_TxnRef", DateTime.Now.Ticks.ToString());

            string paymentUrl = vnpay.CreateRequestUrl("https://sandbox.vnpayment.vn/paymentv2/vpcpay.html", "FH9F2MY20V6O8BEWWISJXCU4CTR9CWHC");
            return paymentUrl;
        }

        public ActionResult VNPayReturn() {
            var vnpay = new VnPayLibrary();
            var vnpayData = Request.QueryString;
            foreach (string s in vnpayData) {
                vnpay.AddResponseData(s, vnpayData[s]);
            }

            long vnpayTranId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            string vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            string vnp_SecureHash = Request.QueryString["vnp_SecureHash"];

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, "FH9F2MY20V6O8BEWWISJXCU4CTR9CWHC");

            if (checkSignature) {
                if (vnp_ResponseCode == "00") {
                    tblOrder ddh = new tblOrder();
                    List<ProductInCart> gh = LayGioHang();
                    tblUser kh = (tblUser)Session["User"];

                    ddh.FK_iAccountID = kh.PK_iAccountID;
                    ddh.sDeliveryAddress = (string)Session["sDeliveryAddress"];
                    ddh.sCustomerName = (string)Session["sCustomerName"];
                    ddh.sCustomerPhone = (string)Session["sCustomerPhone"];
                    ddh.iDeliveryMethod = 0;
                    ddh.iPaid = 1;
                    ddh.iTotal = Convert.ToInt32(Session["TongTien"]); ;
                    ddh.dInvoidDate = DateTime.Now;
                    ddh.fSurcharge = float.Parse(TongTien().ToString());
                    ddh.sState = "Chờ xác nhận";
                    db.tblOrders.Add(ddh);
                    db.SaveChanges();

                    foreach (var item in gh) {
                        tblCheckoutDetail ctDH = new tblCheckoutDetail();
                        ctDH.FK_iOrderID = ddh.PK_iOrderID;
                        ctDH.FK_iProductID = item.ProductID;
                        ctDH.iQuantity = item.Quantity;
                        ctDH.fPrice = (double)item.Price;
                        db.tblCheckoutDetails.Add(ctDH);
                        ctDH.iQuantity -= item.Quantity;
                    }
                    db.SaveChanges();

                    Session["GioHang"] = null;
                    ViewBag.Message = "Thanh toán thành công!";
                } else {
                    ViewBag.Message = "Thanh toán không thành công!";
                }
            } else {
                ViewBag.Message = "Có lỗi xảy ra trong quá trình xử lý!";
            }

            return View();
        }
    }
}