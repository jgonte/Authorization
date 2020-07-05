using DomainFramework.Core;
using System;
using System.Collections.Generic;

namespace Authorization.AccessControl
{
    public class UserOutputDto : IOutputDataTransferObject
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public IEnumerable<RoleOutputDto> Roles { get; set; }

    }
}