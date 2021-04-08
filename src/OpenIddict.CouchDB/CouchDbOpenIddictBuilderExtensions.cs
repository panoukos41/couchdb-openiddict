/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenIddict.CouchDB;
using OpenIddict.CouchDB.Models;
using OpenIddict.CouchDB.Resolvers;
using OpenIddict.CouchDB.Stores;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Exposes extensions allowing to register the OpenIddict MongoDB services.
    /// </summary>
    public static class CouchDbOpenIddictBuilderExtensions
    {
        /// <summary>
        /// Registers the MongoDB stores services in the DI container and
        /// configures OpenIddict to use the MongoDB entities by default.
        /// </summary>
        /// <param name="builder">The services builder used by OpenIddict to register new services.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="CouchDbOpenIddictBuilder"/>.</returns>
        public static CouchDbOpenIddictBuilder UseCouchDb(this OpenIddictCoreBuilder builder)
        {
            Check.NotNull(builder, nameof(builder));

            // Note: Mongo uses simple binary comparison checks by default so the additional
            // query filtering applied by the default OpenIddict managers can be safely disabled.
            builder.DisableAdditionalFiltering();

            builder.SetDefaultApplicationEntity<CouchDbOpenIddictApplication>()
                   .SetDefaultAuthorizationEntity<CouchDbOpenIddictAuthorization>()
                   .SetDefaultScopeEntity<CouchDbOpenIddictScope>()
                   .SetDefaultTokenEntity<CouchDbOpenIddictToken>();

            // Note: the Mongo stores/resolvers don't depend on scoped/transient services and thus
            // can be safely registered as singleton services and shared/reused across requests.
            builder.ReplaceApplicationStoreResolver<CouchDOpenIddictbApplicationStoreResolver>(ServiceLifetime.Singleton)
                   .ReplaceAuthorizationStoreResolver<CouchDOpenIddictbAuthorizationStoreResolver>(ServiceLifetime.Singleton)
                   .ReplaceScopeStoreResolver<CouchDbOpenIddictScopeStoreResolver>(ServiceLifetime.Singleton)
                   .ReplaceTokenStoreResolver<CouchDbOpenIddictTokenStoreResolver>(ServiceLifetime.Singleton);

            builder.Services.TryAddSingleton(typeof(CouchDbOpenIddictApplicationStore<>));
            builder.Services.TryAddSingleton(typeof(CouchDOpenIddictbAuthorizationStore<>));
            builder.Services.TryAddSingleton(typeof(CouchDbOpenIddictScopeStore<>));
            builder.Services.TryAddSingleton(typeof(CouchDbOpenIddictTokenStore<>));

            return new CouchDbOpenIddictBuilder(builder.Services);
        }

        /// <summary>
        /// Registers the MongoDB stores services in the DI container and
        /// configures OpenIddict to use the MongoDB entities by default.
        /// </summary>
        /// <param name="builder">The services builder used by OpenIddict to register new services.</param>
        /// <param name="configuration">The configuration delegate used to configure the MongoDB services.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="OpenIddictCoreBuilder"/>.</returns>
        public static OpenIddictCoreBuilder UseCouchDb(
            this OpenIddictCoreBuilder builder, Action<CouchDbOpenIddictBuilder> configuration)
        {
            Check.NotNull(builder, nameof(builder));
            Check.NotNull(configuration, nameof(configuration));

            configuration(builder.UseCouchDb());

            return builder;
        }
    }
}