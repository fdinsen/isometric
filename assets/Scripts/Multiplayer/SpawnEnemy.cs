using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private bool ActivateEnemySpawning = true;
    [SerializeField] private float secondsBetweenSpawns = 10f;
    [SerializeField] private int numberOfEnemiesToSpawn = 100;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private Vector3[] _spawnLocations;


    private Vector3 spawnOffset = new Vector3(2, 0, 0);

    
    // Start is called before the first frame update
    void Start()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(SpawnEnemiesLoop());
        }
    }

    public void SpawnOneEnemy()
    {
        Vector3 randomLocation;
        if (_spawnLocations.Length == 0)
        {
            randomLocation = transform.position;
        }
        else
        {
            randomLocation = _spawnLocations[Random.Range(0, _spawnLocations.Length)];
        }

        PhotonNetwork.Instantiate(enemyPrefab.name, randomLocation, Quaternion.identity);
    }

    public IEnumerator SpawnEnemiesLoop()
    {
        int enemiesSpawned = 0;
        while(enemiesSpawned < numberOfEnemiesToSpawn)
        {
            SpawnOneEnemy();
            enemiesSpawned++;
            yield return new WaitForSeconds(secondsBetweenSpawns);
        }
        Debug.Log("FINISHED");
    }
}
