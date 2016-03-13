using System.Collections.Generic;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Thalia.Models;
using Thalia.Services;
using Thalia.Data;
using Thalia.Services.Cache;
using Thalia.Services.Locations;
using Thalia.Services.Photos.Api500px;
using Thalia.Services.Photos.Flickr;
using Thalia.Services.Weather.Yahoo;
using Thalia.Services.Locations.Abstract;
using Thalia.Services.Locations.Providers;
using Thalia.Services.Photos;
using Thalia.Services.Weather;
using Thalia.Services.Weather.Forecast;
using Thalia.Services.Weather.OpenWeatherMap;

namespace Thalia
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
                .AddDbContext<ThaliaContext>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

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

            services.AddLogging();

            services.Configure<DataSettings>(Configuration.GetSection("Data:DefaultConnection"));
            services.Configure<Api500pxKeys>(Configuration.GetSection("ApiSettings:Api500pxKeys"));
            services.Configure<FlickrKeys>(Configuration.GetSection("ApiSettings:FlickrKeys"));
            services.Configure<YahooWeatherKeys>(Configuration.GetSection("ApiSettings:YahooWeatherKeys"));
            services.Configure<OpenWeatherMapKeys>(Configuration.GetSection("ApiSettings:OpenWeatherMapKeys"));
            services.Configure<ForecastKeys>(Configuration.GetSection("ApiSettings:ForecastKeys"));
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
