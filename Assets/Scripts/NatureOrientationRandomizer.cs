using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NatureOrientationRandomizer : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        float scale = Random.Range(1f, 1.5f);
        transform.localScale = new Vector3(Random.Range(1, 1.5f), Random.Range(1, 1.5f), Random.Range(1, 1.5f));
        transform.rotation = new Quaternion(0, Random.Range(0, 360), 0, 0);
    }


}
