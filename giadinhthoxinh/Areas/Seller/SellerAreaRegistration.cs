using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace giadinhthoxinh.Areas.Seller
{
    public class SellerAreaRegistration :  AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Seller";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Seller_default",
                "Seller/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "giadinhthoxinh.Areas.Seller.Controllers" }
            );
        }
    }
}
