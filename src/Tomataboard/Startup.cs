using System.Collections.Generic;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
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

namespace Tomataboard
{


    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile("keys.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]))
                .AddDbContext<TomataboardContext>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc()
                .AddJsonOptions(
                    // will camel case all web api responses
                    opt => opt.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver());

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                             .Database.Migrate();
                    }
                }
                catch { }
            }
            
            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseStaticFiles();

            app.UseIdentity();

            // To configure external authentication please see http://go.microsoft.com/fwlink/?LinkID=532715

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
