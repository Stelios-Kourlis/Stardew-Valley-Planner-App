using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
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
                    if (newSprite.texture.isReadable) {
                        if (TileIsEmpty(newSprite)) {
                            tiles.Add(null);
                            continue;
                        }
                    }
                    tiles.Add(SpriteToTile(newSprite));
                }
            }
            Resources.UnloadUnusedAssets();
            return tiles.ToArray();
        }

        public static Tile SpriteToTile(Sprite sprite) {
            Tile tile = ScriptableObject.CreateInstance<Tile>();
            tile.sprite = sprite;
            return tile;
        }

        private static bool TileIsEmpty(Sprite sprite) {
            Texture2D texture = sprite.texture;
            Rect spriteRect = sprite.textureRect;

            // Get pixels in the sprite
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
            Tile tile = SpriteToTile(tileTexture);
            Resources.UnloadAsset(tileTexture);
            return tile;
        }

        public static Texture2D SpriteToTexture2D(Sprite sprite) {
            Texture2D texture = new((int)sprite.rect.width, (int)sprite.rect.height);
            texture.SetPixels(sprite.texture.GetPixels((int)sprite.textureRect.x, (int)sprite.textureRect.y, (int)sprite.textureRect.width, (int)sprite.textureRect.height));
            texture.Apply();
            return texture;
        }

#if UNITY_EDITOR

        static bool SpritesAreTheSame(Sprite sprite1, Sprite sprite2) {
            bool widthsAreTheSame = sprite1.textureRect.width == sprite2.textureRect.width;
            bool heightsAreTheSame = sprite1.textureRect.height == sprite2.textureRect.height;
            if (!widthsAreTheSame || !heightsAreTheSame) {
                Debug.Log($"Tiles have diffrent dimensions");
                return false;
            }

            Rect spriteRect1 = sprite1.textureRect;
            Color[] pixels1 = sprite1.texture.GetPixels(
                (int)spriteRect1.x,
                (int)spriteRect1.y,
                (int)spriteRect1.width,
                (int)spriteRect1.height
            );
            Rect spriteRect2 = sprite2.textureRect;
            Color[] pixels2 = sprite2.texture.GetPixels(
                (int)spriteRect2.x,
                (int)spriteRect2.y,
                (int)spriteRect2.width,
                (int)spriteRect2.height
            );

            for (int i = 0; i < pixels1.Length; i++) {
                if (pixels1[i] != pixels2[i]) {
                    return false;
                }
            }

            return true;

        }

        [MenuItem("Assets/Create/Split Map To Unique Tiles")]
        public static void SplitMapToTiles() {
            Sprite sprite = Selection.activeObject as Sprite;

            if (sprite == null) {
                Debug.LogError("Please select a sprite to create a tile.");
                return;
            }

            if (!sprite.texture.isReadable) {
                Debug.LogError("Please make the sprite texture readable.");
                return;
            }

            string pathTiles = EditorUtility.SaveFilePanelInProject("Tile Save Location", "Tile", "asset", "Save tile as asset");
            string pathSprites = EditorUtility.SaveFilePanelInProject("Sprite Save Location", "Sprite", "asset", "Save sprite as asset");

            Debug.Log(pathTiles);
            Debug.Log(pathSprites);
            // Debug.Log(pathTiles.Replace($"/{sprite.name}Tile.asset", ""));

            if (pathTiles == "") {
                return; // User canceled the save
            }

            if (pathSprites == "") {
                return; // User canceled the save
            }

            Tile[] existingTiles = Resources.LoadAll<Tile>(pathTiles.Replace($"/Tile.asset", "").Replace("Assets/Resources/", ""));
            Sprite[] existingSprites = Resources.LoadAll<Sprite>(pathSprites.Replace($"/Sprite.asset", "").Replace("Assets/Resources/", ""));
            Debug.Log($"Existing Tiles Length: {existingTiles.Length}");
            Debug.Log($"Existing Sprites: {existingSprites.Length}");
            List<Tile> tiles = new();
            List<Sprite> sprites = new();
            Rect rect = sprite.textureRect;
            int spriteCount = existingSprites.Length;

            for (int y = (int)rect.y; y < rect.y + rect.height; y += 16) {
                for (int x = (int)rect.x; x < rect.x + rect.width; x += 16) {
                    Sprite newSprite = Sprite.Create(sprite.texture, new Rect(x, y, 16, 16), new Vector2(0.5f, 0.5f), 16);
                    newSprite.name = $"Sprite{x / 16}_{y / 16}";
                    foreach (Sprite existingSprite in existingSprites) if (SpritesAreTheSame(newSprite, existingSprite)) goto endOfLoop; //not the best but I need to continue the outer loop
                    foreach (Sprite existingSprite in sprites) if (SpritesAreTheSame(newSprite, existingSprite)) goto endOfLoop;
                    Debug.Log($"Adding new sprite Sprite{spriteCount}");
                    AssetDatabase.CreateAsset(newSprite, pathSprites.Replace("Sprite.asset", $"Sprite{spriteCount++}.asset"));
                    sprites.Add(newSprite);
                    tiles.Add(SpriteToTile(newSprite));
                endOfLoop: { }
                }
            }


            int index = existingTiles.Length;
            // Save the tile as an asset
            foreach (Tile tile in tiles) AssetDatabase.CreateAsset(tile, pathTiles.Replace("Tile.asset", $"Tile{index++}.asset"));
            AssetDatabase.SaveAssets();
        }

        [MenuItem("Assets/Create/Save Map as Tile Data")]
        public static void ConvertMapToTileList() {
            Sprite sprite = Selection.activeObject as Sprite;

            if (sprite == null) {
                Debug.LogError("Please select a sprite to create a tile.");
                return;
            }

            string path = EditorUtility.SaveFilePanelInProject("Map Placement Tiles", $"{sprite.name}TileData", "txt", "Save map as Tile Data asset");

            if (path == "") {
                return; // User canceled the save
            }

            List<Tile> tiles = Resources.LoadAll<Tile>($"Tiles").ToList();
            Rect rect = sprite.textureRect;
            string data = "";

            for (int y = (int)rect.y; y < rect.y + rect.height; y += 16) {
                for (int x = (int)rect.x; x < rect.x + rect.width; x += 16) {
                    Sprite newSprite = Sprite.Create(sprite.texture, new Rect(x, y, 16, 16), new Vector2(0.5f, 0.5f), 16);
                    foreach (Tile tile in tiles) {
                        if (SpritesAreTheSame(newSprite, tile.sprite)) {
                            data += $"{x / 16},{y / 16} : {tile.name.Replace("Tile", "")},\n";
                            break;
                        }
                    }

                }
            }

            File.WriteAllText(path, data);
            AssetDatabase.Refresh();
        }

#endif
    }
}
