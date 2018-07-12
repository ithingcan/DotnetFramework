using System;
using System.Reflection;
using Dotnet.Reflecting;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Dotnet.Serializing
{
    public class CamelCasePropertyNamesContractResolver : Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            ModifyProperty(member, property);

            return property;
        }

        protected virtual void ModifyProperty(MemberInfo member, JsonProperty property)
        {
            if (property.PropertyType != typeof(DateTime) && property.PropertyType != typeof(DateTime?))
            {
                return;
            }

            if (ReflectionHelper.GetSingleAttributeOfMemberOrDeclaringTypeOrDefault<DisableDateTimeNormalizationAttribute>(member) == null)
            {
                property.Converter = new DateTimeConverter();
            }
        }
    }
}