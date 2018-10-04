using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;

namespace CSharpLambdaFunction
{
    public class LambdaHandler
    {
        public static void Main()
        {
            Task.Run(async () =>
            {
                using (var stream = new FileStream($"{Directory.GetCurrentDirectory()}\\testData.json", FileMode.Open))
                {
                    await new LambdaHandler().LogHandler(stream);
                }
            }).GetAwaiter().GetResult();
        }

        public async Task LogHandler(Stream inputStream)
        {
            LogEntry entry;
            using (var reader = new StreamReader(inputStream))
            {
                entry = Deserialize(reader);
            }
            var reqTask = SendToS3AndGetLink(entry.Request);
            var resTask = SendToS3AndGetLink(entry.Response);

            entry.Request = await reqTask;
            entry.Response = await resTask;

            return ConvertToStream(entry);
        }

        private LogEntry Deserialize(TextReader reader)
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();

            return (LogEntry)serializer.Deserialize(reader, typeof(LogEntry));
        }

        private Stream ConvertToStream(LogEntry logEntry)
        {
            var serializer = new Newtonsoft.Json.JsonSerializer();

            var memStream = new MemoryStream();
            var writer = new StreamWriter(memStream);
            serializer.Serialize(writer, logEntry);

            writer.Flush();

            return memStream;
        }

        private Stream ConvertToStream(string value)
        {
            var memStream = new MemoryStream();
            var writer = new StreamWriter(memStream);

            writer.Write(value);
            writer.Flush();

            return memStream;
        }

        private async Task<string> SendToS3AndGetLink(string value)
        {
            var s3 = new AmazonS3Client(Amazon.RegionEndpoint.USWest2); //define your own region here

            var putRequest = new PutObjectRequest();

            var key = Guid.NewGuid().ToString().Replace("-", string.Empty);
            putRequest.BucketName = GetBucketName();
            putRequest.Key = key;
            putRequest.InputStream = ConvertToStream(value);
            putRequest.ContentType = "application/json";

            var response = await s3.PutObjectAsync(putRequest);

            var urlRequest = new GetPreSignedUrlRequest();
            urlRequest.BucketName = GetBucketName();
            urlRequest.Expires = DateTime.UtcNow.AddYears(2);
            urlRequest.Key = key;

            return s3.GetPreSignedURL(urlRequest);
        }

        private string GetBucketName()
        {
            return "cloudncode-logs";
        }
    }

    public class LogEntry
    {
        public string RequestId { get; set; }
        public string SessionId { get; set; }
        public string Timestamp { get; set; }
        public string UserId { get; set; }
        public string ServerIp { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }
        public string Category { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
    }
}