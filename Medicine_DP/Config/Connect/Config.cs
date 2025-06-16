using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace Medicine_DP.Config.Connect
{
    public class Config
    {
        public static readonly string connection = "server=127.0.0.1;uid=root;pwd=;database=Medicine;port=3307;";
        public static readonly MySqlServerVersion version = new MySqlServerVersion(new Version (8,0,11));
    }
}
