using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public IdleTutorialGame game;
    public GameObject[] clickUpgrade = new GameObject[2];
    public GameObject[] productionUpgrade = new GameObject[2];
    
    public Text[] clickUpgradeText = new Text[2];
    public Text[] clickUpgradeMaxText = new Text[2];
     public Text[] productionUpgradeText = new Text[2];
    public Text[] productionUpgradeMaxText = new Text[2];
    public Image clickUpgrade1Bar;
    public Image clickUpgrade2Bar;
    public Image productionUpgrade1Bar;
    public Image productionUpgrade2Bar;

    public double[] clickUpgradeCost;
    public double[] clickUpgradeUnlockCost;
    public double[] productionUpgradeCost;
    public double[] productionUpgradeUnlockCost;
    public double[] clickUpgradePower;
    public int[] clickUpgradeLevels;
    public int[] productionUpgradelevels;
    public double[] clickUpgradeBaseCosts;
    public double[] clickUpgradeMults;
    public double[] productionUpgradeBaseCosts;
    public double[] productionUpgradeMults;
    
    public double clickUpgrade1Cost;
    public double clickUpgrade2Cost;
    public double productionUpgrade1Cost;
    public double productionUpgrade2Cost;


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
        clickUpgradeLevels = new int[2];
        productionUpgradelevels = new int[2];

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
  

        string GetUpgradeCost(int index, double[] upgrade)
        {
            return Methods.NotationMethod(upgrade[index], "F2");
        }

        string GetUpgradeLevel(int index, int[] upgradelevel)
        {
            return Methods.NotationMethod(upgradelevel[index], "F2");
        }
        
        for (var i = 0; i < 2; i++ )
        {
            clickUpgradeText[i].text = $"Click Upgrade {i+1}\nCost:  {GetUpgradeCost(i, clickUpgradeCost)} coins\nPower + {clickUpgradePower[i]}  Click\nLevel:  {GetUpgradeLevel(i, clickUpgradeLevels)}";            
            clickUpgradeText[i].text = $"Buy Max ({BuyClickUpgradeMaxCount(i)})";
            productionUpgradeText[i].text = $"Production Upgrade {i}\nCost: {GetUpgradeCost(i, productionUpgradeCost)} Coins\nPower: + ({Methods.NotationMethod( game.TotalBoost() * Math.Pow(1.1, game.prestige.levels[i]), "F2")} \nLevel: {GetUpgradeLevel(i, clickUpgradeLevels)}";
            productionUpgradeText[i].text = $"Buy Max ({BuyProductionUpgradeMaxCount(i)})";
            clickUpgrade[i].gameObject.SetActive(data.coinsCollected >= clickUpgradeUnlockCost[i]);   
            productionUpgrade[i].gameObject.SetActive(data.coinsCollected >= productionUpgradeUnlockCost[i]); 
        }

        clickUpgrade1Bar.fillAmount = Methods.SmoothLoadingBar(clickUpgrade1Bar.fillAmount, data.coins, clickUpgradeCost[0]);
        clickUpgrade2Bar.fillAmount = Methods.SmoothLoadingBar(clickUpgrade2Bar.fillAmount, data.coins, clickUpgradeCost[1]);

        productionUpgrade1Bar.fillAmount = Methods.SmoothLoadingBar(productionUpgrade1Bar.fillAmount, data.coins, productionUpgradeCost[0]);
        productionUpgrade2Bar.fillAmount = Methods.SmoothLoadingBar(productionUpgrade2Bar.fillAmount, data.coins, productionUpgradeCost[1]);

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

       public void BuyClickUpgradeMax(int index)   //TODO na ginei dinamika to buy MAX, min exoume 100 buyMax sto telos
    {
        var data = game.data;
        var b = clickUpgradeBaseCosts[index];  // b = arxiki timi upgrade TODO katharismo axriston metavliton
        var c = data.coins;         // c = coins
        var r = clickUpgradeMults[index];               // r = rithmos afksisis timis upgrade
        var k = clickUpgradeLevels[index]; // k = torino epipedo upgrade
        var n = (int)Math.Floor(Math.Log(c * (r - 1) / (b * Math.Pow(r, k)) + 1, r));    //n = poses fores boreis na kaneis to upgrade afti ti stigmi

        var cost = b * (Math.Pow(r, k) * (Math.Pow(r, n) - 1) / (r - 1));

        if (data.coins >= cost)
        {
            clickUpgradeLevels[index] += (int)n;
            data.coins -= cost;
            data.coinsClickValue += n * clickUpgradePower[index];
        }
        NonArrayManager();
    }
    public int BuyClickUpgradeMaxCount(int index)
    {
        var data = game.data;
        var b = clickUpgradeBaseCosts[index];  // b = arxiki timi upgrade TODO katharismo axriston metavliton
        var c = data.coins;         // c = coins
        var r = clickUpgradeMults[index];               // r = rithmos afksisis timis upgrade
        var k = clickUpgradeLevels[index]; // k = torino epipedo upgrade
        var n = (int)Math.Floor(Math.Log(c * (r - 1) / (b * Math.Pow(r, k)) + 1, r));    //n = poses fores boreis na kaneis to upgrade afti ti stigmi
        return n;
    }   

    public void BuyProductionUpgradeMax(int index)   //TODO na ginei dinamika to buy MAX, min exoume 100 buyMax sto telos
    {
        var data = game.data;
        var b = productionUpgradeBaseCosts[index];  // b = arxiki timi upgrade TODO katharismo axriston metavliton
        var c = data.coins;         // c = coins
        var r = productionUpgradeMults[index];               // r = rithmos afksisis timis upgrade
        var k = productionUpgradelevels[index]; // k = torino epipedo upgrade
        var n = (int)Math.Floor(Math.Log(c * (r - 1) / (b * Math.Pow(r, k)) + 1, r));    //n = poses fores boreis na kaneis to upgrade afti ti stigmi

        var cost = b * (Math.Pow(r, k) * (Math.Pow(r, n) - 1) / (r - 1));

        if (data.coins >= cost)
        {
            productionUpgradelevels[index] += (int)n;
            data.coins -= cost;
        }
        NonArrayManager();
    }

    public int BuyProductionUpgradeMaxCount(int index)
    {
        var data = game.data;
        var b = productionUpgradeBaseCosts[index];  // b = arxiki timi upgrade TODO katharismo axriston metavliton
        var c = data.coins;         // c = coins
        var r = productionUpgradeMults[index];               // r = rithmos afksisis timis upgrade
        var k = productionUpgradelevels[index]; // k = torino epipedo upgrade
        var n = (int)Math.Floor(Math.Log(c * (r - 1) / (b * Math.Pow(r, k)) + 1, r));    //n = poses fores boreis na kaneis to upgrade afti ti stigmi
        return n;
    }

           
    private void ArrayManager()
    {
        var data = game.data; 

        clickUpgradeLevels[0] = data.clickUpgrade1Level;
        clickUpgradeLevels[1] = data.clickUpgrade2Level;
        productionUpgradelevels[0] = data.productionUpgrade1Level;
        productionUpgradelevels[1] = data.productionUpgrade1Level;
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
