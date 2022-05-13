using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// A <see cref="Season"/> is a <see cref="ScriptableObject"/> that represents a world generation biome.
///
/// As the name might suggest, there are four seasons in the game being generated in the exact following order:
/// Spring => Summer => Autumn => Winter
///
/// This <see cref="ScriptableObject"/> contains ground <see cref="Tile"/>s and <see cref="ObjectGroup"/>s bound
/// to generate only in this <see cref="Season"/>.
/// </summary>
[CreateAssetMenu(fileName = "New Season", menuName = "Game/Season")]
public class Season : ScriptableObject
{
    [Header("General Settings")]
    [Tooltip("The pretty-formatted name of this season")]
    public string seasonName;
    [Tooltip("The list of ObjectGroups that are only applicable to this Season")]
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
    public Tile groundTopAloneTile;
    public Tile groundBottomAloneTile;
    public Tile groundRightAloneTile;
    public Tile groundLeftAloneTile;
}