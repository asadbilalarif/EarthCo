using EarthCo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
        public IHttpActionResult GetSupplierList()
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblUser> Data = new List<tblUser>();
                Data = DB.tblUsers.Where(x => x.UserTypeId == 3 && x.isDelete != true).ToList();
                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }

                return Ok(Data); // 200 - Successful response with data
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        public IHttpActionResult GetSupplier(int id)
        {
            try
            {
                tblUser Data = new tblUser();
                Data = DB.tblUsers.Where(x => x.UserId == id && x.isDelete != true).FirstOrDefault();
                if (Data == null)
                {
                    Data = new tblUser();
                    Data.tblContacts = null;
                    Data.tblServiceLocations = null;
                    string userJson = JsonConvert.SerializeObject(Data);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    responseMessage.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
                    return ResponseMessage(responseMessage);
                }

                return Ok(Data); // 200 - Successful response with data
            }
            catch (Exception ex)
            {
                // Log the exception
                // You may also choose to return a more specific error response (e.g., 500 - Internal Server Error) here.
                return InternalServerError(ex);
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
                    Data.isActive = Customer.isActive;

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
                    Data.isActive = Customer.isActive;

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
            catch (Exception ex)
            {
                // Log the exception
                return InternalServerError(ex); // 500 - Internal Server Error
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
            catch (Exception ex)
            {
                // Log the exception
                return InternalServerError(ex); // 500 - Internal Server Error
            }
        }
    }
}
