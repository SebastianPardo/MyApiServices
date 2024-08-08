using AccountServices.Business;
using AccountServices.Business.Interfaces;
using Microsoft.EntityFrameworkCore;
using Famnances.DataCore.Data;
using Microsoft.OpenApi.Models;
using Famnances.AuthMiddleware.Models;
using Famnances.AuthMiddleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

builder.Services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddCors();

#if DEBUG
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database"), x => x.MigrationsAssembly("Famnances.DataCore")));
#else
builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Database"), x => x.MigrationsAssembly("Famnances.DataCore")));
#endif

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddSwaggerGen(swagger =>
{
    //This is to generate the Default UI of Swagger Documentation  
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "JWT Token Authentication API",
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

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCustomJwtAuthentication();
//builder.Services.AddAuthentication(o =>
//{
//    o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
//    o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
//    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//})
//      .AddCookie()
//      .AddGoogleOpenIdConnect(options =>
//      {
//          options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//          options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//      });


builder.Services.AddScoped<IAccountManager, AccountManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<JwtMiddleware>();
app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

app.Run();
