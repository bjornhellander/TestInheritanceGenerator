// Copyright (c) Björn Hellander 2024.
// Licensed under the MIT No Attribution license.

using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Testing;
using Microsoft.CodeAnalysis.VisualBasic.Testing;

namespace TestInheritanceGenerator.Test.Verifiers
{
    public static partial class VisualBasicCodeRefactoringVerifier<TCodeRefactoring>
        where TCodeRefactoring : CodeRefactoringProvider, new()
    {
        public class Test : VisualBasicCodeRefactoringTest<TCodeRefactoring, DefaultVerifier>
        {
        }
    }
}
