using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FishDeco {

    public string name;
    private Texture2D texture;
    Vector3Int[] position;
    public Tilemap tilemap;
    static int currentDeco;

    public FishDeco(Vector3Int[] position, Tilemap tilemap) {
        name = GetType().Name;
        this.position = position;
        texture = Resources.Load("Buildings/FishDeco") as Texture2D;
        this.tilemap = tilemap;
    }

    ///<param name="type">The decoration you want 0-3</param>
    public Tile[] GetDeco(int type) {
        
        //Debug.Log("called deco");
        if (type > 3 || type < 0) return null;
        return GetDecoSprite(type * 3);
    }

    public Tile[] GetNextDeco(){
        currentDeco++;
        if (currentDeco > 3) currentDeco = 0;
        return GetDeco(currentDeco);
    }

    private Tile[] GetDecoSprite(int yOffset) {
        // Debug.Log("called sprite deco");
        int height = texture.height / 16;
        int width = texture.width / 16;
        //Debug.Log("Height: " + height + " Width: " + width + " yOffset: " + yOffset);
        Tile[] tiles = new Tile[3 * width];
        int index = 0;
        //int initYPivot = yOffset;
        // Debug.Log(yOffset != yOffset + 3);
        for (int yPivot = yOffset; yPivot != yOffset + 3; yPivot++) {
            //            Debug.Log("yPivot: " + yPivot);
            for (int xPivot = 0; xPivot != width; xPivot++) {
                //                Debug.Log(xPivot + "" + yPivot);
                Sprite sprite = Sprite.Create(texture, new Rect(xPivot * 16, yPivot * 16, 16, 16), new Vector2(0.5f, 0.5f), 16);
                Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
                tile.sprite = sprite;
                tiles[index++] = tile;
            }
        }
        foreach (Tile tile in tiles) if (tile == null) Debug.Log("NULL");
        return tiles;
    }

    public Texture2D GetTexture() { return texture; }

    public Vector3Int[] GetPosition() { return position; }
}

