using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//EVERY button should have this so the style is consistent
public class UIElement : TooltipableGameObject, IPointerEnterHandler, IPointerExitHandler {
    AudioSource audioSource;
    public string tooltipMessage = "";
    public override string TooltipMessage => tooltipMessage;

    readonly Vector3 transformScaleChange = new(0.2f, 0.2f, 0.2f);
    Vector3 originalScale;
    public static Actions ActionBeforeEnteringSettings { get; private set; }

    [SerializeField] private bool playSounds;
    [SerializeField] private bool ExpandOnHover;
    [SerializeField] private bool SetActionToNothingOnEnter = true;

    public override void OnAwake() {
        if (!TryGetComponent(out audioSource)) audioSource = gameObject.AddComponent<AudioSource>();
        originalScale = GetComponent<RectTransform>().localScale;
    }

    public new void OnPointerEnter(PointerEventData eventData) {
        base.OnPointerEnter(eventData);

        if (SetActionToNothingOnEnter && BuildingController.CurrentAction != Actions.DO_NOTHING) {
            ActionBeforeEnteringSettings = BuildingController.CurrentAction;
            BuildingController.SetCurrentAction(Actions.DO_NOTHING);
        }

        if (ExpandOnHover) StartCoroutine(UIObjectMover.ScaleObjectInConstantTime(transform, GetComponent<RectTransform>().localScale, originalScale + transformScaleChange, 0.1f));

        if (!playSounds) return;

        AudioClip hoverSound = Resources.Load<AudioClip>("SoundEffects/ButtonHover");
        audioSource.clip = hoverSound;
        audioSource.Play();
    }

    public new void OnPointerExit(PointerEventData eventData) {
        base.OnPointerExit(eventData);

        if (SetActionToNothingOnEnter && (BuildingController.CurrentAction == Actions.DO_NOTHING)) {
            BuildingController.SetCurrentAction(ActionBeforeEnteringSettings);
        }


        if (ExpandOnHover) StartCoroutine(UIObjectMover.ScaleObjectInConstantTime(transform, GetComponent<RectTransform>().localScale, originalScale, 0.1f));
    }

    public override void OnUpdate() { }
}
