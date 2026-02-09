using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetramino { I, O, T, J, L, S, Z, E}

[Serializable]
public struct TetraminoDatas
{
    public Tetramino tetramino;
    public Vector2Int[] cells;
    public Tile tile;
}


