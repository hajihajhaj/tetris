using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class tetrisGrid : MonoBehaviour
{
    public int width = 10; // Width of the Tetris grid
    public int height = 20; // Height of the Tetris grid

    public Transform[,] grid; // Grid representation of the playing field
    public Transform[,] debugGrid; // Debugging visualization of the grid

    TetrisManager tetrisManager; // Reference to TetrisManager for scoring and game management

    void Start()
    {
        tetrisManager = FindObjectOfType<TetrisManager>(); // Find the TetrisManager in the scene

        grid = new Transform[width, height + 4]; // Initialize grid with extra space at the top
        debugGrid = new Transform[width, height]; // Initialize debug grid

        // Create debug cells to visualize the grid
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject cell = new GameObject($"Cell ({i}, {j})");
                cell.transform.position = new Vector3(i, j, 0);
                debugGrid[i, j] = cell.transform;
            }
        }
    }

    // Adds a block to the grid at the specified position
    public void AddBlockToGrid(Transform block, Vector2Int position)
    {
        grid[position.x, position.y] = block;
    }

    // Checks if a cell in the grid is occupied
    public bool IsCellOccupied(Vector2Int position)
    {
        // Return true if out of bounds or occupied
        if (position.x < 0 || position.x >= width || position.y < 0 || position.y >= height + 4)
        {
            return true;
        }
        return grid[position.x, position.y] != null;
    }

    // Checks if an entire row is full
    public bool IsLineFull(int rowNumber)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, rowNumber] == null)
            {
                return false; // If any cell is empty, the row is not full
            }
        }
        return true; // Row is completely filled
    }

    // Clears a full row by destroying blocks
    public void ClearLine(int rowNumber)
    {
        for (int x = 0; x < width; x++)
        {
            Destroy(grid[x, rowNumber].gameObject); // Destroy the block
            grid[x, rowNumber] = null; // Remove reference in the grid
        }
    }

    // Clears all full lines and shifts down remaining blocks
    public void ClearFullLines()
    {
        int linesCleared = 0;
        for (int y = 0; y < height; y++)
        {
            if (IsLineFull(y))
            {
                ClearLine(y);
                ShiftRowsDown(y);
                y--; // Recheck the current row after shifting
                linesCleared++;
            }
        }
        if (linesCleared > 0)
        {
            tetrisManager.CalculateScore(linesCleared); // Update score
        }
    }

    // Moves all blocks above a cleared row down by one position
    public void ShiftRowsDown(int clearedRow)
    {
        for (int y = clearedRow; y < height - 1; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[x, y] = grid[x, y + 1]; // Move block down
                if (grid[x, y] != null)
                {
                    grid[x, y].position += Vector3.down; // Adjust position in Unity
                }
                grid[x, y + 1] = null; // Clear old position
            }
        }
    }

    // Debugging function to draw the grid in the Unity Editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.black; // Set debug color
        if (debugGrid != null)
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Gizmos.DrawWireCube(debugGrid[i, j].position, Vector3.one); // Draw grid cells
                }
            }
        }
    }
}
