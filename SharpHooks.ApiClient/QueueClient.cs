using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpHooks.ApiClient
{
    public class QueueClient
    {
        private const string _baseUrl = "https://queue.sharphooks.net";

        private RestClient _client { get; set; }

        public QueueClient() : this(null, null)
        {
            _client = new RestClient(_baseUrl);
        }

        public QueueClient(string username, string password)
        {
            _client = new RestClient(_baseUrl);
            _client.Authenticator = new HttpBasicAuthenticator(username, password);
        }

        /// <summary>
        /// Adds a new job to the SharpHooks Queue
        /// </summary>
        /// <param name="appName">App identifier</param>
        /// <param name="data">Data to submit</param>
        /// <param name="delay">Delay in seconds</param>
        public void AddJob(string appName, object data, int? delay = null, string callbackUrl = null, string successUrl = null, string failUrl = null, string warnUrl = null)
        {
            var request = new RestRequest("{AppName}", Method.POST);
            request.AddUrlSegment("AppName", appName); // replaces matching token in request.Resource
            request.RequestFormat = DataFormat.Json;
            if (delay.HasValue)
            {
                request.AddHeader("Delay", delay.Value.ToString());
            }
            if (!string.IsNullOrWhiteSpace(callbackUrl))
            {
                request.AddHeader("callbackurl", callbackUrl);
            }
            if (!string.IsNullOrWhiteSpace(successUrl))
            {
                request.AddHeader("successurl", successUrl);
            }
            if (!string.IsNullOrWhiteSpace(failUrl))
            {
                request.AddHeader("failurl", failUrl);
            }
            if (!string.IsNullOrWhiteSpace(warnUrl))
            {
                request.AddHeader("warnurl", warnUrl);
            }
            request.AddBody(data);

            // execute the request
            var response = _client.Execute(request);
            if (response.ErrorException != null)
            {
                throw new ApplicationException("Failed to add job.", response.ErrorException);
            }
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                throw new ApplicationException("Failed to add job: " + response.Content);
            }
        }
    }
}
