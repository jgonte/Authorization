using DomainFramework.Core;
using System;
using System.Collections.Generic;
using Utilities.Validation;

namespace Authorization.AccessControl
{
    public class UserRoleInputDto : IInputDataTransferObject
    {
        public int UserId { get; set; }

        public int RolesId { get; set; }

        public virtual void Validate(ValidationResult result)
        {
            UserId.ValidateNotZero(result, nameof(UserId));

            RolesId.ValidateNotZero(result, nameof(RolesId));
        }

    }
}