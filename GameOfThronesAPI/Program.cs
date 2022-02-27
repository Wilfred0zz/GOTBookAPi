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
            Console.WriteLine("The following will display information of a book from the Game of Thrones Series choosen at random !");

            Random rand = new Random();
            var bookId = rand.Next(0, 10); //generated book number so that it can later fetch info on that specific book in the series

            try
            {
                var result = await client.GetAsync("https://www.anapioficeandfire.com/api/books/" + bookId);//makes call to api with book information generated
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

    class BooksInformation {//sets the property of json files strings to variables that can later be printed
        
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("numberOfPages")]
        public string NumberOfPages { get; set; }

        [JsonProperty("publisher")]
        public string Publisher { get; set; }

        [JsonProperty("authors")]
        [JsonConverter(typeof(SingleStringConverter<string>))]//converts the array without any properties into a string.
        public List<string> Author { get; set; }
    }

    class SingleStringConverter<T> : JsonConverter //class that a turns single line array of one object into an accessible string to be read
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