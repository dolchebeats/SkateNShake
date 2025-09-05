using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField] float speed;
    void Update()
    {
        if (!GameManager.Instance.isGameStarted) return;
        speed = PlayerMovement.Instance.movementSpeed / 5f;
        //transform.Translate(new Vector3(speed, 0, 0) * Time.deltaTime);
        transform.parent.Translate(new Vector3(0, 0, -speed) * Time.deltaTime);
    }
    

}
