using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySkeleton : MonoBehaviour
{
    public float currentHealth = 100.0f;
    public float maxHealth = 100.0f;
    public float movementSpeed = 15.0f;
    public UIManager uiManager;
    //public AudioSource audioSource;
    private Rigidbody rb;
    private Transform playerTransform;
    private Animator anim;
    private int rank = 1;
    private int[] ranks = new int[] { 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 4 };
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        uiManager = GameObject.Find("Manager").GetComponent<UIManager>();
        rank = ranks[Random.Range(0, ranks.Length)];
        Debug.Log("Enemy of rank " + rank + " has spawned.");
        maxHealth = rank * 100;
        currentHealth = maxHealth;
        transform.localScale = new Vector3(rank * 10, rank * 10, rank * 10);
        gameObject.SetActive(true);
        anim.SetBool("Run", true);
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);
        rb.velocity = directionToPlayer * (movementSpeed + rank * Time.deltaTime);
    }
    void Update() {
        Vector3 directionToPlayer = (playerTransform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(directionToPlayer);
        rb.velocity = directionToPlayer * (movementSpeed + rank * Time.deltaTime);
        anim.SetBool("Run", true);
    }
    public virtual void TakeDamage(float damage)
    {
        anim.SetTrigger("Damage");
        Debug.Log("i've been hit! Current health: " + currentHealth);
        currentHealth -= damage;
        uiManager.AddScore(damage * rank);
        rb.AddForce(10.0f * -transform.forward, ForceMode.Impulse);
        if (currentHealth <= 0)
        {
            Die();
        }
        //audioSource.Play();
    }
    public float GetHealth() {
        return currentHealth;
    }
    public float GetMaxHealth() {
        return maxHealth;
    }
    private void Die() {
        anim.SetTrigger("Death");
        Invoke("SelfDestruct", 2.0f);
    }
    private void SelfDestruct() {
        Destroy(gameObject);
    }
}
