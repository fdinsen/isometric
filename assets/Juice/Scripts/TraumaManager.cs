using System; 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraumaManager : MonoBehaviour
{
    public float TraumaLevel;

    private void Awake()
    {
        TraumaLevel = 0;        
    }

    private void FixedUpdate()
    {
        var res = TraumaLevel -= .01f;
        TraumaLevel = Mathf.Max(res, 0.0f);
    }

    public void AddTrauma(float adj) => TraumaLevel = TraumaLevel + adj > 1.0f ? 1.0f : TraumaLevel + adj;
}

public static class Trauma
{
    public enum Level { Mild, Medium, Intense };

    public static Dictionary<Level, float> Traumas = new Dictionary<Level, float>()
    {
        { Level.Mild, 0.3f },
        { Level.Medium, 0.6f },
        { Level.Intense, 0.99f },
    };
}
