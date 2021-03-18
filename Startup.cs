using BasicWebApi.Data;
using BasicWebApi.Data.IdentityData;
using BasicWebApi.Data.Repository.Implementation;
using BasicWebApi.Data.Repository.Interface;
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
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System;
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

            services.AddControllers();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .WithMethods("GET", "DELETE", "PUT", "POST", "OPTIONS")
                        .AllowCredentials()
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyHeader()
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

            services.AddAuthentication( x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(authenticationScheme:JwtBearerDefaults.AuthenticationScheme, config =>
            {
                config.IncludeErrorDetails = true;

                config.RequireHttpsMetadata = true;
                config.SaveToken = true;
                config.RequireHttpsMetadata = true;

                config.Events = new JwtBearerEvents 
                {
                    OnAuthenticationFailed = context => 
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        }

                        return Task.FromResult<object>(null);
                    },

                    OnForbidden = context => 
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return Task.FromResult<object>(null);
                    },

                };

                config.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration.GetSection("key").ToString())),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true
                };
            });

            services.ConfigureApplicationCookie(o => {
                o.Cookie.Name = "MyWebApiCookie";
                o.ExpireTimeSpan = TimeSpan.FromDays(60);
                o.SlidingExpiration = true;

                o.Events.OnRedirectToLogin = context => {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.Headers.Append("WWW-Authenticate", "/api/auth/signin");
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


            services.AddSwaggerGen(x => 
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                { 
                    Title = "MyBasicWebApi",
                    Version = "v1",
                    Description = "Doc for my basic web api",
                    Contact = new OpenApiContact 
                    {
                        Name = "Prabesh Upreti",
                        Email = "upretiyummiee@hotmail.com"
                    }
                });
            });
            services.AddSingleton<IMailSender, OutlookMailSender>();
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

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Add("X-Developed-By", "Wizard");
                await next.Invoke();
            });

            app.UseCors("CorsPolicy");

            app.UseSwagger();
            app.UseSwaggerUI(config => {
                config.SwaggerEndpoint("/swagger/v1/swagger.json", "MyApi");
                config.RoutePrefix = "api";
            });

            app.UseAuthentication();
            app.UseAuthorization();        


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
