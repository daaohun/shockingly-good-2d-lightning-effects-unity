using UnityEngine;
using System.Collections;

/// <summary>
/// 中点位移法实现
/// </summary>
public class mkMidPointReplacement : MonoBehaviour {

	
    //function drawLightning(x1,y1,x2,y2,displace)
    //{
    //  if (displace < curDetail) {
    //    graf.moveTo(x1,y1);
    //    graf.lineTo(x2,y2);
    //  }
    //  else {
    //    var mid_x = (x2+x1)/2;
    //    var mid_y = (y2+y1)/2;
    //    mid_x += (Math.random()-.5)*displace;
    //    mid_y += (Math.random()-.5)*displace;
    //    drawLightning(x1,y1,mid_x,mid_y,displace/2);
    //    drawLightning(x2,y2,mid_x,mid_y,displace/2);
    //  }
    //}

    //Prefab for a line
    public GameObject linePrefab;
    public float curDetail;
    public float thickness;
    public float Alpha = 1.0f;
    public int curBranch = 0;
    public float FadeOutRate = 0.03f;
    public int maxBranch = 3;
    //True if the bolt has completely faded out
    public bool IsComplete { get { return Alpha <= 0; } }

    PoolMgr lingPoorMgr;

    public void Initialize(int maxNum)
    {
        lingPoorMgr = new PoolMgr(linePrefab, null, null);
        lingPoorMgr.PrePopulate(maxNum);
    }

    public void Init()
    {
        Alpha = 1.0f;
        curBranch = 0;
    }

    
    public void drawLighting(Vector2 start, Vector2 end, float displace)
    {

        if(displace < curDetail) {
            activateLine(start, end, thickness);
        }
        else {
            Vector2 mid = (start + end) / 2.0f;
            float x  = (Random.Range(0.0f, 1.0f) - 0.5f) * displace;
            float y  = (Random.Range(0.0f, 1.0f) - 0.5f) * displace;
            mid += new Vector2(x,y);
            drawLighting(start, mid, displace / 2);
            drawLighting(end,   mid, displace / 2);


            //if (curBranch < maxBranch && UnityEngine.Random.Range(0, 2) == 1)
            //{
            //    curBranch++;

            //    Vector2 slope = mid - start;
            //    slope = Quaternion.Euler(Vector3.forward * Random.Range(5.0f, 10.0f)) * slope;
            //    slope *= 0.7f;
            //    Vector2 splitEnd = mid + slope;
            //    ////lengthScale; // lengthScale is, for best results, < 1.  0.7 is a good value.
            //    drawLighting(mid, splitEnd, displace / 2);
            //}
        }

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
    public void DeactivateSegments()
    {
        lingPoorMgr.UnspawnAll();
    }

    public void UpdateBolt()
    {
        Alpha -= FadeOutRate;
    }
    public void Draw()
    {
        //if the bolt has faded out, no need to draw
        if (Alpha <= 0) return;

        foreach (GameObject obj in lingPoorMgr.active)
        {
            Line lineComponent = obj.GetComponent<Line>();
            lineComponent.SetColor(Color.white * (Alpha * 0.6f));
            lineComponent.Draw();
        }
    }
}
