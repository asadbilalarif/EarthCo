using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetEstimateData
    {
        public SPGetEstimateData_Result EstimateData { get; set; }
        public List<SPGetEstimateItemData_Result> EstimateItemData { get; set; }
        public List<SPGetEstimateItemData_Result> EstimateCostItemData { get; set; }
        public List<SPGetEstimateFileData_Result> EstimateFileData { get; set; }
    }
}