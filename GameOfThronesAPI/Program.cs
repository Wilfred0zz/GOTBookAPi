using System;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace GameOfThrons
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();

        static async Task Main(string[] args)
        {
            await ProcessRepositories();
        }

        private static async Task ProcessRepositories()
        {
            Console.WriteLine("The following we display information of a book from the Game of Thrones Series choosen at random");

            Random rand = new Random();
            var bookId = rand.Next(0, 10);

            try
            {
                var result = await client.GetAsync("https://www.anapioficeandfire.com/api/books/" + bookId);
                var resultRead = await result.Content.ReadAsStringAsync();

                var InformationPulled = JsonConvert.DeserializeObject<BooksInformation>(resultRead);

                Console.WriteLine("------");
                Console.WriteLine("Book # in series: " + bookId);
                Thread.Sleep(1000);
                Console.WriteLine("Book Title: " + InformationPulled.Name);
                Thread.Sleep(1000);
                Console.WriteLine("Number of pages book has: " + InformationPulled.NumberOfPages);
                Thread.Sleep(1000);
                Console.WriteLine("Publisher: " + InformationPulled.Publisher);
                Thread.Sleep(1000);
                Console.WriteLine("Author: " +string.Join(",", InformationPulled.Author));
              
            }
            catch(Exception)
            {
                Console.WriteLine("ERROR. Can't find book with ID bookId");
            }
        }
    }

    class BooksInformation {
        
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("numberOfPages")]
        public string NumberOfPages { get; set; }

        [JsonProperty("publisher")]
        public string Publisher { get; set; }

        [JsonProperty("authors")]
        [JsonConverter(typeof(SingleStringConverter<string>))]
        public List<string> Author { get; set; }
    }

    class SingleStringConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(List<T>));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken token = JToken.Load(reader);
            if (token.Type == JTokenType.Array)
            {
                return token.ToObject<List<T>>();
            }
            return new List<T> { token.ToObject<T>() };
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }


}