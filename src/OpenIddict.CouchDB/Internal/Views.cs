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
            /// Key = Id, Value = 1, Document = Application
            /// </summary>
            /// <remarks>Using emit(doc._id, 1) and include_docs=true</remarks>
            public static View<string, int, TApplication> All { get; set; } =
                View<string, int, TApplication>.Create("application", "all");

            /// <summary>
            /// Key = null, Value = Count
            /// </summary>
            /// <remarks>Using reduce _count and emit(doc._id, 1)</remarks>
            public static View<object, int, TApplication> Count { get; set; } =
                View<object, int, TApplication>.Create("application", "count");
        }

        public static class Authorization<TAuthorization>
            where TAuthorization : OpenIddictCouchDbAuthorization
        {
            /// <summary>
            /// Key = Id, Value = 1, Document = Authorization
            /// </summary>
            /// <remarks>Using emit(doc._id, 1) and include_docs=true</remarks>
            public static View<string, int, TAuthorization> All { get; set; } =
                View<string, int, TAuthorization>.Create("authorization", "all");

            /// <summary>
            /// Key = null, Value = Count
            /// </summary>
            /// <remarks>Using reduce _count and emit(doc._id, 1)</remarks>
            public static View<object, int, TAuthorization> Count { get; set; } =
                View<object, int, TAuthorization>.Create("authorization", "count");

            /// <summary>
            /// Key = ApplicationId, Value = Rev
            /// </summary>
            public static View<string, string, TAuthorization> ApplicationId { get; set; } =
                View<string, string, TAuthorization>.Create("authorization", "application_id");

            /// <summary>
            /// Key = Subject, Value = 1
            /// </summary>
            public static View<string, int, TAuthorization> Subject { get; set; } =
                View<string, int, TAuthorization>.Create("authorization", "subject");

            /// <summary>
            /// Key = [ CreationDate, ExpirationDate ] , Value = Rev
            /// </summary>
            /// <remarks>
            /// if doc.status is not 'inactive' and 'valid'<br/>
            /// then call emit([doc.creation_date, doc.expiration_date], doc._rev)
            /// </remarks>
            public static View<DateTime[], string, TAuthorization> Prune { get; set; } =
                View<DateTime[], string, TAuthorization>.Create("token", "prune");
        }

        public static class Scope<TScope>
            where TScope : OpenIddictCouchDbScope
        {
            /// <summary>
            /// Key = Id, Value = 1, Document = Scope
            /// </summary>
            /// <remarks>Using emit(doc._id, 1) and include_docs=true</remarks>
            public static View<string, int, TScope> All { get; set; } =
                View<string, int, TScope>.Create("scope", "all");

            /// <summary>
            /// Key = null, Value = Count
            /// </summary>
            /// <remarks>Using reduce _count and emit(doc._id, 1)</remarks>
            public static View<string, int, TScope> Count { get; set; } =
                View<string, int, TScope>.Create("scope", "count");

            public static View<string, int, TScope> Name { get; set; } =
                View<string, int, TScope>.Create("scope", "name");
        }

        public static class Token<TToken>
            where TToken : OpenIddictCouchDbToken
        {
            /// <summary>
            /// Key = Id, Value = 1, Document = Token
            /// </summary>
            /// <remarks>Using emit(doc._id, 1) and include_docs=true</remarks>
            public static View<string, int, TToken> All { get; set; } =
                View<string, int, TToken>.Create("token", "all");

            /// <summary>
            /// Key = null, Value = Count
            /// </summary>
            /// <remarks>Using reduce _count and emit(doc._id, 1)</remarks>
            public static View<string, int, TToken> Count { get; set; } =
                View<string, int, TToken>.Create("token", "count");

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
            /// Key = [ CreationDate, ExpirationDate ] , Value = Rev
            /// </summary>
            /// <remarks>
            /// if doc.status is not 'inactive' and 'valid'<br/>
            /// then call emit([doc.creation_date, doc.expiration_date], doc._rev)
            /// </remarks>
            public static View<DateTime[], string, TToken> Prune { get; set; } =
                View<DateTime[], string, TToken>.Create("token", "prune");
        }
    }
}