using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using static UnityEditor.Progress;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    public event Action<int> OnCoinsChanged;
    public event Action<int> OnScoreChanged;
    public int coins;
    public int distance;
    public int trickScore=0;
    public int score;
    public bool isAlive;
    public  bool isGameStarted;
    
    public  bool isInControl = false;

    public int Coins {
        get => coins;
        set {
            coins = value;
            OnCoinsChanged?.Invoke(coins);
        }
    }

    public int Score {
        get => score;
        set {
            score = value;
            OnScoreChanged?.Invoke(score);
        }
    }
    private void Start() {
        SaveManager.Load();
        UIManager.Instance.Initialize();
        ShopManager.Instance.RefreshUI();

        
        Time.timeScale = 1.0f;
        isAlive = true;
        isGameStarted = false;

        PlayerMovement.Instance.enabled = false;
        CameraManager.Instance.enabled = false;
        Physics.gravity = new Vector3(0, -20f, 0);
        
        if (SaveManager.saveData.newLevel) {
            UIManager.Instance.ShowLevelReward();
        }
    }

    private void Update() {
        distance = Mathf.RoundToInt(transform.position.z) / 10;
        Score = distance + trickScore;

    }

    //private void OnEnable() {
       // UIManager.Instance.Initialize();

    //}   
    public void AddScore (int amount) {
        trickScore += amount;
    }
    public void OnCollisionEnter(Collision collider) {
        if ((collider.gameObject.tag == "Obstacle") && isAlive) {
            PlayerDeath();
            
        }
        
    }  
    
    public void PlayerDeath() {
        isAlive = false;
        
        SaveManager.saveData.wallet+=coins;
        SaveManager.saveData.totalScore += score;
        if (score > SaveManager.saveData.highScore) {
            SaveManager.saveData.highScore = score;
            UIManager.Instance.newSticker.SetActive(true);
        }
        SaveManager.saveData.levelScore+= score;
        while (SaveManager.saveData.levelScore >= 1000) {
            SaveManager.saveData.levelScore -= 1000;
            SaveManager.saveData.level++;
            SaveManager.saveData.newLevel=true;
        }
      
        

        PlayerMovement.Instance.enabled = false;
        PlayerMovement.Instance.rb.useGravity = false;
        PlayerMovement.Instance.rb.isKinematic = true;

        Time.timeScale = 0.25f;
        AnimationManager.Instance.PlayerDeath();
        AudioManager.Instance.PlayerDeath();
        UIManager.Instance.PlayerDeath(distance);
        SaveManager.Save();
    }  
    public void StartGameCoroutine() {
        StartCoroutine(StartGame());
    }
    public IEnumerator StartGame() {
        AudioManager.Instance.sounds[0].source.volume = AudioManager.Instance.sounds[0].source.volume / 2;
        AnimationManager.Instance.StartGame();
 
        UIManager.Instance.startPanel.SetActive(false);
        
        PlayerMovement.Instance.enabled = true;
        isInControl = false;
        CameraManager.Instance.enabled = true;
        
        
        yield return new WaitForSeconds(1);
        isGameStarted = true;
        isInControl = true;
        UIManager.Instance.gameplayPanel.SetActive(true);
        
    }


    

    

    
}
