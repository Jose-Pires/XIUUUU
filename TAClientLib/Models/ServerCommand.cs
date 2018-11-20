using System;
namespace TAClientLib
{
    public class ServerCommand
    {
        public string Timestamp { get; set; }
        public string Command { get; set; }
        public string Message { get; set; }
        public bool EnableSpy { get; set; }
        public string SpyIP { get; set; }
        public int SpyPort { get; set; }

        public ServerCommand()
        {
            Timestamp = Helpers.GetTimestamp(DateTime.Now);
        }

    }
}
