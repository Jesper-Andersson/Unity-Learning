using UnityEngine;
using UnityEngine.UI;
using TMPro;
/*
    TODO:
    - Rename Script (UIController) DONE
    - NPC / Item Popup (Name/pick up/ talk)
    - ESC menu (Options, Exit)
*/
public class UIController : MonoBehaviour {
    [SerializeField] TMP_Text scoreText, hpText, staminaText, ammoText;
    [SerializeField] FirstPersonController player;
    [SerializeField] GameObject deathScreen;
    [SerializeField] GameObject hud;
    public int score = 0;
    void Start() {
        DisplayScore();
    }
    void Update() {
        DisplayScore();
        DisplayHealth();
        DisplayStamina();
    }
    public void Score(int coinValue) {
		score += coinValue;
	}
    void DisplayScore() {
        scoreText.text = "SCORE: " + score;
    }
    void DisplayHealth() {
        hpText.text = "HP: " + (int)player.health;
    }
    void DisplayStamina() {
        staminaText.text = "STAMINA: " + (int)player.stamina;
    }
    public void DeathScreen() {
        hud.SetActive(false);
        deathScreen.SetActive(true);
    }
    void DisplayAmmo(int mag, int total) {
        ammoText.text = mag + " / " + total;
    }

    public void UpdateAmmo(int magAmmo, int totalAmmo) {
        //DisplayAmmo(magAmmo, totalAmmo);
    }
}