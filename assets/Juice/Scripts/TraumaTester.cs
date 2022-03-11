using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraumaTester : MonoBehaviour
{
    [SerializeField] private TraumaManager m_traumaManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            m_traumaManager.AddTrauma(Trauma.Traumas[Trauma.Level.Mild]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            m_traumaManager.AddTrauma(Trauma.Traumas[Trauma.Level.Medium]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            m_traumaManager.AddTrauma(Trauma.Traumas[Trauma.Level.Intense]);
        }
    }
}
