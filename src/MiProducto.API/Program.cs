using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiProducto.API.Middleware;
using MiProducto.Application.Common.Behaviors;
using MiProducto.Application.Common.Interfaces;
using MiProducto.Infrastructure.Persistence;
using MiProducto.Infrastructure.Services;
using Serilog;

// ─── Serilog bootstrap logger ────────────────────────────────────
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Iniciando MiProducto API...");

    var builder = WebApplication.CreateBuilder(args);

    // ─── Serilog ─────────────────────────────────────────────────
    builder.Host.UseSerilog((context, services, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration)
                     .ReadFrom.Services(services)
                     .Enrich.FromLogContext());

    // ─── Base de datos ────────────────────────────────────────────
    //builder.Services.AddDbContext<AppDbContext>(options =>
    //    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
    // ─── Base de datos ────────────────────────────────────────────────
    var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
        ?? builder.Configuration.GetConnectionString("DefaultConnection");

    if (connectionString!.StartsWith("postgres://") || connectionString.StartsWith("postgresql://"))
    {
        var uri = new Uri(connectionString);
        var userInfo = uri.UserInfo.Split(':');
        connectionString = $"Host={uri.Host};Port={uri.Port};Database={uri.AbsolutePath.TrimStart('/')};Username={userInfo[0]};Password={userInfo[1]};SSL Mode=Require;Trust Server Certificate=true";
    }

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(connectionString));

    // ─── Repositorios y servicios ─────────────────────────────────
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IJwtService, JwtService>();
    builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

    // ─── File Storage ─────────────────────────────────────────────
    //var imagesPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "images");
    //builder.Services.AddSingleton<IFileStorageService>(
    //    new FileStorageService(imagesPath, "/images"));
    // ─── File Storage ─────────────────────────────────────────────
    if (builder.Environment.IsProduction())
    {
        builder.Services.AddSingleton<IFileStorageService>(new CloudinaryStorageService(
            builder.Configuration["Cloudinary:CloudName"]!,
            builder.Configuration["Cloudinary:ApiKey"]!,
            builder.Configuration["Cloudinary:ApiSecret"]!));
    }
    else
    {
        var imagesPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "images");
        builder.Services.AddSingleton<IFileStorageService>(
            new FileStorageService(imagesPath, "/images"));
    }
    // ─── MediatR ──────────────────────────────────────────────────
    builder.Services.AddMediatR(cfg =>
        cfg.RegisterServicesFromAssembly(
            typeof(MiProducto.Application.Features.Products.Queries.GetAllProductsQuery).Assembly));

    // ─── FluentValidation ─────────────────────────────────────────
    builder.Services.AddValidatorsFromAssembly(
        typeof(MiProducto.Application.Features.Products.Commands.CreateProductCommand).Assembly);
    builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

    // ─── JWT + Google Authentication ─────────────────────────────
    var jwtKey = builder.Configuration["Jwt:Key"]!;
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = "Cookies";
    })
    .AddCookie("Cookies")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        options.CallbackPath = "/signin-google";
        options.SignInScheme = "Cookies";
    });

    builder.Services.AddAuthorization();

    // ─── Controllers + Swagger ────────────────────────────────────
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "MiProducto API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Ingresá el token así: Bearer {token}"
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
                Array.Empty<string>()
            }
        });
    });

    // ─── CORS ─────────────────────────────────────────────────────
    builder.Services.AddCors(options =>
        options.AddPolicy("AllowAll", policy =>
            policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

    var app = builder.Build();

    // ─── Migraciones automáticas ──────────────────────────────────
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }

    // ─── Middleware ───────────────────────────────────────────────
    app.UseMiddleware<GlobalExceptionMiddleware>();
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} respondió {StatusCode} en {Elapsed:0.0000} ms";
    });

    //if (app.Environment.IsDevelopment())
    //{
    //    app.UseSwagger();
    //    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiProducto API v1"));
    //}
    // ─── Swagger siempre habilitado ───────────────────────────────
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "MiProducto API v1"));

    app.UseCors("AllowAll");
    app.UseStaticFiles();
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("MiProducto API iniciada correctamente.");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación falló al iniciar.");
}
finally
{
    Log.CloseAndFlush();
}