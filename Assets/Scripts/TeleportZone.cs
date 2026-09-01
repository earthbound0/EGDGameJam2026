using UnityEngine;

public class TeleportZone : MonoBehaviour
{
    private Vector3 destination = new Vector3(0f, 0.675f, 0f);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = destination;

            Rigidbody rb = other.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}