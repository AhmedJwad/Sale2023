using Sale.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace Sale.Shared.Entities
{
    public class Order
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        [Display(Name = "Inventory")]
        [Required(ErrorMessage = "The {0} field is required.")]
        public DateTime Date { get; set; }
        public User? User { get; set; }
        public string? UserId { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comments")]
        public string? Remarks { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public ICollection<SaleDetail>? SaleDetails { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "Lines")]
        public int Lines => SaleDetails == null ? 0 : SaleDetails.Count;

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Amount")]
        public float Quantity => SaleDetails == null ? 0 : SaleDetails.Sum(sd => sd.Quantity);

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Value")]
        public decimal Value => SaleDetails == null ? 0 : SaleDetails.Sum(sd => sd.Value);

    }
}
