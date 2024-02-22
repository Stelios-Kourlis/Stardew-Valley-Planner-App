using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.Tilemaps;

public class TileBuildingData : MonoBehaviour {

    ///<summary>Link this buildingBaseCoordinates with PlaceBuilding.buildingBaseCoordinates</summary>am>
    void Start() {
    }

    ///<summary>Place all the invalid tiles in the PlaceBuildng.buildingBaseCoordinates list</summary>
    ///<param name="farm">The name of the farm, possible farms:
    ///Normal, Riverland, Forest, Hilltop, Wilderness, Four Corners, Beach. IS CASE SENSITIVE</param>
    ///<returns>True if there are no other tiles and within the rectangle, otherwise false</returns>
    public void AddInvalidTilesData(string farm) {

        List<Vector3Int> tempList = new List<Vector3Int>();
        string path = "Assets/Resources/Maps/" + farm + ".txt";
        TextReader reader = File.OpenText(path);
        string text;
        while ((text = reader.ReadLine()) != null) {
            string[] nums = text.Split(' ');
            int x = int.Parse(nums[0]);
            int y = int.Parse(nums[1]);
            int z = int.Parse(nums[2]);
            tempList.Add(new Vector3Int(x, y, z));
        }
        // if (farm.Equals("Normal")) {
        //     for (int i = -36; i < 29; i++) {//Left Column
        //         tempList.Add(new Vector3Int(-25, i, 0));
        //         tempList.Add(new Vector3Int(-26, i, 0));
        //         tempList.Add(new Vector3Int(-27, i, 0));
        //     }
        //     for (int i = -27; i < 53; i++) {//Down Row
        //         if (i == 13 || i == 14) continue;
        //         tempList.Add(new Vector3Int(i, -34, 0));
        //         tempList.Add(new Vector3Int(i, -35, 0));
        //         tempList.Add(new Vector3Int(i, -36, 0));
        //     }
        //     for (int i = -36; i < 29; i++) {//Right Column
        //         if (i == 10 || i == 11 || i == 12 || i == 13) continue;
        //         if (i < 11) tempList.Add(new Vector3Int(50, i, 0));
        //         tempList.Add(new Vector3Int(51, i, 0));
        //         tempList.Add(new Vector3Int(52, i, 0));
        //     }
        //     for (int i = -27; i < 53; i++) {//Upper Row
        //         if (i == 13 || i == 14) continue;
        //         if (!(i >= 19 && i <= 25)) tempList.Add(new Vector3Int(i, 21, 0));
        //         tempList.Add(new Vector3Int(i, 22, 0));
        //         tempList.Add(new Vector3Int(i, 23, 0));
        //         tempList.Add(new Vector3Int(i, 24, 0));
        //         tempList.Add(new Vector3Int(i, 25, 0));
        //         tempList.Add(new Vector3Int(i, 26, 0));
        //         tempList.Add(new Vector3Int(i, 27, 0));
        //         tempList.Add(new Vector3Int(i, 28, 0));
        //     }
        //     tempList.Add(new Vector3Int(21, 21, 0)); // Farm Statue
        //     tempList.Add(new Vector3Int(-24, 20, 0)); // Upper Left Corner
        //     tempList.Add(new Vector3Int(-18, 20, 0)); // Grandpa Shrine
        //     tempList.Add(new Vector3Int(-19, 20, 0));
        //     tempList.Add(new Vector3Int(-20, 20, 0));
        //     for (int i = 26; i < 51; i++) { // Above House
        //         tempList.Add(new Vector3Int(i, 20, 0));
        //         if (i >= 28) tempList.Add(new Vector3Int(i, 19, 0));
        //     }
        //     for (int i = 43; i < 49; i++) { // Small Lake
        //         for (int j = -5; j < 1; j++) {
        //             tempList.Add(new Vector3Int(i, j, 0));
        //         }
        //     }
        //     tempList.Remove(new Vector3Int(43, -5, 0));
        //     tempList.Remove(new Vector3Int(48, -5, 0));
        //     tempList.Remove(new Vector3Int(48, 0, 0));
        //     for (int i = -7; i < 7; i++) { // Left Wall Extrusion
        //         if (i >= -5) tempList.Add(new Vector3Int(-21, i, 0));
        //         if (i >= -5) tempList.Add(new Vector3Int(-22, i, 0));
        //         if (i >= -6) tempList.Add(new Vector3Int(-23, i, 0));
        //         tempList.Add(new Vector3Int(-24, i, 0));
        //     }

        //     for (int i = 12; i < 18; i++) { // House
        //         for (int j = 32; j < 41; j++) {
        //             tempList.Add(new Vector3Int(j, i, 0));
        //         }
        //     }

        //     for (int i = 12; i < 18; i++) { // Big Lake
        //         for (int j = 32; j < 41; j++) {
        //             tempList.Add(new Vector3Int(j, i, 0));
        //         }
        //     }
        //     //49 -33 0
        //     // 42
        //     for (int i = 42; i < 50; i++) {
        //         tempList.Add(new Vector3Int(i, -33, 0));
        //         tempList.Add(new Vector3Int(i, -32, 0));
        //         tempList.Add(new Vector3Int(i, -31, 0));
        //     }
        //     for (int i = 46; i < 50; i++) {
        //         tempList.Add(new Vector3Int(i, -30, 0));
        //         tempList.Add(new Vector3Int(i, -29, 0));
        //         tempList.Add(new Vector3Int(i, -28, 0));
        //     }

        //     for (int i = -30; i < -20; i++) {
        //         if (i == -24 || i == -25 || i == -26) tempList.Add(new Vector3Int(6, i, 0));
        //         if (i >= -28 && i <= -23) tempList.Add(new Vector3Int(7, i, 0));
        //         if (i >= -28 && i <= -22) tempList.Add(new Vector3Int(8, i, 0));
        //         if (i >= -29 && i <= -21) tempList.Add(new Vector3Int(9, i, 0));
        //         tempList.Add(new Vector3Int(10, i, 0));
        //         tempList.Add(new Vector3Int(11, i, 0));
        //         tempList.Add(new Vector3Int(12, i, 0));
        //         tempList.Add(new Vector3Int(13, i, 0));
        //         tempList.Add(new Vector3Int(14, i, 0));
        //         if (i != -30) tempList.Add(new Vector3Int(15, i, 0));
        //         if (i != -30 && i != -21) tempList.Add(new Vector3Int(16, i, 0));
        //         if (i >= -29 && i <= -23) tempList.Add(new Vector3Int(17, i, 0));
        //         if (i >= -28 && i <= -23) tempList.Add(new Vector3Int(18, i, 0));
        //         if (i >= -27 && i <= -24) tempList.Add(new Vector3Int(19, i, 0));
        //     }

        // }
        MainController script = GameObject.FindGameObjectWithTag("LogicComponent").GetComponent<MainController>();
        foreach (Vector3Int vec in tempList) GameObject.FindGameObjectWithTag("Grid").GetComponent<BuildingController>().GetUnavailableCoordinates().Add(vec);

    }

    public void RemoveAllDuplicates() {
        string[] names = { "Normal", "Riverland", "Forest", "Hilltop", "Wilderness", "Four Corners", "Beach" };
        foreach (string mapName in names) {
            string path = "Assets/Resources/Maps" + mapName + ".txt";
            HashSet<Vector3Int> tiles = new HashSet<Vector3Int>();
            TextReader reader = File.OpenText(path);
            string text;
            while ((text = reader.ReadLine()) != null) {
                string[] nums = text.Split(' ');
                int x = int.Parse(nums[0]);
                int y = int.Parse(nums[1]);
                int z = int.Parse(nums[2]);
                tiles.Add(new Vector3Int(x, y, z));
            }
            StreamWriter writer = new StreamWriter(path, true);
            foreach (Vector3Int vec in tiles) {
                writer.WriteLine(vec.x + " " + vec.y + " " + vec.z);
            }
            writer.Close();
        }
    }
}
