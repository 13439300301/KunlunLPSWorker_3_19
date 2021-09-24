using Kunlun.LPS.Worker.Core.MessageQueue;
using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Services.HRTProfile
{
    public class GetMemberResponse : BaseMessage
    {
        public string memberId { get; set; }
        public string memberLevel { get; set; }
        public string innerCardNo { get; set; }
        public string nickName { get; set; }
        public string virtualCardNo { get; set; }
        public string mobile { get; set; }
        public string loginCode { get; set; }
        public string memberStatus { get; set; }
        public string growthValue { get; set; }
        public List<CardModel> cardList { get; set; }

        public string modifyTime { get; set; }

        public List<AddressModel> addressList { get; set; }

        public string registerChannel { get; set; }

        public string certificateType { get; set; }

        public string certificateNo { get; set; }

        public string whiteFlag { get; set; }

        public string name { get; set; }
        public string periodStart { get; set; }
        public string email { get; set; }
        public string birthday { get; set; }
        public string periodEnd { get; set; }
        public string registerDate { get; set; }
        public string memo { get; set; }
        public string isStock { get; set; }
    }
    public class AddressModel
    {
        public string zipCode { get; set; }

        public string mobileNum { get; set; }

        public string consignee { get; set; }

        public string address { get; set; }

        public string city { get; set; }

        public string district { get; set; }

        public string telNo { get; set; }

        public string addressId { get; set; }

        public string defaultAddress { get; set; }

    }

    public class CardModel
    {
        public string merchantCode { get; set; }


        public string cardNo { get; set; }

        public string cardSign { get; set; }

        public string cardType { get; set; }

        public string caradStatus { get; set; }
    }
}
