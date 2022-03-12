using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePoint_Rotation : MonoBehaviour
{
    public Rigidbody2D rb;
    public Camera cam;
    private Vector3 _startPos;
   

    Vector2 mousePos;

    // Update is called once per frame
    private void Start()
    {
        _startPos = transform.localPosition;
    }

    void Update()
    {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;

        transform.localPosition = _startPos;

    }
}
