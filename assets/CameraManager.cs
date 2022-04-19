using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineBrain cmBrain;

    public static CameraManager Instance;

    private List<CinemachineVirtualCamera> allCameras = new List<CinemachineVirtualCamera>();

    private int spectatingIndex = -1;
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

    public void SpectateNext()
    {
        if(allCameras.Count == 0)
        {
            Debug.LogWarning("No cameras in scene to spectate.");
            return;
        }
        if(spectatingIndex+1 >= allCameras.Count)
        {
            spectatingIndex = 0;
        }else
        {
            spectatingIndex++;
        }
        Spectate(spectatingIndex);
    }

    private void Spectate(int index)
    {
        for(int i = 0; i < allCameras.Count; i++)
        {
            allCameras[i].Priority = 1;
        }
        allCameras[index].Priority = 10;
    }

    public void AddVirtualCamera(CinemachineVirtualCamera cam)
    {
        allCameras.Add(cam);
    }

    public void RemoveVirtualCamera(CinemachineVirtualCamera cam)
    {
        allCameras.Remove(cam);
        spectatingIndex = -1;
    }
}
