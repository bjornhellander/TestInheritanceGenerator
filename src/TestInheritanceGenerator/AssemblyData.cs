﻿namespace TestInheritanceGenerator
{
    internal record AssemblyData(
        string NamePrefix,
        string Version,
        string BaseAssemblyVersion,
        ValueSemanticsList<TestTypeData> TestTypes);
}