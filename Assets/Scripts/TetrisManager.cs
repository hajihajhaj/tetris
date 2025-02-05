using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class TetrisManager : MonoBehaviour
{
    private tetrisGrid grid; // Reference to the Tetris grid
    public int score; // Player's score

    [SerializeField] GameObject gameOverScreen; // UI element for Game Over screen
    [SerializeField] TextMeshProUGUI HighScoreText; // UI text to display high score
    [SerializeField] TextMeshProUGUI InGameHighScoreText; // Additional UI text to display high score in-game
    [SerializeField] Button RestartButton; // UI button to restart the game
    [SerializeField] TextMeshProUGUI ScoreText; // UI text to display the current score
    [SerializeField] TetrisSpawner tetrisSpawner; // Reference to the TetrisSpawner script

    private int highScore; // Stores the highest score

    void Start()
    {
        grid = FindObjectOfType<tetrisGrid>(); // Find the Tetris grid in the scene
        LoadHighScore(); // Load high score when the game starts
        ResetGameState(); // Reset necessary game states

        // Hide UI elements at start
        if (gameOverScreen != null) gameOverScreen.SetActive(false);
        if (RestartButton != null) RestartButton.gameObject.SetActive(false);

        UpdateHighScoreText(); // Ensure the UI reflects the correct high score at start
    }

    void Update()
    {
        CheckGameOver(); // Check if the game should end
    }

    // Calculates the score based on lines cleared
    public void CalculateScore(int linesCleared)
    {
        switch (linesCleared)
        {
            case 1: score += 100; break;
            case 2: score += 300; break;
            case 3: score += 500; break;
            case 4: score += 800; break;
        }

        Debug.Log("New Score: " + score);

        // Update score UI text
        if (ScoreText != null)
        {
            ScoreText.text = "Score: " + score;
        }

        // Check and update high score if necessary
        UpdateHighScore(true);
    }

    // Checks if the game is over
    public void CheckGameOver()
    {
        for (int i = 0; i < grid.width; i++)
        {
            // If any cell at the top row is occupied, trigger Game Over
            if (grid.IsCellOccupied(new Vector2Int(i, grid.height - 1)))
            {
                Debug.Log("Game Over triggered!");

                // Show Game Over screen
                if (gameOverScreen != null)
                {
                    gameOverScreen.SetActive(true);
                }

                // Show Restart button
                if (RestartButton != null)
                {
                    RestartButton.gameObject.SetActive(true);
                }

                // Disable the spawner to stop spawning pieces
                if (tetrisSpawner != null) tetrisSpawner.gameObject.SetActive(false);

                UpdateHighScore(false); // Ensure the high score is saved before pausing the game
                UpdateHighScoreText(); // Ensure the UI reflects the latest high score
                Time.timeScale = 0; // Pause the game
            }
        }
    }

    // Updates and saves the high score if the current score is higher
    public void UpdateHighScore(bool liveUpdate)
    {
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save(); // Ensure PlayerPrefs is saved permanently
            Debug.Log("New high score saved: " + highScore);
        }

        if (liveUpdate)
        {
            UpdateHighScoreText(); // Ensure UI updates in real-time
        }
    }

    // Updates the High Score UI text
    private void UpdateHighScoreText()
    {
        if (HighScoreText != null)
        {
            HighScoreText.text = "High Score: " + highScore;
        }
        if (InGameHighScoreText != null)
        {
            InGameHighScoreText.text = "High Score: " + highScore;
        }
    }

    // Loads the saved high score when the game starts
    private void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        Debug.Log("High score loaded: " + highScore);
    }

    // Resets game state after restart
    private void ResetGameState()
    {
        Time.timeScale = 1; // Ensure the game runs normally after restart
        score = 0; // Reset score
        UpdateHighScoreText(); // Ensure UI updates properly
    }

    // Reloads the scene to restart the game
    public void ReloadScene()
    {
        ResetGameState(); // Reset necessary game states before reloading
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload current scene
    }
}
