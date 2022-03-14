using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePoint_Rotation : MonoBehaviour
{
    public Rigidbody2D rb;
    public Camera cam;
    private Vector3 _startPos;
   

    Vector2 mousePos;
    PhotonView _view;
    // Update is called once per frame
    private void Start()
    {
        _view = GetComponent<PhotonView>();
        _startPos = transform.localPosition;
        cam = Camera.main;
    }

    void Update()
    {
        if(_view.IsMine)
        {
            mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            Vector2 lookDir = mousePos - rb.position;
            float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
            rb.rotation = angle;

            transform.localPosition = _startPos;
        }
    }
}
