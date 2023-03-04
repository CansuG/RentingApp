using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Renting.Identity;
using Renting.Models.Account;
using Renting.Models.Settings;
using Renting.Repository;
using Renting.Services;
using Renting.Web.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Renting.Web;

public class Startup
{

    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.Configure<CloudinaryOptions>(Configuration.GetSection("CloudinaryOptions"));

        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IPhotoService, PhotoService>();

        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IAdvertRepository, AdvertRepository>();
        services.AddScoped<IAdvertPhotoRepository, AdvertPhotoRepository>();

        services.AddIdentityCore<ApplicationUserIdentity>(opt =>
        {
            var allowed = opt.User.AllowedUserNameCharacters
              + "abcçdefgğhıijklmnoöprsştuüvyzxqwABCÇDEFGĞHIİJKLMNOÖPRSŞTUÜVYZXQW-._@+0123456789";
            opt.User.AllowedUserNameCharacters = allowed;
            opt.Password.RequireNonAlphanumeric = true;
            opt.User.RequireUniqueEmail = true;

        })
            .AddUserStore<UserStore>()
            .AddDefaultTokenProviders()
            .AddSignInManager<SignInManager<ApplicationUserIdentity>>();

        services.AddControllers();

        services.AddCors(options =>
        {
            var Origins = Configuration["AllowedOrigins"].Split(";");
            options.AddPolicy(name: "Cors",
                policy =>
                {
                    policy
                        .WithOrigins(Origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
        });


        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
            .AddJwtBearer
            (
                options =>
                {
                    options.RequireHttpsMetadata = false;
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["Jwt:Issuer"],
                        ValidAudience = Configuration["Jwt:Issuer"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("Token-Expired", "true");
                            }
                            return Task.CompletedTask;
                        }
                    };
                }
            );


    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(WebApplication app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
        }

        app.ConfigureExceptionHandler();

        app.UseRouting();

        app.UseStaticFiles();

        app.UseCors("Cors");

        app.UseAuthentication();

        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });

        app.Run();
    }
}
