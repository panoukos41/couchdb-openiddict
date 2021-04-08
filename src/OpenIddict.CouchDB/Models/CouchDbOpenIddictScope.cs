using CouchDB.Driver.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;

namespace OpenIddict.CouchDB.Models
{
    /// <summary>
    /// Represents an OpenIddict scope.
    /// </summary>
    [DebuggerDisplay("Id = {Id.ToString(),nq} ; Name = {Name,nq}")]
    public class CouchDbOpenIddictScope : CouchDocument
    {
        /// <summary>
        /// Initialize a new <see cref="CouchDbOpenIddictToken"/>.
        /// </summary>
        public CouchDbOpenIddictScope()
        {
        }

        /// <summary>
        /// Initialize a new <see cref="CouchDbOpenIddictScope"/> from another.
        /// </summary>
        public CouchDbOpenIddictScope(CouchDbOpenIddictScope other)
        {
            Id = other.Id;
            Rev = other.Rev;
            Description = other.Description;
            Descriptions = other.Descriptions;
            DisplayName = other.DisplayName;
            DisplayNames = other.DisplayNames;
            Name = other.Name;
            Properties = new JObject(other.Properties);
            Resources = other.Resources;
        }

        /// <summary>
        /// Gets or sets the public description associated with the current scope.
        /// </summary>
        [JsonProperty("description")]
        public virtual string? Description { get; set; }

        /// <summary>
        /// Gets or sets the localized public descriptions associated with the current scope.
        /// </summary>
        [JsonProperty("descriptions")]
        public virtual IReadOnlyDictionary<CultureInfo, string> Descriptions { get; set; }
            = ImmutableDictionary.Create<CultureInfo, string>();

        /// <summary>
        /// Gets or sets the display name associated with the current scope.
        /// </summary>
        [JsonProperty("display_name")]
        public virtual string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the localized display names associated with the current scope.
        /// </summary>
        [JsonProperty("display_names")]
        public virtual IReadOnlyDictionary<CultureInfo, string> DisplayNames { get; set; }
            = ImmutableDictionary.Create<CultureInfo, string>();

        /// <summary>
        /// Gets or sets the unique name associated with the current scope.
        /// </summary>
        [JsonProperty("name")]
        public virtual string? Name { get; set; }

        /// <summary>
        /// Gets or sets the additional properties associated with the current scope.
        /// </summary>
        [JsonProperty("properties")]
        public virtual JObject? Properties { get; set; }

        /// <summary>
        /// Gets or sets the resources associated with the current scope.
        /// </summary>
        [JsonProperty("resources")]
        public virtual IReadOnlyList<string> Resources { get; set; } = ImmutableList.Create<string>();
    }
}