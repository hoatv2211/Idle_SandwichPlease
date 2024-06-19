using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CustomerPointCtrl : MonoBehaviour
{
    public List<CustomerPoint> _customerPoints;

    public int TotalFreePoint => _customerPoints.Count(p => !p.IsFilled);
  
    private Vector3 RandomPos(Vector3 pointA, Vector3 pointB)
    {
        Vector3 randomPoint = Vector3.Lerp(pointA, pointB, Random.value);
        Vector3 randomDirection = Random.insideUnitSphere;
        float randomOffset = Random.Range(-1f, 1f);
        randomPoint += randomDirection * randomOffset;
        Debug.Log("Random point on line: " + randomPoint);
        return randomPoint;
    }

    public void Initialize(string productType)
    {
        _customerPoints.ForEach(p => p.ProductType = productType);
    }
}
