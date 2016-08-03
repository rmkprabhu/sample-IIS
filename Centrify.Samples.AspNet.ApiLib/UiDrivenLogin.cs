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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Centrify.Samples.AspNet.ApiLib
{
    public class UiDrivenLogin
    {
        public const string OneTimePassCode = "OTP";
        public const string OathPassCode = "OATH";
        public const string PhoneFactor = "PF";
        public const string Sms = "SMS";
        public const string Email = "EMAIL";
        public const string PasswordReset = "RESET";
        public const string SecurityQuestion = "SQ";

        public static string MechToDescription(dynamic mech)
        {
            string mechName = mech["Name"];

            try
            {
                return mech["PromptSelectMech"];
            }
            catch { /* Doesn't support this property */ }

            switch (mechName)
            {
                case "UP":
                    return "Password";
                case "SMS":
                    return string.Format("SMS to number ending in {0}", mech["PartialDeviceAddress"]);
                case "EMAIL":
                    return string.Format("Email to address ending with {0}", mech["PartialAddress"]);
                case "PF":
                    return string.Format("Phone call to number ending with {0}", mech["PartialPhoneNumber"]);
                case "OATH":
                    return string.Format("OATH compatible client");
                case "SQ":
                    return string.Format("Security Question");
                case "RESET":
                    return string.Format("New Password");
                default:
                    return mechName;
            }
        }

        // http://stackoverflow.com/questions/3404421/password-masking-console-application
        private static string ReadMaskedPassword()
        {
            ConsoleKeyInfo key;
            string password = null;

            do
            {
                // Read a character without echoing it
                key = Console.ReadKey(true);

                if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
                {
                    password += key.KeyChar;
                    Console.Write("*");
                }
                else
                {
                    if (key.Key == ConsoleKey.Backspace && password != null)
                    {
                        password = password.Substring(0, password.Length - 1);
                        Console.Write("\b \b");
                    }
                }
            } while (key.Key != ConsoleKey.Enter);

            Console.WriteLine();
            return password;
        }

        public static string MechToPrompt(dynamic mech)
        {
            string mechName = mech["Name"];
            try
            {
                return mech["PromptMechChosen"];
            }
            catch { /* Doesn't support this property */ }
            switch (mechName)
            {
                case "UP":
                    return "Password: ";
                case "SMS":
                    return string.Format("Enter the code sent via SMS to number ending in {0}: ", mech["PartialDeviceAddress"]);
                case "EMAIL":
                    return string.Format("Please click or open the link sent to the email to address ending with {0}.", mech["PartialAddress"]);
                case "PF":
                    return string.Format("Calling number ending with {0}, please follow the spoken prompt.", mech["PartialPhoneNumber"]);
                case "OATH":
                    return string.Format("Enter your current OATH code: ");
                case "SQ":
                    return string.Format("Enter the response to your secret question: ");
                case "RESET":
                    return string.Format("Enter a new password: ");
                default:
                    return mechName;
            }
        }

        public static Dictionary<string, dynamic> AdvanceForMech(RestClient client, string tenantId, string sessionId, bool persistantLogin, Dictionary<string, dynamic> mech = null, string mechDelimited = null, string mfaAnswer = null, bool answer = false, bool reset = false)
        {
            var mechObject = new Dictionary<string, string>();

            if (!reset)
            {
                if (mech == null)
                {
                    if (mechDelimited != null)
                    {
                        mechObject = mechDelimited.TrimEnd(';').Split(';').ToDictionary(item => item.Split('=')[0], item => item.Split('=')[1]);
                    }
                    else
                    {
                        return null;
                    }

                }
            }
            
            Dictionary<string, dynamic> advanceArgs = new Dictionary<string, dynamic>();
            advanceArgs["TenantId"] = tenantId;
            advanceArgs["SessionId"] = sessionId;
            advanceArgs["PersistentLogin"] = false;

            string answerType = "";

            if (!reset)
            {
                if (mechObject.Count > 0)
                {
                    advanceArgs["MechanismId"] = mechObject["MechanismId"];
                    answerType = mechObject["AnswerType"];
                }
                else if (mech.Count > 0)
                {
                    advanceArgs["MechanismId"] = mech["MechanismId"];
                    answerType = mech["AnswerType"];
                }

                if (answer)
                {
                    if (mfaAnswer == "PollOnce")
                    {
                        answerType = "PollOnce";
                    }
                    else
                    {
                        answerType = "Text";
                    }
                }
            }
            else
            {
                answerType = "Reset";
            }
     

            switch (answerType)
            {
                case "Reset":
                    {                        
                        advanceArgs["Action"] = "ForgotPassword";
                        Dictionary<string, dynamic> result = client.CallApi("/security/advanceauthentication", advanceArgs);

                        return result;
                    }
                //Text only response such as Password or Secret Question
                case "Text":
                    {
                        advanceArgs["Answer"] = mfaAnswer;
                        advanceArgs["Action"] = "Answer";
                        Dictionary<string, dynamic> result = client.CallApi("/security/advanceauthentication", advanceArgs);

                        return result;
                    }
                //Text and Poll response such as SMS
                case "StartTextOob":
                    {
                        // Start oob to get the mech activated. StartTextOob timer will continue to poll while waiting for user input
                        advanceArgs["Action"] = "StartOOB";
                        var result = client.CallApi("/security/advanceauthentication", advanceArgs);

                        return result;
                    }
                case "PollOnce":
                    {
                        // Poll one time and return. Used For StartTextOob Timer
                        advanceArgs["Action"] = "Poll";
                        Dictionary<string, dynamic> result = client.CallApi("/security/advanceauthentication", advanceArgs);

                        return result;
                    }
                case "StartOob":
                    // Pure out of band mech, simply poll until complete or fail                    
                    advanceArgs["Action"] = "StartOOB";
                    client.CallApi("/security/advanceauthentication", advanceArgs);

                    // Poll
                    advanceArgs["Action"] = "Poll";
                    Dictionary<string, dynamic> pollResult = new Dictionary<string, dynamic>();
                    do
                    {                       
                        pollResult = client.CallApi("/security/advanceauthentication", advanceArgs);
                        System.Threading.Thread.Sleep(1000);
                    } while (pollResult["success"] && pollResult["Result"]["Summary"] == "OobPending");
                    return pollResult;
            }
            return null;
        }

        // Performs an MFA login interactively using Console
        public static RestClient Authenticate(string podEndpoint, string userName)
        {
            RestClient client = new RestClient(podEndpoint);

            // /security/startauthentication api takes username and version:
            Dictionary<string, dynamic> args = new Dictionary<string, dynamic>();
            args["User"] = userName;
            args["Version"] = "1.0";
            Dictionary<string, dynamic> startResult = client.CallApi("/security/startauthentication", args);

            // First thing to check for is whether we should repeat the call against a more specific pod name (tenant specific url):
            if (startResult["success"] && startResult["Result"].ContainsKey("PodFqdn"))
            {
                client.Endpoint = string.Format("https://{0}", startResult["Result"]["PodFqdn"]);
                startResult = client.CallApi("/security/startauthentication", args);
            }

            // Get the session id to use in handshaking for MFA
            client.SessionId = startResult["Result"]["SessionId"];
            client.TenantId = startResult["Result"]["TenantId"];
            client.AllowPasswordReset = startResult["Result"]["ClientHints"]["AllowForgotPassword"];

            // Also get the collection of challenges we need to satisfy
            client.ChallengeCollection = startResult["Result"]["Challenges"];

            return client;
        }

        public static RestClient StartSocialAuth(string podEndpoint, string idpName, string postExtIdpAuthCallbackUrl)
        {
            RestClient client = new RestClient(podEndpoint);

            Dictionary<string, dynamic> args = new Dictionary<string, dynamic>();
            args["IdpName"] = idpName;
            args["PostExtIdpAuthCallbackUrl"] = postExtIdpAuthCallbackUrl;
            Dictionary<string, dynamic> startResult = client.CallApi("/Security/StartSocialAuthentication", args);

            if (startResult["success"])
            {
                client.SocialIdpRedirectUrl = startResult["Result"]["IdpRedirectUrl"];
                return client;
            }
            else
            {
                return null;
            }
        }

        public static Dictionary<string, dynamic> ContinueSocialAuth(RestClient client, string extIdpAuthChallengeState)
        {
            Dictionary<string, dynamic> args = new Dictionary<string, dynamic>();
            args["ExtIdpAuthChallengeState"] = extIdpAuthChallengeState;
            Dictionary<string, dynamic> startResult = client.CallApi("/Security/ResumeFromExtIdpAuth", args);

            if (startResult["success"])
            {
                return startResult;
            }
            else
            {
                return null;
            }
        }
    }
}
