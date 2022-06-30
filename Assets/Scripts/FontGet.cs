using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontGet : MonoBehaviour
{
    [SerializeField] bool secondFont;
    // Start is called before the first frame update
    void Start()
    {
        if (secondFont) { GetComponent<Text>().font = GameManager.instance.gameFont2; return; }
        GetComponent<Text>().font = GameManager.instance.gameFont;
    }

}
