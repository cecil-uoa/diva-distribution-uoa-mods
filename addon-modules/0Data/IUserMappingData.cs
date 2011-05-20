/*
 * Copyright (c) Contributors, http://opensimulator.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
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
using OpenMetaverse;
using OpenSim.Framework;
using OpenSim.Services.Interfaces;

namespace Diva.Data
{
    public class UserAccountWithMappingData
    {
        public string FirstName;
        public string LastName;
        public string Email;
        public UUID PrincipalID;
        public UUID ScopeID;
        public int UserLevel;
        public int UserFlags;
        public string UserTitle;

        public string ConnectID;
        public string RealFirstName;
        public string RealLastName;
        public string Institution;

        public Dictionary<string, object> ServiceURLs;

        public int Created;

        public string Name
        {
            get { return FirstName + " " + LastName; }
        }

        public UserAccountWithMappingData(UserAccount accountData, UserMappingData mappingData)
        {
            PrincipalID = accountData.PrincipalID;
            FirstName = accountData.FirstName;
            LastName = accountData.LastName;
            Email = accountData.Email;
            ScopeID = accountData.ScopeID;
            UserLevel = accountData.UserLevel;
            UserFlags = accountData.UserFlags;
            UserTitle = accountData.UserTitle;

            ServiceURLs = accountData.ServiceURLs;
            Created = accountData.Created;

            if (mappingData != null)
            {
                RealFirstName = mappingData.RealFirstName;
                RealLastName = mappingData.RealLastName;
                Institution = mappingData.Institution;
                ConnectID = mappingData.ConnectID;
            }
            else
            {
                RealFirstName = string.Empty;
                RealLastName = string.Empty;
                Institution = string.Empty;
                ConnectID = string.Empty;
            }
        }
    }

    public class UserMappingData
    {
        public string ConnectID;
        public UUID PrincipalID;
        public string RealFirstName;
        public string RealLastName;
        public string Institution;
    }

    /// <summary>
    /// An interface for connecting to the user accounts datastore
    /// </summary>
    public interface IUserMappingData
    {
        UserMappingData[] Get(string[] fields, string[] values);
        bool Store(UserMappingData data);
        bool Delete(string field, string val);
        UserMappingData[] GetUsers(UUID scopeID, string query);
    }
}
