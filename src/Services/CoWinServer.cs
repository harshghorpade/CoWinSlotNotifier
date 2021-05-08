// ==================================
// CoWin service Implementation class
// ==================================

using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using CoWinService.Domain.Interfaces;
using CoWinService.Domain.Model;
using System.Collections.Generic;

namespace CoWinService.Services
{
    public class CoWinServer : ICoWinServer
    {
        private readonly Configuration _config;
        private readonly HttpClient _httpClient;
        private readonly IAmazonSimpleNotificationService _snsClient;
        private static HashSet<string> _sessionIds = new HashSet<string>();

        // Constructor
        public CoWinServer(HttpClient client, IAmazonSimpleNotificationService snsClient)
        {
            _config = new Configuration
            {
                CoWinEndpoint = Environment.GetEnvironmentVariable("ENV_COWIN_ENDPOINT"),
                SnsTopicArn = Environment.GetEnvironmentVariable("ENV_SNS_TOPIC")
            };
            _snsClient = snsClient;
            _httpClient = client;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.11 (KHTML, like Gecko) Chrome/23.0.1271.95 Safari/537.11");
        }

        public async Task SearchByPincode(string pincode, string date)
        {
            try
            {
                var uri = ConstructHttpRequestMessageByPin(pincode, date);

                HttpResponseMessage response = await _httpClient.GetAsync(uri);
                string responseText = await response.Content.ReadAsStringAsync();
                if (responseText.IndexOf("<") == 0)
                {
                    // The request could not be satisfied HTML error page sent, no need to process this error
                    Console.WriteLine($"ERROR: Too much requests, no need to process this error");
                    return;
                }

                var centersArray = JsonConvert.DeserializeObject<Centers>(responseText);
                if (centersArray != null && centersArray.centers != null)
                {
                    foreach (VaccineCenter center in centersArray.centers)
                    {
                        foreach (var session in center.sessions)
                        {
                            // if the age limit is 18+ and available capacity is more than zero
                            // then get notified quickly by SMS or email
                            if (session.min_age_limit == 18 && session.available_capacity > 0)
                            {
                                await publishMessage(center, session);
                                _sessionIds.Add(session.session_id);    // save the session id to avoid duplicate messages
                            }
                            // enable this code if you want to get notification for 45+ years
                            //else if (session.min_age_limit == 45 && session.available_capacity > 0)
                            //{
                            //    await publishMessage(center, session);
                            //    _sessionIds.Add(session.session_id);
                            //}
                        }
                    }
                }
            }
            catch (Exception exceptionObject)
            {
                throw exceptionObject;
            }
        }

        public async Task SearchByDistrict(string districtCode, string date)
        {
            try
            {
                var uri = ConstructHttpRequestMessageByDistrict(districtCode, date);

                HttpResponseMessage response = await _httpClient.GetAsync(uri);
                string responseText = await response.Content.ReadAsStringAsync();
                if(responseText.IndexOf("<")==0)
                {
                    // The request could not be satisfied HTML error page sent, no need to process this error
                    Console.WriteLine($"ERROR: Too much requests, no need to process this error");
                    return;
                }
                var centersArray = JsonConvert.DeserializeObject<Centers>(responseText);
                if (centersArray != null && centersArray.centers != null)
                {
                    foreach (VaccineCenter center in centersArray.centers)
                    {
                        foreach (var session in center.sessions)
                        {
                            if (session.min_age_limit == 18 && session.available_capacity > 0)
                            {
                                await publishMessage(center, session);
                                _sessionIds.Add(session.session_id);    // save the session id to avoid duplicate messages
                            }
                            // add similar else if condition as mentioned above, if you want to get notification for 45+ years
                        }
                    }
                }
            }
            catch (Exception exceptionObject)
            {
                Console.WriteLine($"Error : {exceptionObject.Message}");
            }
        }

        private Uri ConstructHttpRequestMessageByPin(string pincode, string date)
        {
               UriBuilder builder = new UriBuilder($"{_config.CoWinEndpoint}/v2/appointment/sessions/public/calendarByPin");
            builder.Query = $"pincode={pincode}&date={date}";
            return builder.Uri;
        }

        private Uri ConstructHttpRequestMessageByDistrict(string districtId, string date)
        {
            UriBuilder builder = new UriBuilder($"{_config.CoWinEndpoint}/v2/appointment/sessions/public/calendarByDistrict");
            builder.Query = $"district_id={districtId}&date={date}";
            return builder.Uri;
        }

        private async Task publishMessage(VaccineCenter center, Session session)
        {
            // this prevents the duplicate email/messages publishing
            if (!_sessionIds.Contains(session.session_id))
            {
                MessageFormat message = new MessageFormat();
                message.name = center.name;
                message.address = center.address;
                message.available_capacity = session.available_capacity;
                message.min_age_limit = session.min_age_limit;
                message.vaccine = session.vaccine;
                message.pincode = center.pincode;
                var request = JsonConvert.SerializeObject(message);
                var publishRequest = new PublishRequest { TopicArn = _config.SnsTopicArn, Message = request };

                PublishResponse snsResponse = await _snsClient.PublishAsync(publishRequest);
                Console.WriteLine($"SUCCESS: published message response code : {snsResponse.HttpStatusCode}");
            }
        }
    }
}
