/*
 * Licensed under the Apache License, Version 2.0 (http://www.apache.org/licenses/LICENSE-2.0)
 * See https://github.com/openiddict/openiddict-core for more information concerning
 * the license and the contributors participating to this project.
 */

using CouchDB.Driver;

namespace OpenIddict.CouchDB
{
    /// <summary>
    /// Provides various settings needed to configure the OpenIddict MongoDB integration.
    /// </summary>
    public class OpenIddictCouchDbOptions
    {
        /// <summary>
        /// Gets or sets the name of the applications collection (by default, openiddict.applications).
        /// </summary>
        public string ApplicationDiscriminator { get; set; } = "openiddict.application";

        /// <summary>
        /// Gets or sets the name of the authorizations collection (by default, openiddict.authorizations).
        /// </summary>
        public string AuthorizationDiscriminator { get; set; } = "openiddict.authorization";

        /// <summary>
        /// Gets or sets the name of the scopes collection (by default, openiddict.scopes).
        /// </summary>
        public string ScopeDiscriminator { get; set; } = "openiddict.scope";

        /// <summary>
        /// Gets or sets the name of the tokens collection (by default, openiddict.tokens).
        /// </summary>
        public string TokenDiscriminator { get; set; } = "openiddict.token";

        /// <summary>
        /// Gets or sets the <see cref="ICouchClient"/> used by the OpenIddict stores.
        /// If no value is explicitly set, the database is resolved from the DI container.
        /// </summary>
        public ICouchClient? CouchClient { get; set; }

        /// <summary>
        /// The name of the database the stores will use.
        /// </summary>
        public string DatabaseName { get; set; } = "openiddict";

        // max 268435456
        /// <summary>
        /// The limit a query can run is at 268_435_456. This is set to 500_000
        /// by default. You can change it however you want.
        /// </summary>
        public int QueryLimit { get; set; } = 500_000;
    }
}