using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingSystem : MonoBehaviour
{
    int healValue = 20;
    public GameObject HealthPoint;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // OnTriggerStay is called once per frame for every Collider other that is touching the trigger
    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            var healthComponent = other.GetComponent<Health>();

            if(healthComponent!=null && healthComponent.currenthealth<healthComponent.maxHealth)
            {
                healthComponent.AddHealth(healValue);

                Destroy(gameObject);

                //Debug.Log($"Player's healing +{healValue}");
            }
        }
    }
}
