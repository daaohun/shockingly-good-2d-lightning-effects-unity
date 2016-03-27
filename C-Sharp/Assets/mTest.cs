using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 一条闪电
/// </summary>
public class mTest : MonoBehaviour 
{
    //Prefabs to be assigned in Editor
    public GameObject BoltPrefab;

    //For pooling
    List<GameObject> activeBoltsObj;
    List<GameObject> inactiveBoltsObj;
    int maxBolts = 1000;

    //For handling mouse clicks
    int clicks = 0;
    Vector2 pos1, pos2;

    void Start()
    {
        //Initialize lists
        activeBoltsObj = new List<GameObject>();
        inactiveBoltsObj = new List<GameObject>();

        //Grab the parent we'll be assigning to our bolt pool
        GameObject p = GameObject.Find("LightningPoolHolder");

        //For however many bolts we've specified
        for (int i = 0; i < maxBolts; i++)
        {
            //create from our prefab
            GameObject bolt = (GameObject)Instantiate(BoltPrefab);

            //Assign parent
            bolt.transform.parent = p.transform;

            //Initialize our lightning with a preset number of max sexments
            bolt.GetComponent<LightningBolt>().Initialize(25);

            //Set inactive to start
            bolt.SetActive(false);

            //Store in our inactive list
            inactiveBoltsObj.Add(bolt);
        }
    }

    void Update()
    {
        //Declare variables for use later
        GameObject boltObj;
        LightningBolt boltComponent;

        //store off the count for effeciency
        int activeLineCount = activeBoltsObj.Count;

        //loop through active lines (backwards because we'll be removing from the list)
        for (int i = activeLineCount - 1; i >= 0; i--)
        {
            //pull GameObject
            boltObj = activeBoltsObj[i];

            //get the LightningBolt component
            boltComponent = boltObj.GetComponent<LightningBolt>();

            //if the bolt has faded out
            if (boltComponent.IsComplete)
            {
                //deactive the segments it contains
                boltComponent.DeactivateSegments();

                //set it inactive
                boltObj.SetActive(false);

                //move it to the inactive list
                activeBoltsObj.RemoveAt(i);
                inactiveBoltsObj.Add(boltObj);
            }
        }

        //If left mouse button pressed
        if (Input.GetMouseButtonDown(0))
        {
            //if first click
            if (clicks == 0)
            {
                //store starting position
                Vector3 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos1 = new Vector2(temp.x, temp.y);
            }
            else if (clicks == 1) //second click
            {
                //store end position
                Vector3 temp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                pos2 = new Vector2(temp.x, temp.y);

                //create a (pooled) bolt from pos1 to pos2
                CreatePooledBolt(pos1, pos2, Color.white, 1f);
            }

            //increment our tick count
            clicks++;

            //restart the count after 2 clicks
            if (clicks > 1) clicks = 0;
        }

        //update and draw active bolts
        for (int i = 0; i < activeBoltsObj.Count; i++)
        {
            activeBoltsObj[i].GetComponent<LightningBolt>().UpdateBolt();
            activeBoltsObj[i].GetComponent<LightningBolt>().Draw();
        }
    }

    void CreatePooledBolt(Vector2 source, Vector2 dest, Color color, float thickness)
    {
        //if there is an inactive bolt to pull from the pool
        if (inactiveBoltsObj.Count > 0)
        {
            //pull the GameObject
            GameObject boltObj = inactiveBoltsObj[inactiveBoltsObj.Count - 1];

            //set it active
            boltObj.SetActive(true);

            //move it to the active list
            activeBoltsObj.Add(boltObj);
            inactiveBoltsObj.RemoveAt(inactiveBoltsObj.Count - 1);

            //get the bolt component
            LightningBolt boltComponent = boltObj.GetComponent<LightningBolt>();

            //activate the bolt using the given position data
            boltComponent.ActivateBolt(source, dest, color, thickness);
        }
    }
}
