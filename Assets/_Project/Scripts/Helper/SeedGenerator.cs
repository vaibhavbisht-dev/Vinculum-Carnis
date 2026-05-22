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
        // 1. FIX: Safely handle if no mouse is connected (e.g., mobile/console)
        Vector2 delta = Vector2.zero;
        if (Mouse.current != null)
        {
            delta = Mouse.current.delta.ReadValue();
        }

        double timestamp = Time.unscaledTimeAsDouble;

        // 2. FIX: Avoid string interpolation allocations. 
        // We can generate a reliable hash mathematically without creating string garbage.
        unchecked // Allows integer overflow without throwing an error (standard for hashing)
        {
            int hash = 17;
            hash = hash * 31 + delta.x.GetHashCode();
            hash = hash * 31 + delta.y.GetHashCode();
            hash = hash * 31 + timestamp.GetHashCode();
            return hash;
        }
    }


    
}


