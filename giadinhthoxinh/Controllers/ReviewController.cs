using System;
using System.Linq;
using System.Web.Mvc;
using giadinhthoxinh.Models;

namespace giadinhthoxinh.Controllers
{
    public class ReviewController : Controller
    {
        private giadinhthoxinhEntities db = new giadinhthoxinhEntities();

        [HttpPost]
        public ActionResult AddReview(int FK_iProductID, int iStarRating, string sComment)
        {
            try
            {
                if (Session["idUser"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để bình luận" });
                }
                int FK_iAccountID = (int)Session["idUser"];

                bool hasOrdered = db.tblOrders.Any(o => o.FK_iAccountID == FK_iAccountID && o.tblCheckoutDetails.Any(cd => cd.FK_iProductID == FK_iProductID));
                if (!hasOrdered)
                {
                    return Json(new { success = false, message = "Bạn cần mua sản phẩm này trước khi có thể đánh giá nó." });
                }

                tblReview review = new tblReview
                {
                    FK_iProductID = FK_iProductID,
                    FK_iAccountID = FK_iAccountID,
                    iStarRating = iStarRating,
                    dtReviewTime = DateTime.Now,
                    sComment = sComment,
                };

                db.tblReviews.Add(review);
                db.SaveChanges();

                return Json(new { success = true, message = "Đã thêm đánh giá thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult EditReview(int PK_iReviewID, int iStarRating, string sComment)
        {
            try
            {
                if (Session["idUser"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để chỉnh sửa bình luận của bạn" });
                }
                int FK_iAccountID = (int)Session["idUser"];

                tblReview review = db.tblReviews.Find(PK_iReviewID);
                if (review == null || review.FK_iAccountID != FK_iAccountID)
                {
                    return Json(new { success = false, message = "Bạn chỉ có thể chỉnh sửa đánh giá của riêng bạn." });
                }

                review.iStarRating = iStarRating;
                review.sComment = sComment;
                review.dtReviewTime = DateTime.Now;

                db.SaveChanges();

                return Json(new { success = true, message = "Đánh giá đã được chỉnh sửa thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult DeleteReview(int PK_iReviewID)
        {
            try
            {
                if (Session["idUser"] == null)
                {
                    return Json(new { success = false, message = "Vui lòng đăng nhập để xóa bình luận của bạn" });
                }
                int FK_iAccountID = (int)Session["idUser"];

                tblReview review = db.tblReviews.Find(PK_iReviewID);
                if (review == null || review.FK_iAccountID != FK_iAccountID)
                {
                    return Json(new { success = false, message = "Bạn chỉ có thể xóa đánh giá của riêng bạn." });
                }

                db.tblReviews.Remove(review);
                db.SaveChanges();

                return Json(new { success = true, message = "Đã xóa đánh giá thành công" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}