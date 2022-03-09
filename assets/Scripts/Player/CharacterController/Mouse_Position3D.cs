using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse_Position3D : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] LayerMask layerMask;
    

    private void Update()
    {
       Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layerMask))
        {
            transform.position = raycastHit.point;
        }
    
        
        
    }
}
