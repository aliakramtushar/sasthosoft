using Dapper.Contrib.Extensions;
using System;
using static SasthoSoft.Domain.Enums.AppEnum;

namespace SasthoSoft.Domain.Entities;

[Table("Permissions")]
public class Permission
{
    [Key] // Primary key in DB
    public int PermissionID { get; set; }

    public PermissionType PermissionType { get; set; }  // Could be an enum in your C# code

    public int MenuID { get; set; }          // FK to Menu

    public int UserRoleID { get; set; }      // FK to UserRole

    public bool IsActive { get; set; }        // 1 = Active, 0 = Inactive

    public bool IsDelete { get; set; }        // 1 = Deleted, 0 = Not deleted

    public DateTime CreatedDate { get; set; }

    public int CreatedBy { get; set; }       // UserID who created this
}
