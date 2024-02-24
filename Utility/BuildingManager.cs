using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SFB;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Utility.ClassManager;

namespace Utility{
    public static class BuildingManager{
        public static Building DeepCopyOfBuilding(string name, List<Vector3Int> position = null, List<Vector3Int> basePosition = null, Tilemap tilemap = null) {
            var buildingTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(Building)) && !type.IsAbstract);
            Dictionary<string, Type> buildingMap = new Dictionary<string, Type>();
            foreach (var buildingType in buildingTypes) {
                buildingMap.Add(buildingType.Name, buildingType);
            }
            if (buildingMap.ContainsKey(name)) {
                var buildingType = buildingMap[name];
                Building building = null;
                if (buildingType == typeof(Floor)) building = (Building)Activator.CreateInstance(buildingType, position[0]);
                else building = (Building) Activator.CreateInstance(buildingType, position, basePosition, tilemap);
                return building;
            }
            return null;
        }   

        public static bool IsSpecialBuilding(Building building) {
            return building is FishPond || building is Greenhouse;
        }

        public static void Save() {//todo add Load
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", new ExtensionFilter[]{new ExtensionFilter("Stardew Valley Planner Files", "svp")}, false);
            if (paths.Length > 0) {
                using StreamWriter writer = new StreamWriter(paths[0]);
                foreach (Building building in GetBuildingController().GetBuildings()) {
                    writer.WriteLine(building.GetBuildingData());
                }
            }
        }

        public static void Load() {
            var paths = StandaloneFileBrowser.SaveFilePanel("Save File", "", "Farm", "svp");
            // var bp = new BrowserProperties{
            //     filter = "Stardew Valley Planner Files (*.svp)|*.svp|All Files (*.*)|*.*",
            //     filterIndex = 0
            // };
            // new FileBrowser().OpenFileBrowser(bp, path => {
            //     TextReader reader = File.OpenText(path);
            //     BuildingController placer = GameObject.FindGameObjectWithTag("Grid").GetComponent<BuildingController>();
            //     string text;
            //     placer.DeleteAllBuildings();
            //     while ((text = reader.ReadLine()) != null) {
            //         string[] data = text.Split('|');
            //         string name = data[0];
            //         int x = int.Parse(data[1]);
            //         int y = int.Parse(data[2]);
            //         //placer.PlaceBuilding(DeepCopyOfBuilding(name), new Vector3Int(x, y, 0));//todo fix this
            //     }
            // });
        }

        /// <summary>
        /// Create a button to set the current building to the building type given
        /// </summary>
        /// <param name="name">the name of the GameObject created</param>
        /// <param name="imagePath">the path of the image for the button</param>
        /// <param name="type">The type of building this button creates</param>
        /// <param name="transform">The transform of the parent object</param>
        /// <param name="floorType">The type of floor, only use if Type is typeof(Floor)</param>
        public static void CreateButton(string name, string imagePath, Type type, Transform transform, FloorType floorType = FloorType.WOOD_FLOOR){
            GameObject button = GameObject.Instantiate(Resources.Load<GameObject>("BuildingButton"), transform);
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath);
            button.GetComponent<Button>().onClick.AddListener(() => { 
                //if (type != typeof(Floor)) GetBuildingController().SetCurrentBuilding(Activator.CreateInstance(type, null, null, null) as Building);
                // else GetBuildingController().SetCurrentBuilding(Activator.CreateInstance(type, floorType) as Building);
                GetBuildingController().SetCurrentBuildingType(type);
                if (type == typeof(Floor)) Floor.floorType = floorType;
                GetBuildingController().SetCurrentAction(Actions.PLACE); 
                });
            button.name = name;
        }
    }
}
