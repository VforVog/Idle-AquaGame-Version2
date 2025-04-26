using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class PlayerData
{
public bool offlineProgressCheck;

#region Earth
#region Basics
public double coins;
public double coinsCollected;
public double coinsClickValue;
public double coinsPerSecond;
#endregion
#region Upgrades
public int clickUpgrade1Level;
public int productionUpgrade1Level;
public int clickUpgrade2Level;
public int productionUpgrade2Level;
public double productionUpgrade2Power;
public int critLevels;
#endregion
#region Prestige
public double gems;
public double gemsBoost;
public double gemsToGet;
#endregion  
#region Achievements
public int achLevel1;
public int achLevel2;
#endregion
#region Prestige Upgrades
public List<int> prestigeULevels;
#endregion
#region Rebirth
public double souls;
#endregion
#region Automators
public int autolevel1;
public int autolevel2;
#endregion
#endregion
#region Mars
public double marsCoins;
#region Mars Upgrades
public int marsUpgradeLevel1;
public int marsUpgradeLevel2;
#endregion
#endregion     
#region Events
    public double eventTokens;
    public float[] eventCooldown = new float[7];
    public int eventActiveID;
#endregion
#region Settings
public short notationType;
public bool OCDBUy;
#endregion
#region Offline
#endregion
#region DailyRewards
public int currentDay;
public DateTime UTCTime;
public bool dailyRewardReady;
#endregion

public PlayerData()
{

offlineProgressCheck = false;

#region Earth
#region Basics
        coins = 0;
        coinsCollected = 0;
        coinsClickValue = 1;
#endregion

#region Upgrades
        productionUpgrade2Power = 5;
        clickUpgrade1Level = 0;
        clickUpgrade2Level = 0;
        productionUpgrade1Level = 0;
        productionUpgrade2Level = 0;
        critLevels = 0;
#endregion

#region Prestige
 gems = 0;
 eventTokens = 0;
#endregion

#region Prestige Upgrades
  prestigeULevels = new List<int>(4);
#endregion

#region Achievements
        achLevel1 = 0;
        achLevel2 = 0;
#endregion

#region Rebirth
  souls = 0;
#endregion

#region Automators
          autolevel1 = 0;
          autolevel2 = 0;
#endregion
#endregion
#region Mars
marsCoins = 1;
#region Mars Upgrades
marsUpgradeLevel1 = 0;
marsUpgradeLevel2 = 0;
#endregion
#endregion
#region Events
        var howManyEvents = eventCooldown.Length - 1;   //to kano etsi giati einai pio grigoro (allios kanei afti tin praksi mesa stin if ksana kai ksana)
        for (int i = 0; i < howManyEvents - 1; i++)
        {
            eventCooldown[i] = 0;
        }
        eventActiveID = 0;
#endregion  
#region Settings
notationType = 0;
OCDBUy = false;
#endregion
#region DailyRewards
dailyRewardReady = false;
currentDay = 0;
UTCTime = DateTime.UtcNow;
#endregion

}

public void CheckObjects()
{
        if(prestigeULevels == null) prestigeULevels = new List<int>();
}

}