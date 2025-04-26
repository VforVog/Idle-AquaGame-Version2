using System;
using UnityEngine;
using UnityEngine.UI;

public class PrestigeManager : MonoBehaviour
{
    public IdleTutorialGame game;

    public Text[] costText = new Text[3];
    public Image[] costBars = new Image[3];

    public string[] costDesc;
    public double[] costs;
    public int[] levels;

    private double cost1 => Math.Floor(5 * Math.Pow(1.5, game.data.prestigeLevel1));
    private double cost2 => Math.Floor(10 * Math.Pow(1.5, game.data.prestigeLevel2));
    private double cost3 => Math.Floor(100 * Math.Pow(2.5, game.data.prestigeLevel3));
    public void StartPrestige()
    {
        costs = new double[3];
        levels = new int[3];
        costDesc = new[] { "Click is 50% more effective", "You gain 10% more coins per second", "Gems are + 1.01x times better" };

    }

    public void Run()
    {
        ArrayManager();
        UI();

        void UI()
        {
            if (game.prestigeGroup.gameObject.activeSelf)
            {
                var length = costText.Length;   //gia na min kanei calculations mesa stin for
                for (int i = 0; i < length; i++)
                {
                    costText[i].text = "Level " + levels[i] + "\n" + costDesc[i] + "\nCost: " + costs[i] + " gems";
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
                Buy(ref data.prestigeLevel1);
                break;
            case 1:
                Buy(ref data.prestigeLevel2);
                break;
            case 2:
                Buy(ref data.prestigeLevel3);
                break;
            default:
                Debug.Log("Prestige upgrade does not exist");
                break;
        }

        void Buy(ref int level)
        {
            if (data.gems >= costs[id])
            {
                data.gems -= costs[id];
                level++;
            }
        }
    }

    public void ArrayManager()
    {
        var data = game.data;

        costs[0] = cost1;
        costs[1] = cost2;
        costs[2] = cost3;

        levels[0] = data.prestigeLevel1;
        levels[1] = data.prestigeLevel2;
        levels[2] = data.prestigeLevel3;
    }
}
