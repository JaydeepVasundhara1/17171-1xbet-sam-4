using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Brazil : MonoBehaviour
{
    private string apiUrl = "https://v3.football.api-sports.io/fixtures?date=2024-07-11";

    void Start()
    {
        StartCoroutine(GetFixtures());
    }

    IEnumerator GetFixtures()
    {
        UnityWebRequest request = UnityWebRequest.Get(apiUrl);
        request.SetRequestHeader("X-RapidAPI-Key", " 47b506162d9e9c9c5515dff037b59ac6");
        request.SetRequestHeader("X-RapidAPI-Host", "http://api-football-v1.p.rapidapi.com/");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(request.error);
        }
        else
        {
            string jsonResponse = request.downloadHandler.text;
            ParseAndDisplayFixtures(jsonResponse);
        }
    }

    void ParseAndDisplayFixtures(string jsonResponse)
    {
        // Parse the JSON response and display the fixtures
        var fixtures = JsonUtility.FromJson<FixturesResponse>(jsonResponse);

        foreach (var fixture in fixtures.response)
        {
            if (fixture.league.country == "Brazil")
            {
                Debug.Log($"Match: {fixture.teams.home.name} vs {fixture.teams.away.name}");
                Debug.Log($"Date: {fixture.fixture.date}");


            }
        }
    }

    [System.Serializable]
    public class FixturesResponse
    {
        public Fixture[] response;
    }

    [System.Serializable]
    public class Fixture
    {
        public League league;
        public Teams teams;
        public FixtureInfo fixture;
    }

    [System.Serializable]
    public class League
    {
        public string country;
    }

    [System.Serializable]
    public class Teams
    {
        public Team home;
        public Team away;
    }

    [System.Serializable]
    public class Team
    {
        public string name;
    }

    [System.Serializable]
    public class FixtureInfo
    {
        public string date;
    }
}
