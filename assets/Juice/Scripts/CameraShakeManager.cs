using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    [SerializeField] private TraumaManager m_traumaManager;
    //[SerializeField] private int m_cameraShakePower;

    private enum CameraShakePower { Squared, Trippled}
    [SerializeField] private CameraShakePower m_cameraShakePower;

    private PerlinCameraShake m_perlinCameraShake;

    public float GetCameraShakeLevel() => Mathf.Pow(m_traumaManager.TraumaLevel, m_cameraShakePower == CameraShakePower.Squared ? 2 : 3);

    private void Start() => m_perlinCameraShake = Camera.main.GetComponent<PerlinCameraShake>();

    private void FixedUpdate() => m_perlinCameraShake.Trauma = GetCameraShakeLevel();
}
