using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "LootTable", menuName = "Loot Table")]
public class LootTable : ScriptableObject
{
    [SerializeField]                private List<RewardItem> _items;
    [SerializeField] [Range(0,100)] private int _noDropChancePercentage = 50;
    [SerializeField]                private int _maxItemsDropped = 5;

    [System.NonSerialized] private bool isInitialized = false;

    private float _totalWeight;
    private float _initialNoDropChance;

    private void Initialize()
    {
        if (!isInitialized)
        {
            _items = _items.OrderBy(o => o.weight).ToList();
            _totalWeight = _items.Sum(item => item.weight);
            isInitialized = true;
        }
    }

    public RewardItem[] GetRandomItems()
    {
        Initialize();

        List<RewardItem> drops = new List<RewardItem>();

        int iterations = 0;
        while (true)
        {
            iterations++;
            if (iterations >= _maxItemsDropped) return drops.ToArray(); 
            if(Random.Range(0f, 100) < _noDropChancePercentage) return drops.ToArray();
            float rand = Random.Range(0f, _totalWeight);

            foreach(var item in _items)
            {
                if(item.weight >= rand)
                {
                    drops.Add(item);
                    if (!item.multiDrop) return drops.ToArray();
                    break;
                }
                rand -= item.weight;
            }
        }
    }
}

#region Reward Item
[System.Serializable]
public class RewardItem
{
    public string itemName;
    public float weight;
    public bool multiDrop = true;

    public RewardItem(string itemName, float weight, bool multiDrop)
    {
        this.itemName = itemName;
        this.weight = weight;
        this.multiDrop = multiDrop;
    }
}
#endregion