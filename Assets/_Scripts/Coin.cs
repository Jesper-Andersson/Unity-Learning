using UnityEngine;
public class Coin : MonoBehaviour
{
    [SerializeField]
    public bool isSpinning = true;
    [SerializeField, Range(0.1f, 1000f)]
    public float spinSpeed = 1f;
    [SerializeField]
    AudioClip coinSound;
    [SerializeField]
    public float coinVolume=0.5f;
    [SerializeField, Range(-100, 100)]
	int coinValue = 10;
    GameObject cameraObject;
    [SerializeField]
    UIController ui; 
    void Update() {
        Spin();
    }
    public void Spin() {
        if (isSpinning) {
            transform.Rotate(spinSpeed * Time.deltaTime, 0f, 0f);
        }
    }
    void OnTriggerEnter(Collider other) { //FIXME: SOUND
		if (other.gameObject.tag == "Player") 
		{
            ui.Score(coinValue);
			Destroy(gameObject);
        }
    }
}