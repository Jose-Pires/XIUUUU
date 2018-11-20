using System;
namespace TAClientLib
{
    public class ClientMessage
    {
        public string Timestamp { get; set; }
        public string Entity { get; set; }
        public string Operation { get; set; }
        public string Message { get; set; }

        public ClientMessage()
        {
            Timestamp = Helpers.GetTimestamp(DateTime.Now);
        }

    }
}
