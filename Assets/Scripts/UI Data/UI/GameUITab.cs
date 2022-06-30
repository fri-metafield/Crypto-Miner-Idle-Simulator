using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameUITab : MonoBehaviour
{
    Sprite defaultSprite;

    [SerializeField] Sprite UsedSprite;

    [SerializeField] string spriteType;
    [SerializeField] GameObject mainObject;

    // Start is called before the first frame update
    void Start()
    {
        defaultSprite = GetComponent<Image>().sprite;
    }

    // Update is called once per frame
    void Update()
    {
        if(spriteType == "Page1")
        {
            if(GameUI.instance.selectedPage == 1)
                GetComponent<Image>().sprite = UsedSprite;
            else
                GetComponent<Image>().sprite = defaultSprite;
        }
        if (spriteType == "Page2")
        {
            if (GameUI.instance.selectedPage == 2)
                GetComponent<Image>().sprite = UsedSprite;
            else
                GetComponent<Image>().sprite = defaultSprite;
        }
        if (spriteType == "Page3")
        {
            if (GameUI.instance.selectedPage == 3)
                GetComponent<Image>().sprite = UsedSprite;
            else
                GetComponent<Image>().sprite = defaultSprite;
        }
        if (spriteType == "Page4")
        {
            if (GameUI.instance.selectedPage == 4)
                GetComponent<Image>().sprite = UsedSprite;
            else
                GetComponent<Image>().sprite = defaultSprite;
        }

        if (spriteType == "Area1")
        {
            if (GameUI.instance.selectedArea == 1)
                GetComponent<Image>().sprite = UsedSprite;
            else
                GetComponent<Image>().sprite = defaultSprite;
        }
        if (spriteType == "Area2")
        {
            if (GameUI.instance.selectedArea == 2)
                GetComponent<Image>().sprite = UsedSprite;
            else
                GetComponent<Image>().sprite = defaultSprite;
        }
        if (spriteType == "Area3")
        {
            if (GameUI.instance.selectedArea == 3)
                GetComponent<Image>().sprite = UsedSprite;
            else
                GetComponent<Image>().sprite = defaultSprite;
        }

        if (spriteType == "Sell1")
        {
            if (GameUI.instance.selectedMarket == 1)
                GetComponent<Image>().sprite = UsedSprite;
            else
                GetComponent<Image>().sprite = defaultSprite;
        }
        if (spriteType == "Sell2")
        {
            if (GameUI.instance.selectedMarket == 2)
                GetComponent<Image>().sprite = UsedSprite;
            else
                GetComponent<Image>().sprite = defaultSprite;
        }
        if (spriteType == "Sell3")
        {
            if (GameUI.instance.selectedMarket == 3)
                GetComponent<Image>().sprite = UsedSprite;
            else
                GetComponent<Image>().sprite = defaultSprite;
        }
    }
}
