using EarthCo.Models;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Security.Claims;
using System.Data.Entity.Validation;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class PurchaseOrderController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();
        [HttpGet]
        public IHttpActionResult GetPurchaseOrderServerSideList(string Search,int DisplayStart = 0, int DisplayLength = 10, int StatusId = 0, bool isAscending = false)
        {
            try
            {
                List<tblPurchaseOrder> Data = new List<tblPurchaseOrder>();
                List<PurchaseOrderList> Result = new List<PurchaseOrderList>();

                var totalRecords = DB.tblPurchaseOrders.Count(x => !x.isDelete);
                var totalOpenRecords = DB.tblPurchaseOrders.Where(x => x.StatusId == 1).Count(x => !x.isDelete);
                var totalClosedRecords = DB.tblPurchaseOrders.Where(x => x.StatusId == 2).Count(x => !x.isDelete);
                DisplayStart = (DisplayStart - 1) * DisplayLength;
                if (Search == null)
                {
                    Search = "\"\"";
                }
                Search = JsonSerializer.Deserialize<string>(Search);
                if (!string.IsNullOrEmpty(Search) && Search != "")
                {
                    Data = DB.tblPurchaseOrders.Where(x => x.tblUser.FirstName.ToLower().Contains(Search.ToLower())
                                                  || x.tblUser.LastName.ToLower().Contains(Search.ToLower())
                                                  || x.tblUser1.FirstName.ToLower().Contains(Search.ToLower())
                                                  || x.tblUser1.LastName.ToLower().Contains(Search.ToLower())
                                                  || x.tblUser2.FirstName.ToLower().Contains(Search.ToLower())
                                                  || x.tblUser2.LastName.ToLower().Contains(Search.ToLower())
                                                  || x.Amount.ToString().ToLower().Contains(Search.ToLower())
                                                  || x.tblPurchaseOrderStatu.Status.ToLower().Contains(Search.ToLower())).ToList();
                    if (StatusId != 0)
                    {
                        totalRecords = Data.Count(x => !x.isDelete && x.StatusId == StatusId);
                        Data = Data.Where(x => !x.isDelete && x.StatusId == StatusId).OrderBy(o => isAscending? o.PurchaseOrderId:-o.PurchaseOrderId).Skip(DisplayStart).Take(DisplayLength).ToList();
                    }
                    else
                    {
                        totalRecords = Data.Count(x => !x.isDelete);
                        Data = Data.Where(x => !x.isDelete).OrderBy(o => isAscending ? o.PurchaseOrderId : -o.PurchaseOrderId).Skip(DisplayStart).Take(DisplayLength).ToList();
                    }
                }
                else
                {
                    if (StatusId != 0)
                    {
                        totalRecords = DB.tblPurchaseOrders.Count(x => !x.isDelete && x.StatusId==StatusId);
                        Data = DB.tblPurchaseOrders.Where(x => !x.isDelete && x.StatusId == StatusId).OrderBy(o => isAscending ? o.PurchaseOrderId : -o.PurchaseOrderId).Skip(DisplayStart).Take(DisplayLength).ToList();
                    }
                    else
                    {
                        totalRecords = DB.tblPurchaseOrders.Count(x => !x.isDelete);
                        Data = DB.tblPurchaseOrders.Where(x => !x.isDelete).OrderBy(o => isAscending ? o.PurchaseOrderId : -o.PurchaseOrderId).Skip(DisplayStart).Take(DisplayLength).ToList();
                    }
                }

                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }
                else
                {
                    foreach (tblPurchaseOrder item in Data)
                    {
                        PurchaseOrderList Temp = new PurchaseOrderList();
                        Temp.PurchaseOrderId = item.PurchaseOrderId;
                        Temp.PurchaseOrderNumber = item.PurchaseOrderNumber;
                        Temp.SupplierName = item.tblUser.FirstName + " " + item.tblUser.LastName;
                        Temp.Date =(DateTime) item.Date;
                        Temp.Status =item.tblPurchaseOrderStatu.Status;
                        Temp.StatusColor =item.tblPurchaseOrderStatu.Color;
                        Temp.RegionalManager = item.tblUser1.FirstName + " " + item.tblUser1.LastName;
                        Temp.RequestedBy = item.tblUser2.FirstName + " " + item.tblUser2.LastName;
                        Temp.EstimateNumber = item.EstimateNumber;
                        Temp.BillNumber = item.BillNumber;
                        Temp.InvoiceNumber = item.InvoiceNumber;
                        Temp.Amount = item.Amount;
                        Result.Add(Temp);
                    }
                }

                return Ok(new { totalRecords = totalRecords, totalOpenRecords = totalOpenRecords, totalClosedRecords = totalClosedRecords, Data = Result }); // 200 - Successful response with data
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
        }
        [HttpGet]
        public IHttpActionResult GetPurchaseOrderList()
        {
            try
            {
                List<tblPurchaseOrder> Data = new List<tblPurchaseOrder>();
                List<PurchaseOrderList> Result = new List<PurchaseOrderList>();
                Data = DB.tblPurchaseOrders.Where(x => x.isDelete != true).ToList();

                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }
                else
                {
                    foreach (tblPurchaseOrder item in Data)
                    {
                        PurchaseOrderList Temp = new PurchaseOrderList();
                        Temp.PurchaseOrderId = item.PurchaseOrderId;
                        Temp.PurchaseOrderNumber = item.PurchaseOrderNumber;
                        Temp.SupplierName = item.tblUser.FirstName + " " + item.tblUser.LastName;
                        Temp.Date =(DateTime) item.Date;
                        Temp.Status =item.tblPurchaseOrderStatu.Status;
                        Temp.RegionalManager = item.tblUser1.FirstName + " " + item.tblUser1.LastName;
                        Temp.RequestedBy = item.tblUser2.FirstName + " " + item.tblUser2.LastName;
                        Temp.EstimateNumber = item.EstimateNumber;
                        Temp.BillNumber = item.BillNumber;
                        Temp.InvoiceNumber = item.InvoiceNumber;
                        Temp.Amount = item.Amount;
                        Result.Add(Temp);
                    }
                }

                return Ok(Result); // 200 - Successful response with data
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
        }

        [HttpGet]
        public IHttpActionResult GetPurchaseOrder(int id)
        {
            try
            {
                SPGetPurchaseOrderData_Result Data = new SPGetPurchaseOrderData_Result();
                List<SPGetPurchaseOrderItemData_Result> ItemData = new List<SPGetPurchaseOrderItemData_Result>();
                List<SPGetPurchaseOrderFileData_Result> FileData = new List<SPGetPurchaseOrderFileData_Result>();
                Data = DB.SPGetPurchaseOrderData(id).FirstOrDefault();
                ItemData = DB.SPGetPurchaseOrderItemData(id).ToList();
                FileData = DB.SPGetPurchaseOrderFileData(id).ToList();


                GetPurchaseOrderData GetData = new GetPurchaseOrderData();
                if (Data == null)
                {
                    //DB.Configuration.ProxyCreationEnabled = false;
                    //Data = new tblPurchaseOrder();
                    //Data.tblPurchaseOrderItems = null;
                    //Data.tblInvoices = null;
                    //Data.tblEstimate = null;
                    //Data.tblBills = null;
                    //Data.tblPurchaseOrderFiles = null;
                    //Data.tblUser = null;
                    //string userJson = JsonConvert.SerializeObject(Data);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    //responseMessage.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
                    return ResponseMessage(responseMessage);
                }
                else
                {
                    GetData.Data = Data;
                    GetData.ItemData = ItemData;
                    GetData.FileData = FileData;

                }

                return Ok(GetData); // 200 - Successful response with data
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

        }

        [HttpGet]
        public IHttpActionResult GetPurchaseOrderStatus()
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblPurchaseOrderStatu> Data = new List<tblPurchaseOrderStatu>();
                Data = DB.tblPurchaseOrderStatus.ToList();
                if (Data == null || Data.Count == 0)
                {
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    return ResponseMessage(responseMessage);
                }

                return Ok(Data); // 200 - Successful response with data
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

        }

        [HttpPost]
        public IHttpActionResult AddPurchaseOrder()
        {
            var Data1 = HttpContext.Current.Request.Params.Get("PurchaseOrderData");
            HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

            PurchaseOrderFile PurchaseOrder = new PurchaseOrderFile();
            PurchaseOrder.Files = new List<HttpPostedFile>();
            for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                PurchaseOrder.Files.Add(HttpContext.Current.Request.Files[i]); ;
            }

            PurchaseOrder.PurchaseOrderData = JsonSerializer.Deserialize<tblPurchaseOrder>(Data1);

            tblPurchaseOrder Data = new tblPurchaseOrder();
            tblPurchaseOrder CheckData = new tblPurchaseOrder();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);

                var userIdClaim = User.Identity as ClaimsIdentity;
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);

                if (PurchaseOrder.PurchaseOrderData.PurchaseOrderId == 0)
                {

                    CheckData = DB.tblPurchaseOrders.Where(x => x.PurchaseOrderNumber == PurchaseOrder.PurchaseOrderData.PurchaseOrderNumber && x.PurchaseOrderNumber != null && x.PurchaseOrderNumber != "").FirstOrDefault();

                    if (CheckData != null)
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("Purchase Order number already exsist.");
                        return ResponseMessage(responseMessage);
                    }

                    if (PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems != null && PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems.Count != 0)
                    {
                        foreach (var item in PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems)
                        {
                            item.CreatedBy = UserId;
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditBy = UserId;
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.Amount = item.Qty * item.Rate;
                            item.PurchaseOrderId = Data.PurchaseOrderId;
                            item.isActive = true;
                            item.isDelete = false;
                        }
                    }

                    Data = PurchaseOrder.PurchaseOrderData;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("P").FirstOrDefault());
                    if (Data.PurchaseOrderNumber == null || Data.PurchaseOrderNumber == "")
                    {
                        Data.PurchaseOrderNumber = Data.DocNumber;
                    }
                    DB.tblPurchaseOrders.Add(Data);
                    DB.SaveChanges();

                    if(Data.EstimateId!=null && Data.EstimateId!=0)
                    {
                        tblEstimate EstimateData = DB.tblEstimates.Where(x => x.EstimateId == Data.EstimateId).FirstOrDefault();
                        EstimateData.PurchaseOrderId = Data.PurchaseOrderId;
                        DB.Entry(EstimateData);
                        DB.SaveChanges();
                    }


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

                    if (PurchaseOrder.Files != null && PurchaseOrder.Files.Count != 0)
                    {
                        tblPurchaseOrderFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading/PurchaseOrder"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in PurchaseOrder.Files)
                        {
                            FileData = new tblPurchaseOrderFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/PurchaseOrder"), Path.GetFileName("PurchaseOrder" + Data.PurchaseOrderId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading\\PurchaseOrder", Path.GetFileName("PurchaseOrder" + Data.PurchaseOrderId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = "PurchaseOrder" + Data.PurchaseOrderId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName);
                            FileData.Caption = FileData.FileName;
                            FileData.FilePath = path;
                            FileData.PurchaseOrderId = Data.PurchaseOrderId;
                            FileData.CreatedBy = UserId;
                            FileData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.EditBy = UserId;
                            FileData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.isActive = true;
                            FileData.idDelete = false;
                            DB.tblPurchaseOrderFiles.Add(FileData);
                            DB.SaveChanges();
                            NameCount++;
                        }
                    }


                    tblSyncLog Result = new tblSyncLog();
                    Result.Id = Data.PurchaseOrderId;
                    Result.Name = "PurchaseOrder";
                    Result.Operation = "Create";
                    Result.CreatedDate = DateTime.Now;
                    Result.isQB = false;
                    Result.isSync = false;
                    DB.tblSyncLogs.Add(Result);
                    DB.SaveChanges();

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Purchase Order";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    //return Ok("Estimate has been added successfully.");
                    return Ok(new { Id = Data.PurchaseOrderId, Message = "Purchase Order has been added successfully." });
                }
                else
                {
                    Data = DB.tblPurchaseOrders.Select(r => r).Where(x => x.PurchaseOrderId == PurchaseOrder.PurchaseOrderData.PurchaseOrderId).FirstOrDefault();
                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }

                    CheckData = DB.tblPurchaseOrders.Where(x => x.PurchaseOrderNumber == PurchaseOrder.PurchaseOrderData.PurchaseOrderNumber && x.PurchaseOrderNumber != null && x.PurchaseOrderNumber != "").FirstOrDefault();
                    if (CheckData != null && CheckData.PurchaseOrderId != Data.PurchaseOrderId)
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("Purchase Order number already exsist.");
                        return ResponseMessage(responseMessage);
                    }

                    List<tblPurchaseOrderItem> ConList = DB.tblPurchaseOrderItems.Where(x => x.PurchaseOrderId == PurchaseOrder.PurchaseOrderData.PurchaseOrderId).ToList();
                    if (ConList != null && ConList.Count != 0)
                    {
                        DB.tblPurchaseOrderItems.RemoveRange(ConList);
                        DB.SaveChanges();
                    }



                    if (PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems != null && PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems.Count != 0)
                    {
                        foreach (var item in PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems)
                        {
                            item.CreatedBy = UserId;
                            item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.EditBy = UserId;
                            item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            item.Amount = item.Qty * item.Rate;
                            item.PurchaseOrderId = Data.PurchaseOrderId;
                            item.isActive = true;
                            item.isDelete = false;
                        }
                    }


                    Data.SupplierId = PurchaseOrder.PurchaseOrderData.SupplierId;
                    Data.PurchaseOrderNumber = PurchaseOrder.PurchaseOrderData.PurchaseOrderNumber;
                    Data.Tags = PurchaseOrder.PurchaseOrderData.Tags;
                    Data.Date = PurchaseOrder.PurchaseOrderData.Date;
                    Data.DueDate = PurchaseOrder.PurchaseOrderData.DueDate;
                    Data.RegionalManager = PurchaseOrder.PurchaseOrderData.RegionalManager;
                    Data.TermId = PurchaseOrder.PurchaseOrderData.TermId;
                    Data.Requestedby = PurchaseOrder.PurchaseOrderData.Requestedby;
                    Data.StatusId = PurchaseOrder.PurchaseOrderData.StatusId;
                    Data.InvoiceNumber = PurchaseOrder.PurchaseOrderData.InvoiceNumber;
                    Data.InvoiceId = PurchaseOrder.PurchaseOrderData.InvoiceId;
                    Data.BillNumber = PurchaseOrder.PurchaseOrderData.BillNumber;
                    Data.BillId = PurchaseOrder.PurchaseOrderData.BillId;
                    Data.EstimateNumber = PurchaseOrder.PurchaseOrderData.EstimateNumber;
                    Data.EstimateId = PurchaseOrder.PurchaseOrderData.EstimateId;
                    Data.MemoInternal = PurchaseOrder.PurchaseOrderData.MemoInternal;
                    Data.Message = PurchaseOrder.PurchaseOrderData.Message;
                    Data.ShipTo = PurchaseOrder.PurchaseOrderData.ShipTo;
                    Data.Amount = PurchaseOrder.PurchaseOrderData.Amount;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    if (PurchaseOrder.PurchaseOrderData.PurchaseOrderNumber == null || PurchaseOrder.PurchaseOrderData.PurchaseOrderNumber == "")
                    {
                        Data.PurchaseOrderNumber = Data.DocNumber;
                    }
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.Entry(Data);
                    DB.SaveChanges();
                    if (Data.EstimateId != null && Data.EstimateId != 0)
                    {
                        tblEstimate EstimateData = DB.tblEstimates.Where(x => x.EstimateId == Data.EstimateId).FirstOrDefault();
                        EstimateData.PurchaseOrderId = Data.PurchaseOrderId;
                        DB.Entry(EstimateData);
                        DB.SaveChanges();
                    }
                    if (PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems != null && PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems.Count != 0)
                    {
                        tblPurchaseOrderItem ConData = null;

                        foreach (var item in PurchaseOrder.PurchaseOrderData.tblPurchaseOrderItems)
                        {
                            ConData = new tblPurchaseOrderItem();
                            ConData = item;
                            ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.CreatedBy = UserId;
                            ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            ConData.EditBy = UserId;
                            ConData.isActive = true;
                            ConData.isDelete = false;
                            ConData.PurchaseOrderId = Data.PurchaseOrderId;
                            DB.tblPurchaseOrderItems.Add(ConData);
                            DB.SaveChanges();
                        }
                    }

                    //List<tblPurchaseOrderFile> ConFList = DB.tblPurchaseOrderFiles.Where(x => x.PurchaseOrderId == PurchaseOrder.PurchaseOrderData.PurchaseOrderId).ToList();
                    //if (ConFList != null && ConFList.Count != 0)
                    //{
                    //    DB.tblPurchaseOrderFiles.RemoveRange(ConFList);
                    //    DB.SaveChanges();
                    //}

                    if (PurchaseOrder.Files != null && PurchaseOrder.Files.Count != 0)
                    {
                        tblPurchaseOrderFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading/PurchaseOrder"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in PurchaseOrder.Files)
                        {
                            FileData = new tblPurchaseOrderFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/PurchaseOrder"), Path.GetFileName("PurchaseOrder" + Data.PurchaseOrderId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading\\PurchaseOrder", Path.GetFileName("PurchaseOrder" + Data.PurchaseOrderId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = "PurchaseOrder" + Data.PurchaseOrderId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName);
                            FileData.Caption = FileData.FileName;
                            FileData.FilePath = path;
                            FileData.PurchaseOrderId = Data.PurchaseOrderId;
                            FileData.CreatedBy = UserId;
                            FileData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.EditBy = UserId;
                            FileData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            FileData.isActive = true;
                            FileData.idDelete = false;
                            DB.tblPurchaseOrderFiles.Add(FileData);
                            DB.SaveChanges();
                            NameCount++;
                        }
                    }

                    tblSyncLog Result = new tblSyncLog();
                    Result = DB.tblSyncLogs.Where(x => x.Id == Data.PurchaseOrderId && x.Name == "PurchaseOrder").FirstOrDefault();
                    if (Result == null)
                    {
                        Result = new tblSyncLog();
                        Result.Id = Data.PurchaseOrderId;
                        Result.Name = "PurchaseOrder";
                        Result.Operation = "Update";
                        Result.EditDate = DateTime.Now;
                        Result.isQB = false;
                        Result.isSync = false;
                        DB.tblSyncLogs.Add(Result);
                        DB.SaveChanges();
                    }
                    else
                    {
                        Result.QBId = 0;
                        Result.Id = Data.PurchaseOrderId;
                        Result.Name = "PurchaseOrder";
                        Result.Operation = "Update";
                        Result.EditDate = DateTime.Now;
                        Result.isQB = false;
                        Result.isSync = false;
                        DB.Entry(Result);
                        DB.SaveChanges();
                    }
                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Purchase Order";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    //return Ok("Purchase Order has been Update successfully.");
                    return Ok(new { Id = Data.PurchaseOrderId, Message = "Purchase Order has been Update successfully." });
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
        public IHttpActionResult DeletePurchaseOrder(int id)
        {
            tblPurchaseOrder Data = new tblPurchaseOrder();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);

            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                Data = DB.tblPurchaseOrders.Select(r => r).Where(x => x.PurchaseOrderId == id).FirstOrDefault();
                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }


                //List<tblPurchaseOrderFile> ConFile = DB.tblPurchaseOrderFiles.Where(x => x.PurchaseOrderId == id).ToList();
                //if (ConFile != null && ConFile.Count != 0)
                //{
                //    DB.tblPurchaseOrderFiles.RemoveRange(ConFile);
                //    DB.SaveChanges();
                //}

                //List<tblPurchaseOrderItem> ConList = DB.tblPurchaseOrderItems.Where(x => x.PurchaseOrderId == id).ToList();
                //if (ConList != null && ConList.Count != 0)
                //{
                //    DB.tblPurchaseOrderItems.RemoveRange(ConList);
                //    DB.SaveChanges();
                //}


                Data.isDelete = true;
                Data.EditBy = UserId;
                Data.EditDate = DateTime.Now;
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Delete Purchase Order";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();

                tblSyncLog Result = new tblSyncLog();
                Result = DB.tblSyncLogs.Where(x => x.Id == Data.PurchaseOrderId && x.Name == "PurchaseOrder").FirstOrDefault();
                if (Result == null)
                {
                    Result = new tblSyncLog();
                    Result.Id = Data.PurchaseOrderId;
                    Result.Name = "PurchaseOrder";
                    Result.Operation = "Delete";
                    Result.EditDate = DateTime.Now;
                    Result.isQB = false;
                    Result.isSync = false;
                    DB.tblSyncLogs.Add(Result);
                    DB.SaveChanges();
                }
                else
                {
                    Result.QBId = 0;
                    Result.Id = Data.PurchaseOrderId;
                    Result.Name = "PurchaseOrder";
                    Result.Operation = "Delete";
                    Result.EditDate = DateTime.Now;
                    Result.isQB = false;
                    Result.isSync = false;
                    DB.Entry(Result);
                    DB.SaveChanges();
                }

                return Ok("Purchase Order has been deleted successfully.");
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
        }

        [HttpGet]
        public IHttpActionResult GetTermList()
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblTerm> Terms = new List<tblTerm>();
                Terms = DB.tblTerms.Where(x => x.isActive == true).ToList();
                if (Terms == null || Terms.Count == 0)
                {
                    return NotFound();
                }

                return Ok(Terms);
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

        }

        [HttpGet]
        public IHttpActionResult DeletePurchaseOrderFile(int FileId)
        {
            tblPurchaseOrderFile Data = new tblPurchaseOrderFile();

            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                Data = DB.tblPurchaseOrderFiles.Select(r => r).Where(x => x.PurchaseOrderFileId == FileId).FirstOrDefault();
                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }

                Data.idDelete = true;
                Data.EditBy = UserId;
                Data.EditDate = DateTime.Now;
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Delete Purchase Order File";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Purchase Order file has been deleted successfully.");
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
        }
    }
}
