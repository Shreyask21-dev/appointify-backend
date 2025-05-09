using ConsultantDashboard.Infrastructure.Data;     // your DbContext
using ConsultantDashboard.Core.Models;             // your ApplicationUser
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ConsultantDashboard.Services.IImplement;
using ConsultantDashboard.Services.Implement;
using ConsultantDashboard.Services;

var builder = WebApplication.CreateBuilder(args);

// ??? 1. CONFIGURATION ??????????????????????????????????????????????????????????

// Load appsettings.json from the root of this project
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Make IConfiguration injectable
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddScoped<IPatientProfileService, PatientProfileService>();
builder.Services.AddScoped<ICustomerAppointmentService, CustomerAppointmentService>();
builder.Services.AddScoped<IConsultationPlanService, ConsultationPlanService>();
builder.Services.AddScoped<IConsultantProfileService, ConsultantProfileService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IConsultantProfileService, ConsultantProfileService>();
builder.Services.AddScoped<IPatientAuthService, PatientAuthService>();
builder.Services.AddScoped<IStatService, StatService>();
builder.Services.AddScoped<IFaqService, FaqService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<ISection5Service, Section5Service>();
builder.Services.AddScoped<IAppointmentRequestService, AppointmentRequestService>();








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



// ??? 4. CORS ?????????????????????????????????????????????????????????????????????

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowHTTPFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
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

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Production-safe error page for MVC views
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
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
app.UseCors("AllowHTTPFrontend");

app.UseAuthentication();
app.UseAuthorization();

// MVC default route (for your HomeController/Views)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// API controllers
app.MapControllers();

app.Run();
