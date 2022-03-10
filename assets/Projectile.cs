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

    // Update is called once per frame
    void FixedUpdate()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D col)
    {   
        //THIS IS BLACK MAGIC, THAT COMPARES THE LAYERS TO LAYERMASK
        if (((1<<col.gameObject.layer) & layers)!= 0)
        {
            Debug.Log("Hit Wall");
            //Add a Float to add time in seconds
            Destroy(gameObject);
        }
    }
}
