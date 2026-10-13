using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public Transform target;

    private static CameraShake instance;

    private float cameraZ;
    private float strength;
    private float duration;
    private float timeLeft;

    private float CurrentStrength => timeLeft > 0f ? strength * timeLeft / duration : 0f;

    private void Awake()
    {
        instance = this;
        cameraZ = transform.position.z;
    }

    public static void Shake(float power, float time)
    {
        if (instance == null || power < instance.CurrentStrength)
            return;

        instance.strength = power;
        instance.duration = time;
        instance.timeLeft = time;
    }

    private void LateUpdate()
    {

        if (target == null)
            return;

        timeLeft -= Time.deltaTime;

        Vector3 followPos = new Vector3(target.position.x, target.position.y, cameraZ);
        transform.position = followPos + (Vector3)(Random.insideUnitCircle * CurrentStrength);
    }
}
