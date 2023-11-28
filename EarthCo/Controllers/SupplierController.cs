using EarthCo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SupplierController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        [HttpGet]
        public IHttpActionResult GetSearchSupplierList(string Search)
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblUser> Data = new List<tblUser>();
                Data = DB.tblUsers.Where(x => x.UserTypeId == 3 && x.isDelete != true && (x.FirstName.ToLower().Contains(Search.ToLower())|| x.LastName.ToLower().Contains(Search.ToLower()))).Take(10).ToList();
                //Data = DB.tblUsers.Where(x => x.UserTypeId == 2 && x.isDelete != true).ToList();

                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
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

        [HttpGet]
        public IHttpActionResult GetSupplierServerSideList(int DisplayStart = 0, int DisplayLength = 10)
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblUser> Data = new List<tblUser>();
                List<GetSupplierList> SupplierData = new List<GetSupplierList>();

                var totalRecords = DB.tblUsers.Count(x => x.UserTypeId == 3 && x.isDelete != true);
                DisplayStart = (DisplayStart - 1) * DisplayLength;
                Data = DB.tblUsers.Where(x => x.UserTypeId == 3 && x.isDelete != true).OrderBy(o => o.UserId).Skip(DisplayStart).Take(DisplayLength).ToList();

                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }
                foreach (var item in Data)
                {
                    GetSupplierList New = new GetSupplierList();
                    New.SupplierId = item.UserId;
                    New.SupplierName = item.FirstName+" "+item.LastName;
                    New.Address = item.Address;
                    New.Email = item.Email;
                    SupplierData.Add(New);
                }
                return Ok(new { totalRecords = totalRecords, Data = SupplierData }); // 200 - Successful response with data
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
        public IHttpActionResult GetSupplierList()
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblUser> Data = new List<tblUser>();
                List<GetSupplierList> SupplierData = new List<GetSupplierList>();
                Data = DB.tblUsers.Where(x => x.UserTypeId == 3 && x.isDelete != true).ToList();
                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }
                foreach (var item in Data)
                {
                    GetSupplierList New = new GetSupplierList();
                    New.SupplierId = item.UserId;
                    New.SupplierName = item.FirstName+" "+item.LastName;
                    New.Address = item.Address;
                    New.Email = item.Email;
                    SupplierData.Add(New);
                }
                return Ok(SupplierData); // 200 - Successful response with data
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
        public IHttpActionResult GetSupplier(int id)
        {
            try
            {
                tblUser Data = new tblUser();
                Data = DB.tblUsers.Where(x => x.UserId == id && x.isDelete != true).FirstOrDefault();
                Data.Password = "";
                if (Data == null)
                {
                    Data = new tblUser();
                    //Data.tblContacts = null;
                    Data.tblServiceLocations = null;
                    string userJson = JsonConvert.SerializeObject(Data);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    responseMessage.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
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
        public IHttpActionResult AddSupplier([FromBody] tblUser Customer)
        {
            try
            {
                //var userIdClaim = User.Identity as ClaimsIdentity;
                //int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                int userId = 2; // Replace with your authentication mechanism to get the user's ID.

                tblUser Data = new tblUser();

                if (Customer.UserId == 0)
                {
                    // Creating a new customer.
                    if (DB.tblUsers.Select(r => r).Where(x => x.Email == Customer.Email && Customer.isDelete != true).FirstOrDefault() != null)
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("Supplier with same Email already exsist!!!");
                        return ResponseMessage(responseMessage);
                    }

                    Data = Customer;
                    Data.UserTypeId = 3;
                    Data.CreatedDate = DateTime.Now;
                    Data.CreatedBy = userId;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = true;

                    DB.tblUsers.Add(Data);
                    DB.SaveChanges();

                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Add Supplier",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    return Ok(new { Id = Data.UserId, Message = "Supplier has been added successfully." });
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
                        responseMessage.Content = new StringContent("Supplier with same Email already exsist!!!");
                        return ResponseMessage(responseMessage);
                    }
                    Data.FirstName = Customer.FirstName;
                    Data.LastName = Customer.LastName;
                    Data.Address = Customer.Address;
                    Data.City = Customer.City;
                    Data.State = Customer.State;
                    Data.Country = Customer.Country;
                    Data.Code = Customer.Code;
                    Data.UserTypeId = 3;
                    Data.Phone = Customer.Phone;
                    Data.AltPhone = Customer.AltPhone;
                    Data.username = Customer.username;
                    Data.Email = Customer.Email;
                    Data.RoleId = Customer.RoleId;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = true;

                    DB.Entry(Data);
                    DB.SaveChanges();


                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Update Supplier",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();


                    return Ok(new { Id = Data.UserId, Message = "Supplier has been updated successfully." });
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
        public IHttpActionResult DeleteSupplier(int id)
        {
            try
            {
                int userId = 2; // Replace with your authentication mechanism to get the user's ID.


                tblUser Data = DB.tblUsers.FirstOrDefault(c => c.UserId == id);

                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }


                // Remove the customer
                Data.isDelete = true;
                DB.Entry(Data);
                DB.SaveChanges();

                // Log the action
                var logData = new tblLog
                {
                    UserId = userId,
                    Action = "Delete Supplier",
                    CreatedDate = DateTime.Now
                };

                DB.tblLogs.Add(logData);
                DB.SaveChanges();

                return Ok("Supplier has been deleted successfully.");
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
