using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Speech.Google
{
    public class GoogleAuthManager
    {
        private readonly string _clientEmail;
        private readonly string _privateKey;
        private readonly string _tokenUri;
        private string? _accessToken;
        private DateTime _tokenExpiry;

        public GoogleAuthManager(string credentialPath)
        {
            var json = File.ReadAllText(credentialPath);
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            _clientEmail = root.GetProperty("client_email").GetString() 
                ?? throw new InvalidOperationException("client_email not found in credentials");
            _privateKey = root.GetProperty("private_key").GetString() 
                ?? throw new InvalidOperationException("private_key not found in credentials");
            _tokenUri = root.TryGetProperty("token_uri", out var tokenUri) 
                ? tokenUri.GetString() ?? "https://oauth2.googleapis.com/token"
                : "https://oauth2.googleapis.com/token";
        }

        public async Task<string> GetAccessTokenAsync()
        {
            if (_accessToken != null && DateTime.UtcNow < _tokenExpiry.AddMinutes(-5))
            {
                return _accessToken;
            }

            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var exp = now + 3600; // 1 hour

            // Build JWT header
            var header = new { alg = "RS256", typ = "JWT" };
            var headerJson = JsonSerializer.Serialize(header);
            var headerBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));

            // Build JWT payload
            var payload = new
            {
                iss = _clientEmail,
                sub = _clientEmail,
                aud = _tokenUri,
                iat = now,
                exp = exp,
                scope = "https://www.googleapis.com/auth/cloud-platform"
            };
            var payloadJson = JsonSerializer.Serialize(payload);
            var payloadBase64 = Base64UrlEncode(Encoding.UTF8.GetBytes(payloadJson));

            // Sign
            var dataToSign = $"{headerBase64}.{payloadBase64}";
            var signature = SignData(dataToSign);
            var signatureBase64 = Base64UrlEncode(signature);

            var jwt = $"{dataToSign}.{signatureBase64}";

            // Exchange JWT for access token
            using var client = new HttpClient();
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["grant_type"] = "urn:ietf:params:oauth:grant-type:jwt-bearer",
                ["assertion"] = jwt
            });

            var response = await client.PostAsync(_tokenUri, content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Token exchange failed: {response.StatusCode} - {responseBody}");
            }

            using var responseDoc = JsonDocument.Parse(responseBody);
            _accessToken = responseDoc.RootElement.GetProperty("access_token").GetString();
            _tokenExpiry = DateTime.UtcNow.AddSeconds(3600);

            return _accessToken!;
        }

        private byte[] SignData(string data)
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(_privateKey);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            return rsa.SignData(dataBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        }

        private static string Base64UrlEncode(byte[] data)
        {
            return Convert.ToBase64String(data)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }
    }
}
