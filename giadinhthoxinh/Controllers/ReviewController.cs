using System;
using System.Web.Mvc;
using giadinhthoxinh.Models;

namespace giadinhthoxinh.Controllers
{
    public class ReviewController : Controller
    {
        private giadinhthoxinhEntities db = new giadinhthoxinhEntities();

        [HttpPost]
        public ActionResult AddReview(int FK_iProductID, int iStarRating)
        {
            try
            {
                int FK_iAccountID = (int)Session["idUser"];

                tblReview review = new tblReview
                {
                    FK_iProductID = FK_iProductID,
                    FK_iAccountID = FK_iAccountID,
                    iStarRating = iStarRating,
                    dtReviewTime = DateTime.Now
                };

                db.tblReviews.Add(review);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}