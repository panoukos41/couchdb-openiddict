using CouchDB.Driver.Types;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace OpenIddict.CouchDB.Models
{
    /// <summary>
    /// Represents an OpenIddict token.
    /// </summary>
    [DebuggerDisplay("Id = {Id.ToString(),nq} ; Subject = {Subject,nq} ; Type = {Type,nq} ; Status = {Status,nq}")]
    public class CouchDbOpenIddictToken : CouchDocument
    {
        /// <summary>
        /// Gets or sets the identifier of the application associated with the current token.
        /// </summary>
        [JsonProperty("application_id")]
        public virtual string ApplicationId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the identifier of the authorization associated with the current token.
        /// </summary>
        [JsonProperty("authorization_id")]
        public virtual string AuthorizationId { get; set; } = null!;

        /// <summary>
        /// Gets or sets the UTC creation date of the current token.
        /// </summary>
        [JsonProperty("creation_date")]
        public virtual DateTime? CreationDate { get; set; }

        /// <summary>
        /// Gets or sets the UTC expiration date of the current token.
        /// </summary>
        [JsonProperty("expiration_date")]
        public virtual DateTime? ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the payload of the current token, if applicable.
        /// Note: this property is only used for reference tokens
        /// and may be encrypted for security reasons.
        /// </summary>
        [JsonProperty("payload")]
        public virtual string? Payload { get; set; }

        /// <summary>
        /// Gets or sets the additional properties associated with the current token.
        /// </summary>
        [JsonProperty("properties")]
        public virtual JObject? Properties { get; set; }

        /// <summary>
        /// Gets or sets the UTC redemption date of the current token.
        /// </summary>
        [JsonProperty("redemption_date")]
        public virtual DateTime? RedemptionDate { get; set; }

        /// <summary>
        /// Gets or sets the reference identifier associated
        /// with the current token, if applicable.
        /// Note: this property is only used for reference tokens
        /// and may be hashed or encrypted for security reasons.
        /// </summary>
        [JsonProperty("reference_id")]
        public virtual string? ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the status of the current token.
        /// </summary>
        [JsonProperty("status")]
        public virtual string? Status { get; set; }

        /// <summary>
        /// Gets or sets the subject associated with the current token.
        /// </summary>
        [JsonProperty("subject")]
        public virtual string? Subject { get; set; }

        /// <summary>
        /// Gets or sets the type of the current token.
        /// </summary>
        [JsonProperty("type")]
        public virtual string? Type { get; set; }
    }
}