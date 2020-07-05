using DomainFramework.Core;
using System;
using System.Collections.Generic;
using Utilities.Validation;

namespace Authorization.AccessControl
{
    public class UserLoginInputDto : IInputDataTransferObject
    {
        public string Provider { get; set; }

        public string UserKey { get; set; }

        public virtual void Validate(ValidationResult result)
        {
            Provider.ValidateNotEmpty(result, nameof(Provider));

            Provider.ValidateMaxLength(result, nameof(Provider), 128);

            UserKey.ValidateNotEmpty(result, nameof(UserKey));

            UserKey.ValidateMaxLength(result, nameof(UserKey), 128);
        }

    }
}