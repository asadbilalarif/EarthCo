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
        public IHttpActionResult GetPunchlistList()
        {
            try
            {
                List<tblPunchlist> Data = new List<tblPunchlist>();
                Data = DB.tblPunchlists.ToList();
                if (Data == null || Data.Count==0)
                {
                    return NotFound();
                }

                return Ok(Data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
           
        }

        [HttpGet]
        public IHttpActionResult GetPunchlist(int id)
        {
            try
            {
                //DB.Configuration.ProxyCreationEnabled = false;
                tblPunchlist Data = new tblPunchlist();
                Data = DB.tblPunchlists.Where(x => x.PunchlistId == id).FirstOrDefault();
                if (Data == null)
                {
                    return NotFound();
                }

                return Ok(Data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }

        [HttpPost]
        public IHttpActionResult AddPunchList([FromBody] tblPunchlist PunchList)
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
                    return Ok("Punchlist has been added successfully.");
                }
                else
                {
                    Data = DB.tblPunchlists.Select(r => r).Where(x => x.PunchlistId == PunchList.PunchlistId).FirstOrDefault();

                    if(Data==null)
                    {
                        return NotFound();
                    }

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

                    return Ok("Punchlist has been Update successfully.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult AddPunchlistDetail([FromBody] PunchlistDetailItems Punchlist)
        {
            tblPunchlistDetail Data = new tblPunchlistDetail();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);
                int UserId = 2;
                if (Punchlist.PunchlistDetailData.PunchlistDetailId== 0)
                {
                    Data = Punchlist.PunchlistDetailData;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = Punchlist.PunchlistDetailData.isActive;
                    Data.PunchlistId = Punchlist.PunchlistDetailData.PunchlistId;

                    string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    if (Punchlist.Files != null)
                    {
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.PunchlistDetailId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(Punchlist.Files.FileName)));
                        Punchlist.Files.SaveAs(path);
                        path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.PunchlistDetailId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(Punchlist.Files.FileName)));
                        Data.PhotoPath = path;
                    }
                    if (Punchlist.AfterFiles != null)
                    {
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.PunchlistDetailId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(Punchlist.AfterFiles.FileName)));
                        Punchlist.AfterFiles.SaveAs(path);
                        path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.PunchlistDetailId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(Punchlist.AfterFiles.FileName)));
                        Data.AfterPhotoPath = path;
                    }

                    DB.tblPunchlistDetails.Add(Data);
                    DB.SaveChanges();

                    if (Punchlist.PunchlistItemsData != null && Punchlist.PunchlistItemsData.Count != 0)
                    {
                        tblPunchlistItem ConData = null;

                        foreach (var item in Punchlist.PunchlistItemsData)
                        {
                            ConData = new tblPunchlistItem();
                            ConData = item;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = UserId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = UserId;
                            ConData.isActive = item.isActive;
                            ConData.PunchlistDetailId = Data.PunchlistDetailId;
                            DB.tblPunchlistItems.Add(ConData);
                            DB.SaveChanges();
                        }
                    }


                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Punchlist Detail";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    return Ok("Punchlist Detail has been added successfully.");
                }
                else
                {
                    Data = DB.tblPunchlistDetails.Where(x => x.PunchlistDetailId == Punchlist.PunchlistDetailData.PunchlistDetailId).FirstOrDefault();

                    if(Data==null)
                    {
                        return NotFound();
                    }
                    //Data = Punchlist.PunchlistDetailData;
                    Data.Notes = Punchlist.PunchlistDetailData.Notes;
                    Data.Address = Punchlist.PunchlistDetailData.Address;
                    Data.isAfterPhoto = Punchlist.PunchlistDetailData.isAfterPhoto;
                    Data.isComplete = Punchlist.PunchlistDetailData.isComplete;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = Punchlist.PunchlistDetailData.isActive;
                    Data.PunchlistId = Punchlist.PunchlistDetailData.PunchlistId;

                    string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    if (Punchlist.Files != null)
                    {
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.PunchlistDetailId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(Punchlist.Files.FileName)));
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        Punchlist.Files.SaveAs(path);
                        path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.PunchlistDetailId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(Punchlist.Files.FileName)));
                        Data.PhotoPath = path;
                    }
                    if (Punchlist.AfterFiles != null)
                    {
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.PunchlistDetailId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(Punchlist.AfterFiles.FileName)));
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        Punchlist.AfterFiles.SaveAs(path);
                        path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.PunchlistDetailId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(Punchlist.AfterFiles.FileName)));
                        Data.AfterPhotoPath = path;
                    }

                    DB.tblPunchlistDetails.Add(Data);
                    DB.SaveChanges();


                    List<tblPunchlistItem> ConList = DB.tblPunchlistItems.Where(x => x.PunchlistDetailId == Punchlist.PunchlistDetailData.PunchlistDetailId).ToList();
                    if (ConList != null && ConList.Count != 0)
                    {
                        DB.tblPunchlistItems.RemoveRange(ConList);
                        DB.SaveChanges();
                    }

                    if (Punchlist.PunchlistItemsData != null && Punchlist.PunchlistItemsData.Count != 0)
                    {
                        tblPunchlistItem ConData = null;

                        foreach (var item in Punchlist.PunchlistItemsData)
                        {
                            ConData = new tblPunchlistItem();
                            ConData = item;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = UserId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = UserId;
                            ConData.isActive = item.isActive;
                            ConData.PunchlistDetailId = Data.PunchlistDetailId;
                            DB.tblPunchlistItems.Add(ConData);
                            DB.SaveChanges();
                        }

                    }

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Punchlist Detail";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    return Ok("Punchlist Detail has been Update successfully.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult DeletePunchlistDetail(int id)
        {
            tblPunchlistDetail Data = new tblPunchlistDetail();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {
                Data = DB.tblPunchlistDetails.Select(r => r).Where(x => x.PunchlistDetailId == id).FirstOrDefault();
                if(Data==null)
                {
                    return NotFound(); 
                }

                List<tblPunchlistItem> ConList = DB.tblPunchlistItems.Where(x => x.PunchlistDetailId == id).ToList();
                if (ConList != null && ConList.Count != 0)
                {
                    DB.tblPunchlistItems.RemoveRange(ConList);
                    DB.SaveChanges();
                }

                
                DB.Entry(Data).State = EntityState.Deleted;
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete Punchlist detail";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Punchlist detail has been deleted successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult DeletePunchlist(int id)
        {
            tblPunchlist Data = new tblPunchlist();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {
                Data = DB.tblPunchlists.Select(r => r).Where(x => x.PunchlistId == id).FirstOrDefault();
                if(Data==null)
                {
                    return NotFound();
                }

                List<tblPunchlistDetail> ConList = DB.tblPunchlistDetails.Where(x => x.PunchlistId == id).ToList();
                if (ConList != null && ConList.Count != 0)
                {
                    foreach (var item in ConList)
                    {
                        List<tblPunchlistItem> ConItemList = DB.tblPunchlistItems.Where(x => x.PunchlistDetailId == item.PunchlistDetailId).ToList();
                        if (ConItemList != null && ConItemList.Count != 0)
                        {
                            DB.tblPunchlistItems.RemoveRange(ConItemList);
                            DB.SaveChanges();
                        }
                    }
                    DB.tblPunchlistDetails.RemoveRange(ConList);
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
                return Ok("Punchlist has been deleted successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        //[HttpPost]
        //public String AddPunchlistItem([FromBody] tblPunchlistItem PunchlistItem, HttpPostedFile Files, HttpPostedFile AfterFiles)
        //{
        //    tblPunchlistItem Data = new tblPunchlistItem();
        //    try
        //    {
        //        //HttpCookie cookieObj = Request.Cookies["User"];
        //        //int UserId = Int32.Parse(cookieObj["UserId"]);
        //        //int RoleId = Int32.Parse(cookieObj["RoleId"]);
        //        int UserId = 2;
        //        if (PunchlistItem.PunchlistItemId == 0)
        //        {
        //            Data = PunchlistItem;
        //            Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //            Data.CreatedBy = UserId;
        //            Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //            Data.EditBy = UserId;
        //            Data.isActive = PunchlistItem.isActive;
        //            Data.PunchlistItemId = PunchlistItem.PunchlistItemId;

        //            string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
        //            if (!Directory.Exists(folder))
        //            {
        //                Directory.CreateDirectory(folder);
        //            }
        //            if (Files != null )
        //            {
        //                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.PunchlistItemId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(Files.FileName)));
        //                Files.SaveAs(path);
        //                path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.PunchlistItemId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(Files.FileName)));
        //                Data.PhotoPath = path;
        //            }
        //            if (AfterFiles != null )
        //            {
        //                string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.PunchlistItemId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(AfterFiles.FileName)));
        //                AfterFiles.SaveAs(path);
        //                path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.PunchlistItemId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(AfterFiles.FileName)));
        //                Data.AfterPhotoPath = path;
        //            }

        //            DB.tblPunchlistItems.Add(Data);
        //            DB.SaveChanges();

        //            tblLog LogData = new tblLog();
        //            LogData.UserId = UserId;
        //            LogData.Action = "Add Punchlist Item";
        //            LogData.CreatedDate = DateTime.Now;
        //            DB.tblLogs.Add(LogData);
        //            DB.SaveChanges();
        //            return "Punchlist Item has been added successfully.";
        //        }
        //        else
        //        {
        //            return "Punchlist Item has been Update successfully.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}


    }
}
