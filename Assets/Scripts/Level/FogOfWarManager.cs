using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Central registry for the current level's FogTile grid. Any "revealer" (AreaRevealer, Lamp, ...)
/// calls RevealCircle to permanently clear fog tiles within a radius. FogTiles (de)register
/// themselves, so this stays correct across scene loads without any manual reset.
/// </summary>
public static class FogOfWarManager
{
    private static readonly List<FogTile> Tiles = new List<FogTile>();

    public static void Register(FogTile tile) => Tiles.Add(tile);

    public static void Unregister(FogTile tile) => Tiles.Remove(tile);

    public static void RevealCircle(Vector2 worldPosition, float radius, float fadeDuration = 0.6f)
    {
        float sqrRadius = radius * radius;
        foreach (FogTile tile in Tiles)
        {
            if (tile == null || tile.IsRevealed) continue;
            if (((Vector2)tile.transform.position - worldPosition).sqrMagnitude <= sqrRadius)
            {
                tile.Reveal(fadeDuration);
            }
        }
    }
}
