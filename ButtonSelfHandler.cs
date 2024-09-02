using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//EVERY button should have this so the style is consistent
public class ButtonSelfHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    Button button;
    EventTrigger eventTrigger;
    AudioSource audioSource;

    readonly Vector3 transformScaleChange = new(0.2f, 0.2f, 0.2f);
    Vector3 originalScale;

    [SerializeField] private bool playHoverSound = true;

    void Start() {
        if (!TryGetComponent(out button)) button = gameObject.AddComponent<Button>();
        // if (!TryGetComponent(out eventTrigger)) eventTrigger = gameObject.AddComponent<EventTrigger>();
        if (!TryGetComponent(out audioSource)) audioSource = gameObject.AddComponent<AudioSource>();
        originalScale = GetComponent<RectTransform>().localScale;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        StartCoroutine(UIObjectMover.ScaleObjectInConstantTime(transform, GetComponent<RectTransform>().localScale, originalScale + transformScaleChange, 0.1f));

        if (!playHoverSound) return;

        AudioClip hoverSound = Resources.Load<AudioClip>("SoundEffects/ButtonHover");
        audioSource.clip = hoverSound;
        audioSource.Play();
    }

    public void OnPointerExit(PointerEventData eventData) {
        StartCoroutine(UIObjectMover.ScaleObjectInConstantTime(transform, GetComponent<RectTransform>().localScale, originalScale, 0.1f));
    }
}
