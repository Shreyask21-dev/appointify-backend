using ConsultantDashboard.Infrastructure.Data;     // your DbContext
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ConsultantDashboard.Core.Entities;
using ConsultantDashboard.Services.IImplement;
using ConsultantDashboard.Services.Implement;
using ConsultantDashboard.Services;
using ConsultantDashboard.Core.DTOs;

var builder = WebApplication.CreateBuilder(args);


// ??? 1. CONFIGURATION ??????????????????????????????????????????????????????????

// Load appsettings.json from the root of this project
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Make IConfiguration injectable
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<ICustomerAppointmentService, CustomerAppointmentService>();
builder.Services.AddScoped<IConsultationPlanService, ConsultationPlanService>();
builder.Services.AddScoped<IConsultantProfileService, ConsultantProfileService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStatService, StatService>();
builder.Services.AddScoped<IFaqService, FaqService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ISection5Service, Section5Service>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPlanBufferRuleService, PlanBufferRuleService>();
builder.Services.AddScoped<IConsultantShiftService, ConsultantShiftService>();
builder.Services.AddHttpContextAccessor(); // For .NET 6+

builder.Services.Configure<RazorpaySettings>(
    builder.Configuration.GetSection("Razorpay"));








// ??? 2. EF CORE + IDENTITY ??????????????????????????????????????????????????????

// Register your EF Core DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register ASP-NET Core Identity, pointing at your ApplicationUser + EF stores
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();



// ??? 3. JWT AUTHENTICATION ??????????????????????????????????????????????????????

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtKey = builder.Configuration["Jwt:Key"]
        ?? throw new InvalidOperationException("JWT Key is missing in appsettings.json");

    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ConsultantDashboard API", Version = "v1" });

    // 🔐 Enable JWT Auth in Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token like this: **Bearer eyJhbGci...**"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// ??? 4. CORS ?????????????????????????????????????????????????????????????????????

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontendClients",
        policy =>
        {
            policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});






// ??? 5. MVC + SWAGGER ?????????????????????????????????????????????????????????????

builder.Services.AddControllersWithViews();  // MVC controllers + views
builder.Services.AddControllers();           // for attribute-routing APIs
builder.Services.AddSwaggerGen();



var app = builder.Build();



// ??? 6. MIDDLEWARE PIPELINE ??????????????????????????????????????????????????????


{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI();
}


// Global API error handler (for unhandled exceptions)
app.UseExceptionHandler(appError =>
{
    appError.Run(async context =>
    {
        var errorFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (errorFeature != null)
        {
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"Error: {errorFeature.Error.Message}");
        }
    });
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// CORS must come before Authentication/Authorization
app.UseCors("AllowFrontendClients");



app.UseAuthentication();
app.UseAuthorization();

// MVC default route (for your HomeController/Views)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// API controllers
app.MapControllers();

app.Run();
