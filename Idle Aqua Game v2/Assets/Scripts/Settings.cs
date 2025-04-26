using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Settings : MonoBehaviour
{
    public IdleTutorialGame game;
    public Text notationTypeText;
    /*
    Notation Key:
    0 = sci
    1 = engineering
    2 = Letter (100 =a)
    3+ = N/A
    */

    public void StartSettings()
    {
        UpdateNotationText();
    }

    private void UpdateNotationText()
    {
        var note = game.data.notationType;
        switch(note)
        {
            case 0:
                notationTypeText.text = "Scientific Notation (1.23e4)";
                break;
            case 1:
                notationTypeText.text = "Engineering Notation (12.30e3c)";
                break;
            case 2:
                notationTypeText.text = "Letter Notation (12.30a)";
                break;
            case 3:
                notationTypeText.text = "Word Notation (12.30 Thousand)";
                break;
        }
    }

    public void ChangeNotation()
    {
        var note = game.data.notationType;
        switch(note)
        {
            case 0:
                note = 1;
                break;
            case 1:
                note = 2;
                break;
            case 2:
                note = 3;
                break;
            case 3:
                note = 0;
                break;
        }
        game.data.notationType = note;
        Methods.NotationSettings = note;
        UpdateNotationText();
    }
 
}
