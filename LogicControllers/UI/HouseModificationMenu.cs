using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HouseModificationMenu : MonoBehaviour {

    private Transform Content => transform.Find("TabContent");
    public Action<int> spouseChanged;
    [SerializeField] private GameObject wallpaperButtonPrefab;
    [SerializeField] private GameObject flooringButtonPrefab;

    public void Start() {

        var dropdown = Content.Find("Modifications").Find("Marriage").Find("spouse").Find("Dropdown").GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener((int value) => {
            spouseChanged?.Invoke(value);
        });

        for (int wallpaperId = 0; wallpaperId < WallsComponent.TotalWallpaperTextures - 1; wallpaperId++) {
            GameObject wallpaperButton = Instantiate(wallpaperButtonPrefab, GetWallpaperContent());
            wallpaperButton.transform.Find("Mask").Find("WallpaperImage").GetComponent<Image>().sprite = WallsComponent.GetWallpaperSprite(wallpaperId);
            int id = wallpaperId;
            wallpaperButton.GetComponent<Button>().onClick.AddListener(() => {
                WallsComponent.SetSelectedWallpaper(id);
                BuildingController.SetCurrentAction(Actions.PLACE_WALLPAPER);
            });
        }

        for (int floorId = 0; floorId < FlooringComponent.TotalFloorTextures - 1; floorId++) {
            GameObject flooringButton = Instantiate(flooringButtonPrefab, GetFlooringContent());
            flooringButton.transform.Find("Mask").Find("FlooringImage").GetComponent<Image>().sprite = FlooringComponent.GetFloorSprite(floorId);
            int id = floorId;
            flooringButton.GetComponent<Button>().onClick.AddListener(() => {
                FlooringComponent.SetSelectedFloor(id);
                BuildingController.SetCurrentAction(Actions.PLACE_FLOORING);
            });
        }
    }

    public void SetSpouseDropdownInteractability(bool interactable) {
        Content.Find("Modifications").Find("Marriage").Find("spouse").Find("Dropdown").GetComponent<TMP_Dropdown>().interactable = interactable;
    }

    public Toggle GetMarriageToggle() {
        return Content.Find("Modifications").Find("Marriage").Find("Checkbox").GetComponent<Toggle>();
    }

    public Toggle GetCornerRoomToggle() {
        return Content.Find("Modifications").Find("Renovations").Find("CornerRoom").GetComponent<Toggle>();
    }

    public Toggle GetAtticToggle() {
        return Content.Find("Modifications").Find("Renovations").Find("Attic").GetComponent<Toggle>();
    }

    public Toggle GetModificationToggle(HouseExtensionsComponent.HouseModifications modificationName) {
        return Content.Find("Modifications").Find("Renovations").Find(modificationName.ToString()).GetComponent<Toggle>();
    }

    public void SetAllToglesToOff() {
        Sprite offSprite = Resources.Load<Sprite>("UI/CheckBoxOff");
        foreach (Transform toggle in Content.Find("Modifications").Find("Renovations")) {
            if (toggle.TryGetComponent<Toggle>(out _)) toggle.Find("Button").GetComponent<Image>().sprite = offSprite;
        }
        Resources.UnloadAsset(offSprite);
    }

    private Transform GetWallpaperContent() {
        return Content.Find("Wallpaper").Find("Content");
    }

    private Transform GetFlooringContent() {
        return Content.Find("Flooring").Find("Content");
    }


}
