---
layout: post
title: JSON serialization with F#
date: '2015-10-24T09:44:00.000-07:00'
author: Jan Fajfr
tags:
- fsharp, JSON, Json.NET
modified_time: '2015-10-24T01:07:44.866-07:00'
---

This post is about F# types serialization using Json.NET library. Luckily enough Json.NET handles most of the F# types correctly but there are still few issues. Typically you can have problems with the F# specific types.
Luckily enough there are quite a lot of examples on the internet of convertors for each of these types, that one can just plug into Json.NET. Here is a short list with an example of convertor that one can find on the web.

- Lists [Lev Gorodinski's blog covers options and lists](http://gorodinski.com/blog/2013/01/05/json-dot-net-type-converters-for-f-option-list-tuple/)
- Options
- Discriminated Unions [Isaac Abraham](https://gist.github.com/isaacabraham/ba679f285bfd15d2f53e)

Discriminated Union is probably the most "complex" type and as such it will also be very hard to find a versatile convertor which would handle all cases as expected. Since a DU can contain really a lot different heterogeneous data one should really first decide what is the expected behavior. I took the snippet from the web as the base and started to tweak it. At the end I have ended with something quote different.

I am using Discriminated Union in several different ways here are typical 3 examples:
- using Discriminated Union instead of enumerations
- using DU to discriminate between 2 or more simple record types
- using DU with tuples, usually to discriminate between quite heterogeneous types

### Discriminated Union as enumeration
Simple example will make this clear. The DU values have name and don't hold any other type inside.

```ocaml
type Motor =
  | Diesel
  | Electric

type Car = {
  Motor:Motor
  Name:string
}

let test = { Motor=Diesel,Name="VW"}
```
In this  case I would expect a serialization and deserialization into a single string value. Ideally the **test** object should be serialized simply as:

```javascript
  {"Motor":"Diesel","Name":"VW"}
```

### Discriminating only between record types
Here is the second case, which resembles a simple inheritance case from Object Oriented world.

```ocaml
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

let test = {
  Price:10.0
}
```

The compiler will infer the type of test as **CashLeg**. In the resulting Json I would expect a simple JSON object.

```javascript
{
  "Price": "10.0"
}
```
Since Json.NET already handles records in the implementation we can check and if a DU is composed of single record, just serialize the record. During the deserialization however the convertor should automatically determine which case was passed in by looking at the fields and deserialize into that concrete case. This might be a bit tricky, but sounds feasible. The following JSON should be automatically serialized into OptionLeg even if **Leg** type is expected.

```ocaml
{
  Strike:100,
  Expiration: new Date()
}
```

### DU holding different data types
This is the last example which also shows why DU are so cool.

```ocaml
type Result =
        | Error
        | Success of String
        | StrangeError of String
        | SuperSuccess of String*String

let data = SuperSuccess ("All","IsOK")
let json = JsonConvert.SerializeObject(data

json = {"Case":"SuperSuccess","Item1":"All","Item2":"IsOK"}
```
In this case the serialized object should contain the name of the DU case and the serialized vales of the tuple:
The same should work for a DU which is not composed of a tuple but by a single element:

```ocaml
let data = Success "Allright"
let json = JsonConvert.SerializeObject(data)
json = {"Case":"Success","Item1":"Allright"}
```

Now the question is what should the **Error** case be serialized into - if we stick to the first example, it should be just a simple string - but one could probably argue that *{Case:"Error"}* would be better choice. The convertor code is easy to adapt.

```ocaml
let data = Error
let json = JsonConvert.SerializeObject(datas
json = {"Error"}
```

### Conflicts with other convertor
List is implemented as discriminated union, so is the Option type, we have to be careful to specify the usage of our convertor only for types that are concerned. In this case all discriminated unions except the List and Option. This can be easily solved by changing the *CanSerialize* method.

### The code
Now it should be more or less clear what I wanted to achieve. Here is the code for such convertor.

```ocaml
type DuConverter() =
    inherit JsonConverter()

    override __.WriteJson(writer, value, serializer) =
        let unionType = value.GetType()
        let unionCases = fsharpType.GetUnionCases(unionType)
        let case, fields = fsharpValue.GetUnionFields(value, unionType)
        let allCasesHaveValues = unionCases |> Seq.forall (fun c -> c.GetFields() |> Seq.length > 0)

        let distinctCases = unionCases |> Seq.distinctBy (fun c->c.GetFields() |> Seq.map (fun f-> f.DeclaringType))
        let hasAmbigious = (distinctCases |> Seq.length) <> (unionCases |> Seq.length)

        let allSingle = unionCases |> Seq.forall (fun c -> c.GetFields() |> Seq.length = 1)

        match allSingle,fields with
        //simplies case no parameters - just like an enumeration
        | _,[||] -> writer.WriteRawValue(sprintf "\"%s\"" case.Name)
        //all single values - discriminate between record types - so we just serialize the record
        | true,[| singleValue |] -> serializer.Serialize(writer,singleValue)
        //diferent types in same discriminated union - write the case and the items as tuples
        | false,values ->
            writer.WriteStartObject()
            writer.WritePropertyName "Case"
            writer.WriteRawValue(sprintf "\"%s\"" case.Name)
            let valuesCount = Seq.length values
            for i in 1 .. valuesCount do
                let itemName = sprintf "Item%i" i
                writer.WritePropertyName itemName
                serializer.Serialize(writer,values.[i-1])
            writer.WriteEndObject()
        | _,_ -> failwith "Handle this new case"




    override __.ReadJson(reader, destinationType, existingValue, serializer) =
        let parts =
            if reader.TokenType <> JsonToken.StartObject then [| (JsonToken.Undefined, obj()), (reader.TokenType, reader.Value) |]
            else
                seq {
                    yield! reader |> Seq.unfold (fun reader ->
                                         if reader.Read() then Some((reader.TokenType, reader.Value), reader)
                                         else None)
                }
                |> Seq.takeWhile(fun (token, _) -> token <> JsonToken.EndObject)
                |> Seq.pairwise
                |> Seq.mapi (fun id value -> id, value)
                |> Seq.filter (fun (id, _) -> id % 2 = 0)
                |> Seq.map snd
                |> Seq.toArray

        //get simplified key value collection
        let fieldsValues =
            parts
                |> Seq.map (fun ((_, fieldName), (fieldType,fieldValue)) -> fieldName,fieldType,fieldValue)
                |> Seq.toArray
        //all cases of the targe discriminated union
        let unionCases = fsharpType.GetUnionCases(destinationType)

        //the first simple case - this DU contains just simple values - as enum - get the value
        let _,_,firstFieldValue = fieldsValues.[0]

        let fieldsCount = fieldsValues |> Seq.length

        let valuesOnly = fieldsValues |> Seq.skip 1 |> Seq.map (fun (_,_,v) -> v) |> Array.ofSeq

        let foundDirectCase = unionCases |> Seq.tryFind (fun uc -> uc.Name = (firstFieldValue.ToString()))

        let jsonToValue valueType value =
            match valueType with
                                | JsonToken.Date ->
                                    let dateTimeValue = Convert.ToDateTime(value :> Object)
                                    dateTimeValue.ToString("o")
                                | _ -> value.ToString()

        match foundDirectCase, fieldsCount with
            //simpliest case - just like an enum
            | Some case, 1 -> fsharpValue.MakeUnion(case,[||])
            //case is specified - just create the case with the values as parameters
            | Some case, n -> fsharpValue.MakeUnion(case,valuesOnly)
            //case not specified - look up the record type which suites the best
            | None, _ ->
                //this is the second case - this disc union is not of simple value - it may be records or multiple values
                let reconstructedJson = (Seq.fold (fun acc (name,valueType,value) -> acc + String.Format("\t\"{0}\":\"{1}\",\n",name,(jsonToValue valueType value))) "{\n" fieldsValues) + "}"

                //if it is a record lets try to find the case by looking at the present fields
                let implicitCase = unionCases |> Seq.tryPick (fun uc ->
                    //if the case of the discriminated union is a record then this case will contain just one field which will be the record
                    let ucDef = uc.GetFields() |> Seq.head
                    //we need the get the record type and look at the fields
                    let recordType = ucDef.PropertyType
                    let recordFields = recordType.GetProperties()
                    let matched = fieldsValues |> Seq.forall ( fun (fieldName,_,fieldValue) ->
                        recordFields |> Array.exists(fun f-> f.Name = (fieldName :?> string))
                    )    
                    //if we have found a match onthe record let's keep the union case and type of the record
                    match matched with
                        | true -> Some (uc,recordType)
                        | false -> None
                )

                match implicitCase with
                    | Some (case,recordType) ->
                        use stringReader = new StringReader(reconstructedJson)
                        use jsonReader = new JsonTextReader(stringReader)
                        //creating the record - Json.NET can handle that already
                        let unionCaseValue = serializer.Deserialize(jsonReader,recordType)
                        //convert the record to the parent discrimianted union
                        let parentDUValue = fsharpValue.MakeUnion(case,[|unionCaseValue|])
                        parentDUValue
                    | None -> failwith "can't find such disc union type"

    override __.CanConvert(objectType) =
        fsharpType.IsUnion objectType &&
        //it seems that both option and list are implemented using discriminated unions, so we tell json.net to ignore them and use different serializer
        not (objectType.IsGenericType  && objectType.GetGenericTypeDefinition() = typedefof<list<_>>) &&
        not (objectType.IsGenericType  && objectType.GetGenericTypeDefinition() = typedefof<option<_>>) &&
        not (fsharpType.IsRecord objectType)
```
