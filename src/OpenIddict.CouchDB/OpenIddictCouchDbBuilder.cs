/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using CouchDB.Driver;
using OpenIddict.Core;
using OpenIddict.CouchDB;
using OpenIddict.CouchDB.Models;
using System;
using System.ComponentModel;
using SR = OpenIddict.Abstractions.OpenIddictResources;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Exposes the necessary methods required to configure the OpenIddict CouchDB services.
    /// </summary>
    public class OpenIddictCouchDbBuilder
    {
        /// <summary>
        /// Initializes a new instance of <see cref="OpenIddictCouchDbBuilder"/>.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public OpenIddictCouchDbBuilder(IServiceCollection services)
            => Services = services ?? throw new ArgumentNullException(nameof(services));

        /// <summary>
        /// Gets the services collection.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public IServiceCollection Services { get; }

        /// <summary>
        /// Amends the default OpenIddict CouchDB configuration.
        /// </summary>
        /// <param name="configuration">The delegate used to configure the OpenIddict options.</param>
        /// <remarks>This extension can be safely called multiple times.</remarks>
        /// <returns>The <see cref="OpenIddictCouchDbBuilder"/>.</returns>
        public OpenIddictCouchDbBuilder Configure(Action<OpenIddictCouchDbOptions> configuration)
        {
            Check.NotNull(configuration, nameof(configuration));

            Services.Configure(configuration);

            return this;
        }

        /// <summary>
        /// Configures OpenIddict to use the specified entity as the default application entity.
        /// </summary>
        /// <returns>The <see cref="OpenIddictCouchDbBuilder"/>.</returns>
        public OpenIddictCouchDbBuilder ReplaceDefaultApplicationEntity<TApplication>()
            where TApplication : OpenIddictCouchDbApplication
        {
            Services.Configure<OpenIddictCoreOptions>(options => options.DefaultApplicationType = typeof(TApplication));

            return this;
        }

        /// <summary>
        /// Configures OpenIddict to use the specified entity as the default authorization entity.
        /// </summary>
        /// <returns>The <see cref="OpenIddictCouchDbBuilder"/>.</returns>
        public OpenIddictCouchDbBuilder ReplaceDefaultAuthorizationEntity<TAuthorization>()
            where TAuthorization : OpenIddictCouchDbAuthorization
        {
            Services.Configure<OpenIddictCoreOptions>(options => options.DefaultAuthorizationType = typeof(TAuthorization));

            return this;
        }

        /// <summary>
        /// Configures OpenIddict to use the specified entity as the default scope entity.
        /// </summary>
        /// <returns>The <see cref="OpenIddictCouchDbBuilder"/>.</returns>
        public OpenIddictCouchDbBuilder ReplaceDefaultScopeEntity<TScope>()
            where TScope : OpenIddictCouchDbScope
        {
            Services.Configure<OpenIddictCoreOptions>(options => options.DefaultScopeType = typeof(TScope));

            return this;
        }

        /// <summary>
        /// Configures OpenIddict to use the specified entity as the default token entity.
        /// </summary>
        /// <returns>The <see cref="OpenIddictCouchDbBuilder"/>.</returns>
        public OpenIddictCouchDbBuilder ReplaceDefaultTokenEntity<TToken>()
            where TToken : OpenIddictCouchDbToken
        {
            Services.Configure<OpenIddictCoreOptions>(options => options.DefaultTokenType = typeof(TToken));

            return this;
        }

        /// <summary>
        /// Replaces the default application discriminator (by default, openiddict.applications).
        /// </summary>
        /// <param name="discriminator">The discriminator name.</param>
        /// <returns>The <see cref="OpenIddictCouchDbBuilder"/>.</returns>
        public OpenIddictCouchDbBuilder SetApplicationDiscriminator(string discriminator)
        {
            if (string.IsNullOrEmpty(discriminator))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0261), nameof(discriminator));
            }

            return Configure(options => options.ApplicationDiscriminator = discriminator);
        }

        /// <summary>
        /// Replaces the default authorization discriminator (by default, openiddict.authorizations).
        /// </summary>
        /// <param name="discriminator">The discriminator name.</param>
        /// <returns>The <see cref="OpenIddictCouchDbBuilder"/>.</returns>
        public OpenIddictCouchDbBuilder SetAuthorizationDiscriminator(string discriminator)
        {
            if (string.IsNullOrEmpty(discriminator))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0261), nameof(discriminator));
            }

            return Configure(options => options.AuthorizationDiscriminator = discriminator);
        }

        /// <summary>
        /// Replaces the default scope discriminator (by default, openiddict.scopes).
        /// </summary>
        /// <param name="discriminator">The discriminator name.</param>
        /// <returns>The <see cref="OpenIddictCouchDbBuilder"/>.</returns>
        public OpenIddictCouchDbBuilder SetScopeDiscriminator(string discriminator)
        {
            if (string.IsNullOrEmpty(discriminator))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0261), nameof(discriminator));
            }

            return Configure(options => options.ScopeDiscriminator = discriminator);
        }

        /// <summary>
        /// Replaces the default token discriminator (by default, openiddict.tokens).
        /// </summary>
        /// <param name="discriminator">The discriminator name.</param>
        /// <returns>The <see cref="OpenIddictCouchDbBuilder"/>.</returns>
        public OpenIddictCouchDbBuilder SetTokenDiscriminator(string discriminator)
        {
            if (string.IsNullOrEmpty(discriminator))
            {
                throw new ArgumentException(SR.GetResourceString(SR.ID0261), nameof(discriminator));
            }

            return Configure(options => options.TokenDiscriminator = discriminator);
        }

        /// <summary>
        /// Change the database name that will be used on the provided couch client
        /// to retrieve a database.
        /// </summary>
        /// <param name="name">The name of the database.</param>
        /// <returns>The <see cref="OpenIddictCouchDbBuilder"/>.</returns>
        public OpenIddictCouchDbBuilder SetDatabaseName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Database name must not be null or whitespace.", nameof(name));
            }

            return Configure(options => options.DatabaseName = name);
        }

        /// <summary>
        /// Configures the CouchDB stores to use the specified client
        /// instead of retrieving it from the dependency injection container.
        /// </summary>
        /// <param name="client">The <see cref="ICouchClient"/>.</param>
        /// <returns>The <see cref="OpenIddictCouchDbBuilder"/>.</returns>
        public OpenIddictCouchDbBuilder UseClient(ICouchClient client)
        {
            if (client is null)
            {
                throw new ArgumentNullException(nameof(client));
            }

            return Configure(options => options.CouchClient = client);
        }

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override bool Equals(object? obj) => base.Equals(obj);

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override int GetHashCode() => base.GetHashCode();

        /// <inheritdoc/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string? ToString() => base.ToString();
    }
}