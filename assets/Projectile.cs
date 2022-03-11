using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] private LayerMask layers;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (((1<<col.gameObject.layer) & layers)!= 0) //THIS IS BLACK MAGIC, THAT COMPARES THE LAYERS TO LAYERMASK
        {

            Destroy(gameObject);
        }
    }

    
}
