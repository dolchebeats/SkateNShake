using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupHandler : MonoBehaviour
{
    public GameObject coin;
    public GameObject Coin3x;
    public GameObject Stopwatch;
    public GameObject JumpBoost;
    // Start is called before the first frame update
    void Awake()
    {
        float chance = Random.Range(0f, 1f);
        if (chance < 0.9f) {
            Instantiate(coin,gameObject.transform);
        } else if (0.9f <= chance&&chance < 0.93f) {
            Instantiate(Stopwatch, gameObject.transform);
        } else if (0.93f <= chance&&chance < 0.96f) {
            Instantiate(JumpBoost, gameObject.transform);
        } else if (0.96 <= chance&chance< 1f) {
            Instantiate(Coin3x, gameObject.transform);
        } 

    }

}
