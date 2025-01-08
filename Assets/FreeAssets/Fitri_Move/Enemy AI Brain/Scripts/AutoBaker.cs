using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AutoBaker : MonoBehaviour
{
    private void Awake() 
    { 
        if (GameObject.Find("nav_mesh_surface")) 
        { 
            this.GetComponent<NavMeshAgent>().enabled = true; return; 
        } 
        
        GameObject surface = new GameObject(); 
        
        surface.transform.name = "nav_mesh_surface"; 
        
        surface.gameObject.AddComponent<NavMeshSurface>(); 
        
        surface.GetComponent<NavMeshSurface>().BuildNavMesh(); 
        
        this.GetComponent<NavMeshAgent>().enabled = true; 
    }
}