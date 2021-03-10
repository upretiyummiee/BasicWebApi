using AutoMapper;
using BasicWebApi.Data;
using BasicWebApi.Data.IdentityData;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BasicWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string key = "401b09eab3c013d4ca54922bb802bec8fd5318192b" +
                "0a75f201d8b3727429090fb337591abd3e44453b954555b7a0812e1" +
                "081c39b740293f765eae731f5a65ed1";

            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .WithMethods("GET", "DELETE", "PUT", "POST", "OPTIONS")
                        .AllowCredentials()
                        .SetIsOriginAllowed((host) => true)
                        //.AllowAnyHeader()
                        .WithHeaders("Authorization"));
            });

            services.AddMvc(options =>
                options.EnableEndpointRouting = false).SetCompatibilityVersion(CompatibilityVersion.Latest)
                .AddNewtonsoftJson(opt =>
                    opt.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore);

            services.AddEntityFrameworkSqlServer();

            services.AddDbContextPool<AppDbContext>(
                (provider, options) => {
                    options.UseSqlServer(Configuration.GetConnectionString("MyConnection"));
                    options.UseInternalServiceProvider(provider);
                }
            );

            services.AddIdentity<IdentityUserInherit, IdentityRole>(o =>
            {
                o.Lockout.AllowedForNewUsers = false;

                o.Password.RequiredLength = 8;
                o.Password.RequiredUniqueChars = 0;
                o.Password.RequireNonAlphanumeric = false;

                o.SignIn.RequireConfirmedAccount = false;
                o.SignIn.RequireConfirmedEmail = false;
                o.SignIn.RequireConfirmedPhoneNumber = false;
            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

            services.AddAuthentication(x => 
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(authenticationScheme:"JWTBearerAuthentication", config => 
            {
                config.RequireHttpsMetadata = true;
                config.SaveToken = true;
                config.TokenValidationParameters = new TokenValidationParameters 
                { 
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                    
                };
            });

            services.ConfigureApplicationCookie(o => {
                o.Cookie.Name = "MyWebApiCookie";
                o.ExpireTimeSpan = TimeSpan.FromDays(60);
                o.SlidingExpiration = true;

                o.Events.OnRedirectToLogin = context => {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    //var header = HttpResponseHeader.WwwAuthenticate;
                    context.Response.Headers.Append("www-authenticate", "29");
                    return Task.FromResult<object>(null);
                };

                o.Events.OnRedirectToAccessDenied = context => {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    return Task.FromResult<object>(null);
                };

                o.Events.OnRedirectToLogout = context => {
                    context.Response.StatusCode = StatusCodes.Status200OK;
                    return Task.FromResult<object>(null);
                };
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();        


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
