using Famnances.Core.Entities;
using Famnances.Core.Security.Jwt;
using Famnances.Core.Security.Services;
using Famnances.Core.Security.Services.Interfaces;
using Famnances.DataCore.Data;
using FamnancesServices.Business;
using FamnancesServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));
#if DEBUG
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database"), x => x.MigrationsAssembly("Famnances.DataCore")));
#else
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database"), x => x.MigrationsAssembly("Famnances.DataCore")));
#endif

builder.Services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
{
    //This is to generate the Default UI of Swagger Documentation  
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "JWT Token Famnances API",
        Description = ".NET 8 Web API"
    });
    // To Enable authorization using Swagger (JWT)  
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

builder.Services.AddScoped<IAccountManager, AccountManager>();
builder.Services.AddScoped<IAccountTypeManager, AccountTypeManager>();
builder.Services.AddScoped<IAutomaticDiscountManager, AutomaticDiscountManager>();
builder.Services.AddScoped<ICityManager, CityManager>();
builder.Services.AddScoped<ICountryManager, CountryManager>();
builder.Services.AddScoped<IExpensesBudgetManager, ExpensesBudgetManager>();
builder.Services.AddScoped<IFixedExpenseManager, FixedExpenseManager>();
builder.Services.AddScoped<IFixedIncomeManager, FixedIncomeManager>();
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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<JwtMiddleware>();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
