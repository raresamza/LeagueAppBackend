using WebApi.Options;
using Microsoft.Extensions.Options;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Runtime;
namespace WebApi.Services;

public class IdentityService
{
    private readonly JwtOptions? _jwtOptions;
    private readonly byte[] _key;


    public IdentityService(IOptions<JwtOptions?> options)
    {
        _jwtOptions = options.Value;
        ArgumentNullException.ThrowIfNull(_jwtOptions);
        ArgumentNullException.ThrowIfNull(_jwtOptions.SigninKey);
        ArgumentNullException.ThrowIfNull(_jwtOptions.Issuer);
        ArgumentNullException.ThrowIfNull(_jwtOptions.Audiences);
        ArgumentNullException.ThrowIfNull(_jwtOptions.Audiences[0]);
        _key = Encoding.ASCII.GetBytes(_jwtOptions?.SigninKey!);
    }

    public static JwtSecurityTokenHandler TokenHandler => new();

    public SecurityToken CreateSecurityToken(ClaimsIdentity identity)
    {
        var tokenDescriptor = GetTokenDescriptor(identity);

        return TokenHandler.CreateToken(tokenDescriptor);
    }

    public string WriteToken(SecurityToken token)
    {
        return TokenHandler.WriteToken(token);
    }

    private SecurityTokenDescriptor GetTokenDescriptor(ClaimsIdentity identity)
    {
        return new SecurityTokenDescriptor()
        {
            Subject = identity,
            Expires = DateTime.Now.AddHours(2),
            Audience = _jwtOptions!.Audiences?[0]!,
            Issuer = _jwtOptions.Issuer,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_key),SecurityAlgorithms.HmacSha256Signature)
        };
    }
}
