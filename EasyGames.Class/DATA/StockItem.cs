using EasyGames.Class.DATA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyGames.Class.NewFolder
{
    public enum StockCategory
    {
        None = 0,
        // Core
        Book = 1,
        Game = 2,
        Toy = 3,

        // Electronics & Gadgets
        Electronics = 4,
        Mobile = 5,
        Laptop = 6,
        ComputerAccessory = 7,
        Audio = 8,
        Camera = 9,
        SmartWatch = 10,

        // Fashion & Lifestyle
        Clothing = 11,
        Footwear = 12,
        Jewelry = 13,
        Beauty = 14,
        PersonalCare = 15,
        Watch = 16,
        Bag = 17,

        // Home & Living
        Furniture = 18,
        KitchenAppliance = 19,
        HomeDecor = 20,
        Lighting = 21,
        CleaningSupply = 22,
        Bedding = 23,

        // Grocery & Food
        Grocery = 24,
        Beverage = 25,
        Snack = 26,
        FreshProduce = 27,
        FrozenFood = 28,

        // Office & Stationery
        Stationery = 29,
        OfficeSupply = 30,
        ArtSupply = 31,

        // Sports & Outdoors
        Sports = 32,
        Fitness = 33,
        OutdoorGear = 34,
        CycleAccessory = 35,

        // Media & Digital
        Music = 36,
        Movie = 37,
        Software = 38,
        Ebook = 39,
        Subscription = 40,

        // Automotive & Tools
        Automotive = 41,
        Tool = 42,
        Hardware = 43,
        CarAccessory = 44,

        // Kids & Baby
        BabyProduct = 45,
        KidsWear = 46,
        SchoolSupply = 47,

        // Health & Medical
        Medicine = 48,
        Supplement = 49,
        MedicalDevice = 50,

        // Miscellaneous
        PetSupply = 51,
        Gift = 52,
        Accessory = 53,
        Other = 99
    }

    public class StockItem : ModelBase
    {
        public string Name { get; set; } = string.Empty;

        public StockCategory Category { get; set; } = StockCategory.None; // Enum: Book, Game, Toy

        public decimal Price { get; set; }//per one Quantity.
        public long Quantity { get; set; }

        public long AvailableQuantity { get; set; }

        public string? Description { get; set; } // Optional field
    }


}
