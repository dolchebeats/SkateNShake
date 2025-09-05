using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupHandler : MonoBehaviour
{
    public GameObject coin;
    public GameObject Coin3x;
    public GameObject Stopwatch;
    public GameObject JumpBoost;
    public GameObject Shield;
    // Start is called before the first frame update
    void Awake()
    {
        float chance = Random.Range(0f, 1f);
        if (chance < 0.5f) {
            Instantiate(coin,gameObject.transform);
        } else if (0.5f <= chance&&chance < 0.625f) {
            Instantiate(Stopwatch, gameObject.transform);
        } else if (0.625f <= chance&&chance < 0.75f) {
            Instantiate(JumpBoost, gameObject.transform);
        } else if (0.75 <= chance&chance < 0.875f) {
            Instantiate(Coin3x, gameObject.transform);
        } else {
            Instantiate(Shield, gameObject.transform);
        }
    }
}
