using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class IWeapon : MonoBehaviour
{
    public abstract void Shoot(Action onShoot);
    public abstract void Shoot(Vector3 dir, Action onShoot);
    public abstract void Shoot(Vector3 dir);
    public abstract void Shoot();
}
