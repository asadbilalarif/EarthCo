using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EarthCo.Models
{
    public class SyncQB
    {
        public class DataChangeEvent
        {
            public List<Entity> Entities { get; set; }
        }

        public class Entity
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public string Operation { get; set; }
            public DateTime LastUpdated { get; set; }
        }

        public class EventNotification
        {
            public string RealmId { get; set; }
            public DataChangeEvent DataChangeEvent { get; set; }
        }

        public class RootObject
        {
            public List<EventNotification> EventNotifications { get; set; }
        }

    }
}