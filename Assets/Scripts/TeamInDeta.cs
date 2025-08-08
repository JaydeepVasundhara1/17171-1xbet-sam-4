using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

public class TeamInDeta : MonoBehaviour
{
    public static TeamInDeta instance { get; set; }
    public Text Home_Text;
    public Image Home_Logo;
    public Text Away_Text;
    public Image Away_Logo;
    public Text Match_Time;

    public Slider winSlider;
    public Slider drawSlider;
    public Slider loseSlider;
    public Text homeWinPercent;
    public Text drawWinPercent;
    public Text awayWinPercent;
    public Text predictionsUnavailable;

    public Text Result;

    public Text Home_Predictions;
    public Text Draw_Predictions;
    public Text Away_Predictions;
    public RectTransform Home_Image;
    public RectTransform Draw_Image;
    public RectTransform Away_Image;

    public GameObject ResultObject;
    public GameObject Match_TimeOnject;

    public int season;
    public int league;
    public int fixture_id;


    public GameObject game;


    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        game = GameObject.Find("Result").gameObject;
    }

    public void OnClick_Set()
    {
        //game.transform.GetChild(0).gameObject.SetActive(true);

        //GameManger.instance.OnClick_Set();

        var gamemanget = GameManger.instance;
        gamemanget.ResultPanel.gameObject.SetActive(true);

        
        var team = TeamWinsOverAllDetails.instance;


        team.Home_Logo.sprite = Home_Logo.sprite;
        team.Away_Logo.sprite = Away_Logo.sprite;
        team.HomeTeamName.text = Home_Text.text;
        team.AwayTeamName.text = Away_Text.text;

        for (int i = 0; i < gamemanget.Home_Logo.Length; i++)
        {
            gamemanget.Home_Logo[i].sprite = Home_Logo.sprite;
            gamemanget.Away_Logo[i].sprite = Away_Logo.sprite;
        }

        if(ResultObject.activeSelf)
        {
            team.Result_text.text = Result.text;

            team.Result_object.SetActive(true);
            team.Match_object.SetActive(false);
        }
        else
        {
            team.MatchTime.text = Match_Time.text;
            team.Result_object.SetActive(false);
            team.Match_object.SetActive(true);
        }

        gamemanget. Home_Team_Short_Name.text = Home_Text.text.Substring(0, 3).ToUpper().ToString();
        gamemanget.Away_Team_Short_Name.text = Away_Text.text.Substring(0, 3).ToUpper().ToString();

        gamemanget.HomeMonentum_Name_Text.text = Home_Text.text.Substring(0, 3).ToUpper().ToString();
        gamemanget.AwayMonentum_Name_Text.text = Away_Text.text.Substring(0, 3).ToUpper().ToString();

        gamemanget.Att_HomeMonentum_Name_Text.text = Home_Text.text.Substring(0, 3).ToUpper().ToString();
        gamemanget.Att_AwayMonentum_Name_Text.text = Away_Text.text.Substring(0, 3).ToUpper().ToString();

        gamemanget.Def_HomeMonentum_Name_Text.text = Home_Text.text.Substring(0, 3).ToUpper().ToString();
        gamemanget.Def_AwayMonentum_Name_Text.text = Away_Text.text.Substring(0, 3).ToUpper().ToString();

        gamemanget.PredictionsApiCall(GetPredictions.instance.URL + fixture_id);
        GetLeague.instance.Get_LeagueApiCall(league, season, Home_Text.text, Away_Text.text);        

        gamemanget.OverAllButton();
    }
}
