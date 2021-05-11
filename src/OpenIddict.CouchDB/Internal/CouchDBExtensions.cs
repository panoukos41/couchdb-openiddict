using CouchDB.Driver.Types;
using CouchDB.Driver.Views;
using OpenIddict.CouchDB.Internal;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CouchDB.Driver
{
    internal static class CouchDBExtensions
    {
        internal static Task<List<CouchView<TKey, TValue, TDoc>>> GetViewAsync<TKey, TValue, TDoc>(
            this ICouchDatabase<TDoc> db,
            OpenIddictView<TKey, TValue, TDoc> view,
            CouchViewOptions<TKey>? options = null,
            CancellationToken cancellationToken = default)
            where TDoc : CouchDocument
        {
            return db.GetViewAsync<TKey, TValue>(view.Design, view.Value, options, cancellationToken);
        }

        internal static Task<CouchViewList<TKey, TValue, TDoc>> GetDetailedViewAsync<TKey, TValue, TDoc>(
            this ICouchDatabase<TDoc> db,
            OpenIddictView<TKey, TValue, TDoc> view,
            CouchViewOptions<TKey>? options = null,
            CancellationToken cancellationToken = default)
            where TDoc : CouchDocument
        {
            return db.GetDetailedViewAsync<TKey, TValue>(view.Design, view.Value, options, cancellationToken);
        }
    }
}