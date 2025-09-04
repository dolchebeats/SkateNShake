using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotSpawner : MonoBehaviour {
    private int initAmount = 2;
    private float plotSize = 56f;
    private float lastZPos = 27.5f;
    public Transform player;
    private float despawnDistance = 30f;

    private List<GameObject> activePlots = new List<GameObject>();
    void Start() {
        player = GameManager.Instance.transform;


        for (int i = 0; i < initAmount; i++) {
            SpawnPlot(SpawnManager.Instance.CurrentThemeIndex);
        }
    }

    void Update() {
        DespawnBehindPlayer();
    }

    public void SpawnPlot(int themeIndex) {
        List<GameObject> theme = SpawnManager.Instance.themes[themeIndex].plots;

        GameObject plotLeft = Instantiate(theme[Random.Range(0, theme.Count)]);
        GameObject plotRight = Instantiate(theme[Random.Range(0, theme.Count)]);

        float zPos = lastZPos + plotSize;
        plotLeft.transform.position = new Vector3(0, 0, zPos);
        plotLeft.transform.rotation = Quaternion.identity;
        plotLeft.SetActive(true);

        plotRight.transform.position = new Vector3(0, 0, zPos);
        plotRight.transform.rotation = Quaternion.Euler(0, 180, 0);
        plotRight.SetActive(true);

        activePlots.Add(plotLeft);
        activePlots.Add(plotRight);

        lastZPos += plotSize;
    }

    private void DespawnBehindPlayer() {
        for (int i = activePlots.Count - 1; i >= 0; i--) {
            if (activePlots[i] != null &&
                activePlots[i].transform.position.z < player.position.z - despawnDistance) {

                Destroy(activePlots[i]); // or SetActive(false) if pooling
                activePlots.RemoveAt(i);
            }
        }
    }

}
