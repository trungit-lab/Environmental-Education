using UnityEngine;

public class PlayerTools : MonoBehaviour
{
    [Header("Tools")]
    public GameObject wateringCan;
    public GameObject fertilizerBag;
    
    [Header("Settings")]
    public float interactionRange = 3f;
    public LayerMask plantLayerMask = -1;
    public KeyCode waterKey = KeyCode.E;
    public KeyCode fertilizerKey = KeyCode.R;
    
    [Header("Effects")]
    public ParticleSystem waterEffect;
    public ParticleSystem fertilizerEffect;
    
    [Header("UI")]
    public PlantUI plantUI;
    
    private Camera playerCamera;
    private PlantGrowth currentTargetPlant;
    
    void Start()
    {
        playerCamera = Camera.main;
        if (!playerCamera) playerCamera = FindObjectOfType<Camera>();
        
        // Ẩn tools ban đầu
        if (wateringCan) wateringCan.SetActive(false);
        if (fertilizerBag) fertilizerBag.SetActive(false);
    }
    
    void Update()
    {
        // Tìm cây gần nhất trong tầm nhìn
        FindNearestPlant();
        
        // Hiển thị UI khi nhìn vào cây
        if (plantUI)
        {
            plantUI.SetTargetPlant(currentTargetPlant);
        }
        
        // Xử lý input
        if (Input.GetKeyDown(waterKey))
        {
            UseWateringCan();
        }
        
        if (Input.GetKeyDown(fertilizerKey))
        {
            UseFertilizer();
        }
    }
    
    void FindNearestPlant()
    {
        if (!playerCamera) return;
        
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        
        if (Physics.Raycast(ray, out hit, interactionRange, plantLayerMask))
        {
            PlantGrowth plant = hit.collider.GetComponent<PlantGrowth>();
            if (plant != currentTargetPlant)
            {
                currentTargetPlant = plant;
            }
        }
        else
        {
            currentTargetPlant = null;
        }
    }
    
    void UseWateringCan()
    {
        if (!currentTargetPlant) return;
        
        // Hiệu ứng tưới nước
        if (waterEffect)
        {
            waterEffect.transform.position = currentTargetPlant.transform.position;
            waterEffect.Play();
        }
        
        // Tưới nước cho cây
        currentTargetPlant.ApplyWater(0.3f);
        
        // Animation tool (có thể thêm)
        if (wateringCan)
        {
            StartCoroutine(ShowToolAnimation(wateringCan));
        }
        
        Debug.Log($"Đã tưới nước cho cây! Moisture: {currentTargetPlant.moisture:F2}");
    }
    
    void UseFertilizer()
    {
        if (!currentTargetPlant) return;
        
        // Hiệu ứng bón phân
        if (fertilizerEffect)
        {
            fertilizerEffect.transform.position = currentTargetPlant.transform.position;
            fertilizerEffect.Play();
        }
        
        // Bón phân cho cây
        currentTargetPlant.ApplyFertilizer(0.4f);
        
        // Animation tool (có thể thêm)
        if (fertilizerBag)
        {
            StartCoroutine(ShowToolAnimation(fertilizerBag));
        }
        
        Debug.Log($"Đã bón phân cho cây! Nutrients: {currentTargetPlant.nutrients:F2}");
    }
    
    System.Collections.IEnumerator ShowToolAnimation(GameObject tool)
    {
        if (!tool) yield break;
        
        tool.SetActive(true);
        yield return new WaitForSeconds(1f);
        tool.SetActive(false);
    }
    
    // Gizmos để debug
    void OnDrawGizmosSelected()
    {
        if (playerCamera)
        {
            Gizmos.color = Color.blue;
            Vector3 center = playerCamera.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, interactionRange));
            Gizmos.DrawWireSphere(playerCamera.transform.position, interactionRange);
        }
    }
}
