using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class PunchListController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public IHttpActionResult GetPunchlistList()
        {
            try
            {
                List<int> PunchlistIds = new List<int>();
                List<GetPunchlistList> Data = new List<GetPunchlistList>();
                tblPunchlist PunchlistData = new tblPunchlist();
                

                PunchlistIds = DB.tblPunchlists.Select(s=>s.PunchlistId).ToList();
                if (PunchlistIds == null || PunchlistIds.Count==0)
                {
                    return NotFound();
                }
                else
                {
                    foreach (int item in PunchlistIds)
                    {
                        GetPunchlistList Temp = new GetPunchlistList();
                        Temp.DetailDataList = new List<GetPunchlistDetailList>();
                        List<SPGetPunchlistDetailData_Result> DetailTemp = new List<SPGetPunchlistDetailData_Result>();
                        Temp.Data = DB.SPGetPunchlistData(item).FirstOrDefault();
                        PunchlistData = DB.tblPunchlists.FirstOrDefault();
                        DetailTemp = DB.SPGetPunchlistDetailData(item).ToList();
                        foreach (SPGetPunchlistDetailData_Result Detailitem in DetailTemp)
                        {
                            GetPunchlistDetailList PDLT = new GetPunchlistDetailList();
                            
                            PDLT.DetailData = Detailitem;
                            PDLT.ItemData = DB.SPGetPunchlistItemData(Detailitem.PunchlistDetailId).ToList();
                            Temp.DetailDataList.Add(PDLT);
                        }
                        Temp.CustomerName = PunchlistData.tblUser.FirstName + " " + PunchlistData.tblUser.LastName;
                        Temp.AssignToName = PunchlistData.tblUser1.FirstName + " " + PunchlistData.tblUser1.LastName;
                        Temp.Status = PunchlistData.tblPunchlistStatu.Status;
                        Data.Add(Temp);
                    }
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
                SPGetPunchlistData_Result Data = new SPGetPunchlistData_Result();
                Data = DB.SPGetPunchlistData(id).FirstOrDefault();
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

        [HttpGet]
        public IHttpActionResult GetPunchlistDetail(int id)
        {
            try
            {
                //DB.Configuration.ProxyCreationEnabled = false;
                SPGetPunchlistDetailData_Result Data = new SPGetPunchlistDetailData_Result();
                List<SPGetPunchlistItemData_Result> ItemData = new List<SPGetPunchlistItemData_Result>();
                Data = DB.SPGetPunchlistDetailData(id).FirstOrDefault();
                ItemData = DB.SPGetPunchlistItemData(Data.PunchlistDetailId).ToList();


                GetPunchlistDetailList GetData = new GetPunchlistDetailList();
                if (Data == null)
                {
                    return NotFound();
                }
                else
                {
                    GetData.DetailData = Data;
                    GetData.ItemData = ItemData;

                }

                return Ok(GetData);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            
        }

        [HttpGet]
        public IHttpActionResult GetPunchlistStatus()
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblPunchlistStatu> Data = new List<tblPunchlistStatu>();
                Data = DB.tblPunchlistStatus.ToList();
                if (Data == null || Data.Count == 0)
                {
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    return ResponseMessage(responseMessage);
                }

                return Ok(Data); // 200 - Successful response with data
            }
            catch (Exception ex)
            {
                // Log the exception
                // You may also choose to return a more specific error response (e.g., 500 - Internal Server Error) here.
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
                var userIdClaim = User.Identity as ClaimsIdentity;
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                if (PunchList.PunchlistId == 0)
                {
                    Data = PunchList;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.tblPunchlists.Add(Data);
                    DB.SaveChanges();

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Punchlist";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    //return Ok("Punchlist has been added successfully.");
                    return Ok(new { Id = Data.PunchlistId, Message = "Punchlist has been added successfully." });
                }
                else
                {
                    Data = DB.tblPunchlists.Select(r => r).Where(x => x.PunchlistId == PunchList.PunchlistId).FirstOrDefault();

                    if(Data==null)
                    {
                        return NotFound();
                    }

                    Data.Title = PunchList.Title;
                    Data.CustomerId = PunchList.CustomerId;
                    Data.ContactId = PunchList.ContactId;
                    Data.ServiceLocationId = PunchList.ServiceLocationId;
                    Data.AssignedTo = PunchList.AssignedTo;
                    Data.ServiceRequestId = PunchList.ServiceRequestId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Punchlist";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    //return Ok("Punchlist has been Update successfully.");
                    return Ok(new { Id = Data.PunchlistId, Message = "Punchlist has been Update successfully." });
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult AddPunchlistDetail()
        {
            tblPunchlistDetail Data = new tblPunchlistDetail();
            try
            {

                var Data1 = HttpContext.Current.Request.Params.Get("PunchlistDetailData");
                HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

                PunchlistDetailItems Punchlist = new PunchlistDetailItems();
                //Punchlist.Files = new HttpPostedFile();
                if (HttpContext.Current.Request.Files["AfterFiles"] != null)
                {
                    Punchlist.AfterFiles = HttpContext.Current.Request.Files["AfterFiles"];
                }
                if (HttpContext.Current.Request.Files["Files"] != null)
                {
                    Punchlist.Files = HttpContext.Current.Request.Files["Files"];
                }



                Punchlist.PunchlistDetailData = JsonSerializer.Deserialize<tblPunchlistDetail>(Data1);

                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);
                var userIdClaim = User.Identity as ClaimsIdentity;
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                if (Punchlist.PunchlistDetailData.PunchlistDetailId== 0)
                {

                    if (Punchlist.PunchlistDetailData.tblPunchlistItems != null && Punchlist.PunchlistDetailData.tblPunchlistItems.Count != 0)
                    {
                        foreach (var item in Punchlist.PunchlistDetailData.tblPunchlistItems)
                        {
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.CreatedBy = UserId;
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditBy = UserId;
                            item.isActive = true;
                            item.isDelete = false;
                            item.PunchlistDetailId = Data.PunchlistDetailId;
                        }
                    }


                    Data = Punchlist.PunchlistDetailData;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
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

                    //if (Punchlist.PunchlistDetailData.tblPunchlistItems != null && Punchlist.PunchlistDetailData.tblPunchlistItems.Count != 0)
                    //{
                    //    tblPunchlistItem ConData = null;

                    //    foreach (var item in Punchlist.PunchlistDetailData.tblPunchlistItems)
                    //    {
                    //        ConData = new tblPunchlistItem();
                    //        ConData = item;
                    //        ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.CreatedBy = UserId;
                    //        ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.EditBy = UserId;
                    //        ConData.isActive = item.isActive;
                    //        ConData.PunchlistDetailId = Data.PunchlistDetailId;
                    //        DB.tblPunchlistItems.Add(ConData);
                    //        DB.SaveChanges();
                    //    }
                    //}


                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Punchlist Detail";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    //return Ok("Punchlist Detail has been added successfully.");
                    return Ok(new { Id = Data.PunchlistDetailId, Message = "Punchlist Detail has been added successfully." });
                }
                else
                {
                    Data = DB.tblPunchlistDetails.Where(x => x.PunchlistDetailId == Punchlist.PunchlistDetailData.PunchlistDetailId).FirstOrDefault();

                    if(Data==null)
                    {
                        return NotFound();
                    }

                    List<tblPunchlistItem> ConList = DB.tblPunchlistItems.Where(x => x.PunchlistDetailId == Punchlist.PunchlistDetailData.PunchlistDetailId).ToList();
                    if (ConList != null && ConList.Count != 0)
                    {
                        DB.tblPunchlistItems.RemoveRange(ConList);
                        DB.SaveChanges();
                    }

                    if (Punchlist.PunchlistDetailData.tblPunchlistItems != null && Punchlist.PunchlistDetailData.tblPunchlistItems.Count != 0)
                    {
                        foreach (var item in Punchlist.PunchlistDetailData.tblPunchlistItems)
                        {
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.CreatedBy = UserId;
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditBy = UserId;
                            item.isActive = true;
                            item.isDelete = false;
                            item.PunchlistDetailId = Data.PunchlistDetailId;
                        }
                    }

                    //Data = Punchlist.PunchlistDetailData;
                    Data.Notes = Punchlist.PunchlistDetailData.Notes;
                    Data.isAfterPhoto = Punchlist.PunchlistDetailData.isAfterPhoto;
                    Data.isComplete = Punchlist.PunchlistDetailData.isComplete;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
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


                    //List<tblPunchlistItem> ConList = DB.tblPunchlistItems.Where(x => x.PunchlistDetailId == Punchlist.PunchlistDetailData.PunchlistDetailId).ToList();
                    //if (ConList != null && ConList.Count != 0)
                    //{
                    //    DB.tblPunchlistItems.RemoveRange(ConList);
                    //    DB.SaveChanges();
                    //}

                    if (Punchlist.PunchlistDetailData.tblPunchlistItems != null && Punchlist.PunchlistDetailData.tblPunchlistItems.Count != 0)
                    {
                        tblPunchlistItem ConData = null;

                        foreach (var item in Punchlist.PunchlistDetailData.tblPunchlistItems)
                        {
                            ConData = new tblPunchlistItem();
                            ConData = item;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = UserId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = UserId;
                            ConData.isActive = true;
                            ConData.isDelete = false;
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
                    //return Ok("Punchlist Detail has been Update successfully.");
                    return Ok(new { Id = Data.PunchlistDetailId, Message = "Punchlist Detail has been Update successfully." });
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
            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                Data = DB.tblPunchlistDetails.Select(r => r).Where(x => x.PunchlistDetailId == id).FirstOrDefault();
                if(Data==null)
                {
                    return NotFound(); 
                }

                //List<tblPunchlistItem> ConList = DB.tblPunchlistItems.Where(x => x.PunchlistDetailId == id).ToList();
                //if (ConList != null && ConList.Count != 0)
                //{
                //    DB.tblPunchlistItems.RemoveRange(ConList);
                //    DB.SaveChanges();
                //}


                Data.isDelete = true;
                Data.EditBy = UserId;
                Data.EditDate = DateTime.Now;
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
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
            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                Data = DB.tblPunchlists.Select(r => r).Where(x => x.PunchlistId == id).FirstOrDefault();
                if(Data==null)
                {
                    return NotFound();
                }

                //List<tblPunchlistDetail> ConList = DB.tblPunchlistDetails.Where(x => x.PunchlistId == id).ToList();
                //if (ConList != null && ConList.Count != 0)
                //{
                //    foreach (var item in ConList)
                //    {
                //        List<tblPunchlistItem> ConItemList = DB.tblPunchlistItems.Where(x => x.PunchlistDetailId == item.PunchlistDetailId).ToList();
                //        if (ConItemList != null && ConItemList.Count != 0)
                //        {
                //            DB.tblPunchlistItems.RemoveRange(ConItemList);
                //            DB.SaveChanges();
                //        }
                //    }
                //    DB.tblPunchlistDetails.RemoveRange(ConList);
                //    DB.SaveChanges();
                //}

                Data.isDelete = true;
                Data.EditBy = UserId;
                Data.EditDate = DateTime.Now;
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
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
