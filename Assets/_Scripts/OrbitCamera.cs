using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OrbitCamera : MonoBehaviour
{
    [SerializeField]
    Transform focus = default;
	[SerializeField]
	float maxDistance = 10f;
	[SerializeField]
	float minDistance = 1f;
	[SerializeField]
	float zoomScale = 0.5f;
    [SerializeField, Range(1f, 20f)]
    float distance = 5f;
    [SerializeField, Min(0f)]
    float focusRadius = 1f;
    [SerializeField, Range(0f, 1f)]
    float focusCentering = 0.5f;
	[SerializeField, Range(1f, 360f)]
	float rotationSpeed = 90f;
	[SerializeField, Range(-89f, 89f)]
	float minVerticalAngle = -30f, maxVerticalAngle = 60f;
	[SerializeField, Min(0f)]
	float alignDelay = 5f;
	[SerializeField, Range(0f, 90f)]
	float alignSmoothRange = 45f;
	float lastManualRotationTime;
    Vector3 focusPoint, previousFocusPoint;
	Vector2 orbitAngles = new Vector2(45f, 0f);
	Camera regularCamera;

    void Awake() {
		regularCamera = GetComponent<Camera>();
        focusPoint = focus.position;
		transform.localRotation = Quaternion.Euler(orbitAngles);
    }
    void LateUpdate() {
        UpdateFocusPoint();
		ManualRotation();
		
		Quaternion lookRotation;
		
		if(Input.mouseScrollDelta.y > 0f){
			if(distance > minDistance){
				distance -= zoomScale;
			}
		}
		else if (Input.mouseScrollDelta.y < 0f){
			if(distance < maxDistance){
				distance += zoomScale;
			}
		}

		if (ManualRotation() || AutomaticRotation()) {
			ConstrainAngles();
			lookRotation = Quaternion.Euler(orbitAngles);
		}
		else {
			lookRotation = transform.localRotation;
		}

        Vector3 lookDirection = transform.forward;
		Vector3 lookPosition = focusPoint - lookDirection * distance;

		if (Physics.BoxCast(
			focusPoint, CameraHalfExtends, -lookDirection, out RaycastHit hit, 
			lookRotation, distance - regularCamera.nearClipPlane
		)) {
			lookPosition =
				focusPoint - lookDirection * (hit.distance + regularCamera.nearClipPlane);
		}

        transform.SetPositionAndRotation(lookPosition, lookRotation);    
    }
    void UpdateFocusPoint() {
		previousFocusPoint = focusPoint;
        Vector3 targetPoint = focus.position;
        if (focusRadius > 0f) {
            float distance = Vector3.Distance(targetPoint, focusPoint);
            float t = 1f;

            if (distance > 0.1f && focusCentering > 0f) {
                t = Mathf.Pow(1f - focusCentering, Time.deltaTime);
            }

            if (distance > focusRadius) {
                t = Mathf.Min(t, focusRadius / distance);
            }
			focusPoint = Vector3.Lerp(targetPoint, focusPoint, t);
        }
        else {
            focusPoint = targetPoint;
        }
    }
	bool ManualRotation () { //FIXME: Rotation Stutter
		Vector2 input = new Vector2(
			Input.GetAxis("Vertical Camera"),
			Input.GetAxis("Horizontal Camera")
		);
		const float e = 0.001f;
		if (input.x < -e || input.x > e || input.y < -e || input.y > e) {
			orbitAngles += rotationSpeed * Time.unscaledDeltaTime * input;
			lastManualRotationTime = Time.unscaledTime;
			return true;
		}

		return false;
	}
	bool AutomaticRotation() {
		if (Time.unscaledTime - lastManualRotationTime < alignDelay) {
			return false;
		}
		
		Vector2 movement = new Vector2(
			focusPoint.x - previousFocusPoint.x,
			focusPoint.z - previousFocusPoint.z
		);
		float movementDeltaSqr = movement.sqrMagnitude;
		if (movementDeltaSqr < 0.000001f) {
			return false;
		}
		
		
		float headingAngle = GetAngle(movement / Mathf.Sqrt(movementDeltaSqr));
		float deltaAbs = Mathf.Abs(Mathf.DeltaAngle(orbitAngles.y, headingAngle));
		float rotationChange = 
			rotationSpeed * Mathf.Min(Time.unscaledDeltaTime, movementDeltaSqr);
		if (deltaAbs < alignSmoothRange){
			rotationChange *= deltaAbs / alignSmoothRange;
		}
		else if (180f - deltaAbs < alignSmoothRange) {
			rotationChange *= (180f - deltaAbs) / alignSmoothRange;
		}
		
		orbitAngles.y =
			Mathf.MoveTowardsAngle(orbitAngles.y, headingAngle, rotationChange);
		return true;
	}
	void OnValidate() {
		if (maxVerticalAngle < minVerticalAngle) {
			maxVerticalAngle = minVerticalAngle;
		}	
	}
	void ConstrainAngles () {
		orbitAngles.x =
			Mathf.Clamp(orbitAngles.x, minVerticalAngle, maxVerticalAngle);
		
		if (orbitAngles.y < 0f) {
			orbitAngles.y += 360f;
		}
		else if (orbitAngles.y >= 360f) {
			orbitAngles.y -= 360f;
		}
	}
	static float GetAngle (Vector2 direction) {
		float angle = Mathf.Acos(direction.y) * Mathf.Rad2Deg;
		return direction.x < 0f ? 360f - angle : angle;
	}
	Vector3 CameraHalfExtends {
		get {
			Vector3 halfExtends;
			halfExtends.y =
				regularCamera.nearClipPlane *
				Mathf.Tan(0.5f * Mathf.Deg2Rad * regularCamera.fieldOfView);
			halfExtends.x = halfExtends.y * regularCamera.aspect;
			halfExtends.z = 0f;
			return halfExtends;
		}
	}
}