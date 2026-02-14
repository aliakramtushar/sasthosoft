using Dapper.Contrib.Extensions;

namespace SasthoSoft.Domain.Entities;

[Table("UserRoles")]
public class UserRole
{
    [Key]
    public int UserRoleID { get; set; }

    public string UserRoleName { get; set; } = string.Empty;

    public string? Description { get; set; }

    public bool IsActive { get; set; }
}
