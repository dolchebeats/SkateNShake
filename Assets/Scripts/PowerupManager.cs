using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class PowerupManager : MonoBehaviour
{
    public static PowerupManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void OnTriggerEnter(Collider item) {
        if (item.gameObject.tag == "Coin") {
            GameManager.Instance.Coins = GameManager.Instance.coins + 1 * coinMultiplier;
            Destroy(item.gameObject);
            AudioManager.Instance.PlaySound("Coin");

        }
        else if (item.gameObject.tag == "Stopwatch") {
            Destroy(item.gameObject);
            AudioManager.Instance.PlaySound("ClockTick");
            StopCoroutine("Stopwatch");
            StartCoroutine("Stopwatch");
        }
        else if (item.gameObject.tag == "Coin3x") {
            Destroy(item.gameObject);
            AudioManager.Instance.PlaySound("Coin");
            StopCoroutine("Coin3x");
            StartCoroutine("Coin3x");

        }
        else if (item.gameObject.tag == "JumpBoost") {
            Destroy(item.gameObject);
            AudioManager.Instance.PlaySound("Boing");
            StopCoroutine("JumpBoost");
            StartCoroutine("JumpBoost");

        } else if (item.gameObject.tag == "Shield") {
            Destroy(item.gameObject);
            //AudioManager.Instance.PlaySound("Boing");
            StopCoroutine("Shield");
            StartCoroutine("Shield");

        }
    }

    public int coinMultiplier = 1;
    public bool hasJumpBoost;
    public bool hasCoin3x;
    public bool hasStopwatch;
    public bool hasShield;
    public int timerJumpBoost;
    public int timerCoin3x;
    public int timerStopwatch;
    public int timerShield;
    private Dictionary<string, GameObject> activeIcons = new Dictionary<string, GameObject>();
    [SerializeField] GameObject iconPrefab;
    [SerializeField] Transform powerupPanel;
    [SerializeField] Sprite stopwatchSprite, coin3xSprite, jumpboostSprite, shieldSprite;
    public IEnumerator Stopwatch() {
        hasStopwatch = true;
        StartCoroutine(RunPowerupIcon("Stopwatch", stopwatchSprite, 7.5f));

        Time.timeScale = 0.5f;
        AudioManager.Instance.ChangePitch(0.5f);
        //yield return new WaitForSeconds(7.5f);
        timerStopwatch = 15;
        for (int i = 0; i < 15; i++) {
            yield return new WaitForSeconds(0.5f);
            timerStopwatch--;

        }
        if (Time.timeScale == 0.5f && GameManager.Instance.isAlive) {
            Time.timeScale = 1f;
            AudioManager.Instance.ChangePitch(1f);
        }

        hasStopwatch = false;
    }

    public IEnumerator Coin3x() {
        hasCoin3x = true;
        StartCoroutine(RunPowerupIcon("Coin3x", coin3xSprite, 15f));
        coinMultiplier = 3;

        //yield return new WaitForSeconds(15);
        timerCoin3x = 15;
        for (int i = 0; i < 15; i++) {
            yield return new WaitForSeconds(1);
            timerCoin3x--;

        }

        coinMultiplier = 1;
        hasCoin3x = false;
    }
    public IEnumerator JumpBoost() {
        StartCoroutine(RunPowerupIcon("JumpBoost", jumpboostSprite, 15f));
        hasJumpBoost = true;
        PlayerMovement.Instance.jumpForce = 18f;

        //yield return new WaitForSeconds(15);
        timerJumpBoost = 15;
        for (int i = 0; i < 15; i++) {
            yield return new WaitForSeconds(1);
            timerJumpBoost--;

        }
        PlayerMovement.Instance.jumpForce = 12f;

        hasJumpBoost = false;
    }

    public IEnumerator Shield() {
        hasShield = true;
        StartCoroutine(RunPowerupIcon("Shield", shieldSprite, 15f));
        //GameManager.Instance.isAlive = false;

        //yield return new WaitForSeconds(15);
        timerShield = 15;
        for (int i = 0; i < 15; i++) {
            yield return new WaitForSeconds(1);
            timerShield--;

        }
        //GameManager.Instance.isAlive = true;

        hasShield = false;
    }

    public IEnumerator RunPowerupIcon(string powerupTag, Sprite iconSprite, float duration) {
        // Create & show icon
        GameObject icon = Instantiate(iconPrefab, powerupPanel);
        icon.GetComponent<Image>().sprite = iconSprite;
        activeIcons[powerupTag] = icon;

        float solidTime = duration * (2f / 3f); // 10s of 15, or 5s of 7.5
        float flashTime = duration - solidTime; // 5s of 15, or 2.5s of 7.5

        yield return new WaitForSeconds(solidTime);

        // Trigger flashing
        icon.GetComponent<Animator>().SetTrigger("Flash");

        yield return new WaitForSeconds(flashTime);

        Destroy(icon);
        activeIcons.Remove(powerupTag);
    }

}
