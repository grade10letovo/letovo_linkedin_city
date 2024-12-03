using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class UniversityDataFetcher : MonoBehaviour
{
    [Serializable]
    public class University
    {
        public string name;
        public float latitude;
        public float longitude;
    }

    [Serializable]
    public class UniversityList
    {
        public List<University> universities;
    }

    public interface IHttpClient
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
    }

    public class DefaultHttpClient : IHttpClient
    {
        private readonly HttpClient _client = new HttpClient();
        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return _client.GetAsync(requestUri);
        }
    }

    [SerializeField]
    internal APIConfigSO apiConfig;
    private readonly IHttpClient _httpClient;

    public UniversityDataFetcher() : this(new DefaultHttpClient()) { }

    public UniversityDataFetcher(IHttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<University>> GetUniversitiesFromApi()
    {
        if (apiConfig == null || string.IsNullOrEmpty(apiConfig.apiDomen) || string.IsNullOrEmpty(apiConfig.apiUniversityUrl))
        {
            throw new Exception("API URL is not configured.");
        }

        HttpResponseMessage response = await _httpClient.GetAsync(apiConfig.apiDomen + apiConfig.apiUniversityUrl);
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error fetching data: {response.ReasonPhrase}");
        }

        string jsonResponse = await response.Content.ReadAsStringAsync();
        UniversityList universityList = JsonUtility.FromJson<UniversityList>($"{{\"universities\":{jsonResponse}}}");
        return universityList.universities;
    }
}
