

using Famnances.Core.Entities;
using Famnances.Core.Errors;
using Famnances.Core.Security.Authorization;
using Famnances.Core.Security.Jwt;
using Famnances.Core.Security.Services;
using Famnances.Core.Security.Services.Interfaces;
using Famnances.DataCore.Data;
using FamnancesServices.Business;
using FamnancesServices.Business.Interfaces;
using FamnancesServices.Helper;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
#if DEBUG
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database"), x => x.MigrationsAssembly("Famnances.DataCore")));
#else
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database"), x => x.MigrationsAssembly("Famnances.DataCore")));
#endif
builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("AuthService", client =>
{
#if DEBUG
    client.BaseAddress = new Uri("https://localhost:7238/Api/");
#else
    client.BaseAddress = new Uri("https://sp-authservices.azurewebsites.net/api/");
#endif

});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    swagger.SwaggerDoc("v1", new OpenApiInfo { Version = "v1", Title = "JWT Token Famnances API", Description = ".NET 8 Web API" });
    swagger.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    swagger.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] {}
        }
    });
});


builder.Services.AddScoped<IErrorLogManager, ErrorLogManager>();
builder.Services.AddExceptionHandler<ErrorHandler>();
builder.Services.AddScoped<AuthorizeAttribute>();

builder.Services.AddSingleton<ITokenHandler, TokenHandler>();

builder.Services.AddScoped<IAccountManager, AccountManager>();
builder.Services.AddScoped<IAccountTypeManager, AccountTypeManager>();
builder.Services.AddScoped<IFixedIncomeByDiscountManager, FixedIncomeByDiscountManager>();
builder.Services.AddScoped<ICityManager, CityManager>();
builder.Services.AddScoped<ICountryManager, CountryManager>();
builder.Services.AddScoped<IBudgetTypeManager, BudgetTypeManager>();
builder.Services.AddScoped<IExpensesBudgetManager, ExpensesBudgetManager>();
builder.Services.AddScoped<IFixedExpenseManager, FixedExpenseManager>();
builder.Services.AddScoped<IFixedExpensePaymentRecordManager, FixedExpensePaymentRecordManager>();
builder.Services.AddScoped<IFixedIncomeManager, FixedIncomeManager>();
builder.Services.AddScoped<IInflowByDiscountManager, InflowByDiscountManager>();
builder.Services.AddScoped<IIncomeDiscountManager, IncomeDiscountManager>();
builder.Services.AddScoped<IInflowManager, InflowManager>();
builder.Services.AddScoped<ILinkedSocialMediaManager, LinkedSocialMediaManager>();
builder.Services.AddScoped<IOutflowManager, OutflowManager>();
builder.Services.AddScoped<IPeriodManager, PeriodManager>();
builder.Services.AddScoped<IProvinceManager, ProvinceManager>();
builder.Services.AddScoped<ISavingRecordManager, SavingRecordManager>();
builder.Services.AddScoped<ISavingsPocketManager, SavingsPocketManager>();
builder.Services.AddScoped<ISocialMediaManager, SocialMediaManager>();
builder.Services.AddScoped<IUserManager, UserManager>();
builder.Services.AddScoped<IUtilitiesManager, UtilitiesManager>();
builder.Services.AddScoped<ITotalsByPeriodManager, TotalsByPeriodManager>();
builder.Services.AddScoped<IHomeManager, HomeManager>();
builder.Services.AddScoped<IHomeInvitationManager, HomeInvitationManager>();
builder.Services.AddExceptionHandler<ApiErrorHandler>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(options => { });
app.UseStatusCodePages();

app.UseMiddleware<JwtMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();