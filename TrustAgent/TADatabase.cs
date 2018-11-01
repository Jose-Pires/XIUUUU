using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Linq;
using static TrustAgent.StandardPrints;

namespace TrustAgent
{
    public class TADatabase
    {

        public byte[] entities { get; set; }
        public bool hasFile { get; set; }
        int seed1, seed2, selector = 0;
        string dbPath;

        public TADatabase(int seed1, int seed2, string dbPath)
        {
            this.seed1 = seed1;
            this.seed2 = seed2;
            this.dbPath = dbPath;
            if (File.Exists(dbPath))
            {
                hasFile = true;
                selector = LoadSelector();
                LoadFile();
            }
            else
                hasFile = false;
        }

        #region "Enums"

        public enum AddEntityError {
            exists,
            invalidKey,
            noError
        }

        public enum DelEntityError {
            notFound,
            noError
        }

        public enum ValidationError {
            notFound,
            invalidKey,
            noError,
            alreadyConnected
        }

        #endregion

        #region "Cipher Ops"

        /// <summary>
        /// Decrypts data using AES256.
        /// </summary>
        /// <returns>Decrypted data.</returns>
        /// <param name="data">Data to decrypt.</param>
        /// <param name="key">Key bytes.</param>
        /// <param name="iv">IV bytes.</param>
        byte[] DecryptData(byte[] data, byte[] key, byte[] iv)
        {
            try
            {
                using (var aes = Aes.Create())
                {
                    using (var decryptor = aes.CreateDecryptor(key, iv))
                    {
                        using (var ms = new MemoryStream())
                        using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                            cs.Close();
                            return ms.ToArray();
                        }
                    }
                }
            }
            catch (Exception) { return null; }
        }

        /// <summary>
        /// Encrypts data using AES256
        /// </summary>
        /// <returns>Encrypted data.</returns>
        /// <param name="data">Data to decrypt.</param>
        /// <param name="key">Key bytes.</param>
        /// <param name="iv">IV bytes.</param>
        byte[] EncryptData(byte[] data, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                using (var encryptor = aes.CreateEncryptor(key, iv))
                {
                    using (var ms = new MemoryStream())
                    using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        cs.Write(data, 0, data.Length);
                        cs.Close();
                        return ms.ToArray();
                    }
                }
            }
        }


        #endregion

        #region "File Handlers"

        /// <summary>
        /// Loads the selector by reading the first 4 bytes (int size) of the database file.
        /// </summary>
        /// <returns>The key and iv selector.</returns>
        int LoadSelector()
        {
            byte[] _selector = new byte[4];
            using (FileStream fs = new FileStream(dbPath, FileMode.Open, FileAccess.Read))
            {
                fs.Read(_selector, 0, _selector.Length);
                fs.Close();
            }
            return BitConverter.ToInt32(_selector);
        }

        /// <summary>
        /// Loads the database to memory without decrypting it. The selector is not read
        /// </summary>
        void LoadFile()
        {
            using (FileStream fs = new FileStream(dbPath, FileMode.Open, FileAccess.Read))
            {
                entities = new byte[fs.Length];
                fs.Read(entities, 0, (int)fs.Length);
                entities = entities.Skip(4).ToArray();
                fs.Close();
            }
        }

        public void WriteToFile()
        {

            if (entities != null)
            {
                bool dbValidation = ValidateDecryption();
                if (dbValidation)
                {
                    byte[] decrypted = DecryptData(entities, GenKey(), GenIV());
                    if (selector + 1 > 50)
                        selector = 0;
                    selector++;
                    entities = EncryptData(decrypted, GenKey(), GenIV());
                    byte[] selectorBytes = BitConverter.GetBytes(selector);
                    byte[] toWrite = new byte[selectorBytes.Length + entities.Length];
                    Array.Copy(selectorBytes, toWrite, selectorBytes.Length);
                    Array.Copy(entities, 0, toWrite, selectorBytes.Length, entities.Length);
                    using (FileStream fs = new FileStream(dbPath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        fs.Write(toWrite, 0, toWrite.Length);
                        fs.Close();
                    }
                }
            }

        }

        #endregion

        #region "Generators"

        /// <summary>
        /// Generates an IV based on the second seed (seed2) and the selector
        /// </summary>
        /// <returns>Returns the propper IV.</returns>
        byte[] GenIV()
        {
            List<byte[]> ivs = new List<byte[]>();
            Random rnd = new Random(seed2);
            for (int i = 0; i < 50; i++)
            {
                byte[] iv = new byte[16]; //IV 128bits
                rnd.NextBytes(iv);
                ivs.Add(iv);
            }
            return ivs[selector];
        }

        /// <summary>
        /// Generates an Key based on the first seed (seed1) and the selector
        /// </summary>
        /// <returns>Returns the propper Key.</returns>
        byte[] GenKey()
        {
            List<byte[]> keys = new List<byte[]>();
            Random rnd = new Random(seed1);
            for (int i = 0; i < 50; i++)
            {
                byte[] key = new byte[32]; //KEY 256bits
                rnd.NextBytes(key);
                keys.Add(key);
            }
            return keys[selector];
        }

        #endregion

        #region "HMAC Ops"

        public static byte[] Encode(byte[] input, byte[] key)
        {
            using (var hMACSHA256 = new HMACSHA256(key))
            {
                return hMACSHA256.ComputeHash(input);
            }
        }

        #endregion

        #region "Entity Manager"

        /// <summary>
        /// Adds a new entity to the TrustAgent database
        /// </summary>
        /// <returns>An type of error (if no error ocurrs returns AddEntityError.noError</returns>
        /// <param name="entity">Entity name.</param>
        /// <param name="sharedKey">Entity pre-shared key.</param>
        public AddEntityError AddEntity(string entity, string sharedKey)
        {
            byte[] iv = GenIV();
            byte[] key = GenKey();
            byte[] decripted;
            List<EntityClass> _entities;
            if (entities != null)
            {
                bool dbValidation = ValidateDecryption();
                if (dbValidation)
                {
                    decripted = DecryptData(entities, key, iv);
                    _entities = Helpers.FromByteArray<List<EntityClass>>(decripted);
                    decripted = null;

                    if (_entities.Any(m => m.EntityName == entity))
                    {
                        _entities = null;
                        iv = null;
                        key = null;
                        return AddEntityError.exists;
                    }
                } else {
                    _entities = new List<EntityClass>();
                }
            }
            else
            {
                _entities = new List<EntityClass>();
            }

            byte[] sharedKeyBytes = Helpers.StringToByteArray(sharedKey);

            _entities.Add(new EntityClass { EntityName = entity, Key = sharedKeyBytes });

            decripted = Helpers.ToByteArray(_entities);

            entities = EncryptData(decripted, key, iv);

            decripted = null;
            iv = null;
            key = null;

            return AddEntityError.noError;
        }

        /// <summary>
        /// Deletes an entity from the TrustAgent database
        /// </summary>
        /// <returns>An type of error (if no error ocurrs returns DelEntityError.noError</returns>
        /// <param name="entity">Entity name.</param>
        public DelEntityError DelEntity(string entity) {
            byte[] iv = GenIV();
            byte[] key = GenKey();
            byte[] decripted;
            List<EntityClass> _entities;
            if (entities != null)
            {
                bool dbValidation = ValidateDecryption();
                if (!dbValidation)
                    return DelEntityError.notFound;

                decripted = DecryptData(entities, key, iv);
                _entities = Helpers.FromByteArray<List<EntityClass>>(decripted);
                decripted = null;

                if (_entities.Any(m => m.EntityName == entity))
                {
                    _entities.Remove(_entities.First(m => m.EntityName == entity));
                    decripted = Helpers.ToByteArray(_entities);
                    entities = EncryptData(decripted, key, iv);
                    decripted = null;
                    return DelEntityError.noError;
                }

                iv = null;
                key = null;
                _entities = null;
                return DelEntityError.notFound;
            }
            iv = null;
            key = null;
            return DelEntityError.notFound;
        }

        /// <summary>
        /// Received network packet (contains entity name, message, 
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="packet">Packet.</param>
        public ValidationError ValidateEntity(NetworkPacket packet, byte[] hmac, byte[] raw) {
            if (entities == null)
                return ValidationError.notFound;

            bool dbValidation = ValidateDecryption();

            if (!dbValidation)
                return ValidationError.notFound;

            if (Program.sv.clientsList.Contains(packet.Entity))
                return ValidationError.alreadyConnected;

            byte[] decripted = DecryptData(entities, GenKey(), GenIV());

            List<EntityClass> _entities = Helpers.FromByteArray<List<EntityClass>>(decripted);
            decripted = null;
            if (_entities.Any(m => m.EntityName == packet.Entity)) {
                EntityClass storedEntity = _entities.First(m => m.EntityName == packet.Entity);
                byte[] calcHMAC = Encode(raw, storedEntity.Key);
                return (Helpers.ByteArrayToString(hmac) == Helpers.ByteArrayToString(calcHMAC)) ? ValidationError.noError : ValidationError.invalidKey;
            }
            _entities = null;
            return ValidationError.notFound;
        }

        #endregion

        /// <summary>
        /// Verifies if the database can be decrypted, if it can not warns the user and deletes the file
        /// </summary>
        /// <returns><c>true</c>, if decryption was validated, <c>false</c> otherwise.</returns>
        bool ValidateDecryption() {
            if (entities != null) {
                byte[] iv = GenIV();
                byte[] key = GenKey();
                byte[] decripted = DecryptData(entities, key, iv);
                if (decripted != null)
                {
                    try
                    {
                        List<EntityClass> _entities = Helpers.FromByteArray<List<EntityClass>>(decripted);
                        _entities = null;
                        decripted = null;
                        return true;
                    }
                    catch (Exception)
                    {
                        ProcessLog(ProcessPrint.Critical, "Unable to decrypt database. Possible errors: Bad seed1, Bad seed2, tempered file.");
                        ProcessLog(ProcessPrint.Critical, "All previous data was deleted!");
                        Console.WriteLine();
                        File.Delete(dbPath);
                        entities = null;
                        return false;
                    }
                }
                return false;
            }
            return false;
        }

        public byte[] GetKey(string entity) {
            if (entities != null) {
                byte[] iv = GenIV();
                byte[] key = GenKey();
                bool validation = ValidateDecryption();

                if (!validation)
                    return null;
                byte[] decripted = DecryptData(entities, key, iv);
                List<EntityClass> _entities = Helpers.FromByteArray<List<EntityClass>>(decripted);
                decripted = null;
                iv = null;
                key = null;
                return _entities.First(e => e.EntityName == entity).Key;
            }
            return null;
        }

        /// <summary>
        /// Lists all entities on the TrustAgent database
        /// </summary>
        /// <param name="showKey">If set to <c>true</c> shows the keys for the entities.</param>
        public void PrintEntities(bool showKey) {
            if (entities != null) {
                byte[] iv = GenIV();
                byte[] key = GenKey();
                byte[] decripted = DecryptData(entities, key, iv);
                bool validation = ValidateDecryption();

                if (!validation) {
                    Console.WriteLine("");
                    ProcessLog(ProcessPrint.Info, "There are no entities on the database");
                    Console.WriteLine("");
                    return;
                }

                List<EntityClass> _entities = Helpers.FromByteArray<List<EntityClass>>(decripted);

                decripted = null;
                iv = null;
                key = null;

                Console.WriteLine("");
                List<(string, string)> cmds = new List<(string, string)>();
                foreach (EntityClass entity in _entities)
                {
                    (string, string) prt = showKey ? (entity.EntityName, Helpers.ByteArrayToString(entity.Key)) : (entity.EntityName, "** HIDDEN **");
                    cmds.Add(prt);
                }
                decripted = null;
                Console.WriteLine(cmds.ToStringTable(
                    new[] { "Entity", "Key" },
                    a => a.Item1, a => a.Item2));
                cmds = null;

            } else {
                Console.WriteLine("");
                ProcessLog(ProcessPrint.Info, "There are no entities on the database");
                Console.WriteLine("");
            }
        }

    }
}
