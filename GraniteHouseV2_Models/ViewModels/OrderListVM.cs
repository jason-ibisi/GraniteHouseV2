﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace GraniteHouseV2_Models.ViewModels
{
    public class OrderListVM
    {
        public IEnumerable<OrderHeader> OrderList { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
        public string Status { get; set; }
    }
}
