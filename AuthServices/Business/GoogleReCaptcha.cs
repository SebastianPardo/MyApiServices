using Newtonsoft.Json.Linq;
using System.Net;

namespace AccountServices.Business
{
    public class GoogleReCaptcha
    {
        IConfiguration configuration;
        public GoogleReCaptcha(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task<bool> Validate(string recaptcha)
        {
            //string secretKey = Environment.GetEnvironmentVariable("GOOGLE_RECAPTCHA_KEY");            
            string secretKey = configuration["Authentication:GoogleRecaptcha:SecretKey"];
            HttpClient httpClient = new HttpClient();
            var httpResponse = await httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={recaptcha}");
            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                return false;
            }

            string jsonResponse = await httpResponse.Content.ReadAsStringAsync();
            dynamic jsonData = JObject.Parse(jsonResponse);
            if (jsonData.success != true.ToString().ToLower())
            {
                return false;
            }

            return true;
        }
    }
}
