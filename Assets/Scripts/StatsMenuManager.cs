using TMPro;
using UnityEngine;

public class StatsMenuManager : MonoBehaviour 
{
    [SerializeField] TMP_Text statsText;

    void Awake() {
        statsText.text =
            "Level: " + SaveManager.saveData.level +
            "\nHigh Score: " + SaveManager.saveData.highScore +
            "\nTotal Score: " + SaveManager.saveData.totalScore +
            "\nCoins: " + SaveManager.saveData.coins +
            "\nTotal Wealth: " + SaveManager.saveData.totalWealth +
            "\nShop Items Owned: " + SaveManager.saveData.ownedItems.Count +
            "\nLongest Distance: " + SaveManager.saveData.longestDistance + "m" +
            "\nTotal Distance: " + SaveManager.saveData.totalDistance + "m" +
            "\nTricks Done: " + SaveManager.saveData.tricksDone;
    }
}
