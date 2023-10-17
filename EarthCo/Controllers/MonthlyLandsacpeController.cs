using EarthCo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
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
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetMonthlyLandsacpe(int id)
        {
            try
            {
                tblMonthlyLandsacpe Data = new tblMonthlyLandsacpe();
                Data = DB.tblMonthlyLandsacpes.Where(x => x.MonthlyLandsacpeId == id).FirstOrDefault();
                if (Data == null)
                {
                    Data = new tblMonthlyLandsacpe(); ;
                    string userJson = JsonConvert.SerializeObject(Data);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    responseMessage.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
                    return ResponseMessage(responseMessage);
                }

                return Ok(Data);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }

        [HttpPost]
        public IHttpActionResult AddMonthlyLandsacpe([FromBody] tblMonthlyLandsacpe MonthlyLandsacpe)
        {
            tblMonthlyLandsacpe Data = new tblMonthlyLandsacpe();
            try
            {
                //var userIdClaim = User.Identity as ClaimsIdentity;
                //int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                int userId = 2; // Replace with your authentication mechanism to get the user's ID.

                if (MonthlyLandsacpe.MonthlyLandsacpeId == 0)
                {
                    Data = MonthlyLandsacpe;
                    Data.CreatedDate = DateTime.Now;
                    Data.CreatedBy = userId;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = MonthlyLandsacpe.isActive;

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

                    return Ok("Monthly Landsacpe has been added successfully.");
                }
                else
                {
                    // Updating an existing customer.
                    Data = DB.tblMonthlyLandsacpes.SingleOrDefault(c => c.MonthlyLandsacpeId == MonthlyLandsacpe.MonthlyLandsacpeId);

                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }

                    Data.CustomerId = MonthlyLandsacpe.CustomerId;
                    Data.ContactId = MonthlyLandsacpe.ContactId;
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
                    Data.isActive = MonthlyLandsacpe.isActive;

                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Update Monthly Landsacpe",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    return Ok("Monthly Landsacpe has been updated successfully.");
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return InternalServerError(ex); // 500 - Internal Server Error
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
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
