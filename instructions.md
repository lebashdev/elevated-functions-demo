# Introduction

Say we have a ```Toxic<T>``` type, which acts as a safe container around a value of type ```T``` and isolates them from non-toxic values.  All operations against those "toxic" values should be performed within the special container in order to not contaminate the rest of the logic.

We can trap values into the container with the following code:

```csharp
var toxic = Toxic.FromValue(2);
```

Once a value has been put into a ```Toxic``` container, it should never leave it.

Given that information, let's think about how we can perform operation against the trapped values.

# Transformation functions

Let's look at some functions:

```Increment()``` takes an integer, adds one to it, then returns the result as a normal integer.

```csharp
int Increment(int x) => x + 1;
```

```ToxicIncrement()``` also takes an integer, adds one to it, but returns a ```Toxic<int>``` rather than a normal integer.

```csharp
Toxic<int> ToxicIncrement(int x) => Toxic.FromValue(x + 1);
```

```wrappedIncrement``` is the same function as ```Increment()```, but has itself been trapped in a ```Toxic``` container.

```csharp
var wrappedIncrement = Toxic.FromValue<Func<int, int>>(Increment);
```

```Add()``` is a normal function which takes two integers and returns their sum as a normal integer.

```csharp
int Add(int x, int y) => x + y;
```

Those functions are all variations of the same thing, with different signatures.  They all take at least one integer, apply a computation to it, and return an integer of some form (either a normal value, or a trapped value).

# Exercise

The goal of this exercise is to figure out how to apply each of the four functions to a ```Toxic<T>``` (or a pair of ```Toxic<T>``` values in the case of ```Add()```).

Start with this code:

```csharp
var a = Toxic.FromResult(2);
var b = Toxic.FromResult(2);

int Increment(int x) => x + 1;
Toxic<int> ToxicIncrement(int x) => Toxic.FromValue(x + 1);
var wrappedIncrement = Toxic.FromValue<Func<int, int>>(Increment);
int Add(int x, int y) => x + y;
```

## Constraints

 - Because ```a``` and ```b``` are toxic values, you should not try to pull them outside of their containers.  You should apply the transformations inside the toxic contexts.  Conversely, the results of those computation are going to be inherently toxic, therefore they should be trapped in toxic containers themselves.  In other words, all four functions will ultimately take toxic integers in, and return toxic integers.
- It is OK to "transfer" a value from one toxic container to another, as long as the transfer occurs within a toxic context.
- You may not change the way the four functions are implemented.  You may however write code around the functions, which will affect how the functions are invoked.
- The resulting code should be type-safe (no reflection, serialization, etc.)

## Example

If you have a value ```Toxic(1)```, and you apply ```Increment()``` to it, the result should be ```Toxic(2)```.  Because ```Increment()``` only accepts a regular integer as its input, you need to figure out how to pass it a ```Toxic<int>``` instead.  And because ```Increment()``` returns a regular integer, you also need to figure out how to get back a ```Toxic<int>``` from that computation.  That will involve writing one or more functions which will map from one invocation model to the other.

## API

The ```Toxic<T>``` type exposes a simple API, which you should use to suport your solution.

| Name | Description |
| ---- | ---- |
| ```FromValue(value: T) => Toxic<T>``` | The static method traps a regular value in a ```Toxic``` container |
| ```PassInto(f: (value: T) => U) => Toxic<U>``` | Passes the value trapped within the toxic container to the specified lambda and returns the result as a ```Toxic<T>```.  This is the only way to safely access the trapped value as it executes the transformation inside the toxic context rather than outside. |
| ```Unwrap() => Toxic<T>``` | Flattens a nested ```Toxic<Toxic<T>>``` value into a ```Toxic<T>``` |
| ```ToString() => string``` | Display the trapped value as a string.  Use this to print the results of the various invocations in the console. |