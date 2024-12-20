using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SFB;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Utility.ClassManager;

namespace Utility {
    public static class TilemapManager {
        ///<summary>Get all Vector3Int representing tiles in an area</summary>
        ///<param name="position">the vector containing the bottom left coordinates of the rectangle</param>
        ///<param name="height">the height of the rectangle, must be positive</param>
        ///<param name="width">the width of the rectangle, must be positive</param>
        ///<param name="flipped">whether to flip the positions y-wise: (example with height = 3, width = 3)
        ///<para>flipped = false (default):</para>
        /// <para>[ (0,0) , (0,1) , (0,2) ] = [1,2,3]</para>
        /// <para>[ (1,0) , (1,1) , (1,2) ] = [4,5,6]</para>
        /// <para>[ (2,0) , (2,1) , (2,2) ] = [7,8,9]</para>
        /// flipped = true: 
        /// <para>[ (0,0) , (0,1) , (0,2) ] = [7,8,9]</para>
        /// <para>[ (1,0) , (1,1) , (1,2) ] = [4,5,6]</para>
        /// <para>[ (2,0) , (2,1) , (2,2) ] = [1,2,3]</para>
        /// 
        ///</param>
        ///<returns>An array of Vector3Int that containts every vector in the rectangle</returns>
        public static List<Vector3Int> GetRectAreaFromPoint(Vector3Int lowerLeftCorner, int height, int width, bool flipped = false) {
            if (lowerLeftCorner == null) throw new ArgumentNullException("Position cannot be null");
            if (height < 0 || width < 0) throw new ArgumentException($"Height and width must be positive, got {height} height and {width} width.");
            if (height == 0 || width == 0) return new List<Vector3Int>();
            List<Vector3Int> area = new();
            if (!flipped) {
                for (int heightOffset = 0; heightOffset < height; heightOffset++) {
                    for (int widthOffset = 0; widthOffset < width; widthOffset++) {
                        area.Add(new Vector3Int(lowerLeftCorner.x + widthOffset, lowerLeftCorner.y + heightOffset, lowerLeftCorner.z));
                    }
                }
            }
            else {
                for (int i = 0; i < height; i++) {
                    for (int j = 0; j < width; j++) {
                        area.Add(new Vector3Int(lowerLeftCorner.x + j, lowerLeftCorner.y - i + height - 1, lowerLeftCorner.z));
                    }
                }
            }
            return area;
        }

        ///<summary>Get all Vector3Int representing a cross around the given position, used to get the sprinkler tier 1 area of effect,
        ///List is on order {left, right, down, up}</summary>
        public static List<Vector3Int> GetCrossAroundPosition(Vector3Int position) {
            return new List<Vector3Int>{
                // position,
                new(position.x - 1, position.y, position.z),
                new(position.x + 1, position.y, position.z),
                new(position.x, position.y - 1, position.z),
                new(position.x, position.y + 1, position.z)
            };
        }

        public static List<Vector3Int> GetAreaAroundPosition(Vector3Int middlePosition, int radius) {
            return GetRectAreaFromPoint(new Vector3Int(middlePosition.x - radius, middlePosition.y - radius, middlePosition.z), radius * 2 + 1, radius * 2 + 1);
        }

        public static List<Vector3Int> GetCircleAroundPosition(Vector3Int center, int radius) {
            List<Vector3Int> points = new();
            for (int y = -radius; y <= radius; y++) {
                for (int x = -radius; x <= radius; x++) {
                    if (Math.Abs(x) + Math.Abs(y) <= radius) points.Add(new Vector3Int(center.x + x, center.y + y, center.z));
                }
            }
            return points;
        }

        public static List<Vector3Int> GetRangeOfScarecrow(Vector3Int center) {
            HashSet<Vector3Int> coordinates = new();
            Vector3Int lowerLeft = new(center.x - 4, center.y - 8, center.z);
            coordinates.UnionWith(GetRectAreaFromPoint(lowerLeft, 17, 9, true));
            lowerLeft += new Vector3Int(-1, 1, 0);
            coordinates.UnionWith(GetRectAreaFromPoint(lowerLeft, 15, 11, true));
            lowerLeft += new Vector3Int(-1, 1, 0);
            coordinates.UnionWith(GetRectAreaFromPoint(lowerLeft, 13, 13, true));
            lowerLeft += new Vector3Int(-1, 1, 0);
            coordinates.UnionWith(GetRectAreaFromPoint(lowerLeft, 11, 15, true));
            lowerLeft += new Vector3Int(-1, 1, 0);
            coordinates.UnionWith(GetRectAreaFromPoint(lowerLeft, 9, 17, true));
            coordinates.Remove(center);
            coordinates.Remove(new Vector3Int(center.x, center.y + 1, center.z));
            return coordinates.ToList();
        }

        public static List<Vector3Int> GetRangeOfDeluxeScarecrow(Vector3Int center) {
            HashSet<Vector3Int> coordinates = new();
            Vector3Int lowerLeft = new(center.x - 5, center.y - 16, center.z);
            coordinates.UnionWith(GetRectAreaFromPoint(lowerLeft, 33, 11, true));
            lowerLeft += new Vector3Int(-2, 1, 0);
            coordinates.UnionWith(GetRectAreaFromPoint(lowerLeft, 31, 15, true));
            lowerLeft += new Vector3Int(-2, 1, 0);
            coordinates.UnionWith(GetRectAreaFromPoint(lowerLeft, 29, 19, true));
            lowerLeft += new Vector3Int(-1, 1, 0);
            coordinates.UnionWith(GetRectAreaFromPoint(lowerLeft, 27, 21, true));
            lowerLeft += new Vector3Int(-2, 1, 0);
            coordinates.UnionWith(GetRectAreaFromPoint(lowerLeft, 25, 25, true));
            lowerLeft += new Vector3Int(-1, 2, 0);
            coordinates.UnionWith(GetRectAreaFromPoint(lowerLeft, 21, 27, true));
            lowerLeft += new Vector3Int(-1, 1, 0);
            coordinates.UnionWith(GetRectAreaFromPoint(lowerLeft, 19, 29, true));
            lowerLeft += new Vector3Int(-1, 2, 0);
            coordinates.UnionWith(GetRectAreaFromPoint(lowerLeft, 15, 31, true));
            lowerLeft += new Vector3Int(-1, 2, 0);
            coordinates.UnionWith(GetRectAreaFromPoint(lowerLeft, 11, 33, true));
            coordinates.Remove(center);
            coordinates.Remove(new Vector3Int(center.x, center.y + 1, center.z));
            return coordinates.ToList();
        }

        public static List<Vector3Int> GetRangeOfBeehouse(Vector3Int beehousePosition) {
            List<Vector3Int> coordinates = new();
            for (int x = -5; x <= 5; x++) {
                for (int y = -5; y <= 5; y++) {
                    if (Math.Abs(x) + Math.Abs(y) <= 5) {
                        Vector3Int vec = new(beehousePosition.x + x, beehousePosition.y + y, beehousePosition.z);
                        coordinates.Add(vec);
                    }
                }
            }
            return coordinates;
        }


        /// <summary>
        /// Given the unavailable coordinates of an interiror, add all the ones out of bounds
        /// </summary>
        /// <param name="actualUnavailableCoordinates"></param>
        /// <returns></returns>
        public static List<Vector3Int> GetAllInteriorUnavailableCoordinates(Vector3Int[] actualUnavailableCoordinates) {
            Vector3Int bottomLeftVec = GetMapController().gameObject.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(new Vector3(0, 0, 0)));
            Vector3Int topRightVec = GetMapController().gameObject.GetComponent<Tilemap>().WorldToCell(Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0)));
            Vector3Int[] allCordinatesOnScreen = GetAllCoordinatesInArea(bottomLeftVec, topRightVec).ToArray();
            int minX = actualUnavailableCoordinates.Min(vec => vec.x);
            int minY = actualUnavailableCoordinates.Min(vec => vec.y);
            int maxX = actualUnavailableCoordinates.Max(vec => vec.x);
            int maxY = actualUnavailableCoordinates.Max(vec => vec.y);
            foreach (Vector3Int vec in allCordinatesOnScreen) if (vec.x < minX || vec.x > maxX || vec.y < minY || vec.y > maxY) actualUnavailableCoordinates = actualUnavailableCoordinates.Append(vec).ToArray();
            return actualUnavailableCoordinates.ToList();
        }

        ///<summary>Create a Game Object with a Tilemap and a Tilemap Renderer components attached</summary>
        ///<param name="parent">the transform of the game object you want to be the parent of the created tilemap game object</param>
        ///<param name="name">the name of the created object</param>
        ///<returns>True if there are no other tiles and within the rectangle, otherwise false</returns>
        public static GameObject CreateTilemapObject(Transform parent, int sortingOrder, String name = "Tilemap") {
            GameObject tilemap = new(name);
            tilemap.transform.parent = parent.transform;
            tilemap.AddComponent<Tilemap>();
            tilemap.AddComponent<TilemapRenderer>();
            tilemap.GetComponent<TilemapRenderer>().sortingOrder = sortingOrder;
            return tilemap;
        }

        public static GameObject AddTilemapToObject(GameObject obj) {
            if (obj.GetComponent<Tilemap>() == null) obj.AddComponent<Tilemap>();
            if (obj.GetComponent<TilemapRenderer>() == null) obj.AddComponent<TilemapRenderer>();
            return obj;
        }

        public static HashSet<Vector3Int> GetAllCoordinatesInArea(Vector3Int point1, Vector3Int point2) {
            HashSet<Vector3Int> coordinates = new();

            // Determine the lower left and upper right corners
            Vector3Int lowerLeft = new(Math.Min(point1.x, point2.x), Math.Min(point1.y, point2.y), 0);
            Vector3Int upperRight = new(Math.Max(point1.x, point2.x), Math.Max(point1.y, point2.y), 0);

            for (int x = lowerLeft.x; x <= upperRight.x; x++) {
                for (int y = lowerLeft.y; y <= upperRight.y; y++) {
                    coordinates.Add(new Vector3Int(x, y, 0));
                }
            }
            return coordinates;
        }

        /// <summary>
        /// Return the 4 adjacent tiles of the position given
        /// </summary>
        /// <param name="position">The position you want to get the adjacent tiles of</param>
        /// <returns>An array with 4 Vector3Ints for the 4 neightbours going {left, right, down, up}</returns>
        public static Vector3Int[] GetNeighboursOfPosition(Vector3Int position) {
            return new Vector3Int[4]{
                new(position.x - 1, position.y, position.z),
                new(position.x + 1, position.y, position.z),
                new(position.x, position.y - 1, position.z),
                new(position.x, position.y + 1, position.z),
            };
        }

        /// <summary>
        /// Get the middle coordinate of a building in world coordinates
        /// </summary>
        /// <param name="building">The building</param>
        public static Vector3 GetMiddleOfBuildingWorld(Building building) {
            int width = building.Width;
            int height = building.Height;
            if (building.BaseCoordinates.Count() == 1) return building.Base;
            Vector3 result = new(-1, -1);
            if (building.BaseHeight == 1 && building.Width == 1) return building.Base;
            if (width % 2 != 0) {
                Vector3Int leftMiddle = new(building.Base.x + Mathf.FloorToInt(width / 2.0f), 0, 0);
                Vector3Int rightMiddle = new(building.Base.x + Mathf.CeilToInt(width / 2.0f), 0, 0);
                Vector3 leftMiddleWorld = building.Tilemap.CellToWorld(leftMiddle);
                Vector3 rightMiddleWorld = building.Tilemap.CellToWorld(rightMiddle);
                result.x = (leftMiddleWorld.x + rightMiddleWorld.x) / 2;
            }

            else result.x = building.Tilemap.CellToWorld(new Vector3Int(building.Base.x + Mathf.FloorToInt(width / 2), 0, 0)).x;
            if (height % 2 != 0) {
                Vector3Int downMiddle = new(0, building.Base.y + Mathf.FloorToInt(height / 2.0f), 0);
                Vector3Int upMiddle = new(0, building.Base.y + Mathf.CeilToInt(height / 2.0f), 0);
                Vector3 downMiddleWorld = building.Tilemap.CellToWorld(downMiddle);
                Vector3 upMiddleWorld = building.Tilemap.CellToWorld(upMiddle);
                result.y = (upMiddleWorld.y + downMiddleWorld.y) / 2;
            }
            else result.y = building.Tilemap.CellToWorld(new Vector3Int(0, building.Base.y + height / 2, 0)).y;


            return result;
        }

        public static Vector3 GetMiddleOfCoordinates(Vector3Int[] coordinates) {
            if (coordinates.Length == 0) return Vector3.zero;
            if (coordinates.Length == 1) return coordinates[0];

            float sumX = 0;
            float sumY = 0;

            foreach (var coord in coordinates) {
                sumX += coord.x;
                sumY += coord.y;
            }

            float avgX = sumX / coordinates.Length;
            float avgY = sumY / coordinates.Length;

            return new Vector3(avgX, avgY, 0);
        }

        public static void TakePictureOfMap() {
            // Building.CurrentAction = Actions.EDIT;
            BuildingController.LastBuildingObjectCreated.SetActive(false);
            // Save the original camera settings
            Vector3 originalPosition = Camera.main.transform.position;
            float originalSize = Camera.main.orthographicSize;

            // Get the bounds of the Tilemap
            var mapTilemap = GameObject.FindGameObjectWithTag("CurrentMap").GetComponent<Tilemap>();
            Bounds tilemapBounds = mapTilemap.localBounds;

            // Calculate the new camera position and size
            Vector3 newPosition = tilemapBounds.center;
            float newWidth = tilemapBounds.size.x;
            float newHeight = tilemapBounds.size.y;
            float aspectRatio = Camera.main.aspect;

            // Set the camera to cover the entire Tilemap
            Camera.main.transform.position = new Vector3(newPosition.x, newPosition.y, originalPosition.z);
            Camera.main.orthographicSize = (newWidth / aspectRatio > newHeight) ? newWidth / (2f * aspectRatio) : newHeight / 2f;

            // Create a new render texture
            RenderTexture renderTexture = new(Screen.width, Screen.height, 24);
            Camera.main.targetTexture = renderTexture;

            // Render the camera's view to the target texture
            Camera.main.Render();

            // Create a new texture and read the active RenderTexture into it
            Texture2D texture = new(Screen.width, Screen.height, TextureFormat.RGB24, false);
            RenderTexture.active = renderTexture;
            texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
            texture.Apply();

            // Convert the texture to a byte array
            byte[] bytes = texture.EncodeToPNG();

            // Write the byte array to a file
            string defaultScreenshotPath = PlayerPrefs.GetString("DefaultScreenshotPath", Application.dataPath);
            var savePath = StandaloneFileBrowser.SaveFilePanel("Choose a save location", defaultScreenshotPath, "FarmScreenshot", "png");
            if (savePath != "") {
                string directoryPath = Path.GetDirectoryName(savePath);
                PlayerPrefs.SetString("DefaultScreenshotPath", directoryPath);
                File.WriteAllBytes(savePath, bytes);
            }

            // Reset the camera and render texture
            Camera.main.targetTexture = null;
            RenderTexture.active = null;

            // Restore the original camera settings
            Camera.main.transform.position = originalPosition;
            Camera.main.orthographicSize = originalSize;


        }

        public static Vector3Int GetMousePositionInTilemap() {
            Vector3Int pos = GetGridTilemap().WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            pos.z = 0;
            return pos;
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Tile from Sprite")]
        public static void CreateTileFromSprite() {
            // Get selected sprite
            Sprite selectedSprite = Selection.activeObject as Sprite;

            if (selectedSprite == null) {
                Debug.LogError("Please select a sprite to create a tile.");
                return;
            }

            // Create a new Tile
            Tile tile = ScriptableObject.CreateInstance<Tile>();

            // Set the sprite of the tile
            tile.sprite = selectedSprite;

            // Create a path to save the tile asset
            string path = EditorUtility.SaveFilePanelInProject("Save Tile", selectedSprite.name + "Tile", "asset", "Save tile as asset");

            if (path == "") {
                return; // User canceled the save
            }

            // Save the tile as an asset
            AssetDatabase.CreateAsset(tile, path);
            AssetDatabase.SaveAssets();

            Debug.Log("Tile created successfully: " + path);
        }
#endif

        public static void SetTilesOnlyNonNull(Tile[] tiles, Vector3Int[] positions, Tilemap tilemap) {
            for (int i = 0; i < positions.Length; i++) {
                if (tiles[i] != null) {
                    tilemap.SetTile(positions[i], tiles[i]);
                }
            }
        }

    }
}

