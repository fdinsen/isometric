using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Test : MonoBehaviour
{

    private PhotonView view;
    private Rigidbody m_Rigidbody;
    // Start is called before the first frame update
    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        if (view.IsMine)
        {
            Move();
        }
    }

    void Move()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            m_Rigidbody.AddForce(new Vector3(0, 500, 0));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            m_Rigidbody.AddForce(new Vector3(-100, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            m_Rigidbody.AddForce(new Vector3(100, 0, 0));
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            m_Rigidbody.AddForce(new Vector3(0, 0, 100));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            m_Rigidbody.AddForce(new Vector3(0, 0, -100));
        }
    }
}
