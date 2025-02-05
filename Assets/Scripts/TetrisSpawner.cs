using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisSpawner : MonoBehaviour
{
    public GameObject[] tetrominoPrefabs; // Array of Tetromino prefabs
    private tetrisGrid grid; // Reference to the Tetris grid
    private GameObject nextPiece; // Stores the next Tetromino piece

    // Start is called before the first frame update
    void Start()
    {
        grid = FindObjectOfType<tetrisGrid>(); // Find the Tetris grid component in the scene
        if (grid == null)
        {
            // Error handling: Exit if grid is not found
            Debug.LogError("TetrisGrid reference is missing in TetrisSpawner!");
            return;
        }
        SpawnPiece(); // Spawn the first Tetromino piece
    }

    public void SpawnPiece()
    {
        // Calculate the top center of the grid for spawning a new piece
        Vector3 spawnPosition = new Vector3(
            Mathf.Floor(grid.width / 2), // X position: Horizontal center of the grid
            grid.height,                 // Y position: At the top of the grid
            0);                           // Z position (not used in 2D)

        if (nextPiece != null)
        {
            nextPiece.SetActive(true); // Activate the next piece
            nextPiece.transform.position = spawnPosition; // Move it to spawn position
        }
        else
        {
            nextPiece = InstantiateRandomPiece(); // Instantiate a random Tetromino
            nextPiece.transform.position = spawnPosition;
        }

        // Prepare the next piece by instantiating a new random Tetromino
        nextPiece = InstantiateRandomPiece();
        nextPiece.SetActive(false); // Deactivate until it's the next piece
    }

    public GameObject InstantiateRandomPiece()
    {
        int index = Random.Range(0, tetrominoPrefabs.Length); // Select a random Tetromino index
        return Instantiate(tetrominoPrefabs[index]); // Instantiate and return the selected Tetromino
    }
}