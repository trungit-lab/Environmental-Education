using UnityEngine;
using UnityEngine.UI;

public class GameInstructions : MonoBehaviour
{
    [Header("UI")]
    public GameObject instructionPanel;
    public Text instructionText;
    public KeyCode toggleKey = KeyCode.H;
    
    [Header("Instructions")]
    [TextArea(10, 20)]
    public string instructions = @"HƯỚNG DẪN CHƠI GAME

=== DI CHUYỂN ===
WASD - Di chuyển
Space - Nhảy
Mouse - Nhìn xung quanh
L - Khóa/mở khóa chuột

=== TRỒNG CÂY ===
Tab - Mở menu chọn hạt giống
1-9 - Chọn loại hạt giống
Q - Trồng cây (nhìn xuống đất)
ESC - Đóng menu

=== CHĂM SÓC CÂY ===
E - Tưới nước (nhìn vào cây)
R - Bón phân (nhìn vào cây)

=== THÔNG TIN CÂY ===
Khi nhìn vào cây sẽ hiển thị:
- Tiến độ phát triển
- Độ ẩm
- Chất dinh dưỡng
- Giai đoạn hiện tại

=== MẸO ===
- Tưới nước thường xuyên để cây phát triển nhanh
- Bón phân để tăng tốc độ phát triển
- Ánh sáng mặt trời cũng ảnh hưởng đến tốc độ phát triển
- Cây sẽ tự động chuyển giai đoạn khi đủ điều kiện";

    private bool isShowingInstructions = false;
    
    void Start()
    {
        if (instructionPanel)
            instructionPanel.SetActive(false);
    }
    
    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleInstructions();
        }
        
        // Tự động ẩn sau 10 giây
        if (isShowingInstructions && Input.anyKeyDown && !Input.GetKeyDown(toggleKey))
        {
            HideInstructions();
        }
    }
    
    void ToggleInstructions()
    {
        if (isShowingInstructions)
        {
            HideInstructions();
        }
        else
        {
            ShowInstructions();
        }
    }
    
    void ShowInstructions()
    {
        isShowingInstructions = true;
        
        if (instructionPanel)
        {
            instructionPanel.SetActive(true);
        }
        
        if (instructionText)
        {
            instructionText.text = instructions;
        }
        
        // Tạm dừng game
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    void HideInstructions()
    {
        isShowingInstructions = false;
        
        if (instructionPanel)
        {
            instructionPanel.SetActive(false);
        }
        
        // Khôi phục game
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    
    // Hiển thị hướng dẫn khi bắt đầu game
    public void ShowInitialInstructions()
    {
        ShowInstructions();
        
        // Tự động ẩn sau 15 giây
        Invoke(nameof(HideInstructions), 15f);
    }
    
    // Hiển thị hướng dẫn ngắn
    public void ShowQuickTip(string tip, float duration = 3f)
    {
        if (instructionText)
        {
            instructionText.text = tip;
        }
        
        if (instructionPanel)
        {
            instructionPanel.SetActive(true);
        }
        
        Invoke(nameof(HideInstructions), duration);
    }
}
