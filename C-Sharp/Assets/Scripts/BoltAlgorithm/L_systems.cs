using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class L_systems : MonoBehaviour
{

    public PoolMgr lingPoorMgr;

    //Prefab for a line
    public GameObject linePrefab;
    //The color of our bolts
    public Color Tint { get; set; }
    
    //Transparency
    public float Alpha { get; set; }

    //The speed at which our bolts will fade out
    public float FadeOutRate { get; set; }

    //True if the bolt has completely faded out
    public bool IsComplete { get { return Alpha <= 0; } }

    public void Initialize(int maxSegments)
    {
        lingPoorMgr = new PoolMgr(linePrefab, null, null);
        lingPoorMgr.PrePopulate(5);
    }

    public class Segment
    {

        public Vector2 startpoint;
        public Vector2 endPoint;

        public Segment(Vector2 start, Vector2 end)
        {
            this.startpoint = start;
            this.endPoint = end;
        }
    }

    public void ActivateBolt(
        Vector2 startPoint, Vector2 endPoint, Color color,
            //int generationNum = 20, 
                float maximumOffset = 3.0f, float thickness = 1.0f)
    {

        //Store tint
        Tint = color;

        //Store alpha
        Alpha = 1.5f;

        //Store fade out rate
        FadeOutRate = 0.03f;

        //segmentList.Add(new Segment(startPoint, endPoint));
        //offsetAmount = maximumOffset; // the maximum amount to offset a lightning vertex.
        //for each generation (some number of generations)
        //  for each segment that was in segmentList when this generation started
        //    segmentList.Remove(segment); // This segment is no longer necessary.

        //    midPoint = Average(startpoint, endPoint);
        //    // Offset the midpoint by a random amount along the normal.
        //    midPoint += Perpendicular(Normalize(endPoint-startPoint))*RandomFloat(-offsetAmount,offsetAmount);

        //    // Create two new segments that span from the start point to the end point,
        //    // but with the new (randomly-offset) midpoint.
        //    segmentList.Add(new Segment(startPoint, midPoint));
        //    segmentList.Add(new Segment(midPoint, endPoint));
        //  end for
        //  offsetAmount /= 2; // Each subsequent generation offsets at max half as much as the generation before.
        //end for

        Vector2 midPoint;
        Vector2 slope;
        Vector2 normal;
        Vector2 splitEnd;
        float offset;
        //bool flip = true;
        Queue<Segment> segmentList = new Queue<Segment>();
        segmentList.Enqueue(new Segment(startPoint, endPoint));
        float offsetAmount = maximumOffset;

        //slope = segmentList.Peek().endPoint - segmentList.Peek().startpoint;
        //float distance = slope.magnitude;

        for (int i = 0; i < 40; i++)
        {
            Segment current = segmentList.Dequeue();

            midPoint = (current.startpoint + current.endPoint) / 2.0f;
            slope = (current.endPoint - current.startpoint).normalized;
            slope = UnityEngine.Random.Range(0, 2) == 0 ? new Vector2(-slope.y, slope.x) : new Vector2(slope.y, -slope.x);
            midPoint += slope * UnityEngine.Random.Range(-offsetAmount, offsetAmount);

            segmentList.Enqueue(new Segment(current.startpoint, midPoint));
            segmentList.Enqueue(new Segment(midPoint, current.endPoint));

            //direction = midPoint - startPoint;
            //splitEnd = Rotate(direction, randomSmallAngle) * lengthScale + midPoint; // lengthScale is, for best results, < 1.  0.7 is a good value.
            //segmentList.Add(new Segment(midPoint, splitEnd));

            //slope = midPoint - current.startpoint;
            //slope = Quaternion.Euler(Vector3.forward * Random.Range(5.0f, 10.0f)) * slope;
            //slope *= 0.7f;
            //splitEnd = midPoint + slope;
            ////lengthScale; // lengthScale is, for best results, < 1.  0.7 is a good value.
            //segmentList.Enqueue(new Segment(midPoint, splitEnd));

            offsetAmount /= 2.0f;
        }


        foreach (Segment item in segmentList)
        {
            activateLine(item.startpoint, item.endPoint, thickness);
        }
    }

    public void DeactivateSegments()
    {
        lingPoorMgr.UnspawnAll();
    }

    void activateLine(Vector2 A, Vector2 B, float thickness)
    {
        GameObject line = lingPoorMgr.Spawn(Vector3.zero, Quaternion.identity);
        Line lineComponent = line.GetComponent<Line>();
        lineComponent.SetColor(Color.white);
        lineComponent.A = A;
        lineComponent.B = B;
        lineComponent.Thickness = thickness;
    }

    public void Draw()
    {
        //if the bolt has faded out, no need to draw
        if (Alpha <= 0) return;

        foreach (GameObject obj in lingPoorMgr.active)
        {
            Line lineComponent = obj.GetComponent<Line>();
            lineComponent.SetColor(Tint * (Alpha * 0.6f));
            lineComponent.Draw();
        }
    }

    public void UpdateBolt()
    {
        Alpha -= FadeOutRate;
    }
}
