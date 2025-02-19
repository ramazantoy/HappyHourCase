using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public bool isOccupied = false;

    private void OnDrawGizmos()
    {
        Gizmos.color = isOccupied ? Color.red : Color.green;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}