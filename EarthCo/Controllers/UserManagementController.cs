using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class UserManagementController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public IHttpActionResult Users()
        {
            try
            {
                List<tblUser> Users = new List<tblUser>();
                Users = DB.tblUsers.ToList();
                if (Users == null || Users.Count==0)
                {
                    return NotFound();
                }

                return Ok(Users);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }

        [HttpGet]
        public IHttpActionResult GetUser(int id)
        {
            try
            {
                tblUser User = new tblUser();
                User = DB.tblUsers.Where(x => x.UserId == id).FirstOrDefault();
                if (User == null)
                {
                    return NotFound();
                }

                return Ok(User);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
           
        }

        [HttpPost]
        public IHttpActionResult AddUser([FromBody] tblUser User)
        {
            tblUser Data = new tblUser();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);
                int UserId = 2;
                if (User.UserId == 0)
                {
                    if (DB.tblUsers.Select(r => r).Where(x => x.Email == User.Email).FirstOrDefault() == null)
                    {
                        Data = User;
                        byte[] EncDataBtye = new byte[User.Password.Length];
                        EncDataBtye = System.Text.Encoding.UTF8.GetBytes(User.Password);
                        Data.Password = Convert.ToBase64String(EncDataBtye);

                        //string folder = Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        //if (!Directory.Exists(folder))
                        //{
                        //    Directory.CreateDirectory(folder);
                        //}
                        //string path = null;

                        //if (Image != null)
                        //{
                        //    path = Path.Combine(Server.MapPath("~/Uploading"), Path.GetFileName(Image.FileName));
                        //    Image.SaveAs(path);
                        //    path = Path.Combine("\\Uploading", Path.GetFileName(Image.FileName));
                        //    Data.ImagePath = path;
                        //}

                        Data.Address = User.Address;
                        Data.Phone = User.Phone;
                        Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                        Data.CreatedBy = UserId;
                        Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                        Data.EditBy = UserId;
                        Data.isActive = User.isActive;
                        DB.tblUsers.Add(Data);
                        DB.SaveChanges();




                        tblLog LogData = new tblLog();
                        LogData.UserId = UserId;
                        LogData.Action = "Add User";
                        LogData.CreatedDate = DateTime.Now;
                        DB.tblLogs.Add(LogData);
                        DB.SaveChanges();
                        return Ok("User has been added successfully.");
                        
                    }
                    else
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("User Already Exsist!!!");
                        return ResponseMessage(responseMessage);
                    }
                }
                else
                {
                    var check = DB.tblUsers.Select(r => r).Where(x => x.Email == User.Email).FirstOrDefault();
                    if (check == null || check.UserId == User.UserId)
                    {
                        Data = DB.tblUsers.Select(r => r).Where(x => x.UserId == User.UserId).FirstOrDefault();

                        if(Data==null)
                        {
                            return NotFound();
                        }

                        //string folder = Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        //if (!Directory.Exists(folder))
                        //{
                        //    Directory.CreateDirectory(folder);
                        //}
                        //string path = null;

                        //if (Image != null)
                        //{
                        //    path = Path.Combine(Server.MapPath("~/Uploading"), Path.GetFileName(Image.FileName));
                        //    Image.SaveAs(path);
                        //    path = Path.Combine("\\Uploading", Path.GetFileName(Image.FileName));
                        //    Data.ImagePath = path;
                        //}



                        Data.UserId = User.UserId;
                        Data.FirstName = User.FirstName;
                        Data.LastName = User.LastName;
                        Data.Email = User.Email;
                        Data.Address = User.Address;
                        Data.Phone = User.Phone;
                        Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                        Data.EditBy = UserId;
                        Data.isActive = User.isActive;

                        Data.RoleId = User.RoleId;
                        if (User.Password != null)
                        {
                            byte[] EncDataBtye = new byte[User.Password.Length];
                            EncDataBtye = System.Text.Encoding.UTF8.GetBytes(User.Password);
                            Data.Password = Convert.ToBase64String(EncDataBtye);
                        }
                        else
                        {
                            Data.Password = Data.Password;
                        }
                        Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                        Data.EditBy = UserId;
                        DB.Entry(Data);
                        DB.SaveChanges();

                        tblLog LogData = new tblLog();
                        LogData.UserId = UserId;
                        LogData.Action = "Update User";
                        LogData.CreatedDate = DateTime.Now;
                        DB.tblLogs.Add(LogData);
                        DB.SaveChanges();

                        return Ok("User has been Update successfully.");
                    }
                    else
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("User Already Exsist!!!");
                        return ResponseMessage(responseMessage);
                    }

                }


            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }

        }

        [HttpGet]
        public IHttpActionResult DeleteUser(int id)
        {
            tblUser Data = new tblUser();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            int? RoleID = 0;
            try
            {
                Data = DB.tblUsers.Select(r => r).Where(x => x.UserId == id).FirstOrDefault();
                if(Data==null)
                {
                    return NotFound();
                }

                RoleID = Data.RoleId;
                DB.Entry(Data).State = EntityState.Deleted;
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete User";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("User has been deleted successfully.");
            }
            catch (Exception ex)
            {

                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult Roles()
        {
            try
            {
                List<tblRole> Roles = new List<tblRole>();
                Roles = DB.tblRoles.ToList();
                if (Roles == null || Roles.Count == 0)
                {
                    return NotFound();
                }

                return Ok(Roles);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }
        
        [HttpGet]
        public IHttpActionResult GetRole(int id)
        {
            try
            {
                tblRole Role = new tblRole();
                Role = DB.tblRoles.Where(x => x.RoleId == id).FirstOrDefault();
                if (Role == null)
                {
                    return NotFound();
                }

                return Ok(Role);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }
        
        //[HttpPost]
        //public String AddRole([FromBody] tblRole RoleData)
        //{
        //    tblRole Data = new tblRole();
        //    try
        //    {
        //        //HttpCookie cookieObj = Request.Cookies["User"];
        //        //int UserId = Int32.Parse(cookieObj["UserId"]);
        //        //int RoleId = Int32.Parse(cookieObj["RoleId"]);
        //        int UserId = 2;
        //        if (RoleData.RoleId == 0)
        //        {

        //            if (DB.tblRoles.Select(r => r).Where(x => x.Role == RoleData.Role).FirstOrDefault() == null)
        //            {
        //                Data.Role = RoleData.Role;
        //                Data.isActive = true;
        //                Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
        //                Data.CreatedBy = UserId;
        //                Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
        //                Data.EditBy = UserId;
        //                DB.tblRoles.Add(Data);
        //                DB.SaveChanges();

                        

        //                tblLog LogData = new tblLog();
        //                LogData.UserId = UserId;
        //                LogData.Action = "Create Role";
        //                LogData.CreatedDate = DateTime.Now;
        //                DB.tblLogs.Add(LogData);
        //                DB.SaveChanges();

        //                return "Role has been added successfully.";
        //            }
        //            else
        //            {
        //                return "Role already exsist!!!";
        //            }
        //        }
        //        else
        //        {
        //            var check = DB.tblRoles.Select(r => r).Where(x => x.Role == RoleData.Role).FirstOrDefault();
        //            if (check == null || check.RoleId == RoleData.RoleId)
        //            {
        //                Data = DB.tblRoles.Select(r => r).Where(x => x.RoleId == RoleData.RoleId).FirstOrDefault();


        //                Data.Role = RoleData.Role;
        //                Data.isActive = true;
        //                Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
        //                Data.CreatedBy = UserId;
        //                Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
        //                Data.EditBy = UserId;
        //                DB.tblRoles.Add(Data);
        //                DB.SaveChanges();

        //                tblLog LogData = new tblLog();
        //                LogData.UserId = UserId;
        //                LogData.Action = "Update Role";
        //                LogData.CreatedDate = DateTime.Now;
        //                DB.tblLogs.Add(LogData);
        //                DB.SaveChanges();

        //                return "Role has been Update successfully.";
        //            }
        //            else
        //            {
        //                return "Role already exsist!!!";
        //            }

        //        }


        //    }
        //    catch (Exception ex)
        //    {

        //        return ex.Message;
        //    }

        //}

        [HttpGet]
        public IHttpActionResult DeleteRole(int id)
        {
            tblRole Data = new tblRole();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 1;
            int? RoleID = 0;
            try
            {
                Data = DB.tblRoles.Select(r => r).Where(x => x.RoleId == id).FirstOrDefault();
                if(Data==null)
                {
                    return NotFound();
                }

                RoleID = Data.RoleId;
                DB.Entry(Data).State = EntityState.Deleted;
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete Role";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Role has been deleted successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        //public string Roles(int? id)
        //{
        //    List<tblRole> Roles = DB.tblRoles.Where(x => x.isActive == true).ToList();
        //    ViewBag.MenuList = DB.tblMenus.ToList();

        //    try
        //    {
        //        var check = DB.tblAccessLevels.Where(x => x.RoleId == id).FirstOrDefault();
        //        if (id != null && id != 0 && check != null)
        //        {
        //            ViewBag.SelectedRole = DB.tblRoles.Where(x => x.RoleId == id).FirstOrDefault();

        //            ViewBag.SelectedMenuAccess = (from h in DB.tblMenus
        //                                          join t in DB.tblAccessLevels.Where(x => x.RoleId == id) on h.MenuId equals t.MenuId into gj
        //                                          from acc in gj.DefaultIfEmpty()
        //                                              //where acc.roleid == id
        //                                          select new MenuAccess
        //                                          {
        //                                              menu = h,
        //                                              //accesslevelid = acc.accesslevelid,
        //                                              accessedit = acc.EditAccess,
        //                                              accessdelete = acc.DeleteAccess,
        //                                              accesscreate = acc.CreateAccess,
        //                                              isactive = acc.isActive,
        //                                              roleid = id,
        //                                              menuid = h.MenuId,

        //                                          }).ToList();
        //        }
        //        else
        //        {
        //            if (id != null && id != 0)
        //            {
        //                ViewBag.SelectedRole = DB.tblRoles.Where(x => x.RoleId == id).FirstOrDefault();
        //            }
        //            else
        //            {
        //                ViewBag.SelectedRole = null;
        //            }
        //            ViewBag.SelectedMenuAccess = null;
        //        }
        //    }
        //    catch (Exception ex)
        //    {

        //        return ex.Message;
        //    }
        //}

        [HttpPost]
        public IHttpActionResult CreateAccessLevel(CreateAccessLevelClass PataData)
        {
            tblRole Data = new tblRole();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                int UserId = 2;

                if (PataData.RoleId != 0)
                {

                    tblRole check = DB.tblRoles.Select(r => r).Where(x => x.Role == PataData.Role).FirstOrDefault();
                    if (check == null || check.RoleId == PataData.RoleId)
                    {
                        Data = DB.tblRoles.Select(r => r).Where(x => x.RoleId == PataData.RoleId).FirstOrDefault();
                        if(Data==null)
                        {
                            return NotFound();
                        }


                        Data.Role = PataData.Role;
                        Data.isActive = true;
                        Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                        Data.EditBy = UserId;
                        DB.Entry(Data);
                        DB.SaveChanges();
                        foreach (tblAccessLevel AccessLevel in PataData.AccessLevels)
                        {
                            var Del = DB.tblAccessLevels.Select(r => r).Where(x => x.RoleId == AccessLevel.RoleId && x.MenuId == AccessLevel.MenuId).FirstOrDefault();
                            if (Del != null)
                            {
                                DB.tblAccessLevels.Remove(Del);
                            }
                        }

                        foreach (tblAccessLevel AccessLevel in PataData.AccessLevels)
                        {
                            AccessLevel.RoleId = Data.RoleId;
                            AccessLevel.CreatedDate = DateTime.Now;
                            AccessLevel.CreatedBy = UserId;
                            AccessLevel.EditDate = DateTime.Now;
                            AccessLevel.EditBy = UserId;
                            //AccessLevel.tblMenu = DB.tblMenus.Select(r => r).Where(x => x.MenuID == AccessLevel.MenuID).FirstOrDefault();
                            //AccessLevel.tblRole = DB.tblRoles.Select(r => r).Where(x => x.RoleID == AccessLevel.RoleID).FirstOrDefault();
                            DB.tblAccessLevels.Add(AccessLevel);
                            DB.SaveChanges();
                        }
                        DB.SaveChanges();

                        tblLog LogData = new tblLog();
                        LogData.UserId = UserId;
                        LogData.Action = "Update Role";
                        LogData.CreatedDate = DateTime.Now;
                        DB.tblLogs.Add(LogData);
                        DB.SaveChanges();

                        return Ok("Role has been Update successfully.");
                    }
                    else
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("Role already exsist.");
                        return ResponseMessage(responseMessage);
                    }

                }
                else
                {
                    if (DB.tblRoles.Select(r => r).Where(x => x.Role == PataData.Role).FirstOrDefault() == null)
                    {
                        Data.Role = PataData.Role;
                        Data.isActive = true;
                        Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                        Data.CreatedBy = UserId;
                        Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                        Data.EditBy = UserId;
                        DB.tblRoles.Add(Data);
                        DB.SaveChanges();

                        foreach (tblAccessLevel AccessLevel in PataData.AccessLevels)
                        {
                            var Del = DB.tblAccessLevels.Select(r => r).Where(x => x.RoleId == Data.RoleId && x.MenuId == AccessLevel.MenuId).FirstOrDefault();
                            if (Del != null)
                            {
                                DB.tblAccessLevels.Remove(Del);
                            }
                        }

                        foreach (tblAccessLevel AccessLevel in PataData.AccessLevels)
                        {
                            AccessLevel.RoleId = Data.RoleId;
                            AccessLevel.CreatedDate = DateTime.Now;
                            AccessLevel.CreatedBy = UserId;
                            AccessLevel.EditDate = DateTime.Now;
                            AccessLevel.EditBy = UserId;
                            //AccessLevel.tblMenu = DB.tblMenus.Select(r => r).Where(x => x.MenuID == AccessLevel.MenuID).FirstOrDefault();
                            //AccessLevel.tblRole = DB.tblRoles.Select(r => r).Where(x => x.RoleID == AccessLevel.RoleID).FirstOrDefault();
                            DB.tblAccessLevels.Add(AccessLevel);
                            DB.SaveChanges();
                        }
                        DB.SaveChanges();

                        tblLog LogData = new tblLog();
                        LogData.UserId = UserId;
                        LogData.Action = "Create Role";
                        LogData.CreatedDate = DateTime.Now;
                        DB.tblLogs.Add(LogData);
                        DB.SaveChanges();

                        return Ok("Role has been added successfully.");
                    }
                    else
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("Role already exsist.");
                        return ResponseMessage(responseMessage);
                    }
                }

            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return Ok("Role has been added successfully.");
        }

        [HttpGet]
        public IHttpActionResult GetAccessLevel(int RoleId = 0)
        {
            List<MenuAccess> MenuAccess = new List<MenuAccess>();
            List<tblMenu> Menu = new List<tblMenu>();
            DB.Configuration.ProxyCreationEnabled = false;
            try
            {
                Menu = DB.tblMenus.ToList();
                MenuAccess = (from h in DB.tblMenus
                              join t in DB.tblAccessLevels.Where(x => x.RoleId == RoleId) on h.MenuId equals t.MenuId into gj
                              from acc in gj.DefaultIfEmpty()
                              select new MenuAccess
                              {
                                  menu = h,
                                  //accesslevelid = acc.accesslevelid,
                                  accessedit = acc.EditAccess,
                                  accessdelete = acc.DeleteAccess,
                                  accesscreate = acc.CreateAccess,
                                  isactive = acc.isActive,
                                  roleid = RoleId,
                                  menuid = h.MenuId,

                              }).ToList();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

            return Ok(MenuAccess);
        }

    }
}
