using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;

    private Vector3 spawnOffset = new Vector3(2, 0, 0);
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(enemyPrefab.name, transform.position, Quaternion.identity);
            PhotonNetwork.Instantiate(enemyPrefab.name, transform.position - spawnOffset, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
