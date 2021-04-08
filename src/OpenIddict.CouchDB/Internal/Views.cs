using OpenIddict.CouchDB.Models;
using System;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

namespace OpenIddict.CouchDB.Internal
{
    public static class Views
    {
        public static string Document { get; set; }

        static Views() => ApplyOptions(new());

        public static class Application<TApplication>
            where TApplication : CouchDbOpenIddictApplication
        {
            /// <summary>
            /// With reduce = true (default) then Key = null, Value = 'count'<br/>
            /// When reduce = false (manual) then Key = Id, Value = Rev
            /// </summary>
            public static View<string, string, TApplication> Applications { get; set; }

            public static void ApplyOptions(CouchDbOpenIddictViewOptions options)
            {
                Applications = new(Document, options.Application);
            }
        }

        public static class Authorization<TAuthorization>
            where TAuthorization : CouchDbOpenIddictAuthorization
        {
            /// <summary>
            /// With reduce = true (default) then Key = null, Value = 'count'<br/>
            /// When reduce = false (manual) then Key = Id, Value = Rev
            /// </summary>
            public static View<string, string, TAuthorization> Authorizations { get; set; }

            /// <summary>
            /// Key = ApplicationId, Value = Rev
            /// </summary>
            public static View<string, string, TAuthorization> ApplicationId { get; set; }

            /// <summary>
            /// Key = Subject, Value = Rev
            /// </summary>
            public static View<string, string, TAuthorization> Subject { get; set; }

            /// <summary>
            /// Key = [ CreationDate, ExpirationDate ] , Value = Rev
            /// </summary>
            public static View<DateTime[], string, TAuthorization> Prune { get; set; }

            public static void ApplyOptions(CouchDbOpenIddictViewOptions options)
            {
                Authorizations = new(Document, options.Authorization);
                ApplicationId = new(Document, options.AuthorizationApplicationId);
                Subject = new(Document, options.AuthorizationSubject);
                Prune = new(Document, options.AuthorizationPrune);
            }
        }

        public static class Scope<TScope>
            where TScope : CouchDbOpenIddictScope
        {
            /// <summary>
            /// With reduce = true (default) then Key = null, Value = 'count'<br/>
            /// When reduce = false (manual) then Key = Id, Value = Rev
            /// </summary>
            public static View<string, string, TScope> Scopes { get; set; }

            /// <summary>
            /// Key = Name, Value = Rev
            /// </summary>
            public static View<string, string, TScope> Name { get; set; }

            public static void ApplyOptions(CouchDbOpenIddictViewOptions options)
            {
                Scopes = new(Document, options.Scope);
                Name = new(Document, options.ScopeName);
            }
        }

        public static class Token<TToken>
            where TToken : CouchDbOpenIddictToken
        {
            /// <summary>
            /// With reduce = true (default) then Key = null, Value = 'count'<br/>
            /// When reduce = false (manual) then Key = Id, Value = Rev
            /// </summary>
            public static View<string, string, TToken> Tokens { get; set; }

            /// <summary>
            /// Key = ApplicationId, Value = Rev
            /// </summary>
            public static View<string, string, TToken> ApplicationId { get; set; }

            /// <summary>
            /// Key = AuthorizationId, Value = Rev
            /// </summary>
            public static View<string, string, TToken> AuthorizationId { get; set; }

            /// <summary>
            /// Key = [CreationDate, ExpirationDate] , Value = Rev
            /// </summary>
            /// <remarks>
            public static View<DateTime[], string, TToken> Prune { get; set; }

            public static void ApplyOptions(CouchDbOpenIddictViewOptions options)
            {
                Tokens = new(Document, options.Token);
                ApplicationId = new(Document, options.TokenApplicationId);
                AuthorizationId = new(Document, options.TokenAuthorizationId);
                Prune = new(Document, options.TokenPrune);
            }
        }

        /// <summary>
        /// Apply these options to the existing view properties.
        /// </summary>
        /// <param name="options"></param>
        public static void ApplyOptions(CouchDbOpenIddictViewOptions options)
        {
            Check.NotNull(options, nameof(options));
            Document = options.Document;

            Application<CouchDbOpenIddictApplication>.ApplyOptions(options);
            Authorization<CouchDbOpenIddictAuthorization>.ApplyOptions(options);
            Scope<CouchDbOpenIddictScope>.ApplyOptions(options);
            Token<CouchDbOpenIddictToken>.ApplyOptions(options);
        }
    }
}