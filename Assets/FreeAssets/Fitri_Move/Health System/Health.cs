using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    [Range(0, 100)] public float currenthealth = 0.0f;
    [Range(0, 25)] public float smoothness = 7f;

    public Slider healthSlider;

    private float healthLerped = 0.0f;
    public float maxHealth = 100f;

    public UnityEvent DeathEvent;

    private Vector3 start_pos;

    private void Start()
    {
        healthLerped = 100;
        //healthLerped = maxHealth;

        start_pos = transform.position;
    }

    private void Update()
    {
        //Debug.Log($"Health = {healthLerped}");

        currenthealth = Mathf.Lerp(currenthealth, healthLerped, smoothness * Time.deltaTime);
        currenthealth = Mathf.Clamp(currenthealth, 0, maxHealth);

        if (healthSlider != null)
        {
            healthSlider.value = currenthealth;
        }

        if (currenthealth <= 0)
        {
            DeathEvent.Invoke();
        }
    }

    public void AddHealth(float amount)
    {
        healthLerped += amount;

        if (currenthealth > maxHealth)
            currenthealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        healthLerped -= Random.Range(0, amount);
    }

    public void ResetHealth()
    {
        healthLerped = maxHealth;
    }

    public void DamageSelf()
    {
        Destroy(this.gameObject);
    }

    public void FallDeath()
    {
        this.GetComponentInChildren<Animator>().enabled = false;
        if (this.GetComponent<UnityEngine.AI.NavMeshAgent>())
        {
            this.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
        }
        if (this.GetComponent<CapsuleCollider>())
        {
            this.GetComponent<CapsuleCollider>().enabled = false;
        }
        if (this.GetComponent<EnemyAI>())
        {
            this.GetComponent<EnemyAI>().enabled = false;
        }
        if (this.GetComponentInChildren<Canvas>())
        {
            this.GetComponentInChildren<Canvas>().enabled = false;
        }

        StartCoroutine(DestroyAfterTime(5f));
    }

    public IEnumerator DestroyAfterTime(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }

    public void GoToIntialPosition()
    {
        if (this.GetComponent<CharacterController>())
        {
            this.GetComponent<CharacterController>().enabled = false;
        }
        transform.position = start_pos;
        if (this.GetComponent<CharacterController>())
        {
            this.GetComponent<CharacterController>().enabled = true;
        }
    }
}