using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class PunchlistPhotoOnlyFile
    {
        public tblPunchlistPhotoOnly PunchlistPhotoOnlyData { get; set; }
        public List<HttpPostedFile> Photos { get; set; }
        public List<HttpPostedFile> AdditionalPhotos { get; set; }
    }
}