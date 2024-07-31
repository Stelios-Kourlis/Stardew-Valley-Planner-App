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
    private Building Building => gameObject.GetComponent<Building>();

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
        if (!animalsPerTier[GetComponent<TieredBuildingComponent>().Tier].Contains(animal)) { GetNotificationManager().SendNotification($"Cannot add {animal} to Barn tier {GetComponent<TieredBuildingComponent>().Tier}", NotificationManager.Icons.ErrorIcon); return false; }
        AddAnimalButton(animal);
        return true;
    }

    public void UpdateMaxAnimalCapacity(int tier) {
        if (UIElements[1] != null) {
            UIElements[1].GetComponent<Image>().sprite = animalsInBuildingPanelBackgroundAtlas.GetSprite($"AnimalsInBuilding{MaxAnimalCapacity}");
            UIElements[1].GetComponent<RectTransform>().sizeDelta = new Vector2(620, 100 * tier + 100);
            UIElements[1].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(580, 100 * tier + 100 - 50);
        }

        var animalsToRemove = AnimalsInBuilding.Where(animal => !animalsPerTier[tier].Contains(animal.Key)).ToList();
        foreach (var pair in animalsToRemove) {
            Destroy(pair.Value);
            AnimalsInBuilding.Remove(pair);
        }
        if (AnimalsInBuilding.Count > MaxAnimalCapacity) GetNotificationManager().SendNotification($"Removed {AnimalsInBuilding.Count - MaxAnimalCapacity} animals that exceed the new capacity of {GetType()}", NotificationManager.Icons.InfoIcon);
        while (AnimalsInBuilding.Count > MaxAnimalCapacity) {
            Destroy(AnimalsInBuilding.Last().Value);
            AnimalsInBuilding.Remove(AnimalsInBuilding.Last());
        }
    }

    public void AddAnimalMenuObject() {
        //Animal Add Panel (Upper)
        GameObject animalMenuPrefab = Resources.Load<GameObject>($"UI/{Building.GetType()}AnimalMenu");
        GameObject animalMenu = Instantiate(animalMenuPrefab);
        animalMenu.transform.SetParent(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").transform);//this is the button to toggle the animal menu
        animalMenu.GetComponent<RectTransform>().position = new(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.x - 100, gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.y + 25);
        animalMenu.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        animalMenu.SetActive(false);
        GameObject animalMenuContent = animalMenu.transform.GetChild(0).gameObject;
        for (int childIndex = 0; childIndex < animalMenuContent.transform.childCount; childIndex++) {
            Button addAnimalButton = animalMenuContent.transform.GetChild(childIndex).GetComponent<Button>();
            AddHoverEffect(addAnimalButton);
            addAnimalButton.onClick.AddListener(() => {
                AddAnimal((Animals)Enum.Parse(typeof(Animals), addAnimalButton.gameObject.name));
            });
        }

        //Animals In Building Panel (Lower)
        GameObject animalInBuildingMenuPrefab = Resources.Load<GameObject>("UI/AnimalsInBuilding");
        GameObject animalInBuilding = Instantiate(animalInBuildingMenuPrefab);
        animalInBuilding.transform.SetParent(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").transform);
        animalInBuilding.GetComponent<RectTransform>().position = new(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.x - 100, gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.position.y - 25);
        animalInBuilding.GetComponent<RectTransform>().localScale = new Vector2(1, 1);
        animalInBuilding.SetActive(false);

        animalInBuilding.GetComponent<Image>().sprite = animalsInBuildingPanelBackgroundAtlas.GetSprite($"AnimalsInBuilding{MaxAnimalCapacity}");

        UIElements[0] = animalMenu;
        UIElements[1] = animalInBuilding;
    }

    private void AddAnimalButton(Animals animal) {
        GameObject button = new(animal.ToString());
        AnimalsInBuilding.Add(new KeyValuePair<Animals, GameObject>(animal, button));
        button.transform.SetParent(gameObject.GetComponent<InteractableBuildingComponent>().ButtonParentGameObject.transform.Find("ADD_ANIMAL").GetChild(1).GetChild(0));
        button.AddComponent<Image>().sprite = AnimalAtlas.GetSprite(animal.ToString());
        button.AddComponent<Button>().onClick.AddListener(() => {
            AnimalsInBuilding.Remove(new KeyValuePair<Animals, GameObject>(animal, button));
            Destroy(button);
        });
        AddHoverEffect(button.GetComponent<Button>());
        button.transform.localScale = new Vector3(1, 1);
    }

    public void ToggleAnimalMenu() {
        UIElements[0].SetActive(!UIElements[0].activeSelf);
        UIElements[1].SetActive(!UIElements[1].activeSelf);
    }
}
