using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace CodeHorizon.Blazor.Providers
{
    public class JwtSecurityToken
    {
        private readonly string _token;

        public JwtSecurityToken(string token)
        {
            _token = token;
            Claims = ParseClaimsFromToken(token);
        }

        public IEnumerable<Claim> Claims { get; }

        private IEnumerable<Claim> ParseClaimsFromToken(string token)
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return new List<Claim>();

            var payload = parts[1];
            var jsonBytes = Base64UrlDecode(payload);
            var jsonString = Encoding.UTF8.GetString(jsonBytes);

            using var doc = JsonDocument.Parse(jsonString);
            var claims = new List<Claim>();

            foreach (var property in doc.RootElement.EnumerateObject())
            {
                switch (property.Value.ValueKind)
                {
                    case JsonValueKind.String:
                        claims.Add(new Claim(property.Name, property.Value.GetString() ?? string.Empty));
                        break;
                    case JsonValueKind.Array:
                        var array = property.Value.EnumerateArray().Select(x => x.GetString()).Where(x => x != null);
                        foreach (var item in array)
                        {
                            claims.Add(new Claim(property.Name, item ?? string.Empty));
                        }
                        break;
                    default:
                        claims.Add(new Claim(property.Name, property.Value.ToString()));
                        break;
                }
            }

            return claims;
        }

        private byte[] Base64UrlDecode(string input)
        {
            var output = input;
            output = output.Replace('-', '+').Replace('_', '/');
            switch (output.Length % 4)
            {
                case 2: output += "=="; break;
                case 3: output += "="; break;
            }
            return Convert.FromBase64String(output);
        }
    }
}
