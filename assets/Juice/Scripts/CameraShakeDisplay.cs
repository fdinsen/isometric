using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraShakeDisplay : MonoBehaviour
{

    //[SerializeField] private TraumaManager m_traumaManager;
    [SerializeField] private CameraShakeManager m_cameraShakeManager;
    [SerializeField] private Image m_cameraShakeBar;

    public void FixedUpdate()
    {
        m_cameraShakeBar.fillAmount = m_cameraShakeManager.GetCameraShakeLevel();
    }
}
