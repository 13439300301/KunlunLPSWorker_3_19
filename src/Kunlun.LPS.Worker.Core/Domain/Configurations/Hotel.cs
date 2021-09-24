using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Domain.Configurations
{
    public class Hotel : ConfigurationBase
    {
        public string Flag { get; set; }

        public string Des { get; set; }

        public int? Seq { get; set; }

        public string EngName { get; set; }

        public string Remark { get; set; }

        public string CityCode { get; set; }

        public string Keyword { get; set; }

        public DateTime? CreatedOn { get; set; }

        public string Stars { get; set; }

        public string Address { get; set; }

        public int? TotalRoom { get; set; }

        public string ProvinceCode { get; set; }

        public string CountryCode { get; set; }

        public string HotelGroupCode { get; set; }

        public string FullName { get; set; }

        public string EngAddress { get; set; }

        public int? TotalArea { get; set; }

        public int? TotalMeetingArea { get; set; }

        public int? TotalFbArea { get; set; }

        public string UniteHotels { get; set; }

        public string DbName { get; set; }

        public string PostCode { get; set; }

        public string Fax { get; set; }

        public string Tel { get; set; }

        public string Udf1 { get; set; }

        public string Udf2 { get; set; }

        public string Email { get; set; }

        public string Mobile { get; set; }

        public string Currency { get; set; }

        public string HotelOperationType { get; set; }

        public string InternalCode { get; set; }

        public string TimeZone { get; set; }
    }
}
