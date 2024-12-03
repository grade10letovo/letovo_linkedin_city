using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

public class UniversityDataFetcherTests
{
    private class MockHttpClient : UniversityDataFetcher.IHttpClient
    {
        private readonly HttpResponseMessage _response;

        public MockHttpClient(HttpResponseMessage response)
        {
            _response = response;
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return Task.FromResult(_response);
        }
    }

    private APIConfigSO CreateMockConfig(string apiDomen, string apiUniversityUrl)
    {
        var config = ScriptableObject.CreateInstance<APIConfigSO>();
        config.apiDomen = apiDomen;
        config.apiUniversityUrl = apiUniversityUrl;
        return config;
    }

    [Test]
    public async Task GetUniversitiesFromApi_ValidResponse_ReturnsUniversities()
    {
        // Arrange
        string jsonResponse = "[{\"name\":\"MIT\",\"latitude\":42.3601,\"longitude\":-71.0942}]";
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.OK,
            Content = new StringContent(jsonResponse)
        };
        var mockHttpClient = new MockHttpClient(mockResponse);
        var mockConfig = CreateMockConfig("https://mock-api.com", "/universities");

        var fetcher = new UniversityDataFetcher(mockHttpClient) { apiConfig = mockConfig };

        // Act
        List<UniversityDataFetcher.University> universities = await fetcher.GetUniversitiesFromApi();

        // Assert
        Assert.AreEqual(1, universities.Count);
        Assert.AreEqual("MIT", universities[0].name);
        Assert.AreEqual(42.3601f, universities[0].latitude);
        Assert.AreEqual(-71.0942f, universities[0].longitude);
    }

    [Test]
    public void GetUniversitiesFromApi_MissingConfig_ThrowsException()
    {
        // Arrange
        var mockHttpClient = new MockHttpClient(null);
        var fetcher = new UniversityDataFetcher(mockHttpClient) { apiConfig = null };

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () => await fetcher.GetUniversitiesFromApi());
        Assert.IsTrue(ex.Message.Contains("API URL is not configured"));
    }

    [Test]
    public void GetUniversitiesFromApi_ErrorResponse_ThrowsException()
    {
        // Arrange
        var mockResponse = new HttpResponseMessage
        {
            StatusCode = System.Net.HttpStatusCode.BadRequest,
            ReasonPhrase = "Bad Request"
        };
        var mockHttpClient = new MockHttpClient(mockResponse);
        var mockConfig = CreateMockConfig("https://mock-api.com", "/universities");

        var fetcher = new UniversityDataFetcher(mockHttpClient) { apiConfig = mockConfig };

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () => await fetcher.GetUniversitiesFromApi());
        Assert.IsTrue(ex.Message.Contains("Bad Request"));
    }
}
