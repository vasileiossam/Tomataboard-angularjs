using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;
using Tomataboard.Models;
using Tomataboard.Services;
using Tomataboard.Data;
using Tomataboard.Services.AccessTokens;
using Tomataboard.Services.Cache;
using Tomataboard.Services.Encryption;
using Tomataboard.Services.Greetings;
using Tomataboard.Services.Locations;
using Tomataboard.Services.Photos.Api500px;
using Tomataboard.Services.Photos.Flickr;
using Tomataboard.Services.Weather.Yahoo;
using Tomataboard.Services.Locations.Freegeoip;
using Tomataboard.Services.Locations.GeoLite;
using Tomataboard.Services.Locations.IpGeolocation;
using Tomataboard.Services.Photos;
using Tomataboard.Services.Quotes;
using Tomataboard.Services.Weather;
using Tomataboard.Services.Weather.Forecast;
using Tomataboard.Services.Weather.OpenWeatherMap;
using Tomataboard.Services.Photos.Tirolography;

namespace Tomataboard
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile("keys.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                // will cascade over appsettings.json  
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }
            
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get;  }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TomataboardContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("TomataboardConnection")));
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("TomataboardConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.SignIn.RequireConfirmedEmail = true;
                }
            ).AddEntityFrameworkStores<ApplicationDbContext>()
             .AddDefaultTokenProviders();

            services.AddMvc()
                .AddJsonOptions(
                    // will camel case all web api responses
                    opt => opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            // location services
            services.AddTransient<ILocationProvider, LocationProvider>();
            services.AddTransient<IFreegeoipService, FreegeoipService>();
            services.AddTransient<IGeoLiteService, GeoLiteService>();
            services.AddTransient<IIpGeolocationService, IpGeolocationService>();
            services.AddTransient<ICacheRepository<Location>, CacheRepository<Location>>();

            // photo services
            services.AddTransient<IPhotoProvider, PhotoProvider>();
            services.AddTransient<IApi500px, Api500px>();
            services.AddTransient<IFlickrService, FlickrService>();
            services.AddTransient<ITirolographyService, TirolographyService>();
            services.AddTransient<ICacheRepository<List<Photo>>, CacheRepository<List<Photo>>>();

            // weather services
            services.AddTransient<IWeatherProvider, WeatherProvider>();
            services.AddTransient<IYahooWeatherService, YahooWeatherService>();
            services.AddTransient<IOpenWeatherMapService, OpenWeatherMapService>();
            services.AddTransient<IForecastService, ForecastService>();
            services.AddTransient<ICacheRepository<WeatherConditions>, CacheRepository<WeatherConditions>> ();

            services.AddTransient<IEncryptionService, EncryptionService>();
            services.AddTransient<ICookiesService<OauthToken>, CookiesService<OauthToken>>();
            services.AddTransient<IAccessTokensRepository, AccessTokensRepository>();
            services.AddTransient<IQuoteRepository, QuoteRepository>();
            services.AddTransient<IGreetingsService, GreetingsService>();

            services.AddLogging();

            services.Configure<DataSettings>(Configuration.GetSection("Data:DefaultConnection"));
            services.Configure<Api500pxKeys>(Configuration.GetSection("ApiSettings:Api500pxKeys"));
            services.Configure<FlickrServiceKeys>(Configuration.GetSection("ApiSettings:FlickrKeys"));
            services.Configure<YahooWeatherServiceKeys>(Configuration.GetSection("ApiSettings:YahooWeatherKeys"));
            services.Configure<OpenWeatherMapKeys>(Configuration.GetSection("ApiSettings:OpenWeatherMapKeys"));
            services.Configure<ForecastKeys>(Configuration.GetSection("ApiSettings:ForecastKeys"));
            services.Configure<EncryptionServiceKeys>(Configuration.GetSection("ApiSettings:EncryptionServiceKeys"));
            services.Configure<SendGridOptions>(Configuration.GetSection("ApiSettings:SendGridOptions"));
            services.Configure<GmailOptions>(Configuration.GetSection("ApiSettings:GmailOptions"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            //loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddConsole(LogLevel.Debug);
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
               app.UseDeveloperExceptionPage();
               app.UseDatabaseErrorPage();
               app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                //try
                //{
                //    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                //        .CreateScope())
                //    {
                //        serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                //             .Database.Migrate();
                //    }
                //}
                //catch { }
            }
            
            app.UseStaticFiles();

            app.UseIdentity();

            //app.Use(next =>
            //{
            //    return ctx =>
            //    {
            //        ctx.Response.Headers.Remove("Server");
            //        return next(ctx);
            //    };
            //});

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                       name: "dashboard",
                       template: "focus/{controller=Dashboard}/{action=Index}");
            });
        }
    }
}
