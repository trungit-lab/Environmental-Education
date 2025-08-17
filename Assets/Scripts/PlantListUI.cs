using UnityEngine;
using UnityEngine.UI;

public class PlantListUI : MonoBehaviour
{
    public GameObject contentList;
    public GameObject plantButtonPrefab;
    public Sprite[] plantSprites;
    private void Awake()
    {
        plantSprites = Resources.LoadAll<Sprite>("Plant/PlantSprites");
    }
    private void Start()
    {
        for (int i = 0; i < PlantManager.Instance.plants.Count; i++)
        {
            GameObject plantButton = Instantiate(plantButtonPrefab, contentList.transform);
            plantButton.GetComponent<Image>().sprite = plantSprites[i];
            plantButton.GetComponent<Button>().onClick.AddListener(() => PlantManager.Instance.currentPlantIndex = i);
        }
    }
}
