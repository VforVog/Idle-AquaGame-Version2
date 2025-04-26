using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Networking;

public class OfflineManager : MonoBehaviour
{
    public IdleTutorialGame game;
    public DailyRewardManager daily; 

    public GameObject offlinePopUp;
    public Text timeAwayText;
    public Text earthGainsText;
    //We can make for Mars if we want, but ourselves

    public async void LoadOfflineProduction()
    {
        if (game.data.offlineProgressCheck)
        {
            //Offline Time Management
            var tempOfflineTime = Convert.ToInt64(PlayerPrefs.GetString("OfflineTime"));
            var oldTime = DateTime.FromBinary(tempOfflineTime);

            var currentTime = await AwaitGetUTCTime();
            var difference = currentTime.Subtract(oldTime);
            var rawTime = (float) difference.TotalSeconds;
            var offlineTime = rawTime / 10 * (game.data.prestigeULevels[3] * 0.01 + 1);

            offlinePopUp.gameObject.SetActive(true);
            TimeSpan timer = TimeSpan.FromSeconds(rawTime);
            timeAwayText.text = $"You were away for\n<color=#00FFFF>{timer:dd\\:hh\\:mm\\:ss}</color>";

            double coinsGains = game.TotalCoinsPerSecond() * offlineTime;
            game.data.coins += coinsGains;
            earthGainsText.text = $"You earned:\n{Methods.NotationMethod(coinsGains, "F2")} Coins";
        }
    }


    public async Task<DateTime> AwaitGetUTCTime()
    {
        StartCoroutine(daily.GetUTCTime());
        await Task.Delay(1000);
        return daily.tempDateTime;
    }

    public void CloseOffline()
    {
        offlinePopUp.gameObject.SetActive(false);
    }
}
/*ΑΜΑ ΕΙΣΑΙ π.χ:15 ΔΕΥΤΕΡΑ OFFLINE,
ΚΑΝΕΙΣ 15/10 = 1.5 * ΤΑ ΛΕΦΤΑ ΠΟΥ ΒΓΑΖΕΙΣ ΑΝΑ /SEC. ΤΟΣΑ ΕΙΝΑ ΤΑ OFFLINE ΚΕΡΔΗ ΣΟΥ
*Υ.Γ:Ο ΧΡΟΝΟΣ ΜΠΟΡΕΙ ΝΑ ΕΙΝΑΙ 1.5 ΑΛΛΑ ΣΤΗ ΠΡΑΓΜΑΤΙΚΟΤΗΤΑ ΕΙΝΑΙ ΑΠΟ 1.51 ΕΩΣ 1.59
*/