using UnityEngine;
using UnityEngine.AI;
using System.Collections;

//https://catlikecoding.com/unity/tutorials/movement/sliding-a-sphere/

/*
    TODO:
    + + +
    - Pathfinding to player/target (Navmesh) 
    - Attacks (Proximity, Raycast)
    - States (Hostile, Neutral)
    + +
    - Last seen position of player (Raycast?)
    +
    - Stamina and usage
    - Groups/Faction tags
    - Jumping over small obsta
*/

public class NPCController : MonoBehaviour {
    [SerializeField]
    Transform target;
    NavMeshAgent agent;
    private Animator animator;
    private Vector3 lastSeenPosition;
    public bool isDead = false;
	public float health = 100.0f;
    Vector3 dir;
	void Start() 
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
	}
    void Update() 
    {
        if (health <= 0.0f || isDead) {
            Death();
        }
        if (!isDead) {
            FollowPlayer();
        }
    }
    public void HealthChange(float change) 
    {
        health -= change;
    }
    void FollowPlayer() 
    {
        CanSee();

        float distance = Vector3.Distance(gameObject.transform.position, lastSeenPosition);

        if (distance >= 2f) {
            animator.SetBool("isChasing", true);
            animator.SetBool("isAttacking", false);

            agent.destination = lastSeenPosition;
            agent.isStopped = false;
        }
        else {

            if (CanSee()) {
                if(!animator.GetBool("isAttacking")){
                    animator.SetBool("isAttacking", true);
                    animator.SetBool("isChasing", false);
                    Attack();
                }
            }
            else {
                if(animator.GetBool("isChasing")) {
                    animator.SetBool("isChasing", false);
                    animator.SetBool("isAttacking", false);
                }
            }
            agent.isStopped = true;

            return;
        }
    }
    public bool CanSee() 
    {
        Vector3 sightOrigin = gameObject.transform.position + (1.8f * Vector3.up);
        Vector3 sightTarget = target.transform.position;
        Vector3 dir = (sightTarget - sightOrigin).normalized;

        Debug.DrawRay(sightOrigin, dir, Color.red, 3f);
        RaycastHit hit;

        if (Physics.Raycast(sightOrigin, dir, out hit, 100f)) {
            if (hit.transform == target.transform) {
                lastSeenPosition = hit.transform.position;
                Debug.DrawRay(lastSeenPosition, Vector3.up, Color.green, 10f);
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
    }
    public void Attack() 
    {
        RaycastHit hit;

        Vector3 sightOrigin = gameObject.transform.position + (1.8f * Vector3.up);
        Vector3 sightTarget = target.transform.position;
        Vector3 dir = (sightTarget - sightOrigin).normalized;

        if (Physics.Raycast(sightOrigin, dir, out hit, 5f)) {

            NPCController npc = hit.transform.GetComponent<NPCController>();
            FirstPersonController player = hit.transform.GetComponent<FirstPersonController>();

            if(npc != null){
                
            }
            else if(player != null){
                player.HealthChange(-1f);
            }
        }
    }

    void Death() 
    {
        isDead = true;
        health = 0f;
        
        //Setting up for ragdoll
        Rigidbody[] rbParts = GetComponentsInChildren<Rigidbody>();
        Collider[] colliderParts = GetComponentsInChildren<Collider>();
        for (int i = 0; i < rbParts.Length; i++)
        {
            rbParts[i].isKinematic = false;
            colliderParts[i].enabled = true;
        }
        Destroy(GetComponent<Animator>());
        Destroy(GetComponent<Collider>());
        Destroy(agent);
        Destroy(this);
    }
}