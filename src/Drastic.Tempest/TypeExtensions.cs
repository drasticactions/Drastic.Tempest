using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Drastic.Tempest
{
    public static class TypeExtensions
    {
        private static readonly Assembly Tempest = typeof(TypeExtensions).GetTypeInfo().Assembly;
        private static readonly Assembly mscorlib = typeof(string).GetTypeInfo().Assembly;

        public static string GetSimplestName(this Type self)
        {
            if (self == null)
                throw new ArgumentNullException("self");

            var typeInfo = self.GetTypeInfo();
            if (typeInfo.Assembly == mscorlib || typeInfo.Assembly == Tempest)
                return self.FullName;

            //if (!typeInfo.Assembly.GlobalAssemblyCache)
            //    return String.Format("{0}, {1}", self.FullName, typeInfo.Assembly.GetName().Name);

            return self.AssemblyQualifiedName;
        }

        public static bool IsAssignableFrom(this Type baseType, Type derivedType)
        {
            return baseType.GetTypeInfo().IsAssignableFrom(derivedType.GetTypeInfo());
        }

        public static IEnumerable<Type> GetTypes(this Assembly self)
        {
            return self.DefinedTypes.Select(ti => ti.BaseType);
        }

        public static ConstructorInfo GetConstructor(this Type self, Type[] parameterTypes)
        {
            foreach (ConstructorInfo constructor in self.GetTypeInfo().DeclaredConstructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                if (parameters.Length != parameterTypes.Length)
                    continue;

                bool match = true;
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameterTypes[i] != parameters[i].ParameterType)
                    {
                        match = false;
                        break;
                    }
                }

                if (match)
                    return constructor;
            }

            return null;
        }
    }
}
