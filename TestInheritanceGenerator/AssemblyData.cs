// Copyright © Björn Hellander 2024
// Licensed under the MIT License. See LICENSE.txt in the repository root for license information.


// Copyright © Björn Hellander 2024
// Licensed under the MIT License. See LICENSE.txt in the repository root for license information.

using System.Collections.Immutable;

namespace TestInheritanceGenerator
{
    internal record AssemblyData(string NamePrefix, string Version, string BaseAssemblyVersion, ImmutableArray<TestTypeData> TestTypes);
}
