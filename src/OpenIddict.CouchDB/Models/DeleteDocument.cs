using CouchDB.Driver.Types;
using Newtonsoft.Json;

namespace OpenIddict.CouchDB.Models
{
    internal class DeleteDocument : CouchDocument
    {
        [JsonProperty("_deleted")]
        internal bool Deleted { get; } = true;

        internal DeleteDocument(string id, string rev)
        {
            Id = id;
            Rev = rev;
        }
    }
}