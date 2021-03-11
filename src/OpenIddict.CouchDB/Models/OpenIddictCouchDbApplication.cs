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
    /// Represents an OpenIddict application.
    /// </summary>
    [DebuggerDisplay("Id = {Id.ToString(),nq} ; ClientId = {ClientId,nq} ; Type = {Type,nq}")]
    public class OpenIddictCouchDbApplication : CouchDocument
    {
        /// <summary>
        /// Gets or sets the client identifier associated with the current application.
        /// </summary>
        [JsonProperty("client_id")]
        public virtual string? ClientId { get; set; }

        /// <summary>
        /// Gets or sets the client secret associated with the current application.
        /// Note: depending on the application manager used to create this instance,
        /// this property may be hashed or encrypted for security reasons.
        /// </summary>
        [JsonProperty("client_secret")]
        public virtual string? ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the consent type associated with the current application.
        /// </summary>
        [JsonProperty("consent_type")]
        public virtual string? ConsentType { get; set; }

        /// <summary>
        /// Gets or sets the display name associated with the current application.
        /// </summary>
        [JsonProperty("display_name")]
        public virtual string? DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the localized display names associated with the current application.
        /// </summary>
        [JsonProperty("display_names")]
        public virtual IReadOnlyDictionary<CultureInfo, string> DisplayNames { get; set; }
            = ImmutableDictionary.Create<CultureInfo, string>();

        /// <summary>
        /// Gets or sets the permissions associated with the current application.
        /// </summary>
        [JsonProperty("permissions")]
        public virtual IReadOnlyList<string> Permissions { get; set; } = ImmutableList.Create<string>();

        /// <summary>
        /// Gets or sets the logout callback URLs associated with the current application.
        /// </summary>
        [JsonProperty("post_logout_redirect_uris")]
        public virtual IReadOnlyList<string> PostLogoutRedirectUris { get; set; } = ImmutableList.Create<string>();

        /// <summary>
        /// Gets or sets the additional properties associated with the current application.
        /// </summary>
        [JsonProperty("properties")]
        public virtual JObject? Properties { get; set; }

        /// <summary>
        /// Gets or sets the callback URLs associated with the current application.
        /// </summary>
        [JsonProperty("redirect_uris")]
        public virtual IReadOnlyList<string> RedirectUris { get; set; } = ImmutableList.Create<string>();

        /// <summary>
        /// Gets or sets the requirements associated with the current application.
        /// </summary>
        [JsonProperty("requirements")]
        public virtual IReadOnlyList<string> Requirements { get; set; } = ImmutableList.Create<string>();

        /// <summary>
        /// Gets or sets the application type
        /// associated with the current application.
        /// </summary>
        [JsonProperty("type")]
        public virtual string? Type { get; set; }
    }
}