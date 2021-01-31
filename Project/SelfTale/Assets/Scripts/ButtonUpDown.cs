using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine;




[RequireComponent(typeof(UnityEngine.UI.Button))]
public class ButtonUpDown : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public UnityEvent onPointerDown;
    public UnityEvent onPointerUp;

    public UnityEvent whilePointerPressed;

    private UnityEngine.UI.Button _button;

    private void Awake()
    {
        _button = GetComponent<UnityEngine.UI.Button>();
    }

    private IEnumerator WhilePressed()
    {
        while (true)
        {
            whilePointerPressed?.Invoke();
            yield return null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {

        if (!_button.interactable) return;


        StopAllCoroutines();
        StartCoroutine(WhilePressed());

        onPointerDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        StopAllCoroutines();
        onPointerUp?.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        onPointerUp?.Invoke();
    }
}