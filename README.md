<div align=right>
 ![csp.net](assets/icon)
</div>

![dotnetcore](https://www.microsoft.com/net/images/redesign/downloads-dot-net-core.svg?v=U_8I9gzFF2Cqi5zUNx-kHJuou_BWNurkhN_kSm3mCmo)
Csp.net is a programming language that runs on .NET Core.

## Why Csp.net?
Csp.net is more suitable for beginners to get started.

Csp.net is more suitable for writing scripting programs. Compared with similar languages:

Language | Rich library support | Convenience of scripts
------ | ------ | ------
C# and other .NET languages |   | The amount of code is greatly reduced, which is more suitable for script scenarios.
Python, Command and other scripting languages | Interoperate with .NET. It has a unique advantage on Windows. |  

## Grammar specification

### File identifier
*Optional*

`UsingStatement` is used to introduce namespaces.
```
using System
using System.IO
```
***Required***

`NamespaceName` is used to name the current program.

*The Complier will interpret the name as the namespace name of the class (if there are multiple levels) and the name of the class.*
```
namespace MyFirstCsp.Program
```
***Required***

`MainFunc` is used to identify the program entry point. The parameters of the `MainFunc` can be empty `()` or String[] type parameters `(args)`, the returnType can be empty ` ` or int type `: int`.
```
main = {

} 
```
or
```
main = (): int{

} 
```
or
```
main = (args){

} 
```
or
```
main = (args): int{

} 
```

### Statements

In the pair of curly braces of the main function, the following statements are supported.

Statement | Form
------ | ------
Var | var `Varible`~~: `Type`~~ ~~= `Value`~~     *(There is always one that is necessary in `Type` and `Value`.)*
FuncCall | Obj.Func(Paras)
Return | return `Value`
++-- | `Field` `++ | --`
= | `Field` = `Value`

### Operation

When a value is required, the following operations are supported.

Operation | Form
------ | ------
+-*/ | `Value` `+ | - | * | /` `Value`
= | `Field` = `Value`
FuncCall | Obj.Func(Paras)

## Principle

The core algorithm is not a compilation principle, but a new algorithm that constructs a syntax tree recursively with a regular expression as the core.

### Analysis

![demo](assets/code)
->
![result](assets/tree)

### Algorithm
