using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public TetraminoDatas data;
    public Board board;
    public Vector2Int[] cells;

    public Vector2Int position;

    public bool freeze = false;


    int activeCellCount = -1;
    public void Initialize(Board board, Tetramino tetramino)
    {
        this.board = board;

        for (int i = 0; i < board.tetramino.Length; i++)
        {
            if (board.tetramino[i].tetramino == tetramino)
            {
                this.data = board.tetramino[i];
                break;
            }
        }

        cells = new Vector2Int[data.cells.Length];
        for (int i = 0; i < data.cells.Length; i++) cells[i] = data.cells[i];

        position = board.startPosition;

        activeCellCount = cells.Length;
    }

    private void Update()
    {
        if (board.tetrisManager.gameOver) return;

        if (freeze)
        {
            return;
        }

        board.Clear(this);


        if (Input.GetKeyDown(KeyCode.Space))
        {
            HardDrop();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                Move(Vector2Int.left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Move(Vector2Int.right);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                Move(Vector2Int.down);
            }

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.X)) Rotate(1);
            if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.Z)) Rotate(-1);
        }

        board.Set(this);

        if (Input.GetKeyDown(KeyCode.P))
        {
            board.CheckBoard();
        }

    }

    public void IncrementCellCount()
    {
        activeCellCount++;
    }

    void Rotate(int direction)
    {
        //Store cell locations so we can revert it 
        Vector2Int[] temporaryCells = new Vector2Int[cells.Length];

        for (int i = 0; i < cells.Length; i++) temporaryCells[i] = cells[i];

        ApplyRotation(direction);

        if (!board.IsPositionValid(this, position))
        {
            if (!TryWallKicks())
            {
                RevertRotation(temporaryCells);
            }
            else
            {
                Debug.Log("Wall kick succeeded");
            }
        }
    }

    bool TryWallKicks()
    {
        List<Vector2Int> wallKickOffsets = new List<Vector2Int>
            {
                Vector2Int.left,
                Vector2Int.down,
                Vector2Int.right,
                new Vector2Int(-1, -1), //Diagonal down-left
                new Vector2Int(1, -1) //Diagonal down-right
            };

        if (data.tetramino == Tetramino.I)
        {
            wallKickOffsets.Add(2 * Vector2Int.left);
            wallKickOffsets.Add(2 * Vector2Int.right);
        }

        if (data.tetramino == Tetramino.E)
        {
            wallKickOffsets.Add(3 * Vector2Int.left);
            wallKickOffsets.Add(3 * Vector2Int.right); //Kick further because it is bigger and needs 3 to get off the wall
        }

        foreach (Vector2Int offset in wallKickOffsets)
        {
            if (Move(offset)) return true;
        }

        return false;
    }

    void RevertRotation(Vector2Int[] temporaryCells)
    {
        for (int i = 0; i < cells.Length; i++) cells[i] = temporaryCells[i];
    }


    void ApplyRotation(int direction)
    {
        Quaternion rotation = Quaternion.Euler(0, 0, 90 * direction);

        bool isSpecial = data.tetramino == Tetramino.I || data.tetramino == Tetramino.O;
        bool isVerySpecial = data.tetramino == Tetramino.E;

        for (int i = 0; i < cells.Length; i++)
        {
            // convert cell location to a vector3 to work with quaternions
            Vector3 cellPosition = new Vector3(cells[i].x, cells[i].y, 0);

            if (isSpecial)
            {
                cellPosition.x -= 0.5f;
                cellPosition.y -= 0.5f;
            }
            else if (isVerySpecial)
            {
                cellPosition.y -= 1f; //Compensate for E's tallness
            }

            // get the reslt
            Vector3 result = rotation * cellPosition;

            // put it back in the cells data
            //WHAT ABOUT FOR THIS SECTION?
            if (isSpecial)
            {
                cells[i].x = Mathf.CeilToInt(result.x);
                cells[i].y = Mathf.CeilToInt(result.y);
            }
            else
            {
                cells[i].x = Mathf.RoundToInt(result.x);
                cells[i].y = Mathf.RoundToInt(result.y);
            }
        }

    }

    public void HardDrop()
    {
        while (Move(Vector2Int.down))
        {
            //do nothing
        }

        freeze = true;

    }

    public bool Move(Vector2Int translation)
    {

        Vector2Int newPosition = position;
        newPosition += translation;

        bool positionValid = board.IsPositionValid(this, newPosition);
        if (positionValid) position = newPosition;

        return positionValid;
    }

    public void ReduceActiveCount()
    {
        activeCellCount -= 1;
        if (activeCellCount <= 0)
        {
            Destroy(gameObject);
        }
    }
}

