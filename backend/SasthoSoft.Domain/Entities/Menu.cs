using Dapper.Contrib.Extensions;

namespace SasthoSoft.Domain.Entities;

[Table("Menus")]
public class Menu
{
    [Key]
    public int MenuID { get; set; }

    public int? ParentID { get; set; }

    public string MenuName { get; set; } = string.Empty;

    public string? Url { get; set; }

    public string? Icon { get; set; }

    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }

    public int DisplayOrder { get; set; }

    public string? HoverText { get; set; }

    // Not mapped to DB, used to build tree structure in memory
    [Write(false)]
    public List<Menu> Children { get; set; } = new List<Menu>();
}
