using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

namespace Utility{
    public static class TilemapManager{
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
        public static List<Vector3Int> GetAreaAroundPosition(Vector3Int lowerLeftCorner, int height, int width, bool flipped = false) {
            if (lowerLeftCorner == null) throw new ArgumentNullException("Position cannot be null");
            if (height < 0 || width < 0) throw new ArgumentException($"Height and width must be positive, got {height} height and {width} width.");
            List<Vector3Int> area = new();
            if (!flipped){
                for (int heightOffset = 0; heightOffset < height; heightOffset++) {
                    for (int widthOffset = 0; widthOffset < width; widthOffset++) {
                        area.Add(new Vector3Int(lowerLeftCorner.x + widthOffset, lowerLeftCorner.y + heightOffset, lowerLeftCorner.z));
                    }
                }
            }else{
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
        public static List<Vector3Int> GetCrossAroundPosition(Vector3Int position){
            return new List<Vector3Int>{
                // position,
                new(position.x - 1, position.y, position.z),
                new(position.x + 1, position.y, position.z),
                new(position.x, position.y - 1, position.z),
                new(position.x, position.y + 1, position.z)
            };
        }

        public static List<Vector3Int> GetAreaAroundPosition(Vector3Int middlePosition, int radius){
            return GetAreaAroundPosition(new Vector3Int(middlePosition.x - radius, middlePosition.y - radius, middlePosition.z), radius * 2 + 1, radius * 2 + 1);
        }

        // public static List<Vector3Int> GetCircleAroundPosition(Vector3Int middlePosition, int radius){
        //     HashSet<Vector3Int> coordinates = new HashSet<Vector3Int>();
        //     Vector3Int lowerLeft = new Vector3Int(middlePosition.x - radius/2, middlePosition.y - radius, middlePosition.z);
        //     coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 17, 9, true));
        //     lowerLeft += new Vector3Int(-1, 1, 0);
        //     coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 15, 11, true));
        //     lowerLeft += new Vector3Int(-1, 1, 0);
        //     coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 13, 13, true));
        //     lowerLeft += new Vector3Int(-1, 1, 0);
        //     coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 11, 15, true));
        //     lowerLeft += new Vector3Int(-1, 1, 0);
        //     coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 9, 17, true));
        //     coordinates.Remove(middlePosition);
        //     coordinates.Remove(new Vector3Int(middlePosition.x, middlePosition.y + 1, middlePosition.z));
        //     return coordinates.ToList();
        // }

        public static List<Vector3Int> GetCircleAroundPosition(Vector3Int center, int radius)        {
            List<Vector3Int> points = new();
            for (int y = -radius; y <= radius; y++){
                for (int x = -radius; x <= radius; x++){
                    if (Math.Abs(x) + Math.Abs(y) <= radius) points.Add(new Vector3Int(center.x + x, center.y + y, center.z));
                }
            }
            return points;
        }

        public static List<Vector3Int> GetRangeOfScarecrow(Vector3Int center){
            HashSet<Vector3Int> coordinates = new();
            Vector3Int lowerLeft = new(center.x - 4, center.y - 8, center.z);
            coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 17, 9, true));
            lowerLeft += new Vector3Int(-1, 1, 0);
            coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 15, 11, true));
            lowerLeft += new Vector3Int(-1, 1, 0);
            coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 13, 13, true));
            lowerLeft += new Vector3Int(-1, 1, 0);
            coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 11, 15, true));
            lowerLeft += new Vector3Int(-1, 1, 0);
            coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 9, 17, true));
            coordinates.Remove(center);
            coordinates.Remove(new Vector3Int(center.x, center.y + 1, center.z));
            return coordinates.ToList();
        }

        public static List<Vector3Int> GetRangeOfDeluxeScarecrow(Vector3Int center){
            HashSet<Vector3Int> coordinates = new();
            Vector3Int lowerLeft = new(center.x - 5, center.y - 16, center.z);
            coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 33, 11, true));
            lowerLeft += new Vector3Int(-2, 1, 0);
            coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 31, 15, true));
            lowerLeft += new Vector3Int(-2, 1, 0);
            coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 29, 19, true));
            lowerLeft += new Vector3Int(-1, 1, 0);
            coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 27, 21, true));
            lowerLeft += new Vector3Int(-2, 1, 0);
            coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 25, 25, true));
            lowerLeft += new Vector3Int(-1, 2, 0);
            coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 21, 27, true));
            lowerLeft += new Vector3Int(-1, 1, 0);
            coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 19, 29, true));
            lowerLeft += new Vector3Int(-1, 2, 0);
            coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 15, 31, true));
            lowerLeft += new Vector3Int(-1, 2, 0);
            coordinates.UnionWith(GetAreaAroundPosition(lowerLeft, 11, 33, true));
            coordinates.Remove(center);
            coordinates.Remove(new Vector3Int(center.x, center.y + 1, center.z));
            return coordinates.ToList();
        }

        [Obsolete("getPositionsOfBuilding is deprecated, use getAreaArroundPosition with flipped = true instead.")]
        ///<summary>Given the bottom left Vector3Int, calculate the vectors3Ints for the building</summary>
        ///<param name="currentPos">the vector containing the bottom left coordinates of the rectangle</param>
        ///<param name="height">the height of the rectangle</param>
        ///<param name="width">the width of the rectangle</param>
        ///<returns>An array of Vector3Int that containts every vector in the rectangle in correct order to parse in Tilemap.SetTiles()</returns>
        public static List<Vector3Int> GetPositionsOfBuilding(Vector3Int currentPos, int height, int width) {
            List<Vector3Int> BoundsArray = new();
            for (int i = 0; i < height; i++) {
                for (int j = 0; j < width; j++) {
                    BoundsArray.Add(new Vector3Int(currentPos.x + j, currentPos.y - i + height - 1, currentPos.z));
                }
            }
            return BoundsArray;
        }

        ///<summary>Find if a any tile of a rectangle is ovverlapping with invalidTiles in the specified area</summary>
        ///<param name="position">the vector containing the bottom left coordinates of the rectangle</param>
        ///<param name="height">the height of the rectangle</param>
        ///<param name="width">the width of the rectangle</param>
        ///<param name="invalidTiles">the list that checks for overlapping</param>
        ///<returns>True if there are no other tiles and within the rectangle, otherwise false</returns>
        public static bool AreaIsNotObscured(Vector3Int position, int height, int width, HashSet<Vector3Int> invalidTiles) {
            List<Vector3Int> tilesToCheck = GetAreaAroundPosition(position, height, width);
            foreach (Vector3Int vector in tilesToCheck) {
                if (invalidTiles.Contains(vector)) {
                    return false;
                }
            }
            return true;
        }

        ///<summary>Create a Game Object with a Tilemap and a Tilemap Renderer components attached</summary>
        ///<param name="parent">the transform of the game object you want to be the parent of the created tilemap game object</param>
        ///<param name="name">the name of the created object</param>
        ///<returns>True if there are no other tiles and within the rectangle, otherwise false</returns>
        public static GameObject CreateTilemapObject(Transform parent, int sortingOrder, String name = "Tilemap"){
            GameObject tilemap = new(name);
            tilemap.transform.parent = parent.transform;
            tilemap.AddComponent<Tilemap>();
            tilemap.AddComponent<TilemapRenderer>();
            tilemap.GetComponent<TilemapRenderer>().sortingOrder = sortingOrder;
            return tilemap;
        }

        public static GameObject AddTilemapToObject(GameObject obj){
            if (obj.GetComponent<Tilemap>() == null) obj.AddComponent<Tilemap>();
            if (obj.GetComponent<TilemapRenderer>() == null)obj.AddComponent<TilemapRenderer>();
            // obj.AddComponent<Tilemap>();
            // obj.AddComponent<TilemapRenderer>();
            return obj;
        }

        public static HashSet<Vector3Int> GetAllCoordinatesInArea(Vector3Int point1, Vector3Int point2){
            HashSet<Vector3Int> coordinates = new();

            // Determine the lower left and upper right corners
            Vector3Int lowerLeft = new(Math.Min(point1.x, point2.x), Math.Min(point1.y, point2.y), 0);
            Vector3Int upperRight = new(Math.Max(point1.x, point2.x), Math.Max(point1.y, point2.y), 0);

            for (int x = lowerLeft.x; x <= upperRight.x; x++){
                for (int y = lowerLeft.y; y <= upperRight.y; y++){
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
        public static Vector3 GetMiddleOfBuildingWorld(Building building){
            int width = building.Width;
            int height = building.Height;
            Vector3 result = new(-1,-1);
            if (width % 2 != 0){
                Vector3Int leftMiddle = new(building.BaseCoordinates[0].x + Mathf.FloorToInt(width / 2.0f), 0, 0);
                Vector3Int rightMiddle = new(building.BaseCoordinates[0].x + Mathf.CeilToInt(width / 2.0f), 0, 0);
                Vector3 leftMiddleWorld = building.Tilemap.CellToWorld(leftMiddle);
                Vector3 rightMiddleWorld = building.Tilemap.CellToWorld(rightMiddle);
                result.x = (leftMiddleWorld.x + rightMiddleWorld.x) / 2;
            }else result.x = building.Tilemap.CellToWorld(new Vector3Int(building.BaseCoordinates[0].x + Mathf.FloorToInt(width / 2),0,0)).x;
            if (height % 2 != 0){
                Vector3Int downMiddle = new(0, building.BaseCoordinates[0].y + Mathf.FloorToInt(height / 2.0f), 0);
                Vector3Int upMiddle = new(0, building.BaseCoordinates[0].y + Mathf.CeilToInt(height / 2.0f), 0);
                Vector3 downMiddleWorld = building.Tilemap.CellToWorld(downMiddle);
                Vector3 upMiddleWorld = building.Tilemap.CellToWorld(upMiddle);
                result.y = (upMiddleWorld.y + downMiddleWorld.y) / 2;
            }else result.y = building.Tilemap.CellToWorld(new Vector3Int(0, building.BaseCoordinates[0].y + height / 2, 0)).y;
                
            
            return result;
        }

    }
}

