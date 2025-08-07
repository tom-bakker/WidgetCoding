using Microsoft.EntityFrameworkCore;
using WidgetCoding.Core.Entities;

namespace WidgetCoding.Infrastructure.Data {
    public class ApiDataContext : DbContext {

      public ApiDataContext(DbContextOptions options) : base(options) { }

        public DbSet<Widget> Widgets { get; set; }
    }
}
