using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceMapDisplay : MonoBehaviour
{
    Terrain terrain;
    public GameObject quad;
    public Material displayMaterial;
    private GameObject[] displayQuads = new GameObject[100 * 100];

    // Use this for initialization
    void Start()
    {
        terrain = Terrain.activeTerrain;


        for( int outer = 0; outer < 100; outer++)
        {
            for (int inner = 0; inner < 100; inner++)
            {
                displayQuads[outer * 100 + inner] = Instantiate(quad);
                displayQuads[outer * 100 + inner].layer = 2;
                //check and see if this is lined up correctly
                Vector3 position = new Vector3(-50 + inner, 0, -50 + outer);
                displayQuads[outer * 100 + inner].transform.position = new Vector3(-50 + inner + .5f, terrain.SampleHeight(position) + .5f, -50 + outer + .5f);
                displayQuads[outer * 100 + inner].GetComponent<Renderer>().material = displayMaterial;
                displayQuads[outer * 100 + inner].GetComponent<Renderer>().material.color = new Color(0, 0, 0, .7f);
            }
        }
    }

    // Update is called once per frame
    public void UpdateDisplay()
    {
        //InfluenceMap.Instance.map
        for (int outer = 0; outer < 100; outer++)
        {
            for (int inner = 0; inner < 100; inner++)
            {
                displayQuads[(99 -inner) * 100 + outer].GetComponent<Renderer>().material.color = 
                    Color.Lerp(new Color(0,1,0,.5f), new Color(1, 0, 0, .5f), (InfluenceMap.Instance.map[outer, inner] + 1.0f) / 2.0f);
                
            }
        }
    }
}
