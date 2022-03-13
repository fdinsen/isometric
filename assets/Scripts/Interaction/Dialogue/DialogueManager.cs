using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Ink.Runtime;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;


    private Story currentStory;
    private PlayerInput input;

    public bool DialogueIsPlaying { get; private set; }
    public static DialogueManager Instance { get; private set; }
    public delegate void DialogueEvent();
    public event DialogueEvent DialogueStarted;
    public event DialogueEvent DialogueEnded;
    
    void Awake()
    {
        if(Instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene. ");
        }
        Instance = this;
        
        input = new PlayerInput();
        input.Interaction.Enable();
        input.Interaction.Confirm.performed += ctx => { 
            if (currentStory?.currentChoices.Count > 0) return; //skip if currently selecting a choice!
            ContinueStory(); 
        };
    }

    void Start()
    {
        DialogueIsPlaying = false;
        dialoguePanel.SetActive(false);

        // get all of the choices text
        choicesText = new TextMeshProUGUI[choices.Length];
        int i = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[i] = choice.GetComponentInChildren<TextMeshProUGUI>();
            i++;
        }
    }

    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text);
        DialogueIsPlaying = true;
        dialoguePanel.SetActive(true);
        DialogueStarted.Invoke();
        ContinueStory();
    }

    private void ExitDialogueMode()
    {
        DialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        DialogueEnded.Invoke();
    }

    private void ContinueStory()
    {
        if (!DialogueIsPlaying) { return; };
        if (currentStory.canContinue)
        {
            // set text for the current dialogue line
            var story = currentStory.Continue();
            if (story.Trim().Equals("")) ExitDialogueMode();
            dialogueText.text = story;
            
            // display choices, if any, for this dialogue line
            DisplayChoices();
            // handle tags
            HandleTags(currentStory.currentTags);
        }
        else
        {
            ExitDialogueMode();
        }
    }

    public void HandleTags(List<string> currentTags)
    {
        //Loop through each tag for dialogue line
        foreach (string tag in currentTags)
        {
            // parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be properly parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();
            
            // handle the tag
            switch(tagKey)
            {
                case DiaTags.SPEAKER_TAG:
                    Debug.Log("speaker=" + tagValue);
                    break;
                case DiaTags.PORTRAIT_TAG:
                    Debug.Log("portrait=" + tagValue);
                    break;
                case DiaTags.LAYOUT_TAG:
                    Debug.Log("layout=" + tagValue);
                    break;
                case DiaTags.HEAL_TAG:
                    int percentHeal = int.Parse(tagValue) / 100;
                    var p = GameObject.FindGameObjectWithTag("Player");
                    if(p) { 
                        var phealth = p.GetComponent<PlayerHealth>();
                        if (phealth) { phealth.Heal(percentHeal * phealth.MaxHealth); }
                    }
                    break;
                case DiaTags.AMMO_TAG:
                    Debug.Log("ammo=" + tagValue);
                    break;
                case DiaTags.WEAPON_TAG:
                    var pl = GameObject.FindGameObjectWithTag("Player");
                    if(pl)
                    {
                        var swapper = pl.GetComponent<WeaponSwapHandler>();
                        if(swapper) { swapper.SwapWeaponSlot1(tagValue); }
                    }
                    break;
                default:
                    Debug.LogWarning("Tag came in but is not currently being handled: " + tag);
                    break;
            }
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if(currentChoices.Count > choices.Length) //Make sure that there aren't more choices than UI supports!
            Debug.LogError("More choices were given than the UI can support. Number of choices given: " + currentChoices.Count);

        int index = 0;
        foreach(Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        // Event system requires that we clear it first, then wait
        // for at least a frame before setting the current selected object.
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }
}

public static class DiaTags
{
    public const string SPEAKER_TAG = "speaker";
    public const string PORTRAIT_TAG = "portrait";
    public const string LAYOUT_TAG = "layout";
    public const string HEAL_TAG = "heal";
    public const string AMMO_TAG = "ammo";
    public const string WEAPON_TAG = "weapon";
}
