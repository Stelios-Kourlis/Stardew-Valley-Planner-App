using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.SpriteManager;

/// <summary>
/// Methods to handle buildings with an effect that has a range ex. Scarecrows, JunimoHuts etc...
/// </summary>
public class RangeEffectBuilding {
    private Building Building { get; set; }
    private Vector3Int[] RangeArea { get; set; }
    private GameObject rangeEffectGameObject;
    private Tile greenTile;

    public RangeEffectBuilding(Building building) {
        Building = building;
        rangeEffectGameObject = new GameObject("RangeEffect");
        rangeEffectGameObject.transform.SetParent(Building.transform);
        rangeEffectGameObject.AddComponent<TilemapRenderer>().sortingOrder = building.TilemapRenderer.sortingOrder - 1;
        if (rangeEffectGameObject.GetComponent<Tilemap>() == null) rangeEffectGameObject.AddComponent<Tilemap>();
        greenTile = LoadTile("GreenTile");
    }

    public void ShowEffectRange(Vector3Int[] rangeArea) {
        if (RangeArea != null) HideEffectRange();
        RangeArea = rangeArea;
        foreach (Vector3Int position in RangeArea) {
            rangeEffectGameObject.GetComponent<Tilemap>().SetTile(position, greenTile);
        }
    }

    public void HideEffectRange() {
        if (RangeArea == null) return;
        foreach (Vector3Int position in RangeArea) {
            rangeEffectGameObject.GetComponent<Tilemap>().SetTile(position, null);
        }
        RangeArea = null;
    }
}
