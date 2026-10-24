// Copyright (c) Björn Hellander 2024.
// Licensed under the MIT No Attribution license.

using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.VisualBasic.Testing;

namespace TestInheritanceGenerator.Test.Verifiers
{
    public static partial class VisualBasicAnalyzerVerifier<TAnalyzer>
        where TAnalyzer : DiagnosticAnalyzer, new()
    {
        public class Test : VisualBasicAnalyzerTest<TAnalyzer, DefaultVerifier>
        {
            public Test()
            {
            }
        }
    }
}
