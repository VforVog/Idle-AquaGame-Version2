using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class PlayerData
{
    public double coins;
    public double coinsCollected;
    public double coinsClickValue;
    public double coinsPerSecond;
    public int clickUpgrade1Level;
    public int productionUpgrade1Level;
    public double clickUpgrade2Level;
    public double productionUpgrade2Power;
    public int productionUpgrade2Level;
    public double gems;
    public double gemsBoost;
    public double gemsToGet;

    public int achLevel1;
    public int achLevel2;

    //Events
    public double eventTokens;
    public float[] eventCooldown = new float[7];
    public int eventActiveID;

    #region Prestige
    public int prestigeLevel1;
    public int prestigeLevel2;
    public int prestigeLevel3;
    #endregion

    #region Automators
    public int autolevel1;
    public int autolevel2;
    #endregion

    #region Rebirth
    public double souls;
    #endregion

    public void FullReset()
    {
        coins = 0;
        coinsCollected = 0;
        coinsClickValue = 1;
        productionUpgrade2Power = 5;

        clickUpgrade1Level = 0;
        clickUpgrade2Level = 0;
        productionUpgrade1Level = 0;
        productionUpgrade2Level = 0;
        gems = 0;

        achLevel1 = 0;
        achLevel2 = 0;

        eventTokens = 0;

        prestigeLevel1 = 0;
        prestigeLevel2 = 0;
        prestigeLevel3 = 0;

        var howManyEvents = eventCooldown.Length - 1;   //to kano etsi giati einai pio grigoro (allios kanei afti tin praksi mesa stin if ksana kai ksana)
        for (int i = 0; i < howManyEvents - 1; i++)
        {
            eventCooldown[i] = 0;
        }
        eventActiveID = 0;

        souls = 0;

        #region Autos
          autolevel1 = 0;
          autolevel2 = 0;
        #endregion
}
}

public class IdleTutorialGame : MonoBehaviour
{

    public PlayerData data;
    public EventManager events;
    public PrestigeManager prestige;
    public RebirthManager rebirth;
    public AutomatorManager auto;

    //Episode 19
    public GameObject clickUpgrade1;
    public GameObject clickUpgrade2;
    public GameObject productionUpgrade1;
    public GameObject productionUpgrade2;

    //Episode 1
    public Text coinText;
    public Text clickValueText;

    //Episode 2
    public Text coinsPerSecText;
    public Text clickUpgrade1Text;
    public Text clickUpgrade2Text;
    public Text productionUpgrade1Text;
    public Text productionUpgrade2Text;

    //Episode 6
    public Text gemsText;
    public Text gemsBoostText;
    public Text gemsToGetText;

    //Episode 7
    public Image clickUpgrade1Bar;
    public Image clickUpgrade2Bar;
    public Image productionUpgrade1Bar;
    public Image productionUpgrade2Bar;

    //Episode 8
    public Text clickUpgrade1MaxText;
    public Text clickUpgrade2MaxText;
    public Text productionUpgrade1MaxText;
    public Text productionUpgrade2MaxText;

    //Episode 11
    public Canvas mainMenuGroup;
    public Canvas upgradesGroup;
    public Canvas achievementsGroup;
    public Canvas eventsGroup;
    public Canvas prestigeGroup;
    public Canvas rebirthGroup;
    public Canvas autoGroup;




    public GameObject settings;

    //Episoe 16
    public GameObject achievementScreen;
    public List<Achievement> achievementList = new List<Achievement>();

    //Episode 17
    public float saveTimer; //arxikopoiite me 0 panta

    public double clickUpgrade1Cost;
    public double clickUpgrade2Cost;
    public double productionUpgrade1Cost;
    public double productionUpgrade2Cost;

    public double clickUpgrade1StartingCost = 10;
    public double clickUpgrade2StartingCost = 25;
    public double productionUpgrade1StartingCost = 25;
    public double productionUpgrade2StartingCost = 250;


    public void Start()
    {
        Application.targetFrameRate = 60;
        
        //Vriskei ola ta child pou exoun achievement(to script) kai ta prostheti stin lista achievmentList
        foreach (var achievement in achievementScreen.GetComponentsInChildren<Achievement>())
        {
            achievementList.Add(achievement);
        }

        ChangeTabs("main");

        SaveSystem.LoadPlayer(ref data);

        events.StartEvents();
        prestige.StartPrestige();
        auto.StartAutomator();
    }


    public void Update()
    { 
        //episode16
        RunAchievements();  //afto na katevei pio kato (otan teliosoume to tutorial)
        prestige.Run();
        rebirth.Run();
        auto.Run();

        data.gemsToGet = Math.Floor(150 * Math.Sqrt(data.coinsCollected / 1e7) + 1);     //mathimatikos tipos gia balanced prestige gain, kanonika thelei 1e15 oxi 1e7 episeis evala strogilopoiisi pros ta kato
        data.coinsPerSecond = TotalCoinsPerSecond();    //TODO na doume an tha to kratioume etsi, aftos edo to evale pantou kai esvise to data.coinsPerSecond telios

        //Header Text
        coinText.text = "C: " + NotationMethod(data.coins); //TODO den xorane stin othoni giafto grafo "C: "
        coinsPerSecText.text = NotationMethod(data.coinsPerSecond) + " coins/s";
        gemsText.text = "Gems: " + NotationMethod(data.gems);
        gemsBoostText.text = NotationMethod(TotalGemBoost(), "F2") + "x Boost";

        if (mainMenuGroup.gameObject.activeSelf)    //Afta ousiastika kanoun update mono oso o xristeis kathete sto main menu
        {
            gemsToGetText.text = "Prestige: \n+" + NotationMethod(data.gemsToGet) + " Gems";
            clickValueText.text = "Click\n+" + NotationMethod(TotalClickValue()) + " Coins";
        }

        if (upgradesGroup.gameObject.activeSelf)
        {
            //Cost calculations
            clickUpgrade1Cost = clickUpgrade1StartingCost * Math.Pow(1.07, data.clickUpgrade1Level);    //afta ola na ginoun dynamika
            clickUpgrade2Cost = clickUpgrade2StartingCost * Math.Pow(1.07, data.clickUpgrade2Level);    //afta ola na ginoun dynamika
            productionUpgrade1Cost = productionUpgrade1StartingCost * Math.Pow(1.07, data.productionUpgrade1Level);    //afta ola na ginoun dynamika
            productionUpgrade2Cost = productionUpgrade2StartingCost * Math.Pow(1.07, data.productionUpgrade2Level);    //afta ola na ginoun dynamika

            //Click related UI refreshes
            clickUpgrade1.gameObject.SetActive(data.coinsCollected >= clickUpgrade1StartingCost / 2);
            clickUpgrade2.gameObject.SetActive(data.coinsCollected >= clickUpgrade2StartingCost / 2);
            //TODO poli bakalia exei pesei me to totalBoost() kai tous pollaplasiasoums
            clickUpgrade1Text.text = "Click Upgrade 1\nCost: " + NotationMethod(clickUpgrade1Cost) + " coins\nPower " + NotationMethod(TotalBoost() * 1 * Math.Pow(1.5, prestige.levels[0])) + " Click\nLevel: " + data.clickUpgrade1Level;            
            clickUpgrade2Text.text = "Click Upgrade 2\nCost: " + NotationMethod(clickUpgrade2Cost) + " coins\nPower " + NotationMethod(TotalBoost() * 5 * Math.Pow(1.5, prestige.levels[0])) + " Click\nLevel: " + data.clickUpgrade2Level;

            clickUpgrade1MaxText.text = "Buy Max (" + BuyClickUpgrade1MaxCount() + ")";
            clickUpgrade2MaxText.text = "Buy Max (" + BuyClickUpgrade2MaxCount() + ")";

            clickUpgrade1Bar.fillAmount = Methods.SmoothLoadingBar(clickUpgrade1Bar.fillAmount, data.coins, clickUpgrade1Cost);
            clickUpgrade2Bar.fillAmount = Methods.SmoothLoadingBar(clickUpgrade2Bar.fillAmount, data.coins, clickUpgrade2Cost);

            //Production related UI refreshes
            productionUpgrade1.gameObject.SetActive(data.coinsCollected >= productionUpgrade1StartingCost / 2);
            productionUpgrade2.gameObject.SetActive(data.coinsCollected >= productionUpgrade2StartingCost / 2);
            //TODO poli bakalia exei pesei me to totalBoost() kai tous pollaplasiasoums
            productionUpgrade1Text.text = "Production Upgrade 1\nCost: " + NotationMethod(productionUpgrade1Cost) + " coins\nPower +" + NotationMethod(TotalBoost() * Math.Pow(1.1, prestige.levels[1]), "F2") + " Coins/s\nLevel: " + data.productionUpgrade1Level;            
            productionUpgrade2Text.text = "Production Upgrade 2\nCost: " + NotationMethod(productionUpgrade2Cost) + " coins\nPower +" + NotationMethod(data.productionUpgrade2Power * TotalBoost() * Math.Pow(1.1, prestige.levels[1]), "F2") + " Coins/s\nLevel: " + data.productionUpgrade2Level;

            productionUpgrade1MaxText.text = "Buy Max (" + BuyProductionUpgrade1MaxCount() + ")";
            productionUpgrade2MaxText.text = "Buy Max (" + BuyProductionUpgrade2MaxCount() + ")";

            productionUpgrade1Bar.fillAmount = Methods.SmoothLoadingBar(productionUpgrade1Bar.fillAmount, data.coins, productionUpgrade1Cost);
            productionUpgrade2Bar.fillAmount = Methods.SmoothLoadingBar(productionUpgrade2Bar.fillAmount, data.coins, productionUpgrade2Cost);

        }

        data.coins += data.coinsPerSecond * Time.deltaTime; //TODO afta na pane pano apo ta text updates (otan teliosoume to tutorial)
        data.coinsCollected += data.coinsPerSecond * Time.deltaTime;

        saveTimer += Time.deltaTime;
        if (saveTimer >= 5)    //save kathe 5s TODO na doume emeis kathe pote theloume na kanei save
        {
            SaveSystem.SavePlayer(data);
            saveTimer = 0;
        }
    }

    //todo na kanoume na dexete pragmata dinamika
    private static string[] AchievementStrings => new string[] { "Current coins", "Total Coins Collected" };
    private double[] AchievementNumbers => new double[] { data.coins, data.coinsCollected };

    private void RunAchievements()
    {
        //TODO na ta stlenei dinamika me loopa kai oxi na ta grafoume 1-1 me to xeri
        UpdateAchievements(AchievementStrings[0], AchievementNumbers[0], ref data.achLevel1, 
            ref achievementList[0].fill, ref achievementList[0].title, ref achievementList[0].progress);

        UpdateAchievements(AchievementStrings[1], AchievementNumbers[1], ref data.achLevel2,
            ref achievementList[1].fill, ref achievementList[1].title, ref achievementList[1].progress);
    }

    private void UpdateAchievements(string achievName, double currentNumber, ref int achievlevel, ref Image fillBar, ref Text titleText, ref Text progressText)
    {
        double targetNumber = Math.Pow(10, achievlevel);

        if (achievementsGroup.gameObject.activeSelf)    //Afto ousiastika trexei mono otan o xristeis koitaei ta achievements tou
        {
            titleText.text = achievName + "\n(" + achievlevel + ")";
            progressText.text = NotationMethod(currentNumber) + " / " + NotationMethod(targetNumber);

            fillBar.fillAmount = Methods.SmoothLoadingBar(fillBar.fillAmount, currentNumber, targetNumber);
        }

        if (currentNumber >= targetNumber)  //TODO allagi oles aftes oi prakseis oi mises einai axristes
        {
            int levelsGained = 0;
            if (currentNumber / targetNumber >= 1)
            {
                levelsGained = (int)Math.Floor(Math.Log10(currentNumber / targetNumber)) + 1;
            }
            achievlevel += levelsGained;
        }
    }

    //prestige
    public void Prestige()
    {
        //TODO na ginei ligo pio sosto me default values, kai oxi me karfota noumera
        if (data.coinsCollected >  1000)
        {
            PrestigeReset();
            data.gems += data.gemsToGet;
        }
    }

    public void PrestigeReset() //To kano etsi giati borei na xrisimopoiithei kai allou
    {
        data.coins = 0;
        data.coinsCollected = 0;
        data.coinsClickValue = 1;
        data.productionUpgrade2Power = 5;   //TODO na doume ligo poia pragmata xreiazete na ginontai reset

        data.clickUpgrade1Level = 0;
        data.clickUpgrade2Level = 0;
        data.productionUpgrade1Level = 0;
        data.productionUpgrade2Level = 0;
    }

    //TODO na to kanoume allios, pio dinamika
    public double TotalBoost()
    {
        var temp = TotalGemBoost();
        temp *= events.eventTokenBoost;
        return temp;
    }

    public double TotalGemBoost()
    {
        var temp = data.gems;
        temp *= 0.05 + (prestige.levels[2] * 0.01);  //Kathe gem sou dinei 5% boost +1% gia kathe level tou prestige upgrade gia gems
        temp *= rebirth.soulsBoost;
        return temp + 1;
    }

    private double TotalCoinsPerSecond()
    {
        double temp = 0;
        temp += data.productionUpgrade1Level;
        temp += data.productionUpgrade2Level * data.productionUpgrade2Power;
        temp *= TotalBoost();
        temp *= Math.Pow(1.1, prestige.levels[1]);
        return temp;
    }

    private double TotalClickValue()
    {
        var temp = data.coinsClickValue;
        temp *= TotalBoost();
        temp *= Math.Pow(1.5, prestige.levels[0]);
        return temp;
    }


    public void Click()
    {
        data.coins += TotalClickValue();
        data.coinsCollected += TotalClickValue();
    }   
   
    public void BuyClickUpgrade1Max()   //TODO na ginei dinamika to buy MAX, min exoume 100 buyMax sto telos
    {
        var b = clickUpgrade1StartingCost;  // b = arxiki timi upgrade TODO katharismo axriston metavliton
        var c = data.coins;         // c = coins
        var r = 1.07;               // r = rithmos afksisis timis upgrade
        var k = data.clickUpgrade1Level; // k = torino epipedo upgrade
        var n = (int)Math.Floor(Math.Log(c * (r - 1) / (b * Math.Pow(r, k)) + 1, r));    //n = poses fores boreis na kaneis to upgrade afti ti stigmi

        var cost = b * (Math.Pow(r, k) * (Math.Pow(r, n) - 1) / (r - 1));

        if (data.coins >= cost)
        {
            data.clickUpgrade1Level += n;
            data.coins -= cost;
            data.coinsClickValue += n;
        }
    }
    public int BuyClickUpgrade1MaxCount()
    {
        //TODO OOOOOOOLA afta na ginoun dinamika
        var b = clickUpgrade1StartingCost;  // b = arxiki timi upgrade
        var c = data.coins;              // c = coins
        var r = 1.07;               // r = rithmos afksisis timis upgrade
        var k = data.clickUpgrade1Level; // k = torino epipedo upgrade
        var n = (int)Math.Floor(Math.Log(c * (r - 1) / (b * Math.Pow(r, k)) + 1, r));    //n = poses fores boreis na kaneis to upgrade afti ti stigmi
        return n;
    }   

    public void BuyClickUpgrade2Max()
    {
        var b = clickUpgrade2StartingCost;                 // b = arxiki timi upgrade
        var c = data.coins;         // c = coins
        var r = 1.07;               // r = rithmos afksisis timis upgrade
        var k = data.clickUpgrade2Level; // k = torino epipedo upgrade
        var n = (int)Math.Floor(Math.Log(c * (r - 1) / (b * Math.Pow(r, k)) + 1, r));    //n = poses fores boreis na kaneis to upgrade afti ti stigmi

        var cost = b * (Math.Pow(r, k) * (Math.Pow(r, n) - 1) / (r - 1));

        if (data.coins >= cost)
        {
            data.clickUpgrade2Level += n;
            data.coins -= cost;
            data.coinsClickValue += n * 5;
        }
    }
    public int BuyClickUpgrade2MaxCount()
    {
        //TODO OOOOOOOLA afta na ginoun dinamika
        var b = clickUpgrade2StartingCost;                 // b = arxiki timi upgrade
        var c = data.coins;              // c = coins
        var r = 1.07;               // r = rithmos afksisis timis upgrade
        var k = data.clickUpgrade2Level; // k = torino epipedo upgrade
        var n = (int)Math.Floor(Math.Log(c * (r - 1) / (b * Math.Pow(r, k)) + 1, r));    //n = poses fores boreis na kaneis to upgrade afti ti stigmi
        return n;
    }

    public void BuyProductionUpgrade1Max()
    {
        var b = productionUpgrade1StartingCost;                 // b = arxiki timi upgrade
        var c = data.coins;         // c = coins
        var r = 1.07;               // r = rithmos afksisis timis upgrade
        var k = data.productionUpgrade1Level; // k = torino epipedo upgrade
        var n = (int)Math.Floor(Math.Log(c * (r - 1) / (b * Math.Pow(r, k)) + 1, r));    //n = poses fores boreis na kaneis to upgrade afti ti stigmi

        var cost = b * (Math.Pow(r, k) * (Math.Pow(r, n) - 1) / (r - 1));

        if (data.coins >= cost)
        {
            data.productionUpgrade1Level += n;
            data.coins -= cost;
        }
    }
    public int BuyProductionUpgrade1MaxCount()
    {
        //TODO OOOOOOOLA afta na ginoun dinamika
        var b = productionUpgrade1StartingCost;                 // b = arxiki timi upgrade
        var c = data.coins;              // c = coins
        var r = 1.07;               // r = rithmos afksisis timis upgrade
        var k = data.productionUpgrade1Level; // k = torino epipedo upgrade
        var n = (int)Math.Floor(Math.Log(c * (r - 1) / (b * Math.Pow(r, k)) + 1, r));    //n = poses fores boreis na kaneis to upgrade afti ti stigmi
        return n;
    }

    public void BuyProductionUpgrade2Max()
    {
        var b = productionUpgrade2StartingCost;                 // b = arxiki timi upgrade
        var c = data.coins;         // c = coins
        var r = 1.07;               // r = rithmos afksisis timis upgrade
        var k = data.productionUpgrade2Level; // k = torino epipedo upgrade
        var n = (int)Math.Floor(Math.Log(c * (r - 1) / (b * Math.Pow(r, k)) + 1, r));    //n = poses fores boreis na kaneis to upgrade afti ti stigmi

        var cost = b * (Math.Pow(r, k) * (Math.Pow(r, n) - 1) / (r - 1));

        if (data.coins >= cost)
        {
            data.productionUpgrade2Level += n;
            data.coins -= cost;
        }
    }
    public int BuyProductionUpgrade2MaxCount()
    {
        //TODO OOOOOOOLA afta na ginoun dinamika
        var b = productionUpgrade2StartingCost;                 // b = arxiki timi upgrade
        var c = data.coins;              // c = coins
        var r = 1.07;               // r = rithmos afksisis timis upgrade
        var k = data.productionUpgrade2Level; // k = torino epipedo upgrade
        var n = (int)Math.Floor(Math.Log(c * (r - 1) / (b * Math.Pow(r, k)) + 1, r));    //n = poses fores boreis na kaneis to upgrade afti ti stigmi
        return n;
    }

    //Episode 10
    //TODO boroume na ta kanoume me Enum anti gia string
    public void BuyUpgrade(string upgradeID)
    {
        //TODO afta ola mou fenontai ligo bakalistika alla tha doume an tha ta allaksoume
        switch (upgradeID)
        {
            case "C1":
                //TODO OLA DINAMIKA 
                if (data.coins >= clickUpgrade1Cost)
                {
                    data.clickUpgrade1Level++;
                    data.coins -= clickUpgrade1Cost;
                    data.coinsClickValue++;
                }
                break;            
            case "C2":
                if (data.coins >= clickUpgrade2Cost)
                {
                    data.clickUpgrade2Level++;
                    data.coins -= clickUpgrade2Cost;
                    data.coinsClickValue += 5;
                }
                break;
            case "P1":
                if (data.coins >= productionUpgrade1Cost)
                {
                    data.productionUpgrade1Level++;
                    data.coins -= productionUpgrade1Cost;
                }
                break;
            case "P2":
                if (data.coins >= productionUpgrade2Cost)
                {
                    data.productionUpgrade2Level++;
                    data.coins -= productionUpgrade2Cost;
                }
                break;
            case "C1Max":
                BuyClickUpgrade1Max();
                break;
            case "C2Max":
                BuyClickUpgrade2Max();
                break;
            case "P1Max":
                BuyProductionUpgrade1Max();
                break;
            case "P2Max":
                BuyProductionUpgrade2Max();
                break;
            default:
                Debug.Log("Upgrade has not been asigned");
                break;
        }
    }



    //Episode 10
    public string NotationMethod(double x, string format)
    {
        if (x > 1000)
        {
            var exponent = 3 * Math.Floor(Math.Floor(Math.Log10(x)) / 3);
            // exponent (ligo allagmena mathimatika apo to tutorial gia na to kanei ana 3 (ana xiliades) kai oxi sinexeia (ana dekades to kanei to tutorial)

            var mantissa = x / Math.Pow(10, exponent);
            // gets the number, ex, 1.32e3, 1.32 is mantissa.

            return mantissa.ToString("F2") + "e" + exponent;
        }
        return x.ToString(format);
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


    //TODO (Ep 11) afto borei na xreiastei allagi me parapano tabs alla logika tha to allaksei aftos parakato
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
        data.FullReset();
        ChangeTabs("main");
    }

    

}

//TODO to Methods prepei na figei apo edo kai na ginei helperMethod mono tou se diaforetiko script
public class Methods : MonoBehaviour
{
    //TODO afto einai axristo kai den xrisimopoieitai pleon opote delete sto telos tou tutorial
    public static void CanvasGroupChanger(bool appear, CanvasGroup canvasToChange)
    {
        canvasToChange.alpha = appear ? 1 : 0;
        canvasToChange.interactable = appear;
        canvasToChange.blocksRaycasts = appear;
    }

    //Episode 15
    //Prota vazeis to bar.fillAmount pou thes na gemizei smooth, meta to current poso tou gemismatos kai meta to poso pou otan ftasei tha einai max
    public static float SmoothLoadingBar(float currentLoadingBarPosition, double currentNumber, double maxNumber)
    {
        double finalLoadingBarPosition = currentNumber / maxNumber;
        if (finalLoadingBarPosition > 1)
        {
            finalLoadingBarPosition = 1;
        }
        if (currentLoadingBarPosition - 0.01 > finalLoadingBarPosition)
        {
            //an i bara einai toulaxiston 0.01 (1%) pano apoti thaprepe katevenei sto percentage pou thaprepe na einai (giafto -)
            //to noumero 10 einai afto pou allazei ton rithmo pou i bara paei stin pragmatiki tis thesi mikrotero noumer( mexri 1) to kanei pio "apotomo"
            return currentLoadingBarPosition - (float)((currentLoadingBarPosition - finalLoadingBarPosition) / 10 + 0.1 * Time.deltaTime);
        }
        else if (currentLoadingBarPosition < finalLoadingBarPosition)
        {
            //an i bara einai kato apo ekei pou tha eprepe anevenei ekei pou tha eprepe na einai (giafto +)
            return currentLoadingBarPosition + (float)((finalLoadingBarPosition - currentLoadingBarPosition) / 10 + 0.1 * Time.deltaTime);
        }
        else
        {
            return currentLoadingBarPosition;
        }
    }

}
