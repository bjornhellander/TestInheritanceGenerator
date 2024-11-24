using System;

namespace TestInheritanceGenerator.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SkipInheritanceGenerationAttribute : Attribute
    {
    }
}
