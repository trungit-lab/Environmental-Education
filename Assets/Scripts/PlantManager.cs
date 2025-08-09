using UnityEngine;
using System.Collections.Generic;

public class PlantManager : MonoBehaviour
{
    public List<Plant> plants = new List<Plant>();

    void Update()
    {
        foreach (var plant in plants)
        {
            plant.UpdateGrowth(Time.deltaTime);
        }
    }

    public void WaterAll(float amount)
    {
        foreach (var plant in plants)
        {
            plant.Water(amount);
        }
    }
}
