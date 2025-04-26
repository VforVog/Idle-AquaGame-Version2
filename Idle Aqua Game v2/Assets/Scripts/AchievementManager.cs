using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public IdleTutorialGame game;
    private static string[] AchievementStrings => new string[] { "Current coins", "Total Coins Collected" };
    private double[] AchievementNumbers => new double[] { game.data.coins, game.data.coinsCollected };

    
    public GameObject achievementScreen;
    public List<Achievement> achievementList = new List<Achievement>();

    public void Start()
    {
        foreach (var achievement in achievementScreen.GetComponentsInChildren<Achievement>())
        {
            achievementList.Add(achievement);
        }

    }
    public void RunAchievements()
    {
        var data = game.data;

        //TODO na ta stlenei dinamika me loopa kai oxi na ta grafoume 1-1 me to xeri
        UpdateAchievements(AchievementStrings[0], AchievementNumbers[0], ref data.achLevel1, 
            ref achievementList[0].fill, ref achievementList[0].title, ref achievementList[0].progress);

        UpdateAchievements(AchievementStrings[1], AchievementNumbers[1], ref data.achLevel2,
            ref achievementList[1].fill, ref achievementList[1].title, ref achievementList[1].progress);
    }

    private void UpdateAchievements(string achievName, double currentNumber, ref int achievlevel, ref Image fillBar, ref Text titleText, ref Text progressText)
    {
        double targetNumber = Math.Pow(10, achievlevel);

        if (game.achievementsGroup.gameObject.activeSelf)    //Afto ousiastika trexei mono otan o xristeis koitaei ta achievements tou
        {
            titleText.text = achievName + "\n(" + achievlevel + ")";
            progressText.text = Methods.NotationMethod(currentNumber, "F2") + " / " + Methods.NotationMethod(targetNumber, "F2");

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


}
