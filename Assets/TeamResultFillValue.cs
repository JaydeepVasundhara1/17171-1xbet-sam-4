using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamResultFillValue : MonoBehaviour
{
    public static TeamResultFillValue instance { get; set; }
    public Image Home_Wins;
    public Image Draw_Wins;
    public Image Away_Wins;
    public Text Home_WinsInt;
    public Text Draw_WinsInt;
    public Text Away_WinsInt;
    public string Home_Text;
    public string Drew_Text;
    public string Away_Text;


    private void Awake()
    {
        instance = this;
    }

}
