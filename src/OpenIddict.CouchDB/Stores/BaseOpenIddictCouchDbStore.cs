using CouchDB.Driver;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenIddict.CouchDB.Models;
using System;
using System.Linq;

namespace OpenIddict.CouchDB
{
    public abstract class BaseOpenIddictCouchDbStore<TStore> where TStore : OpenIddictCouchDocument
    {
        private ICouchClient Client { get; }

        /// <summary>
        /// Gets the options associated with the current store.
        /// </summary>
        protected IOptionsMonitor<OpenIddictCouchDbOptions> Options { get; }

        /// <summary>
        /// Get the discriminator value used create and to query
        /// different documents.
        /// </summary>
        protected abstract string Discriminator { get; }

        protected BaseOpenIddictCouchDbStore(IServiceProvider provider, IOptionsMonitor<OpenIddictCouchDbOptions> options)
        {
            Options = options;
            Client = options.CurrentValue.CouchClient
                ?? provider.GetRequiredService<ICouchClient>();
        }

        protected ICouchDatabase<TStore> GetDatabase() =>
            GetDatabase<TStore>();

        protected ICouchDatabase<T> GetDatabase<T>()
            where T : OpenIddictCouchDocument
        {
            return Client.GetDatabase<T>(Options.CurrentValue.DatabaseName);
        }

        protected IQueryable<TStore> QueryDb() =>
            QueryDb(GetDatabase());

        protected IQueryable<T> QueryDb<T>(ICouchDatabase<T> database)
            where T : OpenIddictCouchDocument
        {
            return database
                .Take(Options.CurrentValue.QueryLimit)
                .Where(x => x.Discriminator == Discriminator);
        }
    }
}