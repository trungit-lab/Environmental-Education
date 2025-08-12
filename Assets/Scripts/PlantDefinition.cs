using UnityEngine;
using System;

public enum PlantType { TreeA, TreeB, TreeC, TreeD }

[CreateAssetMenu(fileName = "PlantDefinition", menuName = "Plants/Plant Definition")]
public class PlantDefinition : ScriptableObject
{
    public PlantType type;

    [Tooltip("Tổng thời gian (giây) để đi từ 0 -> 100% nếu không có buff.")]
    public float baseTotalGrowTime = 300f;

    [Tooltip("Các giai đoạn trưởng thành theo tỉ lệ tiến trình (0..1). Sắp xếp tăng dần threshold.")]
    public GrowthStage[] stages;

    [Tooltip("VFX khi chuyển stage (tùy chọn).")]
    public GameObject stageTransitionVFX;
}

[Serializable]
public class GrowthStage
{
    public string name;
    [Range(0f, 1f)] public float threshold = 0f; // ví dụ: 0 (hạt), 0.3 (mầm), 0.7 (trung), 1.0 (trưởng thành)
    public GameObject prefab;     
    public Vector3 firstScale = Vector3.zero;
    public Vector3 targetScale = Vector3.one;    // scale mục tiêu của model stage này
}
