using EarthCo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using static EarthCo.Models.SyncQB;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SyncQBController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();
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

                RootObject rootObject = JsonConvert.DeserializeObject<RootObject>(payload);
                tblSyncLog Result = null;
                // Access the data
                foreach (var eventNotification in rootObject.EventNotifications)
                {
                    foreach (var entity in eventNotification.DataChangeEvent.Entities)
                    {
                        Result = new tblSyncLog();
                        Result.QBId =Convert.ToInt32(entity.Id);
                        Result.Name = entity.Name;
                        Result.Operation = entity.Operation;
                        Result.CreatedDate = entity.LastUpdated;
                        Result.isQB = true;
                        Result.isSync = false;
                        Result.Message = payload;
                        DB.tblSyncLogs.Add(Result);
                        DB.SaveChanges();
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
