using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour
{
    public TetraminoDatas[] tetramino;

    public Piece startingPiece;
    public Piece prefabPiece;
    public Piece activePiece;


    public Tilemap tilemap;

    public Vector2Int boardSize;

    public Vector2Int startPosition;

    public TetrisManager tetrisManager;

    public float dropInterval = 0.7f;

    float dropTime = 0.0f;

    public bool boardInitialized = false;

    public int bagCounter = 0;



    //Maps tilemap position to a Piece GameObject
    Dictionary<Vector3Int, Piece> pieces = new Dictionary<Vector3Int, Piece>();
    private void Start()
    {
        InitializePresetBoard();
        SpawnPiece();
    }


    private void Update()
    {
        if (tetrisManager.gameOver) return;

        dropTime += Time.deltaTime;

        if (dropTime >= dropInterval)
        {
            dropTime = 0.0f;

            Clear(activePiece);
            bool moveResult = activePiece.Move(Vector2Int.down);
            Set(activePiece);

            //if the move fails that means the piece is blocked
            //that ends the placement
            if (!moveResult)
            {
                activePiece.freeze = true;

                CheckBoard();
                SpawnPiece();
            }
        }
    }


    int left
    {
        get { return -boardSize.x / 2; }
    }

    int right
    {
        get { return boardSize.x / 2; }
    }

    int top
    {
        get { return (boardSize.y / 2) + 2; }
    }

    int bottom
    {
        get { return -boardSize.y / 2; }
    }


    public void SpawnPiece()
    {
        activePiece = Instantiate(prefabPiece);

        Tetramino t = Tetramino.O;
        
        //Increments each piece spawn until all the pieces to solve the preset board are given out
        if (bagCounter == 1)
        {
            t = (Tetramino.J);
        }
        else if (bagCounter == 2)
        {
            t = (Tetramino.E);
        }
        else if (bagCounter == 3)
        {
            t = (Tetramino.I);
        }
        else if (bagCounter == 4)
        {
            t = (Tetramino.E);
        }
        else if (bagCounter == 5)
        {
            t = (Tetramino.T);
        }
        else if (bagCounter == 6)
        {
            t = (Tetramino.T);
        }
        else if (bagCounter == 7)
        {
            t = (Tetramino.I);
        }
        else if (bagCounter == 8)
        {
            t = (Tetramino.E);
        }
        else if (bagCounter == 9)
        {
            t = (Tetramino.E);
        }
        else
        {
            t = (Tetramino)UnityEngine.Random.Range(0, tetramino.Length);
        }

        activePiece.Initialize(this, t);

        CheckEndGame();

        Set(activePiece);

        bagCounter++; //Increment bagCounter after placing piece to place next piece
    }

    void SetTile(Vector3Int cellPosition, Piece piece)
    {
        if (piece == null)
        {
            tilemap.SetTile(cellPosition, null);

            pieces.Remove(cellPosition);
        }
        else
        {
            tilemap.SetTile(cellPosition, piece.data.tile);

            //the piece GameObject
            pieces[cellPosition] = piece;

        }
    }

    void PlacePiece(int x, int y)
    
    {
        Vector3Int cell = new Vector3Int(y, x, 0); //define cell location
        tilemap.SetTile(cell, startingPiece.data.tile); //set cell in tilemap
        pieces[cell] = startingPiece; //set cell in pieces

        startingPiece.IncrementCellCount(); //set cell in cell count
    }

    void CheckEndGame()
    {
        if (!IsPositionValid(activePiece, activePiece.position))
        {
            tetrisManager.SetGameOver(true);
        }
    }

    public void UpdateGameOver()
    {
        // TM.gameOver being false means we reset or started a new game
        if (!tetrisManager.gameOver)
        {
            if (boardInitialized) ResetBoard();
        }

    }

    void InitializePresetBoard()
    {
        startingPiece = Instantiate(prefabPiece); //Instaintiating the starting piece that is filled with preset board cells
        startingPiece.freeze = true; //Freezing the preset board (starting piece)
        startingPiece.cells = new Vector2Int[0]; //Piece needs cells array

        //Setting each line cell by cell of the preset board
        //Line 1 from bottom
        PlacePiece(-10, 4);
        PlacePiece(-10, 3);
        PlacePiece(-10, 1);
        PlacePiece(-10, 0);
        PlacePiece(-10, -1);
        PlacePiece(-10, -2);
        PlacePiece(-10, -3);
        PlacePiece(-10, -4);
        PlacePiece(-10, -5);
        //Line 2 from bottom
        PlacePiece(-9, 4);
        PlacePiece(-9, 3);
        PlacePiece(-9, 1);
        PlacePiece(-9, 0);
        PlacePiece(-9, -1);
        PlacePiece(-9, -2);
        PlacePiece(-9, -3);
        PlacePiece(-9, -4);
        PlacePiece(-9, -5);
        // Line 3 from bottom
        PlacePiece(-8, 4);
        PlacePiece(-8, 0);
        PlacePiece(-8, -1);
        PlacePiece(-8, -2);
        PlacePiece(-8, -3);
        PlacePiece(-8, -4);
        PlacePiece(-8, -5);
        // Line 4 from bottom
        PlacePiece(-7, -1);
        PlacePiece(-7, -2);
        PlacePiece(-7, -3);
        PlacePiece(-7, -4);
        PlacePiece(-7, -5);
        // Line 5 from bottom
        PlacePiece(-6, -1);
        PlacePiece(-6, -2);
        PlacePiece(-6, -3);
        PlacePiece(-6, -4);
        PlacePiece(-6, -5);
        // Line 6 from bottom
        PlacePiece(-5, -1);
        PlacePiece(-5, -2);
        PlacePiece(-5, -3);
        PlacePiece(-5, -4);
        PlacePiece(-5, -5);
        // Line 7 from bottom
        PlacePiece(-4, 4);
        PlacePiece(-4, 3);
        PlacePiece(-4, 2);
        PlacePiece(-4, 1);
        PlacePiece(-4, 0);
        PlacePiece(-4, -2);
        PlacePiece(-4, -3);
        PlacePiece(-4, -4);
        PlacePiece(-4, -5);
        // Line 8 from bottom
        PlacePiece(-3, 4);
        PlacePiece(-3, 3);
        PlacePiece(-3, 2);
        PlacePiece(-3, 1);
        PlacePiece(-3, 0);
        PlacePiece(-3, -2);
        PlacePiece(-3, -3);
        PlacePiece(-3, -4);
        PlacePiece(-3, -5);
        // Line 9 from bottom
        PlacePiece(-2, 4);
        PlacePiece(-2, 3);
        PlacePiece(-2, 2);
        PlacePiece(-2, 1);
        PlacePiece(-2, 0);
        PlacePiece(-2, -2);
        PlacePiece(-2, -3);
        PlacePiece(-2, -4);
        PlacePiece(-2, -5);
        // Line 10 from bottom
        PlacePiece(-1, 4);
        PlacePiece(-1, 3);
        PlacePiece(-1, 2);
        PlacePiece(-1, 1);
        PlacePiece(-1, 0);
        PlacePiece(-1, -2);
        PlacePiece(-1, -3);
        PlacePiece(-1, -4);
        PlacePiece(-1, -5);
        // Line 11 from bottom
        PlacePiece(0, 4);
        PlacePiece(0, 3);
        PlacePiece(0, 2);
        PlacePiece(0, 1);
        PlacePiece(0, 0);
        PlacePiece(0, -2);
        PlacePiece(0, -4);
        // Line 12 from bottom
        PlacePiece(1, 4);
        PlacePiece(1, 3);
        PlacePiece(1, 2);
        PlacePiece(1, 1);
        PlacePiece(1, 0);
        // Line 13 from bottom
        PlacePiece(2, 4);
        PlacePiece(2, 3);
        PlacePiece(2, 2);
        PlacePiece(2, 0);
        PlacePiece(2, -1);
        PlacePiece(2, -2);
        PlacePiece(2, -3);
        PlacePiece(2, -4);
        PlacePiece(2, -5);
        // Line 14 from bottom
        PlacePiece(3, 4);
        PlacePiece(3, 3);
        PlacePiece(3, 2);
        PlacePiece(3, -2);
        PlacePiece(3, -3);
        PlacePiece(3, -4);
        PlacePiece(3, -5);
        


        boardInitialized = true;
    }

    void ResetBoard()
    {
        Piece[] foundPieces = FindObjectsByType<Piece>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (Piece piece in foundPieces) Destroy(piece.gameObject);

        activePiece = null;

        tilemap.ClearAllTiles();

        //If you have the pieces disctionary
        pieces.Clear();

        bagCounter = 1;

        InitializePresetBoard();

        SpawnPiece();
    }

    //Set will colour in the tiles for a piece
    public void Set(Piece piece)
    {
        for(int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            SetTile(cellPosition, piece);
        }
    }

    public void Clear(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + piece.position);
            SetTile(cellPosition, null);
        }
    }

    public bool IsPositionValid(Piece piece, Vector2Int position)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int cellPosition = (Vector3Int)(piece.cells[i] + position);

            //bounds checks
            if (cellPosition.x < left || cellPosition.x >= right || cellPosition.y < bottom || cellPosition.y >= top) return false;

            //check if this position is occupied in the tile map
            if (tilemap.HasTile(cellPosition)) return false;
        }
        return true;
    }

    bool IsLineFull(int y)
    {
        for (int x = left; x < right; x++)
        {
            Vector3Int cellPosition = new Vector3Int(x, y);
            if (!tilemap.HasTile(cellPosition)) return false;
        }

        return true;
    }

    void DestroyLine(int y)
    {
        //Debug.Log($"Destroy Line {y}");

        for (int x = left; x < right; x++)
        {
            //Find the cell we're trying to destroy
            Vector3Int cellPosition = new Vector3Int(x, y);

            //Check if there's a piece in that cell
            if (pieces.TryGetValue(cellPosition, out Piece piece))
            {
                piece.ReduceActiveCount(); //remove from piece cell count
                pieces.Remove(cellPosition); //remove from pieces
                tilemap.SetTile(cellPosition, null); //remove from tilemap
            }
        }
    }

    void ShiftRowsDown(int clearedRow)
    {
        for (int y = clearedRow + 1; y < top; y++)
        {
            for (int x = left; x < right; x++)
            {
                Vector3Int cellPosition = new Vector3Int(x, y);


                if (pieces.ContainsKey(cellPosition))
                {
                    Piece currentPiece = pieces[cellPosition];

                    //Clear the tile
                    SetTile(cellPosition, null);

                    //Move the tile down
                    cellPosition.y -= 1;
                    SetTile(cellPosition, currentPiece);
                }
            }
        }
    }

    public void CheckBoard()
    {
        List<int> destroyedLines = new List<int>();

        //scan from bottom to top

        for (int y = bottom; y < top; y++)
        {
            if (IsLineFull(y))
            {
                DestroyLine(y);
                destroyedLines.Add(y);
            }
        }


        int rowsShiftedDown = 0;
        foreach (int y in destroyedLines)
        {
            ShiftRowsDown(y - rowsShiftedDown);

            //After each loop we've shifted rows down 1 more
            rowsShiftedDown++;
        }

        int score = tetrisManager.CalculateScore(destroyedLines.Count);
        tetrisManager.ChangeScore(score);
    }
}