using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeagueInValue : MonoBehaviour
{
    public static LeagueInValue instance { get; set; }
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



    private void Awake()
    {
        instance = this;
    }

    public void SetupDataFromApiReceived(int Num_Rank, string LogoUrl, string Name, int MP, int W, int D, int L, int Pts)
    {
        _Num_Rank.text = Num_Rank.ToString();
        Davinci.get().load(LogoUrl).into(_Logo).start();
        _Name.text = Name;
        _MP.text = MP.ToString();
        _W.text = W.ToString();
        _D.text = D.ToString();
        _L.text = L.ToString();
        _PTs.text = Pts.ToString();


    }
}
