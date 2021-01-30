using System;
using System.Collections.Generic;
using System.Linq;
using Artemis.Core.DataModelExpansions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Artemis.Core.Services
{
    internal class DataModelResolver : DefaultContractResolver
    {
        #region Overrides of DefaultContractResolver

        /// <inheritdoc />
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            IList<JsonProperty> props = base.CreateProperties(type, memberSerialization);
            return props.Where(p => !p.AttributeProvider.GetAttributes(typeof(DataModelIgnoreAttribute), true).Any()).ToList();
        }

        #endregion
    }
}
