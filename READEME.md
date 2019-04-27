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

### Set

### Over

### SetInline

### OverInline

### Insert

### SetBoth

### ViewBoth

### Sync