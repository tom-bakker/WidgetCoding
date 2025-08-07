using System.ComponentModel.DataAnnotations;
using WidgetCoding.Core.Globals;

namespace WidgetCoding.Core.Entities;

public class Widget {

    [Required]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(AllowEmptyStrings = false, ErrorMessage = "Please include a name for the widget.")]
    [StringLength(maximumLength: 255, MinimumLength = 3, ErrorMessage = "Name must be {0} to {1} characters long. ")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Price of widget to 2 decimal places.
    /// </summary>
    [Required]
    [RegularExpression(@"^\d+(\.\d{1,2})?$")]
    [Range(0, 9999999999999999.99)]
    public decimal Price { get; set; } = decimal.Zero;

    [Required]
    public WidgetCategories Category { get; set; } = WidgetCategories.Books;

    [Required]
    public DateTimeOffset DateAddedUtc { get; set; } = DateTimeOffset.UtcNow;
}
