using DomainFramework.Core;
using System;
using System.Collections.Generic;
using Utilities.Validation;

namespace Authorization.AccessControl
{
    public class RoleInputDto : IInputDataTransferObject
    {
        public string Name { get; set; }

        public UserRoleInputDto UserRole { get; set; }

        public virtual void Validate(ValidationResult result)
        {
            Name.ValidateMaxLength(result, nameof(Name), 256);

            UserRole.Validate(result);
        }

    }
}