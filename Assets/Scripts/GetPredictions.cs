using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using DG.Tweening;

public class GetPredictions : MonoBehaviour
{
    public static GetPredictions instance { get; set; }
    public string URL = "https://v3.football.api-sports.io/predictions?fixture=";
    
    private void Awake()
    {
        instance = this;
    }

    string _home;
    string _draw;
    string _away;
    public PredictionsResponse predi;
    [System.Obsolete]

    public IEnumerator Get_predictions(string url, TeamInDeta teamInDeta)
    {
        predi = null;
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
                string jsonResponse = webRequest.downloadHandler.text;

                yield return new WaitForSeconds(.2f);

                predi = JsonUtility.FromJson<PredictionsResponse>(jsonResponse);
                teamInDeta.Home_Predictions.text = predi.response[0].predictions.percent.home;
                teamInDeta.Draw_Predictions.text = predi.response[0].predictions.percent.draw;
                teamInDeta.Away_Predictions.text = predi.response[0].predictions.percent.away;
                //predictionsRes.Add(predi);


                string home = predi.response[0].predictions.percent.home;
                string draw = predi.response[0].predictions.percent.draw;
                string away = predi.response[0].predictions.percent.away;

                _home = Regex.Replace(home, "[^0-9]", "");
                _draw = Regex.Replace(draw, "[^0-9]", "");
                _away = Regex.Replace(away, "[^0-9]", "");

                //int home_value = (900 * int.Parse(_home) * 10) / 1000;
                //int draw_value = (900 * int.Parse(_draw) * 10) / 1000;
                //int away_value = (900 * int.Parse(_away) * 10) / 1000;

                teamInDeta.Home_Image.sizeDelta = new Vector2(int.Parse(_home) * 10, 26);
                teamInDeta.Draw_Image.sizeDelta = new Vector2(int.Parse(_draw) * 10, 26);
                teamInDeta.Away_Image.sizeDelta = new Vector2(int.Parse(_away) * 10, 26);

                teamInDeta.Home_Image.anchoredPosition = new Vector2(0, 0);
                teamInDeta.Draw_Image.anchoredPosition = new Vector2(int.Parse(_home) * 10, 0);
                teamInDeta.Away_Image.anchoredPosition = new Vector2(int.Parse(_draw) * 10 + int.Parse(_home) * 10, 0);
            }
        }
    }

    private object WaitForSeconds(float v)
    {
        throw new NotImplementedException();
    }

    public void FillAmount_3thPage()
    {
        var Page_3th = TeamResultFillValue.instance;

        var aa = TeamResultFillValue.instance;

        string b = aa.Home_Text;
        string c = aa.Drew_Text;
        string d = aa.Away_Text;

        var set_Time_1 = b.Split('%');
        var set_Time_2 = c.Split('%');
        var set_Time_3 = d.Split('%');

        float _a = float.Parse(set_Time_1[0]);
        float _b = float.Parse(set_Time_2[0]);
        float _c = float.Parse(set_Time_3[0]);

        DOTween.To(() => Page_3th.Home_Wins.fillAmount, x => Page_3th.Home_Wins.fillAmount = x, (_a / 100f), 1);
        DOTween.To(() => Page_3th.Draw_Wins.fillAmount, x => Page_3th.Draw_Wins.fillAmount = x, (_b / 100f), 1);
        DOTween.To(() => Page_3th.Away_Wins.fillAmount, x => Page_3th.Away_Wins.fillAmount = x, (_c / 100f), 1);

        //Page_3th.Home_Wins.fillAmount = (_a / 100);
        //Page_3th.Draw_Wins.fillAmount = (_b / 100);
        //Page_3th.Away_Wins.fillAmount = (_c / 100);

    }

    public void Fill_Line()
    {
        // 280+280+340
        // 190+290+420
        // 200+400+400
        // 100+450+450

        //    1000 ------ 100%
        //    900   ?
                                                    
        //  total fill amount 900
        //  1] 200%
        //  2] 400%
        //  3] 400%

        //  1]
        //      1000 ---- 200%
        //      900 --- ?
        // ->  ans = 180

        //  2]
        //      1000 ---- 400%
        //      900 --- ?
        //  -> ans = 360

        //  3]
        //      1000 ---- 400%
        //      900 --- ?
        //  -> ans = 360

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
