using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TeamDetails : MonoBehaviour
{
    public static TeamDetails instance { get; set; }
    public Image TeamLogo;
    public Text TeamName;
    public Text LeaguePosition;
    public Text GamesPlayed;
    public Text WinRate;
    public Text PointsPerGame;

    public Text GoalsScoredShrinkHome;
    public Text GoalsScoredShrinkAway;
    public Text GoalsScoredExpandHome;
    public Text GoalsScoredExpandAway;
    public Text GoalsScoredExpandTotal;
    public Text GoalsScoredAverageExpandHome;
    public Text GoalsScoredAverageExpandAway;
    public Text GoalsScoredAverageExpandTotal;

    public Text GoalsConcededShrinkHome;
    public Text GoalsConcededShrinkAway;
    public Text GoalsConcededExpandHome;
    public Text GoalsConcededExpandAway;
    public Text GoalsConcededExpandTotal;
    public Text GoalsConcededAverageExpandHome;
    public Text GoalsConcededAverageExpandAway;
    public Text GoalsConcededAverageExpandTotal;

    public Text TotalGamesPlayedShrinkHome;
    public Text TotalGamesPlayedShrinkAway;
    public Text TotalGamesPlayedExpandHome;
    public Text TotalGamesPlayedExpandAway;
    public Text TotalGamesPlayedExpandTotal;

    public Text TotalWinsShrinkHome;
    public Text TotalWinsShrinkAway;
    public Text TotalWinsExpandHome;
    public Text TotalWinsExpandAway;
    public Text TotalWinsExpandTotal;
    public Text WinRateExpandHome;
    public Text WinRateExpandAway;
    public Text WinRateExpandTotal;

    public Text CleanSheetsShrinkHome;
    public Text CleanSheetsShrinkAway;
    public Text CleanSheetsExpandHome;
    public Text CleanSheetsExpandAway;
    public Text CleanSheetsExpandTotal;

    public Text TotalPenaltiesScoredShrink;
    public Text TotalPenaltiesPercentageShrink;
    public Text TotalPenaltiesScoredExpand;
    public Text TotalPenaltiesExpand;
    public Text TotalPenaltiesPercentageExpand;

    public Image[] recentFormImages;
    public Sprite winSprite;
    public Sprite drawSprite;
    public Sprite loseSprite;

    public Image loadingImage;
    public GameObject detailsPanel;

    void Awake()
    {
        instance = this;
    }

    public void GetTeamDetails(string url, int rank, int points, string name, string logoUrl)
    {
        loadingImage.gameObject.SetActive(true);
        detailsPanel.SetActive(false);

        TeamName.text = name;
        Davinci.get().load(logoUrl).into(TeamLogo).start();

        GamesPlayed.text = "";
        LeaguePosition.text = "";
        WinRate.text = "";
        PointsPerGame.text = "";

        StartCoroutine(Get_TeamDetailsApiCall(url, rank, points));
    }

    public IEnumerator Get_TeamDetailsApiCall(string url, int rank, int points)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("X-RapidAPI-Key", "497fc34e1de0969c721f48c9a9465e6d");
            webRequest.SetRequestHeader("X-RapidAPI-Host", "http://api-football-v1.p.rapidapi.com/");

            yield return webRequest.SendWebRequest();

            //yield return WaitForSeconds(.2f);
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                loadingImage.gameObject.SetActive(false);
                detailsPanel.SetActive(true);

                string jsonResponse = webRequest.downloadHandler.text;
                TeamDetailsResponse teamDetails = JsonUtility.FromJson<TeamDetailsResponse>(jsonResponse);

                //TeamName.text = teamDetails.response.team.name;
                //Davinci.get().load(teamDetails.response.team.logo).into(TeamLogo).start();
                GamesPlayed.text = teamDetails.response.fixtures.played.total.ToString();
                LeaguePosition.text = rank.ToString();

                float winRate = (float)teamDetails.response.fixtures.wins.total / teamDetails.response.fixtures.played.total * 100;
                if (float.IsNaN(winRate)) WinRate.text = "NA";
                else WinRate.text = (winRate).ToString("F1") + "%";

                float pointsPerGame = (float)points / teamDetails.response.fixtures.played.total;
                if (float.IsPositiveInfinity(pointsPerGame)) PointsPerGame.text = "NA";
                else PointsPerGame.text = (pointsPerGame).ToString("F2");

                GoalsScoredShrinkHome.text = teamDetails.response.goals.@for.total.home.ToString();
                GoalsScoredShrinkAway.text = teamDetails.response.goals.@for.total.away.ToString();

                GoalsScoredExpandHome.text = teamDetails.response.goals.@for.total.home.ToString();
                GoalsScoredExpandAway.text = teamDetails.response.goals.@for.total.away.ToString();
                GoalsScoredExpandTotal.text = teamDetails.response.goals.@for.total.total.ToString();

                GoalsScoredAverageExpandHome.text = teamDetails.response.goals.@for.average.home.ToString();
                GoalsScoredAverageExpandAway.text = teamDetails.response.goals.@for.average.away.ToString();
                GoalsScoredAverageExpandTotal.text = teamDetails.response.goals.@for.average.total.ToString();

                GoalsConcededShrinkHome.text = teamDetails.response.goals.against.total.home.ToString();
                GoalsConcededShrinkAway.text = teamDetails.response.goals.against.total.away.ToString();

                GoalsConcededExpandHome.text = teamDetails.response.goals.against.total.home.ToString();
                GoalsConcededExpandAway.text = teamDetails.response.goals.against.total.away.ToString();
                GoalsConcededExpandTotal.text = teamDetails.response.goals.against.total.total.ToString();

                GoalsConcededAverageExpandHome.text = teamDetails.response.goals.against.average.home.ToString();
                GoalsConcededAverageExpandAway.text = teamDetails.response.goals.against.average.away.ToString();
                GoalsConcededAverageExpandTotal.text = teamDetails.response.goals.against.average.total.ToString();

                TotalGamesPlayedShrinkHome.text = teamDetails.response.fixtures.played.home.ToString();
                TotalGamesPlayedShrinkAway.text = teamDetails.response.fixtures.played.away.ToString();

                TotalGamesPlayedExpandHome.text = teamDetails.response.fixtures.played.home.ToString();
                TotalGamesPlayedExpandAway.text = teamDetails.response.fixtures.played.away.ToString();
                TotalGamesPlayedExpandTotal.text = teamDetails.response.fixtures.played.total.ToString();

                TotalWinsShrinkHome.text = teamDetails.response.fixtures.wins.home.ToString();
                TotalWinsShrinkAway.text = teamDetails.response.fixtures.wins.away.ToString();

                TotalWinsExpandHome.text = teamDetails.response.fixtures.wins.home.ToString();
                TotalWinsExpandAway.text = teamDetails.response.fixtures.wins.away.ToString();
                TotalWinsExpandTotal.text = teamDetails.response.fixtures.wins.total.ToString();

                WinRateExpandHome.text = teamDetails.response.fixtures.played.home > 0 ? ((float)teamDetails.response.fixtures.wins.home / teamDetails.response.fixtures.played.home * 100).ToString("F1") + "%" : "NA";
                WinRateExpandAway.text = teamDetails.response.fixtures.played.home > 0 ? ((float)teamDetails.response.fixtures.wins.away / teamDetails.response.fixtures.played.away * 100).ToString("F1") + "%" : "NA";
                WinRateExpandTotal.text = teamDetails.response.fixtures.played.total > 0 ? ((float)teamDetails.response.fixtures.wins.total / teamDetails.response.fixtures.played.total * 100).ToString("F1") + "%" : "NA";

                CleanSheetsShrinkHome.text = teamDetails.response.clean_sheet.home.ToString();
                CleanSheetsShrinkAway.text = teamDetails.response.clean_sheet.away.ToString();

                CleanSheetsExpandHome.text = teamDetails.response.clean_sheet.home.ToString();
                CleanSheetsExpandAway.text = teamDetails.response.clean_sheet.away.ToString();
                CleanSheetsExpandTotal.text = teamDetails.response.clean_sheet.total.ToString();

                TotalPenaltiesScoredShrink.text = teamDetails.response.penalty.scored.total.ToString();
                TotalPenaltiesPercentageShrink.text = teamDetails.response.penalty.scored.percentage;

                TotalPenaltiesScoredExpand.text = teamDetails.response.penalty.scored.total.ToString();
                TotalPenaltiesPercentageExpand.text = teamDetails.response.penalty.scored.percentage;
                TotalPenaltiesExpand.text = teamDetails.response.penalty.total.ToString();

                string form = teamDetails.response.form;
                string recentForm;

                if(form.Length >= 5)
                {
                    recentForm = form.Substring(form.Length - 5);
                }

                else
                {
                    recentForm = form;
                }

                for (int i = 0; i < recentForm.Length; i++)
                {
                    if(recentForm[i] == 'W')
                    {
                        recentFormImages[i].sprite = winSprite;
                    }

                    else if (recentForm[i] == 'D')
                    {
                        recentFormImages[i].sprite = drawSprite;
                    }

                    else if (recentForm[i] == 'L')
                    {
                        recentFormImages[i].sprite = loseSprite;
                    }                    
                }
            }
        }
    }

    string ReverseString(string str)
    {
        char[] charArray = str.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }

    [System.Serializable]
    public class TeamDetailsResponse
    {
        //public string get;
        public Response response;
        //public int results;
    }

    [System.Serializable]
    public class Response
    {
        public Team team;
        public string form;
        public Goals goals;
        public Fixtures fixtures;
        public CleanSheet clean_sheet;
        public Penalty penalty;
    }

    [System.Serializable]
    public class Team
    {
        public string name;
        public string logo;
    }

    [System.Serializable]
    public class Goals
    {
        public For @for;
        public Against against;
    }

    [System.Serializable]
    public class For
    {
        public Total total;
        public Average average;
    }

    [System.Serializable]
    public class Against
    {
        public Total total;
        public Average average;
    }

    [System.Serializable]
    public class Total
    {
        public int home;
        public int away;
        public int total;
    }

    [System.Serializable]
    public class Average
    {
        public float home;
        public float away;
        public float total;
    }

    [System.Serializable]
    public class Fixtures
    {
        public Played played;
        public Wins wins;
    }

    [System.Serializable]
    public class Played
    {
        public int home;
        public int away;
        public int total;
    }

    [System.Serializable]
    public class Wins
    {
        public int home;
        public int away;
        public int total;
    }

    [System.Serializable]
    public class CleanSheet
    {
        public int home;
        public int away;
        public int total;
    }

    [System.Serializable]
    public class Penalty
    {
        public Scored scored;
        public int total;
    }

    [System.Serializable]
    public class Scored
    {
        public int total;
        public string percentage;
    }
}
