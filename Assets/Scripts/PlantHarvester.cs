using UnityEngine;

public class PlantHarvester : MonoBehaviour
{
    [Header("Harvesting")]
    public bool playerInRange = false;
    public float harvestRange = 3f;
    public KeyCode harvestKey = KeyCode.Mouse0;
    
    [Header("UI")]
    public string harvestText = "Thu hoạch";
    
    private PlantGrowth plantGrowth;
    private SelectionManager selectionManager;
    
    void Start()
    {
        plantGrowth = GetComponent<PlantGrowth>();
        selectionManager = FindObjectOfType<SelectionManager>();
        
        // Thêm trigger collider nếu chưa có
        if (GetComponent<Collider>() == null)
        {
            SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = harvestRange;
        }
    }
    
    void Update()
    {
        // Kiểm tra input để thu hoạch
        if (Input.GetKeyDown(harvestKey) && playerInRange)
        {
            HarvestPlant();
        }
    }
    
    void HarvestPlant()
    {
        if (plantGrowth != null && PlantManager.Instance != null)
        {
            PlantManager.Instance.DestroyPlant(plantGrowth);
        }
        else
        {
            // Fallback: destroy trực tiếp nếu không có PlantManager
            Destroy(gameObject);
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            
            // Thông báo cho SelectionManager
            if (selectionManager != null)
            {
                selectionManager.SetInteractableObject(this);
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            
            // Thông báo cho SelectionManager
            if (selectionManager != null)
            {
                selectionManager.ClearInteractableObject();
            }
        }
    }
    
    public string GetHarvestText()
    {
        if (plantGrowth != null && plantGrowth.definition != null)
        {
            return $"Thu hoạch {plantGrowth.definition.type}";
        }
        return harvestText;
    }
    
    // Gizmos để debug
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, harvestRange);
    }
}
