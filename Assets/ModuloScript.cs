using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class ModuloScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] keypadButton;
    public KMSelectable submitButton;
    public KMSelectable clearButton;

    public TextMesh moduloText;
    public TextMesh numberText;
    public TextMesh inputText;

    private int modulo = 0;
    private int pickedNumber = 0;
    private int answer;
    private string answerString;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        foreach (KMSelectable button in keypadButton)
        {
            KMSelectable pressedButton = button;
            button.OnInteract += delegate () { KeypadPress(pressedButton); return false; };
        }
        submitButton.OnInteract += delegate () { PressSubmitButton(); return false; };
        clearButton.OnInteract += delegate () { PressClearButton(); return false; };
    }


    void Start()
    {
        modulo = UnityEngine.Random.Range(3,16);
        pickedNumber = UnityEngine.Random.Range(50,1000);
        moduloText.text = modulo.ToString();
        numberText.text = pickedNumber.ToString();
        inputText.text = "";
        answer = pickedNumber % modulo;
        answerString = answer.ToString();
        Debug.Log(answerString);
        Debug.LogFormat("[Modulo #{0}] Your number is {1}.", moduleId, pickedNumber);
        Debug.LogFormat("[Modulo #{0}] You must modulo {1}.", moduleId, modulo);
        Debug.LogFormat("[Modulo #{0}] The correct answer is {1}.", moduleId, answer);
    }

    public void PressSubmitButton()
    {
        if(moduleSolved)
        {
            return;
        }
        submitButton.AddInteractionPunch();
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        if(inputText.text == answerString)
        {
            Debug.LogFormat("[Modulo #{0}] You entered {1}. That is correct. Module disarmed.", moduleId, answer);
            GetComponent<KMBombModule>().HandlePass();
            moduleSolved = true;
        }
        else
        {
            Debug.LogFormat("[Modulo #{0}] Strike! You entered {1}. That is incorrect.", moduleId, inputText.text);
            GetComponent<KMBombModule>().HandleStrike();
            inputText.text = "";
            Start();
        }
    }
    public int GetDigit(int number, int nth){
        while(number >= 10*nth)
        number /= 10;

    return number % 10;
    }
    public void PressClearButton()
    {
        if(moduleSolved)
        {
            return;
        }
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        clearButton.AddInteractionPunch();
        inputText.text = "";
    }

    void KeypadPress(KMSelectable button)
    {
        if(moduleSolved)
        {
            return;
        }
        GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
        button.AddInteractionPunch();
        string pressedNumber = button.GetComponentInChildren<TextMesh>().text;
        if(inputText.text.Length < 3)
        {
            inputText.text += pressedNumber;
        }
    }
    public string TwitchHelpMessage = "Use '!{0} 123' to press the buttons  1, 2, 3. To submit your answer use '!{0} submit'! And use '!{0} clear' to clear your answer!";
    IEnumerator ProcessTwitchCommand(string command)
    {
        if (command.Equals("submit", StringComparison.InvariantCultureIgnoreCase) )
    {
        yield return submitButton;
    }
           
        else{ if (command.Equals("clear", StringComparison.InvariantCultureIgnoreCase) )
    {
        yield return clearButton;
    }
        else{
        int tried;
        if (int.TryParse(command, out tried)){
            tried=int.Parse(command);
            for(int i = 1; i<=tried.ToString().Length; i++){
                int final = (GetDigit(tried, i))-1;
                if(final == -1){
                    final = 9;
                }
                yield return keypadButton[final];
            }
        }
        }
        }
    }
}
