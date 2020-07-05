using DomainFramework.Core;
using System;
using System.Collections.Generic;

namespace Authorization.AccessControl
{
    public class IdentityProviderOutputDto : IOutputDataTransferObject
    {
        public int IdentityProviderId { get; set; }

        public string Name { get; set; }

        public string Uri { get; set; }

    }
}