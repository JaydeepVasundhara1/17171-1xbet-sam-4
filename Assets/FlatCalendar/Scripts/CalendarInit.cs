using UnityEngine;
using static FlatCalendar2;
using UnityEngine.UI;

public class CalendarInit : MonoBehaviour
{
    //Ref to the Calendar prefab
    public static CalendarInit inst;
    public FlatCalendar2 calendar;

    public GameObject Year_Obj;
    public GameObject Month_Obj;
    public GameObject Date_Obj;

    private void Awake()
    {
        inst = this;
    }
    void Start()
    {
        //Set the event callback 
        calendar.setCallback_OnTriggerEvent(Notify);

        //init the calendar (important)
        calendar.initFlatCalendar();


    }

    //Method called when an event occurs
    public void Notify(EventObj evnt)
    {
        evnt.print();
    }
}
