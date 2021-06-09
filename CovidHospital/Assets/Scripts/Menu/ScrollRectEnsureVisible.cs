using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectEnsureVisible : MonoBehaviour
{
    private RectTransform contentPanel;
    private GameObject lastSelected;
    private RectTransform scrollRectTransform;
    private RectTransform selectedRectTransform;

    private void Start()
    {
        scrollRectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        //just incase content panel gets created in start.
        if (contentPanel == null) contentPanel = GetComponent<ScrollRect>().content;

        var selected = EventSystem.current.currentSelectedGameObject;

        if (selected == null) return;
        if (selected.transform.parent != contentPanel.transform) return;
        if (selected == lastSelected) return;

        selectedRectTransform = selected.GetComponent<RectTransform>();
        contentPanel.anchoredPosition = new Vector2(contentPanel.anchoredPosition.x,
            -selectedRectTransform.localPosition.y - selectedRectTransform.rect.height / 2);

        lastSelected = selected;
    }
}