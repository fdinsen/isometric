using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerPrefab;

    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

     private void Awake()
    {
        Vector2 randomPosition = new Vector3(Random.Range(minX, maxX), gameObject.transform.position.y, Random.Range(minY, maxY));
        PhotonNetwork.Instantiate(playerPrefab.name, randomPosition, Quaternion.identity);
    }
}
