using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using DG.Tweening;
using UnityEngine.UI;

public class GetLeague : MonoBehaviour
{
    public static GetLeague instance { get; set; }
    public string URL = GameManger.BaseURL + "standings?season=2024&league=71";
    public GetData getDats;
    public GameObject LeaguePositonPrefab;
    public GameObject NoDataFoundPrefab;
    public Transform LeaguePositonPrefabParent;

    public CurrentLeagueGetData leagueInfo;
    public int leagueId;
    public int currentSeason;
    public GetData currentLeagueData;
    public GameObject LeagueStandingsPrefab;
    public Transform RegularSeasonPrefabParent;
    public Transform ChampionshipRoundPrefabParent;
    public Transform ConferenceLeaguePlayOffPrefabParent;
    public Transform RelegationRoundPrefabParent;
    private int standingsCount;

    public Transform TeamDetailsGameObjectParent;

    private void Awake()
    {
        instance = this;
    }

    public void GetCurrentLeague(string name, string country)
    {
        StartCoroutine(GetCurrentLeagueApiCall(name, country));
    }

    public IEnumerator GetCurrentLeagueApiCall(string name, string country)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GameManger.BaseURL + "leagues?country=" + country + "&name=" + name + "&current=true"))
        {
            webRequest.SetRequestHeader("X-RapidAPI-Key", GameManger.RapidAPI_Key);
            webRequest.SetRequestHeader("X-RapidAPI-Host", GameManger.RapidAPI_Host);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;

                leagueInfo = JsonUtility.FromJson<CurrentLeagueGetData>(jsonResponse);
                //Debug.Log(leagueInfo.response[0].league.id.GetType());
                leagueId = leagueInfo.response[0].league.id;
                currentSeason = leagueInfo.response[0].seasons[0].year;

                GetCurrentLeagueStandings(leagueId, currentSeason);
            }
        }
    }

    public void GetCurrentLeagueStandings(int LeagueID, int Season)
    {
        StartCoroutine(GetCurrentLeagueStandingsApiCall(LeagueID, Season));

        for (int i = 0; i < RegularSeasonPrefabParent.childCount; i++)
        {
            Destroy(RegularSeasonPrefabParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < ChampionshipRoundPrefabParent.childCount; i++)
        {
            Destroy(ChampionshipRoundPrefabParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < ConferenceLeaguePlayOffPrefabParent.childCount; i++)
        {
            Destroy(ConferenceLeaguePlayOffPrefabParent.GetChild(i).gameObject);
        }

        for (int i = 0; i < RelegationRoundPrefabParent.childCount; i++)
        {
            Destroy(RelegationRoundPrefabParent.GetChild(i).gameObject);
        }
    }

    public IEnumerator GetCurrentLeagueStandingsApiCall(int LeagueID, int Season)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GameManger.BaseURL + "standings?season=2025" + "&league=" + LeagueID))
        {
            webRequest.SetRequestHeader("X-RapidAPI-Key", GameManger.RapidAPI_Key);
            webRequest.SetRequestHeader("X-RapidAPI-Host", GameManger.RapidAPI_Host);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;

                if (jsonResponse.Contains("[["))
                {
                    jsonResponse = jsonResponse.Replace("[[", "[");
                }
                if (jsonResponse.Contains("]]"))
                {
                    jsonResponse = jsonResponse.Replace("]]", "]");
                }

                if (jsonResponse.Contains("],["))
                {
                    jsonResponse = jsonResponse.Replace("],[", ",");
                }

                currentLeagueData = JsonUtility.FromJson<GetData>(jsonResponse);

                if (currentLeagueData.response.Count == 0)
                {
                    currentSeason--;
                    GetCurrentLeagueStandings(leagueId, currentSeason);                    
                }

                else
                {
                    standingsCount = currentLeagueData.response[0].league.standings.Count;
                    SetStandings();
                }
            }
        }
    }

    public void SetStandings()
    {
        SetRegularSeasonStandings();
        //SetChampionshipRoundStandings();
        //SetConferenceLeaguePlayOffStandings();
        //SetRelegationRoundStandings();
    }

    public void SetRegularSeasonStandings()
    {
        for (int i = 0; i < 30; i++)
        {
            GameObject game = Instantiate(LeagueStandingsPrefab);
            game.transform.SetParent(RegularSeasonPrefabParent);
            game.transform.localScale = Vector3.one;
            game.GetComponent<RectTransform>().localPosition = Vector3.zero;
            game.GetComponent<LeagueStandingsValue>().SetupDataFromApiReceived(currentLeagueData.response[0].league.standings[i].rank,
               currentLeagueData.response[0].league.standings[i].team.logo,
               currentLeagueData.response[0].league.standings[i].team.name,
               currentLeagueData.response[0].league.standings[i].all.played,
               currentLeagueData.response[0].league.standings[i].all.win,
               currentLeagueData.response[0].league.standings[i].all.draw,
               currentLeagueData.response[0].league.standings[i].all.lose,
               currentLeagueData.response[0].league.standings[i].points,
               currentLeagueData.response[0].league.standings[i].team.id);
            game.GetComponent<Button>().onClick.AddListener(ShowTeamDetails);
        }
    }

    public void SetChampionshipRoundStandings()
    {
        if(standingsCount > 16)
        {
            for(int i = 0; i < 6; i++)
            {
                GameObject game = Instantiate(LeagueStandingsPrefab);
                game.transform.SetParent(ChampionshipRoundPrefabParent);
                game.transform.localScale = Vector3.one;
                game.GetComponent<RectTransform>().localPosition = Vector3.zero;
                game.GetComponent<LeagueStandingsValue>().SetupDataFromApiReceived(currentLeagueData.response[0].league.standings[i].rank,
                   currentLeagueData.response[0].league.standings[i].team.logo,
                   currentLeagueData.response[0].league.standings[i].team.name,
                   currentLeagueData.response[0].league.standings[i].all.played,
                   currentLeagueData.response[0].league.standings[i].all.win,
                   currentLeagueData.response[0].league.standings[i].all.draw,
                   currentLeagueData.response[0].league.standings[i].all.lose,
                   currentLeagueData.response[0].league.standings[i].points,
                   currentLeagueData.response[0].league.standings[i].team.id);
                game.GetComponent<Button>().onClick.AddListener(ShowTeamDetails);
            }
        }
        else
        {
            SetEmptyChampionshipRoundStandings();
        }
    }

    public void SetConferenceLeaguePlayOffStandings()
    {
        if (standingsCount > 16)
        {
            for (int i = 10; i < 16; i++)
            {
                GameObject game = Instantiate(LeagueStandingsPrefab);
                game.transform.SetParent(ConferenceLeaguePlayOffPrefabParent);
                game.transform.localScale = Vector3.one;
                game.GetComponent<RectTransform>().localPosition = Vector3.zero;
                game.GetComponent<LeagueStandingsValue>().SetupDataFromApiReceived(currentLeagueData.response[0].league.standings[i].rank,
                   currentLeagueData.response[0].league.standings[i].team.logo,
                   currentLeagueData.response[0].league.standings[i].team.name,
                   currentLeagueData.response[0].league.standings[i].all.played,
                   currentLeagueData.response[0].league.standings[i].all.win,
                   currentLeagueData.response[0].league.standings[i].all.draw,
                   currentLeagueData.response[0].league.standings[i].all.lose,
                   currentLeagueData.response[0].league.standings[i].points,
                   currentLeagueData.response[0].league.standings[i].team.id);
                game.GetComponent<Button>().onClick.AddListener(ShowTeamDetails);
            }
        }
        else
        {
            SetEmptyConferenceLeaguePlayOffStandings();
        }
    }

    public void SetRelegationRoundStandings()
    {
        if (standingsCount > 16)
        {
            for (int i = 6; i < 10; i++)
            {
                GameObject game = Instantiate(LeagueStandingsPrefab);
                game.transform.SetParent(RelegationRoundPrefabParent);
                game.transform.localScale = Vector3.one;
                game.GetComponent<RectTransform>().localPosition = Vector3.zero;
                game.GetComponent<LeagueStandingsValue>().SetupDataFromApiReceived(currentLeagueData.response[0].league.standings[i].rank,
                   currentLeagueData.response[0].league.standings[i].team.logo,
                   currentLeagueData.response[0].league.standings[i].team.name,
                   currentLeagueData.response[0].league.standings[i].all.played,
                   currentLeagueData.response[0].league.standings[i].all.win,
                   currentLeagueData.response[0].league.standings[i].all.draw,
                   currentLeagueData.response[0].league.standings[i].all.lose,
                   currentLeagueData.response[0].league.standings[i].points,
                   currentLeagueData.response[0].league.standings[i].team.id);
                game.GetComponent<Button>().onClick.AddListener(ShowTeamDetails);
            }
        }
        else
        {
            SetEmptyRelegationRoundStandings();
        }
    }

    private void SetEmptyChampionshipRoundStandings()
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject game = Instantiate(LeagueStandingsPrefab);
            game.transform.SetParent(ChampionshipRoundPrefabParent);
            game.transform.localScale = Vector3.one;
            game.GetComponent<RectTransform>().localPosition = Vector3.zero;
            game.GetComponent<LeagueStandingsValue>().SetupDataFromApiReceived(i + 1, null, "TBD", 0, 0, 0, 0, 0, 0);
        }
    }

    private void SetEmptyConferenceLeaguePlayOffStandings()
    {
        for (int i = 0; i < 6; i++)
        {
            GameObject game = Instantiate(LeagueStandingsPrefab);
            game.transform.SetParent(ConferenceLeaguePlayOffPrefabParent);
            game.transform.localScale = Vector3.one;
            game.GetComponent<RectTransform>().localPosition = Vector3.zero;
            game.GetComponent<LeagueStandingsValue>().SetupDataFromApiReceived(i + 1, null, "TBD", 0, 0, 0, 0, 0, 0);
        }
    }

    private void SetEmptyRelegationRoundStandings()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject game = Instantiate(LeagueStandingsPrefab);
            game.transform.SetParent(RelegationRoundPrefabParent);
            game.transform.localScale = Vector3.one;
            game.GetComponent<RectTransform>().localPosition = Vector3.zero;
            game.GetComponent<LeagueStandingsValue>().SetupDataFromApiReceived(i + 1, null, "TBD", 0, 0, 0, 0, 0, 0);
        }
    }

    public void ShowTeamDetails()
    {
        TeamDetailsGameObjectParent.DOLocalMoveX(0, 0.25f);
    }

    public void HideTeamDetails()
    {
        TeamDetailsGameObjectParent.DOLocalMoveX(-1280, 0.25f);
    }

    public void Get_LeagueApiCall(int LeagueID, int Season, string HomeName, string AwayName)
    {
        StartCoroutine(Get_League(LeagueID, Season, HomeName, AwayName));

        for (int i = 0; i < LeaguePositonPrefabParent.childCount; i++)
        {
            Destroy(LeaguePositonPrefabParent.GetChild(i).gameObject);
        }
    }

    public IEnumerator Get_League(int LeagueID, int Season, string HomeName, string AwayName)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(GameManger.BaseURL + "standings?season=" + Season + "&league=" + LeagueID))
        {
            webRequest.SetRequestHeader("X-RapidAPI-Key", GameManger.RapidAPI_Key);
            webRequest.SetRequestHeader("X-RapidAPI-Host", GameManger.RapidAPI_Host);

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;                

                if (jsonResponse.Contains("[["))
                {
                    jsonResponse = jsonResponse.Replace("[[", "[");
                }
                if (jsonResponse.Contains("]]"))
                {
                    jsonResponse = jsonResponse.Replace("]]", "]");
                }

                if (jsonResponse.Contains("],["))
                {
                    jsonResponse = jsonResponse.Replace("],[", ",");
                }
                getDats = JsonUtility.FromJson<GetData>(jsonResponse);
                if (getDats.response.Count > 0)
                {
                    //int rank = getDats.response[0].league.standings[0][0].rank;                   
                    
                    for (int i = 0; i < getDats.response[0].league.standings.Count; i++)
                    {
                        GameObject game = Instantiate(LeaguePositonPrefab);
                        game.transform.SetParent(LeaguePositonPrefabParent);
                        game.transform.localScale = Vector3.one;
                        game.GetComponent<RectTransform>().localPosition = Vector3.zero;
                        game.GetComponent<LeagueInValue>().SetupDataFromApiReceived(getDats.response[0].league.standings[i].rank,
                           getDats.response[0].league.standings[i].team.logo,
                           getDats.response[0].league.standings[i].team.name,
                           getDats.response[0].league.standings[i].all.played,
                           getDats.response[0].league.standings[i].all.win,
                           getDats.response[0].league.standings[i].all.draw,
                           getDats.response[0].league.standings[i].all.lose,
                           getDats.response[0].league.standings[i].points);

                        if (HomeName.ToLower().Trim() == getDats.response[0].league.standings[i].team.name.ToLower().Trim())
                        {
                            if (getDats.response[0].league.standings[i].rank % 10 == 1)
                            {
                                GameManger.instance.HomeTeamRank.text = getDats.response[0].league.standings[i].rank + " st";
                            }
                            else if (getDats.response[0].league.standings[i].rank % 10 == 2)
                            {
                                GameManger.instance.HomeTeamRank.text = getDats.response[0].league.standings[i].rank + " nd";
                            }
                            else if (getDats.response[0].league.standings[i].rank % 10 == 3)
                            {
                                GameManger.instance.HomeTeamRank.text = getDats.response[0].league.standings[i].rank + " rd";
                            }
                            else
                            {
                                GameManger.instance.HomeTeamRank.text = getDats.response[0].league.standings[i].rank + " th";
                            }
                        }
                        else if (AwayName.ToLower().Trim() == getDats.response[0].league.standings[i].team.name.ToLower().Trim())
                        {
                            if (getDats.response[0].league.standings[i].rank % 10 == 1)
                            {
                                GameManger.instance.awayTeamRank.text = getDats.response[0].league.standings[i].rank + " st";
                            }
                            else if (getDats.response[0].league.standings[i].rank % 10 == 2)
                            {
                                GameManger.instance.awayTeamRank.text = getDats.response[0].league.standings[i].rank + " nd";
                            }
                            else if (getDats.response[0].league.standings[i].rank % 10 == 3)
                            {
                                GameManger.instance.awayTeamRank.text = getDats.response[0].league.standings[i].rank + " rd";
                            }
                            else
                            {
                                GameManger.instance.awayTeamRank.text = getDats.response[0].league.standings[i].rank + " th";
                            }
                        }
                    }
                }
                else
                {
                    GameObject game = Instantiate(NoDataFoundPrefab);
                    game.transform.SetParent(LeaguePositonPrefabParent);
                    game.transform.localScale = Vector3.one;
                    game.GetComponent<RectTransform>().localPosition = Vector3.zero;

                    GameManger.instance.HomeTeamRank.text = "00";
                    GameManger.instance.awayTeamRank.text = "00";
                }
            }
        }
    }

    [System.Serializable]
    public class GetData
    {
        public string get;
        public List<Response> response;
        public int results;
    }

    public class CurrentLeagueGetData
    {
        public string get;
        public List<CurrentLeagueResponse> response;
        public int results;
    }

    [System.Serializable]
    public class Response
    {
        public League league;
        //public List<Season> season;
    }

    [System.Serializable]
    public class CurrentLeagueResponse
    {
        public CurrentLeague league;
        public List<Season> seasons;
    }

    [System.Serializable]
    public class Season
    {
        public int year;
    }

    [System.Serializable]
    public class CurrentLeague
    {
        public int id;
    }

    [System.Serializable]
    public class League
    {
        public int id;
        public string name;
        public string country;
        public string logo;
        public string flag;
        public int season;
        public List<Standing> standings;
        //public List<List<StandingObj>> standings; 
        //public Root root;
    }

    [System.Serializable]
    public class Standing
    {
        public int rank;
        public Team team;
        public int points;
        public AllObj all;
    }

    [System.Serializable]
    public class Team
    {
        public int id;
        public string name;
        public string logo;

    }

    [System.Serializable]
    public class AllObj
    {
        public int played;
        public int win;
        public int draw;
        public int lose;
    }

    [System.Serializable]
    public class Paging
    {
        public int current;
        public int total;
    }

    [System.Serializable]
    public class Parameters
    {
        public string season;
        public string league;
    }

    [System.Serializable]
    public class Root
    {
        //public string get;
        public Parameters parameters;
        //public List<object> errors;
        public int results;
        //public Paging paging;
        //public Response[] response;
    }



}
