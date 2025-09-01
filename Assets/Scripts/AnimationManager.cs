using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour {
    public static AnimationManager Instance { get; private set; }
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public GameObject skateboard;
    public GameObject fries;
    //public GameObject model;
    public Animator modelAnim;
    public Animator cameraAnim;
    public Animator fryAnim;
    public Animator skateAnim;

    public float normalSpeed = 1f;
    public float jumpBoostSpeed = 0.5f;
    private void Update() {
        // Always keep animators in sync with jump boost state
        float targetSpeed = PowerupManager.Instance.hasJumpBoost ? jumpBoostSpeed : normalSpeed;

        // Apply to the animators that matter
        modelAnim.speed = targetSpeed;
        fryAnim.speed = targetSpeed;
        skateAnim.speed = targetSpeed;
    }
    public void PlayerDeath() {
        skateAnim.enabled = false;
        skateboard.GetComponent<BoxCollider>().enabled=true;
        skateboard.AddComponent<Rigidbody>();
        skateboard.GetComponent<Rigidbody>().AddForce(Vector3.up * 10f, ForceMode.Impulse);
        fryAnim.enabled = false;
        fries.AddComponent<Rigidbody>();
        fries.AddComponent<BoxCollider>();
        modelAnim.enabled = false;
        for (int i = 0; i < fries.transform.childCount; i++) {
            fries.transform.GetChild(i).gameObject.AddComponent<Rigidbody>();
            fries.transform.GetChild(i).gameObject.AddComponent<BoxCollider>();
        }
        
    }

    public void StartGame() {
        cameraAnim.SetTrigger("Start");
        fryAnim.SetTrigger("Start");
        skateAnim.SetTrigger("Start");
    }

    public void EndGrind() {
        skateAnim.SetBool("5-0", false);
        skateAnim.SetBool("50-50", false);
        skateAnim.SetBool("Nosegrind", false);
        skateAnim.SetBool("leftCrooked", false);
        skateAnim.SetBool("rightCrooked", false);
        skateAnim.SetBool("Salad", false);
        skateAnim.SetBool("Suski", false);
        skateAnim.SetBool("Feeble", false);
        skateAnim.SetBool("Smith", false);
        skateAnim.SetBool("Willy", false);
        skateAnim.SetBool("OverWilly", false);
    }


    public void ChooseJumpTrick(string trick) {

        if (string.IsNullOrEmpty(trick))
            return;

        skateAnim.SetTrigger(trick); // assumes trigger names match trick strings
        fryAnim.SetTrigger("Jump");

    }
    public void JumpAnimation(string trick) {
        skateAnim.SetTrigger(trick);
        fryAnim.SetTrigger("Jump");
    }


    public void PlayGrind(string trick) {
        fryAnim.SetTrigger("Grind");
        skateAnim.SetBool(trick, true);

    }

    public void PlaySpin(string trick) {
        modelAnim.SetTrigger(trick);
    }
}