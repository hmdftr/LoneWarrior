using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kolider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<TerrainCollider>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<TerrainCollider>().enabled = true;
    }
}
