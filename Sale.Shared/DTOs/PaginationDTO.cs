using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Shared.DTOs
{
   public class PaginationDTO
    {
        public int Id { get; set; }
        public int page { get; set; } = 1;
        public int REcordNumber { get; set; } = 10;
        public string? Filter { get; set; }

    }
}
