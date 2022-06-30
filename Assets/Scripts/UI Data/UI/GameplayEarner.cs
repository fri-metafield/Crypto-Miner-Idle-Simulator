using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayEarner : MonoBehaviour
{
    [SerializeField] GameObject activateObject;

    public List<EarnObject> curEarning = new List<EarnObject>();
    int count = 0;

    //UI
    [SerializeField] Text itemName;
    [SerializeField] Image itemImage;

    public static GameplayEarner instance;
    private void Awake()
    {
        instance = this;
    }


    // Update is called once per frame
    void Update()
    {
        if(curEarning.Count > 0)
        {
            itemName.text = curEarning[count].earnName;
            itemImage.sprite = curEarning[count].earnImage;
        }
    }

    public void EarnItem(string name, Sprite image)
    {
        var eo = new EarnObject();
        eo.earnImage = image;
        eo.earnName = name;

        curEarning.Add(eo);

        activateObject.SetActive(true);
        count = 0;

        GameManager.instance.PlaySound(GameManager.instance.sfxPopup, false);
    }

    public void CloseButton()
    {
        if(count >= curEarning.Count-1)
        {
            activateObject.SetActive(false);
            curEarning.Clear();
            count = 0;
        }
        else
        {
            count++;
        }
        
    }
}

[System.Serializable]
public class EarnObject
{
    public string earnName;
    public Sprite earnImage;
}
