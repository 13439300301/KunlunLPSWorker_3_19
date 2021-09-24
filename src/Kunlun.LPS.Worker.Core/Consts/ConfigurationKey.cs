using System;
using System.Collections.Generic;
using System.Text;

namespace Kunlun.LPS.Worker.Core.Consts
{
    public static class ConfigurationKey
    {
        public const string CONSUMERS_KEY = "Consumers";

        public const string JOBS_KEY = "Jobs";

        public const string ENABLE = "Enable";

        public const string CRON = "Cron";

        /// <summary>
        /// 字段重复异常Number
        /// </summary>
        public const int KEYREPEAT = 2601;

        /// <summary>
        /// 主键重复异常Number
        /// </summary>
        public const int FIELDREPEAT = 2627;

        public const string SFTP_CONF = "SFTPConfig";

        public const string HOST = "Host";

        public const string LOGINNAME = "LoginName";

        public const string PWD = "Pwd";

        public const string FTPStoredValueAccountHistoryConfig = "FTPStoredValueAccountHistoryConfig";
    }
}
