using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarsManager : MonoBehaviour
{
    public IdleTutorialGame game;
    public Text marsCoinsText;
    public Text marsClickText;
    public Text marsCoinsPerSecText;
    public Text marsTaxesText;

    public Canvas upgradesGroup;

    public double taxesExponentFactor => Math.Pow(Math.Log10(game.data.marsCoins + 1) + 1, 0.1);
    public double taxesExponentFactorClick => Math.Pow(Math.Log10(game.data.marsCoins + 1) + 1, 0.75);

    public float[] marsUpgradeMult;
    public double[] marsUpgradeBaseCosts;
    public double[] marsUpgradeCosts;
    public int[] marsUpgradeLevels;
    private string[] upgradeNames;

    public Text[] upgradeText;
    public Image[] upgradeMarsBars = new Image[2];

    private double marsCoinsTemp;
    private double marsCoinsPerSec => Math.Pow(game.data.marsCoins * (marsUpgradeLevels[1] * 0.0001), 1 / taxesExponentFactor);




    private void Start()
    {
        marsUpgradeCosts = new double[2];
        marsUpgradeLevels = new int[2];
        marsUpgradeMult = new float[] {2.5f, 5};
        marsUpgradeBaseCosts =new double[] {10, 100};
        upgradeNames = new string[] {"Click Power +0.01x", "Gain +0.01% of your Mars Coins per second."};
        UpdateCostUI();
    }


    public void Update()
    {
         var data = game.data;

        ArrayManager();
        UI();


        if (marsUpgradeLevels[1] > 0)
            data.marsCoins += marsCoinsPerSec * Time.deltaTime;

        void UI()
        {
            if(upgradesGroup.gameObject.activeSelf) 
            {
                for (var i = 0; i<2; i++)
                {
                upgradeMarsBars[i].fillAmount = Methods.SmoothLoadingBar(upgradeMarsBars[i].fillAmount, data.marsCoins, marsUpgradeCosts[i]);
                }
            }

            if(!game.planets.Mars.gameObject.activeSelf) return;
            marsCoinsText.text = $"{Methods.NotationMethod(data.marsCoins, "F2")} Mars Coins";
            marsClickText.text = $"Click\n+{Math.Pow(clickPower, 1 / taxesExponentFactorClick):F2}x Mars Coins";
            marsCoinsPerSecText.text = $"{Methods.NotationMethod(marsCoinsPerSec, "F2")} Mars Coins/s";
            marsTaxesText.text = $"Taxes:\n{Methods.NotationMethod(taxesMultPerSecond(), "F2")}x less Mars Coins per Second\n{taxesMultPerClick():F2}x less Mars Coins per Click";
        }

        double taxesMultPerSecond()
        {
            if(marsUpgradeLevels[1] == 0) return 1;
            return game.data.marsCoins *marsUpgradeLevels[1] * 0.0001 / marsCoinsPerSec;
        }
        double taxesMultPerClick()
        {
            return clickPower / Math.Pow(clickPower, 1 / taxesExponentFactorClick);
        }
      
    }

    public void ClickPlanet()
    {
        var data = game.data;  
        data.marsCoins *= Math.Pow(clickPower, 1 / taxesExponentFactorClick);
    }

    public float clickPower => 1.01f + 0.01f *  marsUpgradeLevels[0];

    public void BuyUpgrade(int index)
    {
        var data = game.data;
        if(data.marsCoins >= marsUpgradeCosts[index])
        {
            data.marsCoins -= marsUpgradeCosts[index];
            marsUpgradeLevels[index]++;
        }
        NonArrayManager();
        UpdateCostUI();

    }

    public void BuyMax()
    {
        var data = game.data;
        
        Methods.BuyMax(ref data.marsCoins, marsUpgradeBaseCosts[0], marsUpgradeMult[0], ref marsUpgradeLevels[0]);
        Methods.BuyMax(ref data.marsCoins, marsUpgradeBaseCosts[1], marsUpgradeMult[1], ref marsUpgradeLevels[1]);

        NonArrayManager();
        UpdateCostUI();
    } 

    private void UpdateCostUI()
    {
        var data = game.data;
         for (var i = 0; i<2; i++)
                {
                upgradeText[i].text = $"({marsUpgradeLevels[i]}) {upgradeNames[i]}\nCost: {Methods.NotationMethod(marsUpgradeCosts[i], "F2")} Mars Coins";
                upgradeMarsBars[i].fillAmount = Methods.SmoothLoadingBar(upgradeMarsBars[i].fillAmount, data.marsCoins, marsUpgradeCosts[i]);
                }
    }

    public void TogglePopUP(string id)
    {
       
        switch (id)
        {
            case "upgrades":
                upgradesGroup.gameObject.SetActive(!upgradesGroup.gameObject.activeSelf);
                break;           
        }
    }
       /* public void ChangeTabs(string id)
    {
        DisableAll();
        switch (id)
        {
            case "upgrades":
                upgradesGroup.gameObject.SetActive(true);
                break;           
            default:
                Debug.Log("error happenend in ChangeTabs()");
                break;
        }

        void DisableAll()
        {
            //NADA YET
        }
    }
 */
    private void ArrayManager()
    {
        var data = game.data;
        marsUpgradeLevels[0] = data.marsUpgradeLevel1;
        marsUpgradeLevels[1] = data.marsUpgradeLevel2;
        marsUpgradeCosts[0] = marsUpgradeBaseCosts[0] * Math.Pow(marsUpgradeMult[0], marsUpgradeLevels[0]);
        marsUpgradeCosts[1] = marsUpgradeBaseCosts[1] * Math.Pow(marsUpgradeMult[1], marsUpgradeLevels[1]);
    }

     private void NonArrayManager()
    {
        var data = game.data;

        data.marsUpgradeLevel1 = marsUpgradeLevels[0];
        data.marsUpgradeLevel2 = marsUpgradeLevels[1];
    }

}
