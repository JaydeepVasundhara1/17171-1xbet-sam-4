using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FootBallTeam_Name : MonoBehaviour
{
    public static FootBallTeam_Name instance { get; set; }
    public GameObject FootBall_Level;
    public Transform Content;
    public string URL = "https://v3.football.api-sports.io/predictions?fixture=";

    private float startValue = 0f;

    private void Awake()
    {
        instance = this;
    }

    [System.Obsolete]
    public void FootBall_Team()
    {
        var fixtures = ApiResponce.instacne.brazilResponse;

        for (int j = 0; j < Content.transform.childCount; j++)
        {
            Destroy(Content.transform.GetChild(j).gameObject);
        }


        for (int i = 0; i < fixtures.Count; i++)
        {
            GameObject game = Instantiate(FootBall_Level, Content.transform.position, Quaternion.identity, Content);
            TeamInDeta teamIn = game.GetComponent<TeamInDeta>();


            teamIn.Home_Text.text = fixtures[i].teams.home.name.ToString();
            teamIn.Away_Text.text = fixtures[i].teams.away.name.ToString();


            if (fixtures[i].teams.home.winner == "true" || fixtures[i].teams.home.winner == "false")
            {
                teamIn.Result.text = fixtures[i].goals.home + " : " + fixtures[i].goals.away;

                teamIn.ResultObject.SetActive(true);
                teamIn.Match_TimeOnject.SetActive(false);
            }
            else
            {
                teamIn.ResultObject.SetActive(false);
                teamIn.Match_TimeOnject.SetActive(true);

                string time = fixtures[i].fixture.date;
                var set_Time = time.Split('-', 'T', ':', '+');

                teamIn.Match_Time.text = int.Parse(set_Time[3]) + ":" + int.Parse(set_Time[4]).ToString("00");
            }
            //print("--Match Time----- " + int.Parse(set_Time[0]) + ":" + int.Parse(set_Time[1]) + ":" + int.Parse(set_Time[2]) + ":" +
            //    int.Parse(set_Time[3]) + ":" + int.Parse(set_Time[4]) + ":" + int.Parse(set_Time[5]) + ":" + int.Parse(set_Time[6]));



            teamIn.season = fixtures[i].league.season;
            teamIn.league = fixtures[i].league.id;
            teamIn.fixture_id = fixtures[i].fixture.id;

            // --->  set Logo Image --------
            Davinci.get().load(fixtures[i].teams.home.logo).into(teamIn.Home_Logo).start();
            Davinci.get().load(fixtures[i].teams.away.logo).into(teamIn.Away_Logo).start();

            //StartCoroutine(GetTexture(fixtures[i].teams.home.logo, teamIn.Home_Logo));
            //StartCoroutine(GetTexture(fixtures[i].teams.away.logo, teamIn.Away_Logo));

            //StartCoroutine(GetPredictions.instance.Get_predictions(GetPredictions.instance.URL + fixtures[i].fixture.id, teamIn));
            //print("----fixture.id-   " + GetPredictions.instance.URL + fixtures[i].fixture.id);

            StartCoroutine(GetTeamPredictions(fixtures[i].fixture.id, teamIn));

        }
    }

    public IEnumerator GetTeamPredictions(int fixtureId, TeamInDeta teamIn)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(URL + fixtureId))
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
                string jsonResponse = webRequest.downloadHandler.text;

                PredictionsResponse prediction = JsonUtility.FromJson<PredictionsResponse>(jsonResponse);

                //Debug.Log(prediction.response[0].predictions.percent.home);

                if(prediction.response.Count() > 0)
                {
                    string homeWinPercent = prediction.response[0].predictions.percent.home;
                    string drawWinPercent = prediction.response[0].predictions.percent.draw;
                    string awayWinPercent = prediction.response[0].predictions.percent.away;

                    float winValue = float.Parse(homeWinPercent.Replace("%", "")) / 100f;
                    float drawValue = float.Parse(drawWinPercent.Replace("%", "")) / 100f;
                    float loseValue = float.Parse(awayWinPercent.Replace("%", "")) / 100f;

                    if(teamIn.homeWinPercent != null) teamIn.homeWinPercent.text = homeWinPercent;
                    if(teamIn.drawWinPercent != null) teamIn.drawWinPercent.text = drawWinPercent;
                    if(teamIn.awayWinPercent != null) teamIn.awayWinPercent.text = awayWinPercent;

                    if (teamIn.winSlider != null && teamIn.drawSlider != null && teamIn.loseSlider != null)
                    {
                        teamIn.winSlider.value = startValue;
                        teamIn.drawSlider.value = startValue;
                        teamIn.loseSlider.value = startValue;

                        teamIn.winSlider.interactable = false;
                        teamIn.drawSlider.interactable = false;
                        teamIn.loseSlider.interactable = false;

                        DOTween.To(() => teamIn.winSlider.value, x => teamIn.winSlider.value = x, winValue, 0.33f);
                        DOTween.To(() => teamIn.drawSlider.value, x => teamIn.drawSlider.value = x, winValue + drawValue, 0.66f);
                        DOTween.To(() => teamIn.loseSlider.value, x => teamIn.loseSlider.value = x, 1, 1f);
                    }                     
                }

                else
                {
                    if (teamIn.winSlider != null && teamIn.drawSlider != null && teamIn.loseSlider != null)
                    {
                        teamIn.winSlider.gameObject.SetActive(false);
                        teamIn.drawSlider.gameObject.SetActive(false);
                        teamIn.loseSlider.gameObject.SetActive(false);
                    }
                    
                    if (teamIn.predictionsUnavailable != null) teamIn.predictionsUnavailable.gameObject.SetActive(true);
                    if (teamIn.homeWinPercent != null) teamIn.homeWinPercent.text = "0%";
                    if (teamIn.drawWinPercent != null) teamIn.drawWinPercent.text = "0%";
                    if (teamIn.awayWinPercent != null) teamIn.awayWinPercent.text = "0%";


                }

            }
        }
    }

    int num;
    IEnumerator GetTexture(string url, Image yourImg)
    {
        //print(“GetTexture------------>>“+ IconImgUrl);
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
            print("error "+www.error);
        }
        else
        {
            num++;
            print("---" + num);
            Texture2D tex = ((DownloadHandlerTexture)www.downloadHandler).texture;
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(tex.width / 2, tex.height / 2));
            yourImg.sprite = sprite;
            
        }
    }

    [System.Serializable]
    public class PredictionsResponse
    {
        public Response[] response;
    }

    [System.Serializable]
    public class Response
    {
        public Predictions predictions;
    }

    [System.Serializable]
    public class Predictions
    {
        public Percent percent;
    }

    [System.Serializable]
    public class Percent
    {
        public string home;
        public string draw;
        public string away;
    }
}
