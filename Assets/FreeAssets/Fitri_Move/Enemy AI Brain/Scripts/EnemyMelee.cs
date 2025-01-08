using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    public float Range = 2f;
    public bool isAttack = false;

    public Collider[] cols;

    private float timer = 0.0f;


    private void Update()
    {
        cols = Physics.OverlapSphere(transform.position, Range);

        isAttack = (transform.root.GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Attack") && Time.time > timer) ? true : false;
        if (isAttack){
            timer = Time.time + (transform.root.GetChild(0).GetComponent<Animator>().GetFloat("AttackMultiplier")-0.6f);
        }

        foreach(Collider col in cols){
            if (col.transform.GetComponent<TakeDamageEvent>() && !col.transform.name.Contains("Enemy")){
                if (!isAttack){ return; }
                if (col.transform.root == transform.root){
                    return;
                }
                col.transform.GetComponent<TakeDamageEvent>().PlayEvent();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, Range);
    }
}