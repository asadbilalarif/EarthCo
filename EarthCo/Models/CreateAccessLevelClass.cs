using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class CreateAccessLevelClass
    {
        public List<tblAccessLevel> AccessLevels { get; set; }
        public  string Role { get; set; }
        public int RoleId { get; set; }
    }
}