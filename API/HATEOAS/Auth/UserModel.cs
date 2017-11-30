using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace RefArc.Api.HATEOAS.Filter
{
    //another user model
    public class UserModel
    {
        //[Required]
        //[JsonRequired]
        [JsonProperty("username")] //needed
        public string username { get; internal set; }
        [JsonProperty("grant_type")] //needed
        public string grant_type { get; internal set; }
        //[Required]
        [DataType(DataType.Password)]
        //[JsonRequired]
        [JsonProperty("password")] //needed
        public string password { get; internal set; }
        [JsonProperty("refresh_token")] //needed
        public string refresh_token { get; internal set; }
    }
}