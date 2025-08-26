using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlotSpawner : MonoBehaviour {
    private int initAmount = 5;
    private float plotSize = 56f;
    private float lastZPos = 27.5f;
    public Transform player;

    [Header("Themed Plot Lists")]
    public List<GameObject> naturePlots;
    public List<GameObject> suburbsPlots;
    public List<GameObject> townPlots;

    public List<GameObject> cityPlots;
    public List<GameObject> downtownPlots;

    private List<List<GameObject>> allThemes = new List<List<GameObject>>();
    private Dictionary<int, Queue<GameObject>> pooledPlots = new Dictionary<int, Queue<GameObject>>();
    private Queue<GameObject> activePlots = new Queue<GameObject>();

    // NEW: map active plots to their theme
    private Dictionary<GameObject, int> plotThemeLookup = new Dictionary<GameObject, int>();

    private int maxActivePlots = 10;
    private int spawnCounter = 0;
    public int currentThemeIndex = 0;

    void Start() {
        player = GameManager.Instance.transform;

        allThemes.Add(naturePlots);
        allThemes.Add(suburbsPlots);
        allThemes.Add(townPlots);
        allThemes.Add(cityPlots);
        allThemes.Add(downtownPlots);
        currentThemeIndex = Random.Range(0, allThemes.Count);
        for (int i = 0; i < allThemes.Count; i++) {
            pooledPlots[i] = new Queue<GameObject>();
        }

        for (int i = 0; i < initAmount; i++) {
            SpawnPlot();
        }
    }

    void Update() {
        if (activePlots.Count > 0) {
            GameObject oldestPlot = activePlots.Peek();
            if (player.position.z - oldestPlot.transform.position.z > plotSize * 2) {
                RecyclePlot();
            }
        }

        while (activePlots.Count < maxActivePlots) {
            SpawnPlot();
        }
    }

    public void SpawnPlot() {
        spawnCounter++;

        if (spawnCounter % 5 == 0) {
            currentThemeIndex = Random.Range(0, allThemes.Count);
        }

        List<GameObject> theme = allThemes[currentThemeIndex];

        GameObject plotLeft = GetPlotFromPool(theme, currentThemeIndex);
        GameObject plotRight = GetPlotFromPool(theme, currentThemeIndex);

        float zPos = lastZPos + plotSize;
        plotLeft.transform.position = new Vector3(0, 0, zPos);
        plotLeft.transform.rotation = Quaternion.identity;
        plotLeft.SetActive(true);

        plotRight.transform.position = new Vector3(0, 0, zPos);
        plotRight.transform.rotation = Quaternion.Euler(0, 180, 0);
        plotRight.SetActive(true);

        activePlots.Enqueue(plotLeft);
        activePlots.Enqueue(plotRight);

        // track which theme these plots belong to
        plotThemeLookup[plotLeft] = currentThemeIndex;
        plotThemeLookup[plotRight] = currentThemeIndex;

        lastZPos += plotSize;
    }

    private GameObject GetPlotFromPool(List<GameObject> theme, int themeIndex) {
        if (pooledPlots[themeIndex].Count > 0) {
            return pooledPlots[themeIndex].Dequeue();
        }
        else {
            return Instantiate(theme[Random.Range(0, theme.Count)]);
        }
    }

    private void RecyclePlot() {
        GameObject oldPlot = activePlots.Dequeue();
        oldPlot.SetActive(false);

        // find which theme this plot belonged to
        if (plotThemeLookup.TryGetValue(oldPlot, out int themeIndex)) {
            pooledPlots[themeIndex].Enqueue(oldPlot);
        }
        else {
            // fallback: just dump it into current theme pool
            pooledPlots[currentThemeIndex].Enqueue(oldPlot);
        }
    }
}
