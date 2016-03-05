using Microsoft.Data.Entity;
using Microsoft.Extensions.OptionsModel;
using Thalia.Data.Entities;

namespace Thalia.Data
{
    public class ThaliaContext : DbContext
    {
        public DbSet<ServiceRequest> ServiceRequests { get; set; }

        private IOptions<DataSettings> _settings;

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
