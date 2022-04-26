using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using FMODUnity;
using ExtensionMethods;

public class AudioPlayer : MonoBehaviour
{
    public void PlayOneshotRPC(FMOD.GUID guid, Vector3 position, PhotonView view, RpcTarget target)
    {
        (int data1, int data2, int data3, int data4) = HelperMethods.ConvertGuidToData(guid);
        view.RPC("PlayOneShot", target, data1, data2, data3, data4, position);
    }

    [PunRPC]
    protected void PlayOneShot(int data1, int data2, int data3, int data4, Vector3 position)
    {
        RuntimeManager.PlayOneShot(HelperMethods.ConvertDataToGuid(data1, data2, data3, data4), position);
    }

    public void PlayOneshotAttachedRPC(FMOD.GUID guid, PhotonView view, RpcTarget target)
    {
        (int data1, int data2, int data3, int data4) = HelperMethods.ConvertGuidToData(guid);
        view.RPC("PlayOneshotAttached", target, data1, data2, data3, data4);
    }

    [PunRPC]
    protected void PlayOneshotAttached(int data1, int data2, int data3, int data4)
    {
        RuntimeManager.PlayOneShotAttached(HelperMethods.ConvertDataToGuid(data1, data2, data3, data4), gameObject);
    }  
}
