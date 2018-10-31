using System;
namespace TrustAgent
{
    public class NetworkPacket
    {
        public string Entity { get; set; }
        public string Operation { get; set; }
        public string Message { get; set; }
        public string HMAC { get; set; }
    }
}
