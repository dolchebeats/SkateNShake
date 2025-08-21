using UnityEngine;

public class AnimationManager : MonoBehaviour
{
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



    public void PlayerDeath() {
        Destroy(skateAnim);
        skateboard.AddComponent<BoxCollider>();
        skateboard.AddComponent<Rigidbody>(); 
        Destroy(fryAnim);
        fries.AddComponent<Rigidbody>();
        fries.AddComponent<BoxCollider>();
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


    public void ChooseJumpTrick() {

        switch(SwipeManager.CurrentSwipe) {
            case Swipe.Up:
                JumpAnimation("Jump");
                    break;
            case Swipe.Left:
                if (SaveManager.saveData.level >= 3)
                    JumpAnimation("Pop Shuv");
                else
                    JumpAnimation("Jump");
                break;
            case Swipe.UpLeft:
                if (SaveManager.saveData.level >= 4)
                    JumpAnimation("Kickflip");
                else
                    JumpAnimation("Jump");
                break;
            case Swipe.Right:
                if (SaveManager.saveData.level >= 3)
                    JumpAnimation("FS Shuv");
                else
                    JumpAnimation("Jump");
                break;
            case Swipe.UpRight:
                if (SaveManager.saveData.level >= 5)
                    JumpAnimation("Heelflip");
                else
                    JumpAnimation("Jump");
                break;
            case Swipe.Down:
                if (SaveManager.saveData.level >= 1)
                    JumpAnimation("Nollie");
                else
                    JumpAnimation("Jump");
                break;                      
            case Swipe.DownLeft:
                if (SaveManager.saveData.level >= 8)
                    JumpAnimation("Nollie Flip");
                else
                    JumpAnimation("Jump");
                break;
            case Swipe.DownRight:
                if (SaveManager.saveData.level >= 8)
                    JumpAnimation("Nollie Heel");
                else
                    JumpAnimation("Jump");
                break;                        
            default:
                JumpAnimation("Jump");
                break;


        }
    }
    public void JumpAnimation(string trick) {
        skateAnim.SetTrigger(trick);
        fryAnim.SetTrigger("Jump");
    }

    public void GrindSurface() {
        fryAnim.SetTrigger("Grind");
        int grindChance = Random.Range(5, Mathf.Clamp(SaveManager.saveData.level+1,5+1,12+1));
        switch (grindChance) {
            case 6:
                skateAnim.SetBool("5-0", true);
                break;
            case 7:
                skateAnim.SetBool("Nosegrind", true);
                break;
            case 9:
                if (Random.Range(0, 2) == 0) {
                    skateAnim.SetBool("leftCrooked", true);
                }
                else {
                    skateAnim.SetBool("rightCrooked", true);
                }
                break;
            case 10:
                if (Random.Range(0, 2) == 0) {
                    skateAnim.SetBool("Salad", true);
                }
                else {
                    skateAnim.SetBool("Suski", true);
                }
                break;
            case 11:
                if (Random.Range(0, 2) == 0) {
                    skateAnim.SetBool("Feeble", true);
                }
                else {
                    skateAnim.SetBool("Smith", true);
                }
                break;
            case 12:
                if (Random.Range(0, 2) == 0) {
                    skateAnim.SetBool("Willy", true);
                }
                else {
                    skateAnim.SetBool("OverWilly", true);
                }
                break;
                default:
                skateAnim.SetBool("50-50", true); 
                break;


        }
    }
}
