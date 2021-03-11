#pragma warning disable CA1034 // Nested types should not be visible

using OpenIddict.CouchDB.Models;

namespace OpenIddict.CouchDB.Internal
{
    public static class Views
    {
        public static class Application
        {
            //public static (string design, string view) All { get; set; } = ("application", "all");
            public static View<string, int, OpenIddictCouchDbApplication> All { get; set; } =
                View<string, int, OpenIddictCouchDbApplication>.Create("application", "all");

            //public static (string design, string view) Count { get; set; } = ("application", "count");
            public static View<string, int, OpenIddictCouchDbApplication> Count { get; set; } =
                View<string, int, OpenIddictCouchDbApplication>.Create("application", "count");
        }

        public static class Authorization
        {
            //public static (string design, string view) All { get; set; } = ("authorization", "all");
            public static View<string, int, OpenIddictCouchDbAuthorization> All { get; set; } =
                View<string, int, OpenIddictCouchDbAuthorization>.Create("authorization", "all");

            //public static (string design, string view) Count { get; set; } = ("authorization", "count");
            public static View<string, int, OpenIddictCouchDbAuthorization> Count { get; set; } =
                View<string, int, OpenIddictCouchDbAuthorization>.Create("authorization", "count");

            //public static (string design, string view) ApplicationId { get; set; } = ("authorization", "application_id");
            public static View<string, int, OpenIddictCouchDbAuthorization> ApplicationId { get; set; } =
                View<string, int, OpenIddictCouchDbAuthorization>.Create("authorization", "application_id");

            //public static (string design, string view) Subject { get; set; } = ("authorization", "subject");
            public static View<string, int, OpenIddictCouchDbAuthorization> Subject { get; set; } =
                View<string, int, OpenIddictCouchDbAuthorization>.Create("authorization", "subject");
        }

        public static class Scope
        {
            //public static (string design, string view) All { get; set; } = ("scope", "all");
            public static View<string, int, OpenIddictCouchDbScope> All { get; set; } =
                View<string, int, OpenIddictCouchDbScope>.Create("scope", "all");

            //public static (string design, string view) Count { get; set; } = ("scope", "count");
            public static View<string, int, OpenIddictCouchDbScope> Count { get; set; } =
                View<string, int, OpenIddictCouchDbScope>.Create("scope", "count");

            //public static (string design, string view) Name { get; set; } = ("scope", "name");
            public static View<string, int, OpenIddictCouchDbScope> Name { get; set; } =
                View<string, int, OpenIddictCouchDbScope>.Create("scope", "name");
        }

        public static class Token
        {
            //public static (string design, string view) All { get; set; } = ("token", "all");
            public static View<string, int, OpenIddictCouchDbToken> All { get; set; } =
                View<string, int, OpenIddictCouchDbToken>.Create("token", "all");

            //public static (string design, string view) Count { get; set; } = ("token", "count");
            public static View<string, int, OpenIddictCouchDbToken> Count { get; set; } =
                View<string, int, OpenIddictCouchDbToken>.Create("token", "count");

            //public static (string design, string view) Prune { get; set; } = ("token", "prune");
            public static View<string, int, OpenIddictCouchDbToken> Prune { get; set; } =
                View<string, int, OpenIddictCouchDbToken>.Create("token", "prune");

            //public static (string design, string view) ApplicationId { get; set; } = ("token", "application_id");
            public static View<string, int, OpenIddictCouchDbToken> ApplicationId { get; set; } =
                View<string, int, OpenIddictCouchDbToken>.Create("token", "application_id");

            //public static (string design, string view) AuthorizationId { get; set; } = ("token", "authorization_id");
            public static View<string, int, OpenIddictCouchDbToken> AuthorizationId { get; set; } =
                View<string, int, OpenIddictCouchDbToken>.Create("token", "authorization_id");
        }
    }
}