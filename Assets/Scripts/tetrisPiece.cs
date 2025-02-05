using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tetrisPiece : MonoBehaviour
{
    private tetrisGrid grid; // Reference to the Tetris grid script
    private float dropInterval = 1f; // Interval in seconds before piece automatically drops
    private float dropTimer; // Timer to track automatic drop timing
    bool isLocked = false; // Flag to check if the piece is locked in place

    // Start is called before the first frame update
    void Start()
    {
        grid = FindObjectOfType<tetrisGrid>(); // Find the tetrisGrid in the scene
        dropTimer = dropInterval; // Initialize drop timer
    }

    // Update is called once per frame to handle movement and automatic dropping
    void Update()
    {
        HandleAutomaticDrop(); // Handles piece falling over time

        // Check for player input to move, rotate, or drop the piece
        if (Input.GetKeyDown(KeyCode.LeftArrow)) { Move(Vector3.left); }
        if (Input.GetKeyDown(KeyCode.RightArrow)) { Move(Vector3.right); }
        if (Input.GetKeyDown(KeyCode.DownArrow)) { Move(Vector3.down); }
        if (Input.GetKeyDown(KeyCode.UpArrow)) { RotatePiece(); }
        if (Input.GetKeyDown(KeyCode.Space)) { FullDrop(); }
    }

    // Moves the piece in the specified direction
    public void Move(Vector3 direction)
    {
        transform.position += direction;

        if (!IsValidPosition())
        {
            transform.position -= direction; // Revert move if invalid
            if (direction == Vector3.down)
            {
                LockPiece(); // Lock piece if it can't move down further
            }
        }
    }

    // Instantly drops the piece to the lowest available position
    private void FullDrop()
    {
        do
        {
            Move(Vector3.down);
        } while (isLocked == false);
    }

    // Rotates the piece by 90 degrees and applies wall kicks if necessary
    private void RotatePiece()
    {
        Vector3 originalPosition = transform.position; // Store original position
        Quaternion originalRotation = transform.rotation; // Store original rotation

        transform.Rotate(0, 0, 90); // Rotate piece

        if (!IsValidPosition())
        {
            if (!TryWallKick(originalPosition, originalRotation))
            {
                transform.position = originalPosition; // Revert position if rotation fails
                transform.rotation = originalRotation; // Revert rotation if invalid
                Debug.Log("Rotation invalid, reverting rotation/position");
            }
            else
            {
                Debug.Log("Rotation/position adjusted with wall kick");
            }
        }
    }

    // Checks if the piece is in a valid position on the grid
    private bool IsValidPosition()
    {
        foreach (Transform block in transform)
        {
            Vector2Int position = Vector2Int.RoundToInt(block.position);

            if (grid.IsCellOccupied(position))
            {
                return false; // Return false if the position is occupied or out of bounds
            }
        }
        return true; // Position is valid
    }

    // Handles automatic downward movement of the piece
    private void HandleAutomaticDrop()
    {
        dropTimer -= Time.deltaTime;

        if (dropTimer <= 0)
        {
            Move(Vector3.down);
            dropTimer = dropInterval; // Reset timer
        }
    }

    // Locks the piece in place, adds it to the grid, and spawns a new piece
    private void LockPiece()
    {
        isLocked = true;
        foreach (Transform block in transform)
        {
            Vector2Int position = Vector2Int.RoundToInt(block.position);
            grid.AddBlockToGrid(block, position); // Add block to grid
        }

        grid.ClearFullLines(); // Check for and clear full lines

        if (FindObjectOfType<TetrisSpawner>())
        {
            FindObjectOfType<TetrisSpawner>().SpawnPiece(); // Spawn a new piece
        }

        Destroy(this); // Remove this script to finalize the piece
    }

    // Attempts to adjust the piece's position using wall kicks
    private bool TryWallKick(Vector3 originalPosition, Quaternion originalRotation)
    {
        //define the wall kicks (srs guidelines
        Vector2Int[] wallKickOffsets = new Vector2Int[]
        {
            new Vector2Int(1,0), //move Right by 1
            new Vector2Int(-1,0), //move Left
            new Vector2Int(1, -1), //move Down
            new Vector2Int(1, -1), //move Diagonally right-down
            new Vector2Int(-1,-1), //move Diagonally Left-down

            new Vector2Int(2, 0), //move Right by 2
            new Vector2Int(-2, 0), //move Left
            new Vector2Int(0, -2), //move Down
            new Vector2Int(2, -1), //move Diagonally right-down
            new Vector2Int(-2, -1), //move Diagonally Left-down
            new Vector2Int(2, -2), //move Diagonally right-down
            new Vector2Int(-2, -2), //move Diagonally Left-down

            new Vector2Int(3, 0), //move Right by 3
            new Vector2Int(-3, 0), //move Left
            new Vector2Int(0, -3), //move Down
            new Vector2Int(3, -1), //move Diagonally right-down
            new Vector2Int(-3, -1), //move Diagonally Left-down
            new Vector2Int(3, -2), //move Diagonally right-down
            new Vector2Int(-3, -2), //move Diagonally Left-down
            new Vector2Int(3, -3), //move Diagonally right-down
            new Vector2Int(-3, -3), //move Diagonally Left-down
        };

        foreach (Vector2Int offset in wallKickOffsets)
        {
            transform.position += (Vector3Int)offset; // Apply offset

            if (IsValidPosition())
            {
                return true; // Return true if a valid position is found
            }

            transform.position -= (Vector3Int)offset; // Revert position if invalid
        }
        return false; // Return false if no valid position is found
    }
}
