using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameUIMover : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Vector3 curPos;
    [SerializeField] Vector3 page1Pos;
    [SerializeField] Vector3 page2Pos;
    [SerializeField] Vector3 page3Pos;
    [SerializeField] Vector3 page4Pos;

    [SerializeField] string moveType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.DOLocalMove(curPos, moveSpeed);

        if(moveType == "Page")
        {
            if (GameUI.instance.selectedPage == 1) curPos = page1Pos;
            else if (GameUI.instance.selectedPage == 2) curPos = page2Pos;
            else if (GameUI.instance.selectedPage == 3) curPos = page3Pos;
            else if (GameUI.instance.selectedPage == 4) curPos = page4Pos;
        }
        else if (moveType == "Area")
        {
            if (GameUI.instance.selectedArea == 1) curPos = page1Pos;
            else if (GameUI.instance.selectedArea == 2) curPos = page2Pos;
            else if (GameUI.instance.selectedArea == 3) curPos = page3Pos;
        }
        else if (moveType == "Selection")
        {
            if (GameUI.instance.selectedSelection == 1) curPos = page1Pos;
            else if (GameUI.instance.selectedSelection == 2) curPos = page2Pos;
            else if (GameUI.instance.selectedSelection == 3) curPos = page3Pos;
        }
        else if (moveType == "Market")
        {
            if (GameUI.instance.selectedMarket == 1) curPos = page1Pos;
            else if (GameUI.instance.selectedMarket == 2) curPos = page2Pos;
            else if (GameUI.instance.selectedMarket == 3) curPos = page3Pos;
        }

    }
}
