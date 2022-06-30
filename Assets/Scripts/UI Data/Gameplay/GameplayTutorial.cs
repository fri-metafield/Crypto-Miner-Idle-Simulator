using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayTutorial : MonoBehaviour
{
    [SerializeField] int tutorialStage;
    [SerializeField] GameObject tutorialObject;
    [SerializeField] GameObject tutorialObjectOutside;

    [SerializeField] List<TutorialDialogue> tutorialSentence = new List<TutorialDialogue>();
    string curSentence;

    //UI
    [SerializeField] Text tutorialDialogueText;
    [SerializeField] GameObject tutorialBGMask;
    [SerializeField] Image tutorialExpresion;

    //tutorial Related
    [SerializeField] GameObject invOpen;
    [SerializeField] GameplayRig rig1;

    //expresion
    [SerializeField] Sprite charNormal;
    [SerializeField] Sprite charHappy;
    [SerializeField] Sprite charDead;
    [SerializeField] Sprite charMoney;

    public static GameplayTutorial instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(tutorialObject.activeSelf == true)
        {
            if (tutorialSentence.Count > 0)
            {

                tutorialDialogueText.text = curSentence;
                

                foreach (TutorialDialogue td in tutorialSentence)
                {
                    if (td.tutorialActivateObject)
                        td.tutorialActivateObject.SetActive(tutorialStage == tutorialSentence.IndexOf(td));

                    

                    if (tutorialStage <= (tutorialSentence.Count - 1))
                        tutorialBGMask.SetActive(tutorialSentence[tutorialStage].useBG);
                    else
                        tutorialBGMask.SetActive(false);
                }
            }

            if (tutorialSentence[tutorialStage].expresion == TutorialExpresion.Normal) tutorialExpresion.sprite = charNormal;
            if (tutorialSentence[tutorialStage].expresion == TutorialExpresion.Happy) tutorialExpresion.sprite = charHappy;
            if (tutorialSentence[tutorialStage].expresion == TutorialExpresion.Dead) tutorialExpresion.sprite = charDead;
            if (tutorialSentence[tutorialStage].expresion == TutorialExpresion.Money) tutorialExpresion.sprite = charMoney;

            /////////////////////TTUTORIAL CONDITION FINDER
            if (tutorialSentence[tutorialStage].tutorialStageName == "RigBuy")
            {
                if (GameUI.instance.allUnlockedRigs >= 1)
                {
                    TutorialNextStage(false);
                }
            }
            if (tutorialSentence[tutorialStage].tutorialStageName == "SlotOpen")
            {
                if (GameUI.instance.selectedSelection == 3)
                {
                    TutorialNextStage(false);
                }
            }
            if (tutorialSentence[tutorialStage].tutorialStageName == "SlotInvOpen")
            {
                if (GameUI.instance.selectedSelection == 3)
                {
                    TutorialNextStage(false);
                }
            }
            if (tutorialSentence[tutorialStage].tutorialStageName == "SlotAssign")
            {
                if (rig1.rigSlots2[0].gpuSeries)
                {
                    TutorialNextStage(false);
                }
            }
            if (tutorialSentence[tutorialStage].tutorialStageName == "SlotClose")
            {
                if (GameUI.instance.selectedSelection == 1)
                {
                    TutorialNextStage(false);
                }
            }
            if (tutorialSentence[tutorialStage].tutorialStageName == "Overclock")
            {
                if (rig1.isOverclocking)
                {
                    TutorialNextStage(false);
                }
            }
            if (tutorialSentence[tutorialStage].tutorialStageName == "ExchangeOpen")
            {
                if (GameUI.instance.selectedPage == 3)
                {
                    TutorialNextStage(false);
                }
            }
            if (tutorialSentence[tutorialStage].tutorialStageName == "ExchangeSell")
            {
                if (PlayerData.player_Money > 0)
                {
                    TutorialNextStage(false);
                }


            }
        }
        

    }
    public void StartTutorial()
    {
        tutorialObject.SetActive(true);
        tutorialObjectOutside.SetActive(true);
        InitiateDialogue();
    }

    [ContextMenu("Next Stage")]
    public void TutorialNextStage(bool clicked)
    {
        if(clicked)
        {
            if (tutorialSentence[tutorialStage].tutorialActivateObject)
                return;

        }

        tutorialStage++;
        if(tutorialStage >= tutorialSentence.Count)
        {
            FinishTutorial();
            return;
        }
        else
        {
            InitiateDialogue();
        }
            
    }

    void InitiateDialogue()
    {
        curSentence = "";
        curSentence = tutorialSentence[tutorialStage].tutorialText;
    }

    void FinishTutorial()
    {
        curSentence = null;
        tutorialObject.SetActive(false);
        tutorialObjectOutside.SetActive(false);
    }

    //button
    public void ButtonConfirmTutorial(bool confirm)
    {
        if(confirm)
        {
            TutorialNextStage(false);
        }
        else
        {
            FinishTutorial();
        }
    }

    

}

[System.Serializable]
public class TutorialDialogue
{
    public string tutorialStageName;
    public TutorialExpresion expresion;
    [TextArea(2,10)]
    public string tutorialText;
    public GameObject tutorialActivateObject;
    public bool useBG;
}

public enum TutorialExpresion
{
    Normal,
    Happy,
    Dead,
    Money
}