using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

//util

public class QuaternionConverter : JsonConverter
{
    public override bool CanConvert(System.Type type)
    {
        return type == typeof(Quaternion);
    }

    public override Dictionary<string, object> WriteJson(System.Type type, object value)
    {
        if (!CanConvert(type))
            throw new System.NotImplementedException("Can only read Quaternion Not objects of type " + (object)type);

        Quaternion v = (Quaternion)value;
        Dictionary<string, object> dict = new Dictionary<string, object>();
        dict.Add("x", v.x);
        dict.Add("y", v.y);
        dict.Add("z", v.z);
        dict.Add("w", v.w);
        return dict;
    }

    public override object ReadJson(System.Type type, Dictionary<string, object> value)
    {
        if (!CanConvert(type))
            throw new System.NotImplementedException("Can only read Quaternion Not objects of type " + (object)type);

        Quaternion v = new Quaternion(CastFloat(value["x"]), CastFloat(value["y"]), CastFloat(value["z"]), CastFloat(value["w"]));
        return v;
    }
} // class QuaternionConverter

public class VectorConverter : JsonConverter
{
    public override bool CanConvert(System.Type type)
    {
        return type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4);
    }

    public override object ReadJson(System.Type type, Dictionary<string, object> values)
    {
        if (object.Equals((object)type, (object)typeof(Vector2)))
            return (object)new Vector2(this.CastFloat(values["x"]), this.CastFloat(values["y"]));
        if (object.Equals((object)type, (object)typeof(Vector3)))
            return (object)new Vector3(this.CastFloat(values["x"]), this.CastFloat(values["y"]), this.CastFloat(values["z"]));
        if (object.Equals((object)type, (object)typeof(Vector4)))
            return (object)new Vector4(this.CastFloat(values["x"]), this.CastFloat(values["y"]), this.CastFloat(values["z"]), this.CastFloat(values["w"]));
        throw new System.NotImplementedException("Can only read Vector2,3,4. Not objects of type " + (object)type);
    }

    public override Dictionary<string, object> WriteJson(System.Type type, object value)
    {
        if (object.Equals((object)type, (object)typeof(Vector2)))
        {
            Vector2 vector2 = (Vector2)value;
            return new Dictionary<string, object>() { { "x", (object)vector2.x }, { "y", (object)vector2.y } };
        }
        if (object.Equals((object)type, (object)typeof(Vector3)))
        {
            Vector3 vector3 = (Vector3)value;
            return new Dictionary<string, object>() { { "x", (object)vector3.x }, { "y", (object)vector3.y }, { "z", (object)vector3.z } };
        }
        if (!object.Equals((object)type, (object)typeof(Vector4)))
            throw new System.NotImplementedException("Can only write Vector2,3,4. Not objects of type " + (object)type);

        Vector4 vector4 = (Vector4)value;
        return new Dictionary<string, object>() { { "x", (object)vector4.x }, { "y", (object)vector4.y }, { "z", (object)vector4.z }, { "w", (object)vector4.w } };
    }
} // class VectorConverter

public class ColorConverter : JsonConverter
{
    public override bool CanConvert(System.Type type)
    {
        return type == typeof(Color);
    }


    public override object ReadJson(System.Type type, Dictionary<string, object> values)
    {
        if (object.Equals((object)type, (object)typeof(Color)))
            return (object)new Color(this.CastFloat(values["r"]), this.CastFloat(values["g"]), this.CastFloat(values["b"]), this.CastFloat(values["a"]));
        throw new System.NotImplementedException("Can only read Color Not objects of type " + (object)type);
    }

    public override Dictionary<string, object> WriteJson(System.Type type, object value)
    {
        if (!object.Equals((object)type, (object)typeof(Color)))
            throw new System.NotImplementedException("Can only read Color Not objects of type " + (object)type);

        Color color = (Color)value;
        return new Dictionary<string, object>() { { "r", (object)color.r }, { "g", (object)color.g }, { "b", (object)color.b }, { "a", (object)color.a } };
    }
} // class ColorConverter
