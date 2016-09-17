using System.Collections.Generic;
using CoreDocker.Core.Mappers;
using CoreDocker.Dal.Models;
using CoreDocker.Dal.Models.Reference;

namespace CoreDocker.Core.DataIntegrity
{
    public class IntegrityOperators
    {
        private static readonly List<IIntegrity> _allIntegrity;

        static IntegrityOperators()
        {
            _allIntegrity = new List<IIntegrity>
            {
                new PropertyIntegrity<Project, ProjectReference, User>(u => u.DefaultProject, g => g.Users,r => x => x.DefaultProject.Id == r.Id, x=>x.ToReference()),
                //Sample with array of items
                //new PropertyIntegrity<Project, ProjectReference, User>(u => u.AllowedProject[-1], g => g.Users,r => x => x.AllowedProject.Any(a=>a.Id == r.Id) , x=>x.ToReference()),
            };
        }

        public static List<IIntegrity> Default
        {
            get
            {
                return _allIntegrity;
            }
        }
    }
}