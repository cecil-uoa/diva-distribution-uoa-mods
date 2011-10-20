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
using System.Collections;
using System.Collections.Generic;
using System.Data;
using OpenMetaverse;
using OpenSim.Framework;
using MySql.Data.MySqlClient;
using OpenSim.Data.MySQL;

namespace Diva.Data.MySQL
{
    public class MySqlUserMappingData : MySQLGenericTableHandler<UserMappingData>, IUserMappingData
    {
        public MySqlUserMappingData(string connectionString, string realm)
            : base(connectionString, realm, "useraccounts_Connect_Map")
        {
        }

        public UserMappingData[] GetUsers(UUID scopeID, string query)
        {
            string[] words = query.Split(new char[] {' '});

            // Remove empty sections
            for (int i = 0 ; i < words.Length ; i++)
            {
                if (words[i].Length < 3)
                {
                    if (i != words.Length - 1)
                        Array.Copy(words, i + 1, words, i, words.Length - i - 1);
                    Array.Resize(ref words, words.Length - 1);
                }
            }

            if (words.Length != 2)
            {
                return new UserMappingData[0];
            }

            MySqlCommand cmd = new MySqlCommand();

            if (words[0] == "ConnectID")
            {
                cmd.CommandText = String.Format("select * from {0} where ConnectID=?ConnectID", m_Realm);
                cmd.Parameters.AddWithValue("?ConnectID", words[1]);
            }
            else
            {
                cmd.CommandText = String.Format("select * from {0} where PrincipalID=?PrincipalID", m_Realm);
                cmd.Parameters.AddWithValue("?PrincipalID", words[1]);
            }

            return DoQuery(cmd);
        }
    }
}
