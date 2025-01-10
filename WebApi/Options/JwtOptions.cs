namespace WebApi.Options;

public class JwtOptions
{
    public string? SigninKey { get; set; }
    public string? Issuer { get; set; }
    public string[]? Audiences { get; set; }
}
