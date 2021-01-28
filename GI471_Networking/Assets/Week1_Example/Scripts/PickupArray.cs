using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupArray : MonoBehaviour
{
    public Text textDisplay;
    public InputField inputSelectionText;
    public Text textResult;
    public Button enterFind;
    
    private List<string> textList = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        textList.Add("Unity");
        textList.Add("Unreal");
        textList.Add("ResidentEvil");
        textList.Add("Google");
        textList.Add("MongoDB");

        enterFind.onClick.AddListener(OnEnterFind);

        for(int i = 0; i < textList.Count; i++)
        {
            textDisplay.text += textList[i] + "\n";
        }
    }

    void OnEnterFind()
    {
        bool isFoundText = false;

        for(int i = 0; i < textList.Count; i++)
        {
            if(textList[i] == inputSelectionText.text)
            {
                isFoundText = true;
                break;
            }
        }

        if(isFoundText)
        {
            textResult.text = $"[ <color=green>{inputSelectionText.text}</color> ] is found.";
        }
        else
        {
            textResult.text = $"[ <color=red>{inputSelectionText.text}</color> ] is not found.";
        }
    }

    
}
