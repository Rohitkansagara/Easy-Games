using EasyGames.Class.NewFolder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EasyGames.Class.Dtos
{
    public class StockItemDto : ModelBaseDto
    {
        public string Name { get; set; } = string.Empty;

        public StockCategory Category { get; set; }

        public decimal Price { get; set; }//per one Quantity.

        public long Quantity { get; set; }

        public long AvailableQuantity { get; set; }

        public string? Description { get; set; } // Optional field
    }

    public class StockItemListDto : ModelBaseDto
    {
        public string Name { get; set; }

        public StockCategory Category { get; set; }

        public decimal Price { get; set; }

        public long Quantity { get; set; }

        public long AvailableQuantity { get; set; }

        public string? Description { get; set; }

        public static Expression<Func<StockItem, StockItemListDto>> ToDto = e => new StockItemListDto
        {
            Name = e.Name,
            Category = e.Category,
            Price = e.Price,
            AvailableQuantity = e.AvailableQuantity,
            Description = e.Description,
            Quantity = e.Quantity,

            Id = e.Id,
            CreatedById = e.CreatedById,
            CreatedOn = e.CreatedOn,
            ModifiedById = e.ModifiedById,
            ModifiedOn = e.ModifiedOn,
            Disabled = e.Disabled,
            EnableDisabled = e.EnableDisabled,
            RowVersion = e.RowVersion
        };
    }
}
