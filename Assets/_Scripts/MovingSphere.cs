using UnityEngine;
/*
https://catlikecoding.com/unity/tutorials/movement/sliding-a-sphere/
	TODO:
	- Animations + Character 
	- Switch from RB
	- Slow walk/Crouching
	- Sprint
	- Health Recovery
	- Stamina Recovery
	- Deathstate
	- Rename Script (PlayerController(?)

	FIXME:
	- Refactor vaiable mess(?)
	- Problems
*/
public class MovingSphere : MonoBehaviour {
	[SerializeField, Range(0f, 100f)]
	float maxAcceleration = 10f, maxAirAcceleration = 1f, maxSpeed = 10f;
	[SerializeField, Range(0f, 15f)]
	float jumpHeight = 2f;
	[SerializeField, Range(0, 10)]
	int maxAirJumps = 0;
	[SerializeField]
	Transform playerInputSpace = default;
	[SerializeField]
	Vector2 playerInput;
	[SerializeField]
    UIController ui;
	[SerializeField]
	AudioClip coinPickupSound;
	[SerializeField]
	float jumpCost = 20f;
	[SerializeField]
	float staminaRegenRate = 0.5f;
	AudioSource playerAudioSource;
	Vector3 velocity, desiredVelocity;
	Rigidbody body;
	bool desiredJump, onGround;
	int jumpPhase;
	float maxHealth = 100f, maxStamina = 100f;
	public float Health = 100f, Stamina = 100f;
	void Awake() {
		body = GetComponent<Rigidbody>();
		playerAudioSource = this.GetComponent<AudioSource>();
		Cursor.visible = false;
		
	}
	void Update() {
		playerInput.x = Input.GetAxis("Horizontal");
		playerInput.y = Input.GetAxis("Vertical");
		playerInput = Vector2.ClampMagnitude(playerInput, 1f);
		Debug.Log(playerInput.x + playerInput.y);
	
		if (playerInputSpace) {
			Vector3 forward = playerInputSpace.forward;
			forward.y = 0f;
			forward.Normalize();

			Vector3 right = playerInputSpace.right;
			right.y = 0f;
			right.Normalize();

			desiredVelocity =
				(forward * playerInput.y + right * playerInput.x) * maxSpeed;
		}
		else {
			desiredVelocity = 
				new Vector3(playerInput.x, 0f, playerInput.y) * maxSpeed;
		}
		desiredJump |= Input.GetButtonDown("Jump");
	}
	void FixedUpdate() {
		UpdateState();
		body.AddTorque(0f, playerInput.x, 0f);
		float acceleration = onGround ? maxAcceleration : maxAirAcceleration;
		float maxSpeedChange = acceleration * Time.deltaTime;

		velocity.x =
			Mathf.MoveTowards(velocity.x, desiredVelocity.x, maxSpeedChange);
		velocity.z =
			Mathf.MoveTowards(velocity.z, desiredVelocity.z, maxSpeedChange);
		
		if (desiredJump) {
			desiredJump = false;
			Jump();
		}
		if (Stamina < maxStamina){ //FIXME: Slower when moving, faster when still
			Stamina = Stamina + staminaRegenRate;
		}
		body.velocity = velocity;
		onGround = false;
	}
	void UpdateState() {
		velocity = body.velocity;
		if (onGround) {
			jumpPhase = 0;
		}
	}
	void EvaluateCollision(Collision collision) {
		for (int i = 0; i < collision.contactCount; i++) {
			Vector3 normal = collision.GetContact(i).normal;
			onGround |= normal.y > 0.9f;
		}
	}
	void Jump() {
		if ((onGround || jumpPhase < maxAirJumps) && Stamina > jumpCost) {
			jumpPhase += 1;
			float jumpSpeed = Mathf.Sqrt(-2f * Physics.gravity.y * jumpHeight);
			if (velocity.y > 0f) {
				jumpSpeed = Mathf.Max(jumpSpeed - velocity.y, 0f);
			}
			velocity.y += jumpSpeed;
			Stamina -= jumpCost;
		}
	}
	void OnCollisionEnter(Collision collision) {
		EvaluateCollision(collision);
	}
	void OnCollisionStay(Collision collision) {
		EvaluateCollision(collision);
	}
}