using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText; // UI text element displaying the timer
    [SerializeField] float remainingTime; // Countdown timer value
    [SerializeField] GameObject gameOverScreen; // Reference to Game Over UI panel
    [SerializeField] TextMeshProUGUI highScoreText; // Reference to High Score UI text
    [SerializeField] Button restartButton; // Restart button reference
    [SerializeField] TetrisManager tetrisManager; // Reference to TetrisManager for updating scores

    private bool isGameOver = false; // Flag to check if the game is over

    void Start()
    {
        // Ensure Game Over UI elements are hidden at the start
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (restartButton != null) restartButton.gameObject.SetActive(false);
    }

    void Update()
    {
        // Run timer countdown if the game is still active
        if (!isGameOver)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime; // Decrease time every frame
            }
            else
            {
                remainingTime = 0;
                TriggerGameOver(); // Call Game Over function when time runs out
            }

            // Convert time to minutes and seconds format for display
            int minutes = Mathf.FloorToInt(remainingTime / 60);
            int seconds = Mathf.FloorToInt(remainingTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    void TriggerGameOver()
    {
        isGameOver = true; // Set game over flag
        timerText.color = Color.red; // Change timer text color to red

        // Display Game Over UI
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true);
        }

        // Enable restart button and add restart functionality
        if (restartButton != null)
        {
            restartButton.gameObject.SetActive(true);
            restartButton.onClick.AddListener(RestartGame);
        }

        // Update high score using TetrisManager, if available
        if (tetrisManager != null)
        {
            tetrisManager.UpdateHighScore(true);
        }
        else
        {
            Debug.LogError("TetrisManager reference is missing in Timer script!");
        }

        Time.timeScale = 0; // Pause the game
    }

    void RestartGame()
    {
        Time.timeScale = 1; // Resume game speed
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name); // Reload current scene
    }
}
