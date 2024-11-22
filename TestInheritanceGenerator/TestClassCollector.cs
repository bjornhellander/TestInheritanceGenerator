using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace TestInheritanceGenerator
{
    internal class TestClassCollector : SymbolVisitor
    {
        private readonly Dictionary<INamedTypeSymbol, string> attributeTypeNameCache = new(SymbolEqualityComparer.Default);
        private readonly ICollection<TestTypeData> testTypes;

        public TestClassCollector(ICollection<TestTypeData> testTypes)
        {
            this.testTypes = testTypes;
        }

        public override void DefaultVisit(ISymbol symbol)
        {
            throw new NotImplementedException();
        }

        public override void VisitAssembly(IAssemblySymbol symbol)
        {
            Visit(symbol.GlobalNamespace);
        }

        public override void VisitNamespace(INamespaceSymbol symbol)
        {
            foreach (var member in symbol.GetMembers())
            {
                Visit(member);
            }
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            if (symbol.IsAbstract || symbol.IsStatic || symbol.DeclaredAccessibility != Accessibility.Public)
            {
                return;
            }

            if (!IsTestType(symbol, out var attributeName))
            {
                return;
            }

            var testType = new TestTypeData(symbol.Name, symbol.ContainingNamespace.ToString(), attributeName);
            testTypes.Add(testType);
        }

        private bool IsTestType(INamedTypeSymbol symbol, out string? attributeName)
        {
            foreach (var attr in symbol.GetAttributes())
            {
                var attributeClass = attr.AttributeClass;
                if (attributeClass == null)
                {
                    continue;
                }

                if (attributeTypeNameCache.TryGetValue(attributeClass, out var typeName))
                {
                    attributeName = typeName;
                    return true;
                }

                if (attributeClass.Name == "TestClassAttribute")
                {
                    attributeName = attributeClass.ToDisplayString();
                    attributeTypeNameCache.Add(attributeClass, attributeName);
                    return true;
                }
            }

            foreach (var method in symbol.GetMembers().OfType<IMethodSymbol>())
            {
                foreach (var attr in method.GetAttributes())
                {
                    switch (attr.AttributeClass?.Name)
                    {
                        case "FactAttribute":
                        case "TheoryAttribute":
                        case "TestAttribute":
                        case "TestCaseAttriute":
                            attributeName = null;
                            return true;

                        default:
                            break;
                    }
                }
            }

            attributeName = null;
            return false;
        }
    }
}
