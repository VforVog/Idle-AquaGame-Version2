using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class IdleTutorialGame : MonoBehaviour
{

    public PlayerData data;
    public EventManager events;
    public PrestigeManager prestige;
    public RebirthManager rebirth;
    public AutomatorManager auto;
    public AchievementManager achievements;
    public UpgradeManager upgrades;
    public PlanetManager planets;
    public OfflineManager offline;
    public Settings settingsManager;
    public SaveSystem nonStaticSaveSytsem;

    
    //Episode 1
    public Text coinText;
    public Text coinsPerSecText;
    public Text clickValueText;

    public CriticalClick crit;
    public GameObject critSpawn;


    //Episode 11
    public Canvas mainMenuGroup;
    public Canvas upgradesGroup;
    public Canvas achievementsGroup;
    public Canvas eventsGroup;
    public Canvas prestigeGroup;
    public Canvas rebirthGroup;
    public Canvas autoGroup;
    public Canvas planetGroup;
    public int tabSwitcher;


    public GameObject settings;

    private double pCost => 25 * Math.Pow(1.07, data.productionUpgrade1Level);
    private double pCost2 => 250 * Math.Pow(1.07, data.productionUpgrade2Level);

    private double clickCost1 => 10 * Math.Pow(1.07, data.clickUpgrade1Level);
    internal double coinsTemp => 0;
    private double clickCost2 => 25 * Math.Pow(1.07, data.clickUpgrade2Level);


    //Episode 17

    public void Start()
    {
        Application.targetFrameRate = 60;

        mainMenuGroup.gameObject.SetActive(true);
        upgradesGroup.gameObject.SetActive(false);
        prestigeGroup.gameObject.SetActive(false);

        tabSwitcher = 0;

        data = SaveSystem.SaveExists("PlayerData") ? SaveSystem.LoadPlayer<PlayerData>("PlayerData") : new PlayerData();
        data.CheckObjects();
        events.StartEvents();
        prestige.StartPrestige();
        auto.StartAutomator();
        TotalCoinsPerSecond();
        offline.LoadOfflineProduction();
        settingsManager.StartSettings();

        Methods.NotationSettings = data.notationType;

    }


    public async void Update()
    { 
        //episode16
        achievements.RunAchievements();  //afto na katevei pio kato (otan teliosoume to tutorial)
        prestige.Run();
        rebirth.Run();
        auto.Run();
        if (upgradesGroup.gameObject.activeSelf) upgrades.RunUpgradesUI();
        upgrades.RunUpgrades();

        

        //Header Text
        coinText.text = "$" + Methods.NotationMethod(data.coins, "F2"); //TODO den xorane stin othoni giafto grafo "C: "
        coinsPerSecText.text = "$" + Methods.NotationMethod(TotalCoinsPerSecond(), "F2") + " coins/s";
       
       if (mainMenuGroup.gameObject.activeSelf) clickValueText.text = "Click\n+" + Methods.NotationMethod(TotalClickValue(), "F2") + " Coins";
        /*
       string BuyMaxFormat(double x)
       {
           return $"Buy Max ({x})";
       } */

        data.coins += TotalCoinsPerSecond() * Time.deltaTime; //TODO afta na pane pano apo ta text updates (otan teliosoume to tutorial)
        data.coinsCollected += TotalCoinsPerSecond() * Time.deltaTime;

        saveTimer += Time.deltaTime;

        if (!(saveTimer >= 15)) return;
        Save();
        data.offlineProgressCheck = true;
        saveTimer = 0;
        
    }

    public async void Save()
    {
        await nonStaticSaveSytsem.AwaitGetUTCTime();
        SaveSystem.SavePlayer(data, "PlayerData");
    }


    public float saveTimer; //arxikopoiite me 0 panta


    //TODO na to kanoume allios, pio dinamika
    public double TotalBoost()
    {
        var temp = prestige.TotalGemBoost();
        temp *= events.eventTokenBoost;
        return temp;
    }

   

    public double TotalCoinsPerSecond()
    {
        double temp = 0;
        temp += data.productionUpgrade1Level;
        temp += data.productionUpgrade2Level * data.productionUpgrade2Power;
        temp *= TotalBoost();
        temp *= Math.Pow(1.1, data.prestigeULevels[1]);
        temp *= planets.EMBoost;
        return temp;
    }

    private double TotalClickValue()
    {
        var temp = data.coinsClickValue;
        temp *= TotalBoost();
        temp *= Math.Pow(1.5, data.prestigeULevels[0]);
        return temp;
    }

    private void GenerateCritText()
    {
        var clone = Instantiate(crit, critSpawn.transform);
        clone.Play();

    }

    public void Click()
    {
        data.coins += TotalClickValue();
        data.coinsCollected += TotalClickValue();
        if (data.critLevels <= 0) return;
        var critNum = new System.Random().Next(1, 1000);
        if (critNum <= 1000 - data.critLevels) return;
        data.coins += TotalClickValue() * 100;
        data.coinsCollected += TotalClickValue() * 100;
        GenerateCritText();
    }   
   
 

    //Overide gia na exei os default "F0" diladi na min exei dekadika psifia apo piso (min to sviseis ego to eftiaksa)
    public string NotationMethod(double x)
    {
        if (x > 1000)
        {
            var exponent = 3 * Math.Floor(Math.Floor(Math.Log10(x)) / 3);
            // exponent (ligo allagmena mathimatika apo to tutorial gia na to kanei ana 3 (ana xiliades) kai oxi sinexeia (ana dekades to kanei to tutorial)

            var mantissa = x / Math.Pow(10, exponent);
            // gets the number, ex, 1.32e3, 1.32 is mantissa.

            return mantissa.ToString("F2") + "e" + exponent;
        }
        return x.ToString("F0");
    }

    public void ChangeTabs(string id)
    {
        DisableAll();
        switch (id)
        {
            case "main":
                mainMenuGroup.gameObject.SetActive(true);
                break;
            case "upgrades":
                upgradesGroup.gameObject.SetActive(true);
                break;
            case "achievements":              
                achievementsGroup.gameObject.SetActive(true);
                break;
            case "events":                    
                eventsGroup.gameObject.SetActive(true);
                break;
            case "prestige":                    
                prestigeGroup.gameObject.SetActive(true);
                break;
            case "rebirth":                    
                rebirthGroup.gameObject.SetActive(true);
                break;
            case "auto":                    
                autoGroup.gameObject.SetActive(true);
                break;
            case "planets":                    
                planetGroup.gameObject.SetActive(true);
                break;
            default:
                Debug.Log("error happenend in ChangeTabs()");
                break;
        }

        void DisableAll()
        {
            mainMenuGroup.gameObject.SetActive(false);
            upgradesGroup.gameObject.SetActive(false);
            achievementsGroup.gameObject.SetActive(false);
            eventsGroup.gameObject.SetActive(false);
            prestigeGroup.gameObject.SetActive(false);
            rebirthGroup.gameObject.SetActive(false);
            autoGroup.gameObject.SetActive(false);
            planetGroup.gameObject.SetActive(false);
        }
    }

    //TODO to kanei ligo bakalistika, tha doume pos tha to kanoume
    public void GoToSettings()
    {
        settings.gameObject.SetActive(true);
    } 
    
    public void GoBackFromSettings()
    {
        settings.gameObject.SetActive(false);
    }

    public void FullReset()
    {
        data = new PlayerData();
        ChangeTabs("main");
    }

    

}

