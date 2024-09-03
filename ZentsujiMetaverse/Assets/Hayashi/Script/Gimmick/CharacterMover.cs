using UnityEngine;

public class CharacterMover : MonoBehaviour
{
    [SerializeField,Header("移動する目的地のリスト")]
    public Transform[] waypoints; 
    [SerializeField,Header("移動速度")]
    public float moveSpeed = 2.0f; 
    [SerializeField,Header("回転速度")]
    public float rotationSpeed = 5.0f;

    private int currentWaypointIndex = 0;
    private float lastMoveTime;
    private float moveDuration;
    private float pauseDuration;

    void Start()
    {
        StartMoving();
    }

    void Update()
    {
        MoveTowardsWaypoint();
    }



    void MoveTowardsWaypoint()
    {
        if (waypoints.Length == 0) return;

        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 directionToWaypoint = targetWaypoint.position - transform.position;
        directionToWaypoint.y = 0;
        Quaternion targetRotation = Quaternion.LookRotation(directionToWaypoint);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.position += directionToWaypoint.normalized * moveSpeed * Time.deltaTime;

        if (directionToWaypoint.magnitude < 0.5f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
            lastMoveTime = Time.time;
            pauseDuration = Random.Range(4f, 12f);
        }
    }

    void StartMoving()
    {
        if (waypoints.Length == 0) return;
        lastMoveTime = Time.time;
        moveDuration = Random.Range(5f, 15f);
    }
}
