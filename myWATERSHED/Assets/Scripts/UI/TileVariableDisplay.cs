using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileVariableDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private Image icon;
    [SerializeField] private Slider slider;

    [SerializeField] private Image sliderFill;

    public VariableClass variableClassToRead;

    public void SetVariableClass(VariableClass varClass)
    {
        variableClassToRead = varClass;
    }

    private void Update()
    {
        if (variableClassToRead != null)
        {
            if (variableClassToRead.wholeNumbers)
            {
                nameText.text = variableClassToRead.variableName + " : " + variableClassToRead.value;
                slider.maxValue = 50;
            }
            else
            {
                nameText.text = variableClassToRead.variableName + " : " + variableClassToRead.value.ToString("F3");
                slider.maxValue = 1;
            }
             
            slider.value = variableClassToRead.value;
            if (variableClassToRead.moreIsBad)
            {
                sliderFill.color = new Color((1f* slider.value), (1f* (1f- slider.value)), 0f);
            }
            else
            {
                sliderFill.color = new Color((1f * (1f - slider.value)), (1f * slider.value), 0f);
            }
        }
    }

}
