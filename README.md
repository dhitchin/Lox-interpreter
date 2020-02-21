# Lox-interpreter

This repo is for my implementations of the "jlox" interpreter and "clox" compiler from [Crafting Interpreters](https://craftinginterpreters.com/).

## C# interpreter

This interpreter is very similar to the "jlox" interpreter outlined for Java in the Crafting Interpreters book.  There are some changes to variable names, and some syntax changes to make it more "C#-esque".

Some changes include:
  * Use of text templates for code generation instead of an external tool.
  * Addition of '++' and '--' postfix operators

TODO:
 * Ternary operator
 * Additional built-in functions
 * Bugfixes when dealing with variables and closures
