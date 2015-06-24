/*
 * Copyright 2014 Dominick Baier, Brock Allen
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using BrockAllen.MembershipReboot;
using BrockAllen.MembershipReboot.WebHost;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebHost.MR
{
    public class CustomConfig : MembershipRebootConfiguration<CustomUser>
    {
        public static readonly CustomConfig Config;

        static CustomConfig()
        {
            Config = new CustomConfig();
            //Config.PasswordHashingIterationCount = 10000;
            //Config.RequireAccountVerification = false;
            //config.EmailIsUsername = true;
            Config.RequireAccountVerification = true;

            var delivery = new SmtpMessageDelivery();

            var appinfo = new AspNetApplicationInformation("SLEEK Portal", "SLEEK Auth System",
                "UserAccount/Login",
                "UserAccount/ChangeEmail/Confirm/",
                "UserAccount/Register/Cancel/",
                "UserAccount/PasswordReset/Confirm/");
            var formatter = new CustomEmailMessageFormatter(appinfo);

            Config.AddEventHandler(new EmailAccountEventsHandler<CustomUser>(formatter, delivery));

            /*
            var appinfo = new AspNetApplicationInformation("Auth System", "Auth System Email Signature",
                "Permissions", //"UserAccount/Login",
                "UserAccount/ChangeEmail/Confirm/",
                "", //"UserAccount/Register/Cancel/",
                "UserAccount/PasswordReset/Confirm/");
            var emailFormatter = new EmailMessageFormatter<CustomUser>(appinfo);
            // uncomment if you want email notifications -- also update smtp settings in web.config            
            Config.AddEventHandler(new EmailAccountEventsHandler<CustomUser>(emailFormatter));
            */
        }
    }

    //public class CustomConfig
    //{               

    //    public static MembershipRebootConfiguration<CustomUser> Create()
    //    {
    //        var settings = SecuritySettings.Instance;
                        
    //        var config = new MembershipRebootConfiguration<CustomUser>(settings);
          
    //        var delivery = new SmtpMessageDelivery();

    //        var appinfo = new AspNetApplicationInformation("Test", "Test Email Signature",
    //            "UserAccount/Login",
    //            "UserAccount/ChangeEmail/Confirm/",
    //            "UserAccount/Register/Cancel/",
    //            "UserAccount/PasswordReset/Confirm/");
    //        var formatter = new CustomEmailMessageFormatter(appinfo);

    //        config.AddEventHandler(new EmailAccountEventsHandler<CustomUser>(formatter, delivery));
    //        return config;
    //    }
    //}
}