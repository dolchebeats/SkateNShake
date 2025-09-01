using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveManager
{
    public static string directory = "/data/";
    public static string fileName = "data.sav";
    public static SaveData saveData;

    public static void Save() {
        string dir = Application.persistentDataPath + directory;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        string json = JsonUtility.ToJson(saveData);
        File.WriteAllText(dir + fileName, json);
    }

    public static void Load() {

        string fullPath = Application.persistentDataPath + directory + fileName;
        SaveData save = new SaveData();
        if(File.Exists(fullPath)) {
            string json  = File.ReadAllText(fullPath);
            save = JsonUtility.FromJson<SaveData>(json);
        }
        saveData = save;

    }

}

public class SaveData {
    public ShopItemSO deck;
    public ShopItemSO trucks;
    public ShopItemSO wheels;
    public ShopItemSO graphic;
    public List<ShopItemSO> ownedItems;
    
    public int coins;
    public int totalWealth;
    public int longestDistance;
    public int totalDistance;
    public int highScore;
    public int level;
    public int levelScore;
    public int totalScore;
    public int tricksDone;
    public bool newLevel;

    public SaveData() {
        deck = ShopManager.Instance.shopItemsSO[0];
        trucks = ShopManager.Instance.shopItemsSO[1];
        wheels = ShopManager.Instance.shopItemsSO[2];
        graphic = ShopManager.Instance.shopItemsSO[3];
        ownedItems = new List<ShopItemSO>
        {
            deck,
            trucks,
            wheels,
            graphic
        };

        coins = 0;
        totalWealth = 0;
        longestDistance = 0;
        totalDistance = 0;
        highScore = 0;
        level = 1;
        levelScore = 0;
        totalScore = 0;
        tricksDone = 0;
        newLevel = false;
  
    }

}
