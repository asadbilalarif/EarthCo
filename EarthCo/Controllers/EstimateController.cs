using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class EstimateController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public IHttpActionResult GetEstimateList()
        {
            try
            {
                List<GetEstimateItem> EstData = new List<GetEstimateItem>();
                
                List<tblEstimate> Data = new List<tblEstimate>();
                Data = DB.tblEstimates.ToList();
                if (Data == null || Data.Count==0)
                {
                    return NotFound(); // 404 - No data found
                }

                foreach (var item in Data)
                {
                    GetEstimateItem Temp = new GetEstimateItem();
                    Temp.EstimateId = (int) item.EstimateId;
                    Temp.CustomerId =(int) item.CustomerId;
                    Temp.CustomerName = item.tblCustomer.CustomerName;
                    Temp.EstimateAmount =(double) item.tblEstimateItems.Sum(s=>s.Amount);
                    Temp.DescriptionofWork = item.EstimateNotes;
                    Temp.DateCreated =(DateTime)item.CreatedDate;
                    Temp.Status =item.tblEstimateStatu.Status;
                    Temp.QBStatus =item.QBStatus;
                    EstData.Add(Temp);
                }


                return Ok(EstData); // 200 - Successful response with data
            }
            catch (Exception ex)
            {
                // Log the exception
                // You may also choose to return a more specific error response (e.g., 500 - Internal Server Error) here.
                return InternalServerError(ex);
            }


            
        }

        [HttpGet]
        public IHttpActionResult GetEstimate(int id)
        {
            try
            {
                //DB.Configuration.ProxyCreationEnabled = false;
                tblEstimate Data = new tblEstimate();
                Data = DB.tblEstimates.Where(x => x.EstimateId == id).FirstOrDefault();
                if (Data == null)
                {
                    return NotFound(); // 404 - No data found
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
        public IHttpActionResult AddEstimate()
        {
            var Data1 = HttpContext.Current.Request.Params.Get("EstimateData");
            HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

            EstimateFiles Estimate = new EstimateFiles();
            Estimate.Files = new List<HttpPostedFile>();
            for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                Estimate.Files.Add(HttpContext.Current.Request.Files[i]); ;
            }

            Estimate.EstimateData = JsonSerializer.Deserialize<tblEstimate>(Data1);

            tblEstimate Data = new tblEstimate();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);
                int UserId = 2;
                if (Estimate.EstimateData.EstimateId == 0)
                {

                    if (Estimate.EstimateData.tblEstimateItems != null && Estimate.EstimateData.tblEstimateItems.Count != 0)
                    {
                        foreach (var item in Estimate.EstimateData.tblEstimateItems)
                        {
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.Amount = item.Qty * item.Rate;
                            item.EstimateId = Data.EstimateId;
                        }
                    }

                    Data = Estimate.EstimateData;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = Estimate.EstimateData.isActive;
                    DB.tblEstimates.Add(Data);
                    DB.SaveChanges();

                    //if (Estimate.EstimateData.tblEstimateItems!= null && Estimate.EstimateData.tblEstimateItems.Count != 0)
                    //{
                    //    tblEstimateItem ConData = null;

                    //    foreach (var item in Estimate.EstimateData.tblEstimateItems)
                    //    {
                    //        ConData = new tblEstimateItem();
                    //        ConData = item;
                    //        ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.CreatedBy = UserId;
                    //        ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.EditBy = UserId;
                    //        ConData.isActive = item.isActive;
                    //        ConData.EstimateId = Data.EstimateId;
                    //        DB.tblEstimateItems.Add(ConData);
                    //        DB.SaveChanges();
                    //    }
                    //}

                    if (Estimate.Files != null && Estimate.Files.Count != 0)
                    {
                        tblEstimateFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in Estimate.Files)
                        {
                            FileData = new tblEstimateFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.EstimateId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.EstimateId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = Path.GetFileName(item.FileName);
                            FileData.Caption = FileData.FileName;
                            FileData.FilePath = path;
                            FileData.EstimateId = Data.EstimateId;
                            DB.tblEstimateFiles.Add(FileData);
                            DB.SaveChanges();
                            NameCount++;
                        }
                    }

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Estimate";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    return Ok("Estimate has been added successfully.");
                }
                else
                {
                    Data = DB.tblEstimates.Select(r => r).Where(x => x.EstimateId == Estimate.EstimateData.EstimateId).FirstOrDefault();
                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }

                    List<tblEstimateItem> ConList = DB.tblEstimateItems.Where(x => x.EstimateId == Estimate.EstimateData.EstimateId).ToList();
                    if (ConList != null && ConList.Count != 0)
                    {
                        DB.tblEstimateItems.RemoveRange(ConList);
                        DB.SaveChanges();
                    }



                    if (Estimate.EstimateData.tblEstimateItems != null && Estimate.EstimateData.tblEstimateItems.Count != 0)
                    {
                        foreach (var item in Estimate.EstimateData.tblEstimateItems)
                        {
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.Amount = item.Qty * item.Rate;
                            item.EstimateId = Data.EstimateId;
                        }
                    }


                    Data.EstimateNumber = Estimate.EstimateData.EstimateNumber;
                    Data.ServiceLocation = Estimate.EstimateData.ServiceLocation;
                    Data.Email = Estimate.EstimateData.Email;
                    Data.QBStatus = Estimate.EstimateData.QBStatus;
                    Data.EstimateNotes = Estimate.EstimateData.EstimateNotes;
                    Data.ServiceLocationNotes = Estimate.EstimateData.ServiceLocationNotes;
                    Data.PrivateNotes = Estimate.EstimateData.PrivateNotes;
                    Data.IssueDate = DateTime.Now;
                    Data.EstimateStatusId = Estimate.EstimateData.EstimateStatusId;
                    Data.CustomerId = Estimate.EstimateData.CustomerId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = Estimate.EstimateData.isActive;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    //if (Estimate.EstimateData.tblEstimateItems != null && Estimate.EstimateData.tblEstimateItems.Count != 0)
                    //{
                    //    tblEstimateItem ConData = null;

                    //    foreach (var item in Estimate.EstimateData.tblEstimateItems)
                    //    {
                    //        ConData = new tblEstimateItem();
                    //        ConData = item;
                    //        ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.CreatedBy = UserId;
                    //        ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    //        ConData.EditBy = UserId;
                    //        ConData.isActive = item.isActive;
                    //        ConData.EstimateId = Data.EstimateId;
                    //        DB.tblEstimateItems.Add(ConData);
                    //        DB.SaveChanges();
                    //    }
                    //}

                    List<tblEstimateFile> ConFList = DB.tblEstimateFiles.Where(x => x.EstimateId == Estimate.EstimateData.EstimateId).ToList();
                    if (ConFList != null && ConFList.Count != 0)
                    {
                        DB.tblEstimateFiles.RemoveRange(ConFList);
                        DB.SaveChanges();
                    }

                    if (Estimate.Files != null && Estimate.Files.Count != 0)
                    {
                        tblEstimateFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in Estimate.Files)
                        {
                            FileData = new tblEstimateFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading"), Path.GetFileName("UploadFile" + Data.EstimateId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading", Path.GetFileName("UploadFile" + Data.EstimateId.ToString() + NameCount + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = Path.GetFileName(item.FileName);
                            FileData.Caption = "";
                            FileData.FilePath = path;
                            FileData.EstimateId = Data.EstimateId;
                            DB.tblEstimateFiles.Add(FileData);
                            DB.SaveChanges();
                            NameCount++;
                        }
                    }


                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Estimate";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    return Ok("Estimate has been Update successfully.");
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult DeleteEstimate(int id)
        {
            tblEstimate Data = new tblEstimate();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {
                Data = DB.tblEstimates.Select(r => r).Where(x => x.EstimateId == id).FirstOrDefault();
                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }


                List<tblEstimateFile> ConFile = DB.tblEstimateFiles.Where(x => x.EstimateId == id).ToList();
                if (ConFile != null && ConFile.Count != 0)
                {
                    DB.tblEstimateFiles.RemoveRange(ConFile);
                    DB.SaveChanges();
                }

                List<tblEstimateItem> ConList = DB.tblEstimateItems.Where(x => x.EstimateId == id).ToList();
                if (ConList != null && ConList.Count != 0)
                {
                    DB.tblEstimateItems.RemoveRange(ConList);
                    DB.SaveChanges();
                }

                
                DB.Entry(Data).State = EntityState.Deleted;
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete Estimate";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Estimate has been deleted successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        public IHttpActionResult UpdateAllSelectedEstimateStatus(UpdateStatus ParaData)
        {
            tblEstimate Data = new tblEstimate();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {
                foreach (var item in ParaData.id)
                {
                    Data = DB.tblEstimates.Select(r => r).Where(x => x.EstimateId == item).FirstOrDefault();
                    Data.EstimateStatusId = ParaData.StatusId;
                    DB.Entry(Data);
                    DB.SaveChanges();
                }

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Update All Selected Estimate status";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("All selected Estimate status has been updated successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }


        [HttpPost]
        public IHttpActionResult DeleteAllSelectedEstimate(DeleteSelected ParaData)
        {
            tblEstimate Data = new tblEstimate();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {
                foreach (var item in ParaData.id)
                {
                    List<tblEstimateItem> ConList = DB.tblEstimateItems.Where(x => x.EstimateId == item).ToList();
                    if (ConList != null && ConList.Count != 0)
                    {
                        DB.tblEstimateItems.RemoveRange(ConList);
                        DB.SaveChanges();
                    }
                    Data = DB.tblEstimates.Select(r => r).Where(x => x.EstimateId == item).FirstOrDefault();
                    DB.Entry(Data).State = EntityState.Deleted;
                    DB.SaveChanges();
                }

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete All Selected Estimate";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("All selected Estimate has been deleted successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
