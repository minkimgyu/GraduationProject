using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WeightApplier
{
    float storedWeight;

    public float StoredWeight { get { return storedWeight; } }

    float minWeight = 0;

    [SerializeField]
    float maxWeight = 0.05f;

    [SerializeField]
    float weightMultiplier = 0.01f;
    [SerializeField]
    float weightDecreation = 0.0001f;

    public void MultiplyWeight()
    {
        storedWeight += weightMultiplier;
        if (maxWeight < storedWeight) storedWeight = maxWeight;
    }

    void DecreaseWeight()
    {
        storedWeight -= weightDecreation;
        if (storedWeight < minWeight)
        {
            storedWeight = minWeight;
        }
    }

    public void OnUpdate()
    {
        DecreaseWeight();
    }
}
