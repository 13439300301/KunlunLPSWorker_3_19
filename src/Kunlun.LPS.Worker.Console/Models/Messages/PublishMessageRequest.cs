namespace Kunlun.LPS.Worker.Console.Models.Messages
{
    public class PublishMessageRequest
    {
        public string RoutingKey { get; set; }

        public string Message { get; set; }
    }
}
