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
    
    [Header("Effects")]
    public ParticleSystem plantEffect;
    public AudioClip plantSound;
    
    [Header("UI")]
    public GameObject seedSelectionUI;
    public Transform seedButtonContainer;
    public GameObject seedButtonPrefab;
    
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
        if (seedSelectionUI)
            seedSelectionUI.SetActive(false);
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
        if (seedSelectionUI)
        {
            seedSelectionUI.SetActive(true);
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
        if (seedSelectionUI)
            seedSelectionUI.SetActive(false);
        
        // Khôi phục game
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    void CreateSeedButtons()
    {
        if (!seedButtonContainer || !seedButtonPrefab) return;
        
        // Xóa buttons cũ
        foreach (Transform child in seedButtonContainer)
        {
            Destroy(child.gameObject);
        }
        
        // Tạo buttons mới
        for (int i = 0; i < availableSeeds.Length; i++)
        {
            GameObject buttonGO = Instantiate(seedButtonPrefab, seedButtonContainer);
            Button button = buttonGO.GetComponent<Button>();
            Text buttonText = buttonGO.GetComponentInChildren<Text>();
            
            if (buttonText)
                buttonText.text = $"{i + 1}. {availableSeeds[i].type}";
            
            int seedIndex = i; // Capture index
            button.onClick.AddListener(() => SelectSeed(seedIndex));
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
        if (availableSeeds == null || availableSeeds.Length == 0) return;
        
        RaycastHit hit;
        Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        
        if (Physics.Raycast(ray, out hit, plantRange, groundLayerMask))
        {
            PlantSeed(hit.point, hit.normal);
        }
        else
        {
            Debug.Log("Không tìm thấy vị trí trồng cây!");
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
        
        // Hiệu ứng trồng cây
        if (plantEffect)
        {
            plantEffect.transform.position = position;
            plantEffect.Play();
        }
        
        // Âm thanh
        if (plantSound && audioSource)
        {
            audioSource.PlayOneShot(plantSound);
        }
        
        Debug.Log($"Đã trồng cây {selectedSeed.type} tại {position}");
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
}
