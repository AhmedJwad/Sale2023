using Sale.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Shared.DTOs
{
    public class SaleDTO
    {
        public int Id { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string Remarks { get; set; } = string.Empty;
    }
}
