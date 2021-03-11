using CouchDB.Driver.Types;
using Newtonsoft.Json;

namespace OpenIddict.CouchDB.Models
{
    public class CouchDocumentDelete : CouchDocument
    {
        [JsonProperty("_deleted")]
        public bool Deleted { get; set; }

        public CouchDocumentDelete(string id, string rev, bool deleted = true)
        {
            Id = id;
            Rev = rev;
            Deleted = deleted;
        }
    }
}