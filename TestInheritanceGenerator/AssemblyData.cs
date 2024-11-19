using System.Collections.Immutable;

namespace TestInheritanceGenerator
{
    internal record AssemblyData(
        string NamePrefix,
        string Version,
        string BaseAssemblyVersion,
        ImmutableArray<TestTypeData> TestTypes); // TODO: ImmutableArray does not have value semantics. Review!
}
