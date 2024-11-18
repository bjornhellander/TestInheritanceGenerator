// Copyright © Björn Hellander 2024
// Licensed under the MIT License. See LICENSE.txt in the repository root for license information.

using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace TestInheritanceGenerator
{
    internal class TestClassCollector : SymbolVisitor
    {
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

        private static bool IsTestType(INamedTypeSymbol symbol, out string? attributeName)
        {
            foreach (var attr in symbol.GetAttributes())
            {
                // TODO: Support other frameworks?
                if (attr.AttributeClass?.Name == "TestClassAttribute")
                {
                    // TODO: Avoid allocation?
                    attributeName = attr.AttributeClass.ToDisplayString();
                    return true;
                }
            }

            attributeName = null;
            return false;
        }
    }
}
