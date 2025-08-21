using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
            GameManager.Instance.coins = GameManager.Instance.coins + 1 * coinMultiplier;
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

        }
    }

    public int coinMultiplier = 1;
    public bool hasJumpBoost;
    public bool hasCoin3x;
    public bool hasStopwatch;
    public int timerJumpBoost;
    public int timerCoin3x;
    public int timerStopwatch;

    public IEnumerator Stopwatch() {
        hasStopwatch = true;
        UIManager.Instance.sliderStopwatch.gameObject.GetComponent<Slider>().value = 15;
        UIManager.Instance.sliderStopwatch.gameObject.SetActive(true);
        Time.timeScale = 0.5f;
        AudioManager.Instance.ChangePitch(0.5f);
        //yield return new WaitForSeconds(7.5f);
        timerStopwatch = 15;
        for (int i = 0; i < 15; i++) {
            yield return new WaitForSeconds(0.5f);
            timerStopwatch--;
            UIManager.Instance.sliderStopwatch.gameObject.GetComponent<Slider>().value = timerStopwatch;
        }
        if (Time.timeScale == 0.5f && GameManager.Instance.isAlive) {
            Time.timeScale = 1f;
            AudioManager.Instance.ChangePitch(1f);
        }
        UIManager.Instance.sliderStopwatch.gameObject.SetActive(false);
        hasStopwatch = false;
    }

    public IEnumerator Coin3x() {
        hasCoin3x = true;
        coinMultiplier = 3;
        UIManager.Instance.sliderCoin3x.gameObject.GetComponent<Slider>().value = 15;
        UIManager.Instance.sliderCoin3x.gameObject.SetActive(true);
        //yield return new WaitForSeconds(15);
        timerCoin3x = 15;
        for (int i = 0; i < 15; i++) {
            yield return new WaitForSeconds(1);
            timerCoin3x--;
            UIManager.Instance.sliderCoin3x.gameObject.GetComponent<Slider>().value = timerCoin3x;
        }
        UIManager.Instance.sliderCoin3x.gameObject.SetActive(false);
        coinMultiplier = 1;
        hasCoin3x = false;
    }
    public IEnumerator JumpBoost() {
        hasJumpBoost = true;
        PlayerMovement.Instance.jumpForce = 18f;
        UIManager.Instance.sliderJumpBoost.gameObject.GetComponent<Slider>().value = 15;
        UIManager.Instance.sliderJumpBoost.gameObject.SetActive(true);
        //yield return new WaitForSeconds(15);
        timerJumpBoost = 15;
        for (int i = 0; i < 15; i++) {
            yield return new WaitForSeconds(1);
            timerJumpBoost--;
            UIManager.Instance.sliderJumpBoost.gameObject.GetComponent<Slider>().value = timerJumpBoost;
        }
        PlayerMovement.Instance.jumpForce = 12f;
        UIManager.Instance.sliderJumpBoost.gameObject.SetActive(false);
        hasJumpBoost = false;
    }

}
