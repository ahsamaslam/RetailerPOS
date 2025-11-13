using System.Security.Cryptography;
using System.Text;

namespace AuthModule.API.Services
{
    public class TokenHasher
    {
        private readonly byte[] _hmacKey;
        public TokenHasher(string base64Key)
        {
            if (string.IsNullOrWhiteSpace(base64Key)) throw new ArgumentNullException(nameof(base64Key));
            _hmacKey = Convert.FromBase64String(base64Key);
        }


        public string Hash(string token)
        {
            using var hmac = new HMACSHA256(_hmacKey);
            var bytes = Encoding.UTF8.GetBytes(token);
            var hash = hmac.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
