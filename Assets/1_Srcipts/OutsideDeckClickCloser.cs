using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutsideDeckClickCloser : MonoBehaviour, IPointerClickHandler
{
    public NewShopSystem newShopSystem;
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked! Target: " + eventData.pointerCurrentRaycast.gameObject);
        GameObject clickedObj = eventData.pointerCurrentRaycast.gameObject;
        DescriptionPanelManager.Instance.CloseEverythingOutside(clickedObj);
        newShopSystem.DeselectCurrentCard();
    }
}