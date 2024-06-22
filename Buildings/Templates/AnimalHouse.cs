using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Utility.TilemapManager;
using static Utility.BuildingManager;
using static Utility.ClassManager;
public class AnimalHouseComponent : MonoBehaviour {
    public SpriteAtlas AnimalAtlas { get; private set; }
    private readonly GameObject[] UIElements = new GameObject[2];
    private SpriteAtlas animalsInBuildingPanelBackgroundAtlas;
    public List<KeyValuePair<Animals, GameObject>> AnimalsInBuilding { get; private set; }
    public int MaxAnimalCapacity => gameObject.GetComponent<TieredBuildingComponent>().Tier * 4; //capacity is based on tier
    private IAnimalHouse Building => gameObject.GetComponent<IAnimalHouse>();

    public void Awake() {
        AnimalAtlas = Resources.Load($"{Building.GetType()}AnimalsAtlas") as SpriteAtlas;
        animalsInBuildingPanelBackgroundAtlas = Resources.Load("UI/AnimalsInBuildingAtlas") as SpriteAtlas;
        AnimalsInBuilding = new List<KeyValuePair<Animals, GameObject>>();
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
    }

    public bool AddAnimal(Animals animal) {
        if (AnimalsInBuilding.Count >= MaxAnimalCapacity) {
            GetNotificationManager().SendNotification($"{Building.GetType()} is full ({AnimalsInBuilding.Count}/{MaxAnimalCapacity}), cannot add {animal}", NotificationManager.Icons.ErrorIcon);
            return false;
        }
        return true;
    }

    public void UpdateMaxAnimalCapacity(int tier) {
        if (UIElements[1] != null) {
            UIElements[1].GetComponent<Image>().sprite = animalsInBuildingPanelBackgroundAtlas.GetSprite($"AnimalsInBuilding{MaxAnimalCapacity}");
            UIElements[1].GetComponent<RectTransform>().sizeDelta = new Vector2(620, 100 * tier + 100);
            UIElements[1].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(580, 100 * tier + 100 - 50);
        }
    }

    public void AddAnimalMenuObject() {
        //Animal Add Panel
        GameObject animalMenuPrefab = Building.GetType() switch {
            // Type t when t == typeof(Coop) => Resources.Load<GameObject>("UI/CoopAnimalMenu"),
            Type t when t == typeof(Barn) => Resources.Load<GameObject>("UI/BarnAnimalMenu"),
            _ => throw new ArgumentException("This should never happen")
        };
        GameObject animalMenu = Instantiate(animalMenuPrefab);
        animalMenu.transform.SetParent(Building.ButtonParentGameObject.transform.GetChild(5).transform);//this is the button to toggle the animal menu
        Vector3 animalMenuPositionWorld = new(Building.Tilemap.CellToWorld(Building.BaseCoordinates[0] + new Vector3Int(1, 0, 0)).x, GetMiddleOfBuildingWorld(Building).y + 4);
        animalMenu.transform.position = Camera.main.WorldToScreenPoint(animalMenuPositionWorld);
        animalMenu.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        animalMenu.SetActive(false);
        GameObject animalMenuContent = animalMenu.transform.GetChild(0).gameObject;
        IAnimalHouse animalHouse = Building as IAnimalHouse;
        for (int childIndex = 0; childIndex < animalMenuContent.transform.childCount; childIndex++) {
            Button addAnimalButton = animalMenuContent.transform.GetChild(childIndex).GetComponent<Button>();
            AddHoverEffect(addAnimalButton);
            addAnimalButton.onClick.AddListener(() => {
                animalHouse.AddAnimal((Animals)Enum.Parse(typeof(Animals), addAnimalButton.gameObject.name));
            });
        }

        //Animals In Building Panel
        GameObject animalInBuildingMenuPrefab = Resources.Load<GameObject>("UI/AnimalsInBuilding");
        GameObject animalInBuilding = GameObject.Instantiate(animalInBuildingMenuPrefab);
        animalInBuilding.transform.SetParent(Building.ButtonParentGameObject.transform.GetChild(5).transform);
        Vector3 animalInBuildingMenuPositionWorld = new(Building.Tilemap.CellToWorld(Building.BaseCoordinates[0] + new Vector3Int(1, 0, 0)).x, GetMiddleOfBuildingWorld(Building).y + 1);
        animalInBuilding.transform.position = Camera.main.WorldToScreenPoint(animalInBuildingMenuPositionWorld);
        animalInBuilding.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        animalInBuilding.SetActive(false);

        animalInBuilding.GetComponent<Image>().sprite = animalsInBuildingPanelBackgroundAtlas.GetSprite($"AnimalsInBuilding{MaxAnimalCapacity}");

        UIElements[0] = animalMenu;
        UIElements[1] = animalInBuilding;
    }

    public void ToggleAnimalMenu() {
        UIElements[0].SetActive(!UIElements[0].activeSelf);
        UIElements[1].SetActive(!UIElements[1].activeSelf);
    }
}
