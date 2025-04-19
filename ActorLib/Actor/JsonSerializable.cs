using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ActorLib.Actor
{
    public interface IJsonSerializable
    {
    }

    public static class JsonSerializableExtensions
    {
        public static string ToJsonString(this IJsonSerializable obj)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = false,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            };
            return JsonSerializer.Serialize(obj, obj.GetType(), options);
        }
    }
}
