using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class GetIrrigationData
    {
        public SPGetIrrigationData_Result IrrigationData { get; set; }
        public List<SPGetIrrigationControllerData_Result> ControllerData { get; set; }
    }
}