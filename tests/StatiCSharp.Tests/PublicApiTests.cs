using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using StatiCSharp.Interfaces;
using Xunit;

namespace StatiCSharp.Tests;

/// <summary>
/// Guards decisions about the public surface that a compiler error elsewhere would not catch.
/// </summary>
public class PublicApiTests
{
    [Fact]
    public void NoPublicMemberHandsOutAMutableCollection()
    {
        // A theme should not be able to write into what it is rendering. The default theme used
        // to sort section.Items in place while building a page, so a section came out of a
        // render in a different order than it went in - possible only because the list was
        // handed over as a List<T>.
        Type[] mutable = [typeof(List<>), typeof(ICollection<>), typeof(IList<>), typeof(ISet<>), typeof(IDictionary<,>)];

        static bool IsMutable(Type type, Type[] mutable)
            => type.IsGenericType && mutable.Contains(type.GetGenericTypeDefinition());

        List<string> offenders = [];

        foreach (Type type in typeof(IWebsite).Assembly.GetExportedTypes())
        {
            foreach (PropertyInfo property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                if (IsMutable(property.PropertyType, mutable))
                {
                    offenders.Add($"{type.Name}.{property.Name} is a {property.PropertyType.Name}");
                }
            }

            foreach (MethodInfo method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static))
            {
                foreach (ParameterInfo parameter in method.GetParameters())
                {
                    if (IsMutable(parameter.ParameterType, mutable))
                    {
                        offenders.Add($"{type.Name}.{method.Name}({parameter.Name}) takes a {parameter.ParameterType.Name}");
                    }
                }

                if (IsMutable(method.ReturnType, mutable))
                {
                    offenders.Add($"{type.Name}.{method.Name} returns a {method.ReturnType.Name}");
                }
            }
        }

        Assert.Empty(offenders);
    }

    [Fact]
    public void ASectionCanOnlyGrowThroughAddItem()
    {
        var section = new Section { SectionName = "posts" };

        section.AddItem(new Item { Title = "older", Date = new DateOnly(2024, 1, 1) });
        section.AddItem(new Item { Title = "newer", Date = new DateOnly(2026, 1, 1) });

        // Kept in order by the insert, so a theme does not have to guess.
        Assert.Equal(["newer", "older"], section.Items.Select(item => item.Title));
    }
}
