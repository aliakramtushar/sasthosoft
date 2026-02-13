namespace SasthoSoft.Domain.Entities;

public class UserRole
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Navigation property
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}