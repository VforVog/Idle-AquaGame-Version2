using UnityEngine.UI;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.PackageManager.Requests;

public class PlanetManager : MonoBehaviour
{
    public IdleTutorialGame game;
    public Canvas Earth;
    public Canvas Mars;

    public Text earthCoinsText;    
    public Text marsCoinsText;    
    public Text EMBoostText;

    public double EMBoost => Math.Log(Math.Sqrt(game.data.marsCoins) + 1, 20) + 1;

    public void Update()
    {
        var data = game.data;
        earthCoinsText.text = $"{Methods.NotationMethod(data.coins, "F2")} Coins";
        marsCoinsText.text = $"{Methods.NotationMethod(data.marsCoins, "F2")} Mars Coins";
        EMBoostText.text = $"{Methods.NotationMethod(EMBoost, "F2")}x\nCoins/s";
    }    

    public void ChangeTabs(string id)
    {
        DisableAll();
        switch (id)
        {
            case "earth":                    
                Earth.gameObject.SetActive(true);
                game.mainMenuGroup.gameObject.SetActive(true);
                break;
            case "mars":                    
                Mars.gameObject.SetActive(true);
                break;
            default:
                Debug.Log("error happenend in ChangeTabs()");
                break;
        }

        void DisableAll()
        {
            Earth.gameObject.SetActive(false);
            Mars.gameObject.SetActive(false);
            game.planetGroup.gameObject.SetActive(false);
        }
    }
}
