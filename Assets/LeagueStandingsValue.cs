using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeagueStandingsValue : MonoBehaviour
{
    public static LeagueStandingsValue instance { get; set; }
    public Text _Num_Rank;
    public Image _Logo;
    public Text _Name;
    public Text _MP;
    public Text _W;
    public Text _D;
    public Text _L;
    public Text _PTs;

    public Text HomeRank;
    public Text AwayRank;

    private int id;
    private int rank;
    private int points;
    private string teamName;
    private string teamLogoUrl;

    private void Awake()
    {
        instance = this;
    }

    public void SetupDataFromApiReceived(int Num_Rank, string LogoUrl, string Name, int MP, int W, int D, int L, int Pts, int Id)
    {
        _Num_Rank.text = Num_Rank.ToString();
        if (LogoUrl == null)
        {
            _Logo.color = new Color(0f, 0f, 0f, 0f);
        }

        else
        {
            Davinci.get().load(LogoUrl).into(_Logo).start();
        }
        _Name.text = Name;
        _MP.text = MP.ToString();
        _W.text = W.ToString();
        _D.text = D.ToString();
        _L.text = L.ToString();
        _PTs.text = Pts.ToString();
        id = Id;
        rank = Num_Rank;
        points = Pts;
        teamName = Name;
        teamLogoUrl = LogoUrl;
    }

    public void OnClickSet()
    {
        //GameManger.instance.TeamDetailsPanel.SetActive(true);
        TeamDetails.instance.GetTeamDetails("https://v3.football.api-sports.io/teams/statistics?league=" + GetLeague.instance.leagueId + "&team=" + id + "&season=" + GetLeague.instance.currentSeason, rank, points, teamName, teamLogoUrl);
    }
}
