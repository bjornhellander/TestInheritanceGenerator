// Copyright (c) Björn Hellander 2024.
// Licensed under the MIT No Attribution license.

using System;

namespace TestInheritanceGenerator.Annotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SkipInheritanceGenerationAttribute : Attribute
    {
    }
}
