using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotSpawner : MonoBehaviour {
    private int initAmount = 5;
    private float plotSize = 55f;
    private float lastZPos = 27.5f;

    public List<GameObject> plots;
    //public List<GameObject> plots2;
    //public List<GameObject> plots3;
    void Start() {
        spawnStartScene();
        for (int i = 0; i < initAmount; i++) {
            SpawnPlot();
        }

    }


    void Update() {

    }

    public void SpawnPlot() {
        GameObject plotleft = plots[Random.Range(0, plots.Count)];
        GameObject plotright = plots[Random.Range(0, plots.Count)];

        float zPos = lastZPos + plotSize;
        Instantiate(plotleft, new Vector3(0, 0, zPos), plotleft.transform.rotation);
        Instantiate(plotright, new Vector3(0, 0, zPos), new Quaternion(0, 180, 0, 0));

        lastZPos += plotSize;
    }

    public void spawnStartScene() {
        GameObject startplot1 = plots[Random.Range(0, plots.Count)];
        GameObject startplot2 = plots[Random.Range(0, plots.Count)];
        Instantiate(startplot1, new Vector3(0,0,-28), startplot2.transform.rotation);
        Instantiate(startplot1, new Vector3(0, 0, 28), startplot2.transform.rotation);
    }
}
