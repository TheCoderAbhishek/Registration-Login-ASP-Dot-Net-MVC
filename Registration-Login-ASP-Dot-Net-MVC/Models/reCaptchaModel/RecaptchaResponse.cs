using Newtonsoft.Json;

namespace Registration_Login_ASP_Dot_Net_MVC.Models.reCaptchaModel
{
    public class RecaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("score")]
        public double Score { get; set; }

        [JsonProperty("action")]
        public string? Action { get; set; }

        [JsonProperty("challenge_ts")]
        public string? ChallengeTimestamp { get; set; }

        [JsonProperty("hostname")]
        public string? Hostname { get; set; }

        [JsonProperty("error-codes")]
        public string[]? ErrorCodes { get; set; }
    }
}
