using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class TerminalManager : MonoBehaviour
{
    public GameObject directoryLine;
    public GameObject responseLine;

    public InputField terminalInput;
    public GameObject userInputLine;
    public ScrollRect sr;
    public GameObject msgList;

    Interpreter interpreter;

    [Header("Startup Responses")]
    string welcome;
    string welcomeHelp;

    private void Start()
    {
        //DontDestroyOnLoad(this);
        interpreter = GetComponent<Interpreter>();
        terminalInput.ActivateInputField();
        //Cursor.lockState = CursorLockMode.Locked;
        StartupResponses();
    }

    public void StartupResponses()
    {
        #region Responses
        welcome = "welcome";
        welcomeHelp = "welcomehelp";
        #endregion

        ScrollToBottom(AddInterpreterLines(interpreter.Interpret(welcome))); userInputLine.transform.SetAsLastSibling();
        ScrollToBottom(AddInterpreterLines(interpreter.Interpret(welcomeHelp))); userInputLine.transform.SetAsLastSibling();
    }

    public void DisconnectedToLobby()
    {
        ScrollToBottom(AddInterpreterLines(interpreter.Interpret("disconnected"))); userInputLine.transform.SetAsLastSibling();
    }

    public void Test()
    {
        ScrollToBottom(AddInterpreterLines(interpreter.Interpret("test"))); userInputLine.transform.SetAsLastSibling();
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
        msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(listSize.x, 107.5935f);
    }

    private void OnGUI()
    {
        //As long as the text isn't blank, it will print whatever we write after pressing ENTER.
        if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return) && interpreter.conn == false)
        {
            string userInput = terminalInput.text;                                      //Stores what the user typed.

            ClearInputField();                                                          //Clears the input field.

            AddDirectoryLine(userInput);                                                //Instantiates a gameobject with a directory prefix.

            int lines = AddInterpreterLines(interpreter.Interpret(userInput));          //Adds the interpretation lines.

            ScrollToBottom(lines);                                                      //Scroll to the bottom of the ScrollRect.

            userInputLine.transform.SetAsLastSibling();                                 //Moves the user input to the end.

            terminalInput.ActivateInputField();                                         //Refocus the input field.
            terminalInput.Select();
        }
        else if (terminalInput.isFocused && terminalInput.text != "" && Input.GetKeyDown(KeyCode.Return) && interpreter.conn == true)
        {
            string userInput = terminalInput.text;

            ClearInputField();

            AddDirectoryLine(userInput);

            int lines = AddInterpreterLines(interpreter.ServerCreationWizard(userInput));

            ScrollToBottom(lines);

            userInputLine.transform.SetAsFirstSibling();

            terminalInput.ActivateInputField();
            terminalInput.Select();
        }
    }

    private void ClearInputField()
    {
        terminalInput.text = "";
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
        for(int i = 0; i < interpretation.Count; i++)
        {
            GameObject res = Instantiate(responseLine, msgList.transform);                                  //Instantiates the response line.

            res.transform.SetAsLastSibling();                                                               //Sets it to the end of all the messages.

            Vector2 listSize = msgList.GetComponent<RectTransform>().sizeDelta;                             //Gets the size of the message list and resizes.
            msgList.GetComponent<RectTransform>().sizeDelta = new Vector2(listSize.x, listSize.y + 35f);

            res.GetComponentInChildren<Text>().text = interpretation[i];                                    //Sets the text of this response line to be whatever the interpreter string is.
        }

        return interpretation.Count;
    }

    private void ScrollToBottom(int lines)
    {
        if(lines > 4)
        {
            sr.velocity = new Vector2(0, 450);
        }
        else
        {
            sr.verticalNormalizedPosition = 0;
        }
    }
}
