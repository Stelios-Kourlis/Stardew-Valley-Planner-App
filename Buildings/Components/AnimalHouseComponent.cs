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
    public Dictionary<int, HashSet<Animals>> animalsPerTier;
    private IAnimalHouse Building => gameObject.GetComponent<IAnimalHouse>();

    public void Awake() {
        AnimalAtlas = Resources.Load($"{Building.GetType()}AnimalsAtlas") as SpriteAtlas;
        animalsInBuildingPanelBackgroundAtlas = Resources.Load("UI/AnimalsInBuildingAtlas") as SpriteAtlas;
        AnimalsInBuilding = new List<KeyValuePair<Animals, GameObject>>();
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        gameObject.GetComponent<InteractableBuildingComponent>().ButtonsCreated += AddAnimalMenuObject;
        gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.ADD_ANIMAL);
    }

    public AnimalHouseComponent SetAllowedAnimalsPerTier(Dictionary<int, HashSet<Animals>> animalsPerTier) {
        this.animalsPerTier = animalsPerTier;
        return this;
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
        //Animal Add Panel (Upper)
        // GameObject animalMenuPrefab = Building.GetType() switch {
        //     Type t when t == typeof(Coop) => Resources.Load<GameObject>("UI/CoopAnimalMenu"),
        //     Type t when t == typeof(Barn) => Resources.Load<GameObject>("UI/BarnAnimalMenu"),
        //     _ => throw new ArgumentException("This should never happen")
        // };
        GameObject animalMenuPrefab = Resources.Load<GameObject>($"UI/{Building.GetType()}AnimalMenu");
        GameObject animalMenu = Instantiate(animalMenuPrefab);
        animalMenu.transform.SetParent(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").transform);//this is the button to toggle the animal menu
        animalMenu.GetComponent<RectTransform>().position = new(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.x - 100, Building.ButtonParentGameObject.transform.position.y + 25);
        animalMenu.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        animalMenu.SetActive(false);
        GameObject animalMenuContent = animalMenu.transform.GetChild(0).gameObject;
        for (int childIndex = 0; childIndex < animalMenuContent.transform.childCount; childIndex++) {
            Button addAnimalButton = animalMenuContent.transform.GetChild(childIndex).GetComponent<Button>();
            AddHoverEffect(addAnimalButton);
            addAnimalButton.onClick.AddListener(() => {
                Building.AddAnimal((Animals)Enum.Parse(typeof(Animals), addAnimalButton.gameObject.name));
            });
        }

        //Animals In Building Panel (Lower)
        GameObject animalInBuildingMenuPrefab = Resources.Load<GameObject>("UI/AnimalsInBuilding");
        GameObject animalInBuilding = Instantiate(animalInBuildingMenuPrefab);
        animalInBuilding.transform.SetParent(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").transform);
        animalInBuilding.GetComponent<RectTransform>().position = new(Building.ButtonParentGameObject.transform.position.x - 100, Building.ButtonParentGameObject.transform.position.y - 25);
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
