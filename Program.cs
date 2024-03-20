
using System.Configuration;
using System.Text;
using AuthenticationApi.Db;
using AuthenticationApi.Entities;
using AuthenticationApi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;


var  MyAllowSpecificOrigins = "_myAllowSpecificOrigins";


var builder = WebApplication.CreateBuilder(args);



var configuration = builder.Configuration;
// Add services to the container.
var connectionString = configuration.GetConnectionString("DefaultConnection");
var MongoDBConntection = configuration.GetConnectionString("MongoDBConntection");



builder.Services.AddScoped<MongoClient>(provider =>{
  return new MongoClient(MongoDBConntection);
});


//specifying origins to support CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy  =>
                      {
                          policy.WithOrigins("http://localhost:4200")
                                             .AllowAnyHeader()
                                             .AllowAnyMethod();
                      });
});



//DB context
builder.Services.AddDbContext<AppDbContext>(options => options.UseMySQL(connectionString));



//Identity
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

//adding authentication
builder.Services.AddAuthentication(options =>{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}) 
  .AddJwtBearer(options => {
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
      options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,  
        ValidateLifetime = true,
         ClockSkew = TimeSpan.Zero,
         ValidIssuer = configuration["Jwt:ValidAudience"],
         ValidAudience = configuration["Jwt:ValidIssuer"],  
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Secret"])) 
    };
  });

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
//Tells the compiler whenever IAuthenticationService is injected use the AuthenticationService Implementation


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors(MyAllowSpecificOrigins); //enable CORS

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 8. Authentication
app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
