using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Utility.ClassManager;

public class Floor : Building {
    //private readonly string name;
    //private readonly Texture2D texture;
    private Vector3Int position;
    private readonly FloorType type;

    /// <summary>
    /// Create a floor tile at the given position with the given type.
    /// </summary>
    /// <param name="position">The position to place the floor at</param>
    /// <param name="type">The type of floor</param>
    // public Floor(Vector3Int position, FloorType type) : base(new Vector3Int[]{position}, new Vector3Int[]{position}, GetFloorTilemap()) {
    //     this.position = position;
    //     this.type = type;
    //     Init();
    // }

    // public Floor(FloorType type) : base(null,null,null){
    //     this.type = type;
    //     Init();
    // }

    protected override void Init() {
        name = this.GetType().Name;
        texture = Resources.Load("Floors") as Texture2D;
    }

    public Tile GetFloorConfig(FloorFlag[] flags, FloorType type) {
        int x = (int)type;
        int y = 0;
        if (flags.Length != 0) foreach (FloorFlag flag in flags) y += (int)flag;
        return GetFloorTile(x, y);
    }

    private Tile GetFloorTile(int xOffset, int yOffset) {
        Sprite sprite = Sprite.Create(texture, new Rect(16 * xOffset, 16 * yOffset, 16, 16), new Vector2(0.5f, 0.5f), 16);
        Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
        tile.sprite = sprite;
        return tile;
    }
    public Vector3Int GetPosition() { return position; }

    public FloorType GetFloorType() { return type; }


}
