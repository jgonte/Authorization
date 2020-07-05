using DomainFramework.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Utilities.Validation;

namespace Authorization.AccessControl
{
    public class AddUserLoginsInputDto : IInputDataTransferObject
    {
        public List<UserLoginInputDto> UserLogins { get; set; } = new List<UserLoginInputDto>();

        public virtual void Validate(ValidationResult result)
        {
            var userLoginsCount = (uint)UserLogins.Where(item => item != null).Count();

            userLoginsCount.ValidateCountIsEqualOrGreaterThan(result, 1, nameof(UserLogins));

            foreach (var user in UserLogins)
            {
                user.Validate(result);
            }
        }

    }
}