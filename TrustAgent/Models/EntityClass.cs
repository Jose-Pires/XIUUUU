/*
 * TrustAgent.EntityClass.cs 
 * Developer: Pedro Cavaleiro
 * Developement stage: Completed
 * Tested on: macOS Mojave (10.14.1) -> PASSED
 * 
 * Model used to store entities and their keys in the database
 * 
 * Requires initialization: NO
 * Serializable
 * 
 */

using System;

namespace TrustAgent
{
    [Serializable]
    public class EntityClass
    {
        public string EntityName { get; set; }
        public byte[] Key { get; set; }
    }
}
