using UnityEngine;
using System.Collections;
public class Bow : MonoBehaviour {
    [SerializeField] private float damage = 5f;
    [SerializeField] private float range = 500f;
    [SerializeField] private float fireRate = 3f;
    [SerializeField] private bool isEquipped = true;
    [Header("Ammo Related")]
    [SerializeField] private int currentAmmoTotal = 60;
    [SerializeField] private int maxMagAmmo = 6;
    [SerializeField] private int currentMagAmmo = 3;
    [SerializeField] private float reloadSpeed = 1f; //TODO:
    [SerializeField] AudioClip[] weaponFireClips;
    [SerializeField] UIController uiController;
    private Camera bowCamera;
    private AudioSource audioSource;
    

    private float nextTimeToFire = 0f;
    private ParticleSystem muzzleFlashParticle;
    
    void Awake() {
        bowCamera = GetComponentInParent<Camera>();
        audioSource = GetComponent<AudioSource>();
        muzzleFlashParticle = GetComponentInChildren<ParticleSystem>();
        uiController.UpdateAmmo(currentMagAmmo, currentAmmoTotal);
    }
    void Update() 
    {
        if (isEquipped) {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire) {
                Debug.Log("Click");
                nextTimeToFire = Time.time + 1f/fireRate;
                Shoot();
            }
            if (Input.GetButtonDown("Reload")) {
                Reload();
                Debug.Log("RELOADING!!");
            }
        }
    }
    void Shoot()
    {
        Debug.Log("Could not fire");
        if (currentMagAmmo > 0) {
            Debug.Log("Boom");
            currentMagAmmo -= 1;
            uiController.UpdateAmmo(currentMagAmmo, currentAmmoTotal);
            ShootAudio();
            muzzleFlashParticle.Play();

            
            Debug.DrawRay(bowCamera.transform.position, bowCamera.transform.forward * range, Color.red, 2f);

            RaycastHit hit;
            if (Physics.Raycast(bowCamera.transform.position, bowCamera.transform.forward, out hit, range)) {
                NPCController npc = hit.transform.root.GetComponent<NPCController>();
                if (npc != null) {
                    npc.HealthChange(damage);
                }
                else if (hit.transform.GetComponent<Rigidbody>()){
                    Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                    rb.AddForce(bowCamera.transform.forward * (damage * 2));
                }
            }
        }
    }
    void ShootAudio() 
    {   
        if (weaponFireClips.Length > 0) {
            int nextFireSound = Random.Range(0, weaponFireClips.Length);

            audioSource.PlayOneShot(weaponFireClips[nextFireSound]);
        }
        else {
            Debug.Log("No audioClips in: " + gameObject.name);
        }
    }
    private void Reload()
    {
        if (currentMagAmmo < maxMagAmmo && currentAmmoTotal > 0) {
            currentAmmoTotal += currentMagAmmo;
            currentMagAmmo = 0;

            if (maxMagAmmo >= currentAmmoTotal) {
                currentMagAmmo = maxMagAmmo;
                currentAmmoTotal -= maxMagAmmo;
            } else {
                while (currentMagAmmo < maxMagAmmo && currentAmmoTotal > 0) {
                    currentMagAmmo += 1;
                    currentAmmoTotal -= 1;
                }
            }
        }
        else {
            return;
        }
        uiController.UpdateAmmo(currentMagAmmo, currentAmmoTotal);
    }
    public void Equip(bool setEquipped)
    {
        isEquipped = setEquipped;
    }
}
