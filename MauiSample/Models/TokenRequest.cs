﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiSample.Models
{
    public class TokenRequest
    {
        public bool Enabled { get; set; }
        public int ExpirationDays { get; set; }
        public string Name { get; set; }
    }
}
