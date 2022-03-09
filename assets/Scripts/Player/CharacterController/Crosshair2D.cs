using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Crosshair2D : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private GameObject crosshair_object;

    private PhotonView view;

    private void Awake()
    {
        view = GetComponent<PhotonView>();

        if(!view.IsMine)
        {
            crosshair_object.SetActive(false);
        }
    }

    private void Update()
    {
        if (!view.IsMine) return;
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;
        crosshair_object.transform.position = mouseWorldPosition;
    }
}
