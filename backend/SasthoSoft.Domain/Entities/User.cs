using Dapper.Contrib.Extensions;

namespace SasthoSoft.Domain.Entities;

[Table("Users")]
public class User
{
    [Key]
    public int UserID { get; set; }

    public string Username { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string? Email { get; set; }

    public int RoleID { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public int? UpdatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }
}
