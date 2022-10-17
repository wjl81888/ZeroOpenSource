using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json;
using Microsoft.Net.Http.Headers;
using System.Net.Http.Headers;

namespace Tencent.TencentCloud
{
    public class TencentCloudAuthHandler : DelegatingHandler
    {
        private readonly string[] CommonParameters = new string[] { "Action", "Region", "Version", "Timestamp", "Authorization", "Token" };
        private readonly string Algorithm = "TC3-HMAC-SHA256";

        private readonly TencentCloudOptions _tencentCloudOptions;

        public TencentCloudAuthHandler(IOptions<TencentCloudOptions> tencentCloudOptionsAccessor)
        {
            _tencentCloudOptions = tencentCloudOptionsAccessor.Value;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string canonicalQueryString = BuildCanonicalQueryString(request);
            string requestPayload = BuildRequestPayload(request);
            string contentType = BuildContentType(request);

            var headers = BuildHeaders(request, contentType, requestPayload, canonicalQueryString);

            foreach (KeyValuePair<string, string> kvp in headers)
            {
                if (kvp.Key.Equals(HeaderNames.ContentType))
                {
                    request.Content = new StringContent(requestPayload, Encoding.UTF8, kvp.Value);
                }
                else if (kvp.Key.Equals(HeaderNames.Host))
                {
                    request.Headers.Host = kvp.Value;
                }
                else if (kvp.Key.Equals(HeaderNames.Authorization))
                {
                    string algorithm = "TC3-HMAC-SHA256";
                    request.Headers.Authorization = new AuthenticationHeaderValue(algorithm, kvp.Value.Substring(algorithm.Length));
                }
                else
                {
                    request.Headers.Add(kvp.Key, kvp.Value);
                }
            }

            string fullUrl = $"{request?.RequestUri?.Scheme}://{request?.RequestUri?.Host}";

            if (request?.Method == HttpMethod.Get)
            {
                fullUrl += $"?{canonicalQueryString}";
            }

            request!.RequestUri = new Uri(fullUrl);

            return await base.SendAsync(request, cancellationToken);
        }

        [Obsolete]
        private string BuildCanonicalQueryString(HttpRequestMessage request)
        {
            if (request.Method == HttpMethod.Get)
            {
                var parameters = request.Properties.Where(p => !CommonParameters.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);

                StringBuilder urlBuilder = new StringBuilder();

                foreach (var kvp in parameters)
                {
                    urlBuilder.Append($"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value.ToString())}&");
                }
                return urlBuilder.ToString().TrimEnd('&');
            }

            return string.Empty;
        }

        [Obsolete]
        private string BuildRequestPayload(HttpRequestMessage request)
        {
            if (request.Method == HttpMethod.Post)
            {
                var parameters = request.Properties.Where(p => !CommonParameters.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);

                var jsonOptions = new JsonSerializerOptions { IgnoreNullValues = true, WriteIndented = false };
                return JsonSerializer.Serialize(parameters, jsonOptions);
            }

            return string.Empty;
        }

        private string BuildContentType(HttpRequestMessage request)
        {
            if (request.Method == HttpMethod.Get)
            {
                return "application/x-www-form-urlencoded";
            }
            else
            {
                return System.Net.Mime.MediaTypeNames.Application.Json;
            }
        }

        private Dictionary<string, string> BuildHeaders(HttpRequestMessage request, string contentType, string requestPayload, string canonicalQueryString)
        {
            string endpoint = request.RequestUri.Host;

            string httpRequestMethod = request.Method.Method;
            string canonicalUri = "/";
            string canonicalHeaders = $"content-type:{contentType}; charset=utf-8\nhost:{endpoint}\n";

            string signedHeaders = "content-type;host";
            string hashedRequestPayload = Sha256Hex(requestPayload);
            string canonicalRequest = $"{httpRequestMethod}\n{canonicalUri}\n{canonicalQueryString}\n{canonicalHeaders}\n{signedHeaders}\n{hashedRequestPayload}";

            long timestamp = DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000;
            string requestTimestamp = timestamp.ToString();
            request.Properties.Add("Timestamp", requestTimestamp);
            string date = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(timestamp).ToString("yyyy-MM-dd");
            string service = endpoint.Split('.').First();
            string credentialScope = $"{date}/{service}/tc3_request";
            string hashedCanonicalRequest = Sha256Hex(canonicalRequest);

            string stringToSign = $"{Algorithm}\n{requestTimestamp}\n{credentialScope}\n{hashedCanonicalRequest}";

            byte[] tc3SecretKey = Encoding.UTF8.GetBytes("TC3" + _tencentCloudOptions.SecretKey);
            byte[] secretDate = HmacSha256(tc3SecretKey, Encoding.UTF8.GetBytes(date));
            byte[] secretService = HmacSha256(secretDate, Encoding.UTF8.GetBytes(service));
            byte[] secretSigning = HmacSha256(secretService, Encoding.UTF8.GetBytes("tc3_request"));
            byte[] signatureBytes = HmacSha256(secretSigning, Encoding.UTF8.GetBytes(stringToSign));
            string signature = BitConverter.ToString(signatureBytes).Replace("-", "").ToLower();

            string authorization = $"{Algorithm} Credential={_tencentCloudOptions.SecretId}/{credentialScope}, SignedHeaders={signedHeaders}, Signature={signature}";

            var headers = new Dictionary<string, string>
            {
                { HeaderNames.Host, endpoint },
                { HeaderNames.ContentType, contentType },
                { HeaderNames.Authorization, authorization }
            };
            var parameters = request.Properties.Where(p => CommonParameters.Contains(p.Key)).ToDictionary(p => p.Key, p => p.Value);

            foreach (var parameter in parameters)
            {
                headers.Add($"X-TC-{parameter.Key}", parameter.Value?.ToString());
            }

            return headers;
        }

        private string Sha256Hex(string str)
        {
            using SHA256 sha256 = SHA256.Create();

            byte[] hashbytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hashbytes.Length; ++i)
            {
                builder.Append(hashbytes[i].ToString("x2"));
            }
            return builder.ToString();
        }

        private byte[] HmacSha256(byte[] key, byte[] buffer)
        {
            using (HMACSHA256 hmacSha256 = new HMACSHA256(key))
            {
                return hmacSha256.ComputeHash(buffer);
            }
        }
    }
}