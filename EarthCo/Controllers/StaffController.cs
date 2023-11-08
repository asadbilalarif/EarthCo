using EarthCo.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
            catch (Exception ex)
            {
                return InternalServerError(ex);
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
                    string userJson = JsonConvert.SerializeObject(newData);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    responseMessage.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
                    return ResponseMessage(responseMessage);
                }
                else
                {
                    GetData.Data = Data;
                }

                return Ok(GetData); // 200 - Successful response with data
            }
            catch (Exception ex)
            {
                // Log the exception
                // You may also choose to return a more specific error response (e.g., 500 - Internal Server Error) here.
                return InternalServerError(ex);
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

                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Add Staff",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    return Ok(new { Id = Data.UserId, Message = "Staff has been added successfully." });
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


                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Update Staff",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();


                    return Ok(new { Id = Data.UserId, Message = "Staff has been updated successfully." });
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
            catch (Exception ex)
            {
                // Log the exception
                return InternalServerError(ex); // 500 - Internal Server Error
            }
        }
    }
}
