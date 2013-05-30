namespace Microsoft.WPSync.UI.Utilities
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Reflection;

    public static class ReflectionHelper
    {
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId="0"), SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId="obj")]
        public static PropertyInfo GetPropertyForObject(object obj, string propertyName)
        {
            return obj.GetType().GetProperty(propertyName);
        }

        private static bool InterfaceFilter(Type typeObj, object criteriaObj)
        {
            return (typeObj.ToString() == criteriaObj.ToString());
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId="0")]
        public static bool InterfacePresentInType(Type checkType, Type compareType)
        {
            TypeFilter filter = new TypeFilter(ReflectionHelper.InterfaceFilter);
            Type[] typeArray = checkType.FindInterfaces(filter, compareType);
            return ((typeArray != null) && (typeArray.Length > 0));
        }
    }
}

