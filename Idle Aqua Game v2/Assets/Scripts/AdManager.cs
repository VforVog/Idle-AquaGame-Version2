using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using System;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{

    string appID = "ca-app-pub-5861682469694178~6533419536";    //to link einai apo to admob account mou ftiagmeno idika gia aftin tin efarmogi
   // string rewardVideoAdID = "ca-app-pub-3940256099942544/5224354917";    //to link einai apo testing ads

    private RewardedAd rewardGemsAd;

    public int timesFailedToLoad;
    public Button watchAd;
    public GameObject rewardPopUp;

    public void Start()
    {
        MobileAds.Initialize(initStatus => { });

        timesFailedToLoad = 0;
        watchAd.gameObject.SetActive(false);
        rewardPopUp.gameObject.SetActive(false);

        this.CreateAndLoadRewardedAd();
    }


    void CreateAndLoadRewardedAd()
    {
        //TODO na figoun afta an eimaste mono android
        //#if UNITY_ANDROID
            string rewardVideoAdID = "ca-app-pub-3940256099942544/5224354917";  //to link einai apo testing ads
        //#elif UNITY_IPHONE
        //    string rewardVideoAdID = "unexpected_platform";
        //#else
        //    string rewardVideoAdID = "unexpected_platform";
        //#endif

        this.rewardGemsAd = new RewardedAd(rewardVideoAdID);

        this.rewardGemsAd.OnAdLoaded += HandleRewardedAdLoaded;
        this.rewardGemsAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        this.rewardGemsAd.OnAdOpening += HandleRewardedAdOpening;
        this.rewardGemsAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        this.rewardGemsAd.OnUserEarnedReward += HandleUserEarnedReward;
        this.rewardGemsAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardGemsAd.LoadAd(request);
    }

    public void HandleRewardedAdLoaded(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdLoaded event received");
        watchAd.gameObject.SetActive(true);
    }

    public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
    {
        switch (timesFailedToLoad)
        {
            case 0:
                Invoke("CreateAndLoadRewardedAd()", 10);    //i invoke kalei mia sinartisei meta apo X defterolepta
                break;
            case 1:
                Invoke("CreateAndLoadRewardedAd()", 30);
                break;
            case 2:
                Invoke("CreateAndLoadRewardedAd()", 60);
                break;
            case 3:
                Invoke("CreateAndLoadRewardedAd()", 120);
                break;
            default:
                Invoke("CreateAndLoadRewardedAd()", 300);
                break;
        }
        timesFailedToLoad++;
    }

    public void HandleRewardedAdOpening(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdOpening event received");
    }

    public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
    {
        Debug.Log("HandleRewardedAdFailedToShow event received with message: "+ args.Message);
    }

    public void HandleRewardedAdClosed(object sender, EventArgs args)
    {
        Debug.Log("HandleRewardedAdClosed event received");
        this.CreateAndLoadRewardedAd(); //TODO na to elenksoume se android an doulevei
        timesFailedToLoad = 0;
    }

    public void HandleUserEarnedReward(object sender, Reward args)
    {
        string type = args.Type;
        double amount = args.Amount;
        Debug.Log("HandleRewardedAdRewarded event received for "+ amount.ToString() + " " + type);  //TODO na to elenksoume se android an doulevei
    }

    public void WatchAd()
    {
        if (this.rewardGemsAd.IsLoaded())
        {
            this.rewardGemsAd.Show();
            watchAd.gameObject.SetActive(false);
            rewardPopUp.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("what happened?");
        }
    }

    public void CloseRewardPopUp()
    {
        rewardPopUp.gameObject.SetActive(false);
    }

}
