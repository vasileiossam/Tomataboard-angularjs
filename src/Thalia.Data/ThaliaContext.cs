using Microsoft.Data.Entity;
using Microsoft.Extensions.OptionsModel;
using Thalia.Data.Entities;

namespace Thalia.Data
{
    public class ThaliaContext : DbContext
    {
        public DbSet<GeoLite2IPv4> GeoLite2IPv4 { get; set; }
        public DbSet<GeoLite2IPv6> GeoLite2IPv6 { get; set; }
        public DbSet<GeoLite2Location> GeoLite2Locations { get; set; }
        public DbSet<Cache> Cache { get; set; }
        public DbSet<Quote> Quotes { get; set; }

        private readonly IOptions<DataSettings> _settings;

        public ThaliaContext(IOptions<DataSettings> settings)
        {
            _settings = settings;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_settings.Value.ThaliaContextConnection);
            base.OnConfiguring(optionsBuilder);
        }
    }
}
