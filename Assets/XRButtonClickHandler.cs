using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;

public class XRButtonClickHandler : MonoBehaviour
{
    public Image buttonImage; // Reference to the Image component
    public Color normalColor = Color.white; // Default color (no interaction)
    public Color hoverColor = Color.yellow; // Color when hovering
    public Color clickedColor = Color.green; // Color when clicked

    private void Start()
    {
        if (buttonImage != null)
        {
            SetButtonColor(normalColor); // Set the initial color to normal
        }
        else
        {
            Debug.LogError("Button Image not assigned!");
        }
    }

    // Called when the button is clicked via XR interaction
    public void OnSendOrderButtonClick()
    {
        Debug.Log("Button clicked via XR Interaction!");
        SetButtonColor(clickedColor); // Change color to clicked color
    }

    // Called when the button is hovered over
    public void OnHoverEnter()
    {
        Debug.Log("Hover started!");
        SetButtonColor(hoverColor); // Change color to hover color
    }

    // Called when the hover is exited
    public void OnHoverExit()
    {
        Debug.Log("Hover ended!");
        SetButtonColor(normalColor); // Revert back to normal color when hover ends
    }

    // Helper function to set the button's color
    private void SetButtonColor(Color color)
    {
        if (buttonImage != null)
        {
            buttonImage.color = color; // Change the color of the Image component
            Debug.Log("Button color changed to: " + color); // Debug log
        }
    }
}
