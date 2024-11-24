# InheritedTestsGenerator

## Purpose

This source generator package generates test class "stubs" which inherit from test classes in a referenced test project.
The typical use case is if you have test projects per version of some software component and you want all tests
for the previous version to be executed for the next version as well. This is accomplished automatically by this
package by generating a file for each found test class in the referenced test project and by adding inheritance
to the original class.

## Usage

Add a reference to this nuget package in the package you want to inherit test into and
also reference the test project for the previous version.

It is possible to disable inheritance generation for specific test classes by adding an attribute called SkipInheritanceGenerationAttribute.

## Supported test frameworks

The package has support for the following test frameworks:
* MSTest
* xUnit
* NUnit

## Analyzers

The following diagnostics are reported as guidance, if no tests are being generated:
* [TestInherit001 - Incorrect assembly name](https://github.com/bjornhellander/TestInheritanceGenerator/blob/master/doc/TestInh001.md)
* [TestInherit002 - No base assembly](https://github.com/bjornhellander/TestInheritanceGenerator/blob/master/doc/TestInh002.md)
* [TestInherit003 - No tests in base assembly](https://github.com/bjornhellander/TestInheritanceGenerator/blob/master/doc/TestInh003.md)

## Credits

This work is heavily inspired by a similar source generator implemented by [Sam Harwell](https://github.com/sharwell) in [StyleCop.Analyzers](https://github.com/DotNetAnalyzers/StyleCopAnalyzers).
