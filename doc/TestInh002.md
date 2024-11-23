## TestInh002 - No base assembly

## Cause

The package is unable to find the test assembly which contains the tests to inherit from.

## Rule description

To be able to find the test classes to inherit from, the assembly containing the tests must be referenced
from the project referencing this package and it's name must match the assembly name of this project, like this:

* Example.Test.V2 -> Example.Test.V1
* Example.Test.V2_0 -> Example.Test.V1_3

A violation of this rule occurs when the package can not figure out which assembly to extract test classes from.

## How to fix violations

To fix a violation of this rule, make sure the "base" assembly is referenced and that their names match.
