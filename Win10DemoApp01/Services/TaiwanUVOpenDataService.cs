using Microsoft.ApplicationInsights;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Win10DemoApp01.Models;
using Windows.Data.Json;
using Windows.Web.Http;

namespace Win10DemoApp01.Services
{
    public class TaiwanUVOpenDataService
    {
        const string TaiwanUVOpenDataUrl = @"http://opendata.epa.gov.tw/ws/Data/UV/?format=json";
        const string TaiwanAirConditionOpenDataUrl = @"http://opendata.epa.gov.tw/ws/Data/AQX/?format=json";
        private string RawContent;

        public async Task<List<TaiwanCityUV>> GetTaiwanUVData()
        {

                List<TaiwanCityUV> taiwanUVData = new List<TaiwanCityUV>();

                string content = "";

                HttpClient httpClient = new HttpClient();
                try
                {
                    content = await httpClient.GetStringAsync(new Uri(TaiwanUVOpenDataUrl));

                    RawContent = content;
                    JsonArray jArray = JsonArray.Parse(content);
                    IJsonValue outValue;

                    string testContent = "";
                    foreach (var item in jArray)
                    {
                        JsonObject obj = item.GetObject();
                        // Assume there is a “backgroundImage” column coming back
                        if (obj.TryGetValue("SiteName", out outValue))
                        {
                            testContent += outValue.GetString() + " ";
                        }


                    }
                    RawContent = testContent;
                    RawContent = "There are " + taiwanUVData.Count + " " + RawContent;

                    taiwanUVData=DeserializeTaiwanUVJason(content);

                  


                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message+ " "+ ex.StackTrace);

                    var telemetry = new TelemetryClient();
                    telemetry.TrackException(ex);
                            
            }

            // Once your app is done using the HttpClient object call dispose to 

            httpClient.Dispose();
            return taiwanUVData;

        }

        public List<TaiwanCityUV> DeserializeTaiwanUVJason(string inTaiwanUVJsonContent)
        {
            List<TaiwanCityUV> parsedTaiwanUVData = new List<TaiwanCityUV>();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(IEnumerable<TaiwanCityUV>));
            using (MemoryStream stream = new MemoryStream(Encoding.Unicode.GetBytes(inTaiwanUVJsonContent)))
            {
                var rootObject = serializer.ReadObject(stream) as IEnumerable<TaiwanCityUV>;
                parsedTaiwanUVData = new List<TaiwanCityUV>(rootObject);
            }

            return parsedTaiwanUVData;
        }


    }
}
