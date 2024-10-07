using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Utility {

    public enum Tiles {
        Red,
        Green
    }
    public static class SpriteManager {
        public static Tile[] SplitSprite(Sprite sprite) {
            List<Tile> tiles = new();
            Rect rect = sprite.textureRect;
            for (int y = (int)rect.y; y < rect.y + rect.height; y += 16) {
                for (int x = (int)rect.x; x < rect.x + rect.width; x += 16) {
                    Sprite newSprite = Sprite.Create(sprite.texture, new Rect(x, y, 16, 16), new Vector2(0.5f, 0.5f), 16);
                    Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
                    tile.sprite = newSprite;
                    tiles.Add(tile);
                    // Resources.UnloadAsset(newSprite);
                }
            }
            Resources.UnloadUnusedAssets();
            return tiles.ToArray();
        }

        public static Tile LoadTile(Tiles tilesType) {
            string path = $"{tilesType}TileSprite";
            Sprite tileTexture = Resources.Load<Sprite>(path);
            Debug.Assert(tileTexture != null, "Tile texture is null");
            Tile tile = SplitSprite(tileTexture)[0];
            // Resources.UnloadAsset(tileTexture);
            return tile;
        }

        public static Texture2D SpriteToTexture2D(Sprite sprite) {
            Texture2D texture = new((int)sprite.rect.width, (int)sprite.rect.height);
            texture.SetPixels(sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height));
            texture.Apply();
            return texture;
        }
    }

}
