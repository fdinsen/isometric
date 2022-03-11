using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraumaLevelDisplay : MonoBehaviour
{
    [SerializeField] private Image m_traumaImageBar;
    [SerializeField] private TraumaManager m_traumaManager;

    private void LateUpdate()
    {
        m_traumaImageBar.fillAmount = m_traumaManager.TraumaLevel;
    }
}
