using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class DisplayObjectMapping
{
    public double minRange;
    public double maxRange;
    public GameObject displayObject;
}

public class Manager : MonoBehaviour
{
    public VerticalLayoutGroup buttonGroup;
    public HorizontalLayoutGroup bottomRow;
    public RectTransform canvasRect;
    CalcButton[] bottomButtons;

    public Text digitLabel;
    public Text operatorLabel;
    bool errorDisplayed;
    bool displayValid;
    bool specialAction;
    double currentVal;
    double storedVal;
    double result;
    char storedOperator;

    bool canvasChanged;

    public List<DisplayObjectMapping> displayObjectMappings;  // displayObjectと範囲の対応をリストで管理

    private void Awake()
    {
        bottomButtons = bottomRow.GetComponentsInChildren<CalcButton>();
    }

    // Use this for initialization
    void Start()
    {
        bottomRow.childControlWidth = false;
        canvasChanged = true;
        buttonTapped('c');
    }

    // Update is called once per frame
    void Update()
    {
        if (canvasChanged)
        {
            canvasChanged = false;
            adjustButtons();
        }
    }

    private void OnRectTransformDimensionsChange()
    {
        canvasChanged = true;
    }

    void adjustButtons()
    {
        if (bottomButtons == null || bottomButtons.Length == 0)
            return;
        float buttonSize = canvasRect.sizeDelta.x / 4;
        float bWidth = buttonSize - bottomRow.spacing;
        for (int i = 1; i < bottomButtons.Length; i++)
        {
            bottomButtons[i].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                                                                     bWidth);
        }
        bottomButtons[0].rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                                                                 bWidth * 2 + bottomRow.spacing);
    }

    void clearCalc()
    {
        if (digitLabel != null)
        {
            digitLabel.text = "0";
        }
        else
        {
            Debug.LogError("digitLabel is null!");
        }

        operatorLabel.text = "";
        specialAction = displayValid = errorDisplayed = false;
        currentVal = result = storedVal = 0;
        storedOperator = ' ';

        // もし表示が何かされていたら、消す。みたいなことをかく。
        foreach (var mapping in displayObjectMappings)
        {
            mapping.displayObject.SetActive(false);
        }
    }

    void updateDigitLabel()
    {
        if (!errorDisplayed)
            digitLabel.text = currentVal.ToString();
        displayValid = false;
    }

    void calcResult(char activeOp)
    {
        switch (activeOp)
        {
            case '=':
                result = currentVal;
                break;
            case '+':
                result = storedVal + currentVal;
                break;
            case '-':
                result = storedVal - currentVal;
                break;
            case 'x':
                result = storedVal * currentVal;
                break;
            case '÷':
                if (currentVal != 0)
                {
                    result = storedVal / currentVal;
                }
                else
                {
                    errorDisplayed = true;
                    digitLabel.text = "ERROR";
                }
                break;
            default:
                Debug.Log("unknown: " + activeOp);
                break;
        }

        currentVal = result;
        updateDigitLabel();

        // 範囲ごとに対応するdisplayObjectを表示
        foreach (var mapping in displayObjectMappings)
        {
            if (result >= mapping.minRange && result <= mapping.maxRange)
            {
                mapping.displayObject.SetActive(true);
            }
            else
            {
                mapping.displayObject.SetActive(false);
            }
        }
    }

    public void buttonTapped(char caption)
    {
        if (caption == '.' && digitLabel.text.Contains("."))
            return;

        if (errorDisplayed)
            clearCalc();

        if ((caption >= '0' && caption <= '9') || caption == '.')
        {
            if (digitLabel.text.Length < 15 || !displayValid)
            {
                if (!displayValid)
                    digitLabel.text = (caption == '.' ? "0" : "");
                else if (digitLabel.text == "0" && caption != '.')
                    digitLabel.text = "";
                digitLabel.text += caption;
                displayValid = true;
            }
        }
        else if (caption == 'c')
        {
            clearCalc();
        }
        else if (caption == '±')
        {
            currentVal = -double.Parse(digitLabel.text);
            updateDigitLabel();
            specialAction = true;
        }
        else if (caption == '%')
        {
            currentVal = double.Parse(digitLabel.text) / 100.0d;
            updateDigitLabel();
            specialAction = true;
        }
        else if (displayValid || storedOperator == '=' || specialAction)
        {
            currentVal = double.Parse(digitLabel.text);
            displayValid = false;
            if (storedOperator != ' ')
            {
                calcResult(storedOperator);
                storedOperator = ' ';
            }
            operatorLabel.text = caption.ToString();
            storedOperator = caption;
            storedVal = currentVal;
            updateDigitLabel();
            specialAction = false;
        }
    }
}
