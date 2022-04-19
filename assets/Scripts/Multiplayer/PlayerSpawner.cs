using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Photon;

public class PlayerSpawner : MonoBehaviourPunCallbacks
{
    public GameObject playerPrefab;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

     private void Awake()
     {
        SpawnPlayer();
     }

    public void SpawnPlayer()
    {
        Vector2 randomPosition = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), gameObject.transform.position.y);
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
        GameManager.Instance?.MarkPlayerAsAlive();
    }
}
