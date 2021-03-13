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
    /// Exposes a method allowing to resolve a token store.
    /// </summary>
    public class OpenIddictCouchDbTokenStoreResolver : IOpenIddictTokenStoreResolver
    {
        private readonly ConcurrentDictionary<Type, Type> _cache = new();
        private readonly IServiceProvider _provider;

        public OpenIddictCouchDbTokenStoreResolver(IServiceProvider provider)
            => _provider = provider;

        /// <summary>
        /// Returns a token store compatible with the specified token type or throws an
        /// <see cref="InvalidOperationException"/> if no store can be built using the specified type.
        /// </summary>
        /// <typeparam name="TToken">The type of the Token entity.</typeparam>
        /// <returns>An <see cref="IOpenIddictTokenStore{TToken}"/>.</returns>
        public IOpenIddictTokenStore<TToken> Get<TToken>() where TToken : class
        {
            var store = _provider.GetService<IOpenIddictTokenStore<TToken>>();
            if (store is not null)
            {
                return store;
            }

            var type = _cache.GetOrAdd(typeof(TToken), key =>
            {
                if (!typeof(OpenIddictCouchDbToken).IsAssignableFrom(key))
                {
                    throw new InvalidOperationException(SR.GetResourceString(SR.ID0260));
                }

                return typeof(OpenIddictCouchDbTokenStore<>).MakeGenericType(key);
            });

            return (IOpenIddictTokenStore<TToken>)_provider.GetRequiredService(type);
        }
    }
}