using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class EstimateController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public IHttpActionResult GetEstimateServerSideList(string Search,int DisplayStart=0,int DisplayLength=10,int StatusId=0, bool isAscending = false)
        {
            try
            {
                List<GetEstimateItem> EstData = new List<GetEstimateItem>();

                List<tblEstimate> Data = new List<tblEstimate>();
                var totalRecords = DB.tblEstimates.Count(x => !x.isDelete);
                var totalApprovedRecords = DB.tblEstimates.Where(x=>x.EstimateStatusId==1).Count(x => !x.isDelete);
                var totalClosedRecords = DB.tblEstimates.Where(x=>x.EstimateStatusId==2).Count(x => !x.isDelete);
                var totalNewRecords = DB.tblEstimates.Where(x=>x.EstimateStatusId==4).Count(x => !x.isDelete);
                DisplayStart = (DisplayStart - 1) * DisplayLength;
                if (Search == null)
                {
                    Search = "\"\"";
                }
                Search = JsonSerializer.Deserialize<string>(Search);
                if (!string.IsNullOrEmpty(Search) && Search != "")
                {
                    Data = DB.tblEstimates.Where(x => x.tblUser.FirstName.ToLower().Contains(Search.ToLower())
                                                  || x.tblUser.LastName.ToLower().Contains(Search.ToLower())
                                                  || x.tblUser1.FirstName.ToLower().Contains(Search.ToLower())
                                                  || x.tblUser1.LastName.ToLower().Contains(Search.ToLower())
                                                  || x.EstimateNumber.ToString().ToLower().Contains(Search.ToLower())
                                                  || x.tblEstimateStatu.Status.ToString().ToLower().Contains(Search.ToLower())
                                                  || x.EstimateNotes.ToLower().Contains(Search.ToLower())
                                                  || x.ProfitPercentage.ToString().ToLower().Contains(Search.ToLower())
                                                  || x.TotalAmount.ToString().ToLower().Contains(Search.ToLower())).ToList();
                    
                    if (StatusId != 0)
                    {
                        totalRecords = Data.Count(x => !x.isDelete && x.EstimateStatusId==StatusId);
                        Data = Data.Where(x => !x.isDelete && x.EstimateStatusId == StatusId).OrderBy(o => isAscending ? o.EstimateId : -o.EstimateId).Skip(DisplayStart).Take(DisplayLength).ToList();
                    }
                    else
                    {
                        totalRecords = Data.Count(x => !x.isDelete);
                        Data = Data.Where(x => !x.isDelete).OrderBy(o => isAscending ? o.EstimateId : -o.EstimateId).Skip(DisplayStart).Take(DisplayLength).ToList();
                    }
                }
                else
                {
                    if (StatusId != 0)
                    {
                        totalRecords = DB.tblEstimates.Count(x => !x.isDelete && x.EstimateStatusId==StatusId);
                        Data = DB.tblEstimates.Where(x => !x.isDelete && x.EstimateStatusId == StatusId).OrderBy(o => isAscending ? o.EstimateId : -o.EstimateId).Skip(DisplayStart).Take(DisplayLength).ToList();
                    }
                    else
                    {
                        totalRecords = DB.tblEstimates.Count(x => !x.isDelete);
                        Data = DB.tblEstimates.Where(x => !x.isDelete).OrderBy(o => isAscending ? o.EstimateId : -o.EstimateId).Skip(DisplayStart).Take(DisplayLength).ToList();
                    }
                }


                
                

               // Data = Data.Skip(DisplayStart)
               //.Take(DisplayLength).ToList();
                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }

                

                foreach (tblEstimate item in Data)
                {
                    GetEstimateItem Temp = new GetEstimateItem();
                    Temp.EstimateId = (int)item.EstimateId;
                    Temp.CustomerId = (int)item.CustomerId;
                    Temp.CustomerName = item.tblUser.FirstName + " " + item.tblUser.LastName;
                    Temp.RegionalManager = item.tblUser1.FirstName + " " + item.tblUser1.LastName;
                    Temp.Date = item.CreatedDate;
                    Temp.Status = item.tblEstimateStatu.Status;
                    Temp.StatusColor = item.tblEstimateStatu.Color;
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
                    Temp.EstimateAmount = (double)item.tblEstimateItems.Sum(s => s.Amount);
                    EstData.Add(Temp);
                }

                return Ok(new { totalRecords=totalRecords, totalApprovedRecords = totalApprovedRecords, totalClosedRecords = totalClosedRecords, totalNewRecords = totalNewRecords, Data = EstData }); // 200 - Successful response with data
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
        public async Task<IHttpActionResult> SyncEstimate()
        {
            try
            {
                tblSyncLog Result1 = new tblSyncLog();
                Result1.Message = "SyncEstimate";
                Result1.CreatedDate = DateTime.Now;
                DB.tblSyncLogs.Add(Result1);
                DB.SaveChanges();
                if (!Request.Method.Equals(HttpMethod.Post))
                {
                    return (IHttpActionResult)Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Method Not Allowed");
                }


                VerifySignature Test = new VerifySignature();
                Dictionary<string, string> headers = Request.Headers.ToDictionary(header => header.Key, header => header.Value.ToString());

                // Replace "yourSecretKeyHere" with your actual secret key
                string verifier = "b4f26f4f-72f2-435f-a00d-af9561b05494";

                string payload = await Request.Content.ReadAsStringAsync();
                // Call IsRequestValid function
                bool isValid = Test.IsRequestValid(headers, payload, verifier);

                if (isValid)
                {

                    tblSyncLog Result = new tblSyncLog();
                    Result.Message = payload;
                    Result.CreatedDate = DateTime.Now;
                    DB.tblSyncLogs.Add(Result);
                    DB.SaveChanges();
                    // Process the valid webhook request...
                    return Ok("Webhook request is valid.");
                }
                else
                {
                    tblSyncLog Result = new tblSyncLog();
                    Result.Message = "Not Valid "+ payload;
                    Result.CreatedDate = DateTime.Now;
                    DB.tblSyncLogs.Add(Result);
                    DB.SaveChanges();
                    // Handle invalid request...
                    return BadRequest("Invalid webhook request.");
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
                tblSyncLog Result = new tblSyncLog();
                Result.Message = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblSyncLogs.Add(Result);
                DB.SaveChanges();
                return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblSyncLog Result = new tblSyncLog();
                Result.Message = "Error1: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblSyncLogs.Add(Result);
                DB.SaveChanges();
                return ResponseMessage(responseMessage);
            }

        }

        [HttpPost]
        public async System.Threading.Tasks.Task<IHttpActionResult> AddEstimate()
        {
            var Data1 = HttpContext.Current.Request.Params.Get("EstimateData");
            HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

            EstimateFiles Estimate = new EstimateFiles();
            Estimate.Files = new List<HttpPostedFile>();
            for (int i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                Estimate.Files.Add(HttpContext.Current.Request.Files[i]);
            }

            Estimate.EstimateData = JsonSerializer.Deserialize<tblEstimate>(Data1);

            tblEstimate Data = new tblEstimate();
            tblEstimate CheckData = new tblEstimate();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);
                var userIdClaim = User.Identity as ClaimsIdentity;
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                if (Estimate.EstimateData.EstimateId == 0)
                {

                    CheckData = DB.tblEstimates.Where(x => x.EstimateNumber == Estimate.EstimateData.EstimateNumber && x.EstimateNumber != null && x.EstimateNumber != "").FirstOrDefault();

                    if (CheckData != null)
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("Estimate number already exsist.");
                        return ResponseMessage(responseMessage);
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

                    Data = Estimate.EstimateData;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("E").FirstOrDefault());
                    if (Data.EstimateNumber == null || Data.EstimateNumber == "")
                    {
                        Data.EstimateNumber = Data.DocNumber;
                    }
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
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading/Estimate"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in Estimate.Files)
                        {
                            FileData = new tblEstimateFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/Estimate"), Path.GetFileName("Estimate" + Data.EstimateId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading\\Estimate", Path.GetFileName("Estimate" + Data.EstimateId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = "Estimate" + Data.EstimateId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss")+ Path.GetExtension(item.FileName);
                            //FileData.FileName = Path.GetFileName(item.FileName);
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

                    tblSyncLog Result = new tblSyncLog();
                    Result.Id = Data.EstimateId;
                    Result.Name = "Estimate";
                    Result.Operation = "Create";
                    Result.CreatedDate = DateTime.Now;
                    Result.isQB = false;
                    Result.isSync = false;
                    DB.tblSyncLogs.Add(Result);
                    DB.SaveChanges();



                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Estimate";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();


                    //var requestData = new
                    //{
                    //    TxnDate = DateTime.Now,
                    //    RefNumber = Data.EstimateNumber,
                    //    IsActive = Data.isActive,
                    //    TotalAmount = Data.TotalAmount,
                    //    Memo = Data.EstimateNotes,
                    //    DiscountAmt = Data.Discount,
                    //    ShippingAmt = Data.Shipping,
                    //    TagList = Data.Tags,
                    //    //Customer_RecordID = Data.CustomerId,
                    //    Customer_RecordID = 2,
                    //    EstimateStatus_RecordID = Data.EstimateStatusId,
                    //    AssignedTo_RecordID = 1,
                    //    //AssignedTo_RecordID = Data.AssignTo,
                    //};

                    //string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);

                    //using (HttpClient client = new HttpClient())
                    //{
                    //    string apiUrl = "https://rest.method.me/api/v1/tables/Estimate";
                    //    client.DefaultRequestHeaders.Add("Authorization", "APIKey NjU2NDk0MWFkMDUzMDRhN2Q3MzY2NDI3LjcxRjNEMEY2MTVDQzRBOTVBNzFCMUVGOEIwRTExRjIw ");

                    //    // Create the request content
                    //    var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                    //    //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    //    // Make the POST request
                    //    HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    //    // Check if the request was successful
                    //    if (response.IsSuccessStatusCode)
                    //    {
                    //        // Parse and use the response data as needed
                    //        string jsonResponse = await response.Content.ReadAsStringAsync();
                    //        // Process jsonResponse
                    //    }
                    //    else
                    //    {
                    //        // Handle error
                    //        string errorMessage = await response.Content.ReadAsStringAsync();
                    //        // Handle error message
                    //    }
                    //}


                    //if (Estimate.EstimateData.tblEstimateItems != null && Estimate.EstimateData.tblEstimateItems.Count != 0)
                    //{
                    //    foreach (var item in Estimate.EstimateData.tblEstimateItems)
                    //    {
                    //        var requestItemData = new
                    //        {
                    //            Desc = item.Description,
                    //            Quantity = item.Qty,
                    //            Rate = item.Rate,
                    //            TotalAmount = item.Amount,
                    //            EstimateRecordID = item.EstimateId,
                    //            Item = item.Name,
                    //            Item_RecordID = item.ItemId,
                    //            SalesTaxCode = item.Tax
                    //        };

                    //        string jsonItemRequest = Newtonsoft.Json.JsonConvert.SerializeObject(requestItemData);

                    //        using (HttpClient client = new HttpClient())
                    //        {
                    //            string apiUrl = "https://rest.method.me/api/v1/tables/EstimateLine";
                    //            client.DefaultRequestHeaders.Add("Authorization", "APIKey NjU2NDk0MWFkMDUzMDRhN2Q3MzY2NDI3LjcxRjNEMEY2MTVDQzRBOTVBNzFCMUVGOEIwRTExRjIw ");

                    //            // Create the request content
                    //            var content = new StringContent(jsonItemRequest, Encoding.UTF8, "application/json");
                    //            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                    //            // Make the POST request
                    //            HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                    //            // Check if the request was successful
                    //            if (response.IsSuccessStatusCode)
                    //            {
                    //                // Parse and use the response data as needed
                    //                string jsonResponse = await response.Content.ReadAsStringAsync();
                    //                // Process jsonResponse
                    //            }
                    //            else
                    //            {
                    //                // Handle error
                    //                string errorMessage = await response.Content.ReadAsStringAsync();
                    //                // Handle error message
                    //            }
                    //        }

                    //    }
                    //}


                    //string url = "https://rest.method.me/api/v1/files";
                    //string apiKey = "NjU2NDk0MWFkMDUzMDRhN2Q3MzY2NDI3LjcxRjNEMEY2MTVDQzRBOTVBNzFCMUVGOEIwRTExRjIw";

                    ////HttpPostedFileBase file = Request.Files["fileInput"];

                    //using (var httpClient = new HttpClient())
                    //{
                    //    using (var formData = new MultipartFormDataContent())
                    //    {
                    //        formData.Add(new StringContent("Estimate"), "table");
                    //        formData.Add(new StringContent("11"), "recordId");
                    //        //formData.Add(new StringContent("true"), "attachToEmail");

                    //        if (file != null && file.ContentLength > 0)
                    //        {
                    //            using (var fileStream = file.InputStream)
                    //            {
                    //                var fileContent = new StreamContent(fileStream);
                    //                fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                    //                {
                    //                    Name = "\"\"",
                    //                    FileName = $"\"{Path.GetFileName(file.FileName)}\""
                    //                };

                    //                formData.Add(fileContent);

                    //                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("APIKey", apiKey);

                    //                using (var response = await httpClient.PostAsync(url, formData))
                    //                {
                    //                    if (response.IsSuccessStatusCode)
                    //                    {
                    //                        string responseContent = await response.Content.ReadAsStringAsync();
                    //                        Console.WriteLine(responseContent);
                    //                        // Handle successful response
                    //                    }
                    //                    else
                    //                    {
                    //                        Console.WriteLine($"Error: {response.StatusCode}");
                    //                        // Handle error response
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

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

                    CheckData = DB.tblEstimates.Where(x => x.EstimateNumber == Estimate.EstimateData.EstimateNumber && x.EstimateNumber != null && x.EstimateNumber != "").FirstOrDefault();
                    if (CheckData != null && CheckData.EstimateId!=Data.EstimateId)
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("Estimate number already exsist.");
                        return ResponseMessage(responseMessage);
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
                    Data.Tags = Estimate.EstimateData.Tags;
                    Data.Discount = Estimate.EstimateData.Discount;
                    Data.Shipping = Estimate.EstimateData.Shipping;
                    Data.Profit = Estimate.EstimateData.Profit;
                    Data.ProfitPercentage = Estimate.EstimateData.ProfitPercentage;
                    Data.TotalAmount = Estimate.EstimateData.TotalAmount;
                    Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    if (Estimate.EstimateData.EstimateNumber == null || Estimate.EstimateData.EstimateNumber == "")
                    {
                        Data.EstimateNumber = Data.DocNumber;
                    }
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

                    //List<tblEstimateFile> ConFList = DB.tblEstimateFiles.Where(x => x.EstimateId == Estimate.EstimateData.EstimateId).ToList();
                    //if (ConFList != null && ConFList.Count != 0)
                    //{
                    //    DB.tblEstimateFiles.RemoveRange(ConFList);
                    //    DB.SaveChanges();
                    //}

                    if (Estimate.Files != null && Estimate.Files.Count != 0)
                    {
                        tblEstimateFile FileData = null;
                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading/Estimate"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        int NameCount = 1;
                        foreach (var item in Estimate.Files)
                        {
                            FileData = new tblEstimateFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/Estimate"), Path.GetFileName("Estimate" + Data.EstimateId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading\\Estimate", Path.GetFileName("Estimate" + Data.EstimateId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            FileData.FileName = "Estimate" + Data.EstimateId.ToString() + NameCount + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName);
                            //FileData.FileName = Path.GetFileName(item.FileName);
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


                    tblSyncLog Result = new tblSyncLog();
                    Result.Id = Data.EstimateId;
                    Result.Name = "Estimate";
                    Result.Operation = "Update";
                    Result.CreatedDate = DateTime.Now;
                    Result.isQB = false;
                    Result.isSync = false;
                    DB.tblSyncLogs.Add(Result);
                    DB.SaveChanges();

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
            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
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
                Data.EditDate = DateTime.Now;
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Delete Estimate";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();

                tblSyncLog Result = new tblSyncLog();
                Result.Id = Data.EstimateId;
                Result.Name = "Estimate";
                Result.Operation = "Delete";
                Result.CreatedDate = DateTime.Now;
                Result.isQB = false;
                Result.isSync = false;
                DB.tblSyncLogs.Add(Result);
                DB.SaveChanges();


                return Ok("Estimate has been deleted successfully.");
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
        public IHttpActionResult UpdateAllSelectedEstimateStatus(UpdateStatus ParaData)
        {
            tblEstimate Data = new tblEstimate();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                foreach (var item in ParaData.id)
                {
                    Data = DB.tblEstimates.Select(r => r).Where(x => x.EstimateId == item).FirstOrDefault();
                    Data.EstimateStatusId = ParaData.StatusId;
                    Data.EditBy = UserId;
                    Data.EditDate = DateTime.Now;
                    DB.Entry(Data);
                    DB.SaveChanges();
                }

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Update All Selected Estimate status";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("All selected Estimate status has been updated successfully.");
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
        public IHttpActionResult DeleteAllSelectedEstimate(DeleteSelected ParaData)
        {
            tblEstimate Data = new tblEstimate();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                foreach (var item in ParaData.id)
                {
                    //List<tblEstimateItem> ConList = DB.tblEstimateItems.Where(x => x.EstimateId == item).ToList();
                    //if (ConList != null && ConList.Count != 0)
                    //{
                    //    DB.tblEstimateItems.RemoveRange(ConList);
                    //    DB.SaveChanges();
                    //}
                    Data = DB.tblEstimates.Select(r => r).Where(x => x.EstimateId == item).FirstOrDefault();
                    Data.isDelete = true;
                    Data.EditBy = UserId;
                    Data.EditDate = DateTime.Now;
                    DB.Entry(Data);
                    DB.SaveChanges();
                }

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Delete All Selected Estimate";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("All selected Estimate has been deleted successfully.");
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
        public IHttpActionResult DeleteEstimateFile(int FileId)
        {
            tblEstimateFile Data = new tblEstimateFile();
            
            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                Data = DB.tblEstimateFiles.Select(r => r).Where(x => x.EstimateFileId == FileId).FirstOrDefault();
                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }

                Data.isDelete = true;
                Data.EditBy = UserId;
                Data.EditDate = DateTime.Now;
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Delete Estimate File";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Estimate file has been deleted successfully.");
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
