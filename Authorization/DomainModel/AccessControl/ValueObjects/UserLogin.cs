using DomainFramework.Core;
using System;
using System.Collections.Generic;

namespace Authorization.AccessControl
{
    public class UserLogin : ValueObject<UserLogin>
    {
        /// <summary>
        /// The name/issuer of the OAuth provider
        /// </summary>
        public string Provider { get; set; }

        /// <summary>
        /// The external user id/subject of the OAuth provider
        /// </summary>
        public string UserKey { get; set; }

        public override bool IsEmpty() => 
            Provider == default(string) && UserKey == default(string);

        protected override IEnumerable<object> GetFieldsToCheckForEquality() => 
            new List<object>
            {
                Provider,
                UserKey
            };

    }
}