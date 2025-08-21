using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopTemplate : MonoBehaviour
{
    public TMP_Text titleText, cost;
    public Button buyBtn;
    public Image image;
    public ShopItemSO shopItem;


    public void Refresh() {
        cost.text = $"{shopItem.cost} COINS";
        titleText.text = $"{shopItem.label}";
        image.sprite = shopItem.image ;
        if (SaveManager.saveData.wallet >= shopItem.cost) {
            buyBtn.interactable = true;
        }
        else {
            buyBtn.interactable = false;
        }
        bool hasItem=false;
        foreach (ShopItemSO so in SaveManager.saveData.ownedItems) {
            if(shopItem.label == so.label) hasItem = true;

        }
        if (hasItem) {
            cost.text = "OWNED";
            buyBtn.interactable = true;
        }

        if (shopItem.label == SaveManager.saveData.deck.label||
            shopItem.label == SaveManager.saveData.trucks.label ||
            shopItem.label == SaveManager.saveData.wheels.label) {
            cost.text = "USING";
            buyBtn.interactable = false;
        }

    }
    public void BuyItem() {
        ShopManager.Instance.PurchaseItem(shopItem);
    }
}
