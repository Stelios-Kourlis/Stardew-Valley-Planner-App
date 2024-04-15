using System;
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
            Tile[] tiles = new Tile[building.Height * building.Width];
            int index = 0;
            Texture2D texture = building.sprite.texture;
            if (reverseLayerOrder) {
                for (int yPivot = building.Height - 1; yPivot >= 0; yPivot--) {
                    for (int xPivot = 0; xPivot < building.Width; xPivot++) {
                        Sprite sprite = Sprite.Create(texture, new Rect(xPivot * 16, yPivot * 16, 16, 16), new Vector2(0.5f, 0.5f), 16);
                        Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
                        tile.sprite = sprite;
                        tiles[index++] = tile;
                    }
                }
            } else {
                for (int yPivot = 0; yPivot < building.Height % 16; yPivot++) {
                    for (int xPivot = 0; xPivot < building.Width % 16; xPivot++) {
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

        public static Tile[] SplitSprite(Sprite sprite){
            // int height = sprite.texture.height;
            // int width = sprite.texture.width;
            List<Tile> tiles = new();
            Rect rect = sprite.textureRect;
            for (int y = (int)rect.y; y < rect.y + rect.height; y += 16) {
                for (int x = (int)rect.x; x < rect.x + rect.width; x += 16) {
                    Sprite newSprite = Sprite.Create(sprite.texture, new Rect(x, y, 16, 16), new Vector2(0.5f, 0.5f), 16);
                    Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
                    tile.sprite = newSprite;
                    tiles.Add(tile);
                }
            }
            return tiles.ToArray();
        }
        public static Tile LoadTile(string path) {
            Texture2D tileTexture = Resources.Load(path) as Texture2D;
            return SplitSprite(tileTexture)[0];
        }

        public static Tile[] LoadTiles(string path, BoxCollider reverseLayerOrder) {
            Texture2D tileTexture = Resources.Load(path) as Texture2D;
            return SplitSprite(tileTexture, reverseLayerOrder);
        }

        ///<summary>Get a part of a 16x16 sprite</summary>
        public static Texture2D GetPartOfSprite(Vector2Int topLeftCorner, int height, Texture2D texture) {
            int width = texture.width / 16;
            // Color[] pixels = texture.GetPixels(topLeftCorner.x * 16, topLeftCorner.y * 16, width * 16, height * 16);
            // Texture2D croppedTexture = new(width * 16, height * 16);
            // croppedTexture.SetPixels(pixels);
            // croppedTexture.Apply();
            Sprite sprite = Sprite.Create(texture, new Rect(topLeftCorner.x * 16, topLeftCorner.y * 16, width * 16, height * 16), new Vector2(0.5f, 0.5f), 16);
            Texture2D croppedTexture = new((int)sprite.rect.width, (int)sprite.rect.height);
            Color[] pixels = sprite.texture.GetPixels((int)sprite.textureRect.x, 
                                                (int)sprite.textureRect.y, 
                                                (int)sprite.textureRect.width, 
                                                (int)sprite.textureRect.height);
            croppedTexture.SetPixels(pixels);
            croppedTexture.Apply();
            return croppedTexture;  
        }
    }

}
