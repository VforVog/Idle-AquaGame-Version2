using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class AutomatorManager : MonoBehaviour
{
    public IdleTutorialGame game;
    public UpgradeManager upgrades;

    public Text[] costText = new Text[2];
    public Image[] costBars = new Image[2];

    public string[] costDesc;
    public double[] costs;
    public int[] levels;
    public int[] levelsCap;
    public float[] intervals;
    public float[] timer;

    private double cost1 => Math.Floor(1e4 * Math.Pow(1.5, game.data.autolevel1));
    private double cost2 => Math.Floor(1e5 * Math.Pow(1.5, game.data.autolevel2));

    public void StartAutomator()
    {
        costs = new double[2];
        levels = new int[2];
        levelsCap = new int[]{21, 21};
        intervals = new float [2];
        costDesc = new[] { "Click Upgrade 1 Autobuyer", "Production Upgrade 1 Autobuyer"};
        timer = new float[2];

    }

    public void Run()
    {
        ArrayManager();
        UI();
        RunAuto(); 

        void UI()
        {
            if (!game.autoGroup.gameObject.activeSelf) return;
            {
                var length = costText.Length;   //gia na min kanei calculations mesa stin for
                for (int i = 0; i < length; i++)
                {
                    costText[i].text = $"{costDesc[i]}\nCost: {Methods.NotationMethod(costs[i], "F2")} coins\nInterval: {(levels[i] >= levelsCap[i] ? "Instant" : intervals[i].ToString("F1"))}";
                    costBars[i].fillAmount = Methods.SmoothLoadingBar(costBars[i].fillAmount, game.data.coins, costs[i]);
                }
            }
        }

        
        void RunAuto()
        {
            CAuto(0, 0);
            PAuto(1, 0);

            void CAuto(int id, int index)
            {
                if (levels[id] <= 0) return;
                if (levels[id] != levelsCap[id])
                    Buy(id);
                
            else
            {
              if (upgrades.BuyProductionUpgradeMaxCount(index) != 0) upgrades.BuyProductionUpgradeMax(index);
            }
        }

        void PAuto(int id, int index)
            {
                if (levels[id] <= 0) return;
                if (levels[id] != levelsCap[id])
                    Buy(id);
                
            else
            {
              if (upgrades.BuyProductionUpgradeMaxCount(index) != 0) upgrades.BuyProductionUpgradeMax(index);
            }
        }

        void Buy(int id)
        {
            timer[id] += Time.deltaTime;
            if (!(timer[id] >= intervals[id])) return;
            BuyUpgrade(id);
            timer[id] = 0;
        }
    }
  }
       public void BuyUpgrade(int id)
    {
        var data = game.data;

        switch (id)
        {
            case 0:
                Buy(ref data.autolevel1);
                break;
            case 1:
                Buy(ref data.autolevel2);
                break;
            default:
                Debug.Log("Prestige upgrade does not exist");
                break;
        }

        void Buy(ref int level)
        {
            if (!(data.coins >= costs[id] & level < levelsCap[id])) return;
            data.coins -= costs[id];
            level++;
            
        }
    }
    private void ArrayManager()
    {
        var data = game.data;

        costs[0] = cost1;
        costs[1] = cost2;
      

        levels[0] = data.autolevel1;
        levels[1] = data.autolevel2;

        if (data.achLevel1 > 0)
            intervals[0] = 10 - (data.autolevel1 -1) * 0.5f;
        if (data.autolevel2 > 0)
            intervals[1] = 10 - (data.autolevel2 -1) * 0.5f;
        

    }
}

