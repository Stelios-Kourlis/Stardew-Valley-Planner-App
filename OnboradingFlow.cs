using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static Utility.ClassManager;
using static Utility.TilemapManager;

public class OnboradingFlow : MonoBehaviour {

    //vscode is wrong, these fields cannot be made readonly since then they cant be serialized in editor
    [SerializeField] private GameObject tutorialText;
    [SerializeField] private GameObject interactionLegend;
    [SerializeField] private GameObject skipButtonPrefab;
    [SerializeField] private GameObject startTutorialDialog;

    private readonly Dictionary<BuildingType, bool> buildingsPlaced = new(){
        {BuildingType.Greenhouse, false},
        {BuildingType.ShippingBin, false},
        {BuildingType.House, false},
        {BuildingType.PetBowl, false}
    };
    public bool IsInTutorial { get; private set; }

    void Start() {
        // PlayerPrefs.SetInt("HasDoneIntro", 0);
        if (PlayerPrefs.GetInt("HasDoneIntro") == 0) {
            GameObject dialog = Instantiate(startTutorialDialog, GameObject.FindGameObjectWithTag("Canvas").transform);
            dialog.transform.Find("Buttons").Find("Yes").GetComponent<Button>().onClick.AddListener(() => {
                StartOnboardingFlow();
                Destroy(dialog);
            });

            dialog.transform.Find("Buttons").Find("No").GetComponent<Button>().onClick.AddListener(() => {
                Destroy(dialog);
            });
        }
        IsInTutorial = false;
    }

    public void StartOnboardingFlow() {

        IEnumerator OnboardingFlow() {
            BuildingController.DeleteAllBuildings(true);
            float cameraMoveTime = 2f;
            float delay = 0.25f;
            CameraController.Instance.SetSizeSmooth(10, delay);
            yield return new WaitForSecondsRealtime(delay);
            CameraController.Instance.ToggleCameraLock();
            CameraController.Instance.enforceBounds = false;

            CameraController.Instance.SetPositionSmooth(GetGridTilemap().CellToWorld(MapController.Instance.GetGreenhousePosition() + new Vector3Int(3, 3, 0)), cameraMoveTime);
            yield return new WaitForSecondsRealtime(cameraMoveTime);
            BuildingController.PlaceGreenhouse();
            buildingsPlaced[BuildingType.Greenhouse] = true;
            yield return new WaitForSecondsRealtime(delay);

            CameraController.Instance.SetPositionSmooth(GetGridTilemap().CellToWorld(MapController.Instance.GetShippingBinPosition() + new Vector3Int(1, 0, 0)), cameraMoveTime);
            yield return new WaitForSecondsRealtime(cameraMoveTime);
            BuildingController.PlaceBin();
            buildingsPlaced[BuildingType.ShippingBin] = true;
            yield return new WaitForSecondsRealtime(delay);

            CameraController.Instance.SetPositionSmooth(GetGridTilemap().CellToWorld(MapController.Instance.GetHousePosition() + new Vector3Int(4, 5, 0)), cameraMoveTime);
            yield return new WaitForSecondsRealtime(cameraMoveTime);
            BuildingController.PlaceHouse();
            buildingsPlaced[BuildingType.House] = true;
            yield return new WaitForSecondsRealtime(delay);

            CameraController.Instance.SetPositionSmooth(GetGridTilemap().CellToWorld(MapController.Instance.GetPetBowlPosition()), cameraMoveTime);
            yield return new WaitForSecondsRealtime(cameraMoveTime);
            BuildingController.PlacePetBowl();
            buildingsPlaced[BuildingType.PetBowl] = true;
            yield return new WaitForSecondsRealtime(delay);

            CameraController.Instance.SetSizeSmooth(15, cameraMoveTime);
            CameraController.Instance.SetPositionSmooth(new Vector3(45, 45, 0), cameraMoveTime);
            yield return new WaitForSecondsRealtime(cameraMoveTime);
            CameraController.Instance.enforceBounds = true;

            GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
            text.GetComponentInChildren<TMP_Text>().text = "When creating a new farm, some buildings are placed automatically. Default buildings depend on the farm type.";
            text.GetComponentInChildren<Button>().onClick.AddListener(() => {
                Destroy(text);
                ShowHowToMoveCamera();
            });

            CameraController.Instance.ToggleCameraLock();
        }

        MoveablePanel.CloseAllPanels?.Invoke();
        IsInTutorial = true;
        BuildingController.SetCurrentAction(Actions.DO_NOTHING);
        InputHandler.Instance.keybindsShouldRegister = false;
        if (DebugCoordinates.DebugModeinOn) DebugCoordinates.ToggleDebugMode();
        GameObject skipButton = Instantiate(skipButtonPrefab, GameObject.FindGameObjectWithTag("Canvas").transform);
        skipButton.GetComponent<Button>().onClick.AddListener(() => {
            EndOnboardingFlow();
            Destroy(skipButton);
        });
        MapController.Instance.SetMapAsync(MapController.MapTypes.Normal);
        GetCanvasGameObject().transform.Find("TopRightButtons").gameObject.SetActive(false);
        GetCanvasGameObject().transform.Find("NoBuilding").gameObject.SetActive(false);
        GetCanvasGameObject().transform.Find("ToggleBuildingSelectButton").gameObject.SetActive(false);
        StartCoroutine(OnboardingFlow());
    }

    public void ShowHowToMoveCamera() {

        IEnumerator CheckCameraMoved() {
            Vector3 cameraPosition = CameraController.Instance.GetPosition();
            float cameraZoom = CameraController.Instance.GetZoom();

            GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
            text.GetComponentInChildren<TMP_Text>().text = "Move the camera by holding middle mouse button and moving the mouse.\nZoom in and out by scrolling.\nTry both out!";
            text.transform.GetChild(1).gameObject.GetComponent<Button>().interactable = false;

            bool movedCamera = false, changedZoom = false;
            while (true) {
                if (Vector3.Distance(cameraPosition, CameraController.Instance.GetPosition()) >= 10) movedCamera = true;
                if (Mathf.Abs(cameraZoom - CameraController.Instance.GetZoom()) >= 3) changedZoom = true;

                if (movedCamera && changedZoom) break;
                yield return null;
            }

            text.transform.GetChild(1).gameObject.GetComponent<Button>().interactable = true;
            text.GetComponentInChildren<Button>().onClick.AddListener(() => {
                Destroy(text);
                ShowHowToPlaceBuilding();
            });

        }

        StartCoroutine(CheckCameraMoved());
    }

    public void ShowHowToPlaceBuilding() {
        GetCanvasGameObject().transform.Find("ToggleBuildingSelectButton").gameObject.SetActive(true);
        GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
        text.GetComponentInChildren<TMP_Text>().text = "To select a building to place press the button on the top left corner. Try placing a building!";
        text.transform.GetChild(1).gameObject.SetActive(false);

        void onBuildingSelectOpen() {
            Destroy(text);
            GetCanvasGameObject().transform.Find("ToggleBuildingSelectButton").GetComponent<Button>().onClick.RemoveListener(onBuildingSelectOpen);
        }
        GetCanvasGameObject().transform.Find("ToggleBuildingSelectButton").GetComponent<Button>().onClick.AddListener(onBuildingSelectOpen);
        BuildingController.anyBuildingPositionChanged += ShowActions;
    }

    public void ShowActions() {
        BuildingController.anyBuildingPositionChanged -= ShowActions;

        GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
        GameObject topRightButtons = GetCanvasGameObject().transform.Find("TopRightButtons").gameObject;
        topRightButtons.SetActive(true);
        topRightButtons.transform.Find("settingsButton").gameObject.SetActive(false);
        topRightButtons.transform.Find("ShowTotalMaterials").gameObject.SetActive(false);
        text.GetComponentInChildren<TMP_Text>().text = "Click the top right arrow to open the actions menu.\nFrom there you can select different actions to perform.";
        text.transform.GetChild(1).gameObject.SetActive(false);

        void onActionButtonClick() {
            Destroy(text);
            ShowWhatActionsDo();
            topRightButtons.transform.Find("ActionButtons").Find("CloseMenuButton").GetComponent<Button>().onClick.RemoveListener(onActionButtonClick);
        }

        GetCanvasGameObject().transform.Find("NoBuilding").gameObject.SetActive(true);
        topRightButtons.transform.Find("ActionButtons").Find("CloseMenuButton").GetComponent<Button>().onClick.AddListener(onActionButtonClick);
    }

    public void ShowWhatActionsDo() {
        GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
        text.GetComponentInChildren<TMP_Text>().text = "The Actions you can do are:\n-Place\n-Pick Up\n-Delete\n-Copy\nTry deleting the building you just placed!";
        text.transform.GetChild(1).gameObject.SetActive(false);
        foreach (Transform actionButton in GetCanvasGameObject().transform.Find("TopRightButtons").transform.Find("ActionButtons")) {
            if (actionButton.name != "DeleteButton") actionButton.GetComponent<Button>().interactable = false;
        }

        void onBuildingDeleted() {
            foreach (Transform actionButton in GetCanvasGameObject().transform.Find("TopRightButtons").transform.Find("ActionButtons")) {
                actionButton.GetComponent<Button>().interactable = true;
            }
            Destroy(text);
            BuildingController.anyBuildingPositionChanged -= onBuildingDeleted;
            ShowHowToInteractWithBuildings();
        }


        BuildingController.anyBuildingPositionChanged += onBuildingDeleted;
    }

    public void ShowHowToInteractWithBuildings() {
        BuildingController.anyBuildingPositionChanged -= ShowHowToInteractWithBuildings;
        CameraController.Instance.SetPositionSmooth(GetGridTilemap().CellToWorld(MapController.Instance.GetHousePosition() + new Vector3Int(4, 1, 0)), 1f);
        CameraController.Instance.ToggleCameraLock();
        CameraController.Instance.SetSize(7);
        GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
        text.transform.GetChild(1).gameObject.SetActive(false);
        text.GetComponentInChildren<TMP_Text>().text = "Some buildings can be interacted with.\nRight Click on the House to see what you can do with it!";

        void onBuildingRightClick() {
            Destroy(text);
            InteractableBuildingComponent.BuildingWasRightClicked -= onBuildingRightClick;
            ShowWhatInteractionsDo();
        }

        InteractableBuildingComponent.BuildingWasRightClicked += onBuildingRightClick;
    }

    public void ShowWhatInteractionsDo() {
        GameObject legend = Instantiate(interactionLegend, GameObject.FindGameObjectWithTag("Canvas").transform);
        GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
        text.GetComponentInChildren<TMP_Text>().text = "These are the all available interactions, different buildings have different interactions. Try entering the house!";
        text.transform.GetChild(1).gameObject.SetActive(false);

        Building house = BuildingController.GetBuildings().First(building => building.Base == MapController.Instance.GetHousePosition());
        InteractableBuildingComponent interactableBuildingComponent = house.GetComponent<InteractableBuildingComponent>();
        foreach (Transform interactionButton in interactableBuildingComponent.ButtonParentGameObject.transform) {
            if (interactionButton.name == "ENTER") continue;
            interactionButton.GetComponent<Button>().interactable = false;
        }

        void onBuildingInteraction() {
            Destroy(text);
            Destroy(legend);
            foreach (Transform interactionButton in interactableBuildingComponent.ButtonParentGameObject.transform) {
                interactionButton.GetComponent<Button>().interactable = true;
            }
            CameraController.Instance.ToggleCameraLock();
            BuildingButtonController.anyActionWasClicked -= onBuildingInteraction;
            ShowInteriorInteractions();
        }
        BuildingButtonController.anyActionWasClicked += onBuildingInteraction;
    }

    public void ShowInteriorInteractions() {
        GameObject legend = Instantiate(interactionLegend, GameObject.FindGameObjectWithTag("Canvas").transform);
        GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
        text.GetComponentInChildren<TMP_Text>().text = "Some building interactions can be performed while inside the building. You can see them on the top left corner of your screen. Try exiting the house!";
        text.transform.GetChild(1).gameObject.SetActive(false);
        Building house = BuildingController.GetBuildings().First(building => building.Base == MapController.Instance.GetHousePosition());
        EnterableBuildingComponent enterableBuildingComponent = house.GetComponent<EnterableBuildingComponent>();
        foreach (GameObject interactionButton in enterableBuildingComponent.interiorButtons) {
            if (interactionButton.name == "ENTER") continue;
            interactionButton.GetComponent<Button>().interactable = false;
        }

        void onHouseExit() {
            Destroy(text);
            Destroy(legend);
            BuildingController.isInsideBuilding.Value.interiorButtonClicked -= (type) => { if (type == ButtonTypes.ENTER) onHouseExit(); };
            foreach (GameObject interactionButton in enterableBuildingComponent.interiorButtons) {
                interactionButton.GetComponent<Button>().interactable = true;
            }
            ShowSettingsAndTotalMaterialCost();
        }

        BuildingController.isInsideBuilding.Value.interiorButtonClicked += (type) => { if (type == ButtonTypes.ENTER) onHouseExit(); };
    }

    public void ShowSettingsAndTotalMaterialCost() {
        GameObject topRightButtons = GetCanvasGameObject().transform.Find("TopRightButtons").gameObject;
        topRightButtons.transform.Find("settingsButton").gameObject.SetActive(true);
        topRightButtons.transform.Find("ShowTotalMaterials").gameObject.SetActive(true);

        GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
        text.GetComponentInChildren<TMP_Text>().text = "Lastly you can open the settings or see the total cost of your farm by pressing the buttons on the top right corner.";
        text.GetComponentInChildren<Button>().onClick.AddListener(() => {
            Destroy(text);
            EndOnboardingFlow();
        });
    }

    public void EndOnboardingFlow() {
        StopAllCoroutines();
        CameraController.Instance.UnlockCamera();
        if (!buildingsPlaced[BuildingType.Greenhouse]) BuildingController.PlaceGreenhouse();
        if (!buildingsPlaced[BuildingType.ShippingBin]) BuildingController.PlaceBin();
        if (!buildingsPlaced[BuildingType.House]) BuildingController.PlaceHouse();
        if (!buildingsPlaced[BuildingType.PetBowl]) BuildingController.PlacePetBowl();
        GameObject topRightButtons = GetCanvasGameObject().transform.Find("TopRightButtons").gameObject;
        topRightButtons.SetActive(true);
        foreach (Transform actionButton in GetCanvasGameObject().transform.Find("TopRightButtons").transform.Find("ActionButtons")) {
            actionButton.GetComponent<Button>().interactable = true;
        }
        Building house = BuildingController.GetBuildings().First(building => building.Base == MapController.Instance.GetHousePosition());
        InteractableBuildingComponent interactableBuildingComponent = house.GetComponent<InteractableBuildingComponent>();
        foreach (Transform interactionButton in interactableBuildingComponent.ButtonParentGameObject.transform) {
            interactionButton.GetComponent<Button>().interactable = true;
        }
        EnterableBuildingComponent enterableBuildingComponent = house.GetComponent<EnterableBuildingComponent>();
        foreach (GameObject interactionButton in enterableBuildingComponent.interiorButtons) {
            interactionButton.GetComponent<Button>().interactable = false;
        }
        GetCanvasGameObject().transform.Find("NoBuilding").gameObject.SetActive(true);
        PlayerPrefs.SetInt("HasDoneIntro", 1);
        PlayerPrefs.Save();
        topRightButtons.transform.Find("settingsButton").gameObject.SetActive(true);
        topRightButtons.transform.Find("ShowTotalMaterials").gameObject.SetActive(true);
        GetCanvasGameObject().transform.Find("ToggleBuildingSelectButton").gameObject.SetActive(true);
        InputHandler.Instance.keybindsShouldRegister = true;

        GameObject text = Instantiate(tutorialText, GameObject.FindGameObjectWithTag("Canvas").transform);
        text.GetComponentInChildren<TMP_Text>().text = "That's all! Start building and experimenting with your farm! You can always redo the tutorial through the settings.";
        text.GetComponentInChildren<Button>().onClick.AddListener(() => {
            Destroy(text);
        });
        IsInTutorial = false;
    }

}
