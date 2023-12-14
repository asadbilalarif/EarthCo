using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class QBErrorClass
    {
        public class ErrorResponse
        {
            public Fault Fault { get; set; }
            public DateTime Time { get; set; }
        }

        public class Fault
        {
            public List<ErrorDetail> Error { get; set; }
            public string Type { get; set; }
        }

        public class ErrorDetail
        {
            public string Message { get; set; }
            public string Detail { get; set; }
            public string Code { get; set; }
            public string Element { get; set; }
        }
    }
}