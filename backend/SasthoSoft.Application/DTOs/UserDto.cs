namespace SasthoSoft.Application.DTOs
{
    // --------------------------
    // DTO for creating a new user
    // --------------------------
    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? Email { get; set; }
        public int RoleID { get; set; }
    }

    // --------------------------
    // DTO for returning user info
    // --------------------------
    public class UserDto
    {
        public int UserID { get; set; }                 // Auto-generated in DB
        public string Username { get; set; } = string.Empty;

        // Only used on create; not returned normally
        public string? Password { get; set; }

        public string? Email { get; set; }
        public int RoleID { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    // --------------------------
    // DTO for updating a user
    // --------------------------
    public class UpdateUserDto
    {
        public string? Password { get; set; }   // Optional
        public string? Email { get; set; }
        public int? RoleID { get; set; }
    }
}
