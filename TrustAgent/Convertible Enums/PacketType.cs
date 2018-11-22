/*
 * TrustAgent.PacketType.cs 
 * Developer: Pedro Cavaleiro
 * Developement stage: Completed
 * Tested on: macOS Mojave (10.14.1) -> PASSED
 * 
 * An enum simulator that can be converted to a int
 * 
 * Requires initialization: NO
 * 
 */

namespace TrustAgent
{
    public class PacketType
    {
        public PacketType(int value) { Value = value; }

        public int Value { get; set; }

        public static PacketType ServerCommand { get { return new PacketType(1); } }
        public static PacketType ClientMessage { get { return new PacketType(2); } }
    }
}
