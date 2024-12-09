using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TractorSupporter.Tests;

public static class TestsHelper
{
    public static void SetPrivateField<T>(object obj, string fieldName, T value)
    {
        var field = obj.GetType().GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
        if (field == null) throw new InvalidOperationException($"Field {fieldName} not found.");

        field.SetValue(obj, value);
    }

    public static T GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType()
            .GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field == null)
            throw new InvalidOperationException($"Field {fieldName} was not found.");

        return (T)field.GetValue(obj)!;
    }
}
