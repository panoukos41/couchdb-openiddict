using OpenIddict.CouchDB.Models;
using System;

#pragma warning disable CA1034 // Nested types should not be visible

namespace OpenIddict.CouchDB.Internal
{
    public static class Views
    {
        public static class Application<TApplication>
            where TApplication : OpenIddictCouchDbApplication
        {
            /// <summary>
            /// With reduce: Key = null, Value = Count <br/>
            /// Without reduce: Key = Id, Value = Rev
            /// </summary>
            public static View<string, string, TApplication> Count { get; set; } =
                View<string, string, TApplication>.Create("application", "count");
        }

        public static class Authorization<TAuthorization>
            where TAuthorization : OpenIddictCouchDbAuthorization
        {
            /// <summary>
            /// With reduce: Key = null, Value = Count <br/>
            /// Without reduce: Key = id, Value = Rev
            /// </summary>
            public static View<string, string, TAuthorization> Count { get; set; } =
                View<string, string, TAuthorization>.Create("authorization", "count");

            /// <summary>
            /// Key = ApplicationId, Value = Rev
            /// </summary>
            public static View<string, string, TAuthorization> ApplicationId { get; set; } =
                View<string, string, TAuthorization>.Create("authorization", "application_id");

            /// <summary>
            /// Key = Subject, Value = Rev
            /// </summary>
            public static View<string, string, TAuthorization> Subject { get; set; } =
                View<string, string, TAuthorization>.Create("authorization", "subject");

            /// <summary>
            /// Key = [ CreationDate, ExpirationDate ] , Value = Rev
            /// </summary>
            public static View<DateTime[], string, TAuthorization> Prune { get; set; } =
                View<DateTime[], string, TAuthorization>.Create("token", "prune");
        }

        public static class Scope<TScope>
            where TScope : OpenIddictCouchDbScope
        {
            /// <summary>
            /// With reduce: Key = null, Value = Count <br/>
            /// Without reduce: Key = Id, Value = Rev
            /// </summary>
            public static View<string, string, TScope> Count { get; set; } =
                View<string, string, TScope>.Create("scope", "count");

            /// <summary>
            /// Key = Name, Value = Rev
            /// </summary>
            public static View<string, string, TScope> Name { get; set; } =
                View<string, string, TScope>.Create("scope", "name");
        }

        public static class Token<TToken>
            where TToken : OpenIddictCouchDbToken
        {
            /// <summary>
            /// With reduce: Key = null, Value = Count <br/>
            /// Without reduce: Key = Id, Value = Rev
            /// </summary>
            public static View<string, string, TToken> Count { get; set; } =
                View<string, string, TToken>.Create("token", "count");

            /// <summary>
            /// Key = ApplicationId, Value = Rev
            /// </summary>
            public static View<string, string, TToken> ApplicationId { get; set; } =
                View<string, string, TToken>.Create("token", "application_id");

            /// <summary>
            /// Key = AuthorizationId, Value = Rev
            /// </summary>
            public static View<string, string, TToken> AuthorizationId { get; set; } =
                View<string, string, TToken>.Create("token", "authorization_id");

            /// <summary>
            /// Key = [CreationDate, ExpirationDate] , Value = Rev
            /// </summary>
            /// <remarks>
            public static View<DateTime[], string, TToken> Prune { get; set; } =
                View<DateTime[], string, TToken>.Create("token", "prune");
        }
    }
}