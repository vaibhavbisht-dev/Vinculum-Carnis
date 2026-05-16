using UnityEngine;

public class VBHelpers
{
    public static float GetRandomAmount(float minRange, float maxRange)
    {
        float _amount;
        UnityEngine.Random.InitState(SeedGenerator.GetMouseJitterSeed());
        // Generate the raw value
        float rawAmount = UnityEngine.Random.Range(minRange, maxRange);

        // Round to 2 decimal places
        _amount = (float)System.Math.Round(rawAmount, 2);
        return _amount;
    }
}
