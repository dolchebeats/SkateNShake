using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this; 
    }
    RoadSpawner roadSpawner;
    PlotSpawner plotSpawner;
    ObstacleSpawner obstacleSpawner;

    public List<ThemeData> themes;
    public int CurrentThemeIndex = 0;// { get; private set; } = 0;
    private int spawnCounter = 0;


    // Start is called before the first frame update
    void Start()
    {
        roadSpawner = GetComponent<RoadSpawner>();
        plotSpawner = GetComponent<PlotSpawner>();
        obstacleSpawner = GetComponent<ObstacleSpawner>();
        CurrentThemeIndex = Random.Range(0, themes.Count);
    }

    public void SpawnTriggerEntered() {
        
        roadSpawner.MoveRoad();
        plotSpawner.SpawnPlot(CurrentThemeIndex);
        obstacleSpawner.SpawnObstacle(CurrentThemeIndex);

        spawnCounter++;
        if (spawnCounter % 3 == 0) {
            CurrentThemeIndex = Random.Range(0, themes.Count);
        }
    }


}

[System.Serializable]
public class ThemeData {
    public string name;
    public List<GameObject> plots;
    public List<GameObject> obstacles;
}
