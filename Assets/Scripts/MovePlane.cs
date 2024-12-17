using Unity.VisualScripting;
using UnityEngine;

public class MovePlane : MonoBehaviour
{
    public GameObject player;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
    }
}
