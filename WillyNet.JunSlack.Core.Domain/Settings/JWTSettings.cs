﻿using System;
using System.Collections.Generic;
using System.Text;

namespace WillyNet.JunSlack.Core.Domain.Settings
{
    public class JWTSettings
    {
        public string Key { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double DurationInMinutes { get; set; }
    }
}
