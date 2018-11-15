using System;
namespace TAClientLib
{
    [Serializable]
    public class EntityClass
    {
        public string EntityName { get; set; }
        public byte[] Key { get; set; }
    }
}
