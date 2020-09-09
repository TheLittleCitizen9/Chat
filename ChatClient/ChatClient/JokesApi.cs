using RestSharp;

namespace ChatClient
{
    public class JokesApi
    {
        private const string URL = "https://sv443.net/jokeapi/v2/joke/Any?format=txt";

        public string SendRequest()
        {
            RestClient restClient = new RestClient(URL);
            var request = new RestRequest(Method.GET);
            IRestResponse response = restClient.Execute(request);
            return response.Content;
        }
    }
}