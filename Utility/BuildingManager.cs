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
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", new ExtensionFilter[]{new ExtensionFilter("Stardew Valley Planner Files", "svp")}, false);
            Type currentType = GetBuildingController().currentBuildingType;
            if (paths.Length > 0) {
                using StreamReader reader = new StreamReader(paths[0]);
                GetBuildingController().DeleteAllBuildings(true);
                GetBuildingController().isLoadingSave = true;
                while (reader.Peek() >= 0){
                    string line = reader.ReadLine();
                    if (line.Equals("")) continue;
                    GetBuildingController().PlaceSavedBuilding(line);
                }
                GetBuildingController().isLoadingSave = false;
            }
            GetBuildingController().SetCurrentBuildingType(currentType);
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
