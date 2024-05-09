using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public class WeightApplier
{
    public float storedWeight;
    [JsonIgnore] public float StoredWeight { get { return storedWeight; } }

    float minWeight = 0;
    public float maxWeight = 0.05f;
    public float weightMultiplier = 0.01f;
    public float weightDecreation = 0.0001f;

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
