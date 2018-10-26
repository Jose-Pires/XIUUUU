using System;

namespace SharedModels
{
    [Serializable]
    public class EntityClass
    {
        public string IdentityName { get; set; }
        public byte[] PreSharedKey { get; set; }
        public byte[] Validation { get; set; }
        public string TimeStamp { get; set; }
    }
}
