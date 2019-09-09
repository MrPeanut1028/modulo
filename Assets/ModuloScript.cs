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
    public string TwitchHelpMessage = "Use !{0} submit <number> to submit the specified number!";
    IEnumerator ProcessTwitchCommand(string command)
    {
        string commt = command.ToUpper();
        string commfinal = commt.Replace("SUBMIT ", "");
        int tried;
        if(int.TryParse(commfinal, out tried)){
            tried = int.Parse(commfinal);
            yield return null;
            clearButton.OnInteract();
            for(int i=0;i<commfinal.Length;i++){
                int btntopress = int.Parse(commfinal[i].ToString())-1;
                if(btntopress==-1){btntopress=9;}
                keypadButton[btntopress].OnInteract();
            }
            submitButton.OnInteract();
            yield break;
        }
        else{
            yield return null;
			yield return "sendtochaterror Digit not valid.";
			yield break;
        }
    }
}
