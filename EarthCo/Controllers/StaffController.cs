using EarthCo.Models;
//using Newtonsoft.Json;
using System.Text.Json;
using System;
using System.Collections.Generic;
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
    public class StaffController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();
        [HttpGet]
        public IHttpActionResult GetStaffServerSideList(string Search,int DisplayStart = 0, int DisplayLength = 10, bool isAscending = false)
        {
            try
            {
                //DB.Configuration.ProxyCreationEnabled = false;
                List<tblUser> Data = new List<tblUser>();
                List<StaffList> StaffData = new List<StaffList>();

                var totalRecords = DB.tblUsers.Count(x => x.UserTypeId == 1 && x.isDelete != true);
                DisplayStart = (DisplayStart - 1) * DisplayLength;
                if (Search == null)
                {
                    Search = "\"\"";
                }
                Search = JsonSerializer.Deserialize<string>(Search);
                if (!string.IsNullOrEmpty(Search) && Search != "")
                {
                    Data = DB.tblUsers.Where(x => x.FirstName.ToLower().Contains(Search.ToLower())
                                                  || x.LastName.ToLower().Contains(Search.ToLower())
                                                  || x.username.ToString().ToLower().Contains(Search.ToLower())
                                                  || x.tblRole.Role.ToLower().Contains(Search.ToLower())).ToList();
                    totalRecords = Data.Count(x => x.UserTypeId == 1 && x.isDelete != true);
                    Data = Data.Where(x => x.UserTypeId == 1 && x.isDelete != true).OrderBy(o => isAscending? o.UserId:-o.UserId).Skip(DisplayStart).Take(DisplayLength).ToList();
                }
                else
                {
                    Data = DB.tblUsers.Where(x => x.UserTypeId == 1 && x.isDelete != true).OrderBy(o => isAscending ? o.UserId : -o.UserId).Skip(DisplayStart).Take(DisplayLength).ToList();
                }

                

                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }
                foreach (var item in Data)
                {
                    StaffList New = new StaffList();
                    New.UserId = item.UserId;
                    New.FirstName = item.FirstName;
                    New.LastName = item.LastName;
                    New.Email = item.Email;
                    New.Role = item.tblRole.Role;
                    StaffData.Add(New);
                }


                return Ok(new { totalRecords = totalRecords, Data = StaffData }); // 200 - Successful response with data
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
        public IHttpActionResult GetStaffList()
        {
            try
            {
                //DB.Configuration.ProxyCreationEnabled = false;
                List<tblUser> Data = new List<tblUser>();
                List<StaffList> StaffData = new List<StaffList>();
                Data = DB.tblUsers.Where(x => x.UserTypeId == 1 && x.isDelete != true).ToList();
                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }
                foreach (var item in Data)
                {
                    StaffList New = new StaffList();
                    New.UserId = item.UserId;
                    New.FirstName = item.FirstName;
                    New.LastName = item.LastName;
                    New.Email = item.Email;
                    New.Role = item.tblRole.Role;
                    StaffData.Add(New);
                }


                return Ok(StaffData); // 200 - Successful response with data
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
        public IHttpActionResult GetStaff(int id)
        {
            try
            {
                //tblUser Data = new tblUser();
                //Data = DB.tblUsers.Where(x => x.UserId == id && x.isDelete != true).FirstOrDefault();


                SPGetStaffData_Result Data = new SPGetStaffData_Result();
                Data = DB.SPGetStaffData(id).FirstOrDefault();


                GetStaffData GetData = new GetStaffData();
                if (Data == null)
                {
                    GetStaffData newData = new GetStaffData();
                    string userJson = Newtonsoft.Json.JsonConvert.SerializeObject(newData);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    responseMessage.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
                    return ResponseMessage(responseMessage);
                }
                else
                {
                    Data.Password = "";
                    GetData.Data = Data;
                    GetData.Data.Password = "";
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
        public IHttpActionResult AddStaff([FromBody] tblUser Customer)
        {
            try
            {
                //var userIdClaim = User.Identity as ClaimsIdentity;
                //int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                var userIdClaim = User.Identity as ClaimsIdentity;
                int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);

                tblUser Data = new tblUser();

                if (Customer.UserId == 0)
                {
                    // Creating a new customer.
                    if (DB.tblUsers.Select(r => r).Where(x => x.Email == Customer.Email && Customer.isDelete != true).FirstOrDefault() != null)
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("Staff with same Email already exsist!!!");
                        return ResponseMessage(responseMessage);
                    }

                    Data = Customer;
                    Data.RoleId = 4;
                    Data.UserTypeId = 1;
                    Data.CreatedDate = DateTime.Now;
                    Data.CreatedBy = userId;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    Data.isLoginAllow = true;

                    byte[] EncDataBtye = new byte[Customer.Password.Length];
                    EncDataBtye = System.Text.Encoding.UTF8.GetBytes(Customer.Password);
                    Data.Password = Convert.ToBase64String(EncDataBtye);

                    DB.tblUsers.Add(Data);
                    DB.SaveChanges();


                    tblSyncLog Result = new tblSyncLog();
                    Result.Id = Data.UserId;
                    Result.Name = "Employee";
                    Result.Operation = "Create";
                    Result.CreatedDate = DateTime.Now;
                    Result.isQB = false;
                    Result.isSync = false;
                    DB.tblSyncLogs.Add(Result);
                    DB.SaveChanges();
                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Add Staff",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    return Ok(new { Id = Data.UserId, SyncId = Result.SyncLogId, Message = "Staff has been added successfully." });
                }
                else
                {
                    // Updating an existing customer.
                    Data = DB.tblUsers.SingleOrDefault(c => c.UserId == Customer.UserId);

                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }
                    tblUser CheckUser = DB.tblUsers.Select(r => r).Where(x => x.Email == Customer.Email && Customer.isDelete != true).FirstOrDefault();

                    if (CheckUser != null && CheckUser.UserId != Customer.UserId)
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("Staff with same Email already exsist!!!");
                        return ResponseMessage(responseMessage);
                    }
                    Data.FirstName = Customer.FirstName;
                    Data.LastName = Customer.LastName;
                    Data.Address = Customer.Address;
                    Data.City = Customer.City;
                    Data.State = Customer.State;
                    Data.Country = Customer.Country;
                    Data.Code = Customer.Code;
                    Data.UserTypeId = 1;
                    Data.Phone = Customer.Phone;
                    Data.AltPhone = Customer.AltPhone;
                    Data.username = Customer.username;
                    Data.Email = Customer.Email;
                    Data.RoleId = Customer.RoleId;
                    Data.isLoginAllow = true;
                    Data.isActive = true;
                    Data.isDelete = false;
                    if (Customer.Password != null && Customer.Password != "")
                    {
                        byte[] EncDataBtye = new byte[Customer.Password.Length];
                        EncDataBtye = System.Text.Encoding.UTF8.GetBytes(Customer.Password);
                        Data.Password = Convert.ToBase64String(EncDataBtye);
                    }
                    else
                    {
                        Data.Password = Data.Password;
                    }


                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;

                    DB.Entry(Data);
                    DB.SaveChanges();

                    tblSyncLog Result = new tblSyncLog();
                    Result = DB.tblSyncLogs.Where(x => x.Id == Data.UserId && x.Name == "Employee").FirstOrDefault();
                    if (Result == null)
                    {
                        Result = new tblSyncLog();
                        Result.Id = Data.UserId;
                        Result.Name = "Employee";
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
                        Result.Id = Data.UserId;
                        Result.Name = "Employee";
                        Result.Operation = "Update";
                        Result.EditDate = DateTime.Now;
                        Result.isQB = false;
                        Result.isSync = false;
                        DB.Entry(Result);
                        DB.SaveChanges();
                    }
                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Update Staff",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();


                    return Ok(new { Id = Data.UserId, SyncId = Result.SyncLogId, Message = "Staff has been updated successfully." });
                    //return Ok("Customer has been updated successfully.");
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
        public IHttpActionResult DeleteStaff(int id)
        {
            try
            {
                var userIdClaim = User.Identity as ClaimsIdentity;
                int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);


                tblUser Data = DB.tblUsers.FirstOrDefault(c => c.UserId == id);

                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }


                // Remove the customer
                Data.isDelete = true;
                Data.EditBy = userId;
                Data.EditDate = DateTime.Now;
                DB.Entry(Data);
                DB.SaveChanges();


                tblSyncLog Result = new tblSyncLog();
                Result = DB.tblSyncLogs.Where(x => x.Id == Data.UserId && x.Name == "Employee").FirstOrDefault();
                if (Result == null)
                {
                    Result = new tblSyncLog();
                    Result.Id = Data.UserId;
                    Result.Name = "Employee";
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
                    Result.Id = Data.UserId;
                    Result.Name = "Employee";
                    Result.Operation = "Delete";
                    Result.EditDate = DateTime.Now;
                    Result.isQB = false;
                    Result.isSync = false;
                    DB.Entry(Result);
                    DB.SaveChanges();
                }
                // Log the action
                var logData = new tblLog
                {
                    UserId = userId,
                    Action = "Delete Staff",
                    CreatedDate = DateTime.Now
                };

                DB.tblLogs.Add(logData);
                DB.SaveChanges();

                return Ok("Staff has been deleted successfully.");
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
