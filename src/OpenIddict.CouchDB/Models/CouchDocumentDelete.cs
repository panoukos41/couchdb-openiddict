using CouchDB.Driver.Types;
using Newtonsoft.Json;

namespace OpenIddict.CouchDB.Models
{
    public class CouchDocumentDelete : CouchDocument
    {
        [JsonProperty("_deleted")]
        public bool Deleted { get; } = true;

        public CouchDocumentDelete(string id, string rev)
        {
            Id = id;
            Rev = rev;
        }
    }
}