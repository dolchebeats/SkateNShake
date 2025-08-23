using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class PlayerMovement : MonoBehaviour {
    public static PlayerMovement Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public float movementSpeed = 15f;

    [SerializeField]
    float hSpeed;
    [SerializeField]
    float vSpeed;
    float hInput = 0;
    [SerializeField]
    Vector3 jump= Vector3.up;
    [SerializeField]
    public float jumpForce = 12;
    float currentSteerInput;


    Vector3 oldPos;
    bool land=true;
    public bool isGrinding;
    public bool recentJump;
    public  bool isKicking;

    public Rigidbody rb;

    void Start() {
        rb = GetComponent<Rigidbody>();
    }
    void ProcessInputs() {
        if (GameManager.Instance.isInControl && !isGrinding) {
            float time = 4f * Time.deltaTime;
            int frameTouches = 0;
            foreach (Touch touch in Input.touches) {

                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary) {
                    if (touch.position.x < Screen.width / 2)
                        hInput = hInput - time;
                    if (touch.position.x > Screen.width / 2)
                        hInput = hInput + time;
                }
                frameTouches++;

            }
            if (frameTouches == 0) {
                if (hInput > 0)
                    hInput = hInput - time;
                if (hInput < 0)
                    hInput = hInput + time;

            }
            hInput = Mathf.Clamp(hInput, -1.0f, 1.0f);
            //Debug.Log(hInput); remove this line before luanching pls vvvvvvvvvvvvvvvvvs
            //hInput = SwipeManager.SteeringInput;
            float targetInput = SwipeManager.SteeringInput;
            currentSteerInput = Mathf.MoveTowards(currentSteerInput, targetInput, Time.deltaTime * 5f);
            hInput = currentSteerInput;
            if (isGrinding)
                hInput = 0;
            
        }
    }
    public void Jump() {
        EndGrind();
        isKicking = false;
        StartCoroutine(RecentJump());
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(jump * jumpForce, ForceMode.Impulse);
        land = false;
    }
    void Update() {

        ProcessInputs();
        
        hSpeed = hInput * movementSpeed / 2;
        vSpeed = movementSpeed;

        

        if (Mathf.Abs(transform.position.x) > 7.5f) {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, -7.5f, 7.5f), oldPos.y, oldPos.z);
            transform.Translate(new Vector3(0, 0, vSpeed) * Time.deltaTime);
            
        }
        else if (isGrounded()&&!isGrinding) {
            transform.Translate(new Vector3(hSpeed, 0, vSpeed) * Time.deltaTime);
            if (GameManager.Instance.isInControl)
                transform.eulerAngles = new Vector3(0, hInput * 10f, 0);
        } else {
            transform.Translate(new Vector3(hSpeed/5, 0, vSpeed) * Time.deltaTime);
            transform.eulerAngles = new Vector3(0, hInput * 10f/5f, 0);
        }


        if (recentJump && Mathf.Abs(hInput) > 0.1f) {
            TrickManager.Instance.TrySpin(hInput);
            recentJump = false;
        }

        //if (isGrounded() && land == false) {
        //  FindObjectOfType<AudioManager>().PlaySound("Land");
        // land = true;

        //}
        if ((isGrounded() || isGrinding ) && !AudioManager.Instance.sounds[2].source.isPlaying) {
            AudioManager.Instance.PlaySound("Skate");
            if (land == false)
                AudioManager.Instance.PlaySound("Land");
        } else if (!isGrounded() && !isGrinding) {
            AudioManager.Instance.StopSound("Skate");
       }
        oldPos = transform.position;
        
    }

    public IEnumerator RecentJump () {
        recentJump = true;
        yield return new WaitForSeconds(0.4f);
        recentJump = false;
    }

    public bool isGrounded() {
        return Physics.Raycast(transform.position, Vector3.down, 0.01f);
    }

    private void OnTriggerEnter(Collider collider) {
        if (collider.CompareTag("SpawnTrigger")) {
            SpawnManager.Instance.SpawnTriggerEntered();
            movementSpeed += 0.25f;
        }
        if ((collider.CompareTag("Grindable")) && GameManager.Instance.isAlive && !isGrounded()) {
            GrindSurface(collider.gameObject);
        }

    }
    public void OnTriggerExit(Collider collider) {
        if ((collider.gameObject.CompareTag("Grindable")) && GameManager.Instance.isAlive) {
            EndGrind();
        }
    }


    public void OnCollisionEnter(Collision collider) {
        if (collider.gameObject.CompareTag("Kicker") && GameManager.Instance.isAlive) {
            StartCoroutine("Kick");
        }
                        
    }

    


    public IEnumerator Kick() {
        isKicking = true;
        yield return new WaitForSeconds(0.4f);
        isKicking = false;
    }
    void GrindSurface(GameObject surface) {

        GameManager.Instance.isInControl = false;
        isGrinding = true;
        rb.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotationX;
        transform.position = new Vector3(surface.transform.position.x, surface.transform.position.y, transform.position.z); 
        transform.eulerAngles = new Vector3 (transform.eulerAngles.x,0,transform.eulerAngles.z);
        TrickManager.Instance.ChooseGrind(); // <-- new

    }
    void EndGrind() {
        GameManager.Instance.isInControl = true;
        isGrinding = false;
        rb.constraints = RigidbodyConstraints.None;;
        AnimationManager.Instance.EndGrind();
    }


}
