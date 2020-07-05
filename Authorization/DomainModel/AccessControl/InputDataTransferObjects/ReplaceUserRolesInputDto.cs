using DomainFramework.Core;
using System;
using System.Collections.Generic;
using Utilities.Validation;

namespace Authorization.AccessControl
{
    public class ReplaceUserRolesInputDto : IInputDataTransferObject
    {
        public int UserId { get; set; }

        public List<RoleInputDto> Roles { get; set; } = new List<RoleInputDto>();

        public virtual void Validate(ValidationResult result)
        {
            UserId.ValidateNotZero(result, nameof(UserId));

            foreach (var role in Roles)
            {
                role.Validate(result);
            }
        }

    }
}