Json.NET handles most of the F# types correctly but we still have few issues with:

- Lists
- Options
- Discriminated Unions

Luckily enough there are quite a lot of examples on the internet of convertors for each of these types, that one can just plug into Json.NET. I have personally tweaked a bit the Discriminated Union convertor a bit because it didn't cover all the scenarios and it intervene with other serialization types.

The first issue is that List is implemented as discriminated union so using the DU converter from the aboved mentioned source would affecet the way lists are converted and the List convererter would not be used. This can be easily solved by changing the *CanSerialize* method.

Other issue that I have encoutered was linked to different types of Discriminated Unions and the fact that not all of them would be defined by the convertor above. For instance I am using a DU just for discriminating between two record types. And such value nested in another record.

```

```

This should be serialized simply as a json object. During the deserialization the convertor should automatically determine which case was passed in by looking at the fields.
