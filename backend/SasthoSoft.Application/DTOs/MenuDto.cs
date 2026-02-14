namespace SasthoSoft.Application.DTOs;

public class MenuDto
{
    public int MenuID { get; set; }
    public int? ParentID { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public string? Url { get; set; }
    public string? Icon { get; set; }
    public int DisplayOrder { get; set; }
    public string? HoverText { get; set; }
    public bool IsActive { get; set; }
    public bool IsDeleted { get; set; }

    // For tree structure
    public List<MenuDto> Children { get; set; } = new List<MenuDto>();

    // Nested DTOs for create/update
    public class Create
    {
        public int? ParentID { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
        public string? HoverText { get; set; }
    }

    public class Update
    {
        public int? ParentID { get; set; }
        public string MenuName { get; set; } = string.Empty;
        public string? Url { get; set; }
        public string? Icon { get; set; }
        public int DisplayOrder { get; set; }
        public string? HoverText { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
