using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse_Position2D : MonoBehaviour
{
    [SerializeField] private Camera mainCamera; 
    private void Update()
    {
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0f;
        transform.position = mouseWorldPosition;
    }
}
