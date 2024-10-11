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
                    if (TileIsEmpty(newSprite)) {
                        tiles.Add(null);
                        continue;
                    }
                    Tile tile = ScriptableObject.CreateInstance(typeof(Tile)) as Tile;
                    tile.sprite = newSprite;
                    tiles.Add(tile);
                    // Resources.UnloadAsset(newSprite);
                }
            }
            Resources.UnloadUnusedAssets();
            return tiles.ToArray();
        }

        private static bool TileIsEmpty(Sprite sprite) {
            Texture2D texture = sprite.texture;
            Rect spriteRect = sprite.textureRect;

            // Get pixels in the sprite
            // Debug.Log(sprite.name);
            Color[] pixels = texture.GetPixels((int)spriteRect.x, (int)spriteRect.y, (int)spriteRect.width, (int)spriteRect.height);

            // Check if all pixels are transparent (alpha == 0)
            foreach (Color pixel in pixels) {
                if (pixel.a != 0f) {
                    return false; // Found a non-transparent pixel, so it's not empty
                }
            }

            return true; // All pixels are transparent, the tile is empty
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
