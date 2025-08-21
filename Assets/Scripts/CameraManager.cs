using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    [SerializeField]
    private Transform player;
    private float yoffset= 3.5f; private float zoffset=-5;
    Animator anim;
    int animIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (animIndex == 2) {
            transform.position = new Vector3(player.position.x, player.position.y + yoffset, player.position.z + zoffset);
        }
        else if (animIndex == 1) {
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + (20*Time.deltaTime));
        }
    }

    public void StartAnimationBegin() {
        animIndex = 1;
    }

    public void StartAnimationEnd() {
        animIndex = 2;
    }
}
