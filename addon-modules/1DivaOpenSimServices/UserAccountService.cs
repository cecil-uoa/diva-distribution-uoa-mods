/*
 * Copyright (c) Crista Lopes (aka Diva) and Marcus Kirsch (aka Marck). All rights reserved.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the OpenSimulator Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net;
using Nini.Config;

using OpenMetaverse;
using OpenSim.Data;
using OpenSim.Services.Interfaces;
using Diva.Data;

namespace Diva.OpenSimServices
{
    public class UserAccountService : OpenSim.Services.UserAccountService.UserAccountService, IUserAccountService
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string m_CastWarning = "[DivaData]: Invalid cast for UserAccount store. Diva.Data required for method {0}.";
        protected IUserMappingData m_Database2 = null;

        public UserAccountService(IConfigSource config)
            : base(config)
        {
            setupMappingDBConnection(config);
        }

        public bool DeleteAccount(UUID scopeID, UUID userID)
        {
            m_Database2.Delete("PrincipalID", userID.ToString());   // Delete the mapping along with the account
            return m_Database.Delete("PrincipalID", userID.ToString());
        }

        public List<UserAccount> GetActiveAccounts(UUID scopeID, string term, string excludeTerm)
        {
            List<UserAccount> activeAccounts = new List<UserAccount>();
            List<UserAccountWithMappingData> accountsWithMapping = new List<UserAccountWithMappingData>();
            try
            {
                UserAccountData[] accounts = ((Diva.Data.IUserAccountData)m_Database).GetActiveAccounts(scopeID, term, excludeTerm);
                foreach (UserAccountData d in accounts)
                    activeAccounts.Add(ToUserAccount(d));
            }
            catch (InvalidCastException)
            {
                m_log.WarnFormat(m_CastWarning, MethodBase.GetCurrentMethod().Name);
            }

            return activeAccounts;
        }

        public long GetActiveAccountsCount(UUID scopeID, string excludeTerm)
        {
            try
            {
                return ((Diva.Data.IUserAccountData)m_Database).GetActiveAccountsCount(scopeID, excludeTerm);
            }
            catch (InvalidCastException)
            {
                m_log.WarnFormat(m_CastWarning, MethodBase.GetCurrentMethod().Name);
            }

            return 0;
        }

        protected UserAccount ToUserAccount(UserAccountData d)
        {
            UserAccount account = new UserAccount(d.Data.ToDictionary(p => p.Key, p => (object)p.Value));
            account.PrincipalID = d.PrincipalID;
            account.ScopeID = d.ScopeID;
            account.FirstName = d.FirstName;
            account.LastName = d.LastName;

            return account;
        }

        public UserMappingData getUserMapping(string ConnectID)
        {
            UserMappingData[] d = m_Database2.Get(new string[] { "ConnectID" }, new string[] { ConnectID });
            if (d.Length < 1)
            {
                return null;
            }

            return d[0];
        }

        public UserMappingData getUserMapping(UUID PrincipalID)
        {
            UserMappingData[] d = m_Database2.Get(new string[] { "PrincipalID" }, new string[] { PrincipalID.ToString() });
            if (d.Length < 1)
            {
                return null;
            }

            return d[0];
        }

        public bool StoreUserMapping(UserMappingData data)
        {
            // Store the account mapping
            return m_Database2.Store(data);
        }

        private void setupMappingDBConnection(IConfigSource config)
        {
            string dllName = String.Empty;
            string connString = String.Empty;
            string realm = "useraccounts_connect_map";

            // Get connection string from orignal
            IConfig dbConfig = config.Configs["DatabaseService"];
            if (dbConfig != null)
            {
                connString = dbConfig.GetString("ConnectionString", String.Empty);
            }

            // Get dll plugin from WifiService section
            dbConfig = config.Configs["WifiService"];
            if (dbConfig != null)
            {
                dllName = dbConfig.GetString("StorageProvider", String.Empty);
            }

            if (dllName == String.Empty)
                throw new Exception("No StorageProvider configured");

            m_Database2 = LoadPlugin<IUserMappingData>(dllName, new Object[] { connString, realm });
            if (m_Database2 == null)
                throw new Exception("Could not find a storage interface in the given module");
        }
    }
}
