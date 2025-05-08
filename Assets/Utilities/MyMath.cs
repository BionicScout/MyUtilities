using UnityEngine;

public static class MyMath {
    public static float AngleWithDirection(Vector3 a, Vector3 b, Vector3 normal) {
        Vector3 u_a = a.normalized;
        Vector3 u_b = b.normalized;
        Vector3 u_normal = normal.normalized;

        float dot = Vector3.Dot(u_a, u_b);
        float determinate = Vector3.Dot(u_normal, Vector3.Cross(u_a, u_b));

        float angle = Mathf.Atan2(determinate, dot);
        return angle * Mathf.Rad2Deg;
    }
}
