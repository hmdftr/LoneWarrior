using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator anim;

    [Range(0, 30)] public float StoppingDistance = 3f;
    [Range(0, 50)] public float speed = 15f;
    [Range(1, 100)] public float DamageForce = 25f;

    private Collider[] cols;
    [ReadOnly] public Transform detectedTarget = null;

    public string player_Tag = "Player";
    [Range(0, 100)] public float detectRange = 5f;

    [ReadOnly] public bool inAttackState = false;

    [Header("Patrol")]
    public Transform[] des;
    public int destPoint = 0;
    [ReadOnly] public Transform SelectedPath = null;


    private void Awake()
    {
        if (!agent)
        {
            agent = this.GetComponent<NavMeshAgent>();
        }
        
        if (!anim)
        {
            anim = this.GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        agent.stoppingDistance = StoppingDistance;

        cols = Physics.OverlapSphere(transform.position, detectRange);
        foreach(Collider col in cols)
        {
            if (col.transform.tag==player_Tag)
            {
                detectedTarget = col.transform;
            }
        }

        inAttackState = ((detectedTarget!=null && Vector3.Distance(transform.position, detectedTarget.position) <= StoppingDistance) ? true : false);

        if (detectedTarget)
        {
            agent.speed = speed;

            if (Vector3.Distance(transform.position, detectedTarget.position) <= StoppingDistance){
                anim.SetBool("run", false);
                agent.SetDestination(transform.position);
                transform.LookAt(new Vector3(detectedTarget.position.x, transform.position.y, detectedTarget.position.z));
            }
            else{
                agent.SetDestination(detectedTarget.position);
                anim.SetBool("run", true);
            }
        }
        else
        {
            agent.speed = (speed-1f);

            anim.SetBool("run", false);
            anim.SetBool("walk", true);

            if (SelectedPath!=null){
                GoToPoint();
                float distanceToTarget = Vector3.Distance(transform.position, SelectedPath.position);
                if(distanceToTarget <= 4f)
                {
                    RandomizePath();
                }
            }
            else{
                RandomizePath();
            }
        }

        if (inAttackState)
        {
            anim.SetTrigger("attack");
        }

        anim.SetBool("is Attack", inAttackState);
    }

    private void GoToPoint()
    {
        if (des.Length == 0)
        {
            return;
        }

        agent.SetDestination(SelectedPath.position);
    }

    private void RandomizePath()
    {
        Transform _des = des[Random.Range(0, des.Length)];
        SelectedPath = _des;
    }

    private void OnDrawGizmosSelected() 
    {
        if (detectedTarget!=null)
        {
            Gizmos.color = Color.red;
        }
        else
        {
            Gizmos.color = Color.green;
        }
        Gizmos.DrawWireSphere(transform.position, detectRange);
    }
}