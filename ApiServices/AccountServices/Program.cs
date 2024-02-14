using AccountServices.Business;
using AccountServices.Business.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using OhMyMoney.AuthMiddleware;
using OhMyMoney.DataCore.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

f DEBUG
//builder.Services.AddDbContext<OrderDirectContext>(options => options.UseSqlServer(Environment.GetEnvironmentVariable("DB_ORDER_DIRECT_ALDER")));
builder.Services.AddDbContext<OrderDirectContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("OrderDirectAlder")));
#else
builder.Services.AddDbContext<OrderDirectContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("OrderDirect")));
#endif

builder.Services.AddCors();
builder.Services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

builder.Services.AddAutoMapper(typeof(Program));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(o =>
{
    o.DefaultChallengeScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    o.DefaultForbidScheme = GoogleOpenIdConnectDefaults.AuthenticationScheme;
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
      .AddCookie()
      .AddGoogleOpenIdConnect(options =>
      {
          options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
          options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
      });

builder.Services.AddSingleton<JwtTokenHandler>();

builder.Services.AddScoped<IUserManager, UserManager>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
