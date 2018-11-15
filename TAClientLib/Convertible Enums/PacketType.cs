using System;
namespace TAClientLib
{
    public class PacketType
    {
        public PacketType(int value) { Value = value; }

        public int Value { get; set; }

        public static PacketType ServerCommand { get { return new PacketType(1); } }
        public static PacketType ClientMessage { get { return new PacketType(2); } }
        public static PacketType ServerResponse { get { return new PacketType(3); } }
    }
}
