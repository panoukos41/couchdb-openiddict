using CouchDB.Driver.Types;

namespace OpenIddict.CouchDB.Internal
{
    public class View<TKey, TValue, TDoc> where TDoc : CouchDocument
    {
        public string Design { get; }

        public string Value { get; }

        public static View<TKey, TValue, TDoc> Create(string design, string view) =>
            new(design, view);

        private View(string design, string view)
        {
            Design = design;
            Value = view;
        }
    }
}