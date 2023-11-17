using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
                Data = DB.tblEstimates.Where(x=>x.isDelete==false).ToList();
                if (Data == null || Data.Count==0)
                {
                    return NotFound(); // 404 - No data found
                }

                foreach (tblEstimate item in Data)
                {
                    GetEstimateItem Temp = new GetEstimateItem();
                    Temp.EstimateId = (int) item.EstimateId;
                    Temp.CustomerId =(int) item.CustomerId;
                    Temp.CustomerName = item.tblUser.FirstName+" "+ item.tblUser.LastName;
                    Temp.RegionalManager = item.tblUser1.FirstName+" "+ item.tblUser1.LastName;
                    Temp.Date = item.CreatedDate;
                    Temp.Status =item.tblEstimateStatu.Status;
                    Temp.EstimateNumber = item.EstimateNumber;
                    Temp.DescriptionofWork = item.EstimateNotes;
                    //if(item.tblPurchaseOrders!=null && item.tblPurchaseOrders.Count != 0)
                    //{
                    //    Temp.PurchaseOrderNumber = item.tblPurchaseOrders.FirstOrDefault().PurchaseOrderNumber;
                    //}
                    //if(item.tblPurchaseOrders != null && item.tblPurchaseOrders.Count != 0)
                    //{
                    //    Temp.BillNumber = item.tblPurchaseOrders.FirstOrDefault().tblBills.FirstOrDefault().BillNumber;
                    //}
                    //if(item.tblInvoices != null && item.tblInvoices.Count != 0)
                    //{
                    //    Temp.InvoiceNumber = item.tblInvoices.FirstOrDefault().InvoiceNumber;
                    //}
                    
                    Temp.ProfitPercentage = item.ProfitPercentage;
                    Temp.EstimateAmount =(double) item.tblEstimateItems.Sum(s=>s.Amount);
                    EstData.Add(Temp);
                }

                return Ok(EstData); // 200 - Successful response with data
            }
            catch (Exception ex)
            {
                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = new StringContent(ex.InnerException.InnerException.Message);

                // You can add custom headers if needed
                //responseMessage.Headers.Add("Custom-Header", "Some-Value");

                return ResponseMessage(responseMessage);
            }


            
        }

        [HttpGet]
        public IHttpActionResult GetEstimate(int id)
        {
            try
            {
                SPGetEstimateData_Result Data = new SPGetEstimateData_Result();
                List<SPGetEstimateItemData_Result> ItemData = new List<SPGetEstimateItemData_Result>();
                List<SPGetEstimateItemData_Result> CostItemData = new List<SPGetEstimateItemData_Result>();
                List<SPGetEstimateFileData_Result> FileData = new List<SPGetEstimateFileData_Result>();
                Data = DB.SPGetEstimateData(id).FirstOrDefault();
                ItemData = DB.SPGetEstimateItemData(id).Where(x=>x.isCost==false).ToList();
                CostItemData = DB.SPGetEstimateItemData(id).Where(x=>x.isCost==true).ToList();
                FileData = DB.SPGetEstimateFileData(id).ToList();
                
                GetEstimateData GetData = new GetEstimateData();
                if (Data == null)
                {
                    return NotFound(); // 404 - No data found
                }
                else
                {
                    GetData.EstimateData = Data;
                    GetData.EstimateItemData = ItemData;
                    GetData.EstimateCostItemData = CostItemData;
                    GetData.EstimateFileData = FileData;

                }

                return Ok(GetData); // 200 - Successful response with data
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
                            item.CreatedBy = UserId;
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditBy = UserId;
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.Amount = item.Qty * item.Rate;
                            item.EstimateId = Data.EstimateId;
                            item.isActive = true;
                            item.isDelete = false;
                        }
                    }

                    Data = Estimate.EstimateData;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
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
                            FileData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.CreatedBy = UserId;
                            FileData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.EditBy = UserId;
                            FileData.isActive = true;
                            FileData.isDelete = false;
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
                    //return Ok("Estimate has been added successfully.");
                    return Ok(new { Id = Data.EstimateId, Message = "Estimate has been added successfully." });
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
                            item.CreatedBy = UserId;
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditBy = UserId;
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.Amount = item.Qty * item.Rate;
                            item.EstimateId = Data.EstimateId;
                            item.isActive = true;
                            item.isDelete = false;
                        }
                    }


                    Data.EstimateNumber = Estimate.EstimateData.EstimateNumber;
                    Data.ServiceLocationId = Estimate.EstimateData.ServiceLocationId;
                    Data.Email = Estimate.EstimateData.Email;
                    Data.IssueDate = DateTime.Now;
                    Data.CustomerId = Estimate.EstimateData.CustomerId;
                    Data.ContactId = Estimate.EstimateData.ContactId;
                    Data.RegionalManagerId = Estimate.EstimateData.RegionalManagerId;
                    Data.AssignTo = Estimate.EstimateData.AssignTo;
                    Data.RequestedBy = Estimate.EstimateData.RequestedBy;
                    Data.EstimateStatusId = Estimate.EstimateData.EstimateStatusId;
                    Data.QBStatus = Estimate.EstimateData.QBStatus;
                    Data.EstimateNotes = Estimate.EstimateData.EstimateNotes;
                    Data.ServiceLocationNotes = Estimate.EstimateData.ServiceLocationNotes;
                    Data.PrivateNotes = Estimate.EstimateData.PrivateNotes;
                    Data.Tax = Estimate.EstimateData.Tax;
                    Data.Discount = Estimate.EstimateData.Discount;
                    Data.Shipping = Estimate.EstimateData.Shipping;
                    Data.Profit = Estimate.EstimateData.Profit;
                    Data.ProfitPercentage = Estimate.EstimateData.ProfitPercentage;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    if (Estimate.EstimateData.tblEstimateItems != null && Estimate.EstimateData.tblEstimateItems.Count != 0)
                    {
                        tblEstimateItem ConData = null;

                        foreach (var item in Estimate.EstimateData.tblEstimateItems)
                        {
                            ConData = new tblEstimateItem();
                            ConData = item;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = UserId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = UserId;
                            ConData.isActive = true;
                            ConData.isDelete = false;
                            ConData.isCost = item.isCost;
                            ConData.EstimateId = Data.EstimateId;
                            DB.tblEstimateItems.Add(ConData);
                            DB.SaveChanges();
                        }
                    }

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
                            FileData.Caption = FileData.FileName;
                            FileData.FilePath = path;
                            FileData.EstimateId = Data.EstimateId;
                            FileData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.CreatedBy = UserId;
                            FileData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.EditBy = UserId;
                            FileData.isActive = true;
                            FileData.isDelete = false;
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

                    //return Ok("Estimate has been Update successfully.");
                    return Ok(new { Id = Data.EstimateId, Message = "Estimate has been Update successfully." });
                }
            }
            catch (DbEntityValidationException dbEx)
            {
                string ErrorString = "";
                // Handle DbEntityValidationException
                foreach (var item in dbEx.EntityValidationErrors)
                {
                    foreach (var item1 in item.ValidationErrors)
                    {
                        ErrorString += item1.ErrorMessage + " ,";
                    }
                }

                Console.WriteLine($"DbEntityValidationException occurred: {dbEx.Message}");
                // Additional handling specific to DbEntityValidationException
                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = new StringContent(ErrorString);

                return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);

                return ResponseMessage(responseMessage);
            }
            return NotFound();
        }

        [HttpGet]
        public IHttpActionResult DeleteEstimate(int id)
        {
            tblEstimate Data = new tblEstimate();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int UserId = 2;
            try
            {
                Data = DB.tblEstimates.Select(r => r).Where(x => x.EstimateId == id).FirstOrDefault();
                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }


                //List<tblEstimateFile> ConFile = DB.tblEstimateFiles.Where(x => x.EstimateId == id).ToList();
                //if (ConFile != null && ConFile.Count != 0)
                //{
                //    DB.tblEstimateFiles.RemoveRange(ConFile);
                //    DB.SaveChanges();
                //}

                //List<tblEstimateItem> ConList = DB.tblEstimateItems.Where(x => x.EstimateId == id).ToList();
                //if (ConList != null && ConList.Count != 0)
                //{
                //    DB.tblEstimateItems.RemoveRange(ConList);
                //    DB.SaveChanges();
                //}

                Data.isDelete = true;
                Data.EditBy = UserId;
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
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

        [HttpGet]
        public IHttpActionResult GetTagList()
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblTag> Tags = new List<tblTag>();
                Tags = DB.tblTags.Where(x => x.isActive == true).ToList();
                if (Tags == null || Tags.Count == 0)
                {
                    return NotFound();
                }

                return Ok(Tags);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }
    }
}
