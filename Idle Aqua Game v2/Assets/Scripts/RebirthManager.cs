using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RebirthManager : MonoBehaviour
{
    public IdleTutorialGame game;
    public PrestigeManager prestige;

    public Text soulsText;
    public Text soulsBoostText;
    public Text soulsToGetText;

    private double soulsToGet => Math.Floor(150 * Math.Sqrt(game.data.gems / 1e4));
    public double soulsBoost => game.data.souls * 0.001 + 1;

    public void Run()
    {

        UI();   //TODO afto edo ginete xoris logo nomizo, tha to doume sto telos tou tutorial

        void UI()
        {
            if (game.rebirthGroup.gameObject.activeSelf)
            {
                soulsText.text = "Souls: " + game.NotationMethod(game.data.souls);
                soulsToGetText.text = game.NotationMethod(soulsToGet) + " Souls";
                soulsBoostText.text = "Gems are: " + Methods.NotationMethod(soulsBoost, "F2") + "x better";
            }
        }
    }

    public void Rebirth()
    {
        var data = game.data;

        data.souls += soulsToGet;

        prestige.PrestigeReset();
        data.coins = 0;
        data.productionUpgrade2Power = 0;
        data.gems = 0;
        data.prestigeULevels = new List<int>(game.prestige.prestigeULevelsSize);  

        game.rebirthGroup.gameObject.SetActive(false);    
        game.mainMenuGroup.gameObject.SetActive(true);    
    }
}
