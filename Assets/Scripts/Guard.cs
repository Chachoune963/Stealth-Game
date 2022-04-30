using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    public static event System.Action OnSpotted;

    [Header ("Movement")]
    public float speed = 5;
    public float waitTime = 2;
    public float turnSpeed = 2;
    public Transform pathHolder;

    [Header ("Detection")]
    public Light spotlight;
    public float viewDistance;
    public LayerMask viewMask;
    float viewAngle;
    public float spottedTime = .5f;
    float spottedTimer;
    GameObject tracked;

    private void Start()
    {
        tracked = GameObject.FindGameObjectWithTag("Player");
        viewAngle = spotlight.spotAngle;
        Vector3[] waypoints = new Vector3[pathHolder.childCount];
        for (int i = 0; i < waypoints.Length; i++)
        {
            waypoints[i] = pathHolder.GetChild(i).position;
            waypoints[i] = new Vector3(waypoints[i].x, transform.position.y, waypoints[i].z);
        }
        StartCoroutine(FollowPath(waypoints));
    }

    private void Update()
    {
        if (CanSeePlayer())
        {
            spotlight.color = Color.yellow;
            if (spottedTimer >= spottedTime)
            {
                spotlight.color = Color.red;
                if (OnSpotted != null)
                {
                    OnSpotted();
                }
            } else
            {
                spottedTimer += Time.deltaTime;
            }
        } else
        {
            spotlight.color = Color.green;
            spottedTimer = 0f;
        }
    }

    bool CanSeePlayer()
    {
        Vector3 trackedVector = tracked.transform.position - transform.position;
        float trackedAngle = Vector3.Angle(transform.forward, trackedVector);
        float trackedDistance = trackedVector.magnitude;
        if (trackedDistance <= viewDistance
            && Mathf.Abs(trackedAngle) <= viewAngle / 2f
            && !Physics.Linecast(transform.position, tracked.transform.position, viewMask))
        {
            return true;
        }
        return false;
    }

    IEnumerator FollowPath(Vector3[] path)
    {
        transform.position = path[0];

        int targetWaypointIndex = 1;
        Vector3 targetWaypoint = path[targetWaypointIndex];
        transform.LookAt(targetWaypoint);

        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetWaypoint, speed * Time.deltaTime);
            if (transform.position == targetWaypoint)
            {
                targetWaypointIndex = (targetWaypointIndex + 1) % path.Length;
                targetWaypoint = path[targetWaypointIndex];
                yield return new WaitForSeconds(waitTime);
                yield return StartCoroutine(RotateToTarget(targetWaypoint));
            }
            yield return null;
        }
    }

    IEnumerator RotateToTarget(Vector3 target)
    {
        Vector3 targetRotation = (target - transform.position).normalized;
        float targetAngle = 90 - Mathf.Atan2(targetRotation.z, targetRotation.x) * Mathf.Rad2Deg;

        while (Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, targetAngle)) > 0.05)
        {
            float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetAngle, turnSpeed * Time.deltaTime);
            transform.eulerAngles = Vector3.up * angle;
            yield return null;
        }
    }

    private void OnDrawGizmos()
    {
        Vector3 startPosition = pathHolder.GetChild(0).position;
        Vector3 previousPosition = startPosition;
        foreach (Transform waypoint in pathHolder)
        {
            Gizmos.DrawSphere(waypoint.position, .3f);
            Gizmos.DrawLine(previousPosition, waypoint.position);
            previousPosition = waypoint.position;
        }
        Gizmos.DrawLine(previousPosition, startPosition);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * viewDistance);
    }
}
