using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//EVERY button should have this so the style is consistent
public class UIElement : TooltipableGameObject, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    AudioSource audioSource;
    public string tooltipMessage = "";
    public override string TooltipMessage => tooltipMessage;

    readonly Vector3 transformScaleChange = new(0.2f, 0.2f, 0.2f);
    Vector3 originalScale;
    public static Actions ActionBeforeEnteringSettings { get; private set; }

    public bool playSounds;
    public bool isCancelButton;
    public bool ExpandOnHover;
    public bool SetActionToNothingOnEnter = true;

    private static AudioClip hoverSound, clickSound, clickSoundCancel;


    public void Start() {
        if (!TryGetComponent(out audioSource)) audioSource = gameObject.AddComponent<AudioSource>();
        originalScale = GetComponent<RectTransform>().localScale;

        if (hoverSound == null) {
            hoverSound = Resources.Load<AudioClip>("SoundEffects/ButtonHover");
            clickSoundCancel = Resources.Load<AudioClip>("SoundEffects/BackButtonSound");
            clickSound = Resources.Load<AudioClip>("SoundEffects/ClickButton");
        }
    }

    public new void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);

        if (SetActionToNothingOnEnter) {
            if (!MoveablePanel.panelWithNoActionRequirementIsOpen) {
                ActionBeforeEnteringSettings = BuildingController.CurrentAction;
                BuildingController.SetCurrentAction(Actions.DO_NOTHING);
            }
        }

        if (ExpandOnHover) StartCoroutine(ObjectMover.ScaleUIObjectInConstantTime(transform, GetComponent<RectTransform>().localScale, originalScale + transformScaleChange, 0.1f));

        if (!playSounds) return;

        audioSource.clip = hoverSound;
        audioSource.Play();
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (SetActionToNothingOnEnter && BuildingController.CurrentAction == Actions.DO_NOTHING) {
            if (!MoveablePanel.panelWithNoActionRequirementIsOpen) BuildingController.SetCurrentAction(ActionBeforeEnteringSettings);
        }


        if (ExpandOnHover) StartCoroutine(ObjectMover.ScaleUIObjectInConstantTime(transform, GetComponent<RectTransform>().localScale, originalScale, 0.1f));
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (playSounds) {
            audioSource.clip = isCancelButton ? clickSoundCancel : clickSound;
            audioSource.Play();
        }
    }
}
