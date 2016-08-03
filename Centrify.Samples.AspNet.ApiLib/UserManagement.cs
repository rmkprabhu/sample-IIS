/**
 * Copyright 2016 Centrify Corporation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 **/

using System;
using System.Collections.Generic;
using System.Net;

namespace Centrify.Samples.AspNet.ApiLib
{
    public class UserManagement
    {
        private RestClient m_restClient = null;

        public UserManagement(RestClient authenticatedClient)
        {
            m_restClient = authenticatedClient;
        }

        public UserManagement(string endpointBase, string bearerToken)
        {
            m_restClient = new RestClient(endpointBase);
            m_restClient.BearerToken = bearerToken;
        }

        public string BearerToken
        {
            get
            {
                if (m_restClient.BearerToken != null)
                {
                    return m_restClient.BearerToken;
                }
                else
                {
                    if (m_restClient.Cookies != null)
                    {
                        CookieCollection endpointCookies = m_restClient.Cookies.GetCookies(new Uri(m_restClient.Endpoint));
                        if (endpointCookies != null)
                        {
                            Cookie bearerCookie = endpointCookies[".ASPXAUTH"];
                            if (bearerCookie != null)
                            {
                                return bearerCookie.Value;
                            }
                        }
                    }
                }
                return null;
            }

            set
            {
                m_restClient.BearerToken = value;
            }
        }

        // Illustrates locking a CUS user via /cdirectoryservice/setuserstate
        public void LockUser(string userUuid)
        {
            Dictionary<string, dynamic> args = new Dictionary<string, dynamic>();
            args["ID"] = userUuid;
            args["state"] = "Locked";

            var result = m_restClient.CallApi("/cdirectoryservice/setuserstate", args);
            if (result["success"] != true)
            {
                Console.WriteLine("LockUser {0} failed: {1}", userUuid, result["Message"]);
                throw new ApplicationException(result["Message"]);
            }
        }

        // Illustrates unlocking a CUS user via /cdirectoryservice/setuserstate
        public void UnlockUser(string userUuid)
        {
            Dictionary<string, dynamic> args = new Dictionary<string, dynamic>();
            args["ID"] = userUuid;
            args["state"] = "None";

            var result = m_restClient.CallApi("/cdirectoryservice/setuserstate", args);
            if (result["success"] != true)
            {
                Console.WriteLine("UnlockUser {0} failed: {1}", userUuid, result["Message"]);
                throw new ApplicationException(result["Message"]);
            }
        }

        // Illustrates usage of /redrock/query to run queries
        public dynamic Query(string sql)
        {
            Dictionary<string, dynamic> args = new Dictionary<string, dynamic>();
            args["Script"] = sql;

            Dictionary<string, dynamic> queryArgs = new Dictionary<string, dynamic>();
            args["Args"] = queryArgs;

            queryArgs["PageNumber"] = 1;
            queryArgs["PageSize"] = 10000;
            queryArgs["Limit"] = 10000;
            queryArgs["Caching"] = -1;

            var result = m_restClient.CallApi("/redrock/query", args);
            if (result["success"] != true)
            {
                Console.WriteLine("Running query failed: {0}", result["Message"]);
                throw new ApplicationException(result["Message"]);
            }

            return result["Result"];
        }

        public Dictionary<string, dynamic> GetUser(string userUuid = null)
        {
            Dictionary<string, dynamic> args = new Dictionary<string, dynamic>();
            args["ID"] = userUuid;

            var result = m_restClient.CallApi("/cdirectoryservice/getuser", args);

            return result;
        }
        public Dictionary<string, dynamic> GetUserInfo(string userUuid = null)
        {
            Dictionary<string, dynamic> args = new Dictionary<string, dynamic>();
            args["ID"] = userUuid;

            var result = m_restClient.CallApi("/usermgmt/getuserinfo", args);

            return result;
        }
        public Dictionary<string, dynamic> GetAliasesForTenant()
        {
            Dictionary<string, dynamic> args = new Dictionary<string, dynamic>();
            var result = m_restClient.CallApi("/core/GetAliasesForTenant", args);

            return result;
        }

        public Dictionary<string, dynamic> ChangeUserPassword(string oldPassword, string newPassword)
        {
            Dictionary<string, dynamic> args = new Dictionary<string, dynamic>();
            args["oldPassword"] = oldPassword;
            args["newPassword"] = newPassword;
            var result = m_restClient.CallApi("/usermgmt/changeuserpassword ", args);

            return result;
        }

        // Illustrates usage of /cdirectoryservice/createuser to create a new CUS user, presumes
        //  username and mail are the same.  Return value is user's UUID
        public Dictionary<string, dynamic> CreateUser(CDUser user, bool passwordNeverExpires, bool forcePassChange, bool sendEmail, bool sendSMS, bool inEverybodyRole)
        {
            Dictionary<string, dynamic> createUserArgs = new Dictionary<string, dynamic>();
            createUserArgs["Name"] = user.Name;
            createUserArgs["DisplayName"] = user.DisplayName;
            createUserArgs["Mail"] = user.Mail;
            createUserArgs["Description"] = user.Description;
            createUserArgs["OfficeNumber"] = user.OfficeNumber;
            createUserArgs["MobileNumber"] = user.MobileNumber;
            createUserArgs["HomeNumber"] = user.HomeNumber;
            createUserArgs["PasswordNeverExpire"] = passwordNeverExpires;
            createUserArgs["Password"] = user.Password;
            createUserArgs["ForcePasswordChangeNext"] = forcePassChange;
            createUserArgs["SendEmailInvite"] = sendEmail;
            createUserArgs["SendSmsInvite"] = sendSMS;
            createUserArgs["InEverybodyRole"] = inEverybodyRole;

            var result = m_restClient.CallApi("/cdirectoryservice/createuser", createUserArgs);

            return result;
        }

        public Dictionary<string, dynamic> ChangeUser(CDUser user, bool inEverybodyRole)
        {
            Dictionary<string, dynamic> changeUserArgs = new Dictionary<string, dynamic>();
            changeUserArgs["ID"] = user.ID;
            changeUserArgs["Name"] = user.Name;
            changeUserArgs["DisplayName"] = user.DisplayName;
            changeUserArgs["Mail"] = user.Mail;
            changeUserArgs["Description"] = user.Description;
            changeUserArgs["OfficeNumber"] = user.OfficeNumber;
            changeUserArgs["MobileNumber"] = user.MobileNumber;
            changeUserArgs["HomeNumber"] = user.HomeNumber;
            changeUserArgs["InEverybodyRole"] = inEverybodyRole;

            var result = m_restClient.CallApi("/cdirectoryservice/changeuser", changeUserArgs);
            return result;
        }
    }
    public class CDUser
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Mail { get; set; }
        public string Description { get; set; }
        public string OfficeNumber { get; set; }
        public string MobileNumber { get; set; }
        public string HomeNumber { get; set; }
        public string Password { get; set; }
    }
}

