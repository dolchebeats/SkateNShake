using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    float speed;
    void Update()
    {
        if (!GameManager.Instance.isGameStarted) return;
        speed = PlayerMovement.Instance.movementSpeed / 10f;
        transform.Translate(new Vector3(speed, 0, 0) * Time.deltaTime);
    }
    
    /*
    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Obstacle")||collision.gameObject.CompareTag("Grindable")) {

            //add smoke frsom engine
            //Destroy(this);
        }
    }*/

}
