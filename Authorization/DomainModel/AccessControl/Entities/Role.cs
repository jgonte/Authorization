using DomainFramework.Core;
using System;
using System.Collections.Generic;

namespace Authorization.AccessControl
{
    public class Role : Entity<int?>
    {
        /// <summary>
        /// The name of the role
        /// </summary>
        public string Name { get; set; }

    }
}