using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleBuildingMenu : MonoBehaviour {
    private Button thisButton;
    private GameObject panel;
    private float moveScale = 3f;
    private Sprite[] arrowButtons = new Sprite[2];
    // Start is called before the first frame update
    void Start() {
        thisButton = gameObject.GetComponent<Button>();
        arrowButtons[0] = Sprite.Create(Resources.Load("UI/ExtendBuildingMenu") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        arrowButtons[1] = Sprite.Create(Resources.Load("UI/HideBuildingMenu") as Texture2D, new Rect(0, 0, 16, 16), new Vector2(0.5f, 0.5f), 16);
        thisButton.GetComponent<Image>().sprite = arrowButtons[0];
        panel = GameObject.FindGameObjectWithTag("Panel");

    }

    public void toggleBuildingMenu() { //Not 0 refrences called from UnityUI
        StartCoroutine(OnClickCour());
    }

    IEnumerator OnClickCour() {
        if (thisButton.GetComponent<Image>().sprite == arrowButtons[0]) thisButton.GetComponent<Image>().sprite = arrowButtons[1];
        else thisButton.GetComponent<Image>().sprite = arrowButtons[0];
        for (int i = 0; i < 80; i++) {
            thisButton.transform.position = new Vector3(thisButton.transform.position.x + moveScale, thisButton.transform.position.y, thisButton.transform.position.z);
            panel.transform.position = new Vector3(panel.transform.position.x + moveScale, panel.transform.position.y, panel.transform.position.z);
            yield return new WaitForSecondsRealtime(0.25f * Time.deltaTime);
        }
        moveScale = -moveScale;
    }
}
