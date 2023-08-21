#nullable disable
using DigitalShoes.Api;
using DigitalShoes.Api.AuthOperations.Repositories;
using DigitalShoes.Api.AuthOperations.Services;

using DigitalShoes.Dal.Context;
using DigitalShoes.Domain;
using DigitalShoes.Domain.Entities;
using DigitalShoes.Domain.FluentValidators;
using DigitalShoes.Service;
using DigitalShoes.Service.Abstractions;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Context;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;
using System.Text;

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
builder.Services.AddScoped<ISearchService, SearchService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IWishListService, WishListService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();


// fluent validation
builder.Services.AddControllers().AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<HashtagValidator>()).AddXmlDataContractSerializerFormatters();


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
        
        ValidIssuer = "https://localhost:7249/",
        ValidateIssuer = true,
        
        ValidateAudience = true,
        ValidAudience = "https://localhost:7249/"
    };
});
builder.Services.AddAuthorization();

// logging
builder.Services.AddHttpContextAccessor();

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

// httplogging
builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("sec-ch-ua");
    logging.ResponseHeaders.Add("MyResponseHeader");
    logging.MediaTypeOptions.AddText("application/javascript");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
});

// cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {        
        policy.WithOrigins("https://localhost:7249")
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

// documenting swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",        
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",        
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

    
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {        
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Digital_ShoesV1");
    });
}

app.UseStaticFiles();
app.UseHttpLogging();
app.UseCors("AllowAll");
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSerilogRequestLogging();
app.Use(async (context, next) =>
{
    var username = context.User?.Identity?.IsAuthenticated != null || true ? context.User.Identity.Name : null;
    LogContext.PushProperty("user_name", username);
    await next.Invoke();
});

app.UseStatusCodePages();

app.MapControllers();

app.Run();

#nullable enable