using SasthoSoft.Domain.Enums;
using System;

namespace SasthoSoft.Application.DTOs;

public class PermissionDto
{
    public int PermissionID { get; set; }
    public AppEnum.PermissionType PermissionType { get; set; }
    public int MenuID { get; set; }
    public int UserRoleID { get; set; }
    public bool IsActive { get; set; }
    public bool IsDelete { get; set; }
    public DateTime CreatedDate { get; set; }
    public int CreatedBy { get; set; }


    public class Create
    {
        public AppEnum.PermissionType PermissionType { get; set; }
        public int MenuID { get; set; }
        public int UserRoleID { get; set; }
        public int CreatedBy { get; set; }  // User who creates this permission
    }


    public class Update
    {
        public AppEnum.PermissionType PermissionType { get; set; }
        public bool IsActive { get; set; }
        public bool IsDelete { get; set; }
    }
}
