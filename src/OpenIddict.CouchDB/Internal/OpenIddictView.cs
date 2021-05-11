using CouchDB.Driver.Types;

namespace OpenIddict.CouchDB.Internal
{
    public class OpenIddictView<TKey, TValue, TDoc> where TDoc : CouchDocument
    {
        public string Design { get; }

        public string Value { get; }

        public OpenIddictView(string design, string view)
        {
            Design = design;
            Value = view;
        }
    }
}