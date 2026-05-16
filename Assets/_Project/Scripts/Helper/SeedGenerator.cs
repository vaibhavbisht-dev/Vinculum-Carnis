using UnityEngine;
using UnityEngine.InputSystem;


/// <summary>
/// Returns a noisy, high-entropy integer based on mouse jitter and timing.
/// </summary>
public class SeedGenerator
{
    /// <summary>
    /// Returns a noisy, high-entropy integer based on mouse jitter and timing.
    /// </summary>
    public static int GetMouseJitterSeed()
    {
        // 1. Get raw sub-pixel delta (movement since last frame)
        Vector2 delta = Mouse.current.delta.ReadValue();

        // 2. Get high-precision time to differentiate frames
        double timestamp = Time.unscaledTimeAsDouble;

        // 3. Combine into a unique string format
        // Including x and y separately captures the specific angle of hand tremor
        string rawData = $"{delta.x}_{delta.y}_{timestamp}";

        // 4. Return the deterministic hash of that noise
        return rawData.GetHashCode();
    }


    
}


