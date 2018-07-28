﻿using System.Collections.Generic;
using CoreDocker.Core.Framework.Mappers;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Projects;
using CoreDocker.Dal.Models.Users;

namespace CoreDocker.Core.Framework.DataIntegrity
{
    public class IntegrityOperators
    {
        static IntegrityOperators()
        {
            Default = new List<IIntegrity>
            {
                new PropertyIntegrity<Project, ProjectReference, User>(u => u.DefaultProject, g => g.Users,r => x => x.DefaultProject.Id == r.Id, x=>x.ToReference()),
//                new PropertyIntegrity<User, UserReference, UserGrant>(u => u.User, g => g.UserGrants,r => x => x.User.Id == r.Id, x=>x.ToReference())
                //Sample with array of items
                //new PropertyIntegrity<Project, ProjectReference, User>(u => u.AllowedProject[-1], g => g.Users,r => x => x.AllowedProject.Any(a=>a.Id == r.Id) , x=>x.ToReference()),
            };
        }

        public static List<IIntegrity> Default { get; }
    }
}