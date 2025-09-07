using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlantListUI : MonoBehaviour
{
    public GameObject contentList;
    public GameObject plantButtonPrefab;
    public Sprite[] plantSprites;
    
    private GameObject[] plantButtons;
    private TextMeshProUGUI[] countTexts;
    
    private void Awake()
    {
        plantSprites = Resources.LoadAll<Sprite>("Plant/PlantSprites");
    }
    
    private void Start()
    {
        CreatePlantButtons();
    }
    
    private void CreatePlantButtons()
    {
        int plantCount = PlantManager.Instance.plants.Count;
        plantButtons = new GameObject[plantCount];
        countTexts = new TextMeshProUGUI[plantCount];
        
        for (int i = 0; i < plantCount; i++)
        {
            GameObject plantButton = Instantiate(plantButtonPrefab, contentList.transform);
            plantButton.GetComponent<Image>().sprite = plantSprites[i];
            
            // Tìm Text component để hiển thị số lượng
            TextMeshProUGUI countText = plantButton.GetComponentInChildren<TextMeshProUGUI>();
            if (countText == null)
            {
                // Tạo Text component nếu chưa có
                GameObject textObj = new GameObject("CountText");
                textObj.transform.SetParent(plantButton.transform);
                countText = textObj.AddComponent<TextMeshProUGUI>();
                countText.fontSize = 12;
                countText.color = Color.white;
                countText.alignment = TextAlignmentOptions.Center;
                
                // Đặt vị trí text
                RectTransform textRect = countText.GetComponent<RectTransform>();
                textRect.anchorMin = new Vector2(0.7f, 0.7f);
                textRect.anchorMax = new Vector2(1f, 1f);
                textRect.offsetMin = Vector2.zero;
                textRect.offsetMax = Vector2.zero;
            }
            
            countTexts[i] = countText;
            plantButtons[i] = plantButton;
            
            // Lưu index để tránh closure problem
            int index = i;
            plantButton.GetComponent<Button>().onClick.AddListener(() => PlantManager.Instance.currentPlantIndex = index);
        }
        
        UpdatePlantCounts();
    }
    
    public void UpdatePlantCounts()
    {
        if (countTexts == null) return;
        
        for (int i = 0; i < countTexts.Length; i++)
        {
            if (countTexts[i] != null)
            {
                PlantType plantType = GetPlantTypeFromIndex(i);
                int seedCount = PlantManager.Instance.GetPlantCount(plantType);
                int plantedCount = PlantManager.Instance.GetPlantedCount(plantType);
                
                countTexts[i].text = $"{seedCount}\n{plantedCount}";
            }
        }
    }
    
    private PlantType GetPlantTypeFromIndex(int index)
    {
        switch (index)
        {
            case 0: return PlantType.TreeA;
            case 1: return PlantType.TreeB;
            case 2: return PlantType.TreeC;
            case 3: return PlantType.TreeD;
            default: return PlantType.TreeA;
        }
    }
}
