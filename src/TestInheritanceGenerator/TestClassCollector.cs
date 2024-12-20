﻿using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace TestInheritanceGenerator
{
    internal class TestClassCollector : SymbolVisitor
    {
        private readonly Dictionary<INamedTypeSymbol, string> attributeTypeNameCache = new(SymbolEqualityComparer.Default);
        private readonly ValueSemanticsList<TestTypeData> testTypes;

        public TestClassCollector(ValueSemanticsList<TestTypeData> testTypes)
        {
            this.testTypes = testTypes;
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
            if (symbol.IsAbstract || symbol.IsStatic)
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
            INamedTypeSymbol? testAttributeType = null;
            foreach (var attr in symbol.GetAttributes())
            {
                var attributeClass = attr.AttributeClass;
                if (attributeClass == null)
                {
                    continue;
                }

                switch (attributeClass.Name)
                {
                    case "TestClassAttribute":
                        testAttributeType = attributeClass;
                        break;

                    case "SkipInheritanceGenerationAttribute":
                        attributeName = null;
                        return false;
                }
            }

            if (testAttributeType != null)
            {
                if (!attributeTypeNameCache.TryGetValue(testAttributeType, out var typeName))
                {
                    typeName = testAttributeType.ToDisplayString();
                    attributeTypeNameCache.Add(testAttributeType, typeName);
                }

                attributeName = typeName;
                return true;
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
                        case "TestCaseAttribute":
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
