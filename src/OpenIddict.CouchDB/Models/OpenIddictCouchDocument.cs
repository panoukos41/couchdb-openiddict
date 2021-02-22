using CouchDB.Driver.Types;
using Newtonsoft.Json;
using System.Runtime.Serialization;

namespace OpenIddict.CouchDB.Models
{
    /// <summary>
    /// Base model from which all couch models derive from.
    /// Models should also derive from <see cref="CouchDocument"/>
    /// </summary>
    [JsonObject("openiddict")]
    public abstract class OpenIddictCouchDocument : CouchDocument
    {
        /// <summary>
        /// Gets the unique value that seperates this document from others.
        /// This value should be unique for one class to the whole application/applications.
        /// </summary>
        [DataMember]
        [JsonProperty("discriminator")]
        public abstract string Discriminator { get; set; }
    }
}