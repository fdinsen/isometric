using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Cinemachine;

public class PlayerCameraHandler : MonoBehaviour
{
    [Header("Crosshair 2D")]
    [SerializeField]
    [Tooltip("The gameobject that should follow the mouse.")]
    private GameObject crosshair_object;

    [Header("Cinemachine Camera Following")]
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField]
    [Tooltip("Empty gameobject which the camera should follow.")]
    private GameObject followPointer;
    [SerializeField] private Vector2 xyClamping = new Vector2(3,2);
    
    
    private Camera mainCamera;
    private PhotonView view;

    //Cached variables
    private Vector3 cachedMouseWorldPosition = new Vector3();
    private Vector3 cachedFollowPointerPosition = new Vector3();
    private CinemachineBasicMultiChannelPerlin cachedCameraPerlin;
    private float shakeTimer;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        mainCamera = Camera.main;
        if (!view.IsMine)
        {
            crosshair_object.SetActive(false);
        }
    }

    private void Update()
    {
        if (!view.IsMine) return;
        cachedMouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        cachedMouseWorldPosition.z = 0f;
        crosshair_object.transform.position = cachedMouseWorldPosition;

        //Calculate position for follow pointer
        cachedFollowPointerPosition = (cachedMouseWorldPosition + transform.position) / 2;
        //followPointerPosition = (followPointerPosition + transform.position) / 2;
        followPointer.transform.position = cachedFollowPointerPosition;
        followPointer.transform.localPosition = ClampXY(followPointer.transform.localPosition, xyClamping);
    }

    private Vector3 ClampXY(Vector3 toClamp, Vector2 clampAt)
    {
        return new Vector3(
            Mathf.Clamp(toClamp.x, -clampAt.x, clampAt.x), //x
            Mathf.Clamp(toClamp.y, -clampAt.y, clampAt.y), //y
                        toClamp.z);                        //z
    }

    public void ShakeCamera(float intensity, float time, bool decreaseIntensityOverTime = true)
    {
        if(cachedCameraPerlin == null)
        {
            cachedCameraPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        cachedCameraPerlin.m_AmplitudeGain = intensity;
        if (decreaseIntensityOverTime) StartCoroutine(DecreaseShakeOverSeconds(time));
        else StartCoroutine(StopShakeAfter(time));
    }

    private IEnumerator StopShakeAfter(float time)
    {
        yield return new WaitForSeconds(time);
        cachedCameraPerlin.m_AmplitudeGain = 0;
    }

    private IEnumerator DecreaseShakeOverSeconds(float time)
    {
        shakeTimer = time;
        while(shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            cachedCameraPerlin.m_AmplitudeGain = 
                Mathf.Lerp(cachedCameraPerlin.m_AmplitudeGain, 0, 1 - (shakeTimer / time));
            yield return new WaitForEndOfFrame();
        }
        cachedCameraPerlin.m_AmplitudeGain = 0;
    }
}
