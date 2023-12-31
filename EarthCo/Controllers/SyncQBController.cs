﻿using EarthCo.Models;
//using Newtonsoft.Json;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using static EarthCo.Models.EstimateQB;
using static EarthCo.Models.PurchaseOrderQB;
using static EarthCo.Models.QBBillCUClass;
using static EarthCo.Models.QBCustomerCUClass;
using static EarthCo.Models.QBErrorClass;
using static EarthCo.Models.QBInvoiceCUClass;
using static EarthCo.Models.QBItemCUClass;
using static EarthCo.Models.QBPurchaseOrderCUClass;
using static EarthCo.Models.QBStaffCUClass;
using static EarthCo.Models.SyncQB;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SyncQBController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public async Task<IHttpActionResult> ConnectToQB()
        {
            try
            {
                HomeController HC = new HomeController();
                var task = await HC.InitiateAuth("Connect to QuickBooks");

                return Ok(task);
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return ResponseMessage(responseMessage);
            }

        }
        [HttpGet]
        public async Task<IHttpActionResult> GenerateToken(string code,string realmId,string state)
        {
            try
            {
                code = JsonSerializer.Deserialize<string>(code);
                realmId = JsonSerializer.Deserialize<string>(realmId);
                state = JsonSerializer.Deserialize<string>(state);
                CallbackController CC = new CallbackController();
                //var task = await CC.Index(state,code, realmId);

                return Ok();
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return ResponseMessage(responseMessage);
            }

        }

        [HttpGet]
        public IHttpActionResult SyncDataAPI(int synclogId=0)   
        {
            try
            {
                List<tblSyncLog> SyncLogData = null;
                if (synclogId == 0)
                {
                     SyncLogData = new List<tblSyncLog>();
                     SyncLogData = DB.tblSyncLogs.Where(x => x.isSync != true).ToList();
                }
                else
                {
                    SyncLogData = new List<tblSyncLog>();
                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId ==synclogId ).ToList();
                }


                foreach (tblSyncLog item in SyncLogData)
                {
                    if(item.Name=="Estimate")
                    {
                        SyncEstimateAsync(item);
                    }
                    else if(item.Name == "PurchaseOrder")
                    {
                        SyncPurchaseOrderAsync(item);
                    }
                    else if(item.Name == "Invoice")
                    {
                        SyncInvoiceAsync(item);
                    }
                    else if(item.Name == "Bill")
                    {
                        SyncBillAsync(item);
                    }
                    else if(item.Name == "Item")
                    {
                        SyncItemAsync(item);
                    }
                    else if(item.Name == "Vendor")
                    {
                        SyncVendorAsync(item);
                    }
                    else if(item.Name == "Employee")
                    {
                        SyncStaffAsync(item);
                    }
                    else if(item.Name == "Customer")
                    {
                        SyncCustomerAsync(item);
                    }
                    else if(item.Name == "Account")
                    {
                        SyncAccountAsync(item);
                    }
                }
                
                return Ok("Sync successfull.");
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return ResponseMessage(responseMessage);
            }

        }

        public string SyncEstimateAsync(tblSyncLog SyncLog)
        {
            try
            {
                tblEstimate Data = new tblEstimate();
                List<tblEstimateItem> ItemData = new List<tblEstimateItem>();
                List<tblEstimateFile> FileData = new List<tblEstimateFile>();
                if(SyncLog.Operation == "Create")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblEstimates.Where(x => x.EstimateId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblEstimateItems.Where(x => x.EstimateId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            DB = new earthcoEntities();
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/"+TokenData.realmId+"/estimate?minorversion=23";

                        QBEstimateClass EsitimateData = new QBEstimateClass();
                        Models.EstimateQB.Line LineData = new Models.EstimateQB.Line();
                        EsitimateData.BillEmail = new BillEmail();
                        EsitimateData.BillEmail.Address = Data.tblUser.Email;
                        EsitimateData.TotalAmt = (decimal)Data.TotalAmount;
                        EsitimateData.DocNumber = Data.EstimateNumber;
                        //EsitimateData.SyncToken = "0";
                        //EsitimateData.Id = "1103";
                        EsitimateData.CustomerRef = new EstimateQB.CustomerRef();
                        EsitimateData.CustomerRef.value = Data.tblUser.QBId.ToString();
                        //EsitimateData.CustomerRef.name = "Cool Cars";

                        //Models.EstimateQB.Line LineData = new Models.EstimateQB.Line();
                        //EsitimateData.BillEmail = new BillEmail();
                        ////EsitimateData.BillEmail.Address = "Cool_Cars@intuit.com";
                        //EsitimateData.TotalAmt = 105;
                        ////EsitimateData.SyncToken = "2";
                        ////EsitimateData.Id = "1103";
                        //EsitimateData.CustomerRef = new EstimateQB.CustomerRef();
                        //EsitimateData.CustomerRef.value = "3";
                        ////EsitimateData.CustomerRef.name = "Cool Cars";
                        ////LineData.Id = "1";
                        ////LineData.Description = "Test";
                        ////LineData.Amount = 105;
                        EsitimateData.Line = new List<Models.EstimateQB.Line>();

                        foreach (var item in ItemData)
                        {
                            //LineData.Id = "1";
                            LineData.Description = item.Description;
                            LineData.Amount = Convert.ToDecimal(item.Amount);
                            //LineData.DetailType = item.tblItem.Type;
                            LineData.DetailType = "SalesItemLineDetail";
                            LineData.SalesItemLineDetail = new Models.EstimateQB.SalesItemLineDetail();
                            LineData.SalesItemLineDetail.ItemRef = new EstimateQB.ItemRef();
                            LineData.SalesItemLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            //LineData.SalesItemLineDetail.ItemRef.name = "Pest Control";
                            LineData.SalesItemLineDetail.UnitPrice = Convert.ToDecimal(item.Rate);
                            LineData.SalesItemLineDetail.Qty = (int)item.Qty;
                            
                            EsitimateData.Line.Add(LineData);

                            //LineData.Description = "Test";
                            //LineData.Amount = 105;
                            //LineData.DetailType = "SalesItemLineDetail";
                            //LineData.SalesItemLineDetail = new Models.EstimateQB.SalesItemLineDetail();
                            //LineData.SalesItemLineDetail.ItemRef = new EstimateQB.ItemRef();
                            //LineData.SalesItemLineDetail.ItemRef.value = "9";
                            ////LineData.SalesItemLineDetail.ItemRef.name = "Pest Control";
                            //LineData.SalesItemLineDetail.UnitPrice = 35;
                            //LineData.SalesItemLineDetail.Qty = 3;
                            //EsitimateData.Line = new List<Models.EstimateQB.Line>();
                            //EsitimateData.Line.Add(LineData);


                        }
                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(EsitimateData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;
                            //HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                //string jsonResponse = await response.Content.ReadAsStringAsync();
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var ResultResponse = jsonResponse.Result;
                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(ResultResponse);
                                QBEstimateResponseClass.EstimateMain ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBEstimateResponseClass.EstimateMain>(ResultResponse);

                                // Now you can access the data using the model
                                //var QBId = estimateModel["Estimate"]["Id"];
                                var QBId = Convert.ToInt32(ResponseData.Estimate.Id);
                                var SyncToken = ResponseData.Estimate.SyncToken;
                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId =Convert.ToInt32(QBId);
                                SyncLogData.isSync =true;
                                SyncLogData.EditDate =DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                //string errorMessage = await response.Content.ReadAsStringAsync();
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var ResultResponse = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = ResultResponse;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }


                        //foreach (tblEstimateFile item in FileData)
                        //{
                        //    string baseUrl = "https://sandbox-quickbooks.api.intuit.com";

                        //    // Endpoint
                        //    //string endpoint = "/v3/company/<realmID>/upload"; // Replace <realmID> with the actual realm ID

                        //    // Complete URL
                        //    //apiUrl = baseUrl + endpoint;

                        //    // Request body
                        //    var requestBody = new
                        //    {
                        //        AttachableRef = new[]
                        //        {
                        //        new
                        //        {
                        //            EntityRef = new
                        //            {
                        //                type = "Estimate",
                        //                value = "95"
                        //            }
                        //        }
                        //    },
                        //        //ContentType = "image/jpg",
                        //        //FileName = "receipt_nov15.jpg"
                        //    };

                        //    // Serialize the request body to JSON
                        //    string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

                        //    using (var httpClient = new HttpClient())
                        //    using (var content = new MultipartFormDataContent())
                        //    {
                        //        // Add AttachableRef as a JSON string
                        //        //var attachableRefJson = "{\"AttachableRef\":[{\"EntityRef\":{\"type\":\"Invoice\",\"value\":\"95\"}}],\"ContentType\":\"image/jpg\",\"FileName\":\"receipt_nov15.jpg\"}";
                        //        content.Add(new StringContent(jsonRequestBody), "application/json");

                        //        // Load the file from a URL
                        //        var fileUrl = "https://earthcoapi.yehtohoga.com/"+item.FilePath+""; // Replace with the actual file URL
                        //        var fileBytes = await httpClient.GetByteArrayAsync(fileUrl);
                        //        var fileContent = new ByteArrayContent(fileBytes);
                        //        content.Add(fileContent, "file", "receipt_nov15.jpg");

                        //        // Make the POST request
                        //        apiUrl = $"/v3/company/" + TokenData.realmId + "/upload";
                        //        var requestUrl = $"{baseUrl}{apiUrl}";

                        //        var response = await httpClient.PostAsync(requestUrl, content);

                        //        if (response.IsSuccessStatusCode)
                        //        {
                        //            // Handle success
                        //            Console.WriteLine("File uploaded successfully!");
                        //        }
                        //        else
                        //        {
                        //            // Handle failure
                        //            Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        //        }
                        //    }
                        //}

                        

                    }
                    else
                    {
                        Data = DB.tblEstimates.Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Convert.ToInt32(Data.EstimateId);
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblToken TokenData = DB.tblTokens.FirstOrDefault();
                            string AccessToken = "";

                            var diffOfDates = DateTime.Now - TokenData.EditDate;
                            do
                            {
                                TokenData = DB.tblTokens.FirstOrDefault();
                                diffOfDates = DateTime.Now - TokenData.EditDate;
                                if (diffOfDates.Value.Hours >= 1)
                                {
                                    HomeController.GetAuthTokensUsingRefreshTokenAsync();
                                }
                            } while (diffOfDates.Value.Hours >= 1);

                            AccessToken = TokenData.AccessToken;
                            Data = new tblEstimate();
                            using (HttpClient client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                // Make the GET request
                                //HttpResponseMessage response = await client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/estimate/" + SyncLog.QBId + "?minorversion=23");
                                var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/estimate/" + SyncLog.QBId + "?minorversion=23");
                                Response.Wait();
                                var response = Response.Result;


                                if (response.IsSuccessStatusCode)
                                {
                                    var readTask = response.Content.ReadAsStringAsync();
                                    readTask.Wait();
                                    string Test = readTask.Result;
                                    QBEstimateResponseClass.EstimateMain ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBEstimateResponseClass.EstimateMain>(Test);
                                    Data = new tblEstimate();
                                    Data.QBId = Convert.ToInt32(ResponseData.Estimate.Id);
                                    Data.EstimateNumber = ResponseData.Estimate.DocNumber;
                                    int QBId = Convert.ToInt32(ResponseData.Estimate.CustomerRef.value); ;
                                    int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.CustomerId = CustomerId;
                                    //Data.ServiceLocationId = Estimate.EstimateData.ServiceLocationId;
                                    //Data.Email = ResponseData.Estimate.Domain;
                                    //Data.IssueDate = DateTime.Now;
                                    //Data.ContactId = Estimate.EstimateData.ContactId;
                                    //Data.RegionalManagerId = Estimate.EstimateData.RegionalManagerId;
                                    //Data.AssignTo = Estimate.EstimateData.AssignTo;
                                    //Data.RequestedBy = Estimate.EstimateData.RequestedBy;
                                    if (ResponseData.Estimate.TxnStatus.Contains("Pending"))
                                    {
                                        Data.EstimateStatusId = 4;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Accepted"))
                                    {
                                        Data.EstimateStatusId = 1;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Rejected"))
                                    {
                                        Data.EstimateStatusId = 5;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Closed"))
                                    {
                                        Data.EstimateStatusId = 2;
                                    }

                                    Data.EstimateNotes = ResponseData.Estimate.CustomerMemo.value;
                                    Data.SyncToken = ResponseData.Estimate.SyncToken;
                                    //Data.Tax = Estimate.EstimateData.Tax;
                                    //Data.Tags = Estimate.EstimateData.Tags;
                                    //Data.Discount = Estimate.EstimateData.Discount;
                                    //Data.Shipping = Estimate.EstimateData.Shipping;
                                    //Data.Profit = Estimate.EstimateData.Profit;
                                    //Data.ProfitPercentage = Estimate.EstimateData.ProfitPercentage;
                                    Data.TotalAmount = (double?)ResponseData.Estimate.TotalAmt;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    //Data.CreatedBy = UserId;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("E").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblEstimates.Add(Data);
                                    DB.SaveChanges();


                                    foreach (QBEstimateResponseClass.Line item in ResponseData.Estimate.Line)
                                    {
                                        if(item.Id!=null)
                                        {
                                            tblEstimateItem Item = new tblEstimateItem();
                                            Item.Description = item.Description;
                                            Item.Qty = Convert.ToInt32(item.SalesItemLineDetail.Qty);
                                            Item.Rate = Convert.ToDouble(item.SalesItemLineDetail.UnitPrice);
                                            Item.Amount = Convert.ToDouble(item.Amount);
                                            QBId = Convert.ToInt32(item.SalesItemLineDetail.ItemRef.value);
                                            int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                            Item.ItemId = ItemId;
                                            Item.EstimateId = Data.EstimateId;
                                            //Item.CreatedBy = UserId;
                                            Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            //Item.EditBy = UserId;
                                            Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.isCost = false;
                                            Item.isActive = true;
                                            Item.isDelete = false;
                                            DB.tblEstimateItems.Add(Item);
                                            DB.SaveChanges();
                                        }
                                        
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.EstimateId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();

                                }
                                else
                                {
                                    var errorMessage = response.Content.ReadAsStringAsync();
                                    errorMessage.Wait();
                                    string ResultReponse = errorMessage.Result;


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = ResultReponse;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                        }




                    }
                }
                else if (SyncLog.Operation == "Update")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblEstimates.Where(x => x.EstimateId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblEstimateItems.Where(x => x.EstimateId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/estimate?minorversion=23";

                        QBEstimateClass EsitimateData = new QBEstimateClass();
                        Models.EstimateQB.Line LineData = new Models.EstimateQB.Line();
                        EsitimateData.BillEmail = new BillEmail();
                        EsitimateData.BillEmail.Address = Data.tblUser.Email;
                        EsitimateData.TotalAmt = (decimal)Data.TotalAmount;
                        EsitimateData.DocNumber = Data.EstimateNumber;
                        EsitimateData.SyncToken = Data.SyncToken;
                        EsitimateData.Id = Data.QBId.ToString();
                        if(EsitimateData.Id=="")
                        {
                            EsitimateData.Id = null;
                        }
                        EsitimateData.CustomerRef = new EstimateQB.CustomerRef();
                        EsitimateData.CustomerRef.value = Data.tblUser.QBId.ToString();
                        //EsitimateData.CustomerRef.name = "Cool Cars";

                        EsitimateData.Line = new List<Models.EstimateQB.Line>();
                        foreach (var item in ItemData)
                        {
                            //LineData.Id = "1";
                            LineData.Description = item.Description;
                            LineData.Amount = Convert.ToDecimal(item.Amount);
                            LineData.DetailType = item.tblItem.Type;
                            LineData.DetailType = "SalesItemLineDetail";
                            LineData.SalesItemLineDetail = new Models.EstimateQB.SalesItemLineDetail();
                            LineData.SalesItemLineDetail.ItemRef = new EstimateQB.ItemRef();
                            LineData.SalesItemLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            //LineData.SalesItemLineDetail.ItemRef.name = "Pest Control";
                            LineData.SalesItemLineDetail.UnitPrice = Convert.ToDecimal(item.Rate);
                            LineData.SalesItemLineDetail.Qty = (int)item.Qty;
                            
                            EsitimateData.Line.Add(LineData);
                        }
                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(EsitimateData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var ResultResponse = jsonResponse.Result;
                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(ResultResponse);
                                QBEstimateResponseClass.EstimateMain ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBEstimateResponseClass.EstimateMain>(ResultResponse);

                                // Now you can access the data using the model
                                //var QBId = estimateModel["Estimate"]["Id"];
                                var QBId = Convert.ToInt32(ResponseData.Estimate.Id);
                                var SyncToken = ResponseData.Estimate.SyncToken;

                                Data.QBId = Convert.ToInt32(QBId);
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var ResultResponse = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = ResultResponse;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;
                        
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Make the GET request
                            var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/estimate/" + SyncLog.QBId + "?minorversion=23");
                            Response.Wait();
                            var response = Response.Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var readTask = response.Content.ReadAsStringAsync();
                                readTask.Wait();
                                string Test = readTask.Result;
                                QBEstimateResponseClass.EstimateMain ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBEstimateResponseClass.EstimateMain>(Test);


                                int QBId = Convert.ToInt32(ResponseData.Estimate.Id);

                                Data = DB.tblEstimates.Where(x => x.QBId == QBId).FirstOrDefault();
                                if (Data != null)
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.Estimate.Id);
                                    Data.EstimateNumber = ResponseData.Estimate.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.Estimate.CustomerRef.value); ;
                                    int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.CustomerId = CustomerId;
                                    //Data.ServiceLocationId = Estimate.EstimateData.ServiceLocationId;
                                    //Data.Email = ResponseData.Estimate.Domain;
                                    //Data.IssueDate = DateTime.Now;
                                    //Data.ContactId = Estimate.EstimateData.ContactId;
                                    //Data.RegionalManagerId = Estimate.EstimateData.RegionalManagerId;
                                    //Data.AssignTo = Estimate.EstimateData.AssignTo;
                                    //Data.RequestedBy = Estimate.EstimateData.RequestedBy;
                                    if (ResponseData.Estimate.TxnStatus.Contains("Pending"))
                                    {
                                        Data.EstimateStatusId = 4;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Accepted"))
                                    {
                                        Data.EstimateStatusId = 1;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Rejected"))
                                    {
                                        Data.EstimateStatusId = 5;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Closed"))
                                    {
                                        Data.EstimateStatusId = 2;
                                    }

                                    Data.EstimateNotes = ResponseData.Estimate.CustomerMemo.value;
                                    Data.SyncToken = ResponseData.Estimate.SyncToken;
                                    //Data.Tax = Estimate.EstimateData.Tax;
                                    //Data.Tags = Estimate.EstimateData.Tags;
                                    //Data.Discount = Estimate.EstimateData.Discount;
                                    //Data.Shipping = Estimate.EstimateData.Shipping;
                                    //Data.Profit = Estimate.EstimateData.Profit;
                                    //Data.ProfitPercentage = Estimate.EstimateData.ProfitPercentage;
                                    Data.TotalAmount = (double?)ResponseData.Estimate.TotalAmt;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;s
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.Entry(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblEstimateItems.Where(x => x.EstimateId == Data.EstimateId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblEstimateItems.RemoveRange(ItemData);
                                    }

                                    foreach (QBEstimateResponseClass.Line item in ResponseData.Estimate.Line)
                                    {
                                        if(item.Id!=null)
                                        {
                                            tblEstimateItem Item = new tblEstimateItem();
                                            Item.Description = item.Description;
                                            Item.Qty = Convert.ToInt32(item.SalesItemLineDetail.Qty);
                                            Item.Rate = Convert.ToDouble(item.SalesItemLineDetail.UnitPrice);
                                            Item.Amount = Convert.ToDouble(item.Amount);
                                            QBId = Convert.ToInt32(item.SalesItemLineDetail.ItemRef.value);
                                            int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                            Item.ItemId = ItemId;
                                            Item.EstimateId = Data.EstimateId;
                                            //Item.CreatedBy = UserId;
                                            Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            //Item.EditBy = UserId;
                                            Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.isCost = false;
                                            Item.isActive = true;
                                            Item.isDelete = false;
                                            DB.tblEstimateItems.Add(Item);
                                            DB.SaveChanges();
                                        }
                                        
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.EstimateId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    Data = new tblEstimate();
                                    Data.QBId = Convert.ToInt32(ResponseData.Estimate.Id);
                                    Data.EstimateNumber = ResponseData.Estimate.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.Estimate.CustomerRef.value); ;
                                    int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.CustomerId = CustomerId;
                                    //Data.ServiceLocationId = Estimate.EstimateData.ServiceLocationId;
                                    //Data.Email = ResponseData.Estimate.Domain;
                                    //Data.IssueDate = DateTime.Now;
                                    //Data.ContactId = Estimate.EstimateData.ContactId;
                                    //Data.RegionalManagerId = Estimate.EstimateData.RegionalManagerId;
                                    //Data.AssignTo = Estimate.EstimateData.AssignTo;
                                    //Data.RequestedBy = Estimate.EstimateData.RequestedBy;
                                    if (ResponseData.Estimate.TxnStatus.Contains("Pending"))
                                    {
                                        Data.EstimateStatusId = 4;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Accepted"))
                                    {
                                        Data.EstimateStatusId = 1;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Rejected"))
                                    {
                                        Data.EstimateStatusId = 5;
                                    }
                                    else if (ResponseData.Estimate.TxnStatus.Contains("Closed"))
                                    {
                                        Data.EstimateStatusId = 2;
                                    }

                                    Data.EstimateNotes = ResponseData.Estimate.CustomerMemo.value;
                                    Data.SyncToken = ResponseData.Estimate.SyncToken;
                                    //Data.Tax = Estimate.EstimateData.Tax;
                                    //Data.Tags = Estimate.EstimateData.Tags;
                                    //Data.Discount = Estimate.EstimateData.Discount;
                                    //Data.Shipping = Estimate.EstimateData.Shipping;
                                    //Data.Profit = Estimate.EstimateData.Profit;
                                    //Data.ProfitPercentage = Estimate.EstimateData.ProfitPercentage;
                                    Data.TotalAmount = (double?)ResponseData.Estimate.TotalAmt;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("E").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblEstimates.Add(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblEstimateItems.Where(x => x.EstimateId == Data.EstimateId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblEstimateItems.RemoveRange(ItemData);
                                    }

                                    foreach (QBEstimateResponseClass.Line item in ResponseData.Estimate.Line)
                                    {
                                        if (item.Id != null && item.SalesItemLineDetail != null)
                                        {
                                            tblEstimateItem Item = new tblEstimateItem();
                                            Item.Description = item.Description;
                                            Item.Qty = Convert.ToInt32(item.SalesItemLineDetail.Qty);
                                            Item.Rate = Convert.ToDouble(item.SalesItemLineDetail.UnitPrice);
                                            Item.Amount = Convert.ToDouble(item.Amount);
                                            QBId = Convert.ToInt32(item.SalesItemLineDetail.ItemRef.value);
                                            int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                            Item.ItemId = ItemId;
                                            Item.EstimateId = Data.EstimateId;
                                            //Item.CreatedBy = UserId;
                                            Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            //Item.EditBy = UserId;
                                            Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.isCost = false;
                                            Item.isActive = true;
                                            Item.isDelete = false;
                                            DB.tblEstimateItems.Add(Item);
                                            DB.SaveChanges();
                                        }
                                        
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.EstimateId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                            else
                            {
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var ResultResponse = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = ResultResponse;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                        }
                    }
                }
                else if (SyncLog.Operation == "Delete")
                {
                    if (SyncLog.isQB != true)
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/estimate?operation=delete";

                        QBDeleteClass DeleteData = new QBDeleteClass();
                        Data = DB.tblEstimates.Where(x => x.EstimateId == SyncLog.Id).FirstOrDefault();
                        DeleteData.Id = Data.QBId.ToString();
                        DeleteData.SyncToken = Data.SyncToken;



                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(DeleteData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var  Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;


                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var ResultResponse = jsonResponse.Result;

                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(ResultResponse);
                                QBEstimateResponseClass.EstimateMain ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBEstimateResponseClass.EstimateMain>(ResultResponse);

                                // Now you can access the data using the model
                                //var QBId = estimateModel["Estimate"]["Id"];
                                var QBId = Convert.ToInt32(ResponseData.Estimate.Id);
                                var SyncToken = ResponseData.Estimate.SyncToken;


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var ResultResponse= errorMessage.Result;

                                var errorResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(ResultResponse);
                                if (errorResponse.Fault.Error.FirstOrDefault().Message.Contains("Object Not Found"))
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorResponse.Fault.Error.FirstOrDefault().Message;
                                    SyncLogData.EditDate = DateTime.Now;
                                    SyncLogData.isSync = true;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = ResultResponse;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }

                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblEstimates.Select(r => r).Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            Data.isDelete = true;
                            Data.EditDate = DateTime.Now;
                            DB.Entry(Data);
                            DB.SaveChanges();

                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Data.EstimateId;
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Message = "Object Not Found";
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }

                    }
                }

                return "Success";
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }

        }

        [HttpPost]
        public string SyncPurchaseOrderAsync(tblSyncLog SyncLog)
        {
            try
            {
                tblPurchaseOrder Data = new tblPurchaseOrder();
                List<tblPurchaseOrderItem> ItemData = new List<tblPurchaseOrderItem>();
                List<tblPurchaseOrderFile> FileData = new List<tblPurchaseOrderFile>();
                if (SyncLog.Operation == "Create")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblPurchaseOrders.Where(x => x.PurchaseOrderId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblPurchaseOrderItems.Where(x => x.PurchaseOrderId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            DB = new earthcoEntities();
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/purchaseorder?minorversion=23";

                        QBPurchaseOrderClass PurchaseOrderData = new QBPurchaseOrderClass();
                        Models.QBPurchaseOrderCUClass.LineDetail LineData = new Models.QBPurchaseOrderCUClass.LineDetail();
                        PurchaseOrderData.APAccountRef = new QBPurchaseOrderCUClass.APAccountRef();
                        PurchaseOrderData.APAccountRef.value = "33";
                        PurchaseOrderData.TotalAmt = (decimal)Data.Amount;
                        PurchaseOrderData.DocNumber = Data.DocNumber;
                        //EsitimateData.SyncToken = "0";
                        //EsitimateData.Id = "1103";
                        PurchaseOrderData.VendorRef = new QBPurchaseOrderCUClass.VendorRef();
                        PurchaseOrderData.VendorRef.value = Data.tblUser.QBId.ToString();
                        if (Data.StatusId==1)
                        {
                            PurchaseOrderData.POStatus = "Open";
                        }
                        else if (Data.StatusId == 2)
                        {
                            PurchaseOrderData.POStatus = "Closed";
                        }

                        PurchaseOrderData.Line = new List<Models.QBPurchaseOrderCUClass.LineDetail>();
                        foreach (var item in ItemData)
                        {
                            //LineData.Id = "1";
                            //LineData.Description = item.Description;
                            LineData.Amount = Convert.ToDecimal(item.Amount);
                            //LineData.DetailType = item.tblItem.Type;
                            LineData.DetailType = "ItemBasedExpenseLineDetail";
                            LineData.ItemBasedExpenseLineDetail = new Models.QBPurchaseOrderCUClass.ItemBasedExpenseLineDetail();
                            LineData.ItemBasedExpenseLineDetail.ItemRef = new QBPurchaseOrderCUClass.ItemRef();
                            LineData.ItemBasedExpenseLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            //LineData.SalesItemLineDetail.ItemRef.name = "Pest Control";
                            LineData.ItemBasedExpenseLineDetail.UnitPrice = Convert.ToDecimal(item.Rate);
                            LineData.ItemBasedExpenseLineDetail.Qty = (int)item.Qty;
                            
                            PurchaseOrderData.Line.Add(LineData);
                        }
                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(PurchaseOrderData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var ResultResponse = jsonResponse.Result;

                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(ResultResponse);
                                PurchaseOrderQB.PurchaseOrderResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<PurchaseOrderQB.PurchaseOrderResponse>(ResultResponse);
                                //// Now you can access the data using the model
                                //var QBId = estimateModel["PurchaseOrder"]["Id"];
                                //var SyncToken = estimateModel["PurchaseOrder"]["SyncToken"];
                                var QBId = Convert.ToInt32(ResponseData.PurchaseOrder.Id);
                                var SyncToken = ResponseData.PurchaseOrder.SyncToken;
                               

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var ResultResponse = errorMessage.Result;


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = ResultResponse;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }

                    }
                    else
                    {
                        Data = DB.tblPurchaseOrders.Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Convert.ToInt32(Data.EstimateId);
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblToken TokenData = DB.tblTokens.FirstOrDefault();
                            string AccessToken = "";

                            var diffOfDates = DateTime.Now - TokenData.EditDate;
                            do
                            {
                                TokenData = DB.tblTokens.FirstOrDefault();
                                diffOfDates = DateTime.Now - TokenData.EditDate;
                                if (diffOfDates.Value.Hours >= 1)
                                {
                                    HomeController.GetAuthTokensUsingRefreshTokenAsync();
                                }
                            } while (diffOfDates.Value.Hours >= 1);

                            AccessToken = TokenData.AccessToken;
                            Data = new tblPurchaseOrder();
                            using (HttpClient client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                // Make the GET request
                                var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/purchaseorder/" + SyncLog.QBId + "?minorversion=23");
                                Response.Wait();
                                var response = Response.Result;

                                if (response.IsSuccessStatusCode)
                                {
                                    var readTask = response.Content.ReadAsStringAsync();
                                    readTask.Wait();
                                    string Test = readTask.Result;
                                    PurchaseOrderQB.PurchaseOrderResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<PurchaseOrderQB.PurchaseOrderResponse>(Test);
                                    Data = new tblPurchaseOrder();
                                    Data.QBId = Convert.ToInt32(ResponseData.PurchaseOrder.Id);
                                    Data.SyncToken = ResponseData.PurchaseOrder.SyncToken;
                                    Data.PurchaseOrderNumber = ResponseData.PurchaseOrder.DocNumber;
                                    int QBId = Convert.ToInt32(ResponseData.PurchaseOrder.VendorRef.value); ;
                                    int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.SupplierId = VendorId;
                                    //Data.ServiceLocationId = Estimate.EstimateData.ServiceLocationId;
                                    //Data.Email = ResponseData.Estimate.Domain;
                                    //Data.IssueDate = DateTime.Now;
                                    //Data.ContactId = Estimate.EstimateData.ContactId;
                                    //Data.RegionalManagerId = Estimate.EstimateData.RegionalManagerId;
                                    //Data.AssignTo = Estimate.EstimateData.AssignTo;
                                    //Data.RequestedBy = Estimate.EstimateData.RequestedBy;
                                    if (ResponseData.PurchaseOrder.POStatus.Contains("Open"))
                                    {
                                        Data.StatusId = 1;
                                    }
                                    else if (ResponseData.PurchaseOrder.POStatus.Contains("Close"))
                                    {
                                        Data.StatusId = 2;
                                    }

                                    //Data.Tax = Estimate.EstimateData.Tax;
                                    //Data.Tags = Estimate.EstimateData.Tags;
                                    //Data.Discount = Estimate.EstimateData.Discount;
                                    //Data.Shipping = Estimate.EstimateData.Shipping;
                                    //Data.Profit = Estimate.EstimateData.Profit;
                                    //Data.ProfitPercentage = Estimate.EstimateData.ProfitPercentage;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    //Data.CreatedBy = UserId;
                                    Data.Date = DateTime.Now;
                                    Data.Amount = (double)ResponseData.PurchaseOrder.TotalAmt;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    Data.PurchaseOrderNumber = ResponseData.PurchaseOrder.DocNumber;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("P").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblPurchaseOrders.Add(Data);
                                    DB.SaveChanges();


                                    foreach (PurchaseOrderQB.Line item in ResponseData.PurchaseOrder.Line)
                                    {
                                        if (item.Id != null && item.ItemBasedExpenseLineDetail != null)
                                        {
                                            tblPurchaseOrderItem Item = new tblPurchaseOrderItem();
                                            //Item.Description = item.Description;
                                            Item.Qty = Convert.ToInt32(item.ItemBasedExpenseLineDetail.Qty);
                                            Item.Rate = Convert.ToDouble(item.ItemBasedExpenseLineDetail.UnitPrice);
                                            Item.Amount = Convert.ToDouble(item.Amount);
                                            QBId = Convert.ToInt32(item.ItemBasedExpenseLineDetail.ItemRef.value);
                                            int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                            Item.ItemId = ItemId;
                                            Item.PurchaseOrderId = Data.PurchaseOrderId;
                                            //Item.CreatedBy = UserId;
                                            Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            //Item.EditBy = UserId;
                                            Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.isActive = true;
                                            Item.isDelete = false;
                                            DB.tblPurchaseOrderItems.Add(Item);
                                            DB.SaveChanges();
                                        }
                                        
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.PurchaseOrderId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();

                                }
                                else
                                {
                                    var errorMessage = response.Content.ReadAsStringAsync();
                                    errorMessage.Wait();
                                    var ResultResponse = errorMessage.Result;

                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = ResultResponse;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                        }




                    }
                }
                else if (SyncLog.Operation == "Update")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblPurchaseOrders.Where(x => x.PurchaseOrderId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblPurchaseOrderItems.Where(x => x.PurchaseOrderId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/purchaseorder?minorversion=23";


                        QBPurchaseOrderClass PurchaseOrderData = new QBPurchaseOrderClass();
                        Models.QBPurchaseOrderCUClass.LineDetail LineData = new Models.QBPurchaseOrderCUClass.LineDetail();
                        PurchaseOrderData.APAccountRef = new QBPurchaseOrderCUClass.APAccountRef();
                        PurchaseOrderData.APAccountRef.value = "33";
                        PurchaseOrderData.TotalAmt = (decimal)Data.Amount;
                        PurchaseOrderData.SyncToken = Data.SyncToken;
                        PurchaseOrderData.DocNumber = Data.PurchaseOrderNumber;
                        PurchaseOrderData.Id = Data.QBId.ToString();
                        if(PurchaseOrderData.Id=="")
                        {
                            PurchaseOrderData.Id = null;
                        }
                        PurchaseOrderData.VendorRef = new QBPurchaseOrderCUClass.VendorRef();
                        PurchaseOrderData.VendorRef.value = Data.tblUser.QBId.ToString();
                        if (Data.StatusId == 1)
                        {
                            PurchaseOrderData.POStatus = "Open";
                        }
                        else if (Data.StatusId == 2)
                        {
                            PurchaseOrderData.POStatus = "Closed";
                        }

                        PurchaseOrderData.Line = new List<Models.QBPurchaseOrderCUClass.LineDetail>();
                        foreach (var item in ItemData)
                        {
                            //LineData.Id = "1";
                            //LineData.Description = item.Description;
                            LineData.Amount = Convert.ToDecimal(item.Amount);
                            //LineData.DetailType = item.tblItem.Type;
                            LineData.DetailType = "ItemBasedExpenseLineDetail";
                            LineData.ItemBasedExpenseLineDetail = new Models.QBPurchaseOrderCUClass.ItemBasedExpenseLineDetail();
                            LineData.ItemBasedExpenseLineDetail.ItemRef = new QBPurchaseOrderCUClass.ItemRef();
                            LineData.ItemBasedExpenseLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            //LineData.SalesItemLineDetail.ItemRef.name = "Pest Control";
                            LineData.ItemBasedExpenseLineDetail.UnitPrice = Convert.ToDecimal(item.Rate);
                            LineData.ItemBasedExpenseLineDetail.Qty = (int)item.Qty;
                            
                            PurchaseOrderData.Line.Add(LineData);
                        }


                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(PurchaseOrderData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var ResultResponse = jsonResponse.Result;

                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(ResultResponse);
                                PurchaseOrderQB.PurchaseOrderResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<PurchaseOrderQB.PurchaseOrderResponse>(ResultResponse);
                                //// Now you can access the data using the model
                                //var QBId = estimateModel["PurchaseOrder"]["Id"];
                                //var SyncToken = estimateModel["PurchaseOrder"]["SyncToken"];
                                var QBId = Convert.ToInt32(ResponseData.PurchaseOrder.Id);
                                var SyncToken = ResponseData.PurchaseOrder.SyncToken;

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var ResultResponse = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = ResultResponse;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Make the GET request
                            var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/purchaseorder/" + SyncLog.QBId + "?minorversion=23");
                            Response.Wait();
                            var response = Response.Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var readTask = response.Content.ReadAsStringAsync();
                                readTask.Wait();
                                string Test = readTask.Result;
                                PurchaseOrderQB.PurchaseOrderResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<PurchaseOrderQB.PurchaseOrderResponse>(Test);


                                int QBId = Convert.ToInt32(ResponseData.PurchaseOrder.Id);

                                Data = DB.tblPurchaseOrders.Where(x => x.QBId == QBId).FirstOrDefault();
                                if (Data != null)
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.PurchaseOrder.Id);
                                    Data.PurchaseOrderNumber = ResponseData.PurchaseOrder.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.PurchaseOrder.VendorRef.value); ;
                                    int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.SupplierId = VendorId;
                                    //Data.ServiceLocationId = Estimate.EstimateData.ServiceLocationId;
                                    //Data.Email = ResponseData.Estimate.Domain;
                                    //Data.IssueDate = DateTime.Now;
                                    //Data.ContactId = Estimate.EstimateData.ContactId;
                                    //Data.RegionalManagerId = Estimate.EstimateData.RegionalManagerId;
                                    //Data.AssignTo = Estimate.EstimateData.AssignTo;
                                    //Data.RequestedBy = Estimate.EstimateData.RequestedBy;
                                    if (ResponseData.PurchaseOrder.POStatus.Contains("Open"))
                                    {
                                        Data.StatusId = 1;
                                    }
                                    else if (ResponseData.PurchaseOrder.POStatus.Contains("Close"))
                                    {
                                        Data.StatusId = 2;
                                    }

                                    Data.SyncToken = ResponseData.PurchaseOrder.SyncToken;
                                    //Data.Tax = Estimate.EstimateData.Tax;
                                    //Data.Tags = Estimate.EstimateData.Tags;
                                    //Data.Discount = Estimate.EstimateData.Discount;
                                    //Data.Shipping = Estimate.EstimateData.Shipping;
                                    //Data.Profit = Estimate.EstimateData.Profit;
                                    //Data.ProfitPercentage = Estimate.EstimateData.ProfitPercentage;
                                    Data.Amount = (double)ResponseData.PurchaseOrder.TotalAmt;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.Entry(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblPurchaseOrderItems.Where(x => x.PurchaseOrderId == Data.PurchaseOrderId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblPurchaseOrderItems.RemoveRange(ItemData);
                                    }

                                    foreach (PurchaseOrderQB.Line item in ResponseData.PurchaseOrder.Line)
                                    {
                                        if (item.Id != null && item.ItemBasedExpenseLineDetail != null)
                                        {
                                            tblPurchaseOrderItem Item = new tblPurchaseOrderItem();
                                            //Item.Description = item.Description;
                                            Item.Qty = Convert.ToInt32(item.ItemBasedExpenseLineDetail.Qty);
                                            Item.Rate = Convert.ToDouble(item.ItemBasedExpenseLineDetail.UnitPrice);
                                            Item.Amount = Convert.ToDouble(item.Amount);
                                            QBId = Convert.ToInt32(item.ItemBasedExpenseLineDetail.ItemRef.value);
                                            int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                            Item.ItemId = ItemId;
                                            Item.PurchaseOrderId = Data.PurchaseOrderId;
                                            //Item.CreatedBy = UserId;
                                            Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            //Item.EditBy = UserId;
                                            Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.isActive = true;
                                            Item.isDelete = false;
                                            DB.tblPurchaseOrderItems.Add(Item);
                                            DB.SaveChanges();
                                        }
                                        
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.PurchaseOrderId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    Data = new tblPurchaseOrder();
                                    Data.QBId = Convert.ToInt32(ResponseData.PurchaseOrder.Id);
                                    Data.PurchaseOrderNumber = ResponseData.PurchaseOrder.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.PurchaseOrder.VendorRef.value); ;
                                    int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.SupplierId = VendorId;
                                    //Data.ServiceLocationId = Estimate.EstimateData.ServiceLocationId;
                                    //Data.Email = ResponseData.Estimate.Domain;
                                    //Data.IssueDate = DateTime.Now;
                                    //Data.ContactId = Estimate.EstimateData.ContactId;
                                    //Data.RegionalManagerId = Estimate.EstimateData.RegionalManagerId;
                                    //Data.AssignTo = Estimate.EstimateData.AssignTo;
                                    //Data.RequestedBy = Estimate.EstimateData.RequestedBy;
                                    if (ResponseData.PurchaseOrder.POStatus.Contains("Open"))
                                    {
                                        Data.StatusId = 1;
                                    }
                                    else if (ResponseData.PurchaseOrder.POStatus.Contains("Close"))
                                    {
                                        Data.StatusId = 2;
                                    }

                                    Data.SyncToken = ResponseData.PurchaseOrder.SyncToken;
                                    //Data.Tax = Estimate.EstimateData.Tax;
                                    //Data.Tags = Estimate.EstimateData.Tags;
                                    //Data.Discount = Estimate.EstimateData.Discount;
                                    //Data.Shipping = Estimate.EstimateData.Shipping;
                                    //Data.Profit = Estimate.EstimateData.Profit;
                                    //Data.ProfitPercentage = Estimate.EstimateData.ProfitPercentage;
                                    Data.Amount = (double)ResponseData.PurchaseOrder.TotalAmt;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("P").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblPurchaseOrders.Add(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblPurchaseOrderItems.Where(x => x.PurchaseOrderId == Data.PurchaseOrderId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblPurchaseOrderItems.RemoveRange(ItemData);
                                    }

                                    foreach (PurchaseOrderQB.Line item in ResponseData.PurchaseOrder.Line)
                                    {
                                        if (item.Id != null && item.ItemBasedExpenseLineDetail != null)
                                        {
                                            tblPurchaseOrderItem Item = new tblPurchaseOrderItem();
                                            //Item.Description = item.Description;
                                            Item.Qty = Convert.ToInt32(item.ItemBasedExpenseLineDetail.Qty);
                                            Item.Rate = Convert.ToDouble(item.ItemBasedExpenseLineDetail.UnitPrice);
                                            Item.Amount = Convert.ToDouble(item.Amount);
                                            QBId = Convert.ToInt32(item.ItemBasedExpenseLineDetail.ItemRef.value);
                                            int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                            Item.ItemId = ItemId;
                                            Item.PurchaseOrderId = Data.PurchaseOrderId;
                                            //Item.CreatedBy = UserId;
                                            Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            //Item.EditBy = UserId;
                                            Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.isActive = true;
                                            Item.isDelete = false;
                                            DB.tblPurchaseOrderItems.Add(Item);
                                            DB.SaveChanges();
                                        }
                                        
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.PurchaseOrderId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                            else
                            {
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var ResultResponse = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = ResultResponse;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                        }
                    }
                }
                else if (SyncLog.Operation == "Delete")
                {
                    if (SyncLog.isQB != true)
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/purchaseorder?operation=delete";

                        QBDeleteClass DeleteData = new QBDeleteClass();
                        Data = DB.tblPurchaseOrders.Where(x => x.PurchaseOrderId == SyncLog.Id).FirstOrDefault();
                        DeleteData.Id = Data.QBId.ToString();
                        DeleteData.SyncToken = Data.SyncToken;



                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(DeleteData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;
                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var ResultResponse = jsonResponse.Result;

                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(ResultResponse);
                                PurchaseOrderQB.PurchaseOrderResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<PurchaseOrderQB.PurchaseOrderResponse>(ResultResponse);
                                //// Now you can access the data using the model
                                //var QBId = estimateModel["PurchaseOrder"]["Id"];
                                //var SyncToken = estimateModel["PurchaseOrder"]["SyncToken"];
                                var QBId = Convert.ToInt32(ResponseData.PurchaseOrder.Id);
                                var SyncToken = ResponseData.PurchaseOrder.SyncToken;


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                var errorResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(RR);
                                if (errorResponse.Fault.Error.FirstOrDefault().Message.Contains("Object Not Found"))
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorResponse.Fault.Error.FirstOrDefault().Message;
                                    SyncLogData.EditDate = DateTime.Now;
                                    SyncLogData.isSync = true;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = RR;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }

                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblPurchaseOrders.Select(r => r).Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            Data.isDelete = true;
                            Data.EditDate = DateTime.Now;
                            DB.Entry(Data);
                            DB.SaveChanges();

                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Data.EstimateId;
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Message = "Object Not Found";
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }

                    }
                }
                return "Success";
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }

        }

        [HttpPost]
        public string SyncInvoiceAsync(tblSyncLog SyncLog)
        {
            try
            {
                tblInvoice Data = new tblInvoice();
                List<tblInvoiceItem> ItemData = new List<tblInvoiceItem>();
                List<tblInvoiceFile> FileData = new List<tblInvoiceFile>();
                if (SyncLog.Operation == "Create")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblInvoices.Where(x => x.InvoiceId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblInvoiceItems.Where(x => x.InvoiceId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            DB = new earthcoEntities();
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/invoice?minorversion=23";

                        QBInvoiceClass InvoiceData = new QBInvoiceClass();
                        Models.QBInvoiceCUClass.LineDetail LineData = new Models.QBInvoiceCUClass.LineDetail();
                        InvoiceData.CustomerMemo = new QBInvoiceCUClass.CustomerMemo();
                        InvoiceData.CustomerMemo.value = Data.CustomerMessage;
                        //InvoiceData.TotalAmt = (decimal)Data.TotalAmount;
                        InvoiceData.Balance = (decimal)Data.BalanceAmount;
                        InvoiceData.DocNumber = Data.InvoiceNumber;
                        if(Data.DueDate!=null)
                        {
                            InvoiceData.DueDate =(DateTime) Data.DueDate;
                        }
                        
                        //EsitimateData.SyncToken = "0";
                        //EsitimateData.Id = "1103";
                        InvoiceData.CustomerRef = new QBInvoiceCUClass.CustomerRef();
                        InvoiceData.CustomerRef.value = Data.tblUser.QBId.ToString();

                        InvoiceData.Line = new List<Models.QBInvoiceCUClass.LineDetail>();
                        foreach (var item in ItemData)
                        {
                            LineData.Amount = Convert.ToDecimal(item.Amount);
                            LineData.Description = item.Description;
                            //LineData.DetailType = item.tblItem.Type;
                            LineData.DetailType = "SalesItemLineDetail";
                            LineData.SalesItemLineDetail = new Models.QBInvoiceCUClass.SalesItemLineDetail();
                            LineData.SalesItemLineDetail.ItemRef = new QBInvoiceCUClass.ItemRef();
                            LineData.SalesItemLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            LineData.SalesItemLineDetail.UnitPrice = Convert.ToDecimal(item.Rate);
                            LineData.SalesItemLineDetail.Qty = (int)item.Qty;
                           
                            InvoiceData.Line.Add(LineData);
                        }
                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(InvoiceData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var RR = jsonResponse.Result;

                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(RR);
                                InvoiceQB.InvoiceResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<InvoiceQB.InvoiceResponse>(RR);

                                var QBId = Convert.ToInt32(ResponseData.Invoice.Id);
                                var SyncToken = ResponseData.Invoice.SyncToken;
                                // Now you can access the data using the model
                                //var QBId = estimateModel["Invoice"]["Id"];
                                //var SyncToken = estimateModel["Invoice"]["SyncToken"];

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }


                        //foreach (tblEstimateFile item in FileData)
                        //{
                        //    string baseUrl = "https://sandbox-quickbooks.api.intuit.com";

                        //    // Endpoint
                        //    //string endpoint = "/v3/company/<realmID>/upload"; // Replace <realmID> with the actual realm ID

                        //    // Complete URL
                        //    //apiUrl = baseUrl + endpoint;

                        //    // Request body
                        //    var requestBody = new
                        //    {
                        //        AttachableRef = new[]
                        //        {
                        //        new
                        //        {
                        //            EntityRef = new
                        //            {
                        //                type = "Estimate",
                        //                value = "95"
                        //            }
                        //        }
                        //    },
                        //        //ContentType = "image/jpg",
                        //        //FileName = "receipt_nov15.jpg"
                        //    };

                        //    // Serialize the request body to JSON
                        //    string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

                        //    using (var httpClient = new HttpClient())
                        //    using (var content = new MultipartFormDataContent())
                        //    {
                        //        // Add AttachableRef as a JSON string
                        //        //var attachableRefJson = "{\"AttachableRef\":[{\"EntityRef\":{\"type\":\"Invoice\",\"value\":\"95\"}}],\"ContentType\":\"image/jpg\",\"FileName\":\"receipt_nov15.jpg\"}";
                        //        content.Add(new StringContent(jsonRequestBody), "application/json");

                        //        // Load the file from a URL
                        //        var fileUrl = "https://earthcoapi.yehtohoga.com/"+item.FilePath+""; // Replace with the actual file URL
                        //        var fileBytes = await httpClient.GetByteArrayAsync(fileUrl);
                        //        var fileContent = new ByteArrayContent(fileBytes);
                        //        content.Add(fileContent, "file", "receipt_nov15.jpg");

                        //        // Make the POST request
                        //        apiUrl = $"/v3/company/" + TokenData.realmId + "/upload";
                        //        var requestUrl = $"{baseUrl}{apiUrl}";

                        //        var response = await httpClient.PostAsync(requestUrl, content);

                        //        if (response.IsSuccessStatusCode)
                        //        {
                        //            // Handle success
                        //            Console.WriteLine("File uploaded successfully!");
                        //        }
                        //        else
                        //        {
                        //            // Handle failure
                        //            Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        //        }
                        //    }
                        //}



                    }
                    else
                    {
                        Data = DB.tblInvoices.Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Convert.ToInt32(Data.InvoiceId);
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblToken TokenData = DB.tblTokens.FirstOrDefault();
                            string AccessToken = "";

                            var diffOfDates = DateTime.Now - TokenData.EditDate;
                            do
                            {
                                TokenData = DB.tblTokens.FirstOrDefault();
                                diffOfDates = DateTime.Now - TokenData.EditDate;
                                if (diffOfDates.Value.Hours >= 1)
                                {
                                    HomeController.GetAuthTokensUsingRefreshTokenAsync();
                                }
                            } while (diffOfDates.Value.Hours >= 1);

                            AccessToken = TokenData.AccessToken;

                            using (HttpClient client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                // Make the GET request
                                var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/invoice/" + SyncLog.QBId + "?minorversion=23");
                                Response.Wait();
                                var response = Response.Result;

                                if (response.IsSuccessStatusCode)
                                {
                                    var readTask = response.Content.ReadAsStringAsync();
                                    readTask.Wait();
                                    string Test = readTask.Result;
                                    InvoiceQB.InvoiceResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<InvoiceQB.InvoiceResponse>(Test);
                                    Data = new tblInvoice();


                                    Data.QBId = Convert.ToInt32(ResponseData.Invoice.Id);
                                    Data.SyncToken = ResponseData.Invoice.SyncToken;
                                    Data.InvoiceNumber = ResponseData.Invoice.DocNumber;
                                    int QBId = Convert.ToInt32(ResponseData.Invoice.CustomerRef.value);
                                    int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.CustomerId = CustomerId;

                                    Data.TotalAmount = (double)ResponseData.Invoice.TotalAmt;
                                    Data.BalanceAmount = (double)ResponseData.Invoice.Balance;
                                    Data.CustomerMessage = ResponseData.Invoice.CustomerMemo.value;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("I").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblInvoices.Add(Data);
                                    DB.SaveChanges();


                                    foreach (InvoiceQB.Line item in ResponseData.Invoice.Line)
                                    {
                                        if(item.Id!=null)
                                        {
                                            tblInvoiceItem Item = new tblInvoiceItem();
                                            Item.Description = item.Description;
                                            Item.Qty = Convert.ToInt32(item.SalesItemLineDetail.Qty);
                                            Item.Rate = Convert.ToDouble(item.SalesItemLineDetail.UnitPrice);
                                            Item.Name = item.SalesItemLineDetail.ItemRef.name;
                                            Item.Amount = Convert.ToDouble(item.Amount);
                                            QBId = Convert.ToInt32(item.SalesItemLineDetail.ItemRef.value);
                                            int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                            Item.ItemId = ItemId;

                                            Item.InvoiceId = Data.InvoiceId;

                                            Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.isCost = false;
                                            Item.isActive = true;
                                            Item.isDelete = false;
                                            DB.tblInvoiceItems.Add(Item);
                                            DB.SaveChanges();
                                        }
                                        
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.InvoiceId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();

                                }
                                else
                                {
                                    var errorMessage = response.Content.ReadAsStringAsync();
                                    errorMessage.Wait();
                                    var RR = errorMessage.Result;

                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = RR;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                        }




                    }
                }
                else if (SyncLog.Operation == "Update")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblInvoices.Where(x => x.InvoiceId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblInvoiceItems.Where(x => x.InvoiceId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/invoice?minorversion=23";


                        QBInvoiceClass InvoiceData = new QBInvoiceClass();
                        Models.QBInvoiceCUClass.LineDetail LineData = new Models.QBInvoiceCUClass.LineDetail();
                        InvoiceData.CustomerMemo = new QBInvoiceCUClass.CustomerMemo();
                        InvoiceData.CustomerMemo.value = Data.CustomerMessage;
                        //InvoiceData.TotalAmt = (decimal)Data.TotalAmount;
                        InvoiceData.Balance = (decimal)Data.BalanceAmount;
                        InvoiceData.DocNumber = Data.InvoiceNumber;
                        if (Data.DueDate != null)
                        {
                            InvoiceData.DueDate = (DateTime)Data.DueDate;
                        }

                        InvoiceData.SyncToken = Data.SyncToken;
                        InvoiceData.Id = Data.QBId.ToString();
                        InvoiceData.CustomerRef = new QBInvoiceCUClass.CustomerRef();
                        InvoiceData.CustomerRef.value = Data.tblUser.QBId.ToString();

                        InvoiceData.Line = new List<Models.QBInvoiceCUClass.LineDetail>();
                        foreach (var item in ItemData)
                        {
                            LineData.Amount = Convert.ToDecimal(item.Amount);
                            LineData.Description = item.Description;
                            LineData.DetailType = "SalesItemLineDetail";
                            LineData.SalesItemLineDetail = new Models.QBInvoiceCUClass.SalesItemLineDetail();
                            LineData.SalesItemLineDetail.ItemRef = new QBInvoiceCUClass.ItemRef();
                            LineData.SalesItemLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            LineData.SalesItemLineDetail.UnitPrice = Convert.ToDecimal(item.Rate);
                            LineData.SalesItemLineDetail.Qty = (int)item.Qty;
                            
                            InvoiceData.Line.Add(LineData);
                        }


                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(InvoiceData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;
                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var RR = jsonResponse.Result;
                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(RR);
                                InvoiceQB.InvoiceResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<InvoiceQB.InvoiceResponse>(RR);

                                var QBId = Convert.ToInt32(ResponseData.Invoice.Id);
                                var SyncToken = ResponseData.Invoice.SyncToken;
                                // Now you can access the data using the model

                                //var QBId = estimateModel["Invoice"]["Id"];
                                //var SyncToken = estimateModel["Invoice"]["SyncToken"];

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Make the GET request
                            var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/invoice/" + SyncLog.QBId + "?minorversion=23");
                            Response.Wait();
                            var response = Response.Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var readTask = response.Content.ReadAsStringAsync();
                                readTask.Wait();
                                string Test = readTask.Result;
                                InvoiceQB.InvoiceResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<InvoiceQB.InvoiceResponse>(Test);


                                int QBId = Convert.ToInt32(ResponseData.Invoice.Id);

                                Data = DB.tblInvoices.Where(x => x.QBId == QBId).FirstOrDefault();
                                if (Data != null)
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.Invoice.Id);
                                    Data.SyncToken = ResponseData.Invoice.SyncToken;
                                    Data.InvoiceNumber = ResponseData.Invoice.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.Invoice.CustomerRef.value); ;
                                    int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.CustomerId = CustomerId;

                                    Data.TotalAmount = (double)ResponseData.Invoice.TotalAmt;
                                    Data.BalanceAmount = (double)ResponseData.Invoice.Balance;
                                    Data.CustomerMessage = ResponseData.Invoice.CustomerMemo.value;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.Entry(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblInvoiceItems.Where(x => x.InvoiceId == Data.InvoiceId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblInvoiceItems.RemoveRange(ItemData);
                                    }

                                    foreach (InvoiceQB.Line item in ResponseData.Invoice.Line)
                                    {
                                        if (item.Id != null && item.SalesItemLineDetail != null)
                                        {
                                            tblInvoiceItem Item = new tblInvoiceItem();
                                            Item.Description = item.Description;
                                            Item.Qty = Convert.ToInt32(item.SalesItemLineDetail.Qty);
                                            Item.Rate = Convert.ToDouble(item.SalesItemLineDetail.UnitPrice);
                                            Item.Name = item.SalesItemLineDetail.ItemRef.name;
                                            Item.Amount = Convert.ToDouble(item.Amount);
                                            QBId = Convert.ToInt32(item.SalesItemLineDetail.ItemRef.value);
                                            int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                            Item.ItemId = ItemId;

                                            Item.InvoiceId = Data.InvoiceId;

                                            Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.isCost = false;
                                            Item.isActive = true;
                                            Item.isDelete = false;
                                            DB.tblInvoiceItems.Add(Item);
                                            DB.SaveChanges();
                                        }
                                        
                                    }

                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.InvoiceId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    Data = new tblInvoice();
                                    Data.QBId = Convert.ToInt32(ResponseData.Invoice.Id);
                                    Data.SyncToken = ResponseData.Invoice.SyncToken;
                                    Data.InvoiceNumber = ResponseData.Invoice.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.Invoice.CustomerRef.value); ;
                                    int CustomerId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.CustomerId = CustomerId;

                                    Data.TotalAmount = (double)ResponseData.Invoice.TotalAmt;
                                    Data.BalanceAmount = (double)ResponseData.Invoice.Balance;
                                    Data.CustomerMessage = ResponseData.Invoice.CustomerMemo.value;
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("I").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblInvoices.Add(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblInvoiceItems.Where(x => x.InvoiceId == Data.InvoiceId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblInvoiceItems.RemoveRange(ItemData);
                                    }

                                    foreach (InvoiceQB.Line item in ResponseData.Invoice.Line)
                                    {
                                        if (item.Id != null && item.SalesItemLineDetail != null)
                                        {
                                            tblInvoiceItem Item = new tblInvoiceItem();
                                            Item.Description = item.Description;
                                            Item.Qty = Convert.ToInt32(item.SalesItemLineDetail.Qty);
                                            Item.Rate = Convert.ToDouble(item.SalesItemLineDetail.UnitPrice);
                                            Item.Name = item.SalesItemLineDetail.ItemRef.name;
                                            Item.Amount = Convert.ToDouble(item.Amount);
                                            QBId = Convert.ToInt32(item.SalesItemLineDetail.ItemRef.value);
                                            int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                            Item.ItemId = ItemId;

                                            Item.InvoiceId = Data.InvoiceId;

                                            Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.isCost = false;
                                            Item.isActive = true;
                                            Item.isDelete = false;
                                            DB.tblInvoiceItems.Add(Item);
                                            DB.SaveChanges();
                                        }
                                        
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.InvoiceId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                            else
                            {
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                        }
                    }
                }
                else if (SyncLog.Operation == "Delete")
                {
                    if (SyncLog.isQB != true)
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/invoice?operation=delete";

                        QBDeleteClass DeleteData = new QBDeleteClass();
                        Data = DB.tblInvoices.Where(x => x.InvoiceId == SyncLog.Id).FirstOrDefault();
                        DeleteData.Id = Data.QBId.ToString();
                        DeleteData.SyncToken = Data.SyncToken;



                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(DeleteData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;
                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var RR = jsonResponse.Result;

                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(RR);
                                InvoiceQB.InvoiceResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<InvoiceQB.InvoiceResponse>(RR);

                                var QBId = Convert.ToInt32(ResponseData.Invoice.Id);
                                var SyncToken = ResponseData.Invoice.SyncToken;
                                // Now you can access the data using the model
                                //var QBId = estimateModel["Invoice"]["Id"];


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;
                                var errorResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(RR);
                                if (errorResponse.Fault.Error.FirstOrDefault().Message.Contains("Object Not Found"))
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorResponse.Fault.Error.FirstOrDefault().Message;
                                    SyncLogData.EditDate = DateTime.Now;
                                    SyncLogData.isSync = true;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = RR;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }

                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblInvoices.Select(r => r).Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            Data.isDelete = true;
                            Data.EditDate = DateTime.Now;
                            DB.Entry(Data);
                            DB.SaveChanges();

                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Data.EstimateId;
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Message = "Object Not Found";
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }

                    }
                }
                return "Success";
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }

        }

        [HttpPost]
        public string SyncBillAsync(tblSyncLog SyncLog)
        {
            try
            {
                tblBill Data = new tblBill();
                List<tblBillItem> ItemData = new List<tblBillItem>();
                List<tblBillFile> FileData = new List<tblBillFile>();
                if (SyncLog.Operation == "Create")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblBills.Where(x => x.BillId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblBillItems.Where(x => x.BillId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            DB = new earthcoEntities();
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/bill?minorversion=23";

                        QBBillClass BillData = new QBBillClass();
                        Models.QBBillCUClass.LineDetail LineData = new Models.QBBillCUClass.LineDetail();
                        BillData.TotalAmt = (decimal)Data.Amount;
                        BillData.DocNumber = Data.BillNumber;
                        if (Data.DueDate != null)
                        {
                            BillData.DueDate = (DateTime)Data.DueDate;
                        }

                        //EsitimateData.SyncToken = "0";
                        //EsitimateData.Id = "1103";
                        BillData.VendorRef = new QBBillCUClass.VendorRef();
                        BillData.VendorRef.value = Data.tblUser.QBId.ToString();

                        BillData.Line = new List<Models.QBBillCUClass.LineDetail>();
                        foreach (var item in ItemData)
                        {
                            LineData.Amount = Convert.ToDecimal(item.Amount);
                            LineData.Description = item.Description;
                            //LineData.DetailType = item.tblItem.Type;
                            LineData.DetailType = "ItemBasedExpenseLineDetail";
                            LineData.ItemBasedExpenseLineDetail = new Models.QBBillCUClass.ItemBasedExpenseLineDetail();
                            LineData.ItemBasedExpenseLineDetail.ItemRef = new QBBillCUClass.ItemRef();
                            LineData.ItemBasedExpenseLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            LineData.ItemBasedExpenseLineDetail.UnitPrice = Convert.ToDecimal(item.Rate);
                            LineData.ItemBasedExpenseLineDetail.Qty = (int)item.Qty;
                            
                            BillData.Line.Add(LineData);
                        }
                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(BillData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var RR = jsonResponse.Result;
                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                                QBBill.BillResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBBill.BillResponse>(RR);

                                var QBId = Convert.ToInt32(ResponseData.Bill.Id);
                                var SyncToken = ResponseData.Bill.SyncToken;
                                // Now you can access the data using the model
                                //var QBId = estimateModel["Bill"]["Id"];
                                //var SyncToken = estimateModel["Bill"]["SyncToken"];

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }


                        //foreach (tblEstimateFile item in FileData)
                        //{
                        //    string baseUrl = "https://sandbox-quickbooks.api.intuit.com";

                        //    // Endpoint
                        //    //string endpoint = "/v3/company/<realmID>/upload"; // Replace <realmID> with the actual realm ID

                        //    // Complete URL
                        //    //apiUrl = baseUrl + endpoint;

                        //    // Request body
                        //    var requestBody = new
                        //    {
                        //        AttachableRef = new[]
                        //        {
                        //        new
                        //        {
                        //            EntityRef = new
                        //            {
                        //                type = "Estimate",
                        //                value = "95"
                        //            }
                        //        }
                        //    },
                        //        //ContentType = "image/jpg",
                        //        //FileName = "receipt_nov15.jpg"
                        //    };

                        //    // Serialize the request body to JSON
                        //    string jsonRequestBody = Newtonsoft.Json.JsonConvert.SerializeObject(requestBody);

                        //    using (var httpClient = new HttpClient())
                        //    using (var content = new MultipartFormDataContent())
                        //    {
                        //        // Add AttachableRef as a JSON string
                        //        //var attachableRefJson = "{\"AttachableRef\":[{\"EntityRef\":{\"type\":\"Bill\",\"value\":\"95\"}}],\"ContentType\":\"image/jpg\",\"FileName\":\"receipt_nov15.jpg\"}";
                        //        content.Add(new StringContent(jsonRequestBody), "application/json");

                        //        // Load the file from a URL
                        //        var fileUrl = "https://earthcoapi.yehtohoga.com/"+item.FilePath+""; // Replace with the actual file URL
                        //        var fileBytes = await httpClient.GetByteArrayAsync(fileUrl);
                        //        var fileContent = new ByteArrayContent(fileBytes);
                        //        content.Add(fileContent, "file", "receipt_nov15.jpg");

                        //        // Make the POST request
                        //        apiUrl = $"/v3/company/" + TokenData.realmId + "/upload";
                        //        var requestUrl = $"{baseUrl}{apiUrl}";

                        //        var response = await httpClient.PostAsync(requestUrl, content);

                        //        if (response.IsSuccessStatusCode)
                        //        {
                        //            // Handle success
                        //            Console.WriteLine("File uploaded successfully!");
                        //        }
                        //        else
                        //        {
                        //            // Handle failure
                        //            Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                        //        }
                        //    }
                        //}



                    }
                    else
                    {
                        Data = DB.tblBills.Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Convert.ToInt32(Data.BillId);
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblToken TokenData = DB.tblTokens.FirstOrDefault();
                            string AccessToken = "";

                            var diffOfDates = DateTime.Now - TokenData.EditDate;
                            do
                            {
                                TokenData = DB.tblTokens.FirstOrDefault();
                                diffOfDates = DateTime.Now - TokenData.EditDate;
                                if (diffOfDates.Value.Hours >= 1)
                                {
                                    HomeController.GetAuthTokensUsingRefreshTokenAsync();
                                }
                            } while (diffOfDates.Value.Hours >= 1);

                            AccessToken = TokenData.AccessToken;

                            using (HttpClient client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                // Make the GET request
                                var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/bill/" + SyncLog.QBId + "?minorversion=23");
                                Response.Wait();
                                var response = Response.Result;

                                if (response.IsSuccessStatusCode)
                                {
                                    var readTask = response.Content.ReadAsStringAsync();
                                    readTask.Wait();
                                    string Test = readTask.Result;
                                    QBBill.BillResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBBill.BillResponse>(Test);
                                    Data = new tblBill();
                                    Data.QBId = Convert.ToInt32(ResponseData.Bill.Id);
                                    Data.SyncToken = ResponseData.Bill.SyncToken;
                                    Data.BillNumber = ResponseData.Bill.DocNumber;
                                    int QBId = Convert.ToInt32(ResponseData.Bill.VendorRef.value); ;
                                    int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.SupplierId = VendorId;

                                    Data.DueDate = (DateTime)ResponseData.Bill.DueDate;
                                    Data.BillDate = (DateTime)ResponseData.Bill.TxnDate;
                                    Data.Amount = (double)ResponseData.Bill.TotalAmt;
                                    Data.Currency = "USD";
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("B").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblBills.Add(Data);
                                    DB.SaveChanges();


                                    foreach (QBBill.Line item in ResponseData.Bill.Line)
                                    {
                                        if (item.Id != null && item.ItemBasedExpenseLineDetail!=null)
                                        {
                                            tblBillItem Item = new tblBillItem();
                                            Item.Description = item.Description;
                                            if(item.ItemBasedExpenseLineDetail!=null)
                                            {
                                                Item.Qty = Convert.ToInt32(item.ItemBasedExpenseLineDetail.Qty);
                                                Item.Rate = Convert.ToDouble(item.ItemBasedExpenseLineDetail.UnitPrice);
                                                Item.Name = item.ItemBasedExpenseLineDetail.ItemRef.name;
                                                QBId = Convert.ToInt32(item.ItemBasedExpenseLineDetail.ItemRef.value);
                                            }

                                            int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                            Item.Amount = Convert.ToDouble(item.Amount);
                                            Item.ItemId = ItemId;

                                            Item.BillId = Data.BillId;

                                            Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.isActive = true;
                                            Item.isDelete = false;
                                            DB.tblBillItems.Add(Item);
                                            DB.SaveChanges();
                                        }
                                        
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.BillId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();

                                }
                                else
                                {
                                    var errorMessage = response.Content.ReadAsStringAsync();
                                    errorMessage.Wait();
                                    var RR = errorMessage.Result;

                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = RR;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                        }




                    }
                }
                else if (SyncLog.Operation == "Update")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblBills.Where(x => x.BillId == SyncLog.Id).FirstOrDefault();
                        ItemData = DB.tblBillItems.Where(x => x.BillId == SyncLog.Id).ToList();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/bill?minorversion=23";


                        QBBillClass BillData = new QBBillClass();
                        Models.QBBillCUClass.LineDetail LineData = new Models.QBBillCUClass.LineDetail();
                        BillData.TotalAmt = (decimal)Data.Amount;
                        BillData.DocNumber = Data.BillNumber;
                        if (Data.DueDate != null)
                        {
                            BillData.DueDate = (DateTime)Data.DueDate;
                        }

                        BillData.SyncToken = Data.SyncToken;
                        BillData.Id = Data.QBId.ToString();
                        BillData.VendorRef = new QBBillCUClass.VendorRef();
                        BillData.VendorRef.value = Data.tblUser.QBId.ToString();

                        BillData.Line = new List<Models.QBBillCUClass.LineDetail>();
                        foreach (var item in ItemData)
                        {
                            LineData.Amount = Convert.ToDecimal(item.Amount);
                            LineData.Description = item.Description;
                            //LineData.DetailType = item.tblItem.Type;
                            LineData.DetailType = "ItemBasedExpenseLineDetail";
                            LineData.ItemBasedExpenseLineDetail = new Models.QBBillCUClass.ItemBasedExpenseLineDetail();
                            LineData.ItemBasedExpenseLineDetail.ItemRef = new QBBillCUClass.ItemRef();
                            LineData.ItemBasedExpenseLineDetail.ItemRef.value = item.tblItem.QBId.ToString();
                            LineData.ItemBasedExpenseLineDetail.UnitPrice = Convert.ToDecimal(item.Rate);
                            LineData.ItemBasedExpenseLineDetail.Qty = (int)item.Qty;
                            
                            BillData.Line.Add(LineData);
                        }


                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(BillData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var RR = jsonResponse.Result;
                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                                QBBill.BillResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBBill.BillResponse>(RR);

                                var QBId = Convert.ToInt32(ResponseData.Bill.Id);
                                var SyncToken = ResponseData.Bill.SyncToken;
                                // Now you can access the data using the model

                                //var QBId = estimateModel["Bill"]["Id"];
                                //var SyncToken = estimateModel["Bill"]["SyncToken"];

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Make the GET request
                            var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/bill/" + SyncLog.QBId + "?minorversion=23");
                            Response.Wait();
                            var response=Response.Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var readTask = response.Content.ReadAsStringAsync();
                                readTask.Wait();
                                string Test = readTask.Result;
                                QBBill.BillResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBBill.BillResponse>(Test);


                                int QBId = Convert.ToInt32(ResponseData.Bill.Id);

                                Data = DB.tblBills.Where(x => x.QBId == QBId).FirstOrDefault();
                                if (Data != null)
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.Bill.Id);
                                    Data.SyncToken = ResponseData.Bill.SyncToken;
                                    Data.BillNumber = ResponseData.Bill.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.Bill.VendorRef.value); ;
                                    int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.SupplierId = VendorId;

                                    Data.DueDate = (DateTime)ResponseData.Bill.DueDate;
                                    Data.BillDate = (DateTime)ResponseData.Bill.TxnDate;
                                    Data.Amount = (double)ResponseData.Bill.TotalAmt;
                                    Data.Currency = "USD";
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.Entry(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblBillItems.Where(x => x.BillId == Data.BillId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblBillItems.RemoveRange(ItemData);
                                    }

                                    foreach (QBBill.Line item in ResponseData.Bill.Line)
                                    {
                                        if (item.Id != null && item.ItemBasedExpenseLineDetail != null)
                                        {
                                            tblBillItem Item = new tblBillItem();
                                            Item.Description = item.Description;
                                            Item.Qty = Convert.ToInt32(item.ItemBasedExpenseLineDetail.Qty);
                                            Item.Rate = Convert.ToDouble(item.ItemBasedExpenseLineDetail.UnitPrice);
                                            Item.Name = item.ItemBasedExpenseLineDetail.ItemRef.name;
                                            Item.Amount = Convert.ToDouble(item.Amount);
                                            QBId = Convert.ToInt32(item.ItemBasedExpenseLineDetail.ItemRef.value);
                                            int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                            Item.ItemId = ItemId;

                                            Item.BillId = Data.BillId;

                                            Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.isActive = true;
                                            Item.isDelete = false;
                                            DB.tblBillItems.Add(Item);
                                            DB.SaveChanges();
                                        }
                                        
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.BillId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    Data =new tblBill();
                                    Data.QBId = Convert.ToInt32(ResponseData.Bill.Id);
                                    Data.SyncToken = ResponseData.Bill.SyncToken;
                                    Data.BillNumber = ResponseData.Bill.DocNumber;
                                    QBId = Convert.ToInt32(ResponseData.Bill.VendorRef.value); ;
                                    int VendorId = DB.tblUsers.Where(x => x.QBId == QBId).Select(s => s.UserId).FirstOrDefault();
                                    Data.SupplierId = VendorId;

                                    Data.DueDate = (DateTime)ResponseData.Bill.DueDate;
                                    Data.BillDate = (DateTime)ResponseData.Bill.TxnDate;
                                    Data.Amount = (double)ResponseData.Bill.TotalAmt;
                                    Data.Currency = "USD";
                                    //Data.BalanceAmount = Estimate.EstimateData.BalanceAmount;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    //Data.CreatedBy = UserId;
                                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("B").FirstOrDefault());
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblBills.Add(Data);
                                    DB.SaveChanges();

                                    ItemData = DB.tblBillItems.Where(x => x.BillId == Data.BillId).ToList();
                                    if (ItemData != null && ItemData.Count != 0)
                                    {
                                        DB.tblBillItems.RemoveRange(ItemData);
                                    }

                                    foreach (QBBill.Line item in ResponseData.Bill.Line)
                                    {
                                        if (item.Id != null && item.ItemBasedExpenseLineDetail != null)
                                        {
                                            tblBillItem Item = new tblBillItem();
                                            Item.Description = item.Description;
                                            Item.Qty = Convert.ToInt32(item.ItemBasedExpenseLineDetail.Qty);
                                            Item.Rate = Convert.ToDouble(item.ItemBasedExpenseLineDetail.UnitPrice);
                                            Item.Name = item.ItemBasedExpenseLineDetail.ItemRef.name;
                                            Item.Amount = Convert.ToDouble(item.Amount);
                                            QBId = Convert.ToInt32(item.ItemBasedExpenseLineDetail.ItemRef.value);
                                            int ItemId = DB.tblItems.Where(x => x.QBId == QBId).Select(s => s.ItemId).FirstOrDefault();
                                            Item.ItemId = ItemId;

                                            Item.BillId = Data.BillId;

                                            Item.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                            Item.isActive = true;
                                            Item.isDelete = false;
                                            DB.tblBillItems.Add(Item);
                                            DB.SaveChanges();
                                        }
                                        
                                    }


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.BillId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                            else
                            {
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                        }
                    }
                }
                else if (SyncLog.Operation == "Delete")
                {
                    if (SyncLog.isQB != true)
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/bill?operation=delete";

                        QBDeleteClass DeleteData = new QBDeleteClass();
                        Data = DB.tblBills.Where(x => x.BillId == SyncLog.Id).FirstOrDefault();
                        DeleteData.Id = Data.QBId.ToString();
                        DeleteData.SyncToken = Data.SyncToken;



                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(DeleteData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var RR = jsonResponse.Result;
                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                                QBBill.BillResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBBill.BillResponse>(RR);

                                var QBId = Convert.ToInt32(ResponseData.Bill.Id);
                                var SyncToken = ResponseData.Bill.SyncToken;
                                // Now you can access the data using the model
                                //var QBId = estimateModel["Bill"]["Id"];


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;
                                var errorResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(RR);
                                if (errorResponse.Fault.Error.FirstOrDefault().Message.Contains("Object Not Found"))
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorResponse.Fault.Error.FirstOrDefault().Message;
                                    SyncLogData.EditDate = DateTime.Now;
                                    SyncLogData.isSync = true;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = RR;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }

                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblBills.Select(r => r).Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            Data.isDelete = true;
                            Data.EditDate = DateTime.Now;
                            DB.Entry(Data);
                            DB.SaveChanges();

                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Data.BillId;
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Message = "Object Not Found";
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }

                    }
                }
                return "Success";
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }

        }

        [HttpPost]
        public string SyncItemAsync(tblSyncLog SyncLog)
        {
            try
            {
                tblItem Data = new tblItem();
                if (SyncLog.Operation == "Create")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblItems.Where(x => x.ItemId == SyncLog.Id).FirstOrDefault();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            DB = new earthcoEntities();
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/item?minorversion=23";

                        QBItemClass ItemData = new QBItemClass();
                        ItemData.Type = "NonInventory";
                        ItemData.Name = Data.ItemName;
                        ItemData.UnitPrice = (decimal)Data.SalePrice;
                        ItemData.PurchaseCost = (decimal)Data.PurchasePrice;

                        //EsitimateData.SyncToken = "0";
                        //EsitimateData.Id = "1103";
                        ItemData.IncomeAccountRef = new IncomeAccountRef();
                        ItemData.IncomeAccountRef.value = Data.tblAccount.QBId.ToString();
                        ItemData.ExpenseAccountRef = new ExpenseAccountRef();
                        ItemData.ExpenseAccountRef.value = Data.tblAccount1.QBId.ToString();

                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(ItemData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var RR = jsonResponse.Result;
                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                                QBItem.ItemResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBItem.ItemResponse>(RR);

                                var QBId = Convert.ToInt32(ResponseData.Item.Id);
                                var SyncToken = ResponseData.Item.SyncToken;
                                // Now you can access the data using the model
                                //var QBId = estimateModel["Item"]["Id"];
                                //var SyncToken = estimateModel["Item"]["SyncToken"];

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblItems.Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Convert.ToInt32(Data.ItemId);
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblToken TokenData = DB.tblTokens.FirstOrDefault();
                            string AccessToken = "";

                            var diffOfDates = DateTime.Now - TokenData.EditDate;
                            do
                            {
                                TokenData = DB.tblTokens.FirstOrDefault();
                                diffOfDates = DateTime.Now - TokenData.EditDate;
                                if (diffOfDates.Value.Hours >= 1)
                                {
                                    HomeController.GetAuthTokensUsingRefreshTokenAsync();
                                }
                            } while (diffOfDates.Value.Hours >= 1);

                            AccessToken = TokenData.AccessToken;

                            using (HttpClient client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                // Make the GET request
                                var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/item/" + SyncLog.QBId + "?minorversion=23");
                                Response.Wait();
                                var response = Response.Result;

                                if (response.IsSuccessStatusCode)
                                {
                                    var readTask = response.Content.ReadAsStringAsync();
                                    readTask.Wait();
                                    string Test = readTask.Result;
                                    QBItem.ItemResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBItem.ItemResponse>(Test);
                                    Data = new tblItem();
                                    Data.QBId = Convert.ToInt32(ResponseData.Item.Id);
                                    Data.SyncToken = ResponseData.Item.SyncToken;

                                    Data.ItemName = ResponseData.Item.Name;
                                    Data.Type = ResponseData.Item.Type;
                                    Data.SalePrice = (double)ResponseData.Item.UnitPrice;
                                    Data.PurchasePrice = (double)ResponseData.Item.PurchaseCost;
                                    int QBId = Convert.ToInt32(ResponseData.Item.IncomeAccountRef.value);
                                    Data.IncomeAccount =DB.tblAccounts.Where(x=>x.QBId==QBId).Select(s=>s.AccountId).FirstOrDefault() ;
                                    QBId = Convert.ToInt32(ResponseData.Item.ExpenseAccountRef.value);
                                    Data.ExpenseAccount =DB.tblAccounts.Where(x=>x.QBId==QBId).Select(s=>s.AccountId).FirstOrDefault() ;
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblItems.Add(Data);
                                    DB.SaveChanges();


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.ItemId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();

                                }
                                else
                                {
                                    var errorMessage = response.Content.ReadAsStringAsync();
                                    errorMessage.Wait();
                                    var RR = errorMessage.Result;

                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = RR;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                        }




                    }
                }
                else if (SyncLog.Operation == "Update")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblItems.Where(x => x.ItemId == SyncLog.Id).FirstOrDefault();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/item?minorversion=23";


                        QBItemClass ItemData = new QBItemClass();
                        ItemData.Type = "NonInventory";
                        ItemData.Name = Data.ItemName;
                        ItemData.UnitPrice = (decimal)Data.SalePrice;
                        ItemData.PurchaseCost = (decimal)Data.PurchasePrice;
                        ItemData.Name = Data.ItemName;

                        ItemData.SyncToken = Data.SyncToken;
                        ItemData.Id = Data.QBId.ToString();
                        ItemData.IncomeAccountRef = new IncomeAccountRef();
                        ItemData.IncomeAccountRef.value = Data.tblAccount.QBId.ToString();
                        ItemData.ExpenseAccountRef = new ExpenseAccountRef();
                        ItemData.ExpenseAccountRef.value = Data.tblAccount1.QBId.ToString();


                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(ItemData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var RR = jsonResponse.Result;
                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                                QBItem.ItemResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBItem.ItemResponse>(RR);

                                var QBId = Convert.ToInt32(ResponseData.Item.Id);
                                var SyncToken = ResponseData.Item.SyncToken;
                                // Now you can access the data using the model

                                //var QBId = estimateModel["Item"]["Id"];
                                //var SyncToken = estimateModel["Item"]["SyncToken"];

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Make the GET request
                            var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/item/" + SyncLog.QBId + "?minorversion=23");
                            Response.Wait();
                            var response = Response.Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var readTask = response.Content.ReadAsStringAsync();
                                readTask.Wait();
                                string Test = readTask.Result;
                                QBItem.ItemResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBItem.ItemResponse>(Test);


                                int QBId = Convert.ToInt32(ResponseData.Item.Id);

                                Data = DB.tblItems.Where(x => x.QBId == QBId).FirstOrDefault();
                                if (Data != null)
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.Item.Id);
                                    Data.SyncToken = ResponseData.Item.SyncToken;

                                    Data.ItemName = ResponseData.Item.Name;
                                    Data.Type = ResponseData.Item.Type;
                                    Data.SalePrice = (double)ResponseData.Item.UnitPrice;
                                    Data.PurchasePrice = (double)ResponseData.Item.PurchaseCost;
                                    QBId = Convert.ToInt32(ResponseData.Item.IncomeAccountRef.value);
                                    Data.IncomeAccount = DB.tblAccounts.Where(x => x.QBId == QBId).Select(s => s.AccountId).FirstOrDefault();
                                    QBId = Convert.ToInt32(ResponseData.Item.ExpenseAccountRef.value);
                                    Data.ExpenseAccount = DB.tblAccounts.Where(x => x.QBId == QBId).Select(s => s.AccountId).FirstOrDefault();
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.Entry(Data);
                                    DB.SaveChanges();


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.ItemId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    Data = new tblItem();
                                    Data.QBId = Convert.ToInt32(ResponseData.Item.Id);
                                    Data.SyncToken = ResponseData.Item.SyncToken;

                                    Data.ItemName = ResponseData.Item.Name;
                                    Data.Type = ResponseData.Item.Type;
                                    Data.SalePrice = (double)ResponseData.Item.UnitPrice;
                                    Data.PurchasePrice = (double)ResponseData.Item.PurchaseCost;
                                    QBId = Convert.ToInt32(ResponseData.Item.IncomeAccountRef.value);
                                    Data.IncomeAccount = DB.tblAccounts.Where(x => x.QBId == QBId).Select(s => s.AccountId).FirstOrDefault();
                                    QBId = Convert.ToInt32(ResponseData.Item.ExpenseAccountRef.value);
                                    Data.ExpenseAccount = DB.tblAccounts.Where(x => x.QBId == QBId).Select(s => s.AccountId).FirstOrDefault();
                                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblItems.Add(Data);
                                    DB.SaveChanges();


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.ItemId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                            else
                            {
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                        }
                    }
                }
                else if (SyncLog.Operation == "Delete")
                {
                    if (SyncLog.isQB != true)
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/item?operation=delete";

                        QBDeleteClass DeleteData = new QBDeleteClass();
                        Data = DB.tblItems.Where(x => x.ItemId == SyncLog.Id).FirstOrDefault();
                        DeleteData.Id = Data.QBId.ToString();
                        DeleteData.SyncToken = Data.SyncToken;



                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(DeleteData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var RR = jsonResponse.Result;
                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                                QBItem.ItemResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBItem.ItemResponse>(RR);

                                var QBId = Convert.ToInt32(ResponseData.Item.Id);
                                var SyncToken = ResponseData.Item.SyncToken;
                                // Now you can access the data using the model
                                //var QBId = estimateModel["Item"]["Id"];


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;
                                var errorResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(RR);
                                if (errorResponse.Fault.Error.FirstOrDefault().Message.Contains("Object Not Found"))
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorResponse.Fault.Error.FirstOrDefault().Message;
                                    SyncLogData.EditDate = DateTime.Now;
                                    SyncLogData.isSync = true;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = RR;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }

                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblItems.Select(r => r).Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            Data.isDelete = true;
                            Data.EditDate = DateTime.Now;
                            DB.Entry(Data);
                            DB.SaveChanges();

                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Data.ItemId;
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Message = "Object Not Found";
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }

                    }
                }
                return "Success";
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }

        }

        [HttpPost]
        public string SyncVendorAsync(tblSyncLog SyncLog)
        {
            try
            {
                tblUser Data = new tblUser();
                if (SyncLog.Operation == "Create")
                {
                    Data = DB.tblUsers.Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                    if (Data != null)
                    {
                        tblSyncLog SyncLogData = new tblSyncLog();
                        SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                        SyncLogData.Id = Convert.ToInt32(Data.UserId);
                        SyncLogData.isSync = true;
                        SyncLogData.EditDate = DateTime.Now;
                        DB.Entry(SyncLogData);
                        DB.SaveChanges();
                    }
                    else
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Make the GET request
                            var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/vendor/" + SyncLog.QBId + "?minorversion=23");
                            Response.Wait();
                            var response = Response.Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var readTask = response.Content.ReadAsStringAsync();
                                readTask.Wait();
                                string Test = readTask.Result;
                                QBVendor.VendorResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBVendor.VendorResponse>(Test);
                                Data = new tblUser();
                                Data.QBId = Convert.ToInt32(ResponseData.Vendor.Id);
                                Data.SyncToken = ResponseData.Vendor.SyncToken;

                                Data.FirstName = ResponseData.Vendor.GivenName;
                                Data.LastName = ResponseData.Vendor.FamilyName;
                                Data.CompanyName = ResponseData.Vendor.CompanyName;
                                if(ResponseData.Vendor.PrimaryEmailAddr!=null)
                                {
                                    Data.Email = ResponseData.Vendor.PrimaryEmailAddr.Address;
                                }
                                if(ResponseData.Vendor.PrimaryPhone != null)
                                {
                                    Data.Phone = ResponseData.Vendor.PrimaryPhone.FreeFormNumber;
                                }
                                
                                
                                Data.UserTypeId = 3;
                                Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Data.isLoginAllow = false;
                                Data.isActive = true;
                                Data.isDelete = false;
                                DB.tblUsers.Add(Data);
                                DB.SaveChanges();


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Id = Convert.ToInt32(Data.UserId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                            }
                            else
                            {
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                        }
                    }
                }
                else if (SyncLog.Operation == "Update")
                {
                    tblToken TokenData = DB.tblTokens.FirstOrDefault();
                    string AccessToken = "";

                    var diffOfDates = DateTime.Now - TokenData.EditDate;
                    do
                    {
                        TokenData = DB.tblTokens.FirstOrDefault();
                        diffOfDates = DateTime.Now - TokenData.EditDate;
                        if (diffOfDates.Value.Hours >= 1)
                        {
                            HomeController.GetAuthTokensUsingRefreshTokenAsync();
                        }
                    } while (diffOfDates.Value.Hours >= 1);

                    AccessToken = TokenData.AccessToken;

                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                        // Make the GET request
                        var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/vendor/" + SyncLog.QBId + "?minorversion=23");
                        Response.Wait();
                        var response = Response.Result;

                        if (response.IsSuccessStatusCode)
                        {
                            var readTask = response.Content.ReadAsStringAsync();
                            readTask.Wait();
                            string Test = readTask.Result;
                            QBVendor.VendorResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBVendor.VendorResponse>(Test);


                            int QBId = Convert.ToInt32(ResponseData.Vendor.Id);

                            Data = DB.tblUsers.Where(x => x.QBId == QBId).FirstOrDefault();
                            if (Data != null)
                            {
                                Data.QBId = Convert.ToInt32(ResponseData.Vendor.Id);
                                Data.SyncToken = ResponseData.Vendor.SyncToken;

                                Data.FirstName = ResponseData.Vendor.GivenName;
                                Data.LastName = ResponseData.Vendor.FamilyName;
                                Data.CompanyName = ResponseData.Vendor.CompanyName;

                                if (ResponseData.Vendor.PrimaryEmailAddr != null)
                                {
                                    Data.Email = ResponseData.Vendor.PrimaryEmailAddr.Address;
                                }
                                if (ResponseData.Vendor.PrimaryPhone != null)
                                {
                                    Data.Phone = ResponseData.Vendor.PrimaryPhone.FreeFormNumber;
                                }
                                Data.UserTypeId = 3;
                                Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Data.isLoginAllow = false;
                                Data.isActive = true;
                                Data.isDelete = false;
                                DB.Entry(Data);
                                DB.SaveChanges();


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Id = Convert.ToInt32(Data.UserId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                            else
                            {
                                Data = new tblUser();
                                Data.QBId = Convert.ToInt32(ResponseData.Vendor.Id);
                                Data.SyncToken = ResponseData.Vendor.SyncToken;

                                Data.FirstName = ResponseData.Vendor.GivenName;
                                Data.LastName = ResponseData.Vendor.FamilyName;
                                Data.CompanyName = ResponseData.Vendor.CompanyName;
                                if (ResponseData.Vendor.PrimaryEmailAddr != null)
                                {
                                    Data.Email = ResponseData.Vendor.PrimaryEmailAddr.Address;
                                }
                                if (ResponseData.Vendor.PrimaryPhone != null)
                                {
                                    Data.Phone = ResponseData.Vendor.PrimaryPhone.FreeFormNumber;
                                }
                                Data.UserTypeId = 3;
                                Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Data.isLoginAllow = false;
                                Data.isActive = true;
                                Data.isDelete = false;
                                DB.tblUsers.Add(Data);
                                DB.SaveChanges();


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Id = Convert.ToInt32(Data.UserId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                            }
                        }
                        else
                        {
                            var errorMessage = response.Content.ReadAsStringAsync();
                            errorMessage.Wait();
                            var RR = errorMessage.Result;

                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Message = RR;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                    }
                }
                else if (SyncLog.Operation == "Delete")
                {
                    Data = DB.tblUsers.Select(r => r).Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                    if (Data != null)
                    {
                        Data.isDelete = true;
                        Data.EditDate = DateTime.Now;
                        DB.Entry(Data);
                        DB.SaveChanges();

                        tblSyncLog SyncLogData = new tblSyncLog();
                        SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                        SyncLogData.Id = Data.UserId;
                        SyncLogData.isSync = true;
                        SyncLogData.EditDate = DateTime.Now;
                        DB.Entry(SyncLogData);
                        DB.SaveChanges();
                    }
                    else
                    {
                        tblSyncLog SyncLogData = new tblSyncLog();
                        SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                        SyncLogData.Message = "Object Not Found";
                        SyncLogData.isSync = true;
                        SyncLogData.EditDate = DateTime.Now;
                        DB.Entry(SyncLogData);
                        DB.SaveChanges();
                    }
                }
                return "Success";
            }
            catch (DbEntityValidationException dbEx)
            {
                string ErrorString = "";
                // Handle DbEntityValidationException
                foreach (var Vendor in dbEx.EntityValidationErrors)
                {
                    foreach (var Vendor1 in Vendor.ValidationErrors)
                    {
                        ErrorString += Vendor1.ErrorMessage + " ,";
                    }
                }
                Console.WriteLine($"DbEntityValidationException occurred: {dbEx.Message}");
                // Additional handling specific to DbEntityValidationException
                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = new StringContent(ErrorString);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }

        }

        [HttpPost]
        public string SyncStaffAsync(tblSyncLog SyncLog)
        {
            try
            {
                tblUser Data = new tblUser();
                if (SyncLog.Operation == "Create")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblUsers.Where(x => x.UserId == SyncLog.Id).FirstOrDefault();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            DB = new earthcoEntities();
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/employee?minorversion=23";

                        QBStaffClass StaffData = new QBStaffClass();
                        StaffData.GivenName = Data.FirstName;
                        StaffData.FamilyName = Data.LastName;
                        StaffData.PrimaryPhone = new QBStaffCUClass.PrimaryPhone();
                        StaffData.PrimaryPhone.FreeFormNumber = Data.Phone;
                        StaffData.Title = Data.tblRole.Role;
                        //EsitimateData.SyncToken = "0";
                        //EsitimateData.Id = "1103";
                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(StaffData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var RR = jsonResponse.Result;
                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                                QBStaff.StaffResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBStaff.StaffResponse>(RR);

                                var QBId = Convert.ToInt32(ResponseData.Employee.Id);
                                var SyncToken = ResponseData.Employee.SyncToken;
                                // Now you can access the data using the model
                                //var QBId = estimateModel["Employee"]["Id"];
                                //var SyncToken = estimateModel["Employee"]["SyncToken"];

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblUsers.Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Convert.ToInt32(Data.UserId);
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblToken TokenData = DB.tblTokens.FirstOrDefault();
                            string AccessToken = "";

                            var diffOfDates = DateTime.Now - TokenData.EditDate;
                            do
                            {
                                TokenData = DB.tblTokens.FirstOrDefault();
                                diffOfDates = DateTime.Now - TokenData.EditDate;
                                if (diffOfDates.Value.Hours >= 1)
                                {
                                    HomeController.GetAuthTokensUsingRefreshTokenAsync();
                                }
                            } while (diffOfDates.Value.Hours >= 1);

                            AccessToken = TokenData.AccessToken;

                            using (HttpClient client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                // Make the GET request
                                var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/employee/" + SyncLog.QBId + "?minorversion=23");
                                Response.Wait();
                                var response = Response.Result;

                                if (response.IsSuccessStatusCode)
                                {
                                    var readTask = response.Content.ReadAsStringAsync();
                                    readTask.Wait();
                                    string Test = readTask.Result;
                                    QBStaff.StaffResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBStaff.StaffResponse>(Test);
                                    Data = new tblUser();
                                    Data.QBId = Convert.ToInt32(ResponseData.Employee.Id);
                                    Data.SyncToken = ResponseData.Employee.SyncToken;

                                    Data.FirstName = ResponseData.Employee.GivenName;
                                    Data.LastName = ResponseData.Employee.FamilyName;
                                    if(ResponseData.Employee.PrimaryPhone!=null)
                                    {
                                        Data.Phone = ResponseData.Employee.PrimaryPhone.FreeFormNumber;
                                    }
                                    
                                    Data.UserTypeId = 1;
                                    Data.RoleId = 4;
                                    Data.CreatedDate = DateTime.Now;
                                    Data.isLoginAllow = true;
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblUsers.Add(Data);
                                    DB.SaveChanges();


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.UserId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();

                                }
                                else
                                {
                                    var errorMessage = response.Content.ReadAsStringAsync();
                                    errorMessage.Wait();
                                    var RR = errorMessage.Result;

                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = RR;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                        }




                    }
                }
                else if (SyncLog.Operation == "Update")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblUsers.Where(x => x.UserId == SyncLog.Id).FirstOrDefault();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/employee?minorversion=23";


                        QBStaffClass StaffData = new QBStaffClass();
                        StaffData.GivenName = Data.FirstName;
                        StaffData.FamilyName = Data.LastName;
                        StaffData.PrimaryPhone = new QBStaffCUClass.PrimaryPhone();
                        StaffData.PrimaryPhone.FreeFormNumber = Data.Phone;
                        StaffData.Title = Data.tblRole.Role;

                        StaffData.SyncToken = Data.SyncToken;
                        StaffData.Id = Data.QBId.ToString();

                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(StaffData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var RR = jsonResponse.Result;
                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                                QBStaff.StaffResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBStaff.StaffResponse>(RR);

                                var QBId = Convert.ToInt32(ResponseData.Employee.Id);
                                var SyncToken = ResponseData.Employee.SyncToken;
                                // Now you can access the data using the model

                                //var QBId = estimateModel["Employee"]["Id"];
                                //var SyncToken = estimateModel["Employee"]["SyncToken"];

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Make the GET request
                            var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/employee/" + SyncLog.QBId + "?minorversion=23");
                            Response.Wait();
                            var response = Response.Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var readTask = response.Content.ReadAsStringAsync();
                                readTask.Wait();
                                string Test = readTask.Result;
                                QBStaff.StaffResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBStaff.StaffResponse>(Test);


                                int QBId = Convert.ToInt32(ResponseData.Employee.Id);

                                Data = DB.tblUsers.Where(x => x.QBId == QBId).FirstOrDefault();
                                if (Data != null)
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.Employee.Id);
                                    Data.SyncToken = ResponseData.Employee.SyncToken;

                                    Data.FirstName = ResponseData.Employee.GivenName;
                                    Data.LastName = ResponseData.Employee.FamilyName;
                                    if(ResponseData.Employee.PrimaryPhone!=null)
                                    {
                                        Data.Phone = ResponseData.Employee.PrimaryPhone.FreeFormNumber;
                                    }
                                    
                                    Data.UserTypeId = 1;
                                    Data.RoleId = 4;
                                    Data.CreatedDate = DateTime.Now;
                                    Data.isLoginAllow = true;
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.Entry(Data);
                                    DB.SaveChanges();


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.UserId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    Data = new tblUser();
                                    Data.QBId = Convert.ToInt32(ResponseData.Employee.Id);
                                    Data.SyncToken = ResponseData.Employee.SyncToken;

                                    Data.FirstName = ResponseData.Employee.GivenName;
                                    Data.LastName = ResponseData.Employee.FamilyName;
                                    if(ResponseData.Employee.PrimaryPhone!=null)
                                    {
                                        Data.Phone = ResponseData.Employee.PrimaryPhone.FreeFormNumber;
                                    }
                                   
                                    Data.UserTypeId = 1;
                                    Data.RoleId = 4;
                                    Data.CreatedDate = DateTime.Now;
                                    Data.isLoginAllow = true;
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblUsers.Add(Data);
                                    DB.SaveChanges();


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.UserId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                            else
                            {
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                        }
                    }
                }
                else if (SyncLog.Operation == "Delete")
                {
                    if (SyncLog.isQB != true)
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/employee?operation=delete";

                        QBDeleteClass DeleteData = new QBDeleteClass();
                        Data = DB.tblUsers.Where(x => x.UserId == SyncLog.Id).FirstOrDefault();
                        DeleteData.Id = Data.QBId.ToString();
                        DeleteData.SyncToken = Data.SyncToken;



                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(DeleteData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var RR = jsonResponse.Result;
                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                                QBStaff.StaffResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBStaff.StaffResponse>(RR);

                                var QBId = Convert.ToInt32(ResponseData.Employee.Id);
                                var SyncToken = ResponseData.Employee.SyncToken;
                                // Now you can access the data using the model
                                //var QBId = estimateModel["Employee"]["Id"];


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;
                                var errorResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(RR);
                                if (errorResponse.Fault.Error.FirstOrDefault().Message.Contains("Object Not Found"))
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorResponse.Fault.Error.FirstOrDefault().Message;
                                    SyncLogData.EditDate = DateTime.Now;
                                    SyncLogData.isSync = true;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = RR;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }

                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblUsers.Select(r => r).Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            Data.isDelete = true;
                            Data.EditDate = DateTime.Now;
                            DB.Entry(Data);
                            DB.SaveChanges();

                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Data.UserId;
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Message = "Object Not Found";
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }

                    }
                }
                return "Success";
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";

                //return ResponseMessage(responseMessage);
            }

        }

        [HttpPost]
        public string SyncCustomerAsync(tblSyncLog SyncLog)
        {
            try
            {
                tblUser Data = new tblUser();
                if (SyncLog.Operation == "Create")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblUsers.Where(x => x.UserId == SyncLog.Id).FirstOrDefault();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            DB = new earthcoEntities();
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/customer?minorversion=23";

                        QBCustomerClass CustomerData = new QBCustomerClass();
                        CustomerData.GivenName = Data.FirstName;
                        CustomerData.FamilyName = Data.LastName;
                        CustomerData.CompanyName = Data.CompanyName;
                        CustomerData.PrimaryPhone = new QBCustomerCUClass.PrimaryPhone();
                        CustomerData.PrimaryPhone.FreeFormNumber = Data.Phone;
                        CustomerData.PrimaryEmailAddr = new QBCustomerCUClass.PrimaryEmailAddr();
                        CustomerData.PrimaryEmailAddr.Address= Data.Email;
                        //EsitimateData.SyncToken = "0";
                        //EsitimateData.Id = "1103";
                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(CustomerData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var RR = jsonResponse.Result;
                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                                QBCustomer.CustomerResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBCustomer.CustomerResponse>(RR);

                                var QBId = Convert.ToInt32(ResponseData.Customer.Id);
                                var SyncToken = ResponseData.Customer.SyncToken;
                                // Now you can access the data using the model
                                //var QBId = estimateModel["Customer"]["Id"];
                                //var SyncToken = estimateModel["Customer"]["SyncToken"];

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblUsers.Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Convert.ToInt32(Data.UserId);
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblToken TokenData = DB.tblTokens.FirstOrDefault();
                            string AccessToken = "";

                            var diffOfDates = DateTime.Now - TokenData.EditDate;
                            do
                            {
                                TokenData = DB.tblTokens.FirstOrDefault();
                                diffOfDates = DateTime.Now - TokenData.EditDate;
                                if (diffOfDates.Value.Hours >= 1)
                                {
                                    HomeController.GetAuthTokensUsingRefreshTokenAsync();
                                }
                            } while (diffOfDates.Value.Hours >= 1);

                            AccessToken = TokenData.AccessToken;

                            using (HttpClient client = new HttpClient())
                            {
                                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                                client.DefaultRequestHeaders.Accept.Clear();
                                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                                // Make the GET request
                                var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/customer/" + SyncLog.QBId + "?minorversion=23");
                                Response.Wait();
                                var response = Response.Result;

                                if (response.IsSuccessStatusCode)
                                {
                                    var readTask = response.Content.ReadAsStringAsync();
                                    readTask.Wait();
                                    string Test = readTask.Result;
                                    QBCustomer.CustomerResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBCustomer.CustomerResponse>(Test);
                                    Data = new tblUser();
                                    Data.QBId = Convert.ToInt32(ResponseData.Customer.Id);
                                    Data.SyncToken = ResponseData.Customer.SyncToken;

                                    Data.FirstName = ResponseData.Customer.GivenName;
                                    Data.LastName = ResponseData.Customer.FamilyName;
                                    if(ResponseData.Customer.PrimaryPhone!=null)
                                    {
                                        Data.Phone = ResponseData.Customer.PrimaryPhone.FreeFormNumber;
                                    }
                                    if(ResponseData.Customer.PrimaryEmailAddr!=null)
                                    {
                                        Data.Email = ResponseData.Customer.PrimaryEmailAddr.Address;
                                    }
                                    
                                    Data.CompanyName = ResponseData.Customer.CompanyName;
                                    Data.UserTypeId = 2;
                                    Data.RoleId = 2;
                                    Data.isLoginAllow = false;
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblUsers.Add(Data);
                                    DB.SaveChanges();


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.UserId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();

                                }
                                else
                                {
                                    var errorMessage = response.Content.ReadAsStringAsync();
                                    errorMessage.Wait();
                                    var RR = errorMessage.Result;

                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = RR;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                        }




                    }
                }
                else if (SyncLog.Operation == "Update")
                {
                    if (SyncLog.isQB != true)
                    {
                        Data = DB.tblUsers.Where(x => x.UserId == SyncLog.Id).FirstOrDefault();

                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/customer?minorversion=23";


                        QBCustomerClass CustomerData = new QBCustomerClass();
                        CustomerData.GivenName = Data.FirstName;
                        CustomerData.FamilyName = Data.LastName;
                        CustomerData.CompanyName = Data.CompanyName;
                        CustomerData.PrimaryPhone = new QBCustomerCUClass.PrimaryPhone();
                        CustomerData.PrimaryPhone.FreeFormNumber = Data.Phone;
                        CustomerData.PrimaryEmailAddr = new QBCustomerCUClass.PrimaryEmailAddr();
                        CustomerData.PrimaryEmailAddr.Address = Data.Email;
                        CustomerData.SyncToken = Data.SyncToken;
                        CustomerData.Id = Data.QBId.ToString();

                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(CustomerData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var RR = jsonResponse.Result;
                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                                QBCustomer.CustomerResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBCustomer.CustomerResponse>(RR);

                                var QBId = Convert.ToInt32(ResponseData.Customer.Id);
                                var SyncToken = ResponseData.Customer.SyncToken;
                                // Now you can access the data using the model

                                //var QBId = estimateModel["Customer"]["Id"];
                                //var SyncToken = estimateModel["Customer"]["SyncToken"];

                                Data.QBId = QBId;
                                Data.SyncToken = SyncToken;
                                DB.Entry(Data);
                                DB.SaveChanges();

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Make the GET request
                            var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/customer/" + SyncLog.QBId + "?minorversion=23");
                            Response.Wait();
                            var response = Response.Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var readTask = response.Content.ReadAsStringAsync();
                                readTask.Wait();
                                string Test = readTask.Result;
                                QBCustomer.CustomerResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBCustomer.CustomerResponse>(Test);


                                int QBId = Convert.ToInt32(ResponseData.Customer.Id);

                                Data = DB.tblUsers.Where(x => x.QBId == QBId).FirstOrDefault();
                                if (Data != null)
                                {
                                    Data.QBId = Convert.ToInt32(ResponseData.Customer.Id);
                                    Data.SyncToken = ResponseData.Customer.SyncToken;

                                    Data.FirstName = ResponseData.Customer.GivenName;
                                    Data.LastName = ResponseData.Customer.FamilyName;
                                    if (ResponseData.Customer.PrimaryPhone != null)
                                    {
                                        Data.Phone = ResponseData.Customer.PrimaryPhone.FreeFormNumber;
                                    }
                                    if (ResponseData.Customer.PrimaryEmailAddr != null)
                                    {
                                        Data.Email = ResponseData.Customer.PrimaryEmailAddr.Address;
                                    }
                                    Data.CompanyName = ResponseData.Customer.CompanyName;
                                    Data.UserTypeId = 2;
                                    Data.RoleId = 2;
                                    Data.isLoginAllow = false;
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.Entry(Data);
                                    DB.SaveChanges();


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.UserId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    Data = new tblUser();
                                    Data.QBId = Convert.ToInt32(ResponseData.Customer.Id);
                                    Data.SyncToken = ResponseData.Customer.SyncToken;

                                    Data.FirstName = ResponseData.Customer.GivenName;
                                    Data.LastName = ResponseData.Customer.FamilyName;
                                    if (ResponseData.Customer.PrimaryPhone != null)
                                    {
                                        Data.Phone = ResponseData.Customer.PrimaryPhone.FreeFormNumber;
                                    }
                                    if (ResponseData.Customer.PrimaryEmailAddr != null)
                                    {
                                        Data.Email = ResponseData.Customer.PrimaryEmailAddr.Address;
                                    }
                                    Data.CompanyName = ResponseData.Customer.CompanyName;
                                    Data.UserTypeId = 2;
                                    Data.RoleId = 2;
                                    Data.isLoginAllow = false;
                                    Data.isActive = true;
                                    Data.isDelete = false;
                                    DB.tblUsers.Add(Data);
                                    DB.SaveChanges();


                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Id = Convert.ToInt32(Data.UserId);
                                    SyncLogData.isSync = true;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                            }
                            else
                            {
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                        }
                    }
                }
                else if (SyncLog.Operation == "Delete")
                {
                    if (SyncLog.isQB != true)
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;


                        string apiUrl = "https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/customer?operation=delete";

                        QBDeleteClass DeleteData = new QBDeleteClass();
                        Data = DB.tblUsers.Where(x => x.UserId == SyncLog.Id).FirstOrDefault();
                        DeleteData.Id = Data.QBId.ToString();
                        DeleteData.SyncToken = Data.SyncToken;



                        string jsonRequest = Newtonsoft.Json.JsonConvert.SerializeObject(DeleteData);

                        // Create HttpClient
                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + AccessToken);
                            client.DefaultRequestHeaders.Add("Accept", "application/json");
                            // Create the request content
                            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");
                            //var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                            // Make the POST request
                            var Response = client.PostAsync(apiUrl, content);
                            Response.Wait();
                            var response = Response.Result;

                            // Check if the request was successful
                            if (response.IsSuccessStatusCode)
                            {
                                // Parse and use the response data as needed
                                var jsonResponse = response.Content.ReadAsStringAsync();
                                jsonResponse.Wait();
                                var RR = jsonResponse.Result;
                                //dynamic estimateModel = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                                QBCustomer.CustomerResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBCustomer.CustomerResponse>(RR);

                                var QBId = Convert.ToInt32(ResponseData.Customer.Id);
                                var SyncToken = ResponseData.Customer.SyncToken;
                                // Now you can access the data using the model
                                //var QBId = estimateModel["Customer"]["Id"];


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.QBId = Convert.ToInt32(QBId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                                // Process jsonResponse
                                //return View();
                            }
                            else
                            {
                                // Handle error
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;
                                var errorResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<ErrorResponse>(RR);
                                if (errorResponse.Fault.Error.FirstOrDefault().Message.Contains("Object Not Found"))
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = errorResponse.Fault.Error.FirstOrDefault().Message;
                                    SyncLogData.EditDate = DateTime.Now;
                                    SyncLogData.isSync = true;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }
                                else
                                {
                                    tblSyncLog SyncLogData = new tblSyncLog();
                                    SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                    SyncLogData.Message = RR;
                                    SyncLogData.EditDate = DateTime.Now;
                                    DB.Entry(SyncLogData);
                                    DB.SaveChanges();
                                }

                                // Handle error message
                                //return View("Error");
                            }
                        }
                    }
                    else
                    {
                        Data = DB.tblUsers.Select(r => r).Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                        if (Data != null)
                        {
                            Data.isDelete = true;
                            Data.EditDate = DateTime.Now;
                            DB.Entry(Data);
                            DB.SaveChanges();

                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Id = Data.UserId;
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                        else
                        {
                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Message = "Object Not Found";
                            SyncLogData.isSync = true;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }

                    }
                }
                return "Success";
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }

        }

        [HttpPost]
        public string SyncAccountAsync(tblSyncLog SyncLog)
        {
            try
            {
                tblAccount Data = new tblAccount();
                if (SyncLog.Operation == "Create")
                {
                    Data = DB.tblAccounts.Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                    if (Data != null)
                    {
                        tblSyncLog SyncLogData = new tblSyncLog();
                        SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                        SyncLogData.Id = Convert.ToInt32(Data.AccountId);
                        SyncLogData.isSync = true;
                        SyncLogData.EditDate = DateTime.Now;
                        DB.Entry(SyncLogData);
                        DB.SaveChanges();
                    }
                    else
                    {
                        tblToken TokenData = DB.tblTokens.FirstOrDefault();
                        string AccessToken = "";

                        var diffOfDates = DateTime.Now - TokenData.EditDate;
                        do
                        {
                            TokenData = DB.tblTokens.FirstOrDefault();
                            diffOfDates = DateTime.Now - TokenData.EditDate;
                            if (diffOfDates.Value.Hours >= 1)
                            {
                                HomeController.GetAuthTokensUsingRefreshTokenAsync();
                            }
                        } while (diffOfDates.Value.Hours >= 1);

                        AccessToken = TokenData.AccessToken;

                        using (HttpClient client = new HttpClient())
                        {
                            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                            client.DefaultRequestHeaders.Accept.Clear();
                            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                            // Make the GET request
                            var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/account/" + SyncLog.QBId + "?minorversion=23");
                            Response.Wait();
                            var response = Response.Result;

                            if (response.IsSuccessStatusCode)
                            {
                                var readTask = response.Content.ReadAsStringAsync();
                                readTask.Wait();
                                string Test = readTask.Result;
                                QBAccount.AccountResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBAccount.AccountResponse>(Test);
                                Data = new tblAccount();
                                Data.QBId = Convert.ToInt32(ResponseData.Account.Id);
                                Data.SyncToken = ResponseData.Account.SyncToken;
                                //Data.Code = item.AcctNum;

                                Data.Name = ResponseData.Account.Name;
                                if (ResponseData.Account.AccountType != null && ResponseData.Account.AccountType.ToString().ToLower() == "expense")
                                {
                                    Data.Type = "Expense Account";
                                }
                                if (ResponseData.Account.AccountType != null && ResponseData.Account.AccountType.ToString().ToLower() == "income")
                                {
                                    Data.Type = "Income Account";
                                }

                                Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Data.isActive = true;
                                Data.isDelete = false;
                                DB.tblAccounts.Add(Data);
                                DB.SaveChanges();


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Id = Convert.ToInt32(Data.AccountId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                            }
                            else
                            {
                                var errorMessage = response.Content.ReadAsStringAsync();
                                errorMessage.Wait();
                                var RR = errorMessage.Result;

                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Message = RR;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                        }
                    }
                }
                else if (SyncLog.Operation == "Update")
                {
                    tblToken TokenData = DB.tblTokens.FirstOrDefault();
                    string AccessToken = "";

                    var diffOfDates = DateTime.Now - TokenData.EditDate;
                    do
                    {
                        TokenData = DB.tblTokens.FirstOrDefault();
                        diffOfDates = DateTime.Now - TokenData.EditDate;
                        if (diffOfDates.Value.Hours >= 1)
                        {
                            HomeController.GetAuthTokensUsingRefreshTokenAsync();
                        }
                    } while (diffOfDates.Value.Hours >= 1);

                    AccessToken = TokenData.AccessToken;

                    using (HttpClient client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TokenData.AccessToken);
                        client.DefaultRequestHeaders.Accept.Clear();
                        client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                        // Make the GET request
                        var Response = client.GetAsync("https://sandbox-quickbooks.api.intuit.com/v3/company/" + TokenData.realmId + "/account/" + SyncLog.QBId + "?minorversion=23");
                        Response.Wait();
                        var response = Response.Result;

                        if (response.IsSuccessStatusCode)
                        {
                            var readTask = response.Content.ReadAsStringAsync();
                            readTask.Wait();
                            string Test = readTask.Result;
                            QBAccount.AccountResponse ResponseData = Newtonsoft.Json.JsonConvert.DeserializeObject<QBAccount.AccountResponse>(Test);


                            int QBId = Convert.ToInt32(ResponseData.Account.Id);

                            Data = DB.tblAccounts.Where(x => x.QBId == QBId).FirstOrDefault();
                            if (Data != null)
                            {
                               
                                Data.QBId = Convert.ToInt32(ResponseData.Account.Id);
                                Data.SyncToken = ResponseData.Account.SyncToken;
                                //Data.Code = item.AcctNum;

                                Data.Name = ResponseData.Account.Name;
                                if (ResponseData.Account.AccountType != null && ResponseData.Account.AccountType.ToString().ToLower() == "expense")
                                {
                                    Data.Type = "Expense Account";
                                }
                                if (ResponseData.Account.AccountType != null && ResponseData.Account.AccountType.ToString().ToLower() == "income")
                                {
                                    Data.Type = "Income Account";
                                }

                                Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Data.isActive = true;
                                Data.isDelete = false;
                                DB.Entry(Data);
                                DB.SaveChanges();


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Id = Convert.ToInt32(Data.AccountId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();
                            }
                            else
                            {
                                Data = new tblAccount();
                                Data.QBId = Convert.ToInt32(ResponseData.Account.Id);
                                Data.SyncToken = ResponseData.Account.SyncToken;
                                //Data.Code = item.AcctNum;

                                Data.Name = ResponseData.Account.Name;
                                if (ResponseData.Account.AccountType != null && ResponseData.Account.AccountType.ToString().ToLower() == "expense")
                                {
                                    Data.Type = "Expense Account";
                                }
                                if (ResponseData.Account.AccountType != null && ResponseData.Account.AccountType.ToString().ToLower() == "income")
                                {
                                    Data.Type = "Income Account";
                                }

                                Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                                Data.isActive = true;
                                Data.isDelete = false;
                                DB.tblAccounts.Add(Data);
                                DB.SaveChanges();


                                tblSyncLog SyncLogData = new tblSyncLog();
                                SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                                SyncLogData.Id = Convert.ToInt32(Data.AccountId);
                                SyncLogData.isSync = true;
                                SyncLogData.EditDate = DateTime.Now;
                                DB.Entry(SyncLogData);
                                DB.SaveChanges();

                            }
                        }
                        else
                        {
                            var errorMessage = response.Content.ReadAsStringAsync();
                            errorMessage.Wait();
                            var RR = errorMessage.Result;

                            tblSyncLog SyncLogData = new tblSyncLog();
                            SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                            SyncLogData.Message = RR;
                            SyncLogData.EditDate = DateTime.Now;
                            DB.Entry(SyncLogData);
                            DB.SaveChanges();
                        }
                    }
                }
                else if (SyncLog.Operation == "Delete")
                {
                    Data = DB.tblAccounts.Select(r => r).Where(x => x.QBId == SyncLog.QBId).FirstOrDefault();
                    if (Data != null)
                    {
                        Data.isDelete = true;
                        Data.EditDate = DateTime.Now;
                        DB.Entry(Data);
                        DB.SaveChanges();

                        tblSyncLog SyncLogData = new tblSyncLog();
                        SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                        SyncLogData.Id = Data.AccountId;
                        SyncLogData.isSync = true;
                        SyncLogData.EditDate = DateTime.Now;
                        DB.Entry(SyncLogData);
                        DB.SaveChanges();
                    }
                    else
                    {
                        tblSyncLog SyncLogData = new tblSyncLog();
                        SyncLogData = DB.tblSyncLogs.Where(x => x.SyncLogId == SyncLog.SyncLogId).FirstOrDefault();
                        SyncLogData.Message = "Object Not Found";
                        SyncLogData.isSync = true;
                        SyncLogData.EditDate = DateTime.Now;
                        DB.Entry(SyncLogData);
                        DB.SaveChanges();
                    }
                }
                return "Success";
            }
            catch (DbEntityValidationException dbEx)
            {
                string ErrorString = "";
                // Handle DbEntityValidationException
                foreach (var Vendor in dbEx.EntityValidationErrors)
                {
                    foreach (var Vendor1 in Vendor.ValidationErrors)
                    {
                        ErrorString += Vendor1.ErrorMessage + " ,";
                    }
                }
                Console.WriteLine($"DbEntityValidationException occurred: {dbEx.Message}");
                // Additional handling specific to DbEntityValidationException
                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = new StringContent(ErrorString);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }
            catch (Exception ex)
            {
                // Handle other exceptions
                Console.WriteLine($"An exception occurred: {ex.Message}");
                // Additional handling for generic exceptions

                var responseMessage = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                responseMessage.Content = ex.InnerException != null && ex.InnerException.InnerException != null ? new StringContent(ex.InnerException.InnerException.Message) : new StringContent(ex.Message);
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return "Error";
                //return ResponseMessage(responseMessage);
            }

        }


        [HttpPost]
        public async Task<IHttpActionResult> SyncData()
        {
            try
            {
                if (!Request.Method.Equals(HttpMethod.Post))
                {
                    return (IHttpActionResult)Request.CreateErrorResponse(HttpStatusCode.MethodNotAllowed, "Method Not Allowed");
                }

                //VerifySignature Test = new VerifySignature();
                Dictionary<string, string> headers = Request.Headers.ToDictionary(header => header.Key, header => header.Value.ToString());

                // Replace "yourSecretKeyHere" with your actual secret key
                string verifier = "b4f26f4f-72f2-435f-a00d-af9561b05494";

                string payload = await Request.Content.ReadAsStringAsync();
                // Call IsRequestValid function
                //bool isValid = Test.IsRequestValid(headers, payload, verifier);

                RootObject rootObject = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(payload);
                tblSyncLog Result = null;
                // Access the data
                

                foreach (var eventNotification in rootObject.EventNotifications)
                {
                    foreach (var entity in eventNotification.DataChangeEvent.Entities)
                    {
                        int ID= Convert.ToInt32(entity.Id);
                        
                        Result = DB.tblSyncLogs.Where(x => x.QBId == ID && x.Name == entity.Name).FirstOrDefault();
                        if (Result == null)
                        {
                            Result = new tblSyncLog();
                            Result.QBId = Convert.ToInt32(entity.Id);
                            Result.Name = entity.Name;
                            Result.Operation = entity.Operation;
                            Result.CreatedDate = entity.LastUpdated;
                            Result.isQB = true;
                            Result.isSync = false;
                            Result.Message = payload;
                            DB.tblSyncLogs.Add(Result);
                            DB.SaveChanges();
                        }
                        else
                        {
                            Result.Id = 0;
                            Result.QBId = Convert.ToInt32(entity.Id);
                            Result.Name = entity.Name;
                            Result.Operation = entity.Operation;
                            Result.CreatedDate = entity.LastUpdated;
                            Result.isQB = true;
                            Result.isSync = false;
                            Result.Message = payload;
                            DB.Entry(Result);
                            DB.SaveChanges();
                        }
                        SyncDataAPI(Result.SyncLogId);
                    }
                }

                //Result.CreatedDate = DateTime.Now;
               
                
                // Process the valid webhook request...
                return Ok("Webhook request is valid.");

                //if (isValid)
                //{

                //    tblSyncLog Result = new tblSyncLog();
                //    Result.Message = payload;
                //    Result.CreatedDate = DateTime.Now;
                //    DB.tblSyncLogs.Add(Result);
                //    DB.SaveChanges();
                //    // Process the valid webhook request...
                //    return Ok("Webhook request is valid.");
                //}
                //else
                //{
                //    tblSyncLog Result = new tblSyncLog();
                //    Result.Message = "Not Valid " + payload;
                //    Result.CreatedDate = DateTime.Now;
                //    DB.tblSyncLogs.Add(Result);
                //    DB.SaveChanges();
                //    // Handle invalid request...
                //    return BadRequest("Invalid webhook request.");
                //}
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
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
                tblLog Result = new tblLog();
                Result.Action = "Error: " + responseMessage;
                Result.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(Result);
                DB.SaveChanges();
                return ResponseMessage(responseMessage);
            }

        }

    }
}
