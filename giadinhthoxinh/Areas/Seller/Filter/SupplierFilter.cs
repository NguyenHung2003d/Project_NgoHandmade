using giadinhthoxinh.Models;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;

namespace giadinhthoxinh.Areas.Seller.Filter
{
    public class SupplierFilter : ActionFilterAttribute
    {
        giadinhthoxinhEntities db = new giadinhthoxinhEntities();

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Lấy userId từ session
            var userId = filterContext.HttpContext.Session["idUser"];
            if (userId == null)
            {
                // Nếu userId không tồn tại trong session, chuyển hướng đến trang đăng nhập
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "User", action = "Login", area = "" }));
                return;
            }

            // Kiểm tra xem người dùng đã có nhà cung cấp hay chưa
            var supplier = db.tblSuppliers.FirstOrDefault(s => s.FK_iAccountID == (int)userId);
            if (supplier == null)
            {
                // Chuyển hướng đến trang tạo nhà cung cấp
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new { controller = "Suppliers", action = "Create", area = "Seller" }));
            }

            base.OnActionExecuting(filterContext);
        }
    }
}