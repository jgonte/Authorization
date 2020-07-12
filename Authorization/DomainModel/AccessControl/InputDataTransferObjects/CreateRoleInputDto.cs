using DomainFramework.Core;
using System;
using System.Collections.Generic;
using Utilities.Validation;

namespace Authorization.AccessControl
{
    public class CreateRoleInputDto : IInputDataTransferObject
    {
        public string Name { get; set; }

        public virtual void Validate(ValidationResult result)
        {
            Name.ValidateMaxLength(result, nameof(Name), 256);
        }

    }
}