using DomainFramework.Core;
using System;
using System.Collections.Generic;
using Utilities.Validation;

namespace Authorization.AccessControl
{
    public class CreateUserInputDto : IInputDataTransferObject
    {
        public string Email { get; set; }

        public virtual void Validate(ValidationResult result)
        {
            Email.ValidateEmail(result, nameof(Email));

            Email.ValidateNotEmpty(result, nameof(Email));

            Email.ValidateMaxLength(result, nameof(Email), 256);
        }

    }
}