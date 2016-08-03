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
    public class ApplicationManagement
    {
        private RestClient m_restClient = null;

        public ApplicationManagement(RestClient authenticatedClient)
        {
            m_restClient = authenticatedClient;
        }

        public ApplicationManagement(string endpointBase, string bearerToken)
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

        // Illustrates getting a users applications using GetUPData
        public dynamic GetUPData()
        {
            Dictionary<string, dynamic> args = new Dictionary<string, dynamic>();
            args["force"] = "True";

            var result = m_restClient.CallApi("/uprest/GetUPData", args);
            if (result["success"] != true)
            {
                return null;
            }
            else
            {
                return result["Result"];
            }
        }        
    }
}

