namespace SasthoSoft.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int RoleId { get; set; }
    public DateTime CreatedAt { get; set; }

    // Navigation property
    public virtual UserRole? Role { get; set; }
}