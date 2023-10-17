using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetEstimateItem
    {
        public int EstimateId { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string EstimateNumber { get; set; }
        public double EstimateAmount { get; set; }
        public string DescriptionofWork { get; set; }
        public DateTime DateCreated { get; set; }
        public string Status { get; set; }
        public string QBStatus { get; set; }
    }
}