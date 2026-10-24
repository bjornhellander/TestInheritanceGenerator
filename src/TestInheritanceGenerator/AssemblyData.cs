// Copyright (c) Björn Hellander 2024.
// Licensed under the MIT No Attribution license.

namespace TestInheritanceGenerator
{
    internal record AssemblyData(
        string NamePrefix,
        string Version,
        string BaseAssemblyVersion,
        ValueSemanticsList<TestTypeData> BaseTestTypes,
        ValueSemanticsList<TestTypeData> CurrTestTypes);
}
