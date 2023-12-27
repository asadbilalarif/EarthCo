using EarthCo.Models;
using System;
using System.Collections.Generic;
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
    public class PunchlistPhotoOnlyController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();
        [HttpGet]
        public IHttpActionResult GetPunchlistPhotoOnlyServerSideList(string Search, int DisplayStart = 0, int DisplayLength = 10, bool isAscending = false)
        {
            try
            {
                List<SPGetPunchlistPhotoOnlyList_Result> Data = new List<SPGetPunchlistPhotoOnlyList_Result>();

                var totalRecords = DB.tblPunchlists.Count(x => !x.isDelete);
                DisplayStart = (DisplayStart - 1) * DisplayLength;
                if (Search == null)
                {
                    Search = "\"\"";
                }
                Search = JsonSerializer.Deserialize<string>(Search);
                if (!string.IsNullOrEmpty(Search) && Search != "")
                {

                    Data = DB.SPGetPunchlistPhotoOnlyList().Where(x => x.CustomerName.ToLower().Contains(Search.ToLower())
                                                   || x.Notes.ToString().ToLower().Contains(Search.ToLower())).OrderBy(o => isAscending ? o.PunchlistPhotoOnlyId : -o.PunchlistPhotoOnlyId).Skip(DisplayStart).Take(DisplayLength).ToList();
                    totalRecords = DB.SPGetPunchlistPhotoOnlyList().Where(x => (x.isDelete != true) && x.CustomerName.ToLower().Contains(Search.ToLower())
                                                   || x.Notes.ToString().ToLower().Contains(Search.ToLower())).OrderBy(o => isAscending ? o.PunchlistPhotoOnlyId : -o.PunchlistPhotoOnlyId).Skip(DisplayStart).Take(DisplayLength).Count();
                }
                else
                {
                    totalRecords = DB.SPGetPunchlistPhotoOnlyList().Count();
                    Data = DB.SPGetPunchlistPhotoOnlyList().OrderBy(o => isAscending ? o.PunchlistPhotoOnlyId : -o.PunchlistPhotoOnlyId).Skip(DisplayStart).Take(DisplayLength).ToList();
                }

                if (Data == null || Data.Count == 0)
                {
                    return NotFound();
                }
                return Ok(new { totalRecords = totalRecords, Data = Data }); // 200 - Successful response with data
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
        public IHttpActionResult GetPunchlistPhotoOnly(int id)
        {
            try
            {
                //DB.Configuration.ProxyCreationEnabled = false;
                SPGetPunchlistPhotoOnlyData_Result Data = new SPGetPunchlistPhotoOnlyData_Result();
                List<SPGetPunchlistPhotoOnlyFileData_Result> FileData = new List<SPGetPunchlistPhotoOnlyFileData_Result>();
                Data = DB.SPGetPunchlistPhotoOnlyData(id).FirstOrDefault();
                FileData = DB.SPGetPunchlistPhotoOnlyFileData(id).ToList();
                if (Data == null)
                {
                    return NotFound();
                }

                var Result = new
                {
                    Data = Data,
                    FileData = FileData
                };

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


        [HttpPost]
        public IHttpActionResult AddPunchlistPhotoOnly()
        {
            try
            {
                var userIdClaim = User.Identity as ClaimsIdentity;
                int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);

                var Data1 = HttpContext.Current.Request.Params.Get("PunchlistPhotoOnlyData");

                PunchlistPhotoOnlyFile PunchlistPhotoOnly = new PunchlistPhotoOnlyFile();
                PunchlistPhotoOnly.Photos = new List<HttpPostedFile>();
                PunchlistPhotoOnly.AdditionalPhotos = new List<HttpPostedFile>();

                HttpFileCollection files = HttpContext.Current.Request.Files;

                if (HttpContext.Current.Request.Files["AdditionalPhotos"] != null)
                {
                    PunchlistPhotoOnly.AdditionalPhotos = files.AllKeys.Where(key => key == "AdditionalPhotos").Select(key => files[key]).ToList();
                }
                if (HttpContext.Current.Request.Files["Photos"] != null)
                {
                    PunchlistPhotoOnly.Photos = files.AllKeys.Where(key => key == "Photos").Select(key => files[key]).ToList();
                }

                tblPunchlistPhotoOnly Data = new tblPunchlistPhotoOnly();
                List<tblPunchlistPhotoOnlyFile> FileData = new List<tblPunchlistPhotoOnlyFile>();

                PunchlistPhotoOnly.PunchlistPhotoOnlyData = JsonSerializer.Deserialize<tblPunchlistPhotoOnly>(Data1);


                if (PunchlistPhotoOnly.PunchlistPhotoOnlyData.PunchlistPhotoOnlyId == 0)
                {

                    Data = PunchlistPhotoOnly.PunchlistPhotoOnlyData;
                    Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.CreatedBy = UserId;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.tblPunchlistPhotoOnlies.Add(Data);
                    DB.SaveChanges();

                    string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading/PunchlistPhotoOnly"));
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    int Count = 1;
                    if (PunchlistPhotoOnly.Photos!= null && PunchlistPhotoOnly.Photos.Count != 0)
                    {
                        foreach (var item in PunchlistPhotoOnly.Photos)
                        {
                            tblPunchlistPhotoOnlyFile Temp = new tblPunchlistPhotoOnlyFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/PunchlistPhotoOnly"), Path.GetFileName("PunchlistPhotoOnlyFile" + Data.PunchlistPhotoOnlyId.ToString()+ Count + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading\\PunchlistPhotoOnly", Path.GetFileName("PunchlistPhotoOnlyFile" + Data.PunchlistPhotoOnlyId.ToString()+ Count + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            Temp.FilePath = path;
                            Temp.PunchlistPhotoOnlyId = Data.PunchlistPhotoOnlyId;
                            Temp.isAdditional = false;
                            Temp.CreatedBy = UserId;
                            Temp.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            Temp.EditBy = UserId;
                            Temp.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            Temp.isActive = true;
                            Temp.isDelete = false;
                            //FileData.Add(Temp);
                            DB.tblPunchlistPhotoOnlyFiles.Add(Temp);
                            Count++;
                        }
                        
                    }
                    if (PunchlistPhotoOnly.AdditionalPhotos != null && PunchlistPhotoOnly.AdditionalPhotos.Count != 0)
                    {
                        foreach (var item in PunchlistPhotoOnly.AdditionalPhotos)
                        {
                            tblPunchlistPhotoOnlyFile Temp = new tblPunchlistPhotoOnlyFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/PunchlistPhotoOnly"), Path.GetFileName("PunchlistPhotoOnlyFile" + Data.PunchlistPhotoOnlyId.ToString() + Count + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading\\PunchlistPhotoOnly", Path.GetFileName("PunchlistPhotoOnlyFile" + Data.PunchlistPhotoOnlyId.ToString() + Count + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            Temp.FilePath = path;
                            Temp.PunchlistPhotoOnlyId = Data.PunchlistPhotoOnlyId;
                            Temp.isAdditional = true;
                            Temp.CreatedBy = UserId;
                            Temp.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            Temp.EditBy = UserId;
                            Temp.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            Temp.isActive = true;
                            Temp.isDelete = false;
                            //FileData.Add(Temp);
                            DB.tblPunchlistPhotoOnlyFiles.Add(Temp);
                            Count++;
                        }
                    }
                    
                    DB.SaveChanges();


                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Add Punchlist Photo Only";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();
                    //return Ok("Punchlist has been added successfully.");
                    return Ok(new { Id = Data.PunchlistPhotoOnlyId, Message = "Punchlist photo only has been added successfully." });
                }
                else
                {
                    Data = DB.tblPunchlistPhotoOnlies.Select(r => r).Where(x => x.PunchlistPhotoOnlyId== PunchlistPhotoOnly.PunchlistPhotoOnlyData.PunchlistPhotoOnlyId).FirstOrDefault();

                    if (Data == null)
                    {
                        return NotFound();
                    }

                    Data.Notes = PunchlistPhotoOnly.PunchlistPhotoOnlyData.Notes;
                    Data.CustomerId = PunchlistPhotoOnly.PunchlistPhotoOnlyData.CustomerId;
                    Data.CustomerName = PunchlistPhotoOnly.PunchlistPhotoOnlyData.CustomerName;
                    Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    Data.EditBy = UserId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.Entry(Data);
                    DB.SaveChanges();

                    tblLog LogData = new tblLog();
                    LogData.UserId = UserId;
                    LogData.Action = "Update Punchlist photo only";
                    LogData.CreatedDate = DateTime.Now;
                    DB.tblLogs.Add(LogData);
                    DB.SaveChanges();

                    string folder = HttpContext.Current.Server.MapPath(string.Format("~/{0}/", "Uploading/PunchlistPhotoOnly"));
                    if (!Directory.Exists(folder))
                    {
                        Directory.CreateDirectory(folder);
                    }
                    int Count = 1;
                    if (PunchlistPhotoOnly.Photos != null && PunchlistPhotoOnly.Photos.Count != 0)
                    {
                        foreach (var item in PunchlistPhotoOnly.Photos)
                        {
                            tblPunchlistPhotoOnlyFile Temp = new tblPunchlistPhotoOnlyFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/PunchlistPhotoOnly"), Path.GetFileName("PunchlistPhotoOnlyFile" + Data.PunchlistPhotoOnlyId.ToString() + Count + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading\\PunchlistPhotoOnly", Path.GetFileName("PunchlistPhotoOnlyFile" + Data.PunchlistPhotoOnlyId.ToString() + Count + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            Temp.FilePath = path;
                            Temp.PunchlistPhotoOnlyId = Data.PunchlistPhotoOnlyId;
                            Temp.isAdditional = false;
                            Temp.CreatedBy = UserId;
                            Temp.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            Temp.EditBy = UserId;
                            Temp.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            Temp.isActive = true;
                            Temp.isDelete = false;
                            //FileData.Add(Temp);
                            DB.tblPunchlistPhotoOnlyFiles.Add(Temp);
                            Count++;
                        }

                    }
                    if (PunchlistPhotoOnly.AdditionalPhotos != null && PunchlistPhotoOnly.AdditionalPhotos.Count != 0)
                    {
                        foreach (var item in PunchlistPhotoOnly.AdditionalPhotos)
                        {
                            tblPunchlistPhotoOnlyFile Temp = new tblPunchlistPhotoOnlyFile();
                            string path = Path.Combine(HttpContext.Current.Server.MapPath("~/Uploading/PunchlistPhotoOnly"), Path.GetFileName("PunchlistPhotoOnlyFile" + Data.PunchlistPhotoOnlyId.ToString() + Count + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            item.SaveAs(path);
                            path = Path.Combine("\\Uploading\\PunchlistPhotoOnly", Path.GetFileName("PunchlistPhotoOnlyFile" + Data.PunchlistPhotoOnlyId.ToString() + Count + DateTime.Now.ToString("ddMMyyyyHHmmss") + Path.GetExtension(item.FileName)));
                            Temp.FilePath = path;
                            Temp.PunchlistPhotoOnlyId = Data.PunchlistPhotoOnlyId;
                            Temp.isAdditional = true;
                            Temp.CreatedBy = UserId;
                            Temp.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            Temp.EditBy = UserId;
                            Temp.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            Temp.isActive = true;
                            Temp.isDelete = false;
                            //FileData.Add(Temp);
                            DB.tblPunchlistPhotoOnlyFiles.Add(Temp);
                            Count++;
                        }
                    }
                    DB.SaveChanges();
                    //return Ok("Punchlist has been Update successfully.");
                    return Ok(new { Id = Data.PunchlistPhotoOnlyId, Message = "Punchlist photo only has been Update successfully." });
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
        public IHttpActionResult DeletePunchlistPhotoOnly(int id)
        {
            tblPunchlistPhotoOnly Data = new tblPunchlistPhotoOnly();
            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                Data = DB.tblPunchlistPhotoOnlies.Select(r => r).Where(x => x.PunchlistPhotoOnlyId == id).FirstOrDefault();
                if (Data == null)
                {
                    return NotFound();
                }

                Data.isDelete = true;
                Data.EditBy = UserId;
                Data.EditDate = DateTime.Now;
                DB.Entry(Data);
                DB.SaveChanges();

                tblLog LogData = new tblLog();
                LogData.UserId = UserId;
                LogData.Action = "Delete Punchlist photo only";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Punchlist photo only has been deleted successfully.");
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
        public IHttpActionResult DeletePunchlistPhotoOnlyFile(int FileId)
        {
            tblPunchlistPhotoOnlyFile Data = new tblPunchlistPhotoOnlyFile();

            var userIdClaim = User.Identity as ClaimsIdentity;
            int UserId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {
                Data = DB.tblPunchlistPhotoOnlyFiles.Select(r => r).Where(x => x.PunchlistPhotoOnlyFileId == FileId).FirstOrDefault();
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
                LogData.Action = "Delete Punchlist Photo Only File";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok(" Punchlist photo only file has been deleted successfully.");
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
