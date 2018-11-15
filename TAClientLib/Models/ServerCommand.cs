using System;
namespace TAClientLib
{
    public class ServerCommand
    {
        public string Command { get; set; }
        public string Message { get; set; }
        public bool EnableSpy { get; set; }
        public string SpyIP { get; set; }
        public int SpyPort { get; set; }
    }
}
