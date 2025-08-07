using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WidgetCoding.Core.Entities;
using WidgetCoding.Core.Globals;
using WidgetCoding.Infrastructure.Data;
using WidgetCoding.Server.Models;

namespace WidgetCoding.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WidgetsController(ApiDataContext context) : ControllerBase
    {
        private readonly ApiDataContext _context = context;

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Widget>>> GetWidgetsAsync()
        {
            var widgets = await _context.Widgets.ToListAsync();

            if(widgets == null) {
                return NotFound();
            }

            return Ok(widgets);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Widget>> GetWidget(Guid id)
        {
            var widget = await _context.Widgets.FindAsync(id);

            if (widget == null)
            {
                return NotFound();
            }

            return Ok(widget);
        }

        /// <summary>
        /// Retrieves the categories for the widget.
        /// </summary>
        /// <returns></returns>
        [HttpGet("categories")]
        public IActionResult GetWidgetCategories() {

            var categoriesJson = Enum.GetValues(typeof(WidgetCategories))
                .Cast<WidgetCategories>()
                .Select(category => new { value = (int)category, text = category.ToString() })
                .OrderBy(category => category.text)
                .ToList();

            return Ok(categoriesJson);

        }

        
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWidget(Guid id, Widget widget)
        {
            if (id != widget.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid) {
                return BadRequest();
            }

            _context.Entry(widget).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WidgetExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        
        [HttpPost]
        public async Task<ActionResult<Widget>> PostWidget(WidgetCreateDTO widget)
        {
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            var newWidget = new Widget() {
                Id = Guid.NewGuid(),
                Name = widget.Name,
                Price = widget.Price,
                Category = (WidgetCategories)widget.Category,
                DateAddedUtc = DateTimeOffset.UtcNow,
            };

            _context.Widgets.Add(newWidget);

            try {
                await _context.SaveChangesAsync();
            }
            catch(Exception ex) {
                return BadRequest(ex.Message);
            }

            return CreatedAtAction("GetWidget", new { id = newWidget.Id }, widget);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWidget(Guid id)
        {
            var widget = await _context.Widgets.FindAsync(id);

            if (widget == null)
            {
                return NotFound();
            }

            _context.Widgets.Remove(widget);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool WidgetExists(Guid id)
        {
            return _context.Widgets.Any(e => e.Id == id);
        }
    }
}
