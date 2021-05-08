// =======================================
// Vaccination Centers object model class
// =======================================

using Newtonsoft.Json;
using System.Collections.Generic;

namespace CoWinService.Domain.Model
{
    // Vaccine center data models
    public class Centers
    {
        public List<VaccineCenter> centers;
    }

    public class VaccineCenter
    {
        [JsonProperty("center_id")]
        public string center_id { get; set; }

        [JsonProperty("name")]
        public string name { get; set; }

        [JsonProperty("address")]
        public string address { get; set; }

        [JsonProperty("state_name")]
        public string state_name { get; set; }

        [JsonProperty("district_name")]
        public string district_name { get; set; }

        [JsonProperty("block_name")]
        public string block_name { get; set; }

        [JsonProperty("pincode")]
        public int pincode { get; set; }

        [JsonProperty("from")]
        public string from { get; set; }

        [JsonProperty("to")]
        public string to { get; set; }

        [JsonProperty("fee_type")]
        public string fee_type { get; set; }

        [JsonProperty("sessions")]
        public List<Session> sessions { get; set; }
    }

    public class Session
    {
        [JsonProperty("session_id")]
        public string session_id { get; set; }

        [JsonProperty("date")]
        public string date { get; set; }

        [JsonProperty("available_capacity")]
        public float available_capacity { get; set; }

        [JsonProperty("min_age_limit")]
        public int min_age_limit { get; set; }

        [JsonProperty("vaccine")]
        public string vaccine { get; set; }

        [JsonProperty("slots")]
        public List<string> slots { get; set; }
    }

}
