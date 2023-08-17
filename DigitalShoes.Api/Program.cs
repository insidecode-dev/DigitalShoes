#nullable disable
using DigitalShoes.Api.AuthOperations.Repositories;
using DigitalShoes.Api.AuthOperations.Services;
using DigitalShoes.Api.logs;
using DigitalShoes.Dal.Context;
using DigitalShoes.Domain;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Domain.FluentValidators;
using DigitalShoes.Service;
using DigitalShoes.Service.Abstractions;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultSQLConnection"), b => b.MigrationsAssembly("DigitalShoes.Api"));
});

// identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>()
    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();


// automapper
builder.Services.AddAutoMapper(typeof(MappingProfile));

// versioning
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

// versioning 
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});


// services
builder.Services.AddScoped<IUserRepository, UserRepository>(); // auth
builder.Services.AddScoped<IUserService, UserService>(); // auth
builder.Services.AddScoped<IShoeService, ShoeService>();
builder.Services.AddScoped<IMageService, ImageService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();

// how to ignore navigation property  
// global exception handling


// fluent validation
builder.Services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<HashtagValidator>()).AddXmlDataContractSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddAuthorization();

// logging
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<ILogEventEnricher, UserNameEnricher>();

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/log.txt")
    .WriteTo.MSSqlServer(
    connectionString: builder.Configuration.GetConnectionString("DefaultSQLConnection"),
    tableName: "logs",
    autoCreateSqlTable: true,
    columnOptions: new ColumnOptions
    {
        AdditionalColumns = new Collection<SqlColumn>
            {                
                new SqlColumn { ColumnName = "user_name", DataType = SqlDbType.NVarChar, DataLength = 256 }
            }
    })
    .Enrich.FromLogContext()
    .CreateLogger();

builder.Host.UseSerilog(Log.Logger);

// documenting swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        //BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        //Type = SecuritySchemeType.Http
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference{Type = ReferenceType.SecurityScheme, Id="Bearer"},
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });


    // creating a swagger document for our specified version 
    // I created swagger documentation for v1
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.0",
        Title = "DigitalShoes V1",
        Description = "API to manage DigitalShoes",
        TermsOfService = new Uri("https://www.postman.com/"),
        Contact = new OpenApiContact
        {
            Name = "insidecode",
            Url = new Uri("https://github.com/insidecode-dev")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://www.google.com/search?q=license&oq=license&aqs=chrome..69i57.3695j0j7&sourceid=chrome&ie=UTF-8")
        }
    });

    options.SwaggerDoc("v2", new OpenApiInfo
    {
        Version = "v2.0",
        Title = "DigitalShoes V2",
        Description = "API to manage DigitalShoes",
        TermsOfService = new Uri("https://www.postman.com/"),
        Contact = new OpenApiContact
        {
            Name = "insidecode",
            Url = new Uri("https://github.com/insidecode-dev")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://www.google.com/search?q=license&oq=license&aqs=chrome..69i57.3695j0j7&sourceid=chrome&ie=UTF-8")
        }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v2/swagger.json", "Digital_ShoesV2");        
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Digital_ShoesV1");
    });
}

app.UseStaticFiles();


app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging();
//app.Use(async (context, next) =>
//{   
//    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null; 
//    LogContext.PushProperty("user_name", username);
//    await next.Invoke();
//});

app.UseStatusCodePages();

app.MapControllers();

app.Run();

#nullable enable