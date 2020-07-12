using DomainFramework.Core;
using System;
using System.Collections.Generic;

namespace Authorization.AccessControl
{
    public class IdentityProvider : Entity<int>
    {
        /// <summary>
        /// The name of a provider that can authenticate the identity of a person
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The Uniform Resource Identifier (URI) of the provider
        /// </summary>
        public string Uri { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedDateTime { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedDateTime { get; set; }

    }
}