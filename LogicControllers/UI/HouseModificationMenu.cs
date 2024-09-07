using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HouseModificationMenu : MonoBehaviour {

    private Transform Content => transform.Find("TabContent");
    public Action<int> spouseChanged;

    public void Start() {

        var dropdown = Content.Find("Modifications").Find("Marriage").Find("spouse").Find("Dropdown").GetComponent<TMP_Dropdown>();
        dropdown.onValueChanged.AddListener((int value) => {
            spouseChanged?.Invoke(value);
        });

        for (int wallpaperId = 0; wallpaperId < WallsComponent.TotalWallpaperTextures - 1; wallpaperId++) {
            GameObject wallpaperButton = Instantiate(Resources.Load<GameObject>("UI/WallpaperButton"), GetWallpaperContent());
            wallpaperButton.transform.Find("Mask").Find("WallpaperImage").GetComponent<Image>().sprite = WallsComponent.GetWallpaperSprite(wallpaperId);
            int id = wallpaperId;
            wallpaperButton.GetComponent<Button>().onClick.AddListener(() => {
                WallsComponent.SetSelectedWallpaper(id);
                BuildingController.SetCurrentAction(Actions.PLACE_WALLPAPER);
            });
        }

        for (int floorId = 0; floorId < FlooringComponent.TotalFloorTextures - 1; floorId++) {
            GameObject flooringButton = Instantiate(Resources.Load<GameObject>("UI/FlooringButton"), GetFlooringContent());
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

    private Transform GetWallpaperContent() {
        return Content.Find("Wallpaper").Find("Content");
    }

    private Transform GetFlooringContent() {
        return Content.Find("Flooring").Find("Content");
    }


}
