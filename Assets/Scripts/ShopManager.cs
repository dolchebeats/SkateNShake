using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour {

    public static ShopManager Instance;
    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    public TMP_Text walletText;
    public ShopItemSO[] shopItemsSO;

    List<GameObject> shopObjects = new List<GameObject>();

    public Material deck, trucks, wheels, graphic;

    public GameObject shopTemplate;
    public GameObject deckStock;
    public GameObject truckStock;
    public GameObject wheelStock;
    public GameObject graphicStock;


    public void RefreshUI() {
        //StartCoroutine(takeShopImage());

        deckStock.transform.parent.parent.gameObject.SetActive(true);
        truckStock.transform.parent.parent.gameObject.SetActive(true);
        wheelStock.transform.parent.parent.gameObject.SetActive(true);
        graphicStock.transform.parent.parent.gameObject.SetActive(true);

        deck.color = SaveManager.saveData.deck.rgb;
        trucks.color = SaveManager.saveData.trucks.rgb;
        wheels.color = SaveManager.saveData.wheels.rgb;
        if (SaveManager.saveData.graphic.texture != null) {
            graphic.SetTexture("_BaseMap", SaveManager.saveData.graphic.texture);
            graphic.color = Color.white;
        } else {
            graphic.color = new Color(0, 0, 0, 0);
        }
        walletText.text = "SKATESHOP\nCOINS: " + SaveManager.saveData.coins;
        List<GameObject> clonedObjs = new List<GameObject>(shopObjects);
        foreach (GameObject obj in clonedObjs) {
            shopObjects.Remove(obj);
            Destroy(obj);
        }

        for (int i = 0; i < shopItemsSO.Length; i++) {

            GameObject newObj = Instantiate(shopTemplate);
            ShopTemplate template = newObj.GetComponent<ShopTemplate>();
            template.shopItem = shopItemsSO[i];
            if (shopItemsSO[i].type == "deck") {
                
                template.transform.SetParent( deckStock.transform , true );
            }
            else if (shopItemsSO[i].type == "trucks") {
                
                template.transform.SetParent( truckStock.transform , true );                
            }               
            else if (shopItemsSO[i].type == "wheels") { 
                
                 template.transform.SetParent( wheelStock.transform , true );
            } else if (shopItemsSO[i].type == "graphic") {
                template.transform.SetParent(graphicStock.transform, true);
            }
            
            template.Refresh();

            shopObjects.Add(newObj);
            newObj.transform.localScale = Vector3.one;
            
        }
        deckStock.transform.parent.parent.gameObject.SetActive(false);
        truckStock.transform.parent.parent.gameObject.SetActive(false);
        wheelStock.transform.parent.parent.gameObject.SetActive(false);
        graphicStock.transform.parent.parent.gameObject.SetActive(false);

    }

    public void PurchaseItem(ShopItemSO shopItem) {

        bool hasItem = false;
        foreach (ShopItemSO so in SaveManager.saveData.ownedItems) {
            if (shopItem.label == so.label) hasItem = true;

        }

        if (hasItem) {
            if (shopItem.type == "deck") {
                SaveManager.saveData.deck = shopItem;
            } else if (shopItem.type == "trucks") {
                SaveManager.saveData.trucks = shopItem;
            } else if (shopItem.type == "wheels") {
                SaveManager.saveData.wheels = shopItem;
            } else if (shopItem.type == "graphic") {
                SaveManager.saveData.graphic = shopItem;
            }

        } else if (SaveManager.saveData.coins >= shopItem.cost) {
            SaveManager.saveData.coins -= shopItem.cost;
            SaveManager.saveData.ownedItems.Add(shopItem);
            if (shopItem.type == "deck") {
                SaveManager.saveData.deck = shopItem;
            }
            else if (shopItem.type == "trucks") {
                SaveManager.saveData.trucks = shopItem;
            }
            else if (shopItem.type == "wheels") {
                SaveManager.saveData.wheels = shopItem;
            } else if (shopItem.type == "graphic") {
                SaveManager.saveData.graphic = shopItem;
            }
        }
        SaveManager.Save();
        RefreshUI();
    }

    public void InitializeSave(SaveData save) {
        save.ownedItems.Add(shopItemsSO[0]);
        save.deck = shopItemsSO[0];
        save.ownedItems.Add(shopItemsSO[1]);
        save.deck = shopItemsSO[1];
        save.ownedItems.Add(shopItemsSO[2]);
        save.deck = shopItemsSO[2];
    }

    /*public IEnumerator takeShopImage() {
        for (int i = 0; i < shopItemsGFX.Length; i++) {
            yield return new WaitForEndOfFrame();
;
            
            Color rgb = shopItemsGFX[i].rgb;
            GameObject stock = null;

            Texture2D texture = new Texture2D(1024, 1024, TextureFormat.ARGB32, false);
            Rect rect = new Rect(0, 0, 1024, 1024);
            stock = graphicStocKModel;
            GameObject objectToDestroy = Instantiate(stock);
            Camera camera = stock.GetComponentInChildren<Camera>();
            deck.color = rgb;
            graphic.SetTexture("_BaseMap", shopItemsGFX[i].texture);
            graphic.color = Color.white;
            RenderTexture renderTexture = new RenderTexture(1024, 1024, 24);
            camera.targetTexture = renderTexture;
            yield return new WaitForEndOfFrame();
            camera.Render();
            RenderTexture currentRenderTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;
            texture.ReadPixels(rect, 0, 0);
            texture.Apply();
            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(Application.dataPath + "/Screenshots/" + shopItemsGFX[i].name + ".png", bytes);
            camera.targetTexture = null;
            RenderTexture.active = currentRenderTexture;
            Destroy(renderTexture);
            Destroy(objectToDestroy);
            //psprite = Sprite.Create(texture, rect, Vector2.zero);
            //sscounter++;
            yield return new WaitForEndOfFrame();
        }

        //return sprite;
    }

    /*public Sprite TakeShopImage(string type, Color rgb) {
        Sprite sprite=null;
        ptype = type;
        prgb = rgb;
        
        return psprite;
    }*/
}
