using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class ColoursScript : MonoBehaviour {
    [SerializeField] List<GameObject> buttons; // The buttons in the scene
    [SerializeField] Transform door; // The door in the scene
    [SerializeField] GameObject player; // The player
    [SerializeField] Light indicator; // The light that tells the player if they did it right or not
    [SerializeField] Color correctColor;

    [HideInInspector] public Dictionary<GameObject, bool> pressed; // Each bool represents if a button has been pressed or not
    private Prototype prototypeScript; // Script inside of the player
    private GameObject possibleButton; // The button the player is standing on (is null if player isn't standing on a button)
    private int currentButton = 0; // The button that should currently be pressed
    private bool movingButton = false; // If there is a moving button or not
    private GameObject pressedButton; // Stores the pressed button for if the player steps off before the animation finishes
    
    void Start() {
        pressed = new Dictionary<GameObject, bool>();

        foreach (GameObject button in buttons) {
            pressed.Add(button, false);
        }

        prototypeScript = player.GetComponent<Prototype>();
    }

    void Update() {
        // Player is playing colours minigame
        if (prototypeScript.colourMinigame) {
            possibleButton = prototypeScript.possibleButton;

            // Player is standing on a button
            if (possibleButton != null && !movingButton) { 
                // If button hasn't already been pressed
                if (!pressed[possibleButton]) {
                    // Move button down and set pressed to true
                    pressedButton = possibleButton;
                    StartCoroutine(MoveButton(possibleButton, true));
                    pressed[possibleButton] = true;
                    Debug.Log("Pressed");
                }
            }
        }
    }

    IEnumerator MoveButton(GameObject button, bool negative) {
        movingButton = true;

        // Slowly moves button down
        if (negative) {
            while (button.transform.position.y > -0.5f) { 
                button.transform.Translate(0, -Time.deltaTime, 0);
                yield return null;
            }
        // Slowly moves button up
        } else {
            while (button.transform.position.y < 0f) {
                button.transform.Translate(0, Time.deltaTime, 0);
                yield return null;
            }
        }

        // Puts button at the right position
        Vector3 position = button.transform.position;
        position.y = negative ? -0.5f : 0;
        button.transform.position = position;

        // Checks if the buttons have been pressed in the right order
        if (negative) CheckButtonStates();

        // Resets press state and lets player move buttons again
        movingButton = false;
        if (!negative) {
            pressed[button] = false;
        }
    }

    // Move the door up
    IEnumerator MoveDoor() {
        // Change the light's color
        indicator.color = correctColor;

        // Move the door
        while (door.position.y < 8.5f) {
            door.transform.Translate(0, Time.deltaTime, 0);
            yield return null;
        }

        // Put the door at the right place
        Vector3 position = door.position;
        position.y = 8.5f;
        door.position = position;
    }

    void CheckButtonStates() {
        // If it's been pressed in the wrong order
        if (pressedButton != buttons[currentButton]) {
            // Move all buttons back up
            foreach (GameObject item in buttons) {
                StartCoroutine(MoveButton(item, false));
            }

            // Reset current button 
            currentButton = 0;

            // Knock player back
            prototypeScript.KnockBack();
            return;
        }

        currentButton++;

        // Checks if player has won the minigame
        foreach (var button in pressed) {
            if (!button.Value) {
                return;
            }
        }

        StartCoroutine(MoveDoor());
    }
}
