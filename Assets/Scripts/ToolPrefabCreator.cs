using UnityEngine;

public class ToolPrefabCreator : MonoBehaviour
{
    [Header("Tool Prefabs")]
    public GameObject wateringCanPrefab;
    public GameObject fertilizerBagPrefab;
    
    [Header("Particle Effects")]
    public GameObject waterParticlePrefab;
    public GameObject fertilizerParticlePrefab;
    
    void Start()
    {
        CreateToolPrefabs();
        CreateParticleEffects();
    }
    
    void CreateToolPrefabs()
    {
        // Tạo bình tưới nước
        if (!wateringCanPrefab)
        {
            wateringCanPrefab = CreateWateringCan();
        }
        
        // Tạo túi phân bón
        if (!fertilizerBagPrefab)
        {
            fertilizerBagPrefab = CreateFertilizerBag();
        }
    }
    
    GameObject CreateWateringCan()
    {
        GameObject wateringCan = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        wateringCan.name = "WateringCan";
        wateringCan.transform.localScale = new Vector3(0.3f, 0.4f, 0.3f);
        
        // Thêm handle
        GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        handle.name = "Handle";
        handle.transform.SetParent(wateringCan.transform);
        handle.transform.localPosition = new Vector3(0.2f, 0.2f, 0f);
        handle.transform.localScale = new Vector3(0.1f, 0.3f, 0.05f);
        
        // Thêm spout
        GameObject spout = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        spout.name = "Spout";
        spout.transform.SetParent(wateringCan.transform);
        spout.transform.localPosition = new Vector3(0f, -0.1f, 0.2f);
        spout.transform.localRotation = Quaternion.Euler(90f, 0f, 0f);
        spout.transform.localScale = new Vector3(0.05f, 0.2f, 0.05f);
        
        // Material xanh
        Renderer renderer = wateringCan.GetComponent<Renderer>();
        if (renderer)
        {
            Material blueMaterial = new Material(Shader.Find("Standard"));
            blueMaterial.color = Color.blue;
            renderer.material = blueMaterial;
        }
        
        return wateringCan;
    }
    
    GameObject CreateFertilizerBag()
    {
        GameObject fertilizerBag = GameObject.CreatePrimitive(PrimitiveType.Cube);
        fertilizerBag.name = "FertilizerBag";
        fertilizerBag.transform.localScale = new Vector3(0.4f, 0.3f, 0.2f);
        
        // Thêm handle
        GameObject handle = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        handle.name = "Handle";
        handle.transform.SetParent(fertilizerBag.transform);
        handle.transform.localPosition = new Vector3(0f, 0.25f, 0f);
        handle.transform.localRotation = Quaternion.Euler(0f, 0f, 90f);
        handle.transform.localScale = new Vector3(0.05f, 0.3f, 0.05f);
        
        // Material nâu
        Renderer renderer = fertilizerBag.GetComponent<Renderer>();
        if (renderer)
        {
            Material brownMaterial = new Material(Shader.Find("Standard"));
            brownMaterial.color = new Color(0.6f, 0.4f, 0.2f);
            renderer.material = brownMaterial;
        }
        
        return fertilizerBag;
    }
    
    void CreateParticleEffects()
    {
        // Tạo hiệu ứng nước
        if (!waterParticlePrefab)
        {
            waterParticlePrefab = CreateWaterParticleEffect();
        }
        
        // Tạo hiệu ứng phân bón
        if (!fertilizerParticlePrefab)
        {
            fertilizerParticlePrefab = CreateFertilizerParticleEffect();
        }
    }
    
    GameObject CreateWaterParticleEffect()
    {
        GameObject waterEffect = new GameObject("WaterParticleEffect");
        ParticleSystem ps = waterEffect.AddComponent<ParticleSystem>();
        
        var main = ps.main;
        main.startLifetime = 2f;
        main.startSpeed = 3f;
        main.startSize = 0.1f;
        main.startColor = Color.cyan;
        main.maxParticles = 50;
        
        var emission = ps.emission;
        emission.rateOverTime = 20;
        
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 15f;
        
        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.y = new ParticleSystem.MinMaxCurve(-2f, -5f);
        
        return waterEffect;
    }
    
    GameObject CreateFertilizerParticleEffect()
    {
        GameObject fertilizerEffect = new GameObject("FertilizerParticleEffect");
        ParticleSystem ps = fertilizerEffect.AddComponent<ParticleSystem>();
        
        var main = ps.main;
        main.startLifetime = 1.5f;
        main.startSpeed = 2f;
        main.startSize = 0.05f;
        main.startColor = new Color(0.6f, 0.4f, 0.2f);
        main.maxParticles = 30;
        
        var emission = ps.emission;
        emission.rateOverTime = 15;
        
        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Sphere;
        shape.radius = 0.1f;
        
        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.World;
        velocity.y = new ParticleSystem.MinMaxCurve(-1f, -3f);
        
        return fertilizerEffect;
    }
    
    // Helper methods để lấy prefabs
    public GameObject GetWateringCanPrefab()
    {
        return wateringCanPrefab;
    }
    
    public GameObject GetFertilizerBagPrefab()
    {
        return fertilizerBagPrefab;
    }
    
    public GameObject GetWaterParticlePrefab()
    {
        return waterParticlePrefab;
    }
    
    public GameObject GetFertilizerParticlePrefab()
    {
        return fertilizerParticlePrefab;
    }
}
