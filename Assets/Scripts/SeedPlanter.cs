using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
public class SeedPlanter : MonoBehaviour
{
    [Header("Planting")]
    public PlantDefinition[] availableSeeds;
    public KeyCode plantKey = KeyCode.Q;
    public float plantRange = 5f;
    public LayerMask groundLayerMask = -1;
    
    
    [Header("UI System")]
    public Transform systemPanel;              // SystemPanel transform để chứa buttons
    public GameObject seedButtonPrefab;        // Prefab cho button hạt giống
    public GameObject seedSelectionPanel;      // Panel chứa toàn bộ UI chọn hạt giống
    
    private Camera playerCamera;
    private AudioSource audioSource;
    private int currentSeedIndex = 0;
    private bool isSelectingSeed = false;
    
    void Start()
    {
        playerCamera = Camera.main;
        if (!playerCamera) playerCamera = FindObjectOfType<Camera>();
        
        audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
        
        // Ẩn UI ban đầu
        if (seedSelectionPanel)
            seedSelectionPanel.SetActive(false);
    }
    
    void Update()
    {
        // Mở/đóng menu chọn hạt giống
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleSeedSelection();
        }
        
        // Chọn hạt giống bằng số
        for (int i = 0; i < availableSeeds.Length && i < 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SelectSeed(i);
            }
        }
        
        // Trồng cây
        if (Input.GetKeyDown(plantKey) && !isSelectingSeed)
        {
            TryPlantSeed();
        }
        
        // Đóng menu bằng ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSeedSelection();
        }
    }
    
    void ToggleSeedSelection()
    {
        if (isSelectingSeed)
        {
            CloseSeedSelection();
        }
        else
        {
            OpenSeedSelection();
        }
    }
    
    void OpenSeedSelection()
    {
        isSelectingSeed = true;
        if (seedSelectionPanel)
        {
            seedSelectionPanel.SetActive(true);
            CreateSeedButtons();
        }
        
        // Tạm dừng player movement
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    void CloseSeedSelection()
    {
        isSelectingSeed = false;
        if (seedSelectionPanel)
            seedSelectionPanel.SetActive(false);
        
        // Khôi phục game
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void CreateSeedButtons()
    {
        if (!systemPanel || !seedButtonPrefab) 
        {
            Debug.LogWarning("SystemPanel hoặc SeedButtonPrefab chưa được assign!");
            return;
        }
        
        // Xóa buttons cũ trong SystemPanel
        foreach (Transform child in systemPanel)
        {
            if (child.CompareTag("SeedButton")) // Chỉ xóa buttons hạt giống
            {
                Destroy(child.gameObject);
            }
        }
        
        // Tạo buttons mới trong SystemPanel
        for (int i = 0; i < availableSeeds.Length; i++)
        {
            GameObject buttonGO = Instantiate(seedButtonPrefab, systemPanel);
            buttonGO.tag = "SeedButton"; // Tag để dễ quản lý
            
            // Lấy components
            Button button = buttonGO.GetComponent<Button>();
            Text buttonText = buttonGO.GetComponentInChildren<Text>();
            
            // Cập nhật text
            if (buttonText)
            {
                buttonText.text = $"{i + 1}. {availableSeeds[i].type}";
            }
            
            // Thêm event listener
            int seedIndex = i; // Capture index
            button.onClick.AddListener(() => SelectSeed(seedIndex));
            
            // Có thể thêm tooltip component sau này nếu cần
            
            Debug.Log($"Đã tạo button cho hạt giống: {availableSeeds[i].type}");
        }
    }
    
    void SelectSeed(int index)
    {
        if (index >= 0 && index < availableSeeds.Length)
        {
            currentSeedIndex = index;
            Debug.Log($"Đã chọn hạt giống: {availableSeeds[index].type}");
            CloseSeedSelection();
        }
    }
    
    void TryPlantSeed()
    {
        if (availableSeeds == null || availableSeeds.Length == 0) 
        {
            Debug.LogWarning("Không có hạt giống nào để trồng!");
            return;
        }
        
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        
        if (Physics.Raycast(ray, out hit, plantRange, groundLayerMask))
        {
            PlantSeed(hit.point, hit.normal);
        }
        else
        {
            Debug.Log("Không tìm thấy vị trí trồng cây! Hãy nhìn vào mặt đất.");
        }
    }
    
    void PlantSeed(Vector3 position, Vector3 normal)
    {
        PlantDefinition selectedSeed = availableSeeds[currentSeedIndex];
        
        // Tạo GameObject cho cây
        GameObject plantRoot = new GameObject($"Plant_{selectedSeed.type}");
        plantRoot.transform.position = position;
        plantRoot.transform.rotation = Quaternion.FromToRotation(Vector3.up, normal);
        
        // Thêm PlantGrowth component
        PlantGrowth growth = plantRoot.AddComponent<PlantGrowth>();
        growth.Configure(selectedSeed, 0.5f, 0.2f, null);
        
       
        
    }
    
    // Gizmos để debug
    void OnDrawGizmosSelected()
    {
        if (playerCamera)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(playerCamera.transform.position, plantRange);
        }
    }
    
    // Helper method để kiểm tra setup
    void OnValidate()
    {
       
    }
}
