using Newtonsoft.Json;
using services.varian.com.AriaWebConnect.Link;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace QAPatient.Services
{
    public static class AriaService
    {
        public static string HostName { get; set; }
        public static string Port { get; set; }
        public static void SetInitial()
        {
            HostName = "gatewayz-tbox.sde.cloud.varian.com";
            Port = "55051";
        }
        /// <summary>
        /// Creates a new patient in ARIA using the provided details.
        /// </summary>
        /// <param name="lastName">New Patient Last Name</param>
        /// <param name="firstName">New Patient First Name</param>
        /// <param name="MRN">New Patient MRN</param>
        /// <param name="apiKey">ARIA Access API key.</param>
        /// <returns></returns>
        public static string CreatePatient(string lastName, string firstName, string MRN, string apiKey)
        {
            // Create new patient providing required details.
            var createPatientRequest = new CreatePatientRequest()
            {
                LastName = new services.varian.com.AriaWebConnect.Common.String { Value = lastName },
                FirstName = new services.varian.com.AriaWebConnect.Common.String { Value = firstName },
                PatientId1 = new services.varian.com.AriaWebConnect.Common.String { Value = MRN }
            };
            //construct json request. 
            var request_base = "{\"__type\":\"";
            var request_createPatient = $"{request_base}CreatePatientRequest:http://services.varian.com/AriaWebConnect/Link\",{JsonConvert.SerializeObject(createPatientRequest).TrimStart('{')}}}";
            string response_createPatient= SendData(request_createPatient, true, apiKey);
            return response_createPatient;
        }

        public static string SendData(string request, bool bIsJson, string apiKey)
        {
            var sMediaTYpe = bIsJson ? "application/json" :
            "application/xml";
            var sResponse = System.String.Empty;
            // Create a new HttpClient with default credentials and pre-authentication.
            using (var c = new HttpClient(new
            HttpClientHandler()
            { UseDefaultCredentials = true, PreAuthenticate = true }))
            {
                if (c.DefaultRequestHeaders.Contains("ApiKey"))
                {
                    c.DefaultRequestHeaders.Remove("ApiKey");
                }
                c.DefaultRequestHeaders.Add("ApiKey", apiKey);
                //in App.Config, change this to the Resource ID for your REST Service.
                var gatewayURL = $"https://{HostName}:{Port}/Gateway/service.svc/interop/rest/Process";
                var task =
                c.PostAsync(gatewayURL,
                new StringContent(request, Encoding.UTF8,
                sMediaTYpe));
                Task.WaitAll(task);
                var responseTask =
                task.Result.Content.ReadAsStringAsync();
                Task.WaitAll(responseTask);
                sResponse = responseTask.Result;
            }
            return sResponse;
        }
    }
}
