using System;
using System.ComponentModel.DataAnnotations;

namespace Kunlun.LPS.Worker.Core.Common
{
    public abstract class BaseEntity
    {
        [Display(Name = "操作人")]
        public string InsertUser
        {
            get;
            set;
        }
        [Display(Name = "操作日期")]
        public DateTime? InsertDate
        {
            get;
            set;
        }
        [Display(Name = "修改人")]
        public string UpdateUser
        {
            get;
            set;
        }
        [Display(Name = "修改日期")]
        public DateTime? UpdateDate
        {
            get;
            set;
        }
    }
}
