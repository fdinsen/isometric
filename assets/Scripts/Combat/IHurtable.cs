using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHurtable
{
    public void DealDamage(int damage);

    public void DealDamage(int damage, Vector2 hitdir);
}
