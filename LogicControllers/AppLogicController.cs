using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Utility.BuildingManager;

public class AppLogicController : MonoBehaviour {

    private readonly float LOGO_TOP_SIZE_SCALE = 75;
    private readonly float LOGO_BOTTOM_MOVE_SCALE = 100;
    public float BUTTON_SCALE_MODIFIER = 50;
    private AudioSource audioSource;
    public void Start() {
        if (!TryGetComponent(out audioSource)) audioSource = gameObject.AddComponent<AudioSource>();
        StartCoroutine(ShowLogo());
        GameObject buttonsParent = GameObject.Find("Buttons");
        for (int i = 0; i < buttonsParent.transform.childCount; i++) {
            GameObject button = buttonsParent.transform.GetChild(i).gameObject;
            button.SetActive(false);
        }
        Cursor.SetCursor(Resources.Load("UI/Cursor") as Texture2D, Vector2.zero, CursorMode.ForceSoftware);
    }

    public void PlayerPressedCreateFarmButton() {
        SceneManager.LoadScene("PermanentScene");
    }

    public void PlayerPressedLoadFarmButton() {
        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("PermanentScene");
        if (BuildingSaverLoader.Instance.LoadFromFile()) {
            Destroy(gameObject);
        }
        else {
            SceneManager.LoadScene("MainMenu");
        }
    }

    private IEnumerator ShowLogo() {
        GameObject logoTop = GameObject.Find("LogoTop");
        GameObject logoBottom = GameObject.Find("LogoBottom");

        logoTop.SetActive(false);
        logoBottom.SetActive(false);
        logoBottom.GetComponent<RectTransform>().localPosition = new Vector3(-350, 250, 0);
        logoTop.GetComponent<RectTransform>().localPosition = new Vector3(-350, 250, 0);
        logoTop.GetComponent<RectTransform>().localScale = new Vector3(5, 5, 5);
        yield return new WaitForSeconds(0.5f);
        logoTop.SetActive(true);
        while (logoTop.GetComponent<RectTransform>().localScale.magnitude > new Vector3(1.1f, 1.1f, 1.1f).magnitude) {
            logoTop.GetComponent<RectTransform>().localScale -= LOGO_TOP_SIZE_SCALE * Time.deltaTime * new Vector3(0.1f, 0.1f, 0.1f);
            yield return null;
        }
        logoTop.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
        logoBottom.SetActive(true);
        while (logoBottom.GetComponent<RectTransform>().localPosition.y > -190) {
            logoBottom.GetComponent<RectTransform>().localPosition -= LOGO_BOTTOM_MOVE_SCALE * Time.deltaTime * new Vector3(0, 5, 0);
            yield return null;
        }
        logoBottom.GetComponent<RectTransform>().localPosition = new Vector3(-350, -190, 0);
        StartCoroutine(ShowButtons());
    }

    private IEnumerator ShowButtons() {
        GameObject buttonsParent = GameObject.Find("Buttons");
        float delay = 0.5f;
        StartCoroutine(ShowButtonAnimation(buttonsParent.transform.GetChild(0).gameObject));
        yield return new WaitForSeconds(delay);
        StartCoroutine(ShowButtonAnimation(buttonsParent.transform.GetChild(1).gameObject));
        yield return new WaitForSeconds(delay);
        StartCoroutine(ShowButtonAnimation(buttonsParent.transform.GetChild(2).gameObject));
    }

    private IEnumerator ShowButtonAnimation(GameObject button) {
        AudioClip hoverSound = Resources.Load<AudioClip>("SoundEffects/ButtonAppear");
        audioSource.clip = hoverSound;
        audioSource.Play();

        button.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
        button.SetActive(true);
        // while (button.GetComponent<RectTransform>().localScale.magnitude < new Vector3(1.2f, 1.2f, 1.2f).magnitude) {
        //     button.GetComponent<RectTransform>().localScale += BUTTON_SCALE_MODIFIER * Time.deltaTime * new Vector3(1, 1, 1);
        //     yield return null;
        // }
        // button.GetComponent<RectTransform>().localScale = new Vector3(1.2f, 1.2f, 1.2f);
        // while (button.GetComponent<RectTransform>().localScale.magnitude > new Vector3(1, 1, 1).magnitude) {
        //     button.GetComponent<RectTransform>().localScale -= BUTTON_SCALE_MODIFIER * Time.deltaTime * new Vector3(1, 1, 1);
        //     yield return null;
        // }
        button.GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);
        yield return null;
    }
    public void Quit() {
        GameObject quitConfirmPanel = GameObject.FindGameObjectWithTag("QuitConfirm");
        quitConfirmPanel.GetComponent<MoveablePanel>().SetPanelToOpenPosition();
    }

    public void QuitApp() {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
    }


}