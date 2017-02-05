using Microsoft.EntityFrameworkCore;
using Tomataboard.Data.Entities;

namespace Tomataboard.Data
{
    public class TomataboardContext : DbContext
    {
        public DbSet<GeoLite2IPv4> GeoLite2IPv4 { get; set; }
        public DbSet<GeoLite2IPv6> GeoLite2IPv6 { get; set; }
        public DbSet<GeoLite2Location> GeoLite2Locations { get; set; }
        public DbSet<Cache> Cache { get; set; }
        public DbSet<Quote> Quotes { get; set; }
        public DbSet<AccessToken> AccessTokens { get; set; }

        //private readonly IOptions<DataSettings> _settings;

        public TomataboardContext(DbContextOptions<TomataboardContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(_settings.Value.TomataboardContextConnection);
            base.OnConfiguring(optionsBuilder);
        }
    }
}