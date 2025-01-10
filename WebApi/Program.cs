using WebApi.Config;
using WebApi.Extentsions;
using WebApi.Options;
using WebApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.RegisterAuthentication();

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwagger();
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddScoped<IEmailSenderService, EmailService>();

builder.Services.AddRepositories();
builder.Services.AddMediatR();
builder.Services.AddDbContext();
//builder.Services.AddJsonOptions();
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
builder.Services.AddSingleton<IdentityService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins("http://localhost:5173") // Allow the frontend's origin
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseTiming();
var jwtSettings = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();

if (jwtSettings == null || jwtSettings.Audiences == null || jwtSettings.Audiences.Length == 0)
{
    Console.WriteLine("JwtOptions or Audineces is not configured correctly.");
    throw new InvalidOperationException("JwtOptions.Audiences must contain at least one value.");
}
else
{
    Console.WriteLine("Audineces loaded successfully:");
    foreach (var audience in jwtSettings.Audiences)
    {
        Console.WriteLine($"- {audience}");
    }
}
app.UseHttpsRedirection();

// Enable CORS before authentication and authorization
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

