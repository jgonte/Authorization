using DomainFramework.Core;
using System;
using System.Collections.Generic;

namespace Authorization.AccessControl
{
    public class User : Entity<int>
    {
        /// <summary>
        /// The user's email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The user's normalized email
        /// </summary>
        public string NormalizedEmail { get; set; }

    }
}