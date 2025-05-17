using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class MinimapDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Tooltip("“от RectTransform, который будет двигатьс€")]
    public RectTransform mapArea;

    private Vector2 lastPointerPosition;

    public void OnBeginDrag(PointerEventData eventData)
    {
        Debug.Log("[MinimapDrag] OnBeginDrag called. Pointer at " + eventData.position);
        lastPointerPosition = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Debug.Log("[MinimapDrag] OnDrag called. delta = " + (eventData.position - lastPointerPosition));
        Vector2 currentPointerPosition = eventData.position;
        Vector2 delta = currentPointerPosition - lastPointerPosition;
        mapArea.anchoredPosition += delta;
        mapArea.anchoredPosition = ClampMapPosition(mapArea.anchoredPosition);
        lastPointerPosition = currentPointerPosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("[MinimapDrag] OnEndDrag called.");
    // ничего дополнительно
    }

    private Vector2 ClampMapPosition(Vector2 targetPos, float overshoot = 50f)
    {
        RectTransform panelRect = GetComponent<RectTransform>();
        Vector2 mapSize = mapArea.rect.size * mapArea.localScale.x;
        Vector2 panelSize = panelRect.rect.size;

        float halfMapW = mapSize.x * .5f, halfMapH = mapSize.y * .5f;
        float halfPanW = panelSize.x * .5f, halfPanH = panelSize.y * .5f;

        float minX = -halfMapW + halfPanW - overshoot;
        float maxX = halfMapW - halfPanW + overshoot;
        float minY = -halfMapH + halfPanH - overshoot;
        float maxY = halfMapH - halfPanH + overshoot;

        if (mapSize.x <= panelSize.x)
        {
            minX = -overshoot;
            maxX = overshoot;
        }

        if (mapSize.y <= panelSize.y)
        {
            minY = -overshoot;
            maxY = overshoot;
        }


        return new Vector2(
            Mathf.Clamp(targetPos.x, minX, maxX),
            Mathf.Clamp(targetPos.y, minY, maxY)
        );
    }
}
