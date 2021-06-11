using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Artemis.Core.Modules;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Artemis.Plugins.WebAPI.Json
{
    internal class DataModelResolver : DefaultContractResolver
    {
        #region Overrides of DefaultContractResolver

        /// <inheritdoc />
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            return props.Where(p => p.PropertyType == typeof(ReadOnlyDictionary<string, DataModel>) || !p.AttributeProvider.GetAttributes(typeof(DataModelIgnoreAttribute), true).Any()).ToList();
        }

        #endregion
    }
}
