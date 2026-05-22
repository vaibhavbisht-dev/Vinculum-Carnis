using UnityEngine;

public class VBHelpers
{

    public static void InitRandom() {
        UnityEngine.Random.InitState(SeedGenerator.GetMouseJitterSeed());
    }
    public static float GetRandomAmount(float minRange, float maxRange)
    {
        float _amount;
        // Generate the raw value
        float rawAmount = UnityEngine.Random.Range(minRange, maxRange);

        // Round to 2 decimal places
        _amount = (float)System.Math.Round(rawAmount, 2);
        return _amount;
    }

    public static int GetRandomAmount(int minRange, int maxRange)
    {
        int _amount;
        _amount = UnityEngine.Random.Range(minRange, maxRange);
        return _amount;
    }

    public static int GetRandomValuefromArrayLength(int array)
    {
        
        return UnityEngine.Random.Range(0, array);
        
    }

    /// <summary>
    /// Determines whether the specified viewer is looking at a target position within a given field of view angle on
    /// the XZ plane.
    /// </summary>
    /// <remarks>This method ignores the Y component of both the viewer and the target position, effectively
    /// projecting the check onto the horizontal plane. Useful for 2D or top-down scenarios where vertical orientation
    /// is not relevant.</remarks>
    /// <param name="viewer">The transform representing the viewer's position and forward direction. Cannot be null.</param>
    /// <param name="targetPosition">The world-space position of the target to check, using only the X and Z coordinates.</param>
    /// <param name="fovAngle">The field of view angle, in degrees, centered on the viewer's forward direction. Must be greater than 0 and less
    /// than or equal to 360.</param>
    /// <returns>true if the target position is within the viewer's field of view on the XZ plane; otherwise, false.</returns>
    public static bool IsLookingAt(Transform viewer, Vector3 targetPosition, float fovAngle)
    {
        // 1. Flatten the positions to the XZ plane (ignore Y)
        Vector2 viewerPos2D = new Vector2(viewer.position.x, viewer.position.z);
        Vector2 targetPos2D = new Vector2(targetPosition.x, targetPosition.z);

        // 2. Get the viewer's forward direction and flatten it
        // viewer.forward is derived from the object's Quaternion rotation
        Vector2 viewerForward2D = new Vector2(viewer.forward.x, viewer.forward.z);

        // 3. Calculate the direction to the target
        Vector2 directionToTarget = targetPos2D - viewerPos2D;

        // Normalize the vectors (make their length 1)
        viewerForward2D.Normalize();
        directionToTarget.Normalize();

        // 4. Calculate the Dot Product
        float dotProduct = Vector2.Dot(viewerForward2D, directionToTarget);

        // 5. Calculate the threshold
        // We use half the FOV because the angle extends to the left and right of center
        float halfFov = fovAngle / 2f;

        // Convert the angle to radians for the math function
        float threshold = Mathf.Cos(halfFov * Mathf.Deg2Rad);

        // Return true if the dot product is greater than the threshold
        return dotProduct >= threshold;
    }

    /// <summary>
    /// Gives the angle in degrees that an arrow should be rotated to point from the viewer towards the target position.
    /// </summary>
    /// <param name="viewerPosition">The world-space position of the viewer.</param>
    /// <param name="targetPosition">The world-space position of the target.</param>
    /// <returns>The angle in degrees that the arrow should be rotated to point from the viewer towards the target.</returns>
    public static float GetArrowRotationAngle(Transform viewer, Vector3 targetPosition)
    {
        // 1. Convert the target's world position into the viewer's local space.
        // This perfectly accounts for both the player's position AND rotation.
        // - If the target is straight ahead, localTargetPos.z will be positive, and x will be 0.
        // - If the target is behind, localTargetPos.z will be negative.
        Vector3 localTargetPos = viewer.InverseTransformPoint(targetPosition);

        // 2. Use Atan2 on the local X and Z coordinates.
        // Now, Atan2(0, positive_Z) = 0 (Forward/Up)
        // Atan2(0, negative_Z) = 180 (Backward/Down)
        float angleRadians = Mathf.Atan2(localTargetPos.x, localTargetPos.z);

        // 3. Convert radians to degrees
        float angleDegrees = angleRadians * Mathf.Rad2Deg;

        // Optional: Ensure the angle is between 0 and 360
        if (angleDegrees < 0)
        {
            angleDegrees += 360f;
        }

        return angleDegrees;
    }

}
