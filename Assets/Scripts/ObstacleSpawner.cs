using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour {
    private int initAmount = 4;
    private float plotSize = 14f;
    private float lastZPos = 7+14f;
    bool side;
    public GameObject player;

    public List<GameObject> coins;
    public List<GameObject> barriers;
    public List<GameObject> cars;
    public List<GameObject> trucks;
    void Start() {
        for (int i = 0; i < initAmount; i++) {
            SpawnObstacle();
        }

    }


    void Update() {

    }

    public void SpawnObstacle() {

        for (int i = 0; i < 4; i++) {
            float zPos = lastZPos + plotSize;
            float distance = GameManager.Instance.distance;
            float chance = Random.Range(0f, 1f);
            float coinC, obsC, carC;
            float sedanC, truckC;
                        
            if (distance > 1000f) {
                coinC = 0f;
                obsC = 0.2f;
                carC = 0.5f;

                sedanC = 0f;
                truckC = 0.5f;
            } else if (distance > 500f) {
                coinC = 0f;
                obsC = 0.3f;
                carC = 0.6f;

                sedanC = 0f;
                truckC = 0.7f;
            } else if (distance > 250f) {
                coinC = 0f;
                obsC = 0.32f;
                carC = 0.7f;

                sedanC = 0f;
                truckC = 0.8f;
            } else if (distance > 100f) {
                coinC = 0f;
                obsC = 0.35f;
                carC = 0.8f;

                sedanC = 0f;
                truckC = 0.9f;
            } else if (distance > 50f) {
                coinC = 0f;
                obsC = 0.38f;
                carC = 0.9f;

                sedanC = 0f;
                truckC = 1f;
            } else if (distance > 25f) {
                coinC = 0f;
                obsC = .4f;
                carC = 0.9f;

                sedanC = 0f;
                truckC = 1f;
            } else {
                coinC = 0f;
                obsC = 0.4f;
                carC = 1f;

                sedanC = 0f;
                truckC = 1f;
            }

            
            if (coinC < chance && chance < obsC) {

                if (Random.Range(0f,1f) > 0.5) {
                    GameObject obs = coins[Random.Range(0, coins.Count)];
                    Instantiate(obs, new Vector3(0, 0, zPos), obs.transform.rotation);
                }
                else {
                    GameObject obs = coins[Random.Range(0, coins.Count)];
                    Instantiate(obs, new Vector3(0, 0, zPos), new Quaternion(0, 180, 0, 0));
                }
            } else if (obsC < chance && chance < carC) {
                if (Random.Range(0f, 1f) > 0.5) {
                    GameObject obs = barriers[Random.Range(0, barriers.Count)];
                    Instantiate(obs, new Vector3(0, 0, zPos), obs.transform.rotation);
                }
                else {
                    GameObject obs = barriers[Random.Range(0, barriers.Count)];
                    Instantiate(obs, new Vector3(0, 0, zPos), new Quaternion(0, 180, 0, 0));
                }
            } else if (carC < chance) {
                chance = Random.Range(0f, 1f);
                if (sedanC < chance && chance < truckC) {
                    if (Random.Range(0, 2) > 0.5) {
                        GameObject obs = cars[Random.Range(0, cars.Count)];
                        Instantiate(obs, new Vector3(0, 0, zPos), obs.transform.rotation);
                    }
                    else {
                        GameObject obs = cars[Random.Range(0, cars.Count)];
                        Instantiate(obs, new Vector3(0, 0, zPos), new Quaternion(0, 180, 0, 0));
                    }
                } else if (truckC < chance) {
                    if (Random.Range(0, 2) > 0.5) {
                        GameObject obs = trucks[Random.Range(0, trucks.Count)];
                        Instantiate(obs, new Vector3(0, 0, zPos), obs.transform.rotation);
                    }
                    else {
                        GameObject obs = trucks[Random.Range(0, trucks.Count)];
                        Instantiate(obs, new Vector3(0, 0, zPos), new Quaternion(0, 180, 0, 0));
                    }
                }
                
            }

            lastZPos += plotSize;
        }
    }
}
