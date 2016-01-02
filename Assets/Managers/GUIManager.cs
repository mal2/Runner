using UnityEngine;

public class GUIManager : MonoBehaviour
{

    public GUIText boostsText, distanceText, gameOverText, instructionsText, runnerText;
    private static GUIManager instance;

    void Start()
    {
        instance = this;
        GameEventManager.GameStart += GameStart;
        GameEventManager.GameOver += GameOver;
        gameOverText.enabled = false;
    }

    public static void SetBoosts(int boosts)
    {
        instance.boostsText.text = boosts.ToString();
    }

    public static void SetDistance(float distance)
    {
        instance.distanceText.text = distance.ToString("f0");
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            AudioManager.PlayGUI();
            GameEventManager.TriggerGameStart();

        }
    }

    private void GameStart()
    {
        AudioManager.PlayBM();
        gameOverText.enabled = false;
        instructionsText.enabled = false;
        runnerText.enabled = false;
        enabled = false;
    }

    private void GameOver()
    {
        AudioManager.StopBM();
        AudioManager.PlayDeath();
        PlayerPrefs.SetInt("highscore", (int)Runner.distanceTraveled);
        PlayerPrefs.Save();
        gameOverText.enabled = true;
        instructionsText.enabled = true;
        enabled = true;
    }
}