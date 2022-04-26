using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using FMODUnity;

namespace ExtensionMethods
{
    public static class ExtensionMethods
    {
        
    }

    public static class HelperMethods
    {
        public static Quaternion LookRotation2D(Vector3 dir)
        {
            Vector3 rotatedDirVector = Quaternion.Euler(0, 0, 90) * dir;
            var targetRotation = Quaternion.LookRotation(forward: Vector3.forward, rotatedDirVector);
            targetRotation = Quaternion.Euler(0, 0, targetRotation.eulerAngles.z);
            return targetRotation;
        }

        public static (int, int, int, int) ConvertGuidToData(FMOD.GUID guid)
        {
            return (guid.Data1, guid.Data2, guid.Data3, guid.Data4);
        }

        public static FMOD.GUID ConvertDataToGuid(int data1, int data2, int data3, int data4)
        {
            return new FMOD.GUID { Data1 = data1, Data2 = data2, Data3 = data3, Data4 = data4 };
        }
    }
}

