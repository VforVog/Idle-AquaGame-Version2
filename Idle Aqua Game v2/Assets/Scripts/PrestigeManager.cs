using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class PrestigeManager : MonoBehaviour
{
    public IdleTutorialGame game;
    public RebirthManager rebirth;

    public Canvas prestige;

    public Text[] costText = new Text[4];
    public Image[] costBars = new Image[4];

    public string[] costDesc;
    public double[] costs;

    private double cost1 => 5 * Math.Pow(1.5, game.data.prestigeULevels[0]);
    private double cost2 => 10 * Math.Pow(1.5, game.data.prestigeULevels[1]);
    private double cost3 => 100 * Math.Pow(2.5, game.data.prestigeULevels[2]);
    private double cost4 => 1e3 * Math.Pow(1.75, game.data.prestigeULevels[3]);

    public double gemsTemp;
    public Text gemsText;
    public Text gemsBoostText;
    public Text gemsToGetText;
    public int prestigeULevelsSize;


    public void StartPrestige()
    {
        var data = game.data;

        prestigeULevelsSize = 4;
        costs = new double[prestigeULevelsSize];
        costDesc = new[] { "Click is 50% more effective", "You gain 10% more coins per second", "Gems are + 1.01x times better", "Offline Progress is +1% Better" };

        if(data.prestigeULevels.Count != prestigeULevelsSize)
        {
            var tempCount = data.prestigeULevels.Count;
            for(var i = 0; i < prestigeULevelsSize - tempCount; i++)
            {
                data.prestigeULevels.Add(0);
            }
        }
    }

    public void Run()
    {
        var data= game.data;

        ArrayManager();
        UI();
        
        data.gemsToGet = 150 * Math.Sqrt(data.coinsCollected / 1e7);     //mathimatikos tipos gia balanced prestige gain, kanonika thelei 1e15 oxi 1e7 episeis evala strogilopoiisi pros ta kato

        gemsText.text = "Gems: " + Methods.NotationMethod(Math.Floor(data.gems), "F2");
        gemsBoostText.text = Methods.NotationMethod(TotalGemBoost(), "F2") + "x Boost";

        if (game.mainMenuGroup.gameObject.activeSelf)    //Afta ousiastika kanoun update mono oso o xristeis kathete sto main menu
        {
            gemsToGetText.text = "Prestige: \n+" + Math.Floor(data.gemsToGet).ToString("F2") + " Gems";
        }

        void UI()
        {
            if (!game.prestigeGroup.gameObject.activeSelf) return;
            {
                 var length = costText.Length;   //gia na min kanei calculations mesa stin for
                for (int i = 0; i < length; i++)
                {
                    costText[i].text = $"Level {data.prestigeULevels[i]}\n{costDesc[i]}\nCost: {Methods.NotationMethod(costs[i], "F2")}  Money";
                    costBars[i].fillAmount = Methods.SmoothLoadingBar(costBars[i].fillAmount,game.data.gems, costs[i]);
                }

            } 
        }
    }

    public void BuyUpgrade(int id)
    {
        var data = game.data;

        switch (id)
        {
            case 0:
                Buy(ref data.prestigeULevels, 0);
                break;
            case 1:
                Buy(ref data.prestigeULevels, 1);
                break;
            case 2:
                Buy(ref data.prestigeULevels, 2);
                break;
            case 3:
                Buy(ref data.prestigeULevels, 3);
                break;
        }

        void Buy(ref List<int> level, int lvl)
        {
            if (data.gems < costs[id]) return;
            data.gems -= costs[id];
            level[lvl]++;
        }
    }

    private void ArrayManager()
    {
        costs[0] = cost1;
        costs[1] = cost2;
        costs[2] = cost3;
        costs[3] = cost4;
    }

    
    public void Prestige()
    {
        var data = game.data;
        //TODO na ginei ligo pio sosto me default values, kai oxi me karfota noumera
        if (data.coinsCollected >  1000)
        {
            PrestigeReset();
            data.coins = 1;
            data.gems += data.gemsToGet;
            data.productionUpgrade2Power = 5;
        }
    }
     public double TotalGemBoost()
    {
        var temp = game.data.gems;
        temp *= 0.05 + (game.data.prestigeULevels[2] * 0.01);  //Kathe gem sou dinei 5% boost +1% gia kathe level tou prestige upgrade gia gems
        temp *= rebirth.soulsBoost;
        return temp + 1;
    }

    public void PrestigeReset() //To kano etsi giati borei na xrisimopoiithei kai allou. Xrhsimopoieitai kai sto Rebirth
    {
        var data = game.data;

        data.coinsCollected = 0; 
        data.coinsClickValue = 1;   //TODO na doume ligo poia pragmata xreiazete na ginontai reset

        data.clickUpgrade1Level = 0;
        data.clickUpgrade2Level = 0;
        data.productionUpgrade1Level = 0;
        data.productionUpgrade2Level = 0;
    }

}
