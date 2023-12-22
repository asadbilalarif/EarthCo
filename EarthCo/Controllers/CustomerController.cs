using EarthCo.Models;
//using Newtonsoft.Json;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace EarthCo.Controllers
{
    //[Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    [Authorize]
    public class CustomerController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();

        //[HttpGet]
        //public List<GetCustomerContact> GetCustomersList()
        //{
        //    List<GetCustomerContact> Data = new List<GetCustomerContact>();
        //    GetCustomerContact Temp = null;
        //    List<tblCustomer> CusData = new List<tblCustomer>();
        //    tblContact ConData = new tblContact();
        //    CusData = DB.tblCustomers.ToList();
        //    foreach (var item in CusData)
        //    {
        //        Temp = new GetCustomerContact();
        //        ConData = item.tblContacts.Where(x => x.isPrimary == true).FirstOrDefault();

        //        Temp.CustomerId = item.CustomerId;
        //        Temp.CustomerName = item.CustomerName;
        //        if (ConData != null)
        //        {
        //            Temp.ContactId = ConData.ContactId;
        //            Temp.ContactName = ConData.FirstName + " " + ConData.FirstName;
        //            Temp.ContactCompany = ConData.CompanyName;
        //            Temp.ContactEmail = ConData.Email;
        //        }

        //        Data.Add(Temp);
        //    }

        //    return Data;
        //}

        [HttpGet]
        public IHttpActionResult GetSearchCustomersList(string Search)
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblUser> Data = new List<tblUser>();
                Data = DB.tblUsers.Where(x => x.UserTypeId==2 && x.isDelete !=true && x.CompanyName.ToLower().Contains(Search.ToLower())).Take(10).ToList();
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
        public IHttpActionResult GetCustomersServerSideList(string Search,int DisplayStart = 0, int DisplayLength = 10, bool isAscending = false)
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblUser> Data = new List<tblUser>();
                List<GetCustomerList> Result = new List<GetCustomerList>();
                
                var totalRecords = DB.tblUsers.Count(x => x.UserTypeId == 2 && x.isDelete != true);
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
                                                  || x.CompanyName.ToString().ToLower().Contains(Search.ToLower())
                                                  || x.Address.ToLower().Contains(Search.ToLower())
                                                  || x.Email.ToLower().Contains(Search.ToLower())
                                                  || x.UserId.ToString().ToLower().Contains(Search.ToLower())).ToList();
                    totalRecords = Data.Count(x => x.UserTypeId == 2 && x.isDelete != true);
                    Data = Data.Where(x => x.UserTypeId == 2 && x.isDelete != true).OrderBy(o =>isAscending? o.UserId:-o.UserId).Skip(DisplayStart).Take(DisplayLength).ToList();
                }
                else
                {
                    Data = DB.tblUsers.Where(x => x.UserTypeId == 2 && x.isDelete != true).OrderBy(o => isAscending ? o.UserId : -o.UserId).Skip(DisplayStart).Take(DisplayLength).ToList();
                }


                

                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }
                else
                {
                    foreach (tblUser item in Data)
                    {
                        GetCustomerList Temp = new GetCustomerList();
                        Temp.CustomerId = item.UserId;
                        Temp.CompanyName = item.CompanyName;
                        Temp.ContactName = item.ContactName;
                        Temp.CustomerName = item.FirstName + " " + item.LastName;
                        Temp.Address = item.Address;
                        Temp.Email = item.Email;
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
        public IHttpActionResult GetCustomersList()
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblUser> Data = new List<tblUser>();
                List<GetCustomerList> Result = new List<GetCustomerList>();
                Data = DB.tblUsers.Where(x => x.UserTypeId == 2 && x.isDelete != true).ToList();

                if (Data == null || Data.Count == 0)
                {
                    return NotFound(); // 404 - No data found
                }
                else
                {
                    foreach (tblUser item in Data)
                    {
                        GetCustomerList Temp = new GetCustomerList();
                        Temp.CustomerId = item.UserId;
                        Temp.CompanyName = item.CompanyName;
                        Temp.CustomerName = item.FirstName+" "+item.LastName;
                        Temp.Address = item.Address;
                        Temp.Email = item.Email;
                        Result.Add(Temp);
                    }
                }

                return Ok(Result); // 200 - Successful response with data
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

        //[HttpGet]
        //public GetCustomerContact GetCustomer(int id)
        //{
        //    GetCustomerContact Data = new GetCustomerContact();
        //    tblCustomer CusData = new tblCustomer();
        //    tblContact ConData = new tblContact();
        //    CusData = DB.tblCustomers.Where(x => x.CustomerId == id).FirstOrDefault();
        //    if(CusData==null)
        //    {
        //        return null;
        //    }
        //    ConData = CusData.tblContacts.Where(x => x.isPrimary == true).FirstOrDefault();
        //    Data.CustomerId = CusData.CustomerId;
        //    Data.CustomerName = CusData.CustomerName;
        //    Data.ContactId = ConData.ContactId;
        //    Data.ContactName = ConData.FirstName + " " + ConData.FirstName;
        //    Data.ContactCompany = ConData.CompanyName;
        //    Data.ContactEmail = ConData.Email;

        //    return Data;
        //}

        [HttpGet]
        public IHttpActionResult GetCustomerNameById(int id)
        {
            try
            {
                //tblUser Data = new tblUser();
                //CustomerContacts CustomerData = new CustomerContacts();
                //Data = DB.tblUsers.Where(x => x.UserId == id && x.isDelete != true).FirstOrDefault();

                string Data = DB.tblUsers.Where(x=>x.isActive==true && x.isDelete==false&& x.UserTypeId==2 && x.UserId== id).Select(s=>s.CompanyName).FirstOrDefault();
                
                if (Data == null)
                {
                    //GetCustomerData NewData = new GetCustomerData();
                    //string userJson = JsonConvert.SerializeObject(NewData);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    //responseMessage.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
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


        [HttpGet]
        public IHttpActionResult GetCustomer(int id)
        {
            try
            {
                //tblUser Data = new tblUser();
                //CustomerContacts CustomerData = new CustomerContacts();
                //Data = DB.tblUsers.Where(x => x.UserId == id && x.isDelete != true).FirstOrDefault();

                SPGetCustomerData_Result Data = new SPGetCustomerData_Result();
                List<SPGetCustomerContactData_Result> ContactData = new List<SPGetCustomerContactData_Result>();
                List<SPGetCustomerServiceLocationData_Result> ServiceLocationData = new List<SPGetCustomerServiceLocationData_Result>();
                Data = DB.SPGetCustomerData(id).FirstOrDefault();
                ContactData = DB.SPGetCustomerContactData(id).ToList();
                ServiceLocationData = DB.SPGetCustomerServiceLocationData(id).ToList();


                GetCustomerData GetData = new GetCustomerData();
                if (Data == null)
                {
                    GetCustomerData NewData = new GetCustomerData();
                    string userJson = Newtonsoft.Json.JsonConvert.SerializeObject(NewData);
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
                    responseMessage.Content = new StringContent(userJson, Encoding.UTF8, "application/json");
                    return ResponseMessage(responseMessage);
                }
                else
                {
                    Data.Password = "";
                    GetData.Data = Data;
                    GetData.Data.Password = "";
                    GetData.ContactData = ContactData;
                    GetData.ServiceLocationData = ServiceLocationData;
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

        [HttpGet]
        public IHttpActionResult GetCustomerContact(int id)
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblContact> Data = new List<tblContact>();
                Data = DB.tblContacts.Where(x => x.CustomerId == id && x.isDelete != true).ToList();
                if (Data == null || Data.Count==0)
                {
                    Data = new List<tblContact>();
                    string userJson = Newtonsoft.Json.JsonConvert.SerializeObject(Data);
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

        [HttpGet]
        public IHttpActionResult GetCustomerTypes()
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblCustomerType> Data = new List<tblCustomerType>();
                Data = DB.tblCustomerTypes.ToList();
                if (Data == null || Data.Count == 0)
                {
                    var responseMessage = new HttpResponseMessage(HttpStatusCode.NotFound);
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


        [HttpGet]
        public IHttpActionResult GetCustomerServiceLocation(int id)
        {
            try
            {
                DB.Configuration.ProxyCreationEnabled = false;
                List<tblServiceLocation> Data = new List<tblServiceLocation>();
                Data = DB.tblServiceLocations.Where(x => x.CustomerId == id && x.isDelete != true).ToList();
                if (Data == null || Data.Count == 0)
                {
                    Data = new List<tblServiceLocation>();
                    string userJson = Newtonsoft.Json.JsonConvert.SerializeObject(Data);
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


        //[HttpPost]
        //public String AddCustomer([FromBody] CustomerContacts Customer)
        //{
        //    tblCustomer Data = new tblCustomer();
        //    try
        //    {
        //        //HttpCookie cookieObj = Request.Cookies["User"];
        //        //int UserId = Int32.Parse(cookieObj["UserId"]);
        //        //int RoleId = Int32.Parse(cookieObj["RoleId"]);
        //        int UserId = 2;
        //        if (Customer.CustomerData.CustomerId== 0)
        //        {
        //            Data.CustomerName = Customer.CustomerData.CustomerName;
        //            Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //            Data.CreatedBy = UserId;
        //            Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //            Data.EditBy = UserId;
        //            Data.isActive = Customer.CustomerData.isActive;
        //            DB.tblCustomers.Add(Data);
        //            DB.SaveChanges();

        //            if(Customer.ContactData!=null && Customer.ContactData.Count != 0)
        //            {
        //                tblContact ConData = null;

        //                foreach (var item in Customer.ContactData)
        //                {
        //                    ConData = new tblContact();
        //                    ConData = item;
        //                    ConData.CustomerName = Data.CustomerName;
        //                    ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //                    ConData.CreatedBy = UserId;
        //                    ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //                    ConData.EditBy = UserId;
        //                    ConData.isActive = item.isActive;
        //                    ConData.isPrimary = item.isPrimary;
        //                    ConData.CustomerId = Data.CustomerId;
        //                    DB.tblContacts.Add(ConData);
        //                    DB.SaveChanges();
        //                }

        //            }

        //            tblLog LogData = new tblLog();
        //            LogData.UserId = UserId;
        //            LogData.Action = "Add Customer";
        //            LogData.CreatedDate = DateTime.Now;
        //            DB.tblLogs.Add(LogData);
        //            DB.SaveChanges();
        //            return "Customer has been added successfully.";
        //        }
        //        else
        //        {
        //            Data = DB.tblCustomers.Select(r => r).Where(x => x.CustomerId == Customer.CustomerData.CustomerId).FirstOrDefault();

        //            Data.CustomerName = Customer.CustomerData.CustomerName;
        //            Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //            Data.EditBy = UserId;
        //            Data.isActive = Customer.CustomerData.isActive;
        //            DB.Entry(Data);
        //            DB.SaveChanges();

        //            List<tblContact> ConList = DB.tblContacts.Where(x => x.CustomerId == Customer.CustomerData.CustomerId).ToList();
        //            if(ConList!=null && ConList.Count!=0)
        //            {
        //                DB.tblContacts.RemoveRange(ConList);
        //                DB.SaveChanges();
        //            }

        //            if (Customer.ContactData != null && Customer.ContactData.Count != 0)
        //            {
        //                tblContact ConData = null;

        //                foreach (var item in Customer.ContactData)
        //                {
        //                    ConData = new tblContact();
        //                    ConData = item;
        //                    ConData.CustomerName = Data.CustomerName;
        //                    ConData.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //                    ConData.CreatedBy = UserId;
        //                    ConData.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
        //                    ConData.EditBy = UserId;
        //                    ConData.isActive = item.isActive;
        //                    ConData.isPrimary = item.isPrimary;
        //                    ConData.CustomerId = Data.CustomerId;
        //                    DB.tblContacts.Add(ConData);
        //                    DB.SaveChanges();
        //                }

        //            }

        //            tblLog LogData = new tblLog();
        //            LogData.UserId = UserId;
        //            LogData.Action = "Update Customer";
        //            LogData.CreatedDate = DateTime.Now;
        //            DB.tblLogs.Add(LogData);
        //            DB.SaveChanges();

        //            return "Customer has been Update successfully.";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}

        [HttpPost]
        public IHttpActionResult AddCustomer([FromBody] tblUser Customer)
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
                        responseMessage.Content = new StringContent("Customer with same Email already exsist!!!");
                        return ResponseMessage(responseMessage);
                    }

                    Data = Customer;
                    Data.RoleId = 2;
                    Data.UserTypeId = 2;
                    Data.CreatedDate = DateTime.Now;
                    Data.CreatedBy = userId;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = true;
                    Data.isDelete = false;

                    if(Customer.isLoginAllow==true)
                    {
                        byte[] EncDataBtye = new byte[Customer.Password.Length];
                        EncDataBtye = System.Text.Encoding.UTF8.GetBytes(Customer.Password);
                        Data.Password = Convert.ToBase64String(EncDataBtye);
                    }

                    DB.tblUsers.Add(Data);
                    DB.SaveChanges();


                    tblSyncLog Result = new tblSyncLog();
                    Result.Id = Data.UserId;
                    Result.Name = "Customer";
                    Result.Operation = "Create";
                    Result.CreatedDate = DateTime.Now;
                    Result.isQB = false;
                    Result.isSync = false;
                    DB.tblSyncLogs.Add(Result);
                    DB.SaveChanges();

                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Add Customer",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    return Ok(new { Id = Data.UserId, SyncId = Result.SyncLogId, Message = "Customer has been added successfully." });
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

                    if (CheckUser != null && CheckUser.UserId!=Customer.UserId)
                    {
                        var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                        responseMessage.Content = new StringContent("Customer with same Email already exsist!!!");
                        return ResponseMessage(responseMessage);
                    }
                    Data.CompanyName = Customer.CompanyName;
                    Data.ContactName = Customer.ContactName;
                    Data.FirstName = Customer.FirstName;
                    Data.LastName = Customer.LastName;
                    Data.Address = Customer.Address;
                    Data.City = Customer.City;
                    Data.State = Customer.State;
                    Data.Country = Customer.Country;
                    Data.Code = Customer.Code;
                    Data.UserTypeId = 2;
                    Data.Phone = Customer.Phone;
                    Data.AltPhone = Customer.AltPhone;
                    Data.Fax = Customer.Fax;
                    Data.Notes = Customer.Notes;
                    Data.CustomerTypeId = Customer.CustomerTypeId;
                    Data.username = Customer.username;
                    Data.Email = Customer.Email;
                    Data.RoleId = 2;
                    Data.UserTypeId = 2;
                    Data.isLoginAllow = Customer.isLoginAllow;
                    if (Customer.isLoginAllow == true)
                    {
                        
                        if(Customer.Password!=null && Customer.Password != "")
                        {
                            byte[] EncDataBtye = new byte[Customer.Password.Length];
                            EncDataBtye = System.Text.Encoding.UTF8.GetBytes(Customer.Password);
                            Data.Password = Convert.ToBase64String(EncDataBtye);
                        }
                        else
                        {
                            Data.Password = Data.Password;
                        }
                    }


                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = true;
                    Data.isDelete = false;

                    DB.Entry(Data);
                    DB.SaveChanges();

                    tblSyncLog Result = new tblSyncLog();
                    Result = DB.tblSyncLogs.Where(x => x.Id == Data.UserId && x.Name == "Customer").FirstOrDefault();
                    if (Result == null)
                    {
                        Result = new tblSyncLog();
                        Result.Id = Data.UserId;
                        Result.Name = "Customer";
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
                        Result.Name = "Customer";
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
                        Action = "Update Customer",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();


                    return Ok(new { Id = Data.UserId, SyncId = Result.SyncLogId, Message = "Customer has been updated successfully." });
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


        [HttpPost]
        public IHttpActionResult AddContact([FromBody] tblContact Contact)
        {
            try
            {
                //var userIdClaim = User.Identity as ClaimsIdentity;
                //int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                var userIdClaim = User.Identity as ClaimsIdentity;
                int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);

                tblContact Data = new tblContact();

                if (Contact.ContactId == 0)
                {
                    // Creating a new customer.

                    Data = Contact;
                    Data.CreatedDate = DateTime.Now;
                    Data.CreatedBy = userId;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = true;
                    Data.isDelete = false;

                    //if (Contact.isLoginAllow == true)
                    //{
                    //    byte[] EncDataBtye = new byte[Contact.Password.Length];
                    //    EncDataBtye = System.Text.Encoding.UTF8.GetBytes(Contact.Password);
                    //    Data.Password = Convert.ToBase64String(EncDataBtye);
                    //}

                    DB.tblContacts.Add(Data);
                    DB.SaveChanges();

                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Add Contact",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    return Ok(new { Id = Data.ContactId, Message = "Contact has been added successfully." });
                }
                else
                {
                    // Updating an existing customer.
                    Data = DB.tblContacts.SingleOrDefault(c => c.ContactId == Contact.ContactId);

                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }

                    Data.CompanyName = Contact.CompanyName;
                    Data.FirstName = Contact.FirstName;
                    Data.LastName = Contact.LastName;
                    Data.Phone = Contact.Phone;
                    Data.AltPhone = Contact.AltPhone;
                    Data.Email = Contact.Email;
                    Data.Address = Contact.Address;
                    Data.CustomerId = Contact.CustomerId;
                    Data.Comments = Contact.Comments;
                    Data.UserName = Contact.UserName;
                    Data.CustomerName = Contact.CustomerName;
                   
                    //if (Contact.isLoginAllow == true)
                    //{
                    //    Data.isLoginAllow = Contact.isLoginAllow;
                    //    if (Contact.Password != null && Contact.Password != "")
                    //    {
                    //        byte[] EncDataBtye = new byte[Contact.Password.Length];
                    //        EncDataBtye = System.Text.Encoding.UTF8.GetBytes(Contact.Password);
                    //        Data.Password = Convert.ToBase64String(EncDataBtye);
                    //    }
                    //}

                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = true;
                    Data.isDelete = false;

                    DB.Entry(Data);
                    DB.SaveChanges();


                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Update Customer",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    return Ok(new { Id = Data.ContactId, Message = "Customer has been updated successfully." });
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

        [HttpPost]
        public IHttpActionResult AddServiceLocation([FromBody] tblServiceLocation ServiceLocation)
        {
            try
            {
                //var userIdClaim = User.Identity as ClaimsIdentity;
                //int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                var userIdClaim = User.Identity as ClaimsIdentity;
                int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);

                tblServiceLocation Data = new tblServiceLocation();

                if (ServiceLocation.ServiceLocationId == 0)
                {
                    // Creating a new customer.

                    Data = ServiceLocation;
                    Data.CreatedDate = DateTime.Now;
                    Data.CreatedBy = userId;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = true;
                    Data.isDelete = false;
                    DB.tblServiceLocations.Add(Data);
                    DB.SaveChanges();

                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Add Service Location",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    return Ok(new { Id = Data.ServiceLocationId, Message = "ServiceLocation has been added successfully." });
                }
                else
                {
                    // Updating an existing customer.
                    Data = DB.tblServiceLocations.SingleOrDefault(c => c.ServiceLocationId == ServiceLocation.ServiceLocationId);

                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }

                    Data.Name = ServiceLocation.Name;
                    Data.isBilltoCustomer = ServiceLocation.isBilltoCustomer;
                    Data.CustomerId = ServiceLocation.CustomerId;
                    Data.Phone = ServiceLocation.Phone;
                    Data.AltPhone = ServiceLocation.AltPhone;
                    Data.Address = ServiceLocation.Address;
                    Data.lat = ServiceLocation.lat;
                    Data.lng = ServiceLocation.lng;
                    Data.CustomerId = ServiceLocation.CustomerId;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = true;
                    Data.isDelete = false;

                    DB.Entry(Data);
                    DB.SaveChanges();


                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Update Customer",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    return Ok(new { Id = Data.ServiceLocationId, Message = "ServiceLocation has been updated successfully." });
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

        //[HttpGet]
        //public string DeleteCustomer(int id)
        //{
        //    tblCustomer Data = new tblCustomer();
        //    //HttpCookie cookieObj = Request.Cookies["User"];
        //    //int CUserId = Int32.Parse(cookieObj["UserId"]);
        //    int CUserId = 2;
        //    try
        //    {

        //        List<tblContact> ConList = DB.tblContacts.Where(x => x.CustomerId == id).ToList();
        //        if (ConList != null && ConList.Count != 0)
        //        {
        //            DB.tblContacts.RemoveRange(ConList);
        //            DB.SaveChanges();
        //        }

        //        Data = DB.tblCustomers.Select(r => r).Where(x => x.CustomerId== id).FirstOrDefault();
        //        DB.Entry(Data).State = EntityState.Deleted;
        //        DB.SaveChanges();

        //        tblLog LogData = new tblLog();
        //        LogData.UserId = CUserId;
        //        LogData.Action = "Delete Customer";
        //        LogData.CreatedDate = DateTime.Now;
        //        DB.tblLogs.Add(LogData);
        //        DB.SaveChanges();
        //        return "Customer has been deleted successfully.";
        //    }
        //    catch (Exception ex)
        //    {
        //        return ex.Message;
        //    }
        //}

        [HttpGet]
        public IHttpActionResult DeleteCustomer(int id)
        {
            try
            {
                var userIdClaim = User.Identity as ClaimsIdentity;
                int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);

                // Check if the customer with the specified ID exists

                //tblEstimate CheckEstimate = DB.tblEstimates.Where(x => x.CustomerId == id).FirstOrDefault();
                //tblServiceRequest CheckServices = DB.tblServiceRequests.Where(x => x.CustomerId == id).FirstOrDefault();
                //tblPunchlist CheckPunchlist = DB.tblPunchlists.Where(x => x.CustomerId == id).FirstOrDefault();

                //if (CheckEstimate!=null || CheckServices!=null || CheckPunchlist!=null)
                //{
                //    var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                //    responseMessage.Content = new StringContent("Cannot delete the record due to related data.");
                //    return ResponseMessage(responseMessage);
                //}

                tblUser Data = DB.tblUsers.FirstOrDefault(c => c.UserId == id);

                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }

                //// Remove associated contacts
                //var contactsToDelete = DB.tblContacts.Where(c => c.CustomerId == id).ToList();
                //DB.tblContacts.RemoveRange(contactsToDelete);

                // Remove the customer
                Data.isDelete = true;
                Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                Data.CreatedBy = userId;
                Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                Data.EditBy = userId;
                DB.Entry(Data);
                DB.SaveChanges();

                tblSyncLog Result = new tblSyncLog();
                Result = DB.tblSyncLogs.Where(x => x.Id == Data.UserId && x.Name == "Customer").FirstOrDefault();
                if (Result == null)
                {
                    Result = new tblSyncLog();
                    Result.Id = Data.UserId;
                    Result.Name = "Customer";
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
                    Result.Name = "Customer";
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
                    Action = "Delete Customer",
                    CreatedDate = DateTime.Now
                };

                DB.tblLogs.Add(logData);
                DB.SaveChanges();

                return Ok("Customer has been deleted successfully.");
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
        public IHttpActionResult DeleteContact(int id)
        {
            try
            {
                var userIdClaim = User.Identity as ClaimsIdentity;
                int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);

                // Check if the customer with the specified ID exists

                //tblEstimate CheckEstimate = DB.tblEstimates.Where(x => x.CustomerId == id).FirstOrDefault();
                //tblServiceRequest CheckServices = DB.tblServiceRequests.Where(x => x.CustomerId == id).FirstOrDefault();
                //tblPunchlist CheckPunchlist = DB.tblPunchlists.Where(x => x.CustomerId == id).FirstOrDefault();

                //if (CheckEstimate!=null || CheckServices!=null || CheckPunchlist!=null)
                //{
                //    var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                //    responseMessage.Content = new StringContent("Cannot delete the record due to related data.");
                //    return ResponseMessage(responseMessage);
                //}

                tblContact Data = DB.tblContacts.FirstOrDefault(c => c.ContactId == id);

                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }

                //// Remove associated contacts
                //var contactsToDelete = DB.tblContacts.Where(c => c.CustomerId == id).ToList();
                //DB.tblContacts.RemoveRange(contactsToDelete);

                // Remove the customer
                Data.isDelete = true;
                Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                Data.CreatedBy = userId;
                Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                Data.EditBy = userId;
                DB.Entry(Data);
                DB.SaveChanges();

                // Log the action
                var logData = new tblLog
                {
                    UserId = userId,
                    Action = "Delete Contact",
                    CreatedDate = DateTime.Now
                };

                DB.tblLogs.Add(logData);
                DB.SaveChanges();

                return Ok("Contact has been deleted successfully.");
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
        public IHttpActionResult DeleteServiceLocation(int id)
        {
            try
            {
                var userIdClaim = User.Identity as ClaimsIdentity;
                int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);

                // Check if the customer with the specified ID exists

                //tblEstimate CheckEstimate = DB.tblEstimates.Where(x => x.CustomerId == id).FirstOrDefault();
                //tblServiceRequest CheckServices = DB.tblServiceRequests.Where(x => x.CustomerId == id).FirstOrDefault();
                //tblPunchlist CheckPunchlist = DB.tblPunchlists.Where(x => x.CustomerId == id).FirstOrDefault();

                //if (CheckEstimate!=null || CheckServices!=null || CheckPunchlist!=null)
                //{
                //    var responseMessage = new HttpResponseMessage(HttpStatusCode.Conflict);
                //    responseMessage.Content = new StringContent("Cannot delete the record due to related data.");
                //    return ResponseMessage(responseMessage);
                //}

                tblServiceLocation Data = DB.tblServiceLocations.FirstOrDefault(c => c.ServiceLocationId == id);

                if (Data == null)
                {
                    return NotFound(); // 404 - Customer not found
                }

                //// Remove associated contacts
                //var contactsToDelete = DB.tblContacts.Where(c => c.CustomerId == id).ToList();
                //DB.tblContacts.RemoveRange(contactsToDelete);

                // Remove the customer
                Data.isDelete = true;
                Data.CreatedDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                Data.CreatedBy = userId;
                Data.EditDate = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                Data.EditBy = userId;
                DB.Entry(Data);
                DB.SaveChanges();

                // Log the action
                var logData = new tblLog
                {
                    UserId = userId,
                    Action = "Delete Contact",
                    CreatedDate = DateTime.Now
                };

                DB.tblLogs.Add(logData);
                DB.SaveChanges();

                return Ok("Contact has been deleted successfully.");
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
        public IHttpActionResult SentInvite(InviteEmployee ParaData)
        {
            tblCustomer Data = new tblCustomer();
            //HttpCookie cookieObj = Request.Cookies["User"];
            //int CUserId = Int32.Parse(cookieObj["UserId"]);
            var userIdClaim = User.Identity as ClaimsIdentity;
            int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
            try
            {

                tblSetting setting = DB.tblSettings.Find(1);
                string SenderEmail = setting.Email;
                string SenderPassword = setting.Password;
                SmtpClient Client = new SmtpClient(setting.SMTP, Convert.ToInt32(setting.Port));
                //Client.EnableSsl = false;
                Client.EnableSsl = Convert.ToBoolean(setting.isActive); ;
                Client.Timeout = 100000;
                Client.DeliveryMethod = SmtpDeliveryMethod.Network;
                Client.UseDefaultCredentials = false;
                Client.Credentials = new System.Net.NetworkCredential(SenderEmail, SenderPassword);

                
                string link = Url.Request.RequestUri.ToString();
                //string link = "";
                link = link.Replace("ForgetPassword", "ChangeForgetPassword");

                byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(ParaData.Email);
                string encrypted = Convert.ToBase64String(b);

                byte[] t = System.Text.ASCIIEncoding.ASCII.GetBytes(DateTime.Now.ToString());
                string encryptedTime = Convert.ToBase64String(t);


                string body1 = "";
                body1 += "Welcome to EarthCo!";
                body1 += "<br />"+ParaData.FirstName+" "+ ParaData.LastName+ "";
                body1 += "<br />"+ParaData.InvitaionEmail+"";
                body1 += "<br /><br />Yours,<br />The EarthCo Team";

                string body = "";
                body += "<body  style='background-color:white !important'>";
                body += " <div>";
                //body += "<h3>Hello " + sa.ReceiveName + ",</h3>";
                body += " <table style='background-color: #f2f3f8; max-width:670px;' width='100%' border='0'  cellpadding='0' cellspacing='0'>";
                body += " <tbody> <tr style='background-color:#333333;'><td style='padding: 0 35px; background-color:#333333;text-align: center;'><a><img src='https://ci6.googleusercontent.com/proxy/Ia8xyYsLq6FtQcWzOyAOvF7XpZC5N9JGdMFlTO2LwH6Q_PSpKXU2LVHg6bmHoSGjTN1EKugOuHt6dFMCU82XXyTadS1p1EfV7a70vjNPbIkMB7z9H6h_9hgZNRA9bAJNWW-fi4jazw=s0-d-e1-ft#https://automatische-gartenberegnung.de/wp-content/uploads/2020/05/logo-1_200x50.png' style='padding-top: 1%;' alt='Alternate Text' />  </a></td> </tr>";
                body += "<tr style='color:#455056; font-size:15px;line-height:35px;text-align: center;'><td style='padding:6px;text-align: center;'></td></tr><tr style='color:#455056; font-size:15px;line-height:35px;text-align: center;'><td style='padding:6px;text-align: center;'>" + body1 + "</td></tr>";
                body += "  </tbody></table>";
                body += "</body>";


                MailMessage mailMessage = new MailMessage(SenderEmail, ParaData.Email, "Forget Password Email", body);
                mailMessage.IsBodyHtml = true;
                mailMessage.BodyEncoding = System.Text.UTF8Encoding.UTF8;

                Client.Send(mailMessage);


                tblLog LogData = new tblLog();
                LogData.UserId = userId;
                LogData.Action = "Sent invite";
                LogData.CreatedDate = DateTime.Now;
                DB.tblLogs.Add(LogData);
                DB.SaveChanges();
                return Ok("Invite has been sent successfully.");
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
