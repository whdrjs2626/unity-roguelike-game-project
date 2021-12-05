using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnStone : MonoBehaviour
{
    public GameObject Spawner;
    public GameObject Stone;
    public GameManager gameManager;
    
    void Update()
    {
        transform.Rotate(Vector3.up * 20 * Time.deltaTime);
    }
    void OnTriggerEnter(Collider other) {
        if(other.tag == "Player") {
            gameManager.StageStart();
            Destroy(Stone);
            Spawner.SetActive(true);
        }
    }
}
