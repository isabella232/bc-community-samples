using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Text;

namespace server
{
    public static class HeroFunctions
    {
        public static int tokenCount = 0;
        public static Random Random = new Random();
        public static int MinAbility = 5;
        public static int MaxAbility = 20;

        [FunctionName("GenerateCharacteristics")]
        public static HttpResponseMessage GenerateCharacteristics(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            var hero = new Hero()
            {
                TokenId = ++tokenCount,
                Accuracy = Random.Next(MinAbility, MaxAbility),
                Charisma = Random.Next(MinAbility, MaxAbility),
                Inteligence = Random.Next(MinAbility, MaxAbility),
                Speed = Random.Next(MinAbility, MaxAbility),
                Wisdom = Random.Next(MinAbility, MaxAbility)
            };

            var result = JsonConvert.SerializeObject(hero);
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(result, Encoding.UTF8, "application/json")
            };
        }
    }

    public class Hero
    {
        public int TokenId { get; set; }

        public string Name { get; set; }

        public int Wisdom { get; set; }

        public int Inteligence { get; set; }

        public int Charisma { get; set; }

        public int Speed { get; set; }

        public int Accuracy { get; set; }

        public int Might { get; set; }
    }
}
