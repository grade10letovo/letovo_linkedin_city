using UnityEngine;

public class AnimatorOptimizer : MonoBehaviour
{
    public Animator animator;
    public Transform player;
    public float disableDistance = 0f; // ����������, ����� �������� �������� �����������

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if (distance > disableDistance)
        {
            if (animator.enabled)
            {
                animator.enabled = false; // ��������� Animator
            }
        }
        else
        {
            if (!animator.enabled)
            {
                animator.enabled = true; // �������� �������, ���� �������� ������
            }
        }
    }
}
