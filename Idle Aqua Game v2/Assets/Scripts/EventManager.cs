using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class EventManager : MonoBehaviour
{
    public IdleTutorialGame game;
    public Text eventTokensText;

    public double eventTokenBoost => (game.data.eventTokens * 0.10) + 1;    //10% boost ana token

    public GameObject eventRewardPopUp;
    public Text eventRewardText;
    public GameObject[] events = new GameObject[7];
    public GameObject[] eventsUnlocked = new GameObject[7];
    public Text[] rewardText = new Text[7];
    public Text[] currencyText = new Text[7];
    public Text[] costText = new Text[7];
    public Text[] startText = new Text[7];
    public double[] reward = new double[7];
    public double[] currencies = new double[7];
    public double[] costs = new double[7];
    public int[] levels = new int[7];
    public bool eventActive;

    private string DayOfTheWeek()
    {
        var dt = new DateTime(/* DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day */ 2020, 9, 1);   //TODO Kanonika xrisimopoioumai afto sta sxolia, allios exo grapsei karfota mera pou einai triti = "Tuesday"
        return dt.DayOfWeek.ToString();
    }

    public string previousDayChecked;
    public string today;

    public void StartEvents()
    {
        eventActive = false;
        previousDayChecked = DayOfTheWeek();
        if (game.data.eventActiveID != 0)
        {
            game.data.eventActiveID = 0;
            game.data.eventCooldown = new float[7];
        }
    }

    public void Update()    //TODO polla pragmata mesa stin update, na elenxtoun sto telos tou tutorial
    {
        var data = game.data;

        eventTokensText.text = $"Event Tokens: {Methods.NotationMethod(data.eventTokens, "F2")} ({Methods.NotationMethod(eventTokenBoost, "F2")}x)";

        reward[0] = Math.Log10(currencies[0] + 1);
        reward[1] = Math.Log10(currencies[1] / 5 + 1);

        for (var i = 0; i < 2; i++)
            costs[i] = 10 * Math.Pow(1.15, levels[i]);

        today = DayOfTheWeek();
        if (previousDayChecked != today & eventActive)
        {
            data.eventActiveID = 0;
            for (int i = 0; i < 2; i++) // if more events, then "2" -> "3" 
            {     
                data.eventCooldown[i] = 0;
            }
        }

        switch (today)
        {
            case "Monday":
                if (game.eventsGroup.gameObject.activeSelf) 
                {
                    RunEventUI(0);
                }
                RunEvent(0);
                break;
            case "Tuesday":
                if (game.eventsGroup.gameObject.activeSelf) 
                {
                    RunEventUI(1);
                }
                RunEvent(1);
                break;
        }
        var currentDay = CurrentDay();  //to dilono giati mesa se if xaneis xrono na trexeis ksana kai ksana mia synartisi
        if (game.data.eventCooldown[currentDay] > 0)    //an yparxei cooldown simainei i oti paizei i oti exei cooldown
        {
            game.data.eventCooldown[currentDay] -= Time.deltaTime;
        }        
        else if (data.eventActiveID != 0 & game.data.eventCooldown[currentDay] <= 0)    //an den exei cooldown tote --> an epaize simenei oti teliose, allios tipota
        {
            CompleteEvent(currentDay);
        }

        previousDayChecked = DayOfTheWeek();
    }

    public int CurrentDay() //TODO allages na min yparxei CurrentDay kai DayOfTheWeek ksexorista mias kai kanoun to idio apla to ena string kai to allo int
    {
        switch (DayOfTheWeek())
        {
            case "Monday": return 0;
            case "Tuesday": return 1;
        }
        return -1;  //TODO edo to ekana -1 gia na mas vgazei eror gia na katalavainoume oti exei paei se mera pou den exoume orisei
    }

    public void Click(int id)
    {
        switch (id)
        {
            case 0:
                currencies[id] += 1 + levels[id];
                break;

            case 1:
                currencies[id] += 1;
                break;
        }
    }

    public void Buy(int id)
    {
        if (currencies[id] > costs[id])
        {
            currencies[id] -= costs[id];
            levels[id]++;
        };        
    }

    public void ToggleEvent(int id)
    {
        var id2 = id - 1;   //TODO edo aftos to exei lousei giati allou to event to onomazei id 1 gia monday kai allou id 0, afto thelei ftiaksimo
        var data = game.data;
        DateTime now = DateTime.Now;

        // Start if no other event is currently running AND you have no cooldown on the current event AND it's before 23:55
        if (data.eventActiveID == 0 & data.eventCooldown[id2] <= 0 & !(now.Hour == 23 & now.Minute >= 55)) 
        {
            data.eventActiveID = id;
            data.eventCooldown[id2] = 300;

            currencies[id2] = 0;
            levels[id2] = 0;
        }
        else if (data.eventActiveID != 0 & data.eventCooldown[id2] > 0)    //Exit event if you have an active event AND you still have time (cooldown)
        {
            CompleteEvent(id2);
        }
    }

    private void CompleteEvent(int id)
    {
        var data = game.data;

        data.eventTokens += reward[id];
        eventRewardText.text = $"+{Methods.NotationMethod(reward[id], "F2")} Tokens";

        currencies[id] = 0;
        levels[id] = 0;
        data.eventActiveID = 0;
        data.eventCooldown[id] = 7200;

        eventRewardPopUp.gameObject.SetActive(true);
    }

    public void CloseEventReward()
    {
        eventRewardPopUp.gameObject.SetActive(false);
    }

    private void RunEventUI(int id)
    {
        var data = game.data;

        for (var i = 0; i < 2; i++)
            if (i == id)
            {
                events[id].gameObject.SetActive(true);
            }
            else
            {
                events[i].gameObject.SetActive(false);
            }


        var time = TimeSpan.FromSeconds(data.eventCooldown[id]);
        if (data.eventActiveID == 0)
        {
            startText[id].text = data.eventCooldown[id] > 0 ? time.ToString(@"hh\:mm\:ss") : "Start Event";
        }
        else
        {
            startText[id].text = $"Exit Event ({time:hh\\:mm\\:ss})";
        }

        if (data.eventActiveID != id + 1) return;
        {
            eventsUnlocked[id].gameObject.SetActive(true);
            rewardText[id].text = $"+{Methods.NotationMethod(reward[id], "F2")} Tokens";
            currencyText[id].text = $"{Methods.NotationMethod(currencies[id], "F2")} Event-Coins";
            costText[id].text = $"Cost: {Methods.NotationMethod(costs[id], "F2")}";
        }
    }

    private void RunEvent(int id)
    {
        switch (id)
        {
            case 1:
                currencies[id] += levels[id] * Time.deltaTime;
                break;
        }
    }
}
