﻿/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using OpenIddict.CouchDB.Models;
using OpenIddict.CouchDB.Stores;
using System;
using System.Collections.Concurrent;
using SR = OpenIddict.Abstractions.OpenIddictResources;

namespace OpenIddict.CouchDB.Resolvers
{
    /// <summary>
    /// Exposes a method allowing to resolve an application store.
    /// </summary>
    public class CouchDOpenIddictbApplicationStoreResolver : IOpenIddictApplicationStoreResolver
    {
        private readonly ConcurrentDictionary<Type, Type> _cache = new();
        private readonly IServiceProvider _provider;

        public CouchDOpenIddictbApplicationStoreResolver(IServiceProvider provider)
            => _provider = provider;

        /// <summary>
        /// Returns an application store compatible with the specified application type or throws an
        /// <see cref="InvalidOperationException"/> if no store can be built using the specified type.
        /// </summary>
        /// <typeparam name="TApplication">The type of the Application entity.</typeparam>
        /// <returns>An <see cref="IOpenIddictApplicationStore{TApplication}"/>.</returns>
        public IOpenIddictApplicationStore<TApplication> Get<TApplication>() where TApplication : class
        {
            var store = _provider.GetService<IOpenIddictApplicationStore<TApplication>>();
            if (store is not null)
            {
                return store;
            }

            var type = _cache.GetOrAdd(typeof(TApplication), key =>
            {
                if (!typeof(CouchDbOpenIddictApplication).IsAssignableFrom(key))
                {
                    throw new InvalidOperationException(SR.GetResourceString(SR.ID0257));
                }

                return typeof(CouchDbOpenIddictApplicationStore<>).MakeGenericType(key);
            });

            return (IOpenIddictApplicationStore<TApplication>)_provider.GetRequiredService(type);
        }
    }
}