using System;
using System.Linq;
using System.Reflection;

[AttributeUsage(AttributeTargets.Field)]
public class InheritablePropertyAttribute : Attribute { }

static public class Inheritor
{
    static public void Inherit<T>(T child, T parent)
    {
        var ipaType = typeof(InheritablePropertyAttribute);
        var allFields = typeof(T).GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        var inheritableFields = allFields.Where(f => f.GetCustomAttributes(false).Any(a => a.GetType() == ipaType));
        if (inheritableFields.Count() == 0) {
            var msg = string.Format(
                "Cannot call Inherit on object of type {0} because none of its fields are tagged with {1}.", 
                typeof(T), typeof(InheritablePropertyAttribute));
            throw new InvalidOperationException(msg);
        }
        foreach (var field in inheritableFields) {
            var childValue = field.GetValue(child);
            if (childValue == null || childValue.GetType().IsValueType && childValue == Activator.CreateInstance(childValue.GetType())) {
                field.SetValue(child, field.GetValue(parent));
            }
        }
    }
}
