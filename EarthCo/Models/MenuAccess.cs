using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class MenuAccess
    {
        public int accesslevelid { get; set; }
        public Nullable<bool> accessedit { get; set; }
        public Nullable<bool> accessdelete { get; set; }
        public Nullable<bool> accesscreate { get; set; }
        public Nullable<int> roleid { get; set; }
        public Nullable<int> menuid { get; set; }
        public Nullable<bool> isactive { get; set; }
        public tblMenu menu { get; set; }
    }
}