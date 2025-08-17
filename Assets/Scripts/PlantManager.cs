using UnityEngine;
using System.Collections.Generic;

public class PlantManager : MonoBehaviour
{
    public static PlantManager Instance;
    public List<GameObject> plants;

    public int currentPlantIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    public void PlantATree(Vector3 position)
    {
        Instantiate(plants[currentPlantIndex], position, Quaternion.identity);
        currentPlantIndex++;
        if (currentPlantIndex >= plants.Count)
        {
            currentPlantIndex = 0;
        }
    }
}
