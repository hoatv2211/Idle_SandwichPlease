using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathArrow : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public GameObject arrowPrefab;
    public float arrowSpacing = 1f;


    public void SetTarget(Transform _target)
    {
        pointB = _target;
    }


    [SerializeField] private float bonus = -45;
    private void LateUpdate()
    {
        transform.position = pointA.position;
        transform.LookAt(pointB);
    }

    //private void OnDrawGizmos()
    //{
    //    if (pointA == null || pointB == null)
    //        return;

    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawLine(pointA.position, pointB.position);

    //    Vector3 direction = (pointB.position - pointA.position).normalized;
    //    float distance = Vector3.Distance(pointA.position, pointB.position);
    //    int arrowCount = Mathf.CeilToInt(distance / arrowSpacing);

    //    for (int i = 0; i < arrowCount; i++)
    //    {
    //        float t = i / (float)arrowCount;
    //        Vector3 position = Vector3.Lerp(pointA.position, pointB.position, t);

    //        // Draw an arrow at each point along the line
    //        DrawArrow(position, direction);
    //    }
    //}

    //private void DrawArrow(Vector3 position, Vector3 direction)
    //{
    //    float arrowSize = 0.2f; // Adjust arrow size as needed

    //    Gizmos.color = Color.red;

    //    // Draw the main line
    //    Gizmos.DrawLine(position, position + direction * arrowSize);

    //    // Draw arrowhead
    //    Vector3 right = Quaternion.Euler(0, 160, 0) * direction;
    //    Vector3 left = Quaternion.Euler(0, -160, 0) * direction;

    //    Gizmos.DrawLine(position + direction * arrowSize, position + right * arrowSize * 0.5f);
    //    Gizmos.DrawLine(position + direction * arrowSize, position + left * arrowSize * 0.5f);
    //}



}
