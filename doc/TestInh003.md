## TestInh003 - No tests in base assembly

## Cause

The assembly detected as the base assembly did not contain any supported test classes.

## Rule description

This rule currently detects MSTest, xUnit and NUnit tests.
A violation of this rule occurs when no such tests can be found.

## How to fix violations

To fix a violation of this rule:
* Add test classes,
* Change to one of the supported test frameworks, or
* Create an issue to add support for a new test framework
