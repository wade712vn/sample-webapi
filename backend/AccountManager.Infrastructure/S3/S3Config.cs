using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountManager.Infrastructure.S3
{
    public class S3Config
    {
        public string AwsAccessKey { get; set; }
        public string AwsSecretKey { get; set; }
        public string AwsRegion { get; set; }

        public string AwsBucket { get; set; }
    }
}
