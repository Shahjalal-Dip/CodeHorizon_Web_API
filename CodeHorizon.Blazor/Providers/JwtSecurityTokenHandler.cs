namespace CodeHorizon.Blazor.Providers
{
    public class JwtSecurityTokenHandler
    {
        public JwtSecurityToken ReadJwtToken(string token)
        {
            return new JwtSecurityToken(token);
        }

    }
}
