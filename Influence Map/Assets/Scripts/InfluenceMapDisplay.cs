using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfluenceMapDisplay : MonoBehaviour
{
    Terrain terrain;
    public GameObject quad;
    public Material displayMaterial;
    private GameObject[] displayQuads = new GameObject[100 * 100];
    public GameObject quadHolder;
    private bool accurateMode = true;

    Gradient g;
    GradientColorKey[] gck;
    GradientAlphaKey[] gak;

    // Use this for initialization
    void Start()
    {
        terrain = Terrain.activeTerrain;


        g = new Gradient();
        gck = new GradientColorKey[3];
        gak = new GradientAlphaKey[3];

        gck[0].color = Color.green;
        gck[0].time = 0.0F;
        gck[1].color = Color.gray;
        gck[1].time = .5F;
        gck[1].color = Color.red;
        gck[1].time = 1F;

        gak[0].alpha = .5f;
        gak[0].time = 0.0F;
        gak[1].alpha = .5f;
        gak[1].time = .5f;
        gak[2].alpha = .5f;
        gak[2].time = 1.0F;

        g.SetKeys(gck, gak);

        quadHolder.SetActive(true);

        for ( int outer = 0; outer < 100; outer++)
        {
            for (int inner = 0; inner < 100; inner++)
            {
                displayQuads[outer * 100 + inner] = Instantiate(quad,quadHolder.transform);
                displayQuads[outer * 100 + inner].layer = 2;
                //check and see if this is lined up correctly
                Vector3 position = new Vector3(-50 + inner, 0, -50 + outer);
                displayQuads[outer * 100 + inner].transform.position = new Vector3(-50 + inner + .5f, terrain.SampleHeight(position) + .5f, -50 + outer + .5f);
                displayQuads[outer * 100 + inner].GetComponent<Renderer>().material = displayMaterial;
                displayQuads[outer * 100 + inner].GetComponent<Renderer>().material.color = new Color(0, 0, 0, .7f);
            }
        }
        quadHolder.SetActive(false);
    }

    public void ToggleVisibility()
    {
        quadHolder.SetActive(!quadHolder.activeSelf);
    }

    public void ToggleAccurateMode()
    {
        accurateMode = !accurateMode;
        UpdateDisplay();
    }

    // Update is called once per frame
    public void UpdateDisplay()
    {
        //InfluenceMap.Instance.map
        for (int outer = 0; outer < 100; outer++)
        {
            for (int inner = 0; inner < 100; inner++)
            {
                if(accurateMode)
                { 
                    displayQuads[(99 -inner) * 100 + outer].GetComponent<Renderer>().material.color = 
                    g.Evaluate(
                        ((InfluenceMap.Instance.map[outer, inner] + 1.0f) / 2.0f)
                        );
                }
                else
                {
                    if (InfluenceMap.Instance.map[outer, inner] > 0)
                    {
                        displayQuads[(99 - inner) * 100 + outer].GetComponent<Renderer>().material.color = Color.red;
                    }
                    else if (InfluenceMap.Instance.map[outer, inner] < 0)
                    {
                        displayQuads[(99 - inner) * 100 + outer].GetComponent<Renderer>().material.color = Color.green;
                    }
                    else
                    {
                        displayQuads[(99 - inner) * 100 + outer].GetComponent<Renderer>().material.color = Color.gray;
                    }
                    
                }
            }
        }
    }
}
