// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO;
// using UnityEngine.Tilemaps;
//todo DELETE ALL

// public class TileBuildingData : MonoBehaviour {

//     // ///<summary>Place all the invalid tiles in the PlaceBuildng.buildingBaseCoordinates list</summary>
//     // ///<param name="farm">The name of the farm, possible farms:
//     // ///Normal, Riverland, Forest, Hilltop, Wilderness, Four Corners, Beach. IS CASE SENSITIVE</param>
//     // ///<returns>True if there are no other tiles and within the rectangle, otherwise false</returns>
//     // public void AddInvalidTilesData(MapController.MapTypes farm) {
//     //     List<Vector3Int> tempList = new();
//     //     string path = "Maps/" + farm.ToString();
//     //     // Debug.Log($"Loading Invalid Tiles Data For {path}");
//     //     TextAsset textAsset = Resources.Load<TextAsset>(path);
//     //     string[] tiles = textAsset.text.Split('\n');
//     //     foreach (string tile in tiles) {
//     //         if (tile == "") continue;
//     //         string[] nums = tile.Split(' ');
//     //         int x = int.Parse(nums[0]);
//     //         int y = int.Parse(nums[1]);
//     //         int z = int.Parse(nums[2]);
//     //         tempList.Add(new Vector3Int(x, y, z));
//     //     }
//     //     BuildingController.specialCoordinates.ClearAll();
//     //     BuildingController.specialCoordinates.AddSpecialTileSet(new($"{farm}Invalid", tempList, TileType.Invalid));
//     // }

//     // public void AddPlantableTilesData(MapController.MapTypes farm) {
//     //     List<Vector3Int> tempList = new();
//     //     string path = "Maps/" + farm.ToString() + "P";
//     //     // Debug.Log($"Loading Plantable Tiles Data For {path}");
//     //     TextAsset textAsset = Resources.Load<TextAsset>(path);
//     //     string[] tiles = textAsset.text.Split('\n');
//     //     foreach (string tile in tiles) {
//     //         if (tile == "") continue;
//     //         string[] nums = tile.Split(' ');
//     //         int x = int.Parse(nums[0]);
//     //         int y = int.Parse(nums[1]);
//     //         int z = int.Parse(nums[2]);
//     //         tempList.Add(new Vector3Int(x, y, z));
//     //         // Debug.Log("Added " + x + " " + y + " " + z);
//     //     }
//     //     BuildingController.specialCoordinates.ClearAll();
//     //     BuildingController.specialCoordinates.AddSpecialTileSet(new($"{farm}Plantable", tempList, TileType.Plantable));
//     // }

//     public void RemoveAllDuplicates() {
//         string[] names = { "Normal", "Riverland", "Forest", "Hilltop", "Wilderness", "Four Corners", "Beach", "GingerIsland" };
//         foreach (string mapName in names) {
//             string path = "Assets/Resources/Maps" + mapName + ".txt";
//             HashSet<Vector3Int> tiles = new();
//             TextReader reader = File.OpenText(path);
//             string text;
//             while ((text = reader.ReadLine()) != null) {
//                 string[] nums = text.Split(' ');
//                 int x = int.Parse(nums[0]);
//                 int y = int.Parse(nums[1]);
//                 int z = int.Parse(nums[2]);
//                 tiles.Add(new Vector3Int(x, y, z));
//             }
//             StreamWriter writer = new(path, true);
//             foreach (Vector3Int vec in tiles) {
//                 writer.WriteLine(vec.x + " " + vec.y + " " + vec.z);
//             }
//             writer.Close();
//         }

//         foreach (string mapName in names) {
//             string path = "Assets/Resources/Maps" + mapName + "P.txt";
//             HashSet<Vector3Int> tiles = new();
//             TextReader reader = File.OpenText(path);
//             string text;
//             while ((text = reader.ReadLine()) != null) {
//                 string[] nums = text.Split(' ');
//                 int x = int.Parse(nums[0]);
//                 int y = int.Parse(nums[1]);
//                 int z = int.Parse(nums[2]);
//                 tiles.Add(new Vector3Int(x, y, z));
//             }
//             StreamWriter writer = new(path, true);
//             foreach (Vector3Int vec in tiles) {
//                 writer.WriteLine(vec.x + " " + vec.y + " " + vec.z);
//             }
//             writer.Close();
//         }
//     }
// }
