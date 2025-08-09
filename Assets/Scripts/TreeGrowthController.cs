using UnityEngine;
using System.Collections;

public class TreeGrowthController : MonoBehaviour
{
    public TreeData treeData; // Gắn config ở Inspector
    private int currentStageIndex = 0;
    private GameObject currentStageObj;
    private bool isGrowing = false;

    void Start()
    {
        StartStage(0);
    }

    void StartStage(int index)
    {
        if (currentStageObj != null) Destroy(currentStageObj);

        currentStageIndex = index;
        var stageData = treeData.stages[index];

        currentStageObj = Instantiate(stageData.stagePrefab, transform);
        currentStageObj.transform.localScale = stageData.startScale;

        StartCoroutine(GrowStage(stageData));
    }

    IEnumerator GrowStage(GrowthStage stageData)
    {
        isGrowing = true;
        float timer = 0;

        while (timer < stageData.stageDuration)
        {
            timer += Time.deltaTime;
            float t = timer / stageData.stageDuration;
            currentStageObj.transform.localScale = Vector3.Lerp(stageData.startScale, stageData.endScale, t);
            yield return null;
        }

        isGrowing = false;

        if (currentStageIndex + 1 < treeData.stages.Length)
        {
            StartStage(currentStageIndex + 1);
        }
        else
        {
            Debug.Log($"{treeData.treeName} đã trưởng thành!");
        }
    }
}
