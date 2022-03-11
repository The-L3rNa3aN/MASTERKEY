using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayTerminalManager : MonoBehaviour
{
    public GameObject directoryLine;
    public GameObject responseLine;

    public InputField terminalInput;
    public GameObject userInputLine;
    public ScrollRect sr;
    public GameObject msgList;

    [SerializeField] PlayInterpreter interpreter;

    [Header("LPM Stuff")]
    public bool enableTimer;
    public float timer;
    public float timeTaken;
    public float lpm;
    public int letterCount;
    public int totalLetterCount;
    List<float> lpmCollection = new List<float>();

    private void Start()
    {
        interpreter = GetComponent<PlayInterpreter>();
        terminalInput.ActivateInputField();                         //Activates the input field on start.
        //Cursor.lockState = CursorLockMode.Locked;
        OnConnectResponses();
    }

    private void Update()
    {
        if(enableTimer == true) { timer += Time.deltaTime; }        //Required for the LPM Counter to work.
    }

    public void OnConnectResponses()
    {

        if(interpreter.player.isClient)
        {
            ScrollToBottom(AddInterpreterLines(interpreter.Interpret("isClient"))); userInputLine.transform.SetAsLastSibling();
        }
        else if(interpreter.player.isServer)
        {
            ScrollToBottom(AddInterpreterLines(interpreter.Interpret("isHost"))); userInputLine.transform.SetAsLastSibling();
        }
    }

    #region Powerup Pick Up Responses
    public void AidePickup() { ScrollToBottom(AddInterpreterLines(interpreter.Interpret("aide"))); userInputLine.transform.SetAsLastSibling(); }

    public void VitalisPickup() { ScrollToBottom(AddInterpreterLines(interpreter.Interpret("vitalis"))); userInputLine.transform.SetAsLastSibling(); }

    public void CorruptusPickup() { ScrollToBottom(AddInterpreterLines(interpreter.Interpret("corruptus"))); userInputLine.transform.SetAsLastSibling(); }

    public void VaengrPickup() { ScrollToBottom(AddInterpreterLines(interpreter.Interpret("vaengr"))); userInputLine.transform.SetAsLastSibling(); }

    public void EscrenPickup() { ScrollToBottom(AddInterpreterLines(interpreter.Interpret("escren"))); userInputLine.transform.SetAsLastSibling(); }
    #endregion

    private void OnGUI()
    {
        LPMFunction();
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return))
        {
            string userInput = terminalInput.text;                  //Stores what the user typed.

            ClearInputField();                                      //Clears the input field.

            AddDirectoryLine(userInput);                            //Instantiates a gameobject with a directory prefix.

            //Adds the interpretation lines.
            int lines = AddInterpreterLines(interpreter.Interpret(userInput));

            //Scroll to the bottom of the ScrollRect.
            ScrollToBottom(lines);

            userInputLine.transform.SetAsLastSibling();             //Moves the user input to the end.

            //Refocus the input field.
            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
    }

    private void ClearInputField()
    {
        terminalInput.text = "";
    }

    public void Notifs(string notif)                                //Used for killfeeds and other notifications...
    {
        var uiObj = gameObject.GetComponent<PlayInterpreter>();
        ScrollToBottom(AddInterpreterLines(uiObj.ReceiveNetworkNotifs(notif)));
        userInputLine.transform.SetAsLastSibling();
    }

    private void AddDirectoryLine(string userInput)
    {
        //Resizing the command line container so that ScrollRect doesn't do anything funny.
        Vector2 msgListSize = msgList.GetComponent<RectTransform>().sizeDelta;
        msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(msgListSize.x, msgListSize.y + 35f);

        //Instantiates the directory line.
        GameObject msg = Instantiate(directoryLine, msgList.transform);

        //Sets the child index.
        msg.transform.SetSiblingIndex(msgList.transform.childCount - 1);

        //Sets the text of this new gameobject.
        msg.GetComponentsInChildren<Text>()[1].text = userInput;
    }

    private int AddInterpreterLines(List<string> interpretation)
    {
        for (int i = 0; i < interpretation.Count; i++)
        {
            //Instantiates the response line.
            GameObject res = Instantiate(responseLine, msgList.transform);

            //Sets it to the end of all the messages.
            res.transform.SetAsLastSibling();

            //Gets the size of the message list and resizes.
            Vector2 listSize = msgList.GetComponent<RectTransform>().sizeDelta;
            msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(listSize.x, listSize.y + 35f);

            //Sets the text of this response line to be whatever the interpreter string is.
            res.GetComponentInChildren<Text>().text = interpretation[i];
        }

        return interpretation.Count;
    }

    public void ClearScreen()                                              //Method for clearing the screen.
    {
        foreach (Transform child in msgList.transform)
        {
            if (child.name != "UserInputField")
            {
                Destroy(child.gameObject);
            }
        }

        //Resizes the screen when cleared to its initial dimensions. Not doing this results in a long-ass dimension which is a chore to scroll.
        Vector2 listSize = msgList.GetComponent<RectTransform>().sizeDelta;
        msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(listSize.x, -19.7641f);
    }

    private void ScrollToBottom(int lines)
    {
        if (lines > 4)
        {
            sr.velocity = new Vector2(0, 450);
        }
        else
        {
            sr.verticalNormalizedPosition = 0;
        }
    }

    #region Letters-Per-Minute Functions...
    private void LPMFunction()
    {
        if(terminalInput.text != "")
        {
            enableTimer = true;
            LetterCounter(terminalInput.text);
        }
        else
        {
            enableTimer = false;
            timer = 0f;
        }

        if(terminalInput.text != "" && Input.GetKey(KeyCode.Return))
        {
            enableTimer = false;
            timeTaken = timer;
            timer = 0f;
            lpm = (letterCount * 60) / timeTaken;
            lpmCollection.Add(lpm);
        }
    }

    private void LetterCounter(string sentence) => letterCount = sentence.Length;

    public float AverageLPM()
    {
        return lpmCollection.Average();
    }
    #endregion
}