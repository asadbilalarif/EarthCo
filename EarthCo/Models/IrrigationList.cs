using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class IrrigationList
    {
        public int? IrrigationId { get; set; }
        public string CustomerName { get; set; }
        public DateTime CreatedDate { get; set; }
        public List<string> ControllerNumbers { get; set; }
    }
}