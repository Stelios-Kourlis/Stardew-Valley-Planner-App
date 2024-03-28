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

        public static void Save() {
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
                GetBuildingController().IsLoadingSave = true;
                while (reader.Peek() >= 0){
                    string line = reader.ReadLine();
                    if (line.Equals("")) continue;
                    GetBuildingController().PlaceSavedBuilding(line);
                }
                GetBuildingController().IsLoadingSave = false;
            }
            GetBuildingController().SetCurrentBuildingType(currentType);
        }

        public static bool CanBuildingBePlacedThere(Vector3Int position, Building building){
            MapController.MapTypes mapType = GetMapController().CurrentMapType;
            HashSet<Type> cantBePlacedOnGingerInslad = new HashSet<Type>{
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

        /// <summary>
        /// Create a button to set the current building to the building type given
        /// </summary>
        /// <param name="name">the name of the GameObject created</param>
        /// <param name="imagePath">the path of the image for the button</param>
        /// <param name="type">The type of building this button creates</param>
        /// <param name="transform">The transform of the parent object</param>
        public static void CreateButton(string name, string imagePath, Transform transform, Type type, Action action = null){
            GameObject button = GameObject.Instantiate(Resources.Load<GameObject>("UI/BuildingButton"), transform);
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath);
            button.GetComponent<Button>().onClick.AddListener(() => { 
                GetBuildingController().SetCurrentBuildingType(type);
                GetBuildingController().SetCurrentAction(Actions.PLACE); 
                action?.Invoke();
                });
            button.name = name;
        }

        /// <summary>
        /// Create a button to set the current building to scarecrow
        /// </summary>
        /// <param name="name">the name of the GameObject created</param>
        /// <param name="imagePath">the path of the image for the button</param>
        /// <param name="transform">The transform of the parent object</param>
        /// <param name="isScarecrowDeluxe"> If the scarecrow is deluxe</param>
        public static void CreateButton(string name, string imagePath, Transform transform, bool isScarecrowDeluxe){
            GameObject button = GameObject.Instantiate(Resources.Load<GameObject>("UI/BuildingButton"), transform);
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath);
            button.GetComponent<Button>().onClick.AddListener(() => { 
                GetBuildingController().SetCurrentBuildingToScarecrow(isScarecrowDeluxe);
                GetBuildingController().SetCurrentAction(Actions.PLACE); 
                });
            button.name = name;
        }

        /// <summary>
        /// Create a button to set the current building to floor and set the floor type
        /// </summary>
        /// <param name="name">the name of the GameObject created</param>
        /// <param name="imagePath">the path of the image for the button</param>
        /// <param name="transform">The transform of the parent object</param>
        /// <param name="floorType">The type of floor</param>
        public static void CreateButton(string name, string imagePath, Transform transform, Floor.Type floorType){
            GameObject button = GameObject.Instantiate(Resources.Load<GameObject>("UI/BuildingButton"), transform);
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath);
            button.GetComponent<Button>().onClick.AddListener(() => { 
                GetBuildingController().SetCurrentBuildingToFloor(floorType);
                GetBuildingController().SetCurrentAction(Actions.PLACE); 
                });
            button.name = name;
        }

        /// <summary>
        /// Create a button to set the current building to fence and set the fence type
        /// </summary>
        /// <param name="name">the name of the GameObject created</param>
        /// <param name="imagePath">the path of the image for the button</param>
        /// <param name="transform">The transform of the parent object</param>
        /// <param name="floorType">The type of floor</param>
        public static void CreateButton(string name, string imagePath, Transform transform, Fence.Type fenceType){
            GameObject button = GameObject.Instantiate(Resources.Load<GameObject>("UI/BuildingButton"), transform);
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath);
            button.GetComponent<Button>().onClick.AddListener(() => { 
                GetBuildingController().SetCurrentBuildingToFence(fenceType);
                GetBuildingController().SetCurrentAction(Actions.PLACE); 
                });
            button.name = name;
        }

        /// <summary>
        /// Create a button to set the current building to cabin and set the cabin type
        /// </summary>
        /// <param name="name">the name of the GameObject created</param>
        /// <param name="imagePath">the path of the image for the button</param>
        /// <param name="transform">The transform of the parent object</param>
        /// <param name="floorType">The type of floor</param>
        public static void CreateButton(string name, string imagePath, Transform transform, Cabin.CabinTypes cabinType){
            GameObject button = GameObject.Instantiate(Resources.Load<GameObject>("UI/BuildingButton"), transform);
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath);
            button.GetComponent<Button>().onClick.AddListener(() => { 
                GetBuildingController().SetCurrentBuildingToCabin(cabinType);
                GetBuildingController().SetCurrentAction(Actions.PLACE); 
                });
            button.name = name;
        }


        /// <summary>
        /// Create a button to set the current building to floor and set the floor type
        /// </summary>
        /// <param name="name">the name of the GameObject created</param>
        /// <param name="imagePath">the path of the image for the button</param>
        /// <param name="transform">The transform of the parent object</param>
        /// <param name="craftableType">The type of craftable</param>
        public static void CreateButton(string name, string imagePath, Transform transform, Craftables.Type craftableType){
            GameObject button = GameObject.Instantiate(Resources.Load<GameObject>("UI/BuildingButton"), transform);
            button.GetComponent<Image>().sprite = Resources.Load<Sprite>(imagePath);
            button.GetComponent<Button>().onClick.AddListener(() => { 
                // Debug.Log("Set Craftable to " + craftableType + " in BuildingManager");
                GetBuildingController().SetCurrentBuildingToPlaceable(craftableType);
                GetBuildingController().SetCurrentAction(Actions.PLACE); 
                });
            button.name = name;
        }

        public static void AddHoverEffect(Button button){
            EventTrigger eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
            pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
            pointerEnterEntry.callback.AddListener((eventData) => {
                button.transform.localScale = new Vector3(1.2f, 1.2f);
            });
            eventTrigger.triggers.Add(pointerEnterEntry);
            EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
            pointerExitEntry.eventID = EventTriggerType.PointerExit;
            pointerExitEntry.callback.AddListener((eventData) => {
                button.transform.localScale = new Vector3(1, 1);
            });
            eventTrigger.triggers.Add(pointerExitEntry);
    }
    }
}
