using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// VRPOIUI manages the UI for selecting Points of Interest (POIs) in VR.
/// Handles button instantiation, navigation, and selection using input actions.
/// Inherits from VRPlayerUI for hand-following and UI toggling.
/// </summary>
public class VRPOIUI : VRPlayerUI
{
    /// <summary>
    /// Prefab for POI selection buttons.
    /// </summary>
    public POIButton poiButtonPrefab;

    /// <summary>
    /// Parent transform where POI buttons will be instantiated.
    /// </summary>
    public Transform poiArea;

    /// <summary>
    /// List of all instantiated POI buttons.
    /// </summary>
    public List<POIButton> poiButtons = new List<POIButton>();

    /// <summary>
    /// Currently selected POI button.
    /// </summary>
    public POIButton selectedButton;

    [Header("POI Selection UI")]
    /// <summary>
    /// Color for the selected button.
    /// </summary>
    public Color selectedColor = Color.green;

    /// <summary>
    /// Color for unselected buttons.
    /// </summary>
    public Color defaultColor = Color.white;

    [Header("Selection Input Settings")]
    /// <summary>
    /// Input action for moving selection up.
    /// </summary>
    public InputAction MoveUpAction;

    /// <summary>
    /// Input action for moving selection down.
    /// </summary>
    public InputAction MoveDownAction;

    /// <summary>
    /// Input action for selecting the current POI.
    /// </summary>
    public InputAction SelectAction;

    /// <summary>
    /// Index of the currently selected button in the list.
    /// </summary>
    private int selectedButtonIndex = 0;

    /// <summary>
    /// Called on script start. Spawns POI buttons.
    /// </summary>
    void Start()
    {
        SpawnPOIButtons();
    }

    /// <summary>
    /// Enables input actions and subscribes to their events.
    /// </summary>
    void OnEnable()
    {
        MoveUpAction.Enable();
        MoveUpAction.started += MoveUp;
        MoveDownAction.Enable();
        MoveDownAction.started += MoveDown;
        SelectAction.Enable();
        SelectAction.started += Select;
    }

    /// <summary>
    /// Disables input actions and unsubscribes from their events.
    /// </summary>
    void OnDisable()
    {
        MoveUpAction.Disable();
        MoveUpAction.started -= MoveUp;
        MoveDownAction.Disable();
        MoveDownAction.started -= MoveDown;
        SelectAction.Disable();
        SelectAction.started -= Select;
    }

    /// <summary>
    /// Instantiates POI buttons for each POI in the database and selects the first one.
    /// </summary>
    private void SpawnPOIButtons()
    {
        // Instantiate a button for each POI in the database
        foreach (POIData data in AppManager.instance.tourManager.poiDatabase.poiList)
        {
            POIButton button = Instantiate(poiButtonPrefab, poiArea);
            button.Init(data, AppManager.instance.tourManager);
            poiButtons.Add(button);
        }

        // Select the first button by default
        if (poiButtons.Count > 0)
        {
            SelectButton(poiButtons[selectedButtonIndex]);
        }
    }

    /// <summary>
    /// Moves the selection up in the button list. Wraps to the start if at the end.
    /// </summary>
    /// <param name="context">Input action context</param>
    private void MoveUp(InputAction.CallbackContext context)
    {
        selectedButtonIndex++;
        if (selectedButtonIndex >= poiButtons.Count)
        {
            selectedButtonIndex = 0; // Wrap to first
        }
        if (poiButtons[selectedButtonIndex] != null)
        {
            SelectButton(poiButtons[selectedButtonIndex]);
        }
    }

    /// <summary>
    /// Moves the selection down in the button list. Wraps to the end if at the start.
    /// </summary>
    /// <param name="context">Input action context</param>
    private void MoveDown(InputAction.CallbackContext context)
    {
        selectedButtonIndex--;
        if (selectedButtonIndex < 0)
        {
            selectedButtonIndex = poiButtons.Count - 1; // Wrap to last
        }
        if (poiButtons[selectedButtonIndex] != null)
        {
            SelectButton(poiButtons[selectedButtonIndex]);
        }
    }

    /// <summary>
    /// Invokes the click event on the currently selected button.
    /// </summary>
    /// <param name="context">Input action context</param>
    private void Select(InputAction.CallbackContext context)
    {
        if (selectedButton != null)
        {
            selectedButton.OnButtonClicked();
        }
    }

    /// <summary>
    /// Sets the given button as selected and updates button colors.
    /// </summary>
    /// <param name="button">Button to select</param>
    private void SelectButton(POIButton button)
    {
        if (button != null)
        {
            selectedButton = button;

            // Update colors for all buttons
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
