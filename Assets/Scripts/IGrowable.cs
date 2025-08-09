public interface IGrowable
{
    void Water(float amount);
    void AddFertilizer(float amount);
    void AddSunlight(float hours);
    void ApplyPesticide();
    void UpdateGrowth(float deltaTime);
}
