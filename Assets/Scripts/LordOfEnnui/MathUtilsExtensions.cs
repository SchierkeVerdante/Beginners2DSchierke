using UnityEngine;

public static class MathUtilsExtensions {

    public static float Get2DAngle(this Vector3 direction) {
        return Get2DAngle((Vector2) direction);
    }

    public static float Get2DAngle(this Vector2 direction) {
        float angle = Vector2.SignedAngle(Vector2.right, direction);
        angle = angle < 0 ? 360 + angle : angle;
        return angle;
    }

    public static float GetAngularDistance(this float angle1, float angle2) {
        float absDiff = Mathf.Abs(angle1 - angle2);
        return Mathf.Min(absDiff, 360 - absDiff);
    }
}
