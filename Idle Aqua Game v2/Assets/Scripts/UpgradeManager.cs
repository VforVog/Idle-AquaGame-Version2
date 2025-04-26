using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public IdleTutorialGame game;
    
    public Canvas upgrade;

    public GameObject[] clickUpgrade = new GameObject[2];
    public GameObject[] productionUpgrade = new GameObject[2];


    public Text OCDText;
    public Text buyModeText;
    

    public Text[] clickUpgradeText = new Text[2];
    public Text[] clickUpgradeMaxText = new Text[2];
     public Text[] productionUpgradeText = new Text[2];
    public Text[] productionUpgradeMaxText = new Text[2];
    public Text criticalChanceText;
    public Text criticalChanceMaxText;


    public Image clickUpgrade1Bar;
    public Image clickUpgrade2Bar;
    public Image productionUpgrade1Bar;
    public Image productionUpgrade2Bar;
    public Image criticalUpgradeBar;


    public int buyMode;

    public double[] clickUpgradeCost;
    public double[] clickUpgradeUnlockCost;
    public double[] clickUpgradePower;
    public int[] clickUpgradeLevels;
    public double[] clickUpgradeMults;
    public double clickUpgrade1Cost;
    public double clickUpgrade2Cost;


    public double[] productionUpgradeCost;
    public double[] productionUpgradeUnlockCost;
    public int[] productionUpgradelevels;
    public double[] clickUpgradeBaseCosts;
    public double[] productionUpgradeBaseCosts;
    public double[] productionUpgradeMults;
    public double productionUpgrade1Cost;
    public double productionUpgrade2Cost;


    public double critCost => 1e3 * Math.Pow(2, game.data.critLevels);


    private void Start()
    {
        clickUpgradeCost = new double[2];
        productionUpgradeCost = new double[2];
        clickUpgradeLevels = new int[2];
        productionUpgradelevels = new int[2];
        clickUpgradeBaseCosts = new double[]{10, 25};
        clickUpgradeMults = new double[]{1.07, 1.07};
        clickUpgradePower = new double[]{1, 5};
        clickUpgradeUnlockCost = new double[]{5, 25};
        productionUpgradeBaseCosts = new double[]{25,250};
        productionUpgradeMults = new double[]{1.07, 1.07};
        productionUpgradeUnlockCost = new double[]{15, 250};
        OCDText.text = $"OCD: {(game.data.OCDBUy ? "ON" : "OFF")}";
    }

    public void RunUpgrades()
    {
        var data = game.data;
        ArrayManager();
        clickUpgradeCost[0] = clickUpgradeBaseCosts[0] * Math.Pow(clickUpgradeMults[0], data.clickUpgrade1Level);    //afta ola na ginoun dynamika
        clickUpgradeCost[1] = clickUpgradeBaseCosts[1] * Math.Pow(clickUpgradeMults[1], data.clickUpgrade2Level);    //afta ola na ginoun dynamika
        productionUpgradeCost[0] = productionUpgradeBaseCosts[0] * Math.Pow(productionUpgradeMults[0], data.productionUpgrade1Level);    //afta ola na ginoun dynamika
        productionUpgradeCost[1] = productionUpgradeBaseCosts[1] * Math.Pow(productionUpgradeMults[1], data.productionUpgrade2Level);    //afta ola na ginoun dynamika
    }

    public void RunUpgradesUI()
    {
        var data = game.data;
        
        for (var i = 0; i < 2; i++ )
        {
            clickUpgradeText[i].text = $"Click Upgrade {i+1}\nCost:  {GetUpgradeCost(i, clickUpgradeCost)} Coins\nPower + {clickUpgradePower[i]}  Click\nLevel:  {GetUpgradeLevel(i, clickUpgradeLevels)}";            
            clickUpgradeMaxText[i].text = $"Buy Max ({BuyClickUpgradeMaxCount(i)})";
            productionUpgradeText[i].text = $"Production Upgrade {i+1}\nCost: {GetUpgradeCost(i, productionUpgradeCost)} Coins\nPower: + {Methods.NotationMethod( game.TotalBoost() * Math.Pow(1.1, game.data.prestigeULevels[1]), "F2")}\nLevel: {GetUpgradeLevel(i, productionUpgradelevels)}";
            productionUpgradeMaxText[i].text = $"Buy Max ({BuyProductionUpgradeMaxCount(i)})";
            clickUpgrade[i].gameObject.SetActive(data.coinsCollected >= clickUpgradeUnlockCost[i]);   
            productionUpgrade[i].gameObject.SetActive(data.coinsCollected >= productionUpgradeUnlockCost[i]); 
        }

        criticalChanceText.text = $"100x Critical Chance\nCost: {Methods.NotationMethod(critCost, "F2")} Coins\nPower +0.1%\nLevel: {data.critLevels}";
        criticalChanceMaxText.text = $"Buy Max ({CalculateBuyCount2(critCost, 1e3, 2, data.critLevels)})";

        clickUpgrade1Bar.fillAmount = Methods.SmoothLoadingBar(clickUpgrade1Bar.fillAmount, data.coins, clickUpgradeCost[0]);
        clickUpgrade2Bar.fillAmount = Methods.SmoothLoadingBar(clickUpgrade2Bar.fillAmount, data.coins, clickUpgradeCost[1]);

        productionUpgrade1Bar.fillAmount = Methods.SmoothLoadingBar(productionUpgrade1Bar.fillAmount, data.coins, productionUpgradeCost[0]);
        productionUpgrade2Bar.fillAmount = Methods.SmoothLoadingBar(productionUpgrade2Bar.fillAmount, data.coins, productionUpgradeCost[1]);

        criticalUpgradeBar.fillAmount = Methods.SmoothLoadingBar(criticalUpgradeBar.fillAmount, data.coins, clickUpgradeCost[1]);


        string GetUpgradeCost(int index, double[] upgrade)
        {
            return Methods.NotationMethod(upgrade[index], "F2");
        }

        string GetUpgradeLevel(int index, int[] upgradelevel)
        {
            return Methods.NotationMethod(upgradelevel[index], "F2");
        }
    }

    public void BuyCrit()
    {
        var data = game.data;
        if(data.critLevels >= 1000) return;
        if (data.coins < critCost) return;
        data.critLevels++;
        data.coins -= critCost;
            

         NonArrayManager();
    }

    public void BuyMaxCrit()
    {
        var data = game.data;
        if(data.critLevels >= 1000) return;
        Methods.BuyMaxLimit(ref data.coins, 1e3, 2, ref data.critLevels, 1000);
    }

       public void BuyClickUpgrade(int index)
    {
        var data = game.data;

         if (data.coins >= clickUpgradeCost[index])
            {
                clickUpgradeLevels[index]++;
                data.coins -= clickUpgradeCost[index];
                data.coinsClickValue += clickUpgradePower[index];
            }

         NonArrayManager();
    }

      public void BuyProductionUpgrade(int index)
    {
        var data = game.data;

         if (data.coins >= clickUpgradeCost[index])
            {
                productionUpgradelevels[index]++;
                data.coins -= productionUpgradeCost[index];            
            }
        NonArrayManager();
    }   

    
        public double CalculateBuyCount(double c, double r, double b, double k)
     {
         
        switch(buyMode)
        {
            case 0:
                return Math.Floor(Math.Log(c * (r - 1) / (b * Math.Pow(r, k)) + 1, r));    //n = poses fores boreis na kaneis to upgrade afti ti stigmi
            case 1:
               return game.data.OCDBUy ? k - (int)k % 5 + 5 - k : 5;
            case 2:
                return game.data.OCDBUy ? k - (int)k % 10 + 10 - k : 10;
            case 3:
                return game.data.OCDBUy ? k - (int)k % 100 + 100 - k : 100;
        }

        return 0;
     }

     public double CalculateBuyCount2(double c, double r, double b, double k)
     {
   
        return Math.Floor(Math.Log(c * (r - 1) / (b * Math.Pow(r, k)) + 1, r));    //n = poses fores boreis na kaneis to upgrade afti ti stigmi
           
     }

     //Buy UPG1 Buy Max Method Below

       public void BuyClickUpgradeMax(int index)   //TODO na ginei dinamika to buy MAX, min exoume 100 buyMax sto telos
    {
        var data = game.data;
        var b = clickUpgradeBaseCosts[index];  // b = arxiki timi upgrade TODO katharismo axriston metavliton
        var c = data.coins;         // c = coins
        var r = clickUpgradeMults[index];               // r = rithmos afksisis timis upgrade
        var k = clickUpgradeLevels[index]; // k = torino epipedo upgrade
        var n = CalculateBuyCount(c, b, r, k);

        var cost = b * (Math.Pow(r, k) * (Math.Pow(r, n) - 1) / (r - 1));

        if (data.coins >= cost)
        {
            clickUpgradeLevels[index] += (int)n;
            data.coins -= cost;
            data.coinsClickValue += n * clickUpgradePower[index];
        }
           NonArrayManager();
    }

    public double BuyClickUpgradeMaxCount(int index)
    {
        var data = game.data;
        var b = clickUpgradeBaseCosts[index];  // b = arxiki timi upgrade TODO katharismo axriston metavliton
        var c = data.coins;         // c = coins
        var r = clickUpgradeMults[index];               // r = rithmos afksisis timis upgrade
        var k = clickUpgradeLevels[index]; // k = torino epipedo upgrade

        return CalculateBuyCount(c, b, r, k);
    }   

    public void BuyProductionUpgradeMax(int index)   //TODO na ginei dinamika to buy MAX, min exoume 100 buyMax sto telos
    {
        var data = game.data;
        var b = productionUpgradeBaseCosts[index];  // b = arxiki timi upgrade TODO katharismo axriston metavliton
        var c = data.coins;         // c = coins
        var r = productionUpgradeMults[index];               // r = rithmos afksisis timis upgrade
        var k = productionUpgradelevels[index]; // k = torino epipedo upgrade
        var n = CalculateBuyCount(c, b, r, k);

        var cost = b * (Math.Pow(r, k) * (Math.Pow(r, n) - 1) / (r - 1));

        if (data.coins >= cost)
        {
            productionUpgradelevels[index] += (int)n;
            data.coins -= cost;
            data.coinsClickValue += n * clickUpgradePower[index];
        }
           NonArrayManager();
    }

    public void ChangeBuyMode()
    {
        switch(buyMode)
        {
            case 0:
                buyMode = 1;
                buyModeText.text = "Buy Mode: 5";
                break;
            case 1:
                buyMode = 2;
                buyModeText.text = "Buy Mode: 10";
                break;
            case 2:
                buyMode = 3;
                buyModeText.text = "Buy Mode: 100";
                break;
            case 3:
                buyMode = 0;
                buyModeText.text = "Buy Mode: Max";
                break;
        }
    }   

    public void ToggleOCD()
    {
        game.data.OCDBUy = !game.data.OCDBUy;
        OCDText.text = $"OCD: {(game.data.OCDBUy ? "ON" : "OFF")}";
    }

    public double BuyProductionUpgradeMaxCount(int index)
    {
        var data = game.data;
        var b = productionUpgradeBaseCosts[index];  // b = arxiki timi upgrade TODO katharismo axriston metavliton
        var c = data.coins;         // c = coins
        var r = productionUpgradeMults[index];               // r = rithmos afksisis timis upgrade
        var k = productionUpgradelevels[index]; // k = torino epipedo upgrade

        return CalculateBuyCount(c, b, r, k);
    }

           
    private void ArrayManager()
    {
        var data = game.data; 

        clickUpgradeLevels[0] = data.clickUpgrade1Level;
        clickUpgradeLevels[1] = data.clickUpgrade2Level;
        productionUpgradelevels[0] = data.productionUpgrade1Level;
        productionUpgradelevels[1] = data.productionUpgrade2Level;
    }

    private void NonArrayManager()
    {
        var data = game.data; 

        data.clickUpgrade1Level = clickUpgradeLevels[0];
        data.clickUpgrade2Level = clickUpgradeLevels[1];
        data.productionUpgrade1Level = productionUpgradelevels[0];
        data.productionUpgrade2Level = productionUpgradelevels[1];
    }
}
