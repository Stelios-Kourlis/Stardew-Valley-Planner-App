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
using static BuildingData;
using Newtonsoft.Json.Linq;

[RequireComponent(typeof(Building))]
[Serializable]
public class AnimalHouseComponent : BuildingComponent {
    public static SpriteAtlas AnimalAtlas { get; private set; }
    private readonly GameObject[] UIElements = new GameObject[2];
    private static SpriteAtlas animalsInBuildingPanelBackgroundAtlas;
    [field: SerializeField] public List<Animals> AnimalsInBuilding { get; private set; }
    public int MaxAnimalCapacity => gameObject.GetComponent<TieredBuildingComponent>().Tier * 4; //capacity is based on tier
    public Dictionary<int, HashSet<Animals>> animalsPerTier;

    // [SerializeField] private GameObject animalMenuPrefab;

    public void Awake() {
        if (AnimalAtlas == null) AnimalAtlas = Resources.Load($"{Building.GetType()}AnimalsAtlas") as SpriteAtlas;
        if (animalsInBuildingPanelBackgroundAtlas == null) animalsInBuildingPanelBackgroundAtlas = Resources.Load("UI/AnimalsInBuildingAtlas") as SpriteAtlas;
        AnimalsInBuilding = new List<Animals>();
        if (!gameObject.GetComponent<InteractableBuildingComponent>()) gameObject.AddComponent<InteractableBuildingComponent>();
        gameObject.GetComponent<InteractableBuildingComponent>().ButtonsCreated += AddAnimalMenuObject;
        gameObject.GetComponent<InteractableBuildingComponent>().AddInteractionToBuilding(ButtonTypes.ADD_ANIMAL);

        gameObject.GetComponent<TieredBuildingComponent>().tierChanged += UpdateMaxAnimalCapacity;
    }

    public AnimalHouseComponent SetAllowedAnimalsPerTier(Dictionary<int, HashSet<Animals>> allowedAnimalsPerTier) {
        animalsPerTier = allowedAnimalsPerTier;
        return this;
    }

    public bool AddAnimal(Animals animal) {
        if (AnimalsInBuilding.Count >= MaxAnimalCapacity) {
            NotificationManager.Instance.SendNotification($"{Building.GetType()} is full ({AnimalsInBuilding.Count}/{MaxAnimalCapacity}), cannot add {animal}", NotificationManager.Icons.ErrorIcon);
            return false;
        }
        if (!animalsPerTier[GetComponent<TieredBuildingComponent>().Tier].Contains(animal)) { NotificationManager.Instance.SendNotification($"Cannot add {animal} to {Building.BuildingName} tier {GetComponent<TieredBuildingComponent>().Tier}", NotificationManager.Icons.ErrorIcon); return false; }
        AnimalsInBuilding.Add(animal);
        AddAnimalButton(animal);
        return true;
    }

    public void UpdateMaxAnimalCapacity(int tier) {
        var animalsToRemove = AnimalsInBuilding.Where(animal => !animalsPerTier[tier].Contains(animal)).ToList();

        AnimalsInBuilding.RemoveAll(animal => animalsToRemove.Contains(animal));

        if (AnimalsInBuilding.Count > MaxAnimalCapacity) NotificationManager.Instance.SendNotification($"Removed {AnimalsInBuilding.Count - MaxAnimalCapacity} animals that exceed the new capacity of {Building.BuildingName}", NotificationManager.Icons.InfoIcon);
        while (AnimalsInBuilding.Count > MaxAnimalCapacity) {
            AnimalsInBuilding.Remove(AnimalsInBuilding.Last());
        }

        UpdateAnimals();
    }

    public void UpdateAnimals() {
        GameObject targetGameObject = gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").GetChild(1).GetChild(0).gameObject;

        foreach (Transform child in targetGameObject.transform) {
            Destroy(child.gameObject);
        }

        foreach (var animal in AnimalsInBuilding) {
            AddAnimalButton(animal);
        }
    }

    public void AddAnimalMenuObject() {
        UIElements[0] = CreateAnimalChoiceMenu();
        UIElements[1] = CreateAnimalsInBuildingMenu();
    }

    private GameObject CreateAnimalChoiceMenu() {
        GameObject animalMenuPrefab = Resources.Load<GameObject>($"UI/{Building.GetType()}AnimalMenu");
        GameObject animalMenu = Instantiate(animalMenuPrefab);
        Resources.UnloadAsset(animalMenuPrefab);
        animalMenu.transform.SetParent(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").transform);//this is the button to toggle the animal menu
        animalMenu.GetComponent<RectTransform>().position = new(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.x - 100, gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.y + 25);
        animalMenu.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        animalMenu.SetActive(false);
        GameObject animalMenuContent = animalMenu.transform.GetChild(0).gameObject;
        for (int childIndex = 0; childIndex < animalMenuContent.transform.childCount; childIndex++) {
            Button addAnimalButton = animalMenuContent.transform.GetChild(childIndex).GetComponent<Button>();
            // AddHoverEffect(addAnimalButton);
            addAnimalButton.gameObject.AddComponent<UIElement>();
            addAnimalButton.onClick.AddListener(() => {
                AddAnimal((Animals)Enum.Parse(typeof(Animals), addAnimalButton.gameObject.name));
            });
        }
        return animalMenu;
    }

    private GameObject CreateAnimalsInBuildingMenu() {
        GameObject animalsInBuildingMenuPrefab = Resources.Load<GameObject>("UI/AnimalsInBuilding");
        GameObject animalsInBuilding = Instantiate(animalsInBuildingMenuPrefab);
        Resources.UnloadAsset(animalsInBuildingMenuPrefab);
        animalsInBuilding.transform.SetParent(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").transform);
        animalsInBuilding.GetComponent<RectTransform>().position = new(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.x - 100, gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.y - 25);
        animalsInBuilding.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        animalsInBuilding.SetActive(false);
        return animalsInBuilding;
    }

    private void AddAnimalButton(Animals animal) {
        GameObject button = new(animal.ToString());
        button.transform.SetParent(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").GetChild(1).GetChild(0));
        button.AddComponent<Image>().sprite = AnimalAtlas.GetSprite(animal.ToString());
        button.AddComponent<Button>().onClick.AddListener(() => {
            AnimalsInBuilding.Remove(animal);
            Destroy(button);
        });
        button.AddComponent<UIElement>();
        button.transform.localScale = new Vector3(1, 1, 1);
    }

    public void ToggleAnimalMenu() {
        UIElements[0].SetActive(!UIElements[0].activeSelf);
        UIElements[1].SetActive(!UIElements[1].activeSelf);
    }

    public override ComponentData Save() {
        ComponentData animalHouseData = new(typeof(AnimalHouseComponent), new());
        foreach (var animal in AnimalsInBuilding) {
            var animalAlreadyExists = animalHouseData.componentData.FirstOrDefault(x => x.Name == animal.ToString());
            if (animalAlreadyExists != null) {
                int index = animalHouseData.componentData.IndexOf(animalAlreadyExists);
                animalHouseData.componentData[index] = new JProperty(animal.ToString(), int.Parse(animalAlreadyExists.Value.ToString()) + 1);
            }
            else animalHouseData.componentData.Add(new JProperty(animal.ToString(), 1));
        }
        return animalHouseData;
    }

    public override void Load(ComponentData data) {
        foreach (var property in data.componentData) {
            for (int count = 0; count < int.Parse((string)property.Value); count++) AddAnimal(Enum.Parse<Animals>(property.Name));
        }
    }
}
