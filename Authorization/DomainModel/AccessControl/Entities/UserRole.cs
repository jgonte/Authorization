using DomainFramework.Core;
using System;
using System.Collections.Generic;

namespace Authorization.AccessControl
{
    public class UserRoleId
    {
        public int? UserId { get; set; }

        public int? RolesId { get; set; }

    }

    public class UserRole : Entity<UserRoleId>
    {
        public int CreatedBy { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDateTime { get; set; }

    }
}