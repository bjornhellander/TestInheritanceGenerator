## TestInh001 - Incorrect assembly name

## Cause

The assembly name of the project referencing this package does not meet the expectations.

## Rule description

For this package to be able to generate test classes in a project, the project assembly name must end with a version number, like one of these:
* Example.Test.V1
* Example.Test.V1_2
* Example.Test.V1_2_3
* Example.Test.V1_2_3_4

A violation of this rule occurs when the version can not be identified from the assembly name.

## How to fix violations

To fix a violation of this rule, change the assembly name according to the description above.
This is typically done by renaming the project.
