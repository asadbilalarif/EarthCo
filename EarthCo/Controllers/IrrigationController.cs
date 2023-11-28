using EarthCo.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
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
    public class IrrigationController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public IHttpActionResult GetIrrigationServerSideList(int DisplayStart = 0, int DisplayLength = 10)
        {
            try
            {

                List<tblIrrigation> Data = new List<tblIrrigation>();
                List<IrrigationList> Result = new List<IrrigationList>();

                var totalRecords = DB.tblIrrigations.Count(x => !x.isDelete);
                DisplayStart = (DisplayStart - 1) * DisplayLength;
                Data = DB.tblIrrigations.Where(x => !x.isDelete).OrderBy(o => o.IrrigationId).Skip(DisplayStart).Take(DisplayLength).ToList();

                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }
                else
                {
                    foreach (tblIrrigation item in Data)
                    {
                        IrrigationList Temp = new IrrigationList();
                        Temp.IrrigationId = item.IrrigationId;
                        Temp.CustomerName = item.tblUser.FirstName+" "+item.tblUser.LastName;
                        Temp.CreatedDate = item.CreatedDate;
                        Temp.ControllerNumbers = item.tblControllers.Select(s=>s.SerialNumber).ToList();
                        Result.Add(Temp);
                    }
                }
                return Ok(new { totalRecords = totalRecords, Data = Result }); // 200 - Successful response with data
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
        public IHttpActionResult GetIrrigationList()
        {
            try
            {

                List<tblIrrigation> Data = new List<tblIrrigation>();
                List<IrrigationList> Result = new List<IrrigationList>();
                Data = DB.tblIrrigations.Where(x => x.isDelete != true).ToList();

                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }
                else
                {
                    foreach (tblIrrigation item in Data)
                    {
                        IrrigationList Temp = new IrrigationList();
                        Temp.IrrigationId = item.IrrigationId;
                        Temp.CustomerName = item.tblUser.FirstName+" "+item.tblUser.LastName;
                        Temp.CreatedDate = item.CreatedDate;
                        Temp.ControllerNumbers = item.tblControllers.Select(s=>s.SerialNumber).ToList();
                        Result.Add(Temp);
                    }
                }
                return Ok(Result);
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
        public IHttpActionResult GetIrrigation(int id)
        {
            try
            {

                SPGetIrrigationData_Result Data = new SPGetIrrigationData_Result();
                List<SPGetIrrigationControllerData_Result> ControllerData = new List<SPGetIrrigationControllerData_Result>();
                Data = DB.SPGetIrrigationData(id).FirstOrDefault();
                ControllerData = DB.SPGetIrrigationControllerData(id).ToList();

                GetIrrigationData GetData = new GetIrrigationData();

                if (Data == null)
                {
                    //DB.Configuration.ProxyCreationEnabled = false;
                    //Data = new tblInvoice();
                    //Data.tblInvoiceItems = null;
                    //Data.tblInvoices = null;
                    //Data.tblEstimate = null;
                    //Data.tblInvoices = null;
                    //Data.tblInvoiceFiles = null;
                    //Data.tblUser = null;
                    //string userJson = JsonConvert.SerializeObject(Data);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    //responseMessage.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
                    return ResponseMessage(responseMessage);
                }
                else
                {
                    GetData.IrrigationData = Data;
                    GetData.ControllerData = ControllerData;
                }

                return Ok(GetData);
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
        public IHttpActionResult AddIrrigation([FromBody] tblIrrigation ParaData)
        {
            tblIrrigation Data = new tblIrrigation();
            tblIrrigation CheckData = new tblIrrigation();
            try
            {
                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);
                var userIdClaim = User.Identity as ClaimsIdentity;
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                if (ParaData.IrrigationId == 0)
                {

                    CheckData = DB.tblIrrigations.Where(x => x.IrrigationNumber == ParaData.IrrigationNumber && x.IrrigationNumber != null && x.IrrigationNumber != "").FirstOrDefault();

                    if (CheckData != null)
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("Irrigation number already exsist.");
                        return ResponseMessage(responseMessage);
                    }

                    Data = ParaData;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    Data.DocNumber = Convert.ToString(DB.SPGetNumber("IR").FirstOrDefault());
                    if (Data.IrrigationNumber == null || Data.IrrigationNumber == "")
                    {
                        Data.IrrigationNumber = Data.DocNumber;
                    }
                    DB.tblIrrigations.Add(Data);
                    DB.SaveChanges();

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Irrigation";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    return Ok(new { Id = Data.IrrigationId, Message = "Irrigation has been added successfully." });
                }
                else
                {
                    Data = DB.tblIrrigations.Select(r => r).Where(x => x.IrrigationId == ParaData.IrrigationId).FirstOrDefault();
                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }


                    CheckData = DB.tblIrrigations.Where(x => x.IrrigationNumber == ParaData.IrrigationNumber && x.IrrigationNumber != null && x.IrrigationNumber != "").FirstOrDefault();
                    if (CheckData != null && CheckData.IrrigationId != Data.IrrigationId)
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("Irrigation number already exsist.");
                        return ResponseMessage(responseMessage);
                    }

                    Data.IrrigationNumber = ParaData.IrrigationNumber;
                    Data.CustomerId = ParaData.CustomerId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    if (ParaData.IrrigationNumber == null || ParaData.IrrigationNumber == "")
                    {
                        Data.IrrigationNumber = Data.DocNumber;
                    }
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Irrigation";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    return Ok(new { Id = Data.IrrigationId, Message = "Irrigation has been Update successfully." });
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

        [HttpPost]
        public IHttpActionResult AddController()
        {
            try
            {
                var Data1 = HttpContext.Current.Request.Params.Get("IrrigationControllerData");
                HttpPostedFile file = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;

                ControllerClass ParaData = new ControllerClass();
                //Punchlist.Files = new HttpPostedFile();
                ParaData = JsonSerializer.Deserialize<ControllerClass>(Data1);
                if (HttpContext.Current.Request.Files["Photo"] != null)
                {
                    ParaData.Photo = HttpContext.Current.Request.Files["Photo"];
                }
                if (HttpContext.Current.Request.Files["ControllerPhoto"] != null)
                {
                    ParaData.ControllerPhoto = HttpContext.Current.Request.Files["ControllerPhoto"];
                }





                //HttpCookie cookieObj = Request.Cookies["User"];
                //int UserId = Int32.Parse(cookieObj["UserId"]);
                //int RoleId = Int32.Parse(cookieObj["RoleId"]);
                var userIdClaim = User.Identity as ClaimsIdentity;
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                if (ParaData.ControllerId == 0)
                {
                    tblController ConData = null;
                        ConData = new tblController();
                        ConData.MakeAndModel = ParaData.MakeAndModel;
                        ConData.SerialNumber = ParaData.SerialNumber;
                        ConData.LoacationClosestAddress = ParaData.LoacationClosestAddress;
                        ConData.isSatelliteBased = ParaData.isSatelliteBased;
                        ConData.TypeofWater = ParaData.TypeofWater;
                        ConData.MeterNumber = ParaData.MeterNumber;
                        ConData.MeterSize = ParaData.MeterSize;
                        ConData.NumberofStation = ParaData.NumberofStation;
                        ConData.NumberofValves = ParaData.NumberofValves;
                        ConData.NumberofBrokenMainLines = ParaData.NumberofBrokenMainLines;
                        ConData.TypeofValves = ParaData.TypeofValves;
                        ConData.LeakingValves = ParaData.LeakingValves;
                        ConData.MalfunctioningValves = ParaData.MalfunctioningValves;
                        ConData.NumberofBrokenLateralLines = ParaData.NumberofBrokenLateralLines;
                        ConData.NumberofBrokenHeads = ParaData.NumberofBrokenHeads;
                        ConData.RepairsMade = ParaData.RepairsMade;
                        ConData.UpgradesMade = ParaData.UpgradesMade;
                        ConData.IrrigationId = (int)ParaData.IrrigationId;
                        ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        ConData.CreatedBy = UserId;
                        ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                        ConData.EditBy = UserId;
                        ConData.isActive = true;
                        ConData.isDelete = false;

                        string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading/Irrigation"));
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        if (ParaData.ControllerPhoto != null)
                        {
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/Irrigation"), Path.GetFileName("ControllerPhoto" + ParaData.IrrigationId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(ParaData.ControllerPhoto.FileName)));
                            ParaData.ControllerPhoto.SaveAs(path);
                            path = Path.Combine("\\Uploading\\Irrigation", Path.GetFileName("ControllerPhoto" + ParaData.IrrigationId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(ParaData.ControllerPhoto.FileName)));
                            ConData.ControllerPhotoPath = path;
                        }
                        if (ParaData.Photo != null)
                        {
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/Irrigation"), Path.GetFileName("Photo" + ParaData.IrrigationId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(ParaData.Photo.FileName)));
                            ParaData.Photo.SaveAs(path);
                            path = Path.Combine("\\Uploading\\Irrigation", Path.GetFileName("Photo" + ParaData.IrrigationId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(ParaData.Photo.FileName)));
                            ConData.PhotoPath = path;
                        }

                        DB.tblControllers.Add(ConData);
                        DB.SaveChanges();

                        tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Controller";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    return Ok(new { Id = ConData.ControllerId, Message = "Controller has been added successfully." });
                }
                else
                {
                    tblController ConData = new tblController();
                    ConData = DB.tblControllers.Select(r => r).Where(x => x.ControllerId == ParaData.ControllerId).FirstOrDefault();
                    if (ConData == null)
                    {
                        return NotFound(); // Customer not found.
                    }


                    
                    ConData.MakeAndModel = ParaData.MakeAndModel;
                    ConData.SerialNumber = ParaData.SerialNumber;
                    ConData.isSatelliteBased = ParaData.isSatelliteBased;
                    ConData.TypeofWater = ParaData.TypeofWater;
                    ConData.MeterNumber = ParaData.MeterNumber;
                    ConData.MeterSize = ParaData.MeterSize;
                    ConData.NumberofStation = ParaData.NumberofStation;
                    ConData.NumberofValves = ParaData.NumberofValves;
                    ConData.NumberofBrokenMainLines = ParaData.NumberofBrokenMainLines;
                    ConData.TypeofValves = ParaData.TypeofValves;
                    ConData.LeakingValves = ParaData.LeakingValves;
                    ConData.MalfunctioningValves = ParaData.MalfunctioningValves;
                    ConData.NumberofBrokenLateralLines = ParaData.NumberofBrokenLateralLines;
                    ConData.NumberofBrokenHeads = ParaData.NumberofBrokenHeads;
                    ConData.RepairsMade = ParaData.RepairsMade;
                    ConData.UpgradesMade = ParaData.UpgradesMade;
                    ConData.IrrigationId = (int)ParaData.IrrigationId;
                    ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    ConData.CreatedBy = UserId;
                    ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    ConData.EditBy = UserId;
                    ConData.isActive = true;
                    ConData.isDelete = false;

                    string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading/Irrigation"));
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    if (ParaData.ControllerPhoto != null)
                    {
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/Irrigation"), Path.GetFileName("ControllerPhoto" + ParaData.IrrigationId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(ParaData.ControllerPhoto.FileName)));
                        ParaData.ControllerPhoto.SaveAs(path);
                        path = Path.Combine("\\Uploading\\Irrigation", Path.GetFileName("ControllerPhoto" + ParaData.IrrigationId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(ParaData.ControllerPhoto.FileName)));
                        ConData.ControllerPhotoPath = path;
                    }
                    if (ParaData.Photo != null)
                    {
                        string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/Irrigation"), Path.GetFileName("Photo" + ParaData.IrrigationId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(ParaData.Photo.FileName)));
                        ParaData.Photo.SaveAs(path);
                        path = Path.Combine("\\Uploading\\Irrigation", Path.GetFileName("Photo" + ParaData.IrrigationId.ToString() + DateTime.Now.ToString("dd MM yyyy mm ss") + Path.GetExtension(ParaData.Photo.FileName)));
                        ConData.PhotoPath = path;
                    }

                    DB.Entry(ConData);
                    DB.SaveChanges();

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Controller";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    return Ok(new { Id = ConData.ControllerId, Message = "Controller has been Update successfully." });
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
        public IHttpActionResult DeleteController(int id)
        {
            tblController Data = new tblController();
            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            //int CUserId = 2;
            try
            {
                Data = DB.tblControllers.Select(r => r).Where(x => x.ControllerId == id).FirstOrDefault();

                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }

                //List<tblController> ConList = DB.tblControllers.Where(x => x.IrrigationId == id).ToList();
                //if (ConList != null && ConList.Count != 0)
                //{
                //    DB.tblControllers.RemoveRange(ConList);
                //    DB.SaveChanges();
                //}



                Data.isDelete = true;
                Data.EditBy = UserId;
                Data.EditDate = DateTime.Now;
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Delete Controller";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Controller has been deleted successfully.");
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
        public IHttpActionResult DeleteIrrigation(int id)
        {
            tblIrrigation Data = new tblIrrigation();
            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            //int CUserId = 2;
            try
            {
                Data = DB.tblIrrigations.Select(r => r).Where(x => x.IrrigationId == id).FirstOrDefault();

                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }

                //List<tblController> ConList = DB.tblControllers.Where(x => x.IrrigationId == id).ToList();
                //if (ConList != null && ConList.Count != 0)
                //{
                //    DB.tblControllers.RemoveRange(ConList);
                //    DB.SaveChanges();
                //}



                Data.isDelete = true;
                Data.EditBy = UserId;
                Data.EditDate = DateTime.Now;
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Delete Irrigation";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Irrigation has been deleted successfully.");
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
