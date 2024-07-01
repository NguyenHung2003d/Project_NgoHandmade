namespace giadinhthoxinh.Models {
    using System;
    using System.Collections.Generic;

    public partial class tblReview {
        public int PK_iReviewID { get; set; }
        public int FK_iProductID { get; set; }
        public int FK_iAccountID { get; set; }
        public int iStarRating { get; set; }
        public Nullable<System.DateTime> dtReviewTime { get; set; }
        public string sComment { get; set; }

        public virtual tblProduct tblProduct { get; set; }
        public virtual tblUser tblUser { get; set; }
    }
}
