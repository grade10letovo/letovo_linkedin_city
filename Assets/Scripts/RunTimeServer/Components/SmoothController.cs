using UnityEngine;

public class SmoothFollower : MonoBehaviour
{
    public float speed = 5f;
    private Vector3 target;
    private bool hasTarget = false;

    public void SetTarget(Vector3 position)
    {
        target = position;
        hasTarget = true;
    }

    void Update()
    {
        if (!hasTarget) return;
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * speed);
    }
}