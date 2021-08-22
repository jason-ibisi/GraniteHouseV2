﻿using System.ComponentModel.DataAnnotations;

namespace GraniteHouseV2.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }
        public string Name { get; set; }
        public int DisplayOrder { get; set; }
    }
}
