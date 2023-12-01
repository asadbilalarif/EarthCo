using EarthCo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class MonthlyLandsacpeController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public IHttpActionResult GetMonthlyLandsacpeList()
        {
            try
            {
                List<tblMonthlyLandsacpe> Data = new List<tblMonthlyLandsacpe>();
                Data = DB.tblMonthlyLandsacpes.ToList();
                if (Data == null || Data.Count == 0)
                {
                    return NotFound();
                }

                return Ok(Data);
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
        public IHttpActionResult GetMonthlyLandsacpe(int id=0,int CustomerId=0,int Year=0,int Month=0)
        {
            try
            {   
                SPGetMonthlyLandsacpeData_Result Result = new SPGetMonthlyLandsacpeData_Result();
                Result = DB.SPGetMonthlyLandsacpeData(id, CustomerId,Year,Month).FirstOrDefault();
                if (Result == null)
                {
                    //Data = new tblMonthlyLandsacpe();
                    //string userJson = JsonConvert.SerializeObject(Data);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    //responseMessage.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
                    return ResponseMessage(responseMessage);
                }
                MonthlyLanscapeClass Data = new MonthlyLanscapeClass();

                Data.MonthlyLandsacpeId = Result.MonthlyLandsacpeId;
                Data.CustomerId = Result.CustomerId;
                Data.ContactId = Result.ContactId;
                Data.ServiceLocationId = Result.ServiceLocationId;
                Data.RequestBy = Result.RequestBy;
                Data.SupervisorVisitedthejobweekly = (Result.SupervisorVisitedthejobweekly==true)?"Yes":"No";
                Data.CompletedLitterpickupofgroundareas = (Result.CompletedLitterpickupofgroundareas == true)?"Yes":"No";
                Data.Completedsweepingorblowingofwalkways = (Result.Completedsweepingorblowingofwalkways == true)?"Yes":"No";
                Data.HighpriorityareaswereVisitedweekly = (Result.HighpriorityareaswereVisitedweekly == true)?"Yes":"No";
                Data.VDitcheswerecleanedandinspected = (Result.VDitcheswerecleanedandinspected == true)?"Yes":"No";
                Data.WeepscreeninspectedandcleanedinrotationsectionId = Result.WeepscreeninspectedandcleanedinrotationsectionId;
                Data.Fertilizationoftrufoccoured = Result.Fertilizationoftrufoccoured;
                Data.Trufwasmovedandedgedweekly = (Result.Trufwasmovedandedgedweekly == true)?"Yes":"No";
                Data.Shrubstrimmedaccordingtorotationschedule = (Result.Shrubstrimmedaccordingtorotationschedule == true)?"Yes":"No";
                Data.FertilizationofShrubsoccoured = Result.FertilizationofShrubsoccoured;
                Data.WateringofflowerbedsCompletedandchecked = (Result.WateringofflowerbedsCompletedandchecked == true)?"Yes":"No";
                Data.Headswereadjustedformaximumcoverage = (Result.Headswereadjustedformaximumcoverage == true)?"Yes":"No";
                Data.Repairsweremadetomaintainaneffectivesystem = (Result.Repairsweremadetomaintainaneffectivesystem == true)?"Yes":"No";
                Data.Controllerswereinspectedandadjusted = (Result.Controllerswereinspectedandadjusted == true)?"Yes":"No";
                Data.Mainlinewasrepaired = (Result.Mainlinewasrepaired == true)?"Yes":"No";
                Data.Valvewasrepaired = (Result.Valvewasrepaired == true)?"Yes":"No";
                Data.Thismonthexpectedrotationschedule = Result.Thismonthexpectedrotationschedule;
                Data.Notes = Result.Notes;
                Data.CreatedBy = Result.CreatedBy;
                Data.CreatedDate = Result.CreatedDate;
                Data.EditBy = Result.EditBy;
                Data.EditDate = Result.EditDate;
                Data.isActive = Result.isActive;
                Data.isDelete = Result.isDelete;
                Data.CompanyName = Result.CompanyName;
                Data.RequestByName = Result.RequestByName;
                Data.ServiceLocationName = Result.ServiceLocationName;
                Data.Address = Result.Address;
                Data.Phone = Result.Phone;

                return Ok(Data);
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
        public IHttpActionResult AddMonthlyLandsacpe([FromBody] tblMonthlyLandsacpe MonthlyLandsacpe)
        {
            tblMonthlyLandsacpe Data = new tblMonthlyLandsacpe();
            try
            {
                var userIdClaim = User.Identity as ClaimsIdentity;
                int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);

                Data = DB.tblMonthlyLandsacpes.Where(x => x.CreatedDate.Month == DateTime.Now.Month && x.CreatedDate.Year == DateTime.Now.Year && x.CustomerId == MonthlyLandsacpe.CustomerId).FirstOrDefault();

                if (MonthlyLandsacpe.MonthlyLandsacpeId == 0 && Data==null)
                {
                    Data = MonthlyLandsacpe;
                    Data.CreatedDate = DateTime.Now;
                    Data.CreatedBy = userId;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = true;
                    Data.isDelete = false;

                    DB.tblMonthlyLandsacpes.Add(Data);
                    DB.SaveChanges();

                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Add Monthly Landsacpe",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    //return Ok("Monthly Landscape has been added successfully.");
                    return Ok(new { Id = Data.MonthlyLandsacpeId, Message = "Monthly Landscape has been added successfully." });
                }
                else
                {
                    // Updating an existing customer.
                    if(Data==null)
                    {
                        Data = DB.tblMonthlyLandsacpes.SingleOrDefault(c => c.MonthlyLandsacpeId == MonthlyLandsacpe.MonthlyLandsacpeId);
                    }
                    

                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }

                    Data.CustomerId = MonthlyLandsacpe.CustomerId;
                    Data.ContactId = MonthlyLandsacpe.ContactId;
                    Data.ServiceLocationId = MonthlyLandsacpe.ServiceLocationId;
                    Data.RequestBy = MonthlyLandsacpe.RequestBy;
                    Data.SupervisorVisitedthejobweekly = MonthlyLandsacpe.SupervisorVisitedthejobweekly;
                    Data.CompletedLitterpickupofgroundareas = MonthlyLandsacpe.CompletedLitterpickupofgroundareas;
                    Data.Completedsweepingorblowingofwalkways = MonthlyLandsacpe.Completedsweepingorblowingofwalkways;
                    Data.HighpriorityareaswereVisitedweekly = MonthlyLandsacpe.HighpriorityareaswereVisitedweekly;
                    Data.VDitcheswerecleanedandinspected = MonthlyLandsacpe.VDitcheswerecleanedandinspected;
                    Data.WeepscreeninspectedandcleanedinrotationsectionId = MonthlyLandsacpe.WeepscreeninspectedandcleanedinrotationsectionId;
                    Data.Fertilizationoftrufoccoured = MonthlyLandsacpe.Fertilizationoftrufoccoured;
                    Data.Trufwasmovedandedgedweekly = MonthlyLandsacpe.Trufwasmovedandedgedweekly;
                    Data.Shrubstrimmedaccordingtorotationschedule = MonthlyLandsacpe.Shrubstrimmedaccordingtorotationschedule;
                    Data.FertilizationofShrubsoccoured = MonthlyLandsacpe.FertilizationofShrubsoccoured;
                    Data.WateringofflowerbedsCompletedandchecked = MonthlyLandsacpe.WateringofflowerbedsCompletedandchecked;
                    Data.Headswereadjustedformaximumcoverage = MonthlyLandsacpe.Headswereadjustedformaximumcoverage;
                    Data.Repairsweremadetomaintainaneffectivesystem = MonthlyLandsacpe.Repairsweremadetomaintainaneffectivesystem;
                    Data.Controllerswereinspectedandadjusted = MonthlyLandsacpe.Controllerswereinspectedandadjusted;
                    Data.Mainlinewasrepaired = MonthlyLandsacpe.Mainlinewasrepaired;
                    Data.Valvewasrepaired = MonthlyLandsacpe.Valvewasrepaired;
                    Data.Thismonthexpectedrotationschedule = MonthlyLandsacpe.Thismonthexpectedrotationschedule;
                    Data.Notes = MonthlyLandsacpe.Notes;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = true;
                    Data.isDelete = false;

                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Update Monthly Landsacpe",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    //return Ok("Monthly Landscape has been updated successfully.");
                    return Ok(new { Id = Data.MonthlyLandsacpeId, Message = "Monthly Landscape has been updated successfully." });
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
        }

        [HttpGet]
        public IHttpActionResult DeleteMonthlyLandsacpe(int id)
        {
            tblMonthlyLandsacpe Data = new tblMonthlyLandsacpe();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            int CUserId = 2;
            try
            {
                Data = DB.tblMonthlyLandsacpes.Select(r => r).Where(x => x.MonthlyLandsacpeId == id).FirstOrDefault();

                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }


                DB.Entry(Data).State = EntityState.Deleted;
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = CUserId;
                LogData.Action = "Delete Monthly Landsacpe";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Monthly Landsacpe has been deleted successfully.");
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
