using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineBrain cmBrain;

    public static CameraManager Instance;

    private List<CinemachineVirtualCamera> allCameras = new List<CinemachineVirtualCamera>();

    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple Camera Managers present in scene. Destroying duplicate");
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Cameras in scene: " + allCameras.Count);
    }

    public void AddVirtualCamera(CinemachineVirtualCamera cam)
    {
        allCameras.Add(cam);
    }

    public void RemoveVirtualCamera(CinemachineVirtualCamera cam)
    {
        allCameras.Remove(cam);
    }
}
