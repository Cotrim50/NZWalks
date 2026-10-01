using NZWalks.API.Data;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.Repositories;
using NZWalks.API.Mappings;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using Serilog;
using NZWalks.API.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


//Configuration logger
var logger = new LoggerConfiguration()
  .WriteTo.Console()
  .WriteTo.File("Logs/NzWalks_Log.txt", rollingInterval: RollingInterval.Day)
  .MinimumLevel.Information()
  .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

//builder.Services.AddApiVersioning();

 
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
  options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "NZ Walks API", Version = "v1" });
  options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
  {
    Name = "Authorization",
    Type = SecuritySchemeType.ApiKey,
    Scheme = JwtBearerDefaults.AuthenticationScheme,
    In = ParameterLocation.Header,

  });
  options.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
    {
      new OpenApiSecurityScheme
      {
        Reference = new OpenApiReference
        {
          Type = ReferenceType.SecurityScheme,
          Id = JwtBearerDefaults.AuthenticationScheme
        },
        Scheme = "Oauth2",
        Name = JwtBearerDefaults.AuthenticationScheme,
        In = ParameterLocation.Header
      },
      new List<string>()
    }
  });

});


builder.Services.AddDbContext<NZWalksDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksConnectionString")));
builder.Services.AddDbContext<NZWalksAuthDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("NZWalksAuthConnectionString")));
builder.Services.AddScoped<IRegionRepository, SQLRegionRepository>();
builder.Services.AddScoped<IWalkRepository, SQLWalkRepository>();

builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IImageRepository, LocalImageRepository>();

builder.Services.AddAutoMapper(cfg => { }, typeof(AutoMapperProfiles));

builder.Services.AddIdentityCore<IdentityUser>()
  .AddRoles<IdentityRole>()
  .AddTokenProvider<DataProtectorTokenProvider<IdentityUser>>("NZWalks")
  .AddEntityFrameworkStores<NZWalksAuthDbContext>()
  .AddDefaultTokenProviders();

builder.Services.Configure<IdentityOptions>(options =>
{
  options.Password.RequireDigit = false;
  options.Password.RequireLowercase = false;
  options.Password.RequireUppercase = false;
  options.Password.RequireNonAlphanumeric = false;
  options.Password.RequiredLength = 4;
  options.Password.RequiredUniqueChars = 1;
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
  {
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    //ValidAudience = builder.Configuration["Jwt:Audience"],
    ValidAudiences = new[] { builder.Configuration["Jwt:Audience"] },
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
  });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.UseStaticFiles(new StaticFileOptions
{
  FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(
    Path.Combine(Directory.GetCurrentDirectory(), "Images")),
  RequestPath = "/Images"

}); // Enable serving static files from wwwroot folder

app.MapControllers();

app.Run();
