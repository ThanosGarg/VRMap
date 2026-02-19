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
    }

    void OnDisable()
    {
        MoveUpAction.Disable();
        MoveUpAction.started -= MoveUp;
        MoveDownAction.Disable();
        MoveDownAction.started -= MoveDown;
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

    // private void ChangeSelectedPOI(InputAction.CallbackContext context)
    // {
    //     Debug.Log(context.ReadValue<Vector2>());
    //     Vector2 input = context.ReadValue<Vector2>();

    //     selectedButtonIndex -= (int)input.y;

    //     if (selectedButtonIndex < 0)
    //     {
    //         selectedButtonIndex = poiButtons.Count - 1;
    //     }
    //     else if (selectedButtonIndex >= poiButtons.Count)
    //     {
    //         selectedButtonIndex = 0;
    //     }

    //     if (poiButtons[selectedButtonIndex] != null)
    //     {
    //         SelectButton(poiButtons[selectedButtonIndex]);
    //     }
    // }


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
