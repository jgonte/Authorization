using DomainFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Validation;

namespace Authorization.AccessControl
{
    public class CreateUserInputDto : IInputDataTransferObject
    {
        public string Email { get; set; }

        public List<UserLoginInputDto> UserLogins { get; set; } = new List<UserLoginInputDto>();

        public virtual void Validate(ValidationResult result)
        {
            Email.ValidateEmail(result, nameof(Email));

            Email.ValidateRequired(result, nameof(Email));

            Email.ValidateMaxLength(result, nameof(Email), 256);

            var userLoginsCount = (uint)UserLogins.Where(item => item != null).Count();

            userLoginsCount.ValidateCountIsEqualOrGreaterThan(result, 1, nameof(UserLogins));

            foreach (var user in UserLogins)
            {
                user.Validate(result);
            }
        }

    }
}