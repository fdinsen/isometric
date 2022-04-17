using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDropping : MonoBehaviour
{
    [SerializeField] private LootTable _loottable;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<EnemyHealth>().ThisEnemyDied += DropItems;
    }

    private void DropItems(EnemyHealth.EnemyHealthEventArgs e)
    {
        if (!e.hasBeenHitByActivePlayer) return;
        var items = _loottable.GetRandomItems();
        foreach(var item in items)
        {
            var itemObj = Resources.Load("DroppableItems/" + item.itemName);
            Vector3 direction = Random.insideUnitCircle.normalized;
            Instantiate(itemObj, transform.position + (direction * 0.5f), Quaternion.identity);
        }
    }
}
