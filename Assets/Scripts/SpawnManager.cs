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
    
    // Start is called before the first frame update
    void Start()
    {
        roadSpawner = GetComponent<RoadSpawner>();
        plotSpawner = GetComponent<PlotSpawner>();
        obstacleSpawner = GetComponent<ObstacleSpawner>();

    }

    public void SpawnTriggerEntered() {

        roadSpawner.MoveRoad();
        plotSpawner.SpawnPlot();
        obstacleSpawner.SpawnObstacle();
    }


}
