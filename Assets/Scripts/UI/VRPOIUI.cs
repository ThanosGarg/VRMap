using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRPOIUI : VRPlayerUI
{
    public POIButton poiButtonPrefab;
    public Transform poiArea;
    public List<POIButton> poiButtons = new List<POIButton>();
    public POIButton selectedButton;

    [Header("POI Selection UI")]
    public Color selectedColor = Color.green;
    public Color defaultColor = Color.white;

    [Header("Selection Input Settings")]
    public InputAction MoveUpAction;
    public InputAction MoveDownAction;

    public InputAction SelectAction;

    private int selectedButtonIndex = 0;

    void Start()
    {
        SpawnPOIButtons();
    }
    void OnEnable()
    {
        MoveUpAction.Enable();
        MoveUpAction.started += MoveUp;
        MoveDownAction.Enable();
        MoveDownAction.started += MoveDown;
        SelectAction.Enable();
        SelectAction.started += Select;
    }

    void OnDisable()
    {
        MoveUpAction.Disable();
        MoveUpAction.started -= MoveUp;
        MoveDownAction.Disable();
        MoveDownAction.started -= MoveDown;
        SelectAction.Disable();
        SelectAction.started -= Select;
    }
    private void SpawnPOIButtons()
    {
        foreach (POIData data in AppManager.instance.tourManager.poiDatabase.poiList)
        {
            POIButton button = Instantiate(poiButtonPrefab, poiArea);
            button.Init(data, AppManager.instance.tourManager);
            poiButtons.Add(button);
        }

        SelectButton(poiButtons[selectedButtonIndex]);
    }

    private void MoveUp(InputAction.CallbackContext context)
    {
        selectedButtonIndex++;
        if (selectedButtonIndex >= poiButtons.Count)
        {
            selectedButtonIndex = 0;
        }
        if (poiButtons[selectedButtonIndex] != null)
        {
            SelectButton(poiButtons[selectedButtonIndex]);
        }
    }
    private void MoveDown(InputAction.CallbackContext context)
    {
        selectedButtonIndex--;
        if (selectedButtonIndex < 0)
        {
            selectedButtonIndex = poiButtons.Count - 1;
        }
        if (poiButtons[selectedButtonIndex] != null)
        {
            SelectButton(poiButtons[selectedButtonIndex]);
        }
    }

    private void Select(InputAction.CallbackContext context)
    {
        if (selectedButton != null)
        {
            selectedButton.OnButtonClicked();
        }
    }
    private void SelectButton(POIButton button)
    {
        if (button != null)
        {
            selectedButton = button;

            foreach (POIButton btn in poiButtons)
            {
                if (btn == selectedButton)
                {
                    btn.GetComponent<UnityEngine.UI.Image>().color = selectedColor;
                }
                else
                {
                    btn.GetComponent<UnityEngine.UI.Image>().color = defaultColor;
                }
            }
        }
    }
}
