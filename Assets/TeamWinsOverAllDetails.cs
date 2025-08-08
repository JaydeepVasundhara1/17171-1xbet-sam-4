using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamWinsOverAllDetails : MonoBehaviour
{
    public static TeamWinsOverAllDetails instance { get; set; }
    public Text HomeTeamName;
    public Text AwayTeamName;
    public Text MatchTime;
    public Text Result_text;
    public Image Home_Logo;
    public Image Away_Logo;

    public GameObject Match_object;
    public GameObject Result_object;


    private void Awake()
    {
        instance = this;
    }

   
}
