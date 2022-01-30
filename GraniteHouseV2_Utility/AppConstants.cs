﻿using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GraniteHouseV2_Utility
{
    public class AppConstants
    {
        public const string ImagePath = @"\images\product\";
        public const string SessionCart = "ShoppingCartSession";
        public const string SessionInquiryId = "InquirySession";

        public const string AdminRole = "Admin";
        public const string CustomerRole = "Customer";

        public const string AdminEmail = "twista009@yahoo.com";

        public const string CategoryName = "Category";
        public const string ApplicationTypeName = "ApplicationType";

        public const string Success = "Success";
        public const string Error = "Error";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefunded = "Refunded";

        public static IEnumerable<string> StatusList = new ReadOnlyCollection<string>(
            new List<string>
            {
                StatusPending, StatusApproved, StatusInProcess, StatusShipped, StatusCancelled, StatusRefunded
            });

    }
}
