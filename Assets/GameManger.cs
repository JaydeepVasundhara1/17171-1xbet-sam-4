using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Networking;
using System.Text.RegularExpressions;

public class GameManger : MonoBehaviour
{

    public static string BaseURL = "https://v3.football.api-sports.io/";
    public static string RapidAPI_Key = "497fc34e1de0969c721f48c9a9465e6d";
    public static string RapidAPI_Host = "api-football-v1.p.rapidapi.com";

    public static GameManger instance;
    public GameObject ResultPanel;
    //public GameObject TeamDetailsPanel;

    public Transform MainObjectsParent;
    public Transform MainObjectsBottomLine;

    public Transform LeagueObjectsParent;
    public Transform LeagueObjectsBottomLine;

    public Transform ObjectsParent;
    public Transform ObjectsBottomLine;

    public Image[] Home_Logo;
    public Image[] Away_Logo;

    public Button overAllButton;
    public Button attackButton;
    public Button defenseButton;

    public Button regularSeasonBtn;
    public Button championshipRoundBtn;
    public Button conferenceLeagueBtn;
    public Button relegationRoundBtn;

    public Sprite normalSprite;
    public Sprite selectedSprite;

    [Header("OverAll")]

    public Text Home_Predictions;
    public Text Home_Team_Short_Name;
    public Image Home_Wins;

    public Text Draw_Predictions;    
    public Image Draw_Wins;

    public Text Away_Predictions;
    public Text Away_Team_Short_Name;
    public Image Away_Wins;

    public Text HomeTeamRank;
    public Text awayTeamRank;



    [Header("OverAll - Monutam")]
    public Image HomeMonentum;
    public Text HomeMonentum_Name_Text;
    public Image HomeMonentumPin;
    public Image AwayMonentum;
    public Text AwayMonentum_Name_Text;
    public Image AwayMonentumPin;

    [Header("Total Goal Scored")]
    public Text Team1HomeScore;
    public Text Team1AwayScore;
    public Text Team1TotalScore;

    public Text Team2HomeScore;
    public Text Team2AwayScore;
    public Text Team2TotalScore;

    public Text Team1ShrinkTotalScore;
    public Text Team2ShrinkTotalScore;


    public Text Team1HomeAverage;
    public Text Team1AwayAverage;
    public Text Team1TotalAverage;

    public Text Team2HomeAverage;
    public Text Team2AwayAverage;
    public Text Team2TotalAverage;

    public Text Team1ShrinkTotalAverage;
    public Text Team2ShrinkTotalAverage;


    public Text MostCommonGoalsHome;
    public Text MostCommonGoalsAway;

    public Text MostConcededHome;
    public Text MostConcededsAway;


    [Header("Attack")]
    public Image Att_HomeMonentum;
    public Text Att_HomeMonentum_Name_Text;
    public Image Att_HomeMonentumPin;
    public Image Att_AwayMonentum;
    public Text Att_AwayMonentum_Name_Text;
    public Image Att_AwayMonentumPin;

    public Text Team1HomeScoreAttack;
    public Text Team1AwayScoreAttack;
    public Text Team1TotalScoreAttack;


    public Text Team2HomeScoreAttack;
    public Text Team2AwayScoreAttack;
    public Text Team2TotalScoreAttack;

    public Text Team1ShrinkTotalScoreAttack;
    public Text Team2ShrinkTotalScoreAttack;

    public Sprite WinSprite;
    public Sprite DrawSprite;
    public Sprite LossSprite;

    public Image[] ShrinkFormTeamData;
    public Image[] ShrinkFormTeam2Data;

    public Image[] ExpandFormTeamData;
    public Image[] ExpandFormTeam2Data;


    public Text Team1WinStreak;
    public Text Team1drawStreak;
    public Text Team1LossStreak;

    public Text Team2WinStreak;
    public Text Team2drawStreak;
    public Text Team2LossStreak;

    public Text Team1WinStreakShrink;
    public Text Team2WinStreakShrink;


    public Text paneltyGoalteam1Shrink;
    public Text paneltyGoalteam2Shrink;

    public Text paneltyGoalteam1ShrinkPercentage;
    public Text paneltyGoalteam2ShrinkPercentage;


    [Header("Defence")]
    public Image Def_HomeMonentum;
    public Text Def_HomeMonentum_Name_Text;
    public Image Def_HomeMonentumPin;
    public Image Def_AwayMonentum;
    public Text Def_AwayMonentum_Name_Text;
    public Image Def_AwayMonentumPin;

    public Text MostConcededHomeShrinkDefence;
    public Text MostConcededsAwayShrinkDefence;

    public Text MostConcededsTeam1homeDefence;
    public Text MostConcededsTeam1awatDefence;
    public Text MostConcededsTeam1TotalDefence;

    public Text MostConcededsTeam2homeDefence;
    public Text MostConcededsTeam2awatDefence;
    public Text MostConcededsTeam2TotalDefence;


    public Text CleanSheetHomeShrinkDefence;
    public Text CleanSheetAwayShrinkDefence;

    public Text CleanSheetTeam1homeDefence;
    public Text CleanSheetTeam1awatDefence;
    public Text CleanSheetTeam1TotalDefence;

    public Text CleanSheetTeam2homeDefence;
    public Text CleanSheetTeam2awatDefence;
    public Text CleanSheetTeam2TotalDefence;

    public Text paneltyGoalMissteam1Shrink;
    public Text paneltyGoalMissteam2Shrink;

    public Text paneltyGoalMissteam1ShrinkPercentage;
    public Text paneltyGoalMissteam2ShrinkPercentage;

    public GameObject ExitPanel;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(WaitForNextClick());
        }
    }

    private IEnumerator WaitForNextClick()
    {
        yield return null;

        float waitTime = 1.25f;
        float currentTime = 0f;

        while(currentTime < waitTime)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ExitPanel.SetActive(true);
                yield break;
            }

            else
            {
                currentTime += Time.deltaTime;
                yield return null;
            }
        }

        yield break;
    }

    public void exitYes()
    {
        Application.Quit();
    }

    public void exitNo()
    {
        ExitPanel.SetActive(false);
    }

    public void PredictionsApiCall(string url)
    {
        StartCoroutine(Get_predictions(url));
    }

    public PredictionsResponse predi;

    public IEnumerator Get_predictions(string url)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.SetRequestHeader("X-RapidAPI-Key", RapidAPI_Key);
            webRequest.SetRequestHeader("X-RapidAPI-Host", RapidAPI_Host);

            yield return webRequest.SendWebRequest();
            //yield return WaitForSeconds(.2f);
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(webRequest.error);
            }
            else
            {
                string jsonResponse = webRequest.downloadHandler.text;

                predi = JsonUtility.FromJson<PredictionsResponse>(jsonResponse);
                Home_Predictions.text = "Wins " + predi.response[0].predictions.percent.home;
                Draw_Predictions.text = "Draw " + predi.response[0].predictions.percent.draw;
                Away_Predictions.text = "Away " + predi.response[0].predictions.percent.away;

                var set_Time_1 = predi.response[0].predictions.percent.home.Split('%');
                var set_Time_2 = predi.response[0].predictions.percent.draw.Split('%');
                var set_Time_3 = predi.response[0].predictions.percent.away.Split('%');

                float _a = float.Parse(set_Time_1[0]);
                float _b = float.Parse(set_Time_2[0]);
                float _c = float.Parse(set_Time_3[0]);

                DOTween.To(() => Home_Wins.fillAmount, x => Home_Wins.fillAmount = x, (_a / 100f), 1);
                DOTween.To(() => Draw_Wins.fillAmount, x => Draw_Wins.fillAmount = x, (_b / 100f), 1);
                DOTween.To(() => Away_Wins.fillAmount, x => Away_Wins.fillAmount = x, (_c / 100f), 1);

                var Home_Overall_form = predi.response[0].teams.home.last_5.form.Split('%');
                var Home_Att_form = predi.response[0].teams.home.last_5.att.Split('%');
                var Home_Def_form = predi.response[0].teams.home.last_5.def.Split('%');

                var Away_Overall_form = predi.response[0].teams.away.last_5.form.Split('%');
                var Away_Att_form = predi.response[0].teams.away.last_5.att.Split('%');
                var Away_Def_form = predi.response[0].teams.away.last_5.def.Split('%');

                DOTween.To(() => HomeMonentum.fillAmount, x => HomeMonentum.fillAmount = x, (float.Parse(Home_Overall_form[0]) / 100f), 1);
                DOTween.To(() => AwayMonentum.fillAmount, x => AwayMonentum.fillAmount = x, (float.Parse(Away_Overall_form[0]) / 100f), 1);

                HomeMonentumPin.transform.DOLocalRotate(new Vector3(0, 180, 360 * float.Parse(Home_Overall_form[0]) / 100f), 1);
                AwayMonentumPin.transform.DOLocalRotate(new Vector3(0, 180, 360 * float.Parse(Away_Overall_form[0]) / 100f), 1);


                DOTween.To(() => Att_HomeMonentum.fillAmount, x => Att_HomeMonentum.fillAmount = x, (float.Parse(Home_Att_form[0]) / 100f), 1);
                DOTween.To(() => Att_AwayMonentum.fillAmount, x => Att_AwayMonentum.fillAmount = x, (float.Parse(Away_Att_form[0]) / 100f), 1);

                Att_HomeMonentumPin.transform.DOLocalRotate(new Vector3(0, 180, 360 * float.Parse(Home_Att_form[0]) / 100f), 1);
                Att_AwayMonentumPin.transform.DOLocalRotate(new Vector3(0, 180, 360 * float.Parse(Away_Att_form[0]) / 100f), 1);


                DOTween.To(() => Def_HomeMonentum.fillAmount, x => Def_HomeMonentum.fillAmount = x, (float.Parse(Home_Def_form[0]) / 100f), 1);
                DOTween.To(() => Def_AwayMonentum.fillAmount, x => Def_AwayMonentum.fillAmount = x, (float.Parse(Away_Def_form[0]) / 100f), 1);

                Def_HomeMonentumPin.transform.DOLocalRotate(new Vector3(0, 180, 360 * float.Parse(Home_Def_form[0]) / 100f), 1);
                Def_AwayMonentumPin.transform.DOLocalRotate(new Vector3(0, 180, 360 * float.Parse(Away_Def_form[0]) / 100f), 1);


                Team1ShrinkTotalScore.text = predi.response[0].teams.home.league.goals.@for.total.total.ToString();
                Team2ShrinkTotalScore.text = predi.response[0].teams.away.league.goals.@for.total.total.ToString();


                Team1HomeScore.text = predi.response[0].teams.home.league.goals.@for.total.home.ToString();
                Team1AwayScore.text = predi.response[0].teams.home.league.goals.@for.total.away.ToString();
                Team1TotalScore.text = predi.response[0].teams.home.league.goals.@for.total.total.ToString();

                Team2HomeScore.text = predi.response[0].teams.away.league.goals.@for.total.home.ToString();
                Team2AwayScore.text = predi.response[0].teams.away.league.goals.@for.total.away.ToString();
                Team2TotalScore.text = predi.response[0].teams.away.league.goals.@for.total.total.ToString();


                Team1ShrinkTotalAverage.text = predi.response[0].teams.home.league.goals.@for.average.total.ToString();
                Team2ShrinkTotalAverage.text = predi.response[0].teams.away.league.goals.@for.average.total.ToString();


                Team1HomeAverage.text = predi.response[0].teams.home.league.goals.@for.average.home.ToString();
                Team1AwayAverage.text = predi.response[0].teams.home.league.goals.@for.average.away.ToString();
                Team1TotalAverage.text = predi.response[0].teams.home.league.goals.@for.average.total.ToString();

                Team2HomeAverage.text = predi.response[0].teams.away.league.goals.@for.average.home.ToString();
                Team2AwayAverage.text = predi.response[0].teams.away.league.goals.@for.average.away.ToString();
                Team2TotalAverage.text = predi.response[0].teams.away.league.goals.@for.average.total.ToString();

                MostCommonGoalsHome.text = Mathf.Abs(float.Parse(predi.response[0].predictions.goals.home)).ToString();
                MostCommonGoalsAway.text = Mathf.Abs(float.Parse(predi.response[0].predictions.goals.away)).ToString();

                MostConcededHomeShrinkDefence.text = MostConcededHome.text = predi.response[0].teams.home.league.failed_to_score.total.ToString();
                MostConcededsAwayShrinkDefence.text = MostConcededsAway.text = predi.response[0].teams.away.league.failed_to_score.total.ToString();

                MostConcededsTeam1homeDefence.text = predi.response[0].teams.home.league.failed_to_score.home.ToString();
                MostConcededsTeam1awatDefence.text = predi.response[0].teams.home.league.failed_to_score.away.ToString();
                MostConcededsTeam1TotalDefence.text = predi.response[0].teams.home.league.failed_to_score.total.ToString();

                MostConcededsTeam2homeDefence.text = predi.response[0].teams.away.league.failed_to_score.home.ToString();
                MostConcededsTeam2awatDefence.text = predi.response[0].teams.away.league.failed_to_score.away.ToString();
                MostConcededsTeam2TotalDefence.text = predi.response[0].teams.away.league.failed_to_score.total.ToString();


                Team1ShrinkTotalScoreAttack.text = predi.response[0].teams.home.league.goals.@for.total.total.ToString();
                Team2ShrinkTotalScoreAttack.text = predi.response[0].teams.away.league.goals.@for.total.total.ToString();

                Team1HomeScoreAttack.text = predi.response[0].teams.home.league.goals.@for.total.home.ToString();
                Team1AwayScoreAttack.text = predi.response[0].teams.home.league.goals.@for.total.away.ToString();
                Team1TotalScoreAttack.text = predi.response[0].teams.home.league.goals.@for.total.total.ToString();

                Team2HomeScoreAttack.text = predi.response[0].teams.away.league.goals.@for.total.home.ToString();
                Team2AwayScoreAttack.text = predi.response[0].teams.away.league.goals.@for.total.away.ToString();
                Team2TotalScoreAttack.text = predi.response[0].teams.away.league.goals.@for.total.total.ToString();


                Team1WinStreakShrink.text = predi.response[0].teams.home.league.biggest.streak.wins.ToString();
                Team2WinStreakShrink.text = predi.response[0].teams.away.league.biggest.streak.wins.ToString();

                Team1WinStreak.text = predi.response[0].teams.home.league.biggest.streak.wins.ToString();
                Team1drawStreak.text = predi.response[0].teams.home.league.biggest.streak.draws.ToString();
                Team1LossStreak.text = predi.response[0].teams.home.league.biggest.streak.loses.ToString();

                Team2WinStreak.text = predi.response[0].teams.away.league.biggest.streak.wins.ToString();
                Team2drawStreak.text = predi.response[0].teams.away.league.biggest.streak.draws.ToString();
                Team2LossStreak.text = predi.response[0].teams.away.league.biggest.streak.loses.ToString();


                CleanSheetHomeShrinkDefence.text = predi.response[0].teams.home.league.clean_sheet.total.ToString();
                CleanSheetAwayShrinkDefence.text = predi.response[0].teams.away.league.clean_sheet.total.ToString();

                CleanSheetTeam1homeDefence.text = predi.response[0].teams.home.league.clean_sheet.home.ToString();
                CleanSheetTeam1awatDefence.text = predi.response[0].teams.home.league.clean_sheet.away.ToString();
                CleanSheetTeam1TotalDefence.text = predi.response[0].teams.home.league.clean_sheet.total.ToString();

                CleanSheetTeam2homeDefence.text = predi.response[0].teams.away.league.clean_sheet.home.ToString();
                CleanSheetTeam2awatDefence.text = predi.response[0].teams.away.league.clean_sheet.away.ToString();
                CleanSheetTeam2TotalDefence.text = predi.response[0].teams.away.league.clean_sheet.total.ToString();



                paneltyGoalteam1Shrink.text = predi.response[0].teams.home.league.penalty.scored.total.ToString();
                paneltyGoalteam2Shrink.text = predi.response[0].teams.away.league.penalty.scored.total.ToString();

                paneltyGoalteam1ShrinkPercentage.text= predi.response[0].teams.home.league.penalty.scored.percentage.ToString();
                paneltyGoalteam2ShrinkPercentage.text = predi.response[0].teams.away.league.penalty.scored.percentage.ToString();

                paneltyGoalMissteam1Shrink.text = predi.response[0].teams.home.league.penalty.missed.total.ToString();
                paneltyGoalMissteam2Shrink.text = predi.response[0].teams.away.league.penalty.missed.total.ToString();

                paneltyGoalMissteam1ShrinkPercentage.text = predi.response[0].teams.home.league.penalty.missed.percentage.ToString();
                paneltyGoalMissteam2ShrinkPercentage.text = predi.response[0].teams.away.league.penalty.missed.percentage.ToString();


                string Homeform = predi.response[0].teams.home.league.form;
                string HomeformLast10;

                if (Homeform.Length >= 10)
                {
                    HomeformLast10 = Homeform.Substring(Homeform.Length - 10);
                }
                else
                {
                    HomeformLast10 = "";
                    while (HomeformLast10.Length != 10 - Homeform.Length)
                    {
                        HomeformLast10 += "-";
                    }

                    HomeformLast10 = Homeform;
                }

                List<string> Homearray = new List<string>();
                for (int i = 0; i < HomeformLast10.Length; i++)
                {
                    Homearray.Add(HomeformLast10[i].ToString());
                }


                string Awayform = predi.response[0].teams.away.league.form;
                string AwayformLast10;

                if (Awayform.Length >= 10)
                {
                    AwayformLast10 = Awayform.Substring(Awayform.Length - 10);
                }
                else
                {
                    AwayformLast10 = "";
                    while (AwayformLast10.Length != 10 - Awayform.Length)
                    {
                        AwayformLast10 += "-";
                    }

                    AwayformLast10 += Awayform;
                }

                List<string> Awayarray = new List<string>();
                for (int i = 0; i < AwayformLast10.Length; i++)
                {
                    Awayarray.Add(AwayformLast10[i].ToString());
                }

                if (Homearray.Count > 3)
                {
                    int j = 0;
                    for (int i = Homearray.Count - 3; i < Homearray.Count; i++)
                    {
                        ShrinkFormTeamData[j].gameObject.SetActive(true);

                        if (Homearray[i] == "W")
                        {
                            ShrinkFormTeamData[j].sprite = WinSprite;
                        }
                        else if (Homearray[i] == "D")
                        {
                            ShrinkFormTeamData[j].sprite = DrawSprite;
                        }
                        else if (Homearray[i] == "L")
                        {
                            ShrinkFormTeamData[j].sprite = LossSprite;
                        }
                        else
                        {
                            ShrinkFormTeamData[j].gameObject.SetActive(false);
                        }

                        j++;
                    }
                }

                if (Awayarray.Count > 3)
                {
                    int j = 0;
                    for (int i = Awayarray.Count - 3; i < Awayarray.Count; i++)
                    {
                        ShrinkFormTeam2Data[j].gameObject.SetActive(true);

                        if (Awayarray[i] == "W")
                        {
                            ShrinkFormTeam2Data[j].sprite = WinSprite;
                        }
                        else if (Awayarray[i] == "D")
                        {
                            ShrinkFormTeam2Data[j].sprite = DrawSprite;
                        }
                        else if (Awayarray[i] == "L")
                        {
                            ShrinkFormTeam2Data[j].sprite = LossSprite;
                        }
                        else
                        {
                            ShrinkFormTeam2Data[j].gameObject.SetActive(false);
                        }

                        j++;
                    }
                }
               
                if (Homearray.Count >= 10)
                {
                    for (int i = 0; i < ExpandFormTeamData.Length; i++)
                    {
                        ExpandFormTeamData[i].gameObject.SetActive(true);

                        if (Homearray[i] == "W")
                        {
                            ExpandFormTeamData[i].sprite = WinSprite;
                        }
                        else if (Homearray[i] == "D")
                        {
                            ExpandFormTeamData[i].sprite = DrawSprite;
                        }
                        else if (Homearray[i] == "L")
                        {
                            ExpandFormTeamData[i].sprite = LossSprite;
                        }
                        else
                        {
                            ExpandFormTeamData[i].gameObject.SetActive(false);
                        }
                    }
                }
              
                if (Awayarray.Count >= 10)
                {
                    for (int i = 0; i < ExpandFormTeam2Data.Length; i++)
                    {
                        ExpandFormTeam2Data[i].gameObject.SetActive(true);

                        if (Awayarray[i] == "W")
                        {
                            ExpandFormTeam2Data[i].sprite = WinSprite;
                        }
                        else if (Awayarray[i] == "D")
                        {
                            ExpandFormTeam2Data[i].sprite = DrawSprite;
                        }
                        else if (Awayarray[i] == "L")
                        {
                            ExpandFormTeam2Data[i].sprite = LossSprite;
                        }
                        else
                        {
                            ExpandFormTeam2Data[i].gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }


    public void OverAllButton()
    {
        ObjectsParent.DOLocalMoveX(0, 0.5f);
        ObjectsBottomLine.DOLocalMoveX(-321.3551f, 0.5f);
        //

        //overAllButton.GetComponent<Image>().sprite = selectedSprite;
        //overAllButton.GetComponentInChildren<Text>().color = Color.black;

        //attackButton.GetComponent<Image>().sprite = normalSprite;
        //attackButton.GetComponentInChildren<Text>().color = Color.white;

        //defenseButton.GetComponent<Image>().sprite = normalSprite;
        //defenseButton.GetComponentInChildren<Text>().color = Color.white;
    }

    public void AttackButton()
    {
        ObjectsParent.DOLocalMoveX(-1280, 0.5f);
        ObjectsBottomLine.DOLocalMoveX(0, 0.5f);

        //overAllButton.GetComponent<Image>().sprite = normalSprite;
        //overAllButton.GetComponentInChildren<Text>().color = Color.white;

        //attackButton.GetComponent<Image>().sprite = selectedSprite;
        //attackButton.GetComponentInChildren<Text>().color = Color.black;

        //defenseButton.GetComponent<Image>().sprite = normalSprite;
        //defenseButton.GetComponentInChildren<Text>().color = Color.white;
    }

    public void DefenceButton()
    {
        ObjectsParent.DOLocalMoveX(-2560, 0.5f);
        ObjectsBottomLine.DOLocalMoveX(333.65f, 0.5f);
        //

        //overAllButton.GetComponent<Image>().sprite = normalSprite;
        //overAllButton.GetComponentInChildren<Text>().color = Color.white;

        //attackButton.GetComponent<Image>().sprite = normalSprite;
        //attackButton.GetComponentInChildren<Text>().color = Color.white;

        //defenseButton.GetComponent<Image>().sprite = selectedSprite;
        //defenseButton.GetComponentInChildren<Text>().color = Color.black;
    }

    public void MatchesBtn()
    {
        MainObjectsParent.DOLocalMoveX(0f, 0.5f);
        LeagueObjectsParent.DOLocalMoveX(0, 0.1f);
        MainObjectsBottomLine.DOLocalMoveX(-270, 0.5f);
    }

    public void LeagueBtn()
    {
        MainObjectsParent.DOLocalMoveX(-1280, 0.5f);
        MainObjectsBottomLine.DOLocalMoveX(270, 0.5f);
    }

    public void RegularSeasonBtn()
    {
        LeagueObjectsParent.DOLocalMoveX(0f, 0.5f);
        //LeagueObjectsBottomLine.DOLocalMoveX(-405, 0.5f);

        regularSeasonBtn.GetComponent<Image>().sprite = selectedSprite;
        regularSeasonBtn.GetComponentInChildren<Text>().color = new Color32(254, 203, 39, 255);

        championshipRoundBtn.GetComponent<Image>().sprite = normalSprite;
        championshipRoundBtn.GetComponentInChildren<Text>().color = Color.black;

        conferenceLeagueBtn.GetComponent<Image>().sprite = normalSprite;
        conferenceLeagueBtn.GetComponentInChildren<Text>().color = Color.black;

        relegationRoundBtn.GetComponent<Image>().sprite = normalSprite;
        relegationRoundBtn.GetComponentInChildren<Text>().color = Color.black;
    }

    public void ChampionshipRoundBtn()
    {
        LeagueObjectsParent.DOLocalMoveX(-1280, 0.5f);
        //LeagueObjectsBottomLine.DOLocalMoveX(-135, 0.5f);

        regularSeasonBtn.GetComponent<Image>().sprite = normalSprite;
        regularSeasonBtn.GetComponentInChildren<Text>().color = Color.black;

        championshipRoundBtn.GetComponent<Image>().sprite = selectedSprite;
        championshipRoundBtn.GetComponentInChildren<Text>().color = new Color32(254, 203, 39, 255);

        conferenceLeagueBtn.GetComponent<Image>().sprite = normalSprite;
        conferenceLeagueBtn.GetComponentInChildren<Text>().color = Color.black;

        relegationRoundBtn.GetComponent<Image>().sprite = normalSprite;
        relegationRoundBtn.GetComponentInChildren<Text>().color = Color.black;
    }

    public void ConferenceLeaguePlayoffBtn()
    {
        LeagueObjectsParent.DOLocalMoveX(-2560, 0.5f);
        //LeagueObjectsBottomLine.DOLocalMoveX(135, 0.5f);

        regularSeasonBtn.GetComponent<Image>().sprite = normalSprite;
        regularSeasonBtn.GetComponentInChildren<Text>().color = Color.black;

        championshipRoundBtn.GetComponent<Image>().sprite = normalSprite;
        championshipRoundBtn.GetComponentInChildren<Text>().color = Color.black;

        conferenceLeagueBtn.GetComponent<Image>().sprite = selectedSprite;
        conferenceLeagueBtn.GetComponentInChildren<Text>().color = new Color32(254, 203, 39, 255);

        relegationRoundBtn.GetComponent<Image>().sprite = normalSprite;
        relegationRoundBtn.GetComponentInChildren<Text>().color = Color.black;
    }

    public void RelegationRoundBtn()
    {
        LeagueObjectsParent.DOLocalMoveX(-3840, 0.5f);
        //LeagueObjectsBottomLine.DOLocalMoveX(405, 0.5f);

        regularSeasonBtn.GetComponent<Image>().sprite = normalSprite;
        regularSeasonBtn.GetComponentInChildren<Text>().color = Color.black;

        championshipRoundBtn.GetComponent<Image>().sprite = normalSprite;
        championshipRoundBtn.GetComponentInChildren<Text>().color = Color.black;

        conferenceLeagueBtn.GetComponent<Image>().sprite = normalSprite;
        conferenceLeagueBtn.GetComponentInChildren<Text>().color = Color.black;

        relegationRoundBtn.GetComponent<Image>().sprite = selectedSprite;
        relegationRoundBtn.GetComponentInChildren<Text>().color = new Color32(254, 203, 39, 255);
    }

    [System.Serializable]
    public class PredictionsResponse
    {
        public string get;
        public int results;
        public Response[] response;
    }

    [System.Serializable]
    public class Response
    {
        public Predictions predictions;
        public Leagues league;
        public Teams teams;
        //public Comparison comparison;
        //public List<H2h> h2h;
    }


    [System.Serializable]
    public class Predictions
    {
        public Percent percent;
        public Goalss goals;

    }

    [System.Serializable]
    public class Goalss
    {
        public string home;
        public string away;        
    }

    [System.Serializable]
    public class Percent
    {
        public string home;
        public string draw;
        public string away;
    }

    [System.Serializable]
    public class Leagues
    {
        public int id;
        public string name;
        public string country;
        public string logo;
        public string flag;
        public int season;

    }

    [System.Serializable]
    public class Teams
    {
        public Home home;
        public Away away;
    }

    [System.Serializable]
    public class Home
    {
        public int id;
        public string name;
        public string logo;
        public Last5 last_5;
        public League league;
        public bool winner;
    }

    [System.Serializable]
    public class Away
    {
        public int id;
        public string name;
        public string logo;
        public Last5 last_5;
        public League league;
        public bool winner;
    }
    [System.Serializable]
    public class Last5
    {
        public int played;
        public string form;
        public string att ;
        public string def ;
        public Goals goals ;
    }

    [System.Serializable]
    public class League
    {
        public string form ;
        public Fixtures fixtures ;
        public Goals goals ;
        public Biggest biggest ;
        public CleanSheet clean_sheet ;
        public FailedToScore failed_to_score ;
        public Penalty penalty ;
        public List<object> lineups ;      
        public string round ;
    }


    [System.Serializable]
    public class Fixtures
    {
        public Played played ;
        public Wins wins ;
        public Draws draws ;
        public Loses loses ;
    }
    [System.Serializable]
    public class Played
    {
        public int home ;
        public int away ;
        public int total ;
    }

    [System.Serializable]
    public class Wins
    {
        public int home ;
        public int away ;
        public int total ;
    }

    [System.Serializable]
    public class Draws
    {
        public int home ;
        public int away ;
        public int total ;
    }


    [System.Serializable]
    public class Loses
    {
        public int home ;
        public int away ;
        public int total ;
    }

    [System.Serializable]
    public class Goals
    {
        public string home ;
        public string away ;
        public For @for ;
        public Against against ;
    }

    [System.Serializable]
    public class For
    {
        public TotalGoal total ;
        public average average ;
        public int home ;
        public int away ;
    }

    [System.Serializable]
    public class average
    {
        public string home;
        public string away;
        public string total;
    }

    [System.Serializable]
    public class TotalGoal
    {
        public int home;        
        public int away;
        public int total;
    }

    [System.Serializable]
    public class Against
    {
        public int total ;
        public string average ;      
        public int home ;
        public int away ;
    }

    [System.Serializable]
    public class Biggest
    {
        public Streak streak ;
        public Wins wins ;
        public Loses loses ;
        public Goals goals ;
    }

    [System.Serializable]
    public class Streak
    {
        public int wins ;
        public int draws ;
        public int loses ;
    }

    [System.Serializable]
    public class CleanSheet
    {
        public int home ;
        public int away ;
        public int total ;
    }

    [System.Serializable]
    public class FailedToScore
    {
        public int home ;
        public int away ;
        public int total ;
    }

    [System.Serializable]
    public class Penalty
    {
        public Scored scored ;
        public Missed missed ;
        public int total ;
        public object home ;
        public object away ;
    }

    [System.Serializable]
    public class Scored
    {
        public int total ;
        public string percentage ;
    }

    [System.Serializable]
    public class Missed
    {
        public int total ;
        public string percentage ;
    }


















}
