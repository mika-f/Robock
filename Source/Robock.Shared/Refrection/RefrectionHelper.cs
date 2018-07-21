using System.Reflection;

namespace Robock.Refrection
{
    public static class RefrectionHelper
    {
        public static T GetPrivateField<T>(this object instance, string fieldName)
        {
            var type = instance.GetType();
            var field = type.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField);
            return (T) field?.GetValue(instance);
        }
    }
}