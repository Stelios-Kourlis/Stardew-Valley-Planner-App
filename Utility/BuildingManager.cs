using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SFB;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using static Utility.ClassManager;

namespace Utility{
    public static class BuildingManager{
        
        /// <summary>
        /// Save the current state of the buildings to a file
        /// </summary>
        /// <returns>true, if the user saved, false if the user cancelled</returns>
        public static bool Save() {
            string defaultSavePath = PlayerPrefs.GetString("DefaultSavePath", Application.dataPath);
            string savePath = StandaloneFileBrowser.SaveFilePanel("Choose a save location", defaultSavePath, "FarmScreenshot", "png");;
            if (savePath != "") {
                string directoryPath = Path.GetDirectoryName(savePath);
                PlayerPrefs.SetString("DefaultScreenshotPath", directoryPath);
                using StreamWriter writer = new(savePath);
                foreach (Building building in GetBuildingController().GetBuildings()) {
                    writer.WriteLine(building.GetBuildingData());
                }
            }
            return savePath != "";
        }

        /// <summary>
        /// Load a farm from a file
        /// </summary>
        /// <returns>true, if the user chose a file, false if the user cancelled</returns>
        public static bool Load() {
            string defaultLoadPath = PlayerPrefs.GetString("DefaultLoadPath", Application.dataPath);
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", defaultLoadPath, new ExtensionFilter[]{new("Stardew Valley Planner Files", "svp")}, false);
            Type currentType = GetBuildingController().currentBuildingType;
            if (paths.Length > 0) {
                string directoryPath = Path.GetDirectoryName(paths[0]);
                PlayerPrefs.SetString("DefaultScreenshotPath", directoryPath);
                using StreamReader reader = new(paths[0]);
                GetBuildingController().DeleteAllBuildings(true);
                GetBuildingController().IsLoadingSave = true;
                while (reader.Peek() >= 0){
                    string line = reader.ReadLine();
                    if (line.Equals("")) continue;
                    GetBuildingController().PlaceSavedBuilding(line);
                }
                GetBuildingController().IsLoadingSave = false;
            }
            GetBuildingController().SetCurrentBuildingType(currentType); 
            return paths.Length > 0;
        }

        public static bool CanBuildingBePlacedThere(Vector3Int position, Building building){
            MapController.MapTypes mapType = GetMapController().CurrentMapType;
            HashSet<Type> cantBePlacedOnGingerInslad = new()
            {
                typeof(Barn),
                typeof(Cabin),
                typeof(Coop),
                typeof(FishPond),
                typeof(GoldClock),
                typeof(Greenhouse),
                typeof(House),
                typeof(JunimoHut),
                typeof(Mill),
                typeof(Obelisk),
                typeof(Shed),
                typeof(ShippingBin),
                typeof(Silo),
                typeof(SlimeHutch),
                typeof(Stable),
                typeof(Well),
            };
            if (mapType == MapController.MapTypes.GingerIsland && cantBePlacedOnGingerInslad.Contains(building.GetType())){
                GetNotificationManager().SendNotification($"{building.GetType()} can't be placed on Ginger Island");
                return false;
            }
            if (building.GetType() == typeof(Crop) && !GetBuildingController().GetPlantableCoordinates().Contains(position)){
                GetNotificationManager().SendNotification("Can't place a crop there");
                return false;
            }
            return true;
        }

        public static bool LeftClickShouldRegister(){
            if (EventSystem.current.currentSelectedGameObject && EventSystem.current.currentSelectedGameObject.name != "TopRightButtons") return true;
            if (EventSystem.current.IsPointerOverGameObject()) return false;
            if (GetSettingsModalController().IsOpen) return false;
            return true;
        }
        
        public static void AddHoverEffect(Button button){
            EventTrigger eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry pointerEnterEntry = new();
            pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
            pointerEnterEntry.callback.AddListener((eventData) => {
                button.transform.localScale = new Vector3(1.2f, 1.2f);
            });
            eventTrigger.triggers.Add(pointerEnterEntry);
            EventTrigger.Entry pointerExitEntry = new();
            pointerExitEntry.eventID = EventTriggerType.PointerExit;
            pointerExitEntry.callback.AddListener((eventData) => {
                button.transform.localScale = new Vector3(1, 1);
            });
            eventTrigger.triggers.Add(pointerExitEntry);
    }
    }
}
