using Newtonsoft.Json;

namespace OpenIddict.CouchDB.Models
{
    public class CouchDocumentDelete : OpenIddictCouchDocument
    {
        [JsonProperty("_deleted")]
        public bool Deleted { get; set; }

        public override string Discriminator
        {
            get => throw new System.NotImplementedException("This document is not meant to query with.");
            set => throw new System.NotImplementedException("This document is not meant to query with.");
        }

        public CouchDocumentDelete(string id, string rev, bool deleted = true)
        {
            Id = id;
            Rev = rev;
            Deleted = deleted;
        }
    }
}