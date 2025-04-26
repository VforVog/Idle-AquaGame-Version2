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


public static class SimpleAES
{
    // This size of the IV (in bytes) must = (keysize / 8).  Default keysize is 256, so the IV must be
    // 32 bytes long.  Using a 16 character string here gives us 32 bytes when converted to a byte array.
    private const string initVector = "fp30mv7sr9vmzloe";
    // This constant is used to determine the keysize of the encryption algorithm
    private const int keysize = 256;
    //Encrypt
    public static string EncryptString(string plainText, string passPhrase)
    {
        var initVectorBytes = Encoding.UTF8.GetBytes(initVector);
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        var password = new PasswordDeriveBytes(passPhrase, null);
        var keyBytes = password.GetBytes(keysize / 8);
        RijndaelManaged symmetricKey = new RijndaelManaged() {Mode = CipherMode.CBC};
        var encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
        var memoryStream = new MemoryStream();
        var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
        cryptoStream.FlushFinalBlock();
        var cipherTextBytes = memoryStream.ToArray();
        memoryStream.Close();
        cryptoStream.Close();
        return Convert.ToBase64String(cipherTextBytes);
    }
    //Decrypt
    public static string DecryptString(string cipherText, string passPhrase)
    {
        var initVectorBytes = Encoding.UTF8.GetBytes(initVector);
        var cipherTextBytes = Convert.FromBase64String(cipherText);
        var password = new PasswordDeriveBytes(passPhrase, null);
        var keyBytes = password.GetBytes(keysize / 8);
        RijndaelManaged symmetricKey = new RijndaelManaged() {Mode = CipherMode.CBC};
        var decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
        var memoryStream = new MemoryStream(cipherTextBytes);
        var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        var plainTextBytes = new byte[cipherTextBytes.Length];
        var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
        memoryStream.Close();
        cryptoStream.Close();
        return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
    }
}

public class SaveSystem : MonoBehaviour
{
    public OfflineManager offline;
    private static DateTime currentUTCTime;

    public TMP_InputField importValue;
    public TMP_InputField exportValue;

    private static string encryptKey = "4nddw8fh5yt28cmg4lap209z";    //random 24 letter string
    private static string savePath => Application.persistentDataPath + "/saves/";
    private static string savePathBackUp => Application.persistentDataPath + "/backups/";
    //afto apothikevei sto C:\Users\<OnomaUser>\AppData\LocalLow\DefaultCompany\Idle Aqua Game v2  kai paei ftiaxnei ena arxeio pou legete backups
    //TODO na doume ti akrivos ginete me ta kinita se afti ti fasi
    private static int backUpCount = 0;


    public async Task AwaitGetUTCTime()
    {
        currentUTCTime = await offline.AwaitGetUTCTime();
    }


    public static void SavePlayer<T>(T Data, string name)
    {
        Directory.CreateDirectory(savePath);
        Directory.CreateDirectory(savePathBackUp);
        backUpCount++;
        Save(backUpCount % 4 == 0 ? savePathBackUp : savePath);

        PlayerPrefs.SetString("OfflineTime", currentUTCTime.ToBinary().ToString());

        void Save(string path)
        {
            using (var writer = new StreamWriter(path + name + ".txt"))
            {
                var formatter = new BinaryFormatter();
                var memoryStream = new MemoryStream();
                formatter.Serialize(memoryStream, Data);
                var dataToWrite = SimpleAES.EncryptString(Convert.ToBase64String(memoryStream.ToArray()), encryptKey);
                writer.WriteLine(dataToWrite);
            }
        } 
    }

    public static T LoadPlayer<T>(string name)
{
        Directory.CreateDirectory(savePath);
        Directory.CreateDirectory(savePathBackUp);
        T returnValue;
        var backUpNeeded = false;

        Load(savePath);
        if (backUpNeeded)
        {
            Load(savePathBackUp);
        }
            
    return returnValue;

    void Load(string path)
    {
        using (var reader = new StreamReader(path + name + ".txt"))
        {
            var formatter = new BinaryFormatter();
            var dataToRead = reader.ReadToEnd();
            var memoryStream = new MemoryStream(Convert.FromBase64String(SimpleAES.DecryptString(dataToRead, encryptKey))); 
            try
            {
                returnValue = (T)formatter.Deserialize(memoryStream);
            }       
            catch
            {
                returnValue = default;
                backUpNeeded = true;
            }
        }
    }
}

public static bool SaveExists(string key)
{
    var path = savePath + key + ".txt";
    return File.Exists(path);
}
 
    public void ImportPlayer2(int id)
    {
        var path = "";
        switch (id)
        {
            case 0:
                path = savePath;
                break;
        }

        using (var writer = new StreamWriter(path + name + ".txt"))
        {
            writer.WriteLine(importValue.text);

        }
    }
    public void ExportPlayer2()
    {
        var dataToRead = "";
        var backUpNeeded = false;
        Load(savePath);
        if (backUpNeeded)
        {
            Load(savePathBackUp);
        }
            
        exportValue.text = dataToRead;

    void Load(string path)
    {
        using (var reader = new StreamReader(path + name + ".txt"))
        {
            try
            {
                dataToRead = reader.ReadToEnd();
            }       
            catch
            {
                backUpNeeded = true;
            }
        }
    }
    }
    public void ClearFields()
    {
        exportValue.text = "";
        importValue.text = "";
    }
}
