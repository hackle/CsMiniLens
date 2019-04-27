**Note: this is still a prototype**

```csharp
var person = new Person();
person.Address.Street = "Queen";
```

This innocuous piece of code fails with a `NullReferenceException`. To avoid it, we need to do something like this,

```csharp
if (person != null && person.Address != null) {
    person.Address.Street = "Queen";
}
```

Imagine with more nesting, how tedious this can get!

`Lens` makes this easier.

```csharp
var lens = Lens.For<Person, String>(p => p.Address.Street);

lens.Set(new Person(), "Queen"); // no exception, street not set
lens.Set(new Person { Address = new Address { Street = "Sale" } }, "Queen"); // street is set to "Queen"
```

## Operations

### View

Very similar to `person?.Address?.Street` except with lens there is no need to use `?.`, or coalesce value types like `person?.Address?.Number ?? 0`.

### Set

```csharp
// set street of a person's address
Lens.For<Person, String>(p => p.Address.Street).Set(new Person(), "Queen");
```

Safely sets value to a target field. If there is a null value in between, setting won't happen.

Note though a new `Person` instance will be created - to avoid this, use `SetInline`. 

### Over

```csharp
// override street of a person's address
Lens.For<Person, String>(p => p.Address.Street).Set(new Person(), street => street + " on Queen");
```

`Over` takes the existing value of a field and modifies it.

### SetInline

Works just like `Set` except `SetInline` won't create new instances for each modified containing value.

### OverInline

Works just like `Over` except `OverInline` won't create new instances for each modified containing value.

### Insert

Works like `Set` except `Insert` guarantees that the value is set, through creating instances for each null value in between. Goes without saying that types of those values need to have a parameter-less constructor.

### SetBoth

```csharp
var lens1 = LensFor<Person, string>(p => p.Address.Street);
var lens2 = LensFor<Person, string>(p => p.Mother.Address.Street);
Lens.SetBoth(lens1, lens2, "Queen");
```

"Queen" is set for targets of both lens'es.


### Sync

```csharp
var lens1 = LensFor<Person, string>(p => p.Address.Street);
var lens2 = LensFor<Person, string>(p => p.Mother.Address.Street);
Lens.Sync(lens1, lens2);
```

The `Street` is sync'ed from the target of `lens1` to `lens2`.
This gives us ideas - can it work as if they are Observables? 