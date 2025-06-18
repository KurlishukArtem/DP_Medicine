using Microsoft.EntityFrameworkCore;
using System;

namespace Medicine_DP.Config.Connect
{
    public class Config
    {
        public static readonly string connection = "server=127.0.0.1;uid=root;pwd=;database=Medicine;port=3306;";
        public static readonly MySqlServerVersion version = new MySqlServerVersion(new Version(8, 0, 11));
    }
}
