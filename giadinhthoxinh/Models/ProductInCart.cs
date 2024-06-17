using System;
using System.Linq;

namespace giadinhthoxinh.Models {
    public class ProductInCart {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string LinkImage { get; set; }
        public decimal Price { get; set; } // Use decimal for monetary values
        public string Unit { get; set; } // Make it PascalCase for consistency
        public int Quantity { get; set; } // Rename to Quantity for clarity

        public decimal TotalPrice => Quantity * Price; // Calculated property for total price

        public ProductInCart(int productId, int quantity) {
            using (giadinhthoxinhEntities db = new giadinhthoxinhEntities()) {
                var product = db.tblProducts.FirstOrDefault(n => n.PK_iProductID == productId);

                if (product == null) {
                    // Handle the case where the product is not found
                    throw new ArgumentException("Invalid product ID");
                }

                ProductID = product.PK_iProductID;
                ProductName = product.sProductName;
                LinkImage = product.sImage;
                Price = (decimal)(product.fPrice ?? 0);  // Assuming fPrice is already a decimal
                Quantity = quantity;
                Unit = product.sUnit;

            }
        }
    }
}
