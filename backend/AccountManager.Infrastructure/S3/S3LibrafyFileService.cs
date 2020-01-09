using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountManager.Application.Services;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;

namespace AccountManager.Infrastructure.S3
{
    public class S3LibrafyFileService : ILibraryFileService
    {
        private readonly IAmazonS3 _client;
        private readonly S3Config _config;

        public S3LibrafyFileService(S3Config config)
        {
            _config = config;
            _client = new AmazonS3Client(new BasicAWSCredentials(config.AwsAccessKey, config.AwsSecretKey), RegionEndpoint.GetBySystemName(config.AwsRegion));
        }

        public async Task<bool> FileExists(string fileUrl)
        {
            var bucketName = _config.AwsBucket;
            var key = fileUrl.Substring($"https://s3.amazonaws.com/{bucketName}/ ".Length);

            var request = new GetObjectMetadataRequest()
            {
                BucketName = bucketName,
                Key = key,
            };

            try
            {
                var response = await _client.GetObjectMetadataAsync(request);
                return true;
            }
            catch (AmazonS3Exception e)
            {
                var errorCode = e.ErrorCode;
                if (errorCode == "NotFound")
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
