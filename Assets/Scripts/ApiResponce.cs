using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ApiResponce : MonoBehaviour
{
    public static ApiResponce instacne { get; set; }
    public string Date;
    public string CurrentDate;
    public GameObject NoMatchesObj;
    public GameObject LoadingScreen;
    public List<Response> brazilResponse = new List<Response>();
    public string apiURL = "https://v3.football.api-sports.io/fixtures?date=";

    private string pastFixturesCachePath;
    private string futureFixturesCachePath;
    private string currentFixturesCachePath;

    private DateTime pastCacheLastRefreshTime;
    private DateTime futureCacheLastRefreshTime;
    private DateTime currentCacheLastRefreshTime;

    private TimeSpan pastCacheRefreshTime = TimeSpan.FromDays(1);
    private TimeSpan futureCacheRefreshTime = TimeSpan.FromDays(1);
    private TimeSpan currentCacheRefreshTime = TimeSpan.FromMinutes(30);

    private DateTime currentTime = DateTime.Now;

    private void Awake()
    {
        instacne = this;
    }

    [System.Obsolete]
    private void Start()
    {
        //ClearCache();
        //PlayerPrefs.SetInt("FirstRun", 0);

        //  StartCoroutine(GetRequest(apiURL + Date));

        GetLeague.instance.GetCurrentLeague("liga profesional argentina", "argentina");

        pastFixturesCachePath = Path.Combine(Application.persistentDataPath, "pastFixturesCache.json");
        futureFixturesCachePath = Path.Combine(Application.persistentDataPath, "futureFixturesCache.json");
        currentFixturesCachePath = Path.Combine(Application.persistentDataPath, "currentFixturesCache.json");

        if (IsFirstRun())
        {
            if (!File.Exists(pastFixturesCachePath))
            {
                File.Create(pastFixturesCachePath);
                FetchPastFixtures();
            }

            if (!File.Exists(futureFixturesCachePath))
            {
                File.Create(futureFixturesCachePath);
                FetchFutureFixtures();
            }

            if (!File.Exists(currentFixturesCachePath))
            {
                File.Create(currentFixturesCachePath);
                FetchCurrentFixtures();
            }
        }

        else
        {
            CacheInfo pastCache = JsonUtility.FromJson<CacheInfo>(File.ReadAllText(pastFixturesCachePath));
            CacheInfo futureCache = JsonUtility.FromJson<CacheInfo>(File.ReadAllText(futureFixturesCachePath));
            CacheInfo currentCache = JsonUtility.FromJson<CacheInfo>(File.ReadAllText(currentFixturesCachePath));

            if (pastCache == null)
            {
                FetchPastFixtures();
            }

            else
            {
                pastCacheLastRefreshTime = DateTime.Parse(pastCache.timeStamp);
                if(DateTime.Now - pastCacheLastRefreshTime > pastCacheRefreshTime)
                {
                    FetchPastFixtures();
                }
            }

            if (futureCache == null)
            {
                FetchFutureFixtures();
            }

            else
            {
                futureCacheLastRefreshTime = DateTime.Parse(futureCache.timeStamp);
                if(DateTime.Now - futureCacheLastRefreshTime > futureCacheRefreshTime)
                {
                    FetchFutureFixtures();
                }
            }

            if (currentCache == null)
            {
                FetchCurrentFixtures();
            }

            else
            {
                currentCacheLastRefreshTime = DateTime.Parse(currentCache.timeStamp);
                if(DateTime.Now - currentCacheLastRefreshTime > currentCacheRefreshTime)
                {
                    FetchCurrentFixtures();
                }
            }
        }
    }

    private void Update()
    {
        if(DateTime.Now - currentTime <= currentCacheRefreshTime)
        {
            currentTime.AddSeconds(Time.deltaTime);
        }

        else
        {
            if(File.Exists(currentFixturesCachePath) && !string.IsNullOrEmpty(File.ReadAllText(currentFixturesCachePath)))
            {
                FetchCurrentFixtures();
            }
            
            currentTime = DateTime.Now;
        }
    }

    [System.Obsolete]
    public IEnumerator GetRequest(string uri)
    {
        print("---URL--- " + uri);
        LoadingScreen.SetActive(true);
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Set the request header with the API key
            webRequest.SetRequestHeader("X-RapidAPI-Key", "497fc34e1de0969c721f48c9a9465e6d");
            webRequest.SetRequestHeader("X-RapidAPI-Host", "http://api-football-v1.p.rapidapi.com/");

            // Send the request and wait for a response
            yield return webRequest.SendWebRequest();

            // Check for errors

            LoadingScreen.SetActive(false);
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);

            }
            else
            {
                print("-------FootBall Team webRequest----");
                string jsonResponse = webRequest.downloadHandler.text;
                DisplayFixtures(jsonResponse);
                FootBallTeam_Name.instance.FootBall_Team();

            }
        }

        if (brazilResponse.Count == 0)
        {
            NoMatchesObj.SetActive(true);
            print("NomatchesObj");
        }
        else
        {
            NoMatchesObj.SetActive(false);

        }
    }

    public void FetchPastFixtures()
    {
        StartCoroutine(FetchFixturesFromAPI(DateTime.Now.AddDays(-30), DateTime.Now.AddDays(-1), pastFixturesCachePath));
    }

    public void FetchFutureFixtures()
    {
        StartCoroutine(FetchFixturesFromAPI(DateTime.Now.AddDays(1), DateTime.Now.AddDays(30), futureFixturesCachePath));
    }

    public void FetchCurrentFixtures()
    {
        StartCoroutine(FetchFixturesFromAPI(DateTime.Now, DateTime.Now, currentFixturesCachePath));
    }

    FixturesResponse fixturesResponse;
    [System.Obsolete]
    void DisplayFixtures(string jsonResponse)
    {
        // Parse the JSON response and display the fixtures
        print("Jsomnn------------------>>" + jsonResponse);
        fixturesResponse = null;
        if (brazilResponse != null)
            brazilResponse.Clear();
        fixturesResponse = JsonUtility.FromJson<FixturesResponse>(jsonResponse);


        for (int i = 0; i < fixturesResponse.response.Length; i++)
        {
            if (fixturesResponse.response[i].league.country.Contains("Argentina"))
            {
                brazilResponse.Add(fixturesResponse.response[i]);
            }
        }
    }

    bool IsFirstRun()
    {
        if (PlayerPrefs.GetInt("FirstRun", 0) == 0)
        {
            // First run
            PlayerPrefs.SetInt("FirstRun", 1); // Set the flag to indicate the app has run before
            PlayerPrefs.Save();
            return true;
        }
        return false;
    }

    public bool DataExistsInCache(string date, string type)
    {
        string cachePath = GetPathFromType(type);

        if (File.Exists(cachePath))
        {
            string cachedData = File.ReadAllText(cachePath);

            if (!string.IsNullOrEmpty(cachedData))
            {
                CacheInfo cache = JsonUtility.FromJson<CacheInfo>(cachedData);

                foreach (KeyValuePair<string, FixturesResponse> kvp in cache.fixturesCache)
                {
                    if (kvp.key == date) return true;
                }
            }
        }

        return false;
    }

    public void LoadDataFromCache(string date, string type)
    {
        string cachePath = GetPathFromType(type);

        string cachedData = File.ReadAllText(cachePath);
        CacheInfo cache = JsonUtility.FromJson<CacheInfo>(cachedData);

        string fixtureResponse = "";
        foreach (KeyValuePair<string, FixturesResponse> kvp in cache.fixturesCache)
        {
            if (kvp.key == date)
            {
                fixtureResponse = JsonUtility.ToJson(kvp.value);
                break;
            }
        }
        DisplayFixtures(fixtureResponse);
        FootBallTeam_Name.instance.FootBall_Team();
        Debug.Log("Loaded from cache");

        if (brazilResponse.Count == 0)
        {
            NoMatchesObj.SetActive(true);
            print("NomatchesObj");
        }
        else
        {
            NoMatchesObj.SetActive(false);

        }
    }

    public IEnumerator FetchFixturesFromAPI(DateTime startDate, DateTime endDate, string cachePath)
    {
        CacheInfo cacheInfo = new CacheInfo();
        cacheInfo.fixturesCache = new List<KeyValuePair<string, FixturesResponse>>();

        for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
        {
            string formattedDate = date.ToString("yyyy-MM-dd");

            using (UnityWebRequest webRequest = UnityWebRequest.Get("https://v3.football.api-sports.io/fixtures?date=" + formattedDate))
            {
                // Set the request header with the API key
                webRequest.SetRequestHeader("X-RapidAPI-Key", "497fc34e1de0969c721f48c9a9465e6d");
                webRequest.SetRequestHeader("X-RapidAPI-Host", "http://api-football-v1.p.rapidapi.com/");

                // Send the request and wait for a response
                yield return webRequest.SendWebRequest();

                // Check for errors
                if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(webRequest.error);

                }
                else
                {
                    print("-------FootBall Team webRequest----");
                    string jsonResponse = webRequest.downloadHandler.text;

                    FixturesResponse fixturesResponse = JsonUtility.FromJson<FixturesResponse>(jsonResponse);

                    cacheInfo.fixturesCache.Add(new KeyValuePair<string, FixturesResponse>(formattedDate, fixturesResponse));
                }
            }
        }

        cacheInfo.timeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        File.WriteAllText(cachePath, JsonUtility.ToJson(cacheInfo));
    }

    private string GetPathFromType(string type)
    {
        if (type == "past")
        {
            return pastFixturesCachePath;
        }

        else if (type == "future")
        {
            return futureFixturesCachePath;
        }

        else if (type == "current")
        {
            return currentFixturesCachePath;
        }

        return "";
    }

    void ClearCache()
    {
        string path = Application.persistentDataPath;
        //PlayerPrefs.SetInt("FirstRun", 0);

        // Check if the directory exists
        if (Directory.Exists(path))
        {
            // Get all files in the directory
            string[] files = Directory.GetFiles(path);

            // Loop through all the files and delete them
            foreach (string file in files)
            {
                File.Delete(file);
                Debug.Log("Deleted file: " + file);
            }

            // Optionally, you can delete directories if needed
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                Directory.Delete(directory, true); // 'true' to delete recursively
                Debug.Log("Deleted directory: " + directory);
            }

            Debug.Log("All cache files cleared from persistentDataPath.");
        }
        else
        {
            Debug.Log("Persistent data path does not exist.");
        }
    }

    [System.Serializable]
    public class CacheInfo
    {
        public string timeStamp;
        public List<KeyValuePair<string, FixturesResponse>> fixturesCache;
    }

    [System.Serializable]
    public class KeyValuePair<TKey, TValue>
    {
        public TKey key;
        public TValue value;

        public KeyValuePair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
    }

    //[System.Serializable]
    //public class FixturesResponseDict
    //{
    //    public Dictionary<DateTime, FixturesResponse> fixturesDict;
    //}

    [System.Serializable]
    public class FixturesResponse
    {
        public Response[] response;
    }

    [System.Serializable]
    public class Response
    {
        public League league;
        public Teams teams;
        public FixtureInfo fixture;
        public Goals goals;
    }

    [System.Serializable]
    public class Goals
    {
        public int home;
        public int away;
    }

    [System.Serializable]
    public class League
    {
        public string country;
        public int id;
        public int season;
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
        public string logo;
        public string winner;

    }

    [System.Serializable]
    public class FixtureInfo
    {
        public string date;
        public int id;
    }

    internal IEnumerator GetRequest(object p)
    {
        throw new NotImplementedException();
    }


}


