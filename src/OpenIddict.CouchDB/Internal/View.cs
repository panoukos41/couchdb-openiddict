using CouchDB.Driver.Types;

namespace OpenIddict.CouchDB.Internal
{
    public class View<TKey, TValue, TDoc> where TDoc : CouchDocument
    {
        public string Design { get; }

        public string Value { get; }

        public View(string design, string view)
        {
            Design = design;
            Value = view;
        }
    }
}