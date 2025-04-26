using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class DailyRewardManager : MonoBehaviour
{
    public IdleTutorialGame game;
    public DateTime tempDateTime;
    public float UTCTimer;

   public GameObject dailyReward;
   public GameObject dailyRewardClaimed;
   public Text[] rewardText = new Text[7];
   public Image[] rewardButton = new Image[7];
   public int[] rewardPercents;
   public Text rewardClaimedText;

   public UTCTime utcTime;


   [Serializable]
   public class UTCTime
    {
       public string datetime;
    }

   public void Start()
   {
       StartCoroutine(GetUTCTime());
       rewardPercents = new int[]{1, 2, 3, 4, 5, 10, 25};
   }

   public void Update()
   {
       var data = game.data;
       if (dailyReward.gameObject.activeSelf)
           for (var i = 0; i< 6; i++)
           {
               rewardText[i].text = i < data.currentDay ? "CLAIMED" : $"+{Methods.NotationMethod((data.gems + 100) * ((float)rewardPercents[i] / 100), "F2")} Gems";
               rewardButton[i].color = i < data.currentDay ? Color.green : Color.white;
            }
        
           
               rewardText[6].text = 6 < data.currentDay ? "CLAIMED" : $"+{Methods.NotationMethod((data.gems + 100) * ((float)rewardPercents[6] / 100), "F2")} Gems";
               rewardButton[6].color = 6 < data.currentDay ? Color.green : Color.cyan;
            

       UTCTimer += Time.deltaTime;
       if (UTCTimer < 60) return;
       UTCTimer = 0;
       StartCoroutine(GetUTCTime());
   }

   public void Claim(int id)
   {
       var data = game.data;

       if (data.dailyRewardReady && id <= data.currentDay)
       {
           data.gems += (data.gems + 100) * ((float)rewardPercents[id] / 100);
           data.currentDay++;
           data.dailyRewardReady = false;
           dailyRewardClaimed.gameObject.SetActive(true);
           data.UTCTime = tempDateTime;
           rewardClaimedText.text = $"+{Methods.NotationMethod((data.gems + 100) * ((float)rewardPercents[id] / 100), "F2")} Gems";

       }
   }

   public void CloseRewards()
   {
       dailyRewardClaimed.gameObject.SetActive(false);
       dailyReward.gameObject.SetActive(false);
   }

   public void ToggleRewards()
   {
       dailyReward.gameObject.SetActive(!dailyReward.gameObject.activeSelf);
   }

   public IEnumerator GetUTCTime()
   {
       var data = game.data;

       var request = UnityWebRequest.Get("https://worldtimeapi.org/api/timezone/etc/UTC/");
       yield return request.SendWebRequest();
       if (request.isHttpError || request.isNetworkError) yield break;
       var json = request.downloadHandler.text;
       utcTime = JsonUtility.FromJson<UTCTime>(json);
       tempDateTime = Convert.ToDateTime(utcTime.datetime);

        if((data.UTCTime.Day != tempDateTime.Day || data.UTCTime.Month != tempDateTime.Month || data.UTCTime.Year != tempDateTime.Year) && !data.dailyRewardReady)
       {
           data.dailyRewardReady = true;
           dailyReward.gameObject.SetActive(true);

           if( data.currentDay >= 7)
                data.currentDay = 0;
       }
   }
}
