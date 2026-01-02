using System.Collections.Generic;

namespace PhapClinicX.Models.ViewModels
{
    public class PackageSelectionGroup
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public List<PackageSelectionItem> Packages { get; set; } = new();
    }

    public class PackageSelectionItem
    {
        public int PackageId { get; set; }

        public string PackageName { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }

    public class ProductSelectionGroup
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public List<ProductSelectionItem> Products { get; set; } = new();
    }

    public class ProductSelectionItem
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public decimal Price { get; set; }
    }
}
