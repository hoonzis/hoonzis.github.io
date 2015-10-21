Json.NET handles most of the F# types correctly but we still have few issues with:

- Lists
- Options
- Discriminated Unions

Luckily enough there are quite a lot of examples on the internet of convertors for each of these types, that one can just plug into Json.NET. I have personally tweaked a bit the Discriminated Union convertor a bit because it didn't cover all the scenarios and it intervene with other serialization types.

The first issue is that List is implemented as discriminated union so using the DU converter from the aboved mentioned source would affect the way lists are converted and the List convertor would not be used. This can be easily solved by changing the *CanSerialize* method.

Other issue that I have encounter was linked to different types of Discriminated Unions and the fact that not all of them would be correctly handled by the convertor above.

There are two ways that would not be handled correctly:
- using Discriminated Union instead of enumeration
- using DU to discriminate between 2 or more simple record types.

```Fsharp
type OptionType =
  | Call
  | Put
```
In this first case I would expect a serialization and deserialization into a single strike value.

Here is the second case:
```FSharp
type OptionLeg = {
  Strike:float
  Expiration:DateTime
}

type CashLeg = {
  Price:float
}

type Leg =
  | Cash of CashLeg
  | Option of Optionleg
```
This should be serialized simply as a json object. Since Json.NET already handles records we just need to tell him to serialize the object of the given case.

The following JSON should be automatically serialized into OptionLeg even if **Leg** type is expected.
```
{
  Strike:100,
  Expiration: new Date()
}
```
During the deserialization however the convertor should automatically determine which case was passed in by looking at the fields and deserialize into that concrete case.
