﻿using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New Season", menuName = "World/Season")]
public class Season : ScriptableObject
{
    [Header("General Settings")]
    public string seasonName;
    public ObjectGroup[] objectGroups;
    
    [Header("Ground Tiles")]
    public Tile groundCenterTile;
    public Tile groundRightTile;
    public Tile groundLeftTile;
    public Tile groundTopTile;
    public Tile groundTopRightTile;
    public Tile groundTopLeftTile;
    public Tile groundBottomTile;
    public Tile groundBottomRightTile;
    public Tile groundBottomLeftTile;
}