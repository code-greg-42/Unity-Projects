using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using StarterAssets;

[RequireComponent(typeof(Animator))]
public class Gun: MonoBehaviour {
    [SerializeField]
    private Transform bulletSpawnPoint;
    [SerializeField]
    private float fireRate = 0.11f;
    [SerializeField]
    private int roundsPerMagazine = 50;
    [SerializeField]
    private int reserveRounds = 250;
    [SerializeField]
    public bool isUnlimitedAmmoMode = true;
    [SerializeField]
    public float damage = 33.4f;
    public GameObject inputs;
    private LayerMask mask;
    private Animator anim;
    public GameObject muzzleFlash;
    public GameObject impactEffect;
    public UIManager uiManager;
    private float nextFireTime = 0.0f;
    private int currentRounds = 0;
    private int totalShots = 0;
    private int totalHits = 0;
    private bool isReloading = false;
    private AudioSource audioSource;
    private void Awake() {
        anim = GetComponent<Animator>();
        anim.SetTrigger("ForceIdle");
        currentRounds = roundsPerMagazine;
        audioSource = transform.GetComponent<AudioSource>();
    }
    private void Start() {
        uiManager.UpdateGunComponents(totalHits, totalShots, currentRounds, roundsPerMagazine);
    }
    private void Update() {
    if (Input.GetMouseButton(0) && Time.time >= nextFireTime && !isReloading)
    {
        Debug.Log("Shooting");
        Shoot();
        nextFireTime = Time.time + fireRate;
    }
}
    public void Shoot() {
        if (currentRounds > 0) {
            currentRounds--;
            totalShots++;
            StartCoroutine(SingleShotCoroutine());
        }
        else
        { Reload(); }
    }
    IEnumerator SingleShotCoroutine() {
        anim.SetTrigger("Shoot");
        GameObject muzzleFlashInstance = Instantiate(muzzleFlash, bulletSpawnPoint.position, Quaternion.identity);
        Destroy(muzzleFlashInstance, 0.1f);
        
        // Get the middle of the screen
        Vector3 middleOfScreen = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        // Create a ray from the camera through the middle of the screen
        Ray ray = Camera.main.ScreenPointToRay(middleOfScreen);

        uiManager.UpdateGunComponents(totalHits, totalShots, currentRounds, roundsPerMagazine);
        audioSource.Play();

        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, Physics.DefaultRaycastLayers)) {
            // EnemyCube enemy = hit.collider.gameObject.GetComponent<EnemyCube>();
            EnemySkeleton enemy = hit.collider.gameObject.GetComponent<EnemySkeleton>();
        if (enemy != null) {
            Debug.Log("found an enemy! woo!");
            UIManager.Instance.ShowHitMarker();
            enemy.TakeDamage(damage);
            totalHits++;
            float enemyHealth = enemy.GetHealth();
            float enemyMaxHealth = enemy.GetMaxHealth();
            uiManager.UpdateEnemyUIComponents(enemyHealth, enemyMaxHealth);
        } else {
            Debug.Log("miss.");
        }
        }
        yield break;
    }
    public void Reload() {
        if (!isReloading && currentRounds < roundsPerMagazine) {
            Debug.Log("reloading!");
            anim.SetBool("Reload", true);
            isReloading = true;

            if (!isUnlimitedAmmoMode) {
                if (reserveRounds > 0 && reserveRounds >= roundsPerMagazine) {
                    reserveRounds -= roundsPerMagazine;
                    currentRounds = roundsPerMagazine;
                } else if (reserveRounds > 0 && reserveRounds < roundsPerMagazine) {
                    currentRounds = reserveRounds;
                    reserveRounds = 0;
                } else {
                    currentRounds = 0;
                }
            } else {
                currentRounds = roundsPerMagazine;
            }

            uiManager.UpdateGunComponents(totalHits, totalShots, currentRounds, roundsPerMagazine);
            Invoke("StopReloadingAnimation", 0.42f);
        }
    }
    // Method to stop the reloading animation
    void StopReloadingAnimation() {
        anim.SetBool("Reload", false);
        isReloading = false;
        Debug.Log("Reloaded! Current rounds: " + currentRounds + " Reserve rounds: " + reserveRounds);
    }
}