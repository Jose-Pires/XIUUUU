using System;
using System.Collections;
using System.Security.Cryptography;

namespace TrustAgent
{
    public static class SHA256hmac
    {

        /// <summary>
        /// Computes the HMAC of an array of bytes
        /// </summary>
        /// <returns>The hmac.</returns>
        /// <param name="input">Input to compute.</param>
        /// <param name="key">Key.</param>
        public static byte[] ComputeHMAC(byte[] input, byte[] key)
        {
            using (var hMACSHA256 = new HMACSHA256(key))
            {
                return hMACSHA256.ComputeHash(input);
            }
        }

        public static bool CompareHMAC(byte[] original, byte[] computed) {
            return ((IStructuralEquatable)original).Equals(computed, StructuralComparisons.StructuralEqualityComparer);
        }

    }
}
