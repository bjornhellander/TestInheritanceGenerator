// Copyright (c) Björn Hellander 2024.
// Licensed under the MIT No Attribution license.

using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestInheritanceGenerator.Test
{
    [TestClass]
    public class XUnitV3GeneratorTests : XUnitGeneratorTests
    {
        protected override ReferenceAssemblies AddReferenceAssemblies(ReferenceAssemblies input)
        {
            var result = input.AddPackages([new PackageIdentity("xunit.v3", "4.0.3")]);
            return result;
        }
    }
}
