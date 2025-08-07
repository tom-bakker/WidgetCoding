using WidgetCoding.Core.Entities;
using WidgetCoding.Infrastructure.Data;

namespace WidgetCoding.Infrastructure.Seeder {

    /// <summary>
    /// Used to seed the widgets if there are no widgets in the database. 
    /// </summary>
    /// <param name="context"></param>
    public class WidgetSeeder (ApiDataContext context) {
        private readonly ApiDataContext _context = context;   

        public void SeedWidgets() {
            if (_context.Widgets.Any()) {
                return;
            }
            
            IEnumerable<Widget> widgets = new List<Widget>() {
                new Widget() {
                    Id = Guid.NewGuid(),
                    Name = "Football",
                    Price = 20.00m,
                    Category = Core.Globals.WidgetCategories.Sports,
                    DateAddedUtc = DateTime.UtcNow,
                },
                new Widget() {
                    Id = Guid.NewGuid(),
                    Name = "Lord of the Rings",
                    Category = Core.Globals.WidgetCategories.Books,
                    Price = 60.00m,
                    DateAddedUtc = DateTime.UtcNow,
                },
                new Widget() {
                    Id = Guid.NewGuid(),
                    Name = "iPhone 20",
                    Price = 1000.00m,
                    Category = Core.Globals.WidgetCategories.Electronics,
                    DateAddedUtc = DateTime.UtcNow,
                },
            };

            _context.AddRange(widgets);
            _context.SaveChanges(); 
        }
    }
}
