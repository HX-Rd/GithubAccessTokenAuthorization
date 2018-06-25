using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace HXRd.Github.GithubAccessTokenAuthorization.Models
{
    public class GithubTokenInfo
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }
        [JsonProperty(PropertyName = "scopes")]
        public List<string> Scopes { get; set; }
        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }
        [JsonProperty(PropertyName = "token_last_eight")]
        public string TokenLastEight { get; set; }
        [JsonProperty(PropertyName = "hashed_token")]
        public string HashedToken { get; set; }
        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }
        [JsonProperty(PropertyName = "note_url")]
        public string NoteUrl { get; set; }
        [JsonProperty(PropertyName = "updated_at")]
        public string UpdatedAt { get; set; }
        [JsonProperty(PropertyName = "created_at")]
        public string CreatedAt { get; set; }
        [JsonProperty(PropertyName = "fingerprint")]
        public string Fingerprint { get; set; }
        [JsonProperty(PropertyName = "app")]
        public GithubApp App { get; set; }
        [JsonProperty(PropertyName = "user")]
        public GithubUser User { get; set; }
    }
}
