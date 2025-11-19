using AuthServices.Business;
using AuthServices.Business.Interfaces;
using AuthServices.Security;
using Famnances.Core.Entities;
using Famnances.Core.Errors;
using Famnances.Core.Security.Authorization;
using Famnances.Core.Security.Services;
using Famnances.Core.Security.Services.Interfaces;
using Famnances.Core.Utils.Services;
using Famnances.Core.Utils.Services.Interface;
using Famnances.DataCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

//AppContext.SetSwitch("Switch.Microsoft.Data.SqlClient.UseManagedNetworkingOnWindows", true);
var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));


builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("LoggingService", client =>
{
#if DEBUG

    client.BaseAddress = new Uri("https://localhost:7246/Api/");
#else
client.BaseAddress = new Uri("https://famnancesservices.azurewebsites.net/api/");
#endif
});
builder.Services.AddExceptionHandler<ApiErrorHandler>();

builder.Services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);
builder.Services.AddCors();

#if DEBUG
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database"), x => x.MigrationsAssembly("Famnances.DataCore")));
#else
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database"), x => x.MigrationsAssembly("Famnances.DataCore")));
#endif

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Auth Services API",
        Description = ".NET 8 Web API"
    });
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });
});

builder.Services.AddSingleton<ITokenHandler, TokenHandler>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddExceptionHandler<ApiErrorHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(options =>
{
});
app.UseStatusCodePages();

app.UseMiddleware<JwtMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();