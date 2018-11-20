/*
 * TrustAgent.Database.cs 
 * Developer: Pedro Cavaleiro
 * Developement stage: Development
 * Tested on: macOS Mojave (10.14.1) -> PASSED
 * 
 * Manages the Trust Agent database
 * 
 * Requires initialization: YES
 * Contains:
 *     Class Level Variables: 3 Private (Read Only), 3 Public
 *     Enums: 3 Public
 *     Methods:
 *         Non Static: 2 Private, 6 Public
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using static TrustAgent.AESCipher;

namespace TrustAgent
{
    public class Database
    {

        public byte[] entities;
        public int selector = 0;
        public bool fileExists;

        readonly string dbPath;
        readonly int seed1;
        readonly int seed2;

        public Database(int seed1, int seed2, string dbPath)
        {
            this.seed1 = seed1;
            this.seed2 = seed2;
            this.dbPath = dbPath;
            if (File.Exists(dbPath))
            {
                fileExists = true;
                selector = LoadSelector();
                LoadFile();
            }
        }

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
                fs.Read(_selector, 0, 4);
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
                byte[] db = new byte[fs.Length];
                fs.Read(db, 0, db.Length);
                entities = new byte[db.Length - 4];
                Array.Copy(db, 4, entities, 0, db.Length - 4);
                fs.Close();
            }
        }

        /// <summary>
        /// Writes to file any changes on the database and updates the selector
        /// </summary>
        public void WriteToFile()
        {

            if (entities != null)
            {
                byte[] key = GenKey(seed1, selector);
                byte[] iv = GenIV(seed2, selector);
                bool dbValidation = ValidateDecryption(entities, key, iv);
                if (dbValidation)
                {
                    byte[] decrypted = DecryptData(entities, key, iv);
                    if (selector + 1 > 50)
                        selector = -1;
                    selector++;
                    byte[] key1 = GenKey(seed1, selector);
                    byte[] iv1 = GenIV(seed2, selector);
                    entities = EncryptData(decrypted, key1, iv1);
                    byte[] selectorBytes = BitConverter.GetBytes(selector);
                    byte[] toWrite = new byte[4 + entities.Length];
                    Array.Copy(selectorBytes, toWrite, 4);
                    Array.Copy(entities, 0, toWrite, 4, entities.Length);
                    using (FileStream fs = new FileStream(dbPath, FileMode.OpenOrCreate, FileAccess.Write))
                    {
                        fs.Write(toWrite, 0, toWrite.Length);
                        fs.Close();
                    }
                }
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
            byte[] key = GenKey(seed1, selector);
            byte[] iv = GenIV(seed2, selector);
            byte[] decripted;
            List<EntityClass> _entities;
            if (entities != null)
            {
                bool dbValidation = ValidateDecryption(entities, key, iv);
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
                        return AddEntityError.Exists;
                    }
                }
                else
                {
                    _entities = new List<EntityClass>();
                }
            }
            else
            {
                _entities = new List<EntityClass>();
            }

            byte[] sharedKeyBytes = sharedKey.FromHexToByteArray();

            _entities.Add(new EntityClass { EntityName = entity, Key = sharedKeyBytes });

            decripted = Helpers.ToByteArray(_entities);

            entities = EncryptData(decripted, key, iv);

            decripted = null;
            iv = null;
            key = null;

            return AddEntityError.NoError;
        }

        /// <summary>
        /// Deletes an entity from the TrustAgent database
        /// </summary>
        /// <returns>An type of error (if no error ocurrs returns DelEntityError.noError</returns>
        /// <param name="entity">Entity name.</param>
        public DelEntityError DelEntity(string entity)
        {
            byte[] key = GenKey(seed1, selector);
            byte[] iv = GenIV(seed2, selector);
            byte[] decripted;
            List<EntityClass> _entities;
            if (entities != null)
            {
                bool dbValidation = ValidateDecryption(entities, key, iv);
                if (!dbValidation)
                    return DelEntityError.NotFound;

                decripted = DecryptData(entities, key, iv);
                _entities = Helpers.FromByteArray<List<EntityClass>>(decripted);
                decripted = null;

                if (_entities.Any(m => m.EntityName == entity))
                {
                    _entities.Remove(_entities.First(m => m.EntityName == entity));
                    decripted = Helpers.ToByteArray(_entities);
                    entities = EncryptData(decripted, key, iv);
                    decripted = null;
                    return DelEntityError.NoError;
                }

                iv = null;
                key = null;
                _entities = null;
                return DelEntityError.NotFound;
            }
            iv = null;
            key = null;
            return DelEntityError.NotFound;
        }

        /// <summary>
        /// Received network packet (contains entity name, message, 
        /// </summary>
        /// <returns>The entity.</returns>
        /// <param name="packet">Packet.</param>
        public ValidationError ValidateEntity(ClientMessage packet, byte[] hmac, byte[] packetRaw)
        {
            byte[] key = GenKey(seed1, selector);
            byte[] iv = GenIV(seed2, selector);

            if (entities == null)
                return ValidationError.NotFound;

            bool dbValidation = ValidateDecryption(entities, key, iv);

            if (!dbValidation)
                return ValidationError.NotFound;

            if (Program.server.clientHandlers.Contains(packet.Entity))
                return ValidationError.AlreadyConnected;

            byte[] decripted = DecryptData(entities, key, iv);

            List<EntityClass> _entities = Helpers.FromByteArray<List<EntityClass>>(decripted);
            decripted = null;

            if (_entities.Any(m => m.EntityName == packet.Entity))
            {
                EntityClass storedEntity = _entities.First(m => m.EntityName == packet.Entity);
                byte[] calcHMAC = SHA256hmac.ComputeHMAC(packetRaw, storedEntity.Key);
                return !SHA256hmac.CompareHMAC(hmac, calcHMAC) ? ValidationError.InvalidKey : ValidationError.NoError;
                //return (Helpers.ByteArrayToString(hmac) == Helpers.ByteArrayToString(calcHMAC)) ? ValidationError.NoError : ValidationError.InvalidKey;
            }
            _entities = null;
            return ValidationError.NotFound;
        }

        /// <summary>
        /// Retreives the entity key.
        /// </summary>
        /// <returns>The entity key.</returns>
        /// <param name="entity">Entity.</param>
        public byte[] RetreiveEntityKey(string entity) {
            byte[] key = GenKey(seed1, selector);
            byte[] iv = GenIV(seed2, selector);

            if (entities == null)
                return null;

            bool dbValidation = ValidateDecryption(entities, key, iv);

            if (!dbValidation)
                return null;

            byte[] decripted = DecryptData(entities, key, iv);

            List<EntityClass> _entities = Helpers.FromByteArray<List<EntityClass>>(decripted);
            decripted = null;

            if (_entities.Any(m => m.EntityName == entity))
            {
                EntityClass storedEntity = _entities.First(m => m.EntityName == entity);
                return storedEntity.Key;
            }
            _entities = null;
            return null;
        }

        #endregion

        #region "Enums"

        public enum AddEntityError {
            Exists, InvalidKey, NoError
        } 

        public enum DelEntityError {
            NotFound, NoError
        }

        public enum ValidationError {
            NotFound, InvalidKey, NoError, AlreadyConnected
        }

        #endregion

        /// <summary>
        /// Prints all the entities
        /// </summary>
        /// <param name="ShowKey">If set to <c>true</c> show key.</param>
        public void PrintEntities(bool ShowKey) {
            if (entities != null) {
                byte[] key = GenKey(seed1, selector);
                byte[] iv = GenIV(seed2, selector);
                bool dbValidation = ValidateDecryption(entities, key, iv);

                bool validation = ValidateDecryption(entities, key, iv);

                if (!validation)
                {
                    Console.WriteLine("");
                    StandardPrints.ProcessLog(StandardPrints.ProcessPrint.Info, "There are no entities on the database");
                    Console.WriteLine("");
                    return;
                }

                byte[] decripted = DecryptData(entities, key, iv);

                List<EntityClass> _entities = Helpers.FromByteArray<List<EntityClass>>(decripted);

                decripted = null;
                iv = null;
                key = null;

                Console.WriteLine("");
                List<(string, string)> cmds = new List<(string, string)>();
                foreach (EntityClass entity in _entities)
                {
                    (string, string) prt = ShowKey ? (entity.EntityName, entity.Key.FromByteArrayToHex()) : (entity.EntityName, "** HIDDEN **");
                    cmds.Add(prt);
                }
                decripted = null;
                Console.WriteLine(cmds.ToStringTable(
                    new[] { "Entity", "Key" },
                    a => a.Item1, a => a.Item2));
                cmds = null;

            } else {
                Console.WriteLine("");
                StandardPrints.ProcessLog(StandardPrints.ProcessPrint.Info, "There are no entities on the database");
                Console.WriteLine("");
            }
        }

    }
}
