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
    public class ItemController : ApiController
    {
        earthcoEntities DB = new earthcoEntities();
        [HttpGet]
        public IHttpActionResult GetItemList()
        {
            try
            {
                List<tblItem> Data = new List<tblItem>();
                Data = DB.tblItems.Where(x => x.isDelete != true).ToList();
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
        public IHttpActionResult GetItem(int id)
        {
            try
            {
                tblItem Data = new tblItem();
                Data = DB.tblItems.Where(x => x.ItemId == id && x.isDelete != true).FirstOrDefault();
                if (Data == null)
                {
                    Data = new tblItem();
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
        public IHttpActionResult AddItem([FromBody] tblItem Item)
        {
            try
            {
                //var userIdClaim = User.Identity as ClaimsIdentity;
                //int userId = int.Parse(userIdClaim.FindFirst("userid")?.Value);
                int userId = 2; // Replace with your authentication mechanism to get the user's ID.

                tblItem Data = new tblItem();

                if (Item.ItemId == 0)
                {
                   
                    Data = Item;
                    Data.CreatedDate = DateTime.Now;
                    Data.CreatedBy = userId;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = Item.isActive;


                    DB.tblItems.Add(Data);
                    DB.SaveChanges();

                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Add Item",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();

                    return Ok(new { Id = Data.ItemId, Message = "Item has been added successfully." });
                }
                else
                {
                    // Updating an existing customer.
                    Data = DB.tblItems.SingleOrDefault(c => c.ItemId == Item.ItemId);

                    if (Data == null)
                    {
                        return NotFound(); // Customer not found.
                    }

                    Data.ItemName = Item.ItemName;
                    Data.Description = Item.Description;
                    Data.Type = Item.Type;
                    Data.Price = Item.Price;
                    Data.EditDate = DateTime.Now;
                    Data.EditBy = userId;
                    Data.isActive = Item.isActive;

                    DB.Entry(Data);
                    DB.SaveChanges();


                    var logData = new tblLog
                    {
                        UserId = userId,
                        Action = "Update Item",
                        CreatedDate = DateTime.Now
                    };

                    DB.tblLogs.Add(logData);
                    DB.SaveChanges();


                    return Ok(new { Id = Data.ItemId, Message = "Item has been updated successfully." });
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
        public IHttpActionResult DeleteItem(int id)
        {
            try
            {
                int userId = 2; // Replace with your authentication mechanism to get the user's ID.


                tblItem Data = DB.tblItems.FirstOrDefault(c => c.ItemId == id);

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
                    Action = "Delete Item",
                    CreatedDate = DateTime.Now
                };

                DB.tblLogs.Add(logData);
                DB.SaveChanges();

                return Ok("Item has been deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception
                return InternalServerError(ex); // 500 - Internal Server Error
            }
        }
    }
}
