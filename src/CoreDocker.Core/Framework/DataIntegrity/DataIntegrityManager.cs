using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using CoreDocker.Dal.Persistence;

namespace CoreDocker.Core.Framework.DataIntegrity
{
    public class DataIntegrityManager : IDataIntegrityManager
    {
        private readonly IGeneralUnitOfWork _generalUnitOfWork;
        private readonly List<IIntegrity> _integrityUpdatetor;

        public DataIntegrityManager(IGeneralUnitOfWork generalUnitOfWork, List<IIntegrity> integrityUpdatetors)
        {
            _generalUnitOfWork = generalUnitOfWork;
            _integrityUpdatetor = integrityUpdatetors;
        }

        #region IDataIntegrityManager Members

        public async Task<long> UpdateAllReferences<T>(T updatedValue)
        {
            var referenceTotal = 0L;
            var allowedUpdates = _integrityUpdatetor.Where(x => x.UpdateAllowed(updatedValue)).ToArray();
            if (allowedUpdates.Any())
                foreach (var integrity in allowedUpdates)
                    referenceTotal += await integrity.UpdateReferences(_generalUnitOfWork, updatedValue);
            return referenceTotal;
        }

        public async Task<long> GetReferenceCount<T>(T updatedValue)
        {
            var referenceTotal = 0L;
            foreach (var integrity in _integrityUpdatetor.Where(x => x.UpdateAllowed(updatedValue)))
                if (integrity.UpdateAllowed(updatedValue))
                    referenceTotal += await integrity.GetReferenceCount(_generalUnitOfWork, updatedValue);
            return referenceTotal;
        }

        #endregion

        public string[] FindMissingIntegrityOperators<TDal, TReference>(Assembly assembly)
        {
            var allDalTypes = assembly.GetTypes().Where(x => IsOfType(x.GetTypeInfo(), typeof(TDal))).ToArray();
            var allReferences = assembly.GetTypes().Where(x => IsOfType(x.GetTypeInfo(), typeof(TReference))).ToArray();

            return allDalTypes
                .SelectMany(dalType => ScanType(assembly, dalType, allReferences, dalType))
                .ToArray();
        }

        #region Private Methods

        private bool IsOfType(TypeInfo x, Type type)
        {
            return !x.IsInterface && x.IsPublic && !x.IsAbstract && type.IsAssignableFrom(x.AsType());
        }

        private IEnumerable<string> ScanType(Assembly assembly, Type dalType, Type[] allReferences, Type className,
            string prefix = "")
        {
            var propertyInfos = dalType.GetProperties();
            foreach (var property in propertyInfos)
            {
                var memberString = prefix + property.Name;
                if (allReferences.Contains(property.PropertyType) &&
                    !_integrityUpdatetor.Any(x => x.IsIntegration(className, memberString)))
                {
                    yield return $"Missing {property.Name} on {className.Name} " +
                                 $"[ new PropertyIntegrity<{property.PropertyType.Name.Replace("Reference", "")}, {property.PropertyType.Name}, {className}>" +
                                 $"(u => u.{memberString}, g => g.{className}s,r => x => x.{memberString}.Id == r.Id, x=>x.ToReference()) ]";
                }
                else
                {
                    var typeInfo = property.PropertyType.GetTypeInfo();
                    if (typeInfo.IsClass && typeInfo.Assembly == assembly)
                        foreach (var resultString in ScanType(assembly, property.PropertyType, allReferences, className,
                            property.Name + "."))
                            yield return resultString;
                }
            }
        }

        #endregion
    }
}