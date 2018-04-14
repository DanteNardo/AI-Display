using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Influencer : MonoBehaviour {

    public int influenceVal = 0;

    public int xLoc;
    public int yLoc;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetMapLocation(int xIn, int yIn)
    {
        xLoc = xIn;
        yLoc = yIn;
    }

    public KeyValuePair<int, int> GetMapLocation()
    {
        return new KeyValuePair<int, int>(xLoc, yLoc);
    }
}
