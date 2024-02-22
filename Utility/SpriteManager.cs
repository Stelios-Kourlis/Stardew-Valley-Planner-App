using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Utility{
    public static class SpriteManager{
        ///<summary>Split a sprite of a building in 16x16 tiles</summary>
        ///<param name="building">the building whose sprite you want to slice</param>
        ///<param name="reverseLayerOrder">if true the building layers will be flipped y-wise, true by default</param>
        ///<returns>An array of Tiles, each tile is a 16x16 area of the sprite</returns>
        public static Tile[] SplitSprite(Building building, bool reverseLayerOrder = true) {
            Tile[] tiles = new Tile[building.height * building.width];
            int index = 0;
            Texture2D texture = building.texture;
            if (reverseLayerOrder) {
                for (int yPivot = building.height - 1; yPivot >= 0; yPivot--) {
                    for (int xPivot = 0; xPivot < building.width; xPivot++) {
                        Sprite sprite = Sprite.Create(texture, new Rect(xPivot * 16, yPivot * 16, 16, 16), new Vector2(0.5f, 0.5f), 16);
                        Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
                        tile.sprite = sprite;
                        tiles[index++] = tile;
                    }
                }
            } else {
                for (int yPivot = 0; yPivot < building.height % 16; yPivot++) {
                    for (int xPivot = 0; xPivot < building.width % 16; xPivot++) {
                        Sprite sprite = Sprite.Create(texture, new Rect(xPivot * 16, yPivot * 16, 16, 16), new Vector2(0.5f, 0.5f), 16);
                        Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
                        tile.sprite = sprite;
                        tiles[index++] = tile;
                    }
                }
            }

            return tiles;
        }

        public static Tile[] SplitSprite(Texture2D texture, bool reverseLayerOrder = true) {
            int height = texture.height;
            int width = texture.width;
            Tile[] tiles = new Tile[height * width];
            int index = 0;
            if (reverseLayerOrder) {
                for (int yPivot = (height / 16) - 1; yPivot >= 0; yPivot--) {
                    for (int xPivot = 0; xPivot < width / 16; xPivot++) {
                        Sprite sprite = Sprite.Create(texture, new Rect(xPivot * 16, yPivot * 16, 16, 16), new Vector2(0.5f, 0.5f), 16);
                        Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
                        tile.sprite = sprite;
                        tiles[index++] = tile;
                    }
                }
            } else {
                for (int yPivot = 0; yPivot < height % 16; yPivot++) {
                    for (int xPivot = 0; xPivot < width % 16; xPivot++) {
                        Sprite sprite = Sprite.Create(texture, new Rect(xPivot * 16, yPivot * 16, 16, 16), new Vector2(0.5f, 0.5f), 16);
                        Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
                        tile.sprite = sprite;
                        tiles[index++] = tile;
                    }
                }
            }
            return tiles;
        }

        public static Tile LoadTile(string path) {
            Texture2D tileTexture = Resources.Load(path) as Texture2D;
            return SplitSprite(tileTexture)[0];
        }

        public static Tile[] LoadTiles(string path, BoxCollider reverseLayerOrder) {
            Texture2D tileTexture = Resources.Load(path) as Texture2D;
            return SplitSprite(tileTexture, reverseLayerOrder);
        }
}

}
