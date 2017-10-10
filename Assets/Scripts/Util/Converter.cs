using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JsonFx.Json;

public class Converter
{
    private Converter() { }

    public static string Serialize(object obj)
    {
        System.Text.StringBuilder output = new System.Text.StringBuilder();

        JsonWriterSettings settings = new JsonWriterSettings();
        settings.PrettyPrint = true;
        settings.AddTypeConverter(new VectorConverter());
        settings.AddTypeConverter(new ColorConverter());
        settings.AddTypeConverter(new QuaternionConverter());

        JsonWriter writer = new JsonWriter(output, settings);
        writer.Write(obj);

        return output.ToString();
    }

    public static T Deserialize<T>(string data)
    {
        data.Trim();
        JsonReaderSettings settings = new JsonReaderSettings();        
        settings.AddTypeConverter(new VectorConverter());
        settings.AddTypeConverter(new ColorConverter());
        settings.AddTypeConverter(new QuaternionConverter());
        JsonReader reader = new JsonReader(data, settings);

        var bi = reader.Deserialize<T>();

        return bi;
    }
}
