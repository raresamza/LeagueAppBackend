using System.Data;
using System.Security.Claims;
using InfrastructureLayer.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Services;
using System.IdentityModel.Tokens.Jwt;
namespace WebApi.Controllers;


public enum Role
{
    Admin,
    User
}
[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IdentityService _identityService;
    private readonly AppDbContext _appDbContext;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(RoleManager<IdentityRole> roleManager, SignInManager<IdentityUser> signInManager, IdentityService identityService, AppDbContext appDbContext, UserManager<IdentityUser> userManager)
    {
        _appDbContext = appDbContext;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _identityService = identityService;
        _userManager = userManager;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login(LoginUser login)
    {
        var user = await _userManager.FindByNameAsync(login.Email);
        Console.WriteLine(user.UserName);
        if (user is null) return BadRequest();

        var result = await _signInManager.CheckPasswordSignInAsync(user, login.Password, false);
        if (!result.Succeeded) return BadRequest();

        var roles = await _userManager.GetRolesAsync(user);

        var claims = await _userManager.GetClaimsAsync(user);

        var claimsIdentity = new ClaimsIdentity(new Claim[]
        {
            new(JwtRegisteredClaimNames.Sub, user.Email ?? throw new InvalidOperationException()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? throw new InvalidOperationException()),
            new("UserId", user.Id ?? throw new InvalidOperationException()),
        });

        claimsIdentity.AddClaims(claims);
        foreach (var role in roles)
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
        }
        var token = _identityService.CreateSecurityToken(claimsIdentity);
        var response = new AuthenticationResult(_identityService.WriteToken(token));

        return Ok(response);
    }


    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUser register)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var identity = new IdentityUser
        {
            Email = register.Email,
            UserName = register.Name,
            PhoneNumber = register.PhoneNumber,
        };

        var createdIdentityResult = await _userManager.CreateAsync(identity, register.Password);
        if (!createdIdentityResult.Succeeded)
        {
            return BadRequest(createdIdentityResult.Errors);
        }

        var newClaims = new List<Claim>
        {
            new("Name", register.Name),
        };

        var claimsResult = await _userManager.AddClaimsAsync(identity, newClaims);
        if (!claimsResult.Succeeded)
        {
            await _userManager.DeleteAsync(identity);
            return BadRequest(claimsResult.Errors);
        }

        IdentityRole role = null;
        string roleName = register.Role == Role.Admin ? "Admin" : "User";

        role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
        {
            role = new IdentityRole(roleName);
            var roleResult = await _roleManager.CreateAsync(role);
            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(identity);
                return BadRequest(roleResult.Errors);
            }
        }

        var roleAssignResult = await _userManager.AddToRoleAsync(identity, roleName);
        if (!roleAssignResult.Succeeded)
        {
            await _userManager.DeleteAsync(identity);
            return BadRequest(roleAssignResult.Errors);
        }

        newClaims.Add(new Claim(ClaimTypes.Role, roleName));
        var claimsIdentity = new ClaimsIdentity(new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, identity.Email ?? throw new InvalidOperationException()),
            new Claim(JwtRegisteredClaimNames.Email, identity.Email ?? throw new InvalidOperationException()),
            new Claim("UserId", identity.Id ?? throw new InvalidOperationException()),
        });

        claimsIdentity.AddClaims(newClaims);

        var token = _identityService.CreateSecurityToken(claimsIdentity);
        var response = new AuthenticationResult(_identityService.WriteToken(token));


        return Ok(response);
    }





    public record RegisterUser(Role Role, string Email, string Password, string Name, int Age, string PhoneNumber);
    public record AuthenticationResult(string Token);
    public record LoginUser(string Email, string Password);
}