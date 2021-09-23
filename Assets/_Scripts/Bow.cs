using UnityEngine;

public class Bow : MonoBehaviour {
    public float damage = 5f;
    public float range = 500f;
    public float fireRate = 15f;
    public bool isEquipped = true;
    private Camera bowCamera;
    private float nextTimeToFire = 0f;
    

    private void Start() {
        bowCamera = GetComponentInParent<Camera>();
    }

    // Update is called once per frame
    void Update() {
        if (isEquipped) {
            if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire) {
                nextTimeToFire = Time.time + 1f/fireRate;
                Shoot();
            }
        }
    }

    void Shoot(){
        RaycastHit hit;
        Debug.DrawRay(bowCamera.transform.position, bowCamera.transform.forward * range, Color.red, 2f);
        if (Physics.Raycast(bowCamera.transform.position, bowCamera.transform.forward, out hit, range)) {
            NPCController npc = hit.transform.root.GetComponent<NPCController>();
            if (npc != null) {
                npc.HealthChange(damage);
            }
            if (hit.transform.GetComponent<Rigidbody>()){
                Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
                rb.AddForce(bowCamera.transform.forward * (damage * 2));
            }
        }
    }
}
