// ==== UI/NameTag.cs ====
using UnityEngine;
using TMPro;

public class NameTag : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0, 2.2f, 0);
    public TextMeshPro text;

    private void LateUpdate()
    {
        if (target == null) return;

        // UI элемент следует за игроком
        transform.position = target.position + offset;
        transform.rotation = Quaternion.LookRotation(transform.position - Camera.main.transform.position);
    }

    public void SetName(string name)
    {
        if (text != null)
            text.text = name;
    }
}