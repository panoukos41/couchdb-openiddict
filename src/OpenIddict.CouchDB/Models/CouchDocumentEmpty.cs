namespace OpenIddict.CouchDB.Models
{
    public class CouchDocumentEmpty : OpenIddictCouchDocument
    {
        public override string Discriminator
        {
            get => throw new System.NotImplementedException("This class is not meant to be used for queries.");
            set => throw new System.NotImplementedException("This class is not meant to be used for queries.");
        }
    }
}