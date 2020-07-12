using DomainFramework.Core;
using System;
using System.Collections.Generic;
using Utilities.Validation;

namespace Authorization.AccessControl
{
    public class CreateUserRoleInputDto : IInputDataTransferObject
    {
        public int RoleId { get; set; }

        public int UserId { get; set; }

        public virtual void Validate(ValidationResult result)
        {
            RoleId.ValidateRequired(result, nameof(RoleId));

            UserId.ValidateRequired(result, nameof(UserId));
        }

    }
}