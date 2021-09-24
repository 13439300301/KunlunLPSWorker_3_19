using System;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class FbItem : ConfigurationBase
    {
        public decimal Price { get; set; }

        public string HotelCode { get; set; }

        public string InsertUser
        {
            get;
            set;
        }
        
        public DateTime? InsertDate
        {
            get;
            set;
        }
        
        public string UpdateUser
        {
            get;
            set;
        }
        
        public DateTime? UpdateDate
        {
            get;
            set;
        }
    }
}
