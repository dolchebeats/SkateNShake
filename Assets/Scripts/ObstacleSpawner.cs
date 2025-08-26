using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {
    [Header("Settings")]
    private int initAmount = 2;
    private float plotSize = 14f;
    private float lastZPos = 21f; // 7 + 14
    private int maxActive = 25;   // cap active obstacles

    [Header("Obstacle Pools")]
    public List<GameObject> coins;
    public List<GameObject> barriers;
    public List<GameObject> cars;
    public List<GameObject> trucks;

    private List<GameObject> activeObstacles = new List<GameObject>();
    private Dictionary<GameObject, Queue<GameObject>> pool = new Dictionary<GameObject, Queue<GameObject>>();
    private Dictionary<GameObject, GameObject> prefabLookup = new Dictionary<GameObject, GameObject>(); // maps instance -> prefab

    void Start() {
        InitPool(coins);
        InitPool(barriers);
        InitPool(cars);
        InitPool(trucks);

        for (int i = 0; i < initAmount; i++) {
            SpawnObstacle();
        }
    }

    void InitPool(List<GameObject> prefabs) {
        foreach (var prefab in prefabs) {
            if (!pool.ContainsKey(prefab)) {
                pool[prefab] = new Queue<GameObject>();
            }
        }
    }

    GameObject GetFromPool(GameObject prefab) {
        GameObject obj;
        if (pool[prefab].Count > 0) {
            obj = pool[prefab].Dequeue();
            obj.SetActive(true);
        }
        else {
            obj = Instantiate(prefab);
        }
        prefabLookup[obj] = prefab; // track which prefab this instance belongs to
        return obj;
    }

    void ReturnToPool(GameObject obj) {
        if (prefabLookup.TryGetValue(obj, out GameObject prefab)) {
            obj.SetActive(false);
            pool[prefab].Enqueue(obj);
            prefabLookup.Remove(obj);
        }
        else {
            Debug.LogWarning("Trying to return object without known prefab: " + obj.name);
            Destroy(obj); // fallback
        }
    }
    void Update() {

    }

    public void SpawnObstacle() {
        for (int i = 0; i < 4; i++) {
            float zPos = lastZPos + plotSize;
            float chance = Random.Range(0f, 1f);
            float distance = GameManager.Instance.distance;

            // difficulty scaling
            float coinC, obsC, carC, sedanC, truckC;
            if (distance > 1000f) { coinC = 0f; obsC = 0.2f; carC = 0.5f; sedanC = 0f; truckC = 0.5f; }
            else if (distance > 500f) { coinC = 0f; obsC = 0.3f; carC = 0.6f; sedanC = 0f; truckC = 0.7f; }
            else if (distance > 250f) { coinC = 0f; obsC = 0.32f; carC = 0.7f; sedanC = 0f; truckC = 0.8f; }
            else if (distance > 100f) { coinC = 0f; obsC = 0.35f; carC = 0.8f; sedanC = 0f; truckC = 0.9f; }
            else if (distance > 50f) { coinC = 0f; obsC = 0.38f; carC = 0.9f; sedanC = 0f; truckC = 1f; }
            else { coinC = 0f; obsC = 0.4f; carC = 1f; sedanC = 0f; truckC = 1f; }

            GameObject chosenPrefab = null;

            if (chance < obsC) {
                chosenPrefab = coins[Random.Range(0, coins.Count)];
            }
            else if (chance < carC) {
                chosenPrefab = barriers[Random.Range(0, barriers.Count)];
            }
            else {
                float carChance = Random.Range(0f, 1f);
                chosenPrefab = (carChance < truckC) ? cars[Random.Range(0, cars.Count)] : trucks[Random.Range(0, trucks.Count)];
            }

            if (chosenPrefab != null) {
                GameObject obj = GetFromPool(chosenPrefab);
                obj.transform.position = new Vector3(0, 0, zPos);
                obj.transform.rotation = (Random.value > 0.5f) ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

                activeObstacles.Add(obj);

                // cleanup oldest if too many
                if (activeObstacles.Count > maxActive) {
                    var old = activeObstacles[0];
                    activeObstacles.RemoveAt(0);
                    ReturnToPool(old);
                }
            }

            lastZPos += plotSize;
        }
    }
}
