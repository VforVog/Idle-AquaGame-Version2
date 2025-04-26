using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;



public class Methods : MonoBehaviour
{
    public static int NotationSettings;
    //TODO afto einai axristo kai den xrisimopoieitai pleon opote delete sto telos tou tutorial
    public static void CanvasGroupChanger(bool appear, CanvasGroup canvasToChange)
    {
        canvasToChange.alpha = appear ? 1 : 0;
        canvasToChange.interactable = appear;
        canvasToChange.blocksRaycasts = appear;
    }

    //Episode 15
    //Prota vazeis to bar.fillAmount pou thes na gemizei smooth, meta to current poso tou gemismatos kai meta to poso pou otan ftasei tha einai max
    public static float SmoothLoadingBar(float currentLoadingBarPosition, double currentNumber, double maxNumber)
    {
        double finalLoadingBarPosition = currentNumber / maxNumber;
        if (finalLoadingBarPosition > 1)
        {
            finalLoadingBarPosition = 1;
        }
        if (currentLoadingBarPosition - 0.01 > finalLoadingBarPosition)
        {
            //an i bara einai toulaxiston 0.01 (1%) pano apoti thaprepe katevenei sto percentage pou thaprepe na einai (giafto -)
            //to noumero 10 einai afto pou allazei ton rithmo pou i bara paei stin pragmatiki tis thesi mikrotero noumer( mexri 1) to kanei pio "apotomo"
            return currentLoadingBarPosition - (float)((currentLoadingBarPosition - finalLoadingBarPosition) / 10 + 0.1 * Time.deltaTime);
        }
        else if (currentLoadingBarPosition < finalLoadingBarPosition)
        {
            //an i bara einai kato apo ekei pou tha eprepe anevenei ekei pou tha eprepe na einai (giafto +)
            return currentLoadingBarPosition + (float)((finalLoadingBarPosition - currentLoadingBarPosition) / 10 + 0.1 * Time.deltaTime);
        }
        else
        {
            return currentLoadingBarPosition;
        }
    }

  public static string NotationMethod(double x, string format)
    {
        if (x <= 1000) return x.ToString(format);
        switch (NotationSettings)
        {
            case 0:
            {
                var exponent = 3 * Math.Floor(Math.Log10(Math.Abs(x)));
                var mantissa = x / Math.Pow(10, exponent);
                // gets the number, ex, 1.32e3, 1.32 is mantissa.
                return mantissa.ToString("F2") + "e" + exponent;
            }
            case 1:
            {
                var exponent = 3 *Math.Floor(Math.Floor(Math.Log10(x)) / 3);
                var mantissa = x / Math.Pow(10, exponent);
                return mantissa.ToString("F2") + "e" + exponent;
            }
            case 2:
            {
                var exponentSci =  Math.Floor(Math.Log10(x));
                var exponentEng = 3 * Math.Floor(exponentSci / 3);
                var letterOne = ((char)Math.Floor(((double)exponentEng - 3 * 26) / 3 * 26 % 26 + 97)).ToString();
                if((double) exponentEng / 3 >= 27)
                {
                    var letterTwo = ((char)(Math.Floor(((double)exponentEng - 3 * 26) / (3 * 26)) % 26 + 97)).ToString();
                    return (x / Math.Pow(10, exponentEng)).ToString("F2") + letterOne;
                }
                if(x > 100)
                    return (x / Math.Pow(10, exponentEng)).ToString("F2") + letterOne;
                return x.ToString("F2");
            }
            case 3:
            {
                var exponent = Math.Floor(Math.Log10(x));
                var thirdExponent = 3 * Math.Floor(exponent / 3);
                var mantissa = x / Math.Pow(10, thirdExponent);

                if (x <= 1000) return x.ToString("F2");
                if(x >= 1e75 ) return mantissa.ToString("F2") + "e" + thirdExponent; // If 1e75 or more, do engineering notation
                return mantissa.ToString("F2") + prefixes[(int)thirdExponent]; //Otheriwse do word notation
                  
            }
        }

        return "";
        
    }

    private static readonly Dictionary<int, string> prefixes = new Dictionary<int, string>
    {
                    {3, " Thousand"},
                    {6, " Million"},
                    {9, " Billion"},
                    {12, " Trillion"},
                    {15, " Quadrillion"},
                    {18, " Quintillion"},
                    {21, " Sextillion"},
                    {24, " Septillion"},
                    {27, " Octillion"},
                    {30, " Nonillion"},
                    {33, " Decillion"},
                    {36, " Undecillion"},
                    {39, " Duodecillion"},
                    {42, " Tredecillion"},
                    {45, " Quattuordecillion"},
                    {48, " quindecillion"},
                    {51, " sexdecillion"},
                    {54, " septendecillion"},
                    {57, " octodecillion"},
                    {60, " novemdecillion"},
                    {63, " vigintillion"},
                    {66, " unvigintillion"},
                    {69, " duovigintillion"},
                    {72, " trevigintillion"},
                    {75, " quattuorvigintillion"},
             
    };

public static void BuyMax(ref double c, double b, float r, ref int k)   
    {
        var n = Math.Floor(Math.Log((c *(r - 1)) / (b * Math.Pow(r, k)) + 1,  r));        
        var cost = b * (Math.Pow(r, k) * (Math.Pow(r, n) - 1) / (r - 1));
        
        if (c >= cost)
        {
            k += (int)n;
            c -= cost;
        }
    }

    public static void BuyMaxLimit(ref double c, double b, float r, ref int k, int limit)   
    {
        var n = Math.Floor(Math.Log((c *(r - 1)) / (b * Math.Pow(r, k)) + 1,  r));   
        if (n + k > limit) n  -= k;  
        var cost = b * (Math.Pow(r, k) * (Math.Pow(r, n) - 1) / (r - 1));
        
        if (c >= cost) return;
        k += (int)n;
        c -= cost;
    }

}
