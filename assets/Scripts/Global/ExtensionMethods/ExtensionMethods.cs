using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }
}

