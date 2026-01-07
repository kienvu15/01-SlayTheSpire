using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OutsideShopClickCloser : MonoBehaviour, IPointerClickHandler
{
    public NewShopSystem shopSystem;

    public void OnPointerClick(PointerEventData eventData)
    {
        //// Object mà chuột/tap đang trúng
        //GameObject clickedObj = eventData.pointerCurrentRaycast.gameObject;

        //// Tìm tất cả CardDisplay trong scene
        //List<CardDisplay> cards = shopSystem.spawnedCardDisplays;

        //foreach (var card in cards)
        //{
        //    if (card.descriptionPanel == null) continue;
        //    if (!card.descriptionPanel.activeSelf) continue;

        //    // Nếu click vào chính panel hoặc card → bỏ qua
        //    if (clickedObj != null)
        //    {
        //        if (clickedObj.transform.IsChildOf(card.descriptionPanel.transform)) continue;
        //        if (clickedObj.transform.IsChildOf(card.transform)) continue;
        //    }

        //    // Ngược lại → click ngoài → ẩn
        //    card.HideDescription();
        //}

        //// Tìm tất cả CardDisplay trong scene
        //List<RelicDisplay> relics = shopSystem.spawnedRelicDisplays;

        //foreach (var relic in relics)
        //{
        //    if (relic.descriptionPanel == null) continue;
        //    if (!relic.descriptionPanel.activeSelf) continue;

        //    // Nếu click vào chính panel hoặc card → bỏ qua
        //    if (clickedObj != null)
        //    {
        //        if (clickedObj.transform.IsChildOf(relic.descriptionPanel.transform)) continue;
        //        if (clickedObj.transform.IsChildOf(relic.transform)) continue;
        //    }

        //    // Ngược lại → click ngoài → ẩn
        //    relic.HideDescription();
        //}

        //shopSystem.DeselectCurrentCard();
    }
}