using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Line : MonoBehaviour
{
    [SerializeField] private LineRenderer lr;

    private List<Vector2> points;

    public void SetPoint(Vector2 point)
    {
        points.Add(point);

        lr.positionCount = points.Count;
        lr.SetPosition(points.Count - 1, point);
    }

    public void UpdateLine(Vector2 position)
    {
        if (points == null)
        {
            points = new List<Vector2>();
            SetPoint(position);
            return;
        }
        
        if (Vector2.Distance(points.Last(), position) > 0.1f)
        {
            SetPoint(position);
        }
    }
}
