using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {
    [Header("Settings")]
    private int initAmount = 2;
    private float plotSize = 14f;
    private float lastZPos = 21f; // 7 + 14
    [SerializeField] private float despawnDistance = 20f;
    private Transform player;
    private List<GameObject> activeObstacles = new List<GameObject>();

    void Start() {
        player = GameManager.Instance.transform;

        for (int i = 0; i < initAmount; i++) {
            SpawnObstacle(SpawnManager.Instance.CurrentThemeIndex);
        }
    }

    void Update() {
        DespawnBehindPlayer();
    }

    public void SpawnObstacle(int themeIndex) {
        for (int i = 0; i < 4; i++) {
            float zPos = lastZPos + plotSize;


                List<GameObject> theme = SpawnManager.Instance.themes[themeIndex].obstacles;

                GameObject obj = Instantiate(theme[Random.Range(0,theme.Count)]);
                obj.transform.position = new Vector3(0, 0, zPos);
                obj.transform.rotation = (Random.value > 0.5f) ? Quaternion.identity : Quaternion.Euler(0, 180, 0);

            activeObstacles.Add(obj);
            lastZPos += plotSize;
        }
    }

    private void DespawnBehindPlayer() {
        for (int i = activeObstacles.Count - 1; i >= 0; i--) {
            if (activeObstacles[i] != null &&
                activeObstacles[i].transform.position.z < player.position.z - despawnDistance) {

                Destroy(activeObstacles[i]); // or SetActive(false) if pooling
                activeObstacles.RemoveAt(i);
            }
        }
    }
}
