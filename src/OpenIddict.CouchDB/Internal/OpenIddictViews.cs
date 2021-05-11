using OpenIddict.CouchDB.Models;
using System;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor.

namespace OpenIddict.CouchDB.Internal
{
    public static class OpenIddictViews
    {
        public static string Document { get; set; }

        static OpenIddictViews() => ApplyOptions(new());

        public static class Application<TApplication>
            where TApplication : CouchDbOpenIddictApplication
        {
            /// <summary>
            /// With reduce = true (default) then Key = null, Value = 'count'<br/>
            /// When reduce = false (manual) then Key = Id, Value = Rev
            /// </summary>
            public static OpenIddictView<string, string, TApplication> Applications { get; set; }
            
            /// <summary>
            /// Key = ClientId, Value = Rev
            /// </summary>
            public static OpenIddictView<string, string, TApplication> ClientId { get; set; }
            
            /// <summary>
            /// Key = RedirectUri, Value = Rev
            /// </summary>
            /// <remarks>Each URI for each application is returned as a single row.</remarks>
            public static OpenIddictView<string, string, TApplication> RedirectUris { get; set; }

            /// <summary>
            /// Key = PostLogoutRedirectUri, Value = Rev
            /// </summary>
            /// <remarks>Each URI for each application is returned as a single row.</remarks>
            public static OpenIddictView<string, string, TApplication> PostLogoutRedirectUris { get; set; }
            
            public static void ApplyOptions(CouchDbOpenIddictViewOptions options)
            {
                Applications = new(Document, options.Application);
                ClientId = new(Document, options.ApplicationClientId);
                RedirectUris = new(Document, options.ApplicationRedirectUri);
                PostLogoutRedirectUris = new(Document, options.ApplicationPostLogoutRedirectUri);
            }
        }

        public static class Authorization<TAuthorization>
            where TAuthorization : CouchDbOpenIddictAuthorization
        {
            /// <summary>
            /// With reduce = true (default) then Key = null, Value = 'count'<br/>
            /// When reduce = false (manual) then Key = Id, Value = Rev
            /// </summary>
            public static OpenIddictView<string, string, TAuthorization> Authorizations { get; set; }

            /// <summary>
            /// Key = ApplicationId, Value = Rev
            /// </summary>
            public static OpenIddictView<string, string, TAuthorization> ApplicationId { get; set; }

            /// <summary>
            /// Key = Subject, Value = Rev
            /// </summary>
            public static OpenIddictView<string, string, TAuthorization> Subject { get; set; }

            /// <summary>
            /// Key = [ CreationDate, ExpirationDate ] , Value = Rev
            /// </summary>
            public static OpenIddictView<DateTime[], string, TAuthorization> Prune { get; set; }

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
            public static OpenIddictView<string, string, TScope> Scopes { get; set; }

            /// <summary>
            /// Key = Name, Value = Rev
            /// </summary>
            public static OpenIddictView<string, string, TScope> Name { get; set; }
            
            /// <summary>
            /// Key = Resource, Value = Rev
            /// </summary>
            /// <remarks>Each Resource for each scope is returned as a single row.</remarks>
            public static OpenIddictView<string, string, TScope> Resources { get; set; }

            public static void ApplyOptions(CouchDbOpenIddictViewOptions options)
            {
                Scopes = new(Document, options.Scope);
                Name = new(Document, options.ScopeName);
                Resources = new(Document, options.ScopeResources);
            }
        }

        public static class Token<TToken>
            where TToken : CouchDbOpenIddictToken
        {
            /// <summary>
            /// With reduce = true (default) then Key = null, Value = 'count'<br/>
            /// When reduce = false (manual) then Key = Id, Value = Rev
            /// </summary>
            public static OpenIddictView<string, string, TToken> Tokens { get; set; }

            /// <summary>
            /// Key = ApplicationId, Value = Rev
            /// </summary>
            public static OpenIddictView<string, string, TToken> ApplicationId { get; set; }

            /// <summary>
            /// Key = AuthorizationId, Value = Rev
            /// </summary>
            public static OpenIddictView<string, string, TToken> AuthorizationId { get; set; }
            
            /// <summary>
            /// Key = ReferenceId, Value = Rev
            /// </summary>
            public static OpenIddictView<string, string, TToken> ReferenceId { get; set; }

            /// <summary>
            /// Key = Subject, Value = Rev
            /// </summary>
            public static OpenIddictView<string, string, TToken> Subject { get; set; }

            /// <summary>
            /// Key = [CreationDate, ExpirationDate] , Value = Rev
            /// </summary>
            /// <remarks>
            public static OpenIddictView<DateTime[], string, TToken> Prune { get; set; }

            public static void ApplyOptions(CouchDbOpenIddictViewOptions options)
            {
                Tokens = new(Document, options.Token);
                ApplicationId = new(Document, options.TokenApplicationId);
                AuthorizationId = new(Document, options.TokenAuthorizationId);
                ReferenceId = new(Document, options.TokenReferenceId);
                Subject = new(Document, options.TokenSubject);
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