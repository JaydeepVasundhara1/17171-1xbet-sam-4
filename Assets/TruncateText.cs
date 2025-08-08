using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TruncateText : MonoBehaviour
{
    private int maxLength = 15;

    // Start is called before the first frame update
    void Start()
    {
        string text = GetComponent<Text>().text;
        if (text.Length > maxLength + 3)
        {
            GetComponent<Text>().text = text.Substring(0, maxLength) + "...";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
