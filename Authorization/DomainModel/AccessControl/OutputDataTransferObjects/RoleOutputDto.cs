using DomainFramework.Core;
using System;
using System.Collections.Generic;

namespace Authorization.AccessControl
{
    public class RoleOutputDto : IOutputDataTransferObject
    {
        public int RoleId { get; set; }

        public string Name { get; set; }

    }
}