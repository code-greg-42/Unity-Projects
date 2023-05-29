using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyCube : MonoBehaviour
{
    public float health = 100.0f;
    public float maxHealth = 100.0f;
    public float speed = 3.0f;
    public GameObject deathEffect;
    public float stopDistance = 8.0f;
    public bool isHead = false;
    public Color[] colors;
    public int[] pointValues;
    public UIManager uiManager;
    public AudioSource audioSourceBody;
    private Rigidbody rb;
    private Transform playerTransform;
    private Vector3 randomDirection;
    private float timeUntilRandomChange = 0f;
    private float maxTimeUntilRandomChange = 10f;
    private int randomColorIndex;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        uiManager = GameObject.Find("Manager").GetComponent<UIManager>();
        randomDirection = GetRandomDirection();
        colors = new Color[] { Color.green, Color.green, Color.green, Color.blue, Color.blue, new Color(255, 173, 0), new Color(255, 173, 0), new Color(162, 0, 255)};
        pointValues = new int[] { 1, 1, 1, 2, 2, 5, 5, 10};
        randomColorIndex = Random.Range(0, colors.Length);
        GetComponent<Renderer>().material.color = colors[randomColorIndex];
        health = pointValues[randomColorIndex] * 35;
        maxHealth = health;
    }
    void Update()
    {
        // Move towards the player
        float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);
        if (distanceToPlayer > stopDistance) {
            transform.LookAt(playerTransform);
            rb.velocity = transform.forward * speed;
        } else {
            rb.velocity = Vector3.zero;
        }
        // Move randomly
        timeUntilRandomChange -= Time.deltaTime;
        if (timeUntilRandomChange <= 0f) {
            randomDirection = GetRandomDirection();
            timeUntilRandomChange = Random.Range(0.5f, maxTimeUntilRandomChange);
        }
        rb.velocity += randomDirection * speed / 2f;
        // Die if health is 0
        if (health <= 0)
        { 
            Die();
        }
    }
    private Vector3 GetRandomDirection() {
        return new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized;
    }
    public virtual void TakeDamage()
    {
        Debug.Log("Hit! Enemy Starting Health: " + health);
        health -= 33.4f;
        Debug.Log("\nEnemy Current Health: " + health);
        rb.AddForce(30.0f * -transform.forward, ForceMode.Impulse);
        if (health <= 0 || isHead)
        {
            if (isHead) {
                if (transform.parent != null && transform.parent.GetComponent<EnemyCube>() != null)
                {
                    transform.parent.GetComponent<EnemyCube>().Die();
                }
            } else {
                if (transform.childCount > 0 && transform.GetChild(0).GetComponent<EnemyCube>() != null)
                {
                    transform.GetChild(0).GetComponent<EnemyCube>().Die();
                }
            }
            Die();
        }
        audioSourceBody.Play();
    }
    public virtual void Die()
    {
        Instantiate(deathEffect, transform.position, Quaternion.identity);
        if (isHead) {
            uiManager.AddScore(pointValues[randomColorIndex]);
        } else {
            uiManager.AddScore(pointValues[randomColorIndex] * 3);
        }
        Destroy(gameObject);
    }
    public float GetHealth() {
        return health;
    }
    public float GetMaxHealth() {
        return maxHealth;
    }
}
