using TMPro;

using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }


    public GameObject gameplayPanel;
    public GameObject gameOverPanel;
    public GameObject startPanel;
    public GameObject newLevelPanel;
    public GameObject shopPanel;

    public TMP_Text stats;
    public TMP_Text endScore;
    public TMP_Text startScore;
    public TMP_Text newLevelText;
    public GameObject levelBarGO;
    public GameObject newSticker;
    public Slider levelBar;
    public TMP_Text levelBarText;

    public GameObject sliderJumpBoost;
    public GameObject sliderCoin3x;
    public GameObject sliderStopwatch;

    public void Initialize() {
        startScore.text = "COINS: " + SaveManager.saveData.wallet + " COINS\nHIGH SCORE: " + SaveManager.saveData.highScore + "m";
        startPanel.SetActive(true);
        stats.text = null;
        levelBar.value = SaveManager.saveData.levelScore;
        levelBarText.text = "LEVEL " + SaveManager.saveData.level;
    }
    private void OnEnable() {
        GameManager.Instance.OnCoinsChanged += UpdateCoins;
        GameManager.Instance.OnScoreChanged += UpdateDistance;

    }

    private void OnDisable() {
        if (GameManager.Instance == null) return;
        GameManager.Instance.OnCoinsChanged -= UpdateCoins;
        GameManager.Instance.OnScoreChanged -= UpdateDistance;

    }
    private void UpdateCoins(int coins) {
        RefreshStats();
    }

    private void UpdateDistance(int distance) {
        RefreshStats();
    }
    private void RefreshStats() {
        stats.text = "COINS: " + GameManager.Instance.coins + "\nSCORE: " + GameManager.Instance.score + "m" + "\nHIGH SCORE: " + SaveManager.saveData.highScore + "m";
    }

    public void PlayerDeath(int distance) {
        endScore.text = "SCORE: " + GameManager.Instance.score + "m\nHIGH SCORE: " + SaveManager.saveData.highScore + "m";
        gameplayPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    public void ShowShop() {
        startPanel.SetActive(false);
        shopPanel.SetActive(true);
    }

    public void HideShop() {

        startScore.text = "COINS: " + SaveManager.saveData.wallet + " COINS\nHIGH SCORE: " + SaveManager.saveData.highScore + "m";
        startPanel.SetActive(true);
        shopPanel.SetActive(false);
    }

    public void EndLevelReward() {
        startPanel.SetActive(true);
        newLevelPanel.SetActive(false);
        SaveManager.saveData.newLevel = false;
    }
    public void ShowLevelReward() {
        startPanel.SetActive(false);
        Instance.newLevelPanel.SetActive(true);
        switch (SaveManager.saveData.level) {
            case 2:
                newLevelText.text = "NEW TRICK LEARNED!\r\n\r\n180\r\n\r\nSTEER AFTER JUMPING"; // 180s
                break;
            case 3:
                newLevelText.text = "NEW TRICK LEARNED!\r\n\r\nPOP SHUV\r\n\r\nSWIPE LEFT OR RIGHT"; // pop shuvs
                break;
            case 4:
                newLevelText.text = "NEW TRICK LEARNED!\r\n\r\nKICKFLIP\r\n\r\nSWIPE LEFT"; // kickflip
                break;
            case 5:
                newLevelText.text = "NEW TRICK LEARNED!\r\n\r\nHEELFLIP\r\n\r\nSWIPE UP-RIGHT"; // heelflip
                break;
            case 6:
                newLevelText.text = "NEW TRICK LEARNED!\r\n\r\n5-0\r\n\r\nGRIND A RAIL OR LEDGE"; // 5-0
                break;
            case 7:
                newLevelText.text = "NEW TRICK LEARNED!\r\n\r\nNOSEGRIND\r\n\r\nGRIND A RAIL OR LEDGE"; // nosegrind
                break;
            case 8:
                newLevelText.text = "NEW TRICK LEARNED!\r\n\r\nNOLLIE FLIP / HEEL\r\n\r\nSWIPE DOWN-LEFT / DOWN-RIGHT"; // nollie flip / heel
                break;
            case 9:
                newLevelText.text = "NEW TRICK LEARNED!\r\n\r\nCROOKED GRIND\r\n\r\nGRIND A RAIL OR LEDGE";  // crooked grind
                break;
            case 10:
                newLevelText.text = "NEW TRICK LEARNED!\r\n\r\nSALAD/SUSKI GRIND\r\n\r\nGRIND A RAIL OR LEDGE"; // salad
                break;
            case 11:
                newLevelText.text = "NEW TRICK LEARNED!\r\n\r\nFEEBLE / SMITH GRIND\r\n\r\nGRIND A RAIL OR LEDGE"; // feeble
                break;
            case 12:
                newLevelText.text = "NEW TRICK LEARNED!\r\n\r\nWILLY GRIND\r\n\r\nGRIND A RAIL OR LEDGE"; // willy
                break;
            default:
                newLevelText.text = "LEVEL UP!\r\n\r\nLEVEL " + SaveManager.saveData.level;
                break;

        }
    }



}
