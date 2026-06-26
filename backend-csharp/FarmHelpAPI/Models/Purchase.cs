namespace FarmHelpAPI.Models
{
    /// <summary>
    /// 采购需求 / 农产品需求
    /// </summary>
    public class Purchase
    {
        public int Id { get; set; }

        public int BuyerId { get; set; }

        public string ProductType { get; set; } = string.Empty;  // 需求类型

        public int Quantity { get; set; }  // 需求数量

        public decimal? Budget { get; set; }  // 预算

        public DateTime? Deadline { get; set; }  // 截止日期

        [System.ComponentModel.DataAnnotations.StringLength(50)]
        public string? ContactName { get; set; }  // 联系人

        [System.ComponentModel.DataAnnotations.StringLength(30)]
        public string? ContactPhone { get; set; }  // 联系电话

        public string Status { get; set; } = "active"; // active, completed

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public class PurchaseCreateRequest
    {
        public int BuyerId { get; set; }
        public string ProductType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal? Budget { get; set; }
        public string? Deadline { get; set; }
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }
    }

    public class PurchaseResponse
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public string ProductType { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal? Budget { get; set; }
        public string? Deadline { get; set; }
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? CreatedAt { get; set; }
    }
}
