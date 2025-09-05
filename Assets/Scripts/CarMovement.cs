using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CarMovement : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] private Material[] possibleMaterials; // assign in Inspector
    private Renderer rend;
    void Update()
    {
        if (!GameManager.Instance.isGameStarted) return;
        speed = PlayerMovement.Instance.movementSpeed / 5f;
        //transform.Translate(new Vector3(speed, 0, 0) * Time.deltaTime);
        transform.parent.Translate(new Vector3(0, 0, -speed) * Time.deltaTime);
    }

    void Start() {
        rend = GetComponent<Renderer>();
        if (rend == null || rend.materials.Length == 0) return;

        // Get a copy of current materials (important, never modify sharedMaterials directly)
        Material[] mats = rend.materials;

        // Replace only the first material with a random one
        if (possibleMaterials.Length > 0) {
            int randomIndex = Random.Range(0, possibleMaterials.Length);
            mats[0] = possibleMaterials[randomIndex];
        }

        // Apply updated array back
        rend.materials = mats;
    }

}
