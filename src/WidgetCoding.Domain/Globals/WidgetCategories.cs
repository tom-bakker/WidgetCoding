using System.ComponentModel.DataAnnotations;

namespace WidgetCoding.Core.Globals;

public enum WidgetCategories {
    Electronics, 
    Clothing,
    [Display(Name = "Home and Garden")]
    HomeAndGarden,
    Books, 
    Sports,
};
