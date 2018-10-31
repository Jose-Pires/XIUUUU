using System;
namespace TrustAgent
{
    public class ServerCommand
    {
        public string Command { get; set; }
        public bool EnableSpy { get; set; }
        public string SpyIP { get; set; }
        public int SpyPort { get; set; }
    }
}
