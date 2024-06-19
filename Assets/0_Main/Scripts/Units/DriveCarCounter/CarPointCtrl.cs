using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CarPointCtrl : MonoBehaviour
{
    public List<CarPoint> _unitPoints;

    public int TotalFreePoint => _unitPoints.Count(p => !p.IsFilled);

    private List<CarPoint> _originalPoints;

    private void Awake()
    {
        _originalPoints = new List<CarPoint>(_unitPoints);
    }

    public CarPoint GetFreePointDynamic()
    {
        if (TotalFreePoint <= 0)
        {
            int currentCount = _unitPoints.Count;
            int originalCount = _originalPoints.Count;
            int index = (currentCount - 1) % (originalCount - 1);
            Vector3 randomPos = RandomPos(_originalPoints[index].transform.position, _originalPoints[index + 1].transform.position);
            CarPoint newPoint = Instantiate<CarPoint>(_unitPoints[0], randomPos, Quaternion.identity);
            newPoint.transform.SetParent(this.transform);
            newPoint.transform.LookAt(_originalPoints[index + 1].transform);
            newPoint.ProductType = _unitPoints[0].ProductType;
            _unitPoints.Add(newPoint);
        }
        int foundedIndex = _unitPoints.FindIndex(p => !p.IsFilled);
        return _unitPoints[foundedIndex];
    }

    public CarPoint GetFreePointStatic()
    {
        int foundedIndex = _unitPoints.FindIndex(p => !p.IsFilled);
        return _unitPoints[foundedIndex];
    }

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
        _unitPoints.ForEach(p => p.ProductType = productType);
    }
}
