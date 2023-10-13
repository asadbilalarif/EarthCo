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

namespace EarthCo.Controllers
{
    public class PunchListController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public List<tblPunchlist> GetPunchlistList()
        {
            List<tblPunchlist> Data = new List<tblPunchlist>();
            Data = DB.tblPunchlists.ToList();
            return Data;
        }

        [HttpGet]
        public tblPunchlist GetPunchlist(int id)
        {
            //DB.Configuration.ProxyCreationEnabled = false;
            tblPunchlist Data = new tblPunchlist();
            Data = DB.tblPunchlists.Where(x => x.PunchlistId == id).FirstOrDefault();
            return Data;
        }

        [HttpPost]
        public String AddPunchList([FromBody] tblPunchlist PunchList)
        {
            tblPunchlist Data = new tblPunchlist();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);
                int UserId = 2;
                if (PunchList.PunchlistId == 0)
                {
                    Data = PunchList;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = PunchList.isActive;
                    DB.tblPunchlists.Add(Data);
                    DB.SaveChanges();

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Punchlist";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    return "Punchlist has been added successfully.";
                }
                else
                {
                    Data = DB.tblPunchlists.Select(r => r).Where(x => x.PunchlistId == PunchList.PunchlistId).FirstOrDefault();

                    Data.Title = PunchList.Title;
                    Data.ContactName = PunchList.ContactName;
                    Data.ContactCompany = PunchList.ContactCompany;
                    Data.ContactEmail = PunchList.ContactEmail;
                    Data.AssignedTo = PunchList.AssignedTo;
                    Data.CustomerId = PunchList.CustomerId;
                    Data.ServiceRequestId = PunchList.ServiceRequestId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = PunchList.isActive;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Punchlist";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    return "Punchlist has been Update successfully.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpPost]
        public String AddPunchlistItem([FromBody] tblPunchlistItem PunchlistItem, HttpPostedFile Files, HttpPostedFile AfterFiles)
        {
            tblPunchlistItem Data = new tblPunchlistItem();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);
                int UserId = 2;
                if (PunchlistItem.PunchlistItemId == 0)
                {
                    Data = PunchlistItem;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = PunchlistItem.isActive;
                    Data.PunchlistId = PunchlistItem.PunchlistId;

                    string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    if (Files != null )
                    {
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.PunchlistItemId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(Files.FileName)));
                        Files.SaveAs(path);
                        path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.PunchlistItemId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(Files.FileName)));
                        Data.PhotoPath = path;
                    }
                    if (AfterFiles != null )
                    {
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.PunchlistItemId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(AfterFiles.FileName)));
                        AfterFiles.SaveAs(path);
                        path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.PunchlistItemId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(AfterFiles.FileName)));
                        Data.AfterPhotoPath = path;
                    }

                    DB.tblPunchlistItems.Add(Data);
                    DB.SaveChanges();

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Punchlist Item";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    return "Punchlist Item has been added successfully.";
                }
                else
                {
                    return "Punchlist Item has been Update successfully.";
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [HttpGet]
        public string DeletePunchlist(int id)
        {
            tblPunchlist Data = new tblPunchlist();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {

                List<tblPunchlistItem> ConList = DB.tblPunchlistItems.Where(x => x.PunchlistId == id).ToList();
                if (ConList != null && ConList.Count != 0)
                {
                    DB.tblPunchlistItems.RemoveRange(ConList);
                    DB.SaveChanges();
                }

                Data = DB.tblPunchlists.Select(r => r).Where(x => x.PunchlistId == id).FirstOrDefault();
                DB.Entry(Data).State = EntityState.Deleted;
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete Punchlist";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return "Punchlist has been deleted successfully.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
