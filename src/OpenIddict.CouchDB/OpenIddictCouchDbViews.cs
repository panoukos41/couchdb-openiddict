#pragma warning disable CA1034 // Nested types should not be visible

namespace OpenIddict.CouchDB
{
    public static class OpenIddictCouchDbViews
    {
        public static class Application
        {
            public static (string design, string view) All { get; set; } = ("application", "all");

            public static (string design, string view) Count { get; set; } = ("application", "count");
        }

        public static class Authorization
        {
            public static (string design, string view) All { get; set; } = ("authorization", "all");

            public static (string design, string view) Count { get; set; } = ("authorization", "count");

            public static (string design, string view) ApplicationId { get; set; } = ("authorization", "application_id");

            public static (string design, string view) Subject { get; set; } = ("authorization", "subject");
        }

        public static class Scope
        {
            public static (string design, string view) All { get; set; } = ("scope", "all");

            public static (string design, string view) Count { get; set; } = ("scope", "count");

            public static (string design, string view) Name { get; set; } = ("scope", "name");
        }

        public static class Token
        {
            public static (string design, string view) All { get; set; } = ("token", "all");

            public static (string design, string view) Count { get; set; } = ("token", "count");

            public static (string design, string view) Prune { get; set; } = ("token", "prune");

            public static (string design, string view) ApplicationId { get; set; } = ("token", "application_id");

            public static (string design, string view) AuthorizationId { get; set; } = ("token", "authorization_id");
        }
    }
}